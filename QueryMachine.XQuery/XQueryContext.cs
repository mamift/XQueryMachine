﻿//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using System.Diagnostics;

namespace DataEngine.XQuery
{
    public class XQueryContext
    {
        public class VariableRecord
        {
            public object expr;
            public XQuerySequenceType varType;
            public SymbolLink link;
        }

        internal bool slave;
        internal XQueryContext master;
        internal NameTable nameTable;
        internal XmlSchemaSet schemaSet;
        internal XQueryNodeInfoTable nodeInfoTable;
        internal XQuerySchemaInfoTable schemaInfoTable;
        internal XmlNamespaceManager nsManager;
        internal Executive lispEngine;
        internal List<XQueryDocument> worklist;
        internal Stack<XQueryOrder> queryOrdering;
        internal List<String> moduleList;
        internal List<VariableRecord> variables;
        internal Dictionary<object, XQuerySequenceType> externalVars;
        internal Dictionary<XmlQualifiedName, string> option;

        public XQueryContext()
        {
            DefaultOrdering = XQueryOrder.Ordered;
            EmptyOrderSpec = XQueryEmptyOrderSpec.Least;
            SearchPath = String.Empty;
            ValidatedParser = true;
            
            nameTable = new NameTable();
            schemaSet = new XmlSchemaSet(nameTable);
            nodeInfoTable = new XQueryNodeInfoTable();
            schemaInfoTable = new XQuerySchemaInfoTable();
            nsManager = new XmlNamespaceManager(nameTable);
            worklist = new List<XQueryDocument>();
            queryOrdering = new Stack<XQueryOrder>();  
            Resolver = new XQueryResolver();
            moduleList = new List<string>();
            variables = new List<VariableRecord>();
            externalVars = new Dictionary<object, XQuerySequenceType>();
            option = new Dictionary<XmlQualifiedName, string>();
            
            Core.Init();
            FunctionTable = XQueryFunctionTable.CreateInstance();

            lispEngine = new Executive(this);
            lispEngine.Set("node#type", typeof(XPathNavigator));
            lispEngine.Enter(Resolver);            
            
            nsManager.AddNamespace("xml", XmlReservedNs.NsXml);
        }

        internal XQueryContext(XQueryContext master)
        {
            DefaultOrdering = XQueryOrder.Ordered;
            EmptyOrderSpec = XQueryEmptyOrderSpec.Least;
            SearchPath = master.SearchPath;
            BaseUri = master.BaseUri;

            nameTable = master.nameTable;
            schemaSet = master.schemaSet;
            nodeInfoTable = master.nodeInfoTable;
            schemaInfoTable = master.schemaInfoTable;
            variables = master.variables;
            externalVars = master.externalVars;
            option = new Dictionary<XmlQualifiedName, string>();

            nsManager = new XmlNamespaceManager(nameTable);
            nsManager.AddNamespace("xml", XmlReservedNs.NsXml);

            worklist = master.worklist;
            queryOrdering = master.queryOrdering;
            moduleList = new List<string>();

            FunctionTable = XQueryFunctionTable.CreateInstance();
            lispEngine = master.lispEngine;
            Resolver = master.Resolver;
            
            this.master = master;
            slave = true;
        }

