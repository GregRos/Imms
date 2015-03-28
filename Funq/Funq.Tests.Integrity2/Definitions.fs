[<AutoOpen>]
module Funq.Tests.Integrity.Definitions

open System.IO

open Printf
open System.Diagnostics
open Funq.FSharp.Implementation
open System
open System.Collections
open System.CodeDom.Compiler
open System.Collections.Generic
open Funq.FSharp
exception OperationNotImplemented of string
exception AssertionFailed of assertion:string * data:obj

let inline assert_cond t message data =
    if not t then
        Debugger.Break()
        raise <| AssertionFailed(message, data)

let inline assert_eq (a, b) =
    assert_cond (obj.Equals(a,b)) "Aren't equal!" (a,b)

let inline assert_true t = 
    assert_cond t "Is false!" t
        
let inline assert_false t = 
    assert_cond (not t) "Is true!" t

let inline assert_neq a b =
    assert_cond (not <| obj.Equals(a,b)) "Are equal!" (a,b)

let inline is_in_range (min,max) x = 
    assert_cond (min <= x <= max) "In range" (x,min,max)
    

let toList1 = List.singleton
///Metadata for a generator
type GeneratorMeta(Name : string, ElementType : Type) as x =
    inherit MetaContainer()
    [<IsMeta>]
    member val Name = Name with get, set
    [<IsMeta>]
    member val ElementType = ElementType with get, set

