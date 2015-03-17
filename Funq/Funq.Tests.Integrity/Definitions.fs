module Funq.Tests.Integrity.Definitions
open Funq.Tests.Integrity.Wrappers
open System.IO

open Printf
open System.Diagnostics
open Funq.FSharp.Implementation
open System
open System.Collections
open System.CodeDom.Compiler
open System.Collections.Generic

type TaggedSeq<'a>(inner : 'a seq) = 
    inherit MetaContainer([])
    interface 'a seq with
        member x.GetEnumerator() = inner.GetEnumerator()
        member x.GetEnumerator() : IEnumerator = inner.GetEnumerator():>_

let toContainer (s : _ seq) = TaggedSeq(s)

let rec equate_by_iterator (reference : #seq<'a>) (other : #seq<'a>) = 
    use ref_iterator = reference.GetEnumerator()
    use other_iterator = other.GetEnumerator()
    let mutable go = true
    let mutable are_equal = true
    while go do
        match ref_iterator.MoveNext(), other_iterator.MoveNext() with
        | true, true -> 
            if ref_iterator.Current <> other_iterator.Current then
                Debugger.Break()
                go <- false
                are_equal <- false
        | false, false -> 
            are_equal <- true
            go <- false
        | true, false | false, true ->
            Debugger.Break()
            go <- false
            are_equal <- false
    are_equal
        
let assert_sequence_equality (writer : IndentedTextWriter)(reference : 'a) (candidates : 'a seq) = 
    fprintfn writer "Reference: %s\t Targets: %s (count: %d)" (reference?Name) (candidates |> Seq.map (fun c -> c?Name) |> Seq.print ", ") (candidates |> Seq.length)
    writer.Push()
    let mutable passed = List()
    let failed = List()
    for current in candidates do
        fprintf writer "* Target '%s' vs reference '%s': " (current?Name) (reference?Name)
        let are_equal = equate_by_iterator reference current
        if are_equal then 
            fprintfn writer " PASSED. Target equal to reference."
            passed.Add(current.Clone)
        else
            fprintfn writer" FAILED. Target not equal to reference."
            failed.Add(current.Clone)
    writer.Pop()
    fprintfn writer "TOTAL: "
    let getName t = t?Name
    writer.Push()
    if passed.Count > 0 then fprintfn writer "Succeeded (%d): %s" (passed.Count) (passed |> Seq.printWordsWith getName) 
    if failed.Count > 0 then fprintfn writer "Failed (%d): %s" (failed.Count) (failed |> Seq.printWordsWith getName)
    
    writer.Pop()
    fprintf writer "SUMMARY: "
    let success = failed.Count = 0
    if success then
        fprintfn writer "SUCCESS"
    else
        fprintfn writer "FAILURE"
    success
 
type TargetResult = 
| Finished of float * obj
| NotImplemented
| Error of Exception
| FailedEquality

type TestResult = 
| ReferenceDidntFinish of TargetResult
| ZeroTargets
| TestSucceeded of Map<string,TargetResult>

type ErasedTest(test : unit -> seq<'v>) = 
    inherit MetaContainer()
    member x.Test = test
    

type Test<'v, 's>(param : int, name : string, test : int -> TestWrapper<'v> -> 's) = 
    inherit MetaContainer([Meta("Name", name); Meta("Parameter", param)])
    member x.Test s = test param s
    (*
    member x.Bind (target : 'input) = 
        let erased = ErasedTest(fun() -> x.Test target)
        erased?Test <- x.Clone
        erased?Target <- target.Clone
        erased
    *)
   
type Runner(writer : TextWriter) = 
    let writer = new IndentedTextWriter(writer)
    member x.run_single (test : Test<_,_>) (reference)(targets : _ list) = 
        let run_test target = 
            fprintf writer "Running on target '%s': " (target?Name)
            writer.Push()
            let mutable res = None
            try
                res <- Some(test.Test target)
                fprintfn writer "SUCCESS."
            with
                | OperationNotImplemented(name) -> 
                    fprintfn writer "The object doesn't implement the required operation '%s'." name
                | ex -> 
                    fprintfn writer "An exception of type '%s' occurred." (ex.GetType().Name)
            writer.Pop()
            res
        fprintfn writer "Executing test '%s(%d)'. Reference: '%s'. Targets: '%s' (%d)." 
            (test?Name) (test?Parameter) (reference?Name) (targets |> Seq.map (fun t -> t?Name) |> Seq.print ", ") (targets.Length) 
        writer.Push()
        let reference_result = reference |> run_test
        let  candidate_results = MList.ofList []
        for target in targets do
            let result = run_test target
            if result.IsSome then candidate_results.Add(result.Value)
    
        let mutable success = true
        if reference_result.IsNone then
            fprintfn writer "Test error: The reference failed the test. Aborting."
            success <- false
        else if candidate_results.Count = 0 then
            fprintfn writer "Test error: No targets completed the test. Aborting."
            success <- false
        else
            fprintfn writer "Tests completed successfully. Verifying results by sequence equality." 
            writer.Push()
            assert_sequence_equality writer (reference_result.Value) (candidate_results) |> ignore
            writer.Pop()
        writer.Pop()

    member x.run_multi  (tests : Test<_,_> list) (reference : _)(targets : _ list) = 
        fprintfn writer "START TEST GROUP. Test Count: %d\t Target Count: %d" (tests.Length) (targets.Length)
        writer.Push()
        for test in tests do
            fprintfn writer "START TEST '%s'" (test?Name)
            writer.Push()
            x.run_single test reference targets
            writer.Pop()
            fprintfn writer "END TEST '%s'" (test?Name)
            fprintfn writer ""
        writer.Pop()
        fprintfn writer "END TEST GROUP"
