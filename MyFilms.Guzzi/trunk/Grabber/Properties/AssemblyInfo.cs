﻿using System.Reflection;
using System.Runtime.InteropServices;
#if MP12
using MediaPortal.Common.Utils;
#endif

//using System.Runtime.CompilerServices;


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("grabber.Properties")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("grabber.Properties")]
[assembly: AssemblyCopyright("Copyright ©  2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c45c7566-aaac-434b-b4f3-9abcc6350a52")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("5.0.1.1205")]
[assembly: AssemblyFileVersion("5.0.1.1205")]
#if MP12
//[assembly: CompatibleVersion("1.1.7.0", "1.1.6.0")]
[assembly: CompatibleVersion("1.1.6.27644")]
[assembly: UsesSubsystem("MP.Config")]
[assembly: UsesSubsystem("MP.DB")]
#endif
