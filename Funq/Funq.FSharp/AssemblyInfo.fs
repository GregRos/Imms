module private Funq.FSharp.AssemblyInfo
open System.Diagnostics;
open System.ComponentModel;
open System.Runtime.InteropServices;
open System.Reflection;
open System.Runtime.CompilerServices
[<assembly: AssemblyName("Funq.FSharp")>]
[<assembly: AssemblyTitle("Funq.FSharp")>]
[<assembly: AssemblyDescription("Companion assembly with F# support")>]
[<assembly: AssemblyVersion("0.4.*")>]
[<assembly: AutoOpen("Funq.FSharp")>]
[<assembly: InternalsVisibleTo("Funq.Tests.Performance")>]
[<assembly: InternalsVisibleTo("Funq.Tests.Integrity")>]
[<assembly: InternalsVisibleTo("Funq.Tests.Integrity2")>]
do()