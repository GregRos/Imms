module private Funq.Tests.Performance.AssemblyInfo
open System.Diagnostics;
open System.ComponentModel;
open System.Runtime.InteropServices;
open System.Reflection;
[<assembly: AssemblyName("Funq.Tests.Performance")>]
[<assembly: AssemblyTitle("Funq.Tests.Performance")>]
[<assembly: AssemblyDescription("Funq collection benchmarking library")>]
[<assembly: AssemblyVersion("0.4.*")>]
[<assembly: AutoOpen("Funq.Tests")>]
do()