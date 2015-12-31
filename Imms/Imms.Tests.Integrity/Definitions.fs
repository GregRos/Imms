[<AutoOpen>]
module Imms.Tests.Integrity.Definitions

open System.IO
open ExtraFunctional
open Printf
open System.Diagnostics
open Imms.FSharp.Implementation
open System
open System.Collections
open System.CodeDom.Compiler
open System.Collections.Generic
open Imms.FSharp
exception OperationNotImplemented of string
exception AssertionFailed of assertion:string * data:obj

type FuncPropertyWrapper<'a>(f : unit -> 'a) = 
    member x.invoke = f()
   
let assert_cond t message data =
    if not t then
        Debugger.Break()
        raise <| AssertionFailed(message, data)

let exp_same (ex1 : Exception) (ex2 : Exception) = 
    ex1.Message = ex2.Message

let assert_eq_meta (a : 'v,b : 'v,meta) =
    assert_cond (obj.Equals(a,b)) "Aren't equal!" (a,b,meta)

let assert_eq (a : 'v, b : 'v) =
    assert_cond (obj.Equals(a,b)) "Aren't equal!" (a,b)

let assert_eq_all lst = 
    match lst with
    | [] | [_]-> ()
    | head::tail -> 
        let res = tail |> List.forall (fun x -> obj.Equals(head, x))
        assert_cond res "Not all were equal!" lst

let assert_true t = 
    assert_cond t "Is false!" t
    
let assert_all ts = 
    assert_cond (ts |> List.forall id) "Are false!" ts
        
let assert_false t = 
    assert_cond (not t) "Is true!" t

let assert_neq(a,b) =
    assert_cond (not <| obj.Equals(a,b)) "Are equal!" (a,b)

let is_in_range (min,max) x = 
    assert_cond (min <= x <= max) "In range" (x,min,max)
    
let is_some opt = 
    assert_cond (opt |> Option.isSome) "Was none!" opt

let is_none opt =
    assert_cond (opt |> Option.isNone) "Was some!" opt

let (.|>) arg f = 
    fun s -> f s arg

let (..|>) arg f =
    fun a b -> f a b arg

let setValue (x : byref<_>) v = 
    x <- v

let getValue (x : byref<_>) () = x

