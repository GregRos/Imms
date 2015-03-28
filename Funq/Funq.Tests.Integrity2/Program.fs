// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Integrity.Main
open Funq.FSharp.Implementation
open System.CodeDom.Compiler
open System

type String with
    member str.ContainsWords (words : string) = 
        let split = words.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
        split |> Seq.forall (fun s -> String.contains_insensitive s str)

let seqTests initial parameter = 
    let x = SeqTests()
    let targets = [Target.Funq.List initial; Target.Funq.Vector initial]
    let reference = Target.Sys.List initial
    let tests = 
        [x.add_drop_first;
         x.insert_range; 
         x.insert; 
         x.complex_add_last_take_and_indexing]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

let setTests initial parameter dataSet= 
    let generator = Seqs.distinctInts(1, 20) |> Seq.take initial |> Seq.toArray
    let gen2 = Seqs.distinctInts(1, 20) |> Seq.take dataSet |> Seq.toArray
    let x = SetTests(gen2)
    let targets = [Target.Funq.Set generator; Target.Funq.OrderedSet generator]
    let reference = Target.Sys.OrderedSet generator
    let tests = 
        [x.add_drop; x.difference; x.except; x.intersection; x.union; x.many_operations; x.add_drop_range]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

let mapTests initial parameter dataSet = 
    let generator = Seqs.distinctInts(1, 20) |> Seq.take initial |> Seq.map (fun x -> Kvp(x,x)) |> Seq.toArray
    let gen2 = Seqs.distinctInts(1, 20) |> Seq.take dataSet |>  Seq.toArray
    let x = MapTests(gen2)
    let targets = [Target.Funq.Map generator; Target.Funq.OrderedMap generator]
    let reference = Target.Sys.OrderedMap generator
    let tests = 
        [x.add_drop;x.add_drop_range; x.add_range; x.drop_range]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

[<EntryPoint>]
let main argv = 
    let x = SeqTests()
    let writer = new IndentedTextWriter(Console.Out)
    let initial = 10000
    let param = 3000
    let ds = 10000
    let allTests initial param ds = 
        seqTests initial param  @ setTests initial param ds @ mapTests initial param ds
    let r = Random()
    let mutable tests = []
    for i = 0 to 5 do
        let initial, param, ds = r.Next(0, 3000), r.Next(100, 1000), r.Next( 5000, 10000)
        printfn "Initial: %d, Param: %d, ds: %d" initial param ds
        tests <- allTests initial param ds @ tests
    let tests = tests |> List.filter (fun t -> t.Test.Kind = MapLike || t.Test.Kind = SetLike )
    tests |> Test.runAll writer |> Report.byTest writer 
    Console.Read() |> ignore
    printfn "%A" argv
    0 // return an integer exit code
