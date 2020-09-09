﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Bluegrams.Application.Attributes;

[assembly: AssemblyTitle("Colora - Color Picker and Converter")]
[assembly: AssemblyDescription("Color Picker and Converter for Windows")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bluegrams")]
[assembly: AssemblyProduct("Colora")]
[assembly: AssemblyCopyright("Copyright © 2019 Bluegrams")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ProductWebsite("https://colora.sourceforge.io")]
[assembly: ProductLicense("LICENSE.txt", "BSD-3-Clause")]
[assembly: CompanyWebsite("http://bluegrams.com", "Bluegrams")]
[assembly: SupportedCultures("en", "de", "es")]

#if PORTABLE
[assembly: AppPortable(true)]
#else
[assembly: AppPortable(false)]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: AssemblyVersion("0.3.0")]
[assembly: AssemblyFileVersion("0.3.0")]
