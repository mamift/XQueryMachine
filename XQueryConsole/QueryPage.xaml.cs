﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WmHelp.XmlGrid;
using DataEngine.XQuery;
using System.ComponentModel;
using ICSharpCode.AvalonEdit;
using System.IO;
using System.Xml;
using Microsoft.Win32;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for QueryPage.xaml
    /// </summary>
    public partial class QueryPage : Page, INotifyPropertyChanged
    {
        private string fileName;
        private string filePath;
        private XmlGridView xmlGrid;
        private Task queryTask;
        private XQueryCommand command;
        private Stopwatch workTime;
        private String status;
        private bool hasTextContent;
        private bool hasModified;

        private GridLength height1;
        private GridLength height2;
        
        private BracketHighlightRenderer bracketRenderer;
        private BracketSearcher bracketSearcher;

        public static readonly ICommand ExecuteCommand =
            new RoutedUICommand("Execute", "Execute", typeof(QueryPage));
        public static readonly ICommand SaveResultCommand =
            new RoutedUICommand("Save Result", "SaveResult", typeof(QueryPage));

        private class EmbeddedGrid : XmlGridView
        {
            public EmbeddedGrid()
                : base()
            {
                SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, false);
            }
        }

        public QueryPage()
        {
            InitializeComponent();

            height1 = layout.RowDefinitions[3].Height;
            height2 = layout.RowDefinitions[4].Height;
            
            filePath = String.Empty;
            fileName = String.Empty;
            HasContent = false;
            ShowResultPane = false;
            hasModified = false;
            
            xmlGrid = new EmbeddedGrid();
            xmlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            xmlGrid.Location = new System.Drawing.Point(0, 100);
            xmlGrid.Name = "xmlGridView1";
            xmlGrid.Size = new System.Drawing.Size(100, 100);
            xmlGrid.TabIndex = 0;
            xmlGrid.AutoHeightCells = true;            
            windowsFormsHost.Child = xmlGrid;

            // see http://community.sharpdevelop.net/forums/t/10312.aspx          
            bracketSearcher = new BracketSearcher();
            bracketRenderer = new BracketHighlightRenderer(textEditor.TextArea.TextView);
            textEditor.TextArea.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            BracketSearchResult bracketSearchResult = bracketSearcher
                .SearchBracket(textEditor.Document, textEditor.TextArea.Caret.Offset);
            bracketRenderer.SetHighlight(bracketSearchResult);
        }

        public void Load()
        {
            HasContent = true;
            textEditor.Load(FileName);
            Update(false);
        }

        public void Save()
        {
            textEditor.Save(FileName);
            Update(false);
        }

        public bool HasContent
        {
            get; private set;
        }

        public bool Modified
        {
            get
            {
                return hasModified;
            }
        }

        public string FileName 
        {
            get
            {
                return System.IO.Path.Combine(filePath, fileName);
            }
            set
            {
                fileName = System.IO.Path.GetFileName(value);
                filePath = System.IO.Path.GetDirectoryName(value);
                Title = fileName;
            }
        }

        public string ShortFileName
        {
            get
            {
                return fileName;
            }
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        public bool ShowResultPane
        {
            get
            {
                return splitter.Visibility == Visibility.Visible;
            }
            set
            {
                if (value != (splitter.Visibility == Visibility.Visible))
                {
                    if (value)
                    {
                        layout.RowDefinitions[3].Height = height1;
                        layout.RowDefinitions[4].Height = height2;
                        splitter.Visibility = Visibility.Visible;                        
                    }
                    else
                    {
                        height1 = layout.RowDefinitions[3].Height;
                        height2 = layout.RowDefinitions[4].Height;
                        layout.RowDefinitions[3].Height = new GridLength(0);
                        layout.RowDefinitions[4].Height = new GridLength(0);
                        splitter.Visibility = Visibility.Hidden;
                    }
                    OnPropertyChanged("ShowResultPane");
                }
            }
        }

        public string StatusText
        {
            get
            {
                return status;
            }
        }

        private void Update(bool modified)
        {
            if (hasModified != modified)
            {
                if (modified)
                    Title = fileName + "*";
                else
                    Title = fileName;
                hasModified = modified;
            }
        }

        public void Close()
        {
            windowsFormsHost.Dispose();
            if (command != null)
                command.Dispose();            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement overflowGrid = gridToolBar.Template.FindName("OverflowGrid", gridToolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;
            ScrollViewer sv = Extensions.FindVisualChild<ScrollViewer>(sourceViewer);
            if (sv != null)
            {
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            textEditor.Focus();
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            HasContent = true;
            Update(true);
        }

        private void textEditor_PreviewDrop(object sender, DragEventArgs e)
        {
            textEditor.Focus();
        }

        private void gridViewButton_Click(object sender, RoutedEventArgs e)
        {
            gridViewBorder.Visibility = System.Windows.Visibility.Visible;
            sourceViewBorder.Visibility = System.Windows.Visibility.Hidden;
        }

        private void xmlSourceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasTextContent)
                CreateTextContent();
            gridViewBorder.Visibility = System.Windows.Visibility.Hidden;
            sourceViewBorder.Visibility = System.Windows.Visibility.Visible;
        }

        delegate void UpdateXmlGridDelegate(GridCellGroup rootCell);
        delegate void RuntimeExceptionDelegate(Exception ex);

        private void CommandBinding_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            DocumentController controller = mainWindow.Controller;
            if (command != null)
                command.Dispose();
            xmlGrid.Cell = null;
            gridViewButton.IsChecked = true;
            gridViewBorder.Visibility = System.Windows.Visibility.Visible;
            sourceViewBorder.Visibility = System.Windows.Visibility.Hidden;
            sourceViewer.Clear();
            hasTextContent = false;
            command = new XQueryCommand(new XQueryDsContext(mainWindow.TreeController2));
            command.CommandText = textEditor.Text;
            command.SearchPath = controller.SearchPath;
            if (filePath != String.Empty)
                command.BaseUri = filePath;
            textEditor.IsReadOnly = true;
            textEditor.Cursor = Cursors.Wait;
            textEditor.ForceCursor = true;
            workTime = new Stopwatch();     
            workTime.Start();
            queryTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        XQueryNodeIterator res = command.Execute();
                        XPathGridBuilder builder = new XPathGridBuilder();
                        GridCellGroup rootCell = new GridCellGroup();
                        builder.ParseNodes(rootCell, res.ToList());
                        Dispatcher.BeginInvoke(new UpdateXmlGridDelegate(UpdateXmlGrid), rootCell);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(new RuntimeExceptionDelegate(HandleException), ex);
                    }
                });
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = queryTask == null;
        }

        private void HandleException(Exception ex)
        {
            workTime.Stop();
            queryTask = null;
            textEditor.IsReadOnly = false;
            textEditor.Cursor = null;
            textEditor.ForceCursor = false;
            if (ex is XQueryException)
                MessageBox.Show(ex.Message /*+ "\r\n" + ex.StackTrace*/, "Runtime error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            else
                if ((ex is OperationCanceledException))
                {
                    status = "Cancelled";
                    OnPropertyChanged("StatusText");
                }
                else                
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Runtime error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            textEditor.Focus();
            command.Dispose();
            command = null;
        }

        private void UpdateXmlGrid(GridCellGroup rootCell)
        {
            workTime.Stop();
            queryTask = null;
            textEditor.IsReadOnly = false;
            textEditor.Cursor = null;
            textEditor.ForceCursor = false;
            textEditor.Focus();
            String elapsed;
            if (workTime.Elapsed.Hours > 0)
                elapsed = String.Format("{0} hr, {1} min, {2} sec",
                    workTime.Elapsed.Hours, workTime.Elapsed.Minutes, workTime.Elapsed.Seconds);
            else if (workTime.Elapsed.Minutes > 0)
                elapsed = String.Format("{0} min, {1} sec",
                    workTime.Elapsed.Minutes, workTime.Elapsed.Seconds);
            else
                elapsed = String.Format("{0} sec ({1} ms)",
                    workTime.Elapsed.Seconds, workTime.ElapsedMilliseconds);
            if (xmlGrid.ShowColumnHeader)
                status = String.Format("{0} row(s) read. {1} elapsed.",
                   rootCell.Table.Height - 1, elapsed);
            else
                status = String.Format("{0} elapsed.", elapsed);
            OnPropertyChanged("StatusText");
            xmlGrid.Cell = rootCell;
            ShowResultPane = true;
        }

        private void CreateTextContent()
        {
            if (xmlGrid.Cell != null)
            {
                Cursor = Cursors.Wait;
                StringBuilder sb = new StringBuilder();
                try
                {
                    GridCellGroup root = xmlGrid.Cell;
                    for (int s = 0; s < root.Table.Height; s++)
                    {
                        if (s > 0)
                            sb.AppendLine();
                        GridCell cell = root.Table[0, s];
                        if (cell is XPathGroupCell)
                        {
                            XPathGroupCell groupCell = (XPathGroupCell)cell;
                            sb.Append(groupCell.Navigator.OuterXml);
                        }
                        else
                            sb.Append(cell.Text);
                    }
                    sourceViewer.Document.Text = sb.ToString();
                    hasTextContent = true;
                }
                finally
                {
                    Cursor = null;
                }
            }
        }

        private void SaveResults(XmlWriter writer)
        {
            Cursor = Cursors.Wait;            
            try
            {
                GridCellGroup root = xmlGrid.Cell;
                for (int s = 0; s < root.Table.Height; s++)
                {
                    if (s > 0)
                        writer.WriteWhitespace("\n");
                    GridCell cell = root.Table[0, s];
                    if (cell is XPathGroupCell)
                    {
                        XPathGroupCell groupCell = (XPathGroupCell)cell;
                        groupCell.Navigator.WriteSubtree(writer);
                    }
                    else
                        writer.WriteString(cell.Text);
                }
            }
            finally
            {
                Cursor = null;
            }
        }

        private void CommandBinding_SaveResExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML files|*.xml|Text documents|*.txt";
            if (dlg.ShowDialog() == true)
            {
                FileStream stream = new FileStream(dlg.FileName, FileMode.Create);
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Auto
                };
                XmlWriter writer = XmlWriter.Create(stream, settings);
                SaveResults(writer);
                writer.Close();
            }
        }

        private void CommandBinding_SaveResCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = xmlGrid != null && xmlGrid.Cell != null;
        }

        private void CommandBinding_StopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (command != null && queryTask != null)
                command.Terminate();
        }

        private void CommandBinding_CanStopExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = queryTask != null;
        }
    }
}
