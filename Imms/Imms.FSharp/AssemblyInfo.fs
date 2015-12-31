module private Imms.FSharp.AssemblyInfo
open System.Diagnostics;
open System.ComponentModel;
open System.Runtime.InteropServices;
open System.Reflection;
open System.Runtime.CompilerServices
[<assembly: AssemblyName("Imms.FSharp")>]
[<assembly: AssemblyTitle("Imms.FSharp")>]
[<assembly: AssemblyDescription("Companion assembly with F# support")>]
[<assembly: AssemblyVersion("0.4.*")>]
[<assembly: AutoOpen("Imms.FSharp")>]
[<assembly: InternalsVisibleTo("Imms.Tests.Performance")>]
[<assembly: InternalsVisibleTo("Imms.Tests.Integrity")>]
do()