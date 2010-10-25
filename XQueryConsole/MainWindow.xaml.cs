﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;
using System.Reflection;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DocumentController Controller { get; private set; }
        public FileTreeController TreeController1 { get; private set; }
        public DataSourceTreeController TreeController2 { get; private set; }
        public FileTreeController TreeController3 { get; private set; }

        public static readonly ICommand CloneQueryCommand =
            new RoutedUICommand("Clone Query", "CloneQuery", typeof(MainWindow));
        public static readonly ICommand ShowResultsCommand =
            new RoutedUICommand("Show Results Panel", "ShowResults", typeof(MainWindow));        


        public QueryPage SelectedPage
        {
            get
            {
                Frame frame = (Frame)QueryTabs.SelectedContent;
                if (frame != null)
                    return (QueryPage)frame.Content;
                return null;
            }
        }

        public bool IsShowedResultPanel
        {
            get
            {
                QueryPage selectedPage = SelectedPage;
                if (selectedPage != null && selectedPage.ShowResultPane)
                    return true;
                return false;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Controller = new DocumentController();
            Controller.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Controller_PropertyChanged);
            TreeController1 = new FileTreeController(Controller.MyQueriesPath);
            TreeController1.MonitorFolders = true;
            TreeController1.FileOpen += new EventHandler(TreeController1_FileOpen);
            treeViewHost1.Child = TreeController1.TreeControl;
            TreeController2 = new DataSourceTreeController(Controller);
            TreeController2.RightClick += new EventHandler(TreeController2_RightClick);
            TreeController2.ShowTooltip += new EventHandler(TreeController2_ShowTooltip);
            treeViewHost2.Child = TreeController2.TreeControl;
            TreeController3 = new FileTreeController(GetSamplesDirectory());
            treeViewHost3.Child = TreeController3.TreeControl;
            TreeController3.FileOpen += new EventHandler(TreeController3_FileOpen);
            INotifyCollectionChanged notify = (INotifyCollectionChanged)QueryTabs.Items; 
            notify.CollectionChanged += new NotifyCollectionChangedEventHandler(Notify_TabItemsCollectionChanged);
            ApplicationCommands.New.Execute(null, this);
        }

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MyQueriesPath")
                TreeController1.ChangeBasePath(Controller.MyQueriesPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TreeController2_RightClick(object sender, EventArgs e)
        {
            treeViewHost2.ContextMenu.IsOpen = true;
        }

        private void TreeController2_ShowTooltip(object sender, EventArgs e)
        {
            ToolTip toolTip = treeViewHost2.ToolTip as ToolTip;
            if (toolTip == null)
            {
                toolTip = new ToolTip();
                toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
                toolTip.PlacementTarget = SelectedPage.textEditor;
                toolTip.Content = treeViewHost2.ToolTip;
                treeViewHost2.ToolTip = toolTip;
            }
            if (toolTip != null)
                toolTip.IsOpen = true;
            DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 5), IsEnabled = true };
            timer.Tick += new EventHandler(delegate(object timerSender, EventArgs timerArgs)
            {
                if (toolTip != null)
                    toolTip.IsOpen = false;
                toolTip = null;
                timer = null;
            });
        }

        private string GetSamplesDirectory()
        {
            String path = System.IO.Path
                .GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return System.IO.Path.Combine(path, "Samples");
        }

        private void TreeController1_FileOpen(object sender, EventArgs e)
        {
            if (TreeController1.SelectedFileName != null)
            {
                Controller.OpenQuery(QueryTabs, TreeController1.SelectedFileName);
                if (SelectedPage != null)
                    SelectedPage.textEditor.Focus();
            }
        }

        private void TreeController3_FileOpen(object sender, EventArgs e)
        {
            if (TreeController3.SelectedFileName != null)
            {
                Controller.OpenQuery(QueryTabs, TreeController3.SelectedFileName);
                if (SelectedPage != null)
                    SelectedPage.textEditor.Focus();
            }
        }

        private void Notify_TabItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (QueryTabs.Items.Count == 0)
            {
                menuEdit.Visibility = System.Windows.Visibility.Collapsed;
                menuQuery.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                menuEdit.Visibility = System.Windows.Visibility.Visible;
                menuQuery.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommandBinding_NewQueryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Controller.NewQuery(QueryTabs);
        }

        private void CommandBinding_CloseQueryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (QueryTabs.SelectedContent != null)
            {
                if (Controller.CloseQuery(SelectedPage))
                    QueryTabs.Items.RemoveAt(QueryTabs.SelectedIndex);
            }
        }

        private void CommandBinding_CanExecuteClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = QueryTabs.Items.Count > 0;
        }

        private void CommandBinding_OpenQueryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xq";
            dlg.Filter = "XQuery files|*.xq|Text documents|*.txt|All files|*.*";
            if (dlg.ShowDialog() == true)
                Controller.OpenQuery(QueryTabs, dlg.FileName);        
        }

        private void CommandBinding_SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (QueryTabs.SelectedContent != null)
                Controller.SaveQuery(SelectedPage);
        }

        private void CommandBinding_SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (QueryTabs.SelectedContent != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.InitialDirectory = SelectedPage.FilePath;
                dlg.FileName = SelectedPage.ShortFileName;
                dlg.DefaultExt = ".xq";                
                dlg.Filter = "XQuery files|*.xq|Text documents|*.txt|All files|*.*";
                if (dlg.ShowDialog() == true)
                    Controller.SaveAsQuery(SelectedPage, dlg.FileName);
            }
        }

        private void CommandBinding_CanExecuteSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedPage != null && SelectedPage.Modified;
        }

        private void CommandBinding_CanExecuteSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedPage != null && SelectedPage.HasContent;
        }

        private class PagePresenter
        {
            public TabItem Item { get; private set; }
            public QueryPage Content { get; private set; }

            public PagePresenter(TabItem item)
            {
                Item = item;
                Frame frame = (Frame)item.Content;
                Content = (QueryPage)frame.Content;
            }

            public override string ToString()
            {
                return Content.Title;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<PagePresenter> pages = new List<PagePresenter>();
            foreach (TabItem item in QueryTabs.Items)
            {
                PagePresenter p = new PagePresenter(item);
                if (p.Content.Modified)
                    pages.Add(p);
            }
            if (pages.Count > 0)
            {
                if (pages.Count == 1)
                {
                    if (!Controller.CloseQuery(pages[0].Content))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    ConfirmSaveDialog dlg = new ConfirmSaveDialog();
                    dlg.Owner = this;
                    foreach (PagePresenter presenter in pages)
                    {
                        dlg.listBox.Items.Add(presenter);
                        dlg.listBox.SelectedItems.Add(presenter);
                    }
                    if (dlg.ShowDialog() != true)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                        foreach (PagePresenter pp in dlg.listBox.SelectedItems)
                        {
                            if (!Controller.CloseQuery(pp.Content))
                            {
                                e.Cancel = true;
                                return;
                            }
                            QueryTabs.Items.Remove(pp.Item);
                        }  
                    
                }
            }
            foreach (TabItem item in QueryTabs.Items)
            {
                PagePresenter p = new PagePresenter(item);
                p.Content.Close();
            }
        }

        private void CommandBinding_CloneQueryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (QueryTabs.SelectedContent != null)
                Controller.CloneQuery(QueryTabs, SelectedPage);
        }

        private void CommandBinding_CanExecuteCloneQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = QueryTabs.SelectedContent != null && SelectedPage.HasContent;
        }

        private void CommandBinding_ShowResultsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedPage != null)
                SelectedPage.ShowResultPane = !SelectedPage.ShowResultPane;
        }

        private void MenuItem_PreferencesClick(object sender, RoutedEventArgs e)
        {
            PreferencesDialog.ShowDialog(Controller, false);
        }

        private void HyperlinkSetFolder_Click(object sender, RoutedEventArgs e)
        {
            PreferencesDialog.ShowDialog(Controller, true);
        }

        private void HyperlinkOrganize_Click(object sender, RoutedEventArgs e)
        {
            Extensions.ShellExecute(Controller.MyQueriesPath);
        }

        private void RefreshConnection_Click(object sender, RoutedEventArgs e)
        {
            TreeController2.RefreshConnection();
        }

        private void EditConnection_Click(object sender, RoutedEventArgs e)
        {
            TreeController2.EditConnection();
        }

        private void DeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            TreeController2.RemoveConnection();
        }

        private void HelpPrj_Click(object sender, RoutedEventArgs e)
        {
            Extensions.ShellExecute("http://qm.codeplex.com");
        }

        private void HelpGetStarted_Click(object sender, RoutedEventArgs e)
        {
            Extensions.ShellExecute("http://docs.google.com/View?id=d5cbh2c_73gk79srg3");
        }

        private void HelpXQTS_Click(object sender, RoutedEventArgs e)
        {
            Extensions.ShellExecute("http://dev.w3.org/2006/xquery-test-suite/PublicPagesStagingArea/XQTSReportSimple_XQTS_1_0_2.html");
        }

        private void HelpXQ_Click(object sender, RoutedEventArgs e)
        {
            Extensions.ShellExecute("http://www.w3.org/standards/xml/query");
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox.Run();
        }
    }
}