        public virtual XmlReaderSettings GetSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = nameTable;
            settings.Schemas = schemaSet;
            settings.ProhibitDtd = false;
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;
            settings.XmlResolver = resolver;
            settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);
            if (ValidatedParser)
            {
                settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation |
                    XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationType = ValidationType.Schema;
            }
            return settings;
        }

        public virtual void InitNamespaces()
        {
            if (nsManager.LookupPrefix(XmlReservedNs.NsXs) == null)
                nsManager.AddNamespace("xs", XmlReservedNs.NsXs);
            if (nsManager.LookupPrefix(XmlReservedNs.NsXsi) == null)
                nsManager.AddNamespace("xsi", XmlReservedNs.NsXsi);
            if (nsManager.LookupPrefix(XmlReservedNs.NsXQueryFunc) == null)
                nsManager.AddNamespace("fn", XmlReservedNs.NsXQueryFunc);
            if (nsManager.LookupPrefix(XmlReservedNs.NsXQueryLocalFunc) == null)
                nsManager.AddNamespace("local", XmlReservedNs.NsXQueryLocalFunc);            
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            ValidationError((XmlReader)sender, e);
        }

        protected virtual void ValidationError(XmlReader sender, ValidationEventArgs e)
        {
            Trace.WriteLine(e.Message);
        }

        public XmlReader CreateReader(Stream stream)
        {            
            return XmlReader.Create(stream, GetSettings());
        }

        public XmlReader CreateReader(String uri)
        {
            return XmlReader.Create(uri, GetSettings());
        }

        public virtual XPathItem CreateItem(object value)
        {
            if (value == null)
                return new XQueryAtomicValue(false);
            else if (value is XQueryDocumentBuilder)
            {
                XQueryDocumentBuilder builder = (XQueryDocumentBuilder)value;
                return builder.m_document.CreateNavigator();
            }
            else
                return new XQueryAtomicValue(value);
        }

        public virtual string GetFileName(string name)
        {
            if (Uri.IsWellFormedUriString(name, UriKind.Absolute))
                return name;
            else
            {
                if (BaseUri != null)
                {
                    if (Uri.IsWellFormedUriString(BaseUri, UriKind.Absolute))
                        return new Uri(new Uri(BaseUri), name).ToString();
                    else
                    {
                        string fileName = Path.Combine(BaseUri, name);
                        if (File.Exists(fileName))
                            return fileName;
                    }
                }
                string[] pathset = SearchPath.Split(new char[] { ';' });
                foreach (string baseDir in pathset)
                {
                    string fileName = Path.Combine(baseDir, name);
                    if (File.Exists(fileName))
                        return fileName;
                }
            }
            return null;
        }

        public virtual Literal[] ResolveModuleImport(string prefix, string targetNamespace)
        {
            if (slave)
                return master.ResolveModuleImport(prefix, targetNamespace);
            else
                throw new XQueryException(Properties.Resources.XQST0059, targetNamespace);
        }
        
        public XQueryDocument CreateDocument()
        {
            XQueryDocument doc = new XQueryDocument(nameTable, nodeInfoTable, schemaInfoTable);
            worklist.Add(doc);
            return doc;
        }

        public IXPathNavigable OpenDocument(string fileName)
        {
            if (slave)
                return master.OpenDocument(fileName);
            else
            {
                Uri uri = new Uri(fileName);
                return OpenDocument(uri);
            }
        }

        public virtual IXPathNavigable OpenDocument(Uri uri)
        {
            if (slave)
                return master.OpenDocument(uri);
            else
            {
                foreach (XQueryDocument doc in worklist)
                {
                    if (doc.baseUri == uri.AbsoluteUri)
                        return doc;
                }
                XQueryDocument ndoc = CreateDocument();
                ndoc.Open(uri, GetSettings(), XmlSpace.Default);
                return ndoc;
            }
        }

        public virtual void Close()
        {
            if (!slave)
            {
                lispEngine.Leave();
                foreach (XQueryDocument doc in worklist)
                    doc.Close();
            }
        }

        public void CopyNamespaces(IXmlNamespaceResolver nsmgr)
        {
            IDictionary<string, string> nss = nsmgr.GetNamespacesInScope(XmlNamespaceScope.All);
            foreach (KeyValuePair<string, string> kvp in nss)
                nsManager.AddNamespace(kvp.Key, kvp.Value);
        }

        public virtual XPathNavigator CreateCollection(string collection_name)
        {
            if (slave)
                return master.CreateCollection(collection_name);
            else
                throw new XQueryException(Properties.Resources.FODC0004, collection_name);
        }

        public bool IsOrdered
        {
            get
            {
                if (queryOrdering.Count == 0)
                    return DefaultOrdering == XQueryOrder.Ordered;
                else
                    return queryOrdering.Peek() == XQueryOrder.Ordered;
            }
        }

        public void EnterOrdering(XQueryOrder order)
        {
            if (order == XQueryOrder.Default)
                throw new ArgumentException();
            queryOrdering.Push(order);
        }

        public void LeaveOrdering()
        {
            queryOrdering.Pop();
        }

        internal void AddExternalVariable(object var, XQuerySequenceType varType)
        {
            externalVars.Add(var, varType);
        }

        internal void AddVariable(object expr, XQuerySequenceType varType, SymbolLink link)
        {
            VariableRecord rec = new VariableRecord();
            rec.expr = expr;
            rec.varType = varType;
            rec.link = link;
            variables.Add(rec);
        }

        public VariableRecord[] GetVariables()
        {
            return variables.ToArray();
        }

        public void SetExternalVariable(object var, object value)
        {
            XQuerySequenceType varType;
            if (!externalVars.TryGetValue(var, out varType))
                throw new XQueryException(Properties.Resources.UnknownExternalVariable, var);
            SymbolLink link = lispEngine.Get(var);
            if (varType == XQuerySequenceType.Item)
                link.Value = value;
            else
                link.Value = Core.CastTo(lispEngine, value, varType);            
        }

        public XmlSchemaSet SchemaSet
        {
            get
            {
                return schemaSet;
            }
        }

        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                return nsManager;
            }
        }

        public Executive Engine
        {
            get
            {
                return lispEngine;
            }
        }

        public String SearchPath { get; set; }

        public String DefaultCollation { get; set; }

        public XQueryOrder DefaultOrdering { get; set; }

        public String DefaultElementNS { get; set; }

        public String DefaultFunctionNS { get; set; }

        public bool PreserveBoundarySpace { get; set; }

        public String BaseUri { get; set; }

        public XQueryEmptyOrderSpec EmptyOrderSpec { get; set; }

        public XQueryFunctionTable FunctionTable { get; private set; }

        public XQueryResolver Resolver { get; private set; }

        public bool ValidatedParser { get; set; }

        public IDictionary<XmlQualifiedName, string> Option
        {
            get
            {
                return option;
            }
        }
    }

    public class XPathContext : XQueryContext
    {
        private Dictionary<String, XPathDocument> docs;

        public XPathContext()
        {
            docs = new Dictionary<String, XPathDocument>();
        }

        public override XmlReaderSettings GetSettings()
        {
            XmlReaderSettings settings = base.GetSettings();
            settings.ValidationFlags = XmlSchemaValidationFlags.None;
            settings.ValidationType = ValidationType.None;
            return settings;
        }

        public override IXPathNavigable OpenDocument(Uri uri)
        {
            XPathDocument doc;
            if (docs.TryGetValue(uri.AbsoluteUri, out doc))
                return doc;
            else
            {
                XmlReader reader = XmlReader.Create(uri.AbsoluteUri, GetSettings());
                doc = new XPathDocument(reader, XmlSpace.Default);
                reader.Close();
                docs.Add(uri.AbsoluteUri, doc);
                return doc;
            }
        }
    }
}