type MutableBox<'v>(getter,setter) = 
    member x.Value 
        with get() : 'v = getter()
        and set v = setter(v)
    member x.On f = 
        x.Value <- f(x.Value)
    static member (!) (box : MutableBox<'v>) =
        box.Value
    static member (<--) (box : MutableBox<'v>, v : 'v) =
        box.Value <- v

let refBox (v : byref<_>) =
    MutableBox(getValue &v, setValue &v)


let countCalls_1 actual  f = 
    fun a -> 
        actual := !actual+1
        f a

let countCalls_2 actual f=
    fun a b ->
        actual := !actual+1
        f a b

let countCalls_3 actual f = 
    fun a b c ->
        actual := !actual+1
        f a b c

let noCallsAfterReturn_1 v f = 
    let stopCalls = ref false
    fun a ->
        assert_false(!stopCalls)
        let r = f a
        if obj.Equals(r, v) then stopCalls := true
        r

let noCallsAfterReturn_1_f cond f = 
    let stopCalls = ref false
    fun a ->
        assert_false(!stopCalls)
        let r = f a
        if cond r then stopCalls := true
        r

let assert_operations_equal_1(ff1,f1,ff2, f2) =
    let fs = [|f1;f2|]
    let runs = [|0; 0|]
    let partial = [| []; [] |]
    let wrap n item = 
        let r = fs.[n] item
        runs.[n] <- runs.[n] + 1
        partial.[n] <- r::partial.[n]
        r
    let r1,r2 = ff1 (wrap 0), ff2 (wrap 1)
    assert_eq(partial.[0], partial.[1])
    assert_eq(r1,r2)
    r1

let assert_operations_equal_2(ff1, f1,ff2, f2) =
    //((_ * _ -> _) -> _)
    let wrap f = fun (a,b) -> f a b
    let unwrap f = fun a b -> f (a,b)
    let wrapp ff = fun f -> ff (unwrap f)
    assert_operations_equal_1(wrapp ff1, wrap f1, wrapp ff2,wrap f2)

type EnsureIterateOnce<'v>(Inner : seq<'v>, Enabled : bool) = 
    let iterated = ref false
    interface seq<'v> with
        member x.GetEnumerator() = 
            if Enabled then 
                assert_false(!iterated)
                iterated := true
            Inner.GetEnumerator()
        member x.GetEnumerator() : IEnumerator = (x :> seq<'v>).GetEnumerator() :>_
    member val Inner = Inner
    member val Enabled = Enabled
    member x.AsDisabled = EnsureIterateOnce<'v>(Inner, false)
    member x.AsEnabled = EnsureIterateOnce<'v>(Inner, true)

module Seq = 
    
    let toMutableList (x : _ seq) = List<_>(x)
    let toMutableListDirty (x : _ seq) = 
        match x with
        | :? MutableList<_> as list -> list
        | _ -> x |> toMutableList

    let countWhere f sq = 
        sq |> Seq.fold (fun st x -> if f x then st + 1 else st) 0

    let smartLength (sq : 'v seq) =
        match sq with
        | :? ICollection as col -> col.Count
        | :? ICollection<'v> as col -> col.Count
        | :? IReadOnlyCollection<'v> as col -> col.Count
        | _ -> sq |> Seq.length

    let keys (sq : Kvp<'k,'v> seq) = 
        sq |> Seq.map (fun kvp -> kvp.Key)

    let values (seq : Kvp<'k,'v> seq) = 
        seq |> Seq.map (fun kvp -> kvp.Value)

    let iterateOnce (sq : 'v seq) = EnsureIterateOnce(sq, false) :> _ seq

    let duplicateRandom prob (r : Random) (sq : 'v seq) = 
        let arr = sq |> Seq.asArray
        let withDuplicates = seq {
            for i = 0 to arr.Length do
                while r.NextDouble() < prob do
                    yield arr.[r.Next(0, arr.Length)]
        }
        withDuplicates |> Seq.take (arr.Length) |> Seq.toArray

    let canEnsureIterateOnce (sq : 'seq :> #seq<'v>) =
        match sq :> obj with
        | :? EnsureIterateOnce<'v> as sq -> sq.AsEnabled :> obj :?> 'seq
        | _ -> sq

    let disableIterateOnce (sq : 'seq :> #seq<'v>) =
        match sq :> obj with
        | :? EnsureIterateOnce<'v> as sq -> sq.AsDisabled :> obj :?> 'seq
        | _ -> sq
        

let inline ( .* ) a b = ((float a) * (float b)) |> int
let inline ( ./ ) a b = ((float a) / (float b)) |> int

let (|Equals|_|) x y = if obj.Equals(x, y) then Some() else None

type ArgOutOfRangeEx = ArgumentOutOfRangeException
type ArgEx = ArgumentException
type ArgNullEx = ArgumentNullException
type InvalidOpEx = InvalidOperationException
type InvalidCastEx = InvalidCastException
type KeyNotFoundEx = KeyNotFoundException
type NotSupportedEx = NotSupportedException
let no_exceptions = true
let assert_ex<'ex,'v when 'ex :> Exception>  (f : unit -> 'v) = 
    if not no_exceptions then
        try 
            f() |> ignore
            Debugger.Break()
            raise <| AssertionFailed(sprintf "Expected exception of type '%A'" typeof<'ex>, null)
        with
        | :? 'ex -> ()
        | :? AssertionFailed -> reraise()
        | OperationNotImplemented str -> ()
        | caught -> 
            Debugger.Break()
            raise <| AssertionFailed(sprintf "Expected exception of type '%A' but got exception of type '%A'" typeof<'ex> (caught.GetType()), caught)


let assert_no_ex (f : unit -> 'v) =
    try
        f() |> ignore
    with
    | OperationNotImplemented name -> ()
    | ex -> 
        Debugger.Break()
        raise <| AssertionFailed(sprintf "Expected no exception, but caught '%A'" ex, ex)

let assert_ex_arg_out_of_range f = assert_ex<ArgOutOfRangeEx,_> f
let assert_ex_arg_null f = assert_ex<ArgNullEx,_> f
let assert_ex_arg f = assert_ex<ArgEx,_> f
let assert_ex_inv_op f = assert_ex<InvalidOpEx,_> f
let assert_ex_inv_cast f = assert_ex<InvalidCastEx,_> f
let assert_ex_key_not_found f = assert_ex<KeyNotFoundEx,_> f
let assert_ex_not_supported f = assert_ex<NotSupportedEx,_> f
let wrapFunc f = FuncPropertyWrapper(f)
let toList1 x = [x :> obj]
///Metadata for a generator
type GeneratorMeta(Name : string, ElementType : Type) =
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
type ExecResult = 
| Success of data:obj list
| NotImplemented of name:string
| Error of ex:Exception
| FailedAssertion of ex:AssertionFailed
| FailedEquality of data:obj list
    member x.Data = 
        match x with
        | Success d -> d
        | FailedEquality d -> d
        | _ -> []
    member x.AsString = 
        match x with
        | Success lst -> 
            let x = lst |> List.ofType<_,IEnumerable> |> List.sumBy (fun x -> x |> Seq.cast<obj> |> Seq.smartLength)
            sprintf "Success: (length: %d, total: %d) %A" (lst.Length) x (lst.Head)
        | NotImplemented str-> sprintf "Operation '%s' not implemented" str
        | Error ex -> sprintf "Error: %s" (ex.Message)
        | FailedAssertion ex -> sprintf "Failed assertion: %s" ex.assertion
        | FailedEquality f-> sprintf "Failed Equality: %A" f.Head
    member x.Succeeded = 
        match x with 
        | Success _ -> true
        | _ -> false
    member x.IsFailure = 
        match x with 
        | Success _ | NotImplemented _ -> false
        | _ -> true

type TestMetadata(Iterations : int, Name : string, Kind : TargetKind) = 
    inherit MetaContainer()
    [<IsMeta>]
    member val Name = Name
    [<IsMeta>]
    member val Iterations = Iterations
    [<IsMeta>]
    member val Kind = Kind

type TestInstance(Result : ExecResult, Target : TargetMetadata, Test : TestMetadata) = 
    [<IsMeta>]
    member val Result = Result
    [<IsMeta>]
    member val Target = Target
    [<IsMeta>]
    member val Test = Test

type Test<'col when 'col :> TargetMetadata and 'col : equality>(Iterations : int, Name : string, Kind : TargetKind, test : 'col  -> obj list) = 
    inherit TestMetadata(Iterations, Name, Kind)
    member x.Run target = 
        let output = 
            try test target |> Success
            with
            | OperationNotImplemented name -> NotImplemented name
            | AssertionFailed _ & ex  -> 
                raise ex
                FailedAssertion (ex :?> AssertionFailed)
            | ex -> 
                reraise()
                Debugger.Break()
                Error ex
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

type TestMapping<'v when 'v :> TargetMetadata and 'v : equality>(Test : Test<'v>, Reference : 'v, Targets : 'v list) = 
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
        fprintfn writer "Checking if targets are equal to start with"
        for target in Targets do
            if not(target.Equals(Reference)) then 
                fprintfn writer "The target '%s' is not equal to reference" target.Name
        fprintfn writer "Running test on reference '%s'" reference.Name
        let ref_result = test.Run reference
        fprintfn writer "Result: %A" ref_result
        let refObj = TestInstance(ref_result, Reference, Test)
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
                            res
                        else
                            fprintfn writer "Equality failed." 
                            FailedEquality (res.Data)
                    TestInstance(res, target, Test)::resultList rest
            let results = resultList targets
            let r = Ended results
            TestSummary(Test, Targets |> List.cast<_,_>, refObj, r)

type Test<'v> with
    member x.Bind (reference : 'col) (targets : 'col list) = 
        TestMapping<'col>(x,reference,targets) :> ITestMapping

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

module Seqs = 
    let rnd = Random()
    let letters(min,max) = 
        Seq.initInfinite (fun _ -> Chars.alphanumeric |> rnd.String(min,max)) |> Seq.cache
    let distinctLetters(min,max) = letters(min,max) |> Seq.lazyDistinct
    let ints(min,max) = 
        let min,max = pown 2 min, pown 2 max
        Seq.initInfinite (fun _ -> rnd.Next(min,max))
    let distinctInts(min,max) = 
        ints(min,max) |> Seq.lazyDistinct

module RndCache = 
    let arrs = Dict.empty<int, int array>

type Random with
    ///Gets a nice mix of very low, medium, and very high values with a relatively low average.
    member x.SpecialRecipe n =
        let space = 
            if RndCache.arrs.ContainsKey n then
                RndCache.arrs.[n]
            else
                let arr = [|for i in 0 .. 100*n -> (100. * (float n) / (float (i + 1))) |> int |] 
                RndCache.arrs.[n] <- arr
                arr
        let totals = space |> Seq.scan (fun (sum,_) cur -> sum + cur, cur) (0, 0) |> Seq.skip 1 |> Seq.toArray
        let last = (totals.Length - 1) |> Array.get totals |> fst
        fun () -> 
            let r = x.Next(0, last)
            let ix = r |> Array.binSearchBy (fst) @? totals
            let res = totals.[ix] |> snd
            let mr = totals.[ix] |> fst
            let z = 5
            ix
        
[<AbstractClass>]
type KeySemantics<'t>() = 
    interface IEq<'t> with
        member x.Equals(a,b) = x.Compare a b = 0
        member x.GetHashCode a = x.GetHashCode a
    interface ICmp<'t> with
        member x.Compare(a,b) = x.Compare a b
    abstract Compare : 't -> 't -> int
    abstract GetHashCode : 't -> int
    member x.Wrap v = ComparableKey(v, x)
    

and ComparableKey<'t>(Value : 't, Comparer : KeySemantics<'t>) =
    interface IComparable with
        member x.CompareTo r = 
            let r = 
                match r with
                | :? ComparableKey<'t> as r -> r.Value
                | _ -> r :?> 't
            Comparer.Compare (r) (Value)
        
    member val Value = Value
    override x.Equals o = (x :> IComparable).CompareTo(o) = 0
    override x.GetHashCode() = Comparer.GetHashCode Value
  
let keySemantics getHashCode compare = 
    {new KeySemantics<'t>() with
        override x.Compare a b = compare a b
        override x.GetHashCode a = getHashCode a} 

let defaultKeySemantics<'t> =
    let dEq = Eq.Default
    let dCmp = Cmp.Default
    keySemantics (dEq.GetHashCode) (fun (a : 't) (b : 't) -> dCmp.Compare(a,b))
                            
type IDataSource = 
    abstract member Name : string
    abstract member Length : int

type DataSource<'e>(Value : 'e seq, Name : string, Length : int) =
    interface IDataSource with
        member x.Name = Name
        member x.Length = Length
    member val Value = Value

type DataCollection<'col> = {
    Source : IDataSource
    Name : string
    Value : 'col
} 

module Data =
    let source length name sq = DataSource(sq, name, length)
    let collection name (builder) (source : _ DataSource)  = {Source = source; Name = name; Value = builder(source.Value)}
    let collection' n name  (col : _) = source n name col |> collection name id 
    let array (source : _ DataSource) = collection "Array" (Seq.toArray >> seq) source 
    let iCollection source = collection "ICollection<T>" (Seq.toMutableListDirty >> seq) source 
    let iterateOnce source = collection "Seq IterateOnce" (Seq.iterateOnce) source 
    let basicCollections source = [array source; iCollection source; iterateOnce source]
        