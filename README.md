# QueryMachine

**NOTE: This is a fork of the [qm](https://github.com/semyonc/qm) project. It specifically forks off from [d7eef30cf0aec2afaa3da7b9c05691ce9fc80b08](https://github.com/semyonc/qm/tree/d7eef30cf0aec2afaa3da7b9c05691ce9fc80b08).**

## .NET Core port

This repository targets .NET Core (3.1), and most supporting library target .NET Standard 2.1. It's currently still a WIP, but a working prototype version of the XQuery.Console project now works as a .NET Core 3.1. app.

## Overview

QueryMachine is database independent SQL engine for queries to 
non-specific source of the data, such as XML, flat files, regular relational
databases or all of above. It supports XML-Related Specifications 
(SQL/XML) FCD ISO/IEC 9075-14:2005 (E).
It was designed for one specific purpose: to execute the query 
for the data containing virtually in any type of the structured storage. 
However, there are extensions to standard SQL-92 which adds compatibility 
with the different data sources.

# QueryMachine.XQuery

* QueryMachine.XQuery is XQuery 1.0 implementation based on XPathNavigator API.
* Key features of this XQuery implementation:
* Standard XPathNavigator API used
* Schema Aware XQuery
* On-demand document parsing
* Huge input files are not loaded into memory entirely
* XQuery expressions are compiled directly into MSIL
* Native support for MS Office OpenXML files
* Hash-join FLWOR optimization and implicit parallelization of XQuery requests
* XML mapping support (an experimental)
* Full implements of all minimal conformance 
features due to standard W3C XML Query Language
[XQuery Test Suite results on W3C](https://dev.w3.org/2006/xquery-test-suite/PublicPagesStagingArea/XQTSReportSimple_XQTS_1_0_2.html)
 
