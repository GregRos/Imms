// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Integrity.Main
open Funq.Tests.Integrity.Definitions
open System.CodeDom.Compiler
open System
open Funq.FSharp
open Funq.FSharp.Implementation
open Funq.Tests.Integrity.Tests 
open System.ComponentModel


[<EntryPoint>]
let main argv = 
    use writer = new IndentedTextWriter(Console.Out)
    let Test = Tests(Random().Next())
    let tests = 
        [Test.add_first; Test.add_last; Test.add_drop_first; Test.add_drop_last; 
        Test.add_drop_first_last; Test.add_first_range; Test.add_last_range; Test.insert_range; Test.insert_range_concat; Test.complex_add_drop_last; Test.complex_add_drop_first_last;
        Test.update; Test.insert; Test.insert_remove_update] |> List.cross_apply1 [1000]// |> List.filter (fun test -> test?Name |> String.contains "remove")

    let tests_seq = [Test.slices; Test.take; Test.get_index; Test.concat] |> List.cross_apply1 [1000]
    let targets = [Target.FunqArray; Target.FunqList] |> List.cross_apply1 [1000]
    let reference = Target.Reference 1000
    let runner = Runner(writer)
    //Runner.run_multi tests reference targets
    //Runner.run_multi tests reference targets

    runner.run_multi tests_seq reference targets
    Console.Read()
    //Console.Read() |> ignore
    0 // return an integer exit code
