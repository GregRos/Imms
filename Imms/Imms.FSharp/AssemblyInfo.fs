module private Imms.FSharp.AssemblyInfo
open System.Diagnostics;
open System.ComponentModel;
open System.Runtime.InteropServices;
open System.Reflection;
open System.Runtime.CompilerServices
open Imms
[<assembly: AssemblyName("Imms.FSharp")>]
[<assembly: AssemblyTitle("Imms.FSharp")>]
[<assembly: AssemblyDescription(
    @"F# companion assembly for the Imms immutable collections library.

Contains extension methods, active patterns, modules, computation expressions, and other things for use with Imms immutable collections.
")>]
[<assembly: AssemblyCompany(ImmsInfo.Author)>]
[<assembly: AssemblyCopyright(ImmsInfo.Copyright)>]
[<assembly: AssemblyProduct("Imms.FSharp")>]
[<assembly: AssemblyVersion(ImmsInfo.Version)>]
[<assembly: AutoOpen("Imms.FSharp")>]
[<assembly: InternalsVisibleTo("Imms.Tests.Performance")>]
[<assembly: InternalsVisibleTo("Imms.Tests.Integrity")>]
do()