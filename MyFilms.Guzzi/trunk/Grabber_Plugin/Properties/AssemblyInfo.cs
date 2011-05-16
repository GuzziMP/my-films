﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if MP12
using MediaPortal.Common.Utils;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MultiGrabber")]
[assembly: AssemblyDescription("Multigrabber to use grabber scripts with MyVideo")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Titof")]
[assembly: AssemblyProduct("MultiGrabber")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1adc3e79-5065-463d-b804-3319f27f7f15")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("5.0.1.815")]
[assembly: AssemblyFileVersion("5.0.1.815")]
#if MP12
//[assembly: CompatibleVersion("1.1.7.0", "1.1.6.0")]
[assembly: CompatibleVersion("1.1.6.27644")]
[assembly: UsesSubsystem("MP.Config")]
[assembly: UsesSubsystem("MP.DB")]
#endif
