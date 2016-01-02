// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Imms.Tests.Integrity.Main
open Imms.FSharp.Implementation
open System.CodeDom.Compiler
open System
open System.Linq
open System.IO
open ExtraFunctional
open Imms.FSharp.Operators
open Imms
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
    let targets = [Target.Imms.List initial; Target.Imms.Vector initial]
    let reference = Target.Sys.List initial
    let tests = 
        [x.Add_remove_first;
         x.Insert_range; 
         x.Insert; 
         x.Update;
         x.Remove_add_last
         x.Add_first_range;
         x.Complex_add_last_slices_indexing;
         x.Complex_add_first_last_take_slices_indexing;
         x.Add_remove_last_first_limited;
         x.Insert_remove_update;
         x.Slices;
         x.Take;
         x.Skip;
         x.Find;
         x.Get_index;
         x.Add_first;
         x.Add_remove_last_first_limited;
         x.Complex_add_remove_first_last;
         x.Add_first_last;
         x.Complex_add_remove_last;
         x.Reverse;
         x.Insert_range]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

let setTests initial parameter dataSet= 
    let generator = Seqs.distinctInts(1, 20) |> Seq.take initial |> Seq.toArray
    let gen2 = Seqs.distinctInts(1, 20) |> Seq.take dataSet |> Seq.toArray
    let x = SetTests(gen2)
    let targets = [Target.Imms.Set generator; Target.Imms.OrderedSet generator]
    let reference = Target.Sys.OrderedSet generator
    let tests = 
        [x.Add_remove; x.Difference; x.Except; x.Intersection; x.Union; x.Find]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

let mapTests initial parameter dataSet = 
    let generator = Seqs.distinctInts(1, 20) |> Seq.take initial |> Seq.map (fun x -> Kvp(x,x)) |> Seq.toArray
    let gen2 = Seqs.distinctInts(1, 20) |> Seq.take dataSet |>  Seq.toArray
    let x = MapTests(gen2)
    let targets = [Target.Imms.Map generator; Target.Imms.OrderedMap generator]
    let reference = Target.Sys.OrderedMap generator
    let tests = 
        [x.Add; x.Remove; x.Add_range; x.Remove_range; x.Except; x.Merge; x.Join; x.Difference; x.Add_remove; x.Find]
        |> List.apply1 parameter
        |> Test.bindAll reference targets
    tests

let f a b = 0
let g a b = 1

[<EntryPoint>]
let main argv = 

    let fl = ImmList<_>.Empty <+ 1
    let s = fl.Select(fun x -> x + 1)
    let dsf = s.First
    let fs = File.OpenWrite("log.txt")
    let writer = Console.Out
    let allTests initial param ds = 
        seqTests initial param  @ setTests initial param ds @ mapTests initial param ds
    let r = Random()
    let mutable tests = []
    for i = 0 to 0 do
        let initial, param, ds = r.Next(0, 1000), r.Next(100, 1000), r.Next( 30000, 40000)
        printfn "Initial: %d, Param: %d, ds: %d" initial param ds
        tests <- allTests initial param ds @ tests
    let tests = tests |> List.filter (fun t -> t.Test.Kind = SetLike)
    tests |> Test.runAll writer |> Report.byTest writer 
    Console.Read() |> ignore
    fs.Flush()
    printfn "%A" argv
    0 // return an integer exit code
