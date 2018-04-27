﻿using log4net.Config;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SharePointRunner.PowerShell")]
[assembly: AssemblyDescription("PowerShell CmdLet to run processes accross a SharePoint Online structure, from tenant to list items and files")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bastien Perdriau")]
[assembly: AssemblyProduct("SharePointRunner.PowerShell")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4bd43187-7333-418b-990e-05c17b0f0368")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.2")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: XmlConfigurator(Watch = true)]