// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Integrity.Main
open Funq.FSharp.Implementation
open System.CodeDom.Compiler
open System
open System.IO
type String with
    member str.ContainsWords (words : string) = 
        let split = words.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
        split |> Seq.forall (fun s -> String.contains_insensitive s str)

type MultiTextWriter(writers : TextWriter list) = 
    inherit TextWriter()
    member x.F = 4
    override x.Encoding = writers.Head.Encoding
    override x.WriteLine str = 
        writers |> List.iter (fun w -> w.WriteLine(str : string))
    override x.Write (str : string) = 
        writers |> List.iter (fun w -> w.Write(str))
    override x.WriteLine() = writers |> List.iter (fun w -> w.WriteLine())

let seqTests initial parameter = 
    let x = SeqTests()
    let targets = [Target.Funq.List initial; Target.Funq.Vector initial]
    let reference = Target.Sys.List initial
    let tests = 
        [x.add_drop_first;
         x.insert_range; 
         x.insert; 
         x.update;
         x.remove_add
         x.add_first_range;
         x.complex_add_last_take_and_indexing;
         x.complex_add_and_take_and_indexing;
         x.insert_remove_update;
         x.insert_range_concat;
         x.concat;
         x.update;
         x.get_index;
         x.insert_range]
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
    let fs = File.OpenWrite("log.txt")
    let writer = Console.Out
    
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
    let tests = tests |> List.filter (fun t -> t.Test.Kind = ListLike)
    tests |> Test.runAll writer |> Report.byTest writer 
    Console.Read() |> ignore
    fs.Flush()
    printfn "%A" argv
    0 // return an integer exit code
