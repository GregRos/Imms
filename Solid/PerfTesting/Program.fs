// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Benchmarks.Main
open Solid
open System
open System.Diagnostics
open CsvHelper
open System.Collections.Immutable
open System.IO
open Benchmarks.TestSuite
open Benchmarks
open System.Linq

let inline (^*) (a : int) (b : int) = pown a b

open Test

       
[<EntryPoint>]
let  main argv =
    //let res = Bench.Invoke (Test.InsertRandom(1000000).Bind(Target.My.List(1000000)))

    
    let lowIters = 100
    //In the following section I generate targets of different sizes
    //And bind several performance tests to each.
    //The type parameter of Builder<'s> is set to the type of target I bound last.
    //Whenever I call Builder.next, the type of the builder is reset to an unknown type.
    //In the next call to Builder.addTarget(s), type inference assigns the type parameter of the target I add.
    let initial = 0.5 * (10. ** 5.) |> int
    let iters = 0.5 * (10. ** 5.) |> int
    //let vect = Vector.Empty.AddLastRange([0 .. 10000000])
    let lowIters = 10
    let tests = 
        Builder.blank
        |> Builder.addTargets [Target.My.List initial]
        |> Builder.addTests   [Test.AddFirst iters; Test.AddLast iters; 
                               Test.DropLast iters; Test.DropFirst iters; Test.InsertRandom iters;
                               Test.RemoveRandom iters; Test.SetRandom iters; Test.GetRandom iters; Test.Take iters]
        |> Builder.addTests   [Array(initial) |> Test.AddLastRange lowIters ]
        |> Builder.addTests   [FlexibleList(initial) |> Test.InsertRangeRandom lowIters]
        |> Builder.addTests   [Array(initial) |> Test.InsertRangeRandom lowIters]
        |> Builder.addTests   [Array(initial) |> Test.AddFirstRange lowIters]
        |> Builder.addTests   [FlexibleList(initial) |> Test.AddLastRange lowIters]
        |> Builder.next
        |> Builder.addTargets [Target.My.Vector initial]
        |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; Test.GetRandom iters; Test.SetRandom iters; Test.Take iters]
        |> Builder.addTests   [Array(initial) |> Test.AddLastRange lowIters ]
        |> Builder.next
        |> Builder.addTargets [Target.Sys.List initial]
        |> Builder.addTests   [Test.AddFirst iters; Test.AddLast iters; Test.DropFirst iters; Test.DropLast iters;
                               Test.InsertRandom iters; Test.RemoveRandom iters; Test.SetRandom iters; Test.GetRandom iters]
        |> Builder.addTests   [Array(initial) |> Test.AddLastRange lowIters ]
        |> Builder.addTests   [Array(initial) |> Test.InsertRangeRandom lowIters]
        |> Builder.addTests   [FlexibleList(initial) |> Test.InsertRangeRandom lowIters]
        |> Builder.next
        |> Builder.addTargets [Target.Sys.List 1000; Target.Sys.List 10000]
        |> Builder.addTests [Test.Take 1000]
        |> Builder.next
        |> Builder.addTargets [Target.Sys.Queue initial]
        |> Builder.addTests   [Test.AddLast iters; Test.DropFirst iters]
        |> Builder.next
        |> Builder.addTargets [Target.Sys.Stack initial]
        |> Builder.addTests   [Test.AddFirst iters; Test.AddFirst iters]
        |> Builder.next
        |> Builder.addTargets [Target.FSharpx.Vector initial]
        |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; Test.GetRandom iters; Test.SetRandom iters]
        |> Builder.next
        |> Builder.addTargets [Target.FSharpx.Deque initial]
        |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; 
                               Test.DropFirst iters; Test.AddFirst iters]
        |> Builder.next
        |> Builder.addTargets [Target.FSharpx.RanAccList initial]
        |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; 
                               Test.GetRandom iters; Test.SetRandom iters]
        |> Builder.finish

    
    //let tests = tests |> List.filter (fun t -> t.Tag.Target = "Solid.Vector")
    let filename = "results.csv"
    do
        use file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
        use stream = new StreamWriter(file)
        use csvWriter = new CsvWriter(stream)
        stream.AutoFlush<- true
        for test in tests do
            let result = Bench.Invoke test
            csvWriter.WriteRecord(result)

            stream.Flush()
        stream.Flush()

    printfn "Done."
    0 // return an integer exit code