type DataGenerator<'v>(Name : string, inner : 'v seq) =
    inherit GeneratorMeta(Name, typeof<'v>)
    member val Generate = inner
    member x.Map f = DataGenerator(Name, inner |> Seq.map f)

type TargetKind = 
| SetLike
| ListLike
| MapLike

[<AbstractClass>]
type TargetMetadata(Name : string) =
    inherit MetaContainer()
    [<IsMeta>]
    member val Name = Name
    [<IsMeta>]
    abstract Length : int
    abstract SelfTest : unit -> bool
[<StructuredFormatDisplay("{AsString}"); DefaultAugmentationAttribute(false)>]
type ExecResult<'s when 's :> TargetMetadata> = 
| Success of data:'s list
| NotImplemented of name:string
| Error of ex:Exception
| FailedAssertion of ex:AssertionFailed
| FailedEquality of data:'s list
| SelfTestFailed of data:'s list
    member x.Data = 
        match x with
        | Success d -> d
        | FailedEquality d -> d
        | _ -> []
    member x.AsString = 
        match x with
        | Success f -> 
            let len = f.Length
            let total = f |> List.sumBy (fun t -> t.Length)
            sprintf "Success: (units: %d, total: %d) %A" len total f.Head
        | NotImplemented str-> sprintf "Operation '%s' not implemented" str
        | Error ex -> sprintf "Error: %s" (ex.Message)
        | FailedAssertion ex -> sprintf "Failed assertion: %s" ex.assertion
        | FailedEquality f-> sprintf "Failed Equality: %A" f.Head
        | SelfTestFailed data -> sprintf "Self test failed: %A" data.Head
    member x.Succeeded = 
        match x with 
        | Success _ -> true
        | _ -> false
    member x.IsFailure = 
        match x with 
        | Success _ | NotImplemented _ -> false
        | _ -> true

type ErasedExecResult = ExecResult<TargetMetadata>

type ExecResult<'s> with
    member x.Erase = 
        match x with
        | Success data -> data |> List.cast<'s,_> |> Success
        | NotImplemented st -> NotImplemented st
        | Error ex -> Error ex
        | FailedAssertion ex -> FailedAssertion ex
        | FailedEquality data -> data |> List.cast<_,_> |> FailedEquality
        | SelfTestFailed data -> data |> List.cast |> SelfTestFailed
type TestMetadata(Iterations : int, Name : string, Kind : TargetKind) = 
    inherit MetaContainer()
    [<IsMeta>]
    member val Name = Name
    [<IsMeta>]
    member val Iterations = Iterations
    [<IsMeta>]
    member val Kind = Kind

type TestInstance(Result : ErasedExecResult, Target : TargetMetadata, Test : TestMetadata) = 
    [<IsMeta>]
    member val Result = Result
    [<IsMeta>]
    member val Target = Target
    [<IsMeta>]
    member val Test = Test

type Test<'v when 'v :> TargetMetadata>(Iterations : int, Name : string, Kind : TargetKind, test : 'v  -> 'v list) = 
    inherit TestMetadata(Iterations, Name, Kind)
    member x.Run target = 
        let output = 
            try test target |> Success
            with
            | OperationNotImplemented name -> NotImplemented name
            | AssertionFailed _ & ex  -> 
                raise ex
                FailedAssertion (ex :?> AssertionFailed)
            | ex -> Error ex
        output



type TestResult = 
| Ended of TestInstance list
| ReferenceDidNotFinish of TestInstance
    member x.HasFailures = 
        match x with
        | Ended lst -> lst |> List.exists (fun inst -> inst.Result.IsFailure)
        | _ -> false
type TestSummary(Test : TestMetadata, Targets : TargetMetadata list, Reference : TestInstance, Result : TestResult) =
    member val Test = Test
    member val Reference = Reference
    member val Result = Result
    member val Targets = Targets

type ITestMapping =
    abstract Run : log:TextWriter -> TestSummary
    abstract Test : TestMetadata
    abstract Reference : TargetMetadata
    abstract Targets : TargetMetadata list

type TestMapping<'v when 'v :> TargetMetadata>(Test : Test<'v>, Reference : 'v, Targets : 'v list) = 
    interface ITestMapping with
        member x.Run log = x.Run log
        member x.Test = Test:>_
        member x.Reference = Reference:>_
        member x.Targets = Targets |> List.cast<_,_>
    member val Test = Test
    member val Reference = Reference
    member val Targets = Targets
    member x.Run (writer : TextWriter) =
        let writer = new IndentedTextWriter(writer)
        let test, reference, targets = Test, Reference, Targets 
        let targetsStr = targets |> Seq.map (fun t -> t.Name) |> String.join ", "
        fprintfn writer "Executing test '%s' with reference '%s' on targets: '%A'" test.Name reference.Name targetsStr
        writer.Push()
        fprintfn writer "Running test on reference '%s'" reference.Name
        let ref_result = test.Run reference
        fprintfn writer "Result: %A" ref_result
        let refObj = TestInstance(ref_result.Erase, Reference, Test)
        if not ref_result.Succeeded then
            let r = ReferenceDidNotFinish refObj
            TestSummary(Test, Targets |> List.cast<_,_>, refObj, r)
        else
            
            let rec resultList : 'v list -> TestInstance list = function
                | [] -> []
                | target::rest -> 
                    let res =   
                        fprintfn writer "Running test on '%s'" target.Name
                        let res = test.Run target
                        fprintfn writer "Test result: '%A'" res
                        if res.Succeeded then fprintfn writer "Doing equality testing."
                        if not res.Succeeded then 
                            fprintfn writer "Test failed."
                            res
                        elif Seq.equals (res.Data) (ref_result.Data) then 
                            fprintfn writer "Equality passed."
                            let selfTests = res.Data |> Seq.forall (fun x -> x.SelfTest())
                            if not selfTests then
                                fprintfn writer "Self-test failed!"
                                SelfTestFailed (res.Data)
                            else res
                        else
                            fprintfn writer "Equality failed." 
                            FailedEquality (res.Data)
                    TestInstance(res.Erase, target, Test)::resultList rest
            let results = resultList targets
            let r = Ended results
            TestSummary(Test, Targets |> List.cast<_,_>, refObj, r)

type Test<'v> with
    member x.Bind (reference : 'v) (targets : 'v list) = 
        TestMapping<'v>(x,reference,targets) :> ITestMapping

module Test = 
    let bind ref targets (test :_ Test) = test.Bind ref targets
    let bindAll  ref targets (test :_ Test list) = test |> List.map (bind ref targets)
    let runAll writer (bound : ITestMapping list) = bound |> List.map(fun x -> x.Run writer)

module List =
    let toBinder (x : _ Test list) = x |> List.map (fun x -> x.Bind)

module Report = 
    let byTest writer (summaries : TestSummary list)  = 
        let w = new IndentedTextWriter(writer)
        fprintfn w "BEGINNING REPORT"
        w.Push()
        fprintfn w "Summing up results of %d tests:" summaries.Length
        w.Push()
        let dict = Dict.empty
        let allTargets = 
            summaries 
            |> Seq.map (fun s -> s.Targets |> Seq.map (fun t -> t.Name)) 
            |> Seq.collect id |> Seq.distinct |> Seq.iter (fun target -> dict.[target] <- 0)
        for summary in summaries |> Seq.sortBy (fun s -> s.Result.HasFailures) do
            fprintf w "* %s:" summary.Test.Name
            match summary.Result with
            | ReferenceDidNotFinish _ -> fprintfn w " Test abored (reference did not finish)"
            | Ended results ->
                fprintfn w " Test finished. Results:"
                w.Push()
                fprintfn w "* Reference %s: %A" (summary.Reference.Target.Name) (summary.Reference.Result)
                for instance in results do
                    fprintfn w "* %s: %A" (instance.Target.Name) (instance.Result)
                    if instance.Result.IsFailure then
                        dict.[instance.Target.Name] <- dict.[instance.Target.Name] + 1
                w.Pop()
        w.Pop()
        fprintfn w "Breakdown of failures by target:"
        w.Push()
        for name,fails in dict |> Dict.toSeq do
            fprintfn w "* %s: %d" name fails 
        w.Pop()
        w.Pop()
        fprintfn w "END REPORT"

        

            
                            
                        
                    
        
        