namespace Imms.Tests.Integrity
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open Imms
open Imms.FSharp.Implementation.Compatibility
open ExtraFunctional
open System
open Imms.Abstract
open Imms
open Imms.FSharp.Implementation
open Imms.FSharp
open System.Diagnostics

[<AbstractClass>]
type ColWrapper<'v>(name : string) = 
    inherit TargetMetadata(name)
    interface 'v IReadOnlyCollection with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator
        member x.Count = x.Length
    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract Find : ('v -> bool) -> 'v option
    abstract Pick<'u> : ('v -> 'u option) -> 'u option
    abstract Fold : 'state -> ('state -> 'v -> 'state) -> 'state
    abstract Reduce : ('v -> 'v -> 'v) -> 'v
    abstract All : ('v -> bool) -> bool
    abstract Any : ('v -> bool) -> bool
    abstract Count : ('v -> bool) -> int
    abstract CanUseHeavily : string -> bool
    abstract Single : unit -> 'v
    abstract IsReference : bool
    default x.IsReference = false
    default x.Single() = raise <| OperationNotImplemented "Single"
    default x.CanUseHeavily name = false


[<AbstractClass>]
type SeqWrapper<'v>(name : string) = 
    inherit ColWrapper<'v>(name)
    //Core operations
    abstract AddLast : 'v -> SeqWrapper<'v>
    abstract AddFirst : 'v -> SeqWrapper<'v>
    abstract AddLastRange : 'v seq -> SeqWrapper<'v>
    abstract FindLast : ('v -> bool) -> 'v option
    abstract FindIndex : ('v -> bool) -> int option
    abstract AddFirstRange : 'v seq -> SeqWrapper<'v>
    abstract RemoveLast : unit -> SeqWrapper<'v>
    abstract Insert : int * 'v -> SeqWrapper<'v>
    abstract FindLastIndex : ('v -> bool) -> int option
    abstract RemoveFirst : unit -> SeqWrapper<'v>
    abstract Remove : int -> SeqWrapper<'v>
    abstract InsertRange : int * 'v seq -> SeqWrapper<'v>
    abstract Take : int -> SeqWrapper<'v>
    abstract Skip : int -> SeqWrapper<'v>
    abstract Range : st:int * en:int * ?step:int -> SeqWrapper<int>
    abstract member Item : int -> 'v
    abstract member Item : int * int -> SeqWrapper<'v>
    abstract First : 'v
    abstract Last : 'v
    abstract TryFirst : 'v option
    abstract TryLast : 'v option
    abstract IsEmpty : bool
    abstract Update : int * 'v -> SeqWrapper<'v>
    abstract Empty : SeqWrapper<'v>
    abstract Inner : seq<'v>
    //Extra operations
    abstract Reverse : unit -> SeqWrapper<'v>
    abstract TakeWhile : ('v -> bool) -> SeqWrapper<'v>
    abstract SkipWhile : ('v -> bool) -> SeqWrapper<'v>
    abstract ReduceBack : ('v -> 'v -> 'v) -> 'v
    abstract FoldBack : 'state -> ('state -> 'v -> 'state) -> 'state
    //implementations
    override x.Equals other = 
        match other with
        | :? SeqWrapper<'v> as other -> Seq.equals x other
        | _ -> false
    override x.GetHashCode() = Comparisons.CompuateSeqHashCode(x)
    default x.AddFirst _ = raise <|  OperationNotImplemented("AddFirst")
    default x.Insert (a,b) = raise <| OperationNotImplemented("Insert")
    default x.RemoveFirst() =raise <| OperationNotImplemented("RemoveFirst")
    default x.Remove _ = raise <| OperationNotImplemented("Remove")
    default x.Skip _ = raise <| OperationNotImplemented("Skip")
    default x.Item(a,b) = raise <| OperationNotImplemented("Slice")
    default x.InsertRange(a,b) = raise <| OperationNotImplemented("InsertRange")
    default x.AddFirstRange _ = raise <| OperationNotImplemented("AddFirstRange")
    member x.Get i = x.[i]
type SeqReferenceWrapper<'v>(inner : ImmutableList<'v>) as x= 
    inherit SeqWrapper<'v>("ImmutableList")
    let wrap x = SeqReferenceWrapper<_>(x) :> SeqWrapper<_>
    let fix i = if i < 0 then i + x.Length else i
    override x.GetEnumerator() = inner.GetEnumerator():>_
    override x.AddLast v = SeqReferenceWrapper(inner.Add v) :>_
    override x.SelfTest() = true
    override x.Range(st,en,?step) = [st..(step.Or 1)..en].ToImmutableList() |> wrap
    override x.AddLastRange vs = SeqReferenceWrapper(inner.AddRange vs) :>_
    override x.AddFirst v = SeqReferenceWrapper(inner.Insert(0, v)) :>_
    override x.AddFirstRange vs = SeqReferenceWrapper(inner.InsertRange(0, vs)) :>_
    override x.RemoveLast() = SeqReferenceWrapper(inner.RemoveAt(inner.Count - 1)) :>_
    override x.Insert(i,v) = 
        let i = if i < 0 then i + x.Length + 1 else i
        SeqReferenceWrapper(inner.Insert(i,v)) :>_
    override x.RemoveFirst() = SeqReferenceWrapper(inner.RemoveAt(0)) :>_
    override x.Remove(i) = 
        let i = fix i
        SeqReferenceWrapper(inner.RemoveAt(i)) :>_
    override x.InsertRange(i, vs) = 
        let i = if i < 0 then i + x.Length + 1 else i
        SeqReferenceWrapper(inner.InsertRange(i,vs)) :>_
    override x.FindLastIndex f =
        let r = inner.FindLastIndex(f |> toPredicate) 
        if r >= 0 then Some r else None

    override x.Take n = 
        if n >= x.Length then x:>_
        else
            SeqReferenceWrapper(inner.GetRange(0, min n x.Length)) :>_
    override x.Skip n = 
        if n >= x.Length then x.Empty
        else
            SeqReferenceWrapper(inner.GetRange(n, inner.Count - n)) :>_
    override x.Item(i1,i2) = 
        let i1, i2 = fix i1, fix i2
        let count = i2 - i1 + 1
        if count < 0 || i2 >= x.Length || i1 < 0 || i1 >= x.Length then Debugger.Break()
        SeqReferenceWrapper(inner.GetRange(i1, i2 - i1 + 1)):>_
    override x.Item ix= 
        let ix = fix ix
        inner.[ix]
    override x.Last = inner.[inner.Count - 1]
    override x.First = inner.[0]
    override x.Length = inner.Count
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = 
        let i = fix i
        SeqReferenceWrapper(inner.SetItem(i, v)) :>_
    override x.Empty = SeqReferenceWrapper(ImmutableList.Empty):>_
    override x.Inner = inner:>_
    override x.IsReference = true
    static member FromSeq s = SeqReferenceWrapper(ImmutableList.CreateRange(s)) :> SeqWrapper<_>

    override x.Fold initial f = x |> Seq.fold f initial
    override x.FoldBack initial f = x.Reverse() |> Seq.fold f initial
    override x.Find f = x |> Seq.tryFind f
    override x.FindLast f = x.Inner |> Seq.tryFindLast f
    override x.FindIndex f = x |> Seq.tryFindIndex f
    override x.Reduce f = x |> Seq.reduce f
    override x.ReduceBack f = x.Inner |> Seq.rev |> Seq.reduce f
    override x.All f = x.Inner |> Seq.forall( f)
    override x.Any f = x.Inner |> Seq.exists f
    override x.Count f = x.Inner |> Seq.countWhere f
    
    override x.Pick f = x |> Seq.tryPick f
    override x.TakeWhile f = 
        let count = inner.FindIndex(fun x -> f x |> not)
        if count = -1 then x :>_ else inner.GetRange(0, count) |> wrap
    override x.SkipWhile f = 
        let start = inner.FindIndex(fun x -> not(f x))
        if start = -1 then x.Empty else inner.GetRange(start, inner.Count - start) |> wrap
    override x.Reverse() = inner.Reverse() |> wrap

type ImmListWrapper<'v>(inner : ImmList<'v>)= 
    inherit SeqWrapper<'v>("ImmList")
    let wrap x = ImmListWrapper(x) :> SeqWrapper<_>
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = ImmListWrapper(inner.AddLast v) :>_
    override x.SelfTest() = true
    override x.AddLastRange vs = ImmListWrapper(inner.AddLastRange(vs)):>_
    override x.AddFirst v = ImmListWrapper(inner.AddFirst v) :>_
    override x.AddFirstRange vs = ImmListWrapper(inner.AddFirstRange(vs)) :>_
    override x.RemoveLast() = ImmListWrapper(inner.RemoveLast()) :>_
    override x.Insert(i,v) = ImmListWrapper(inner.Insert(i,v)) :>_
    override x.RemoveFirst() = ImmListWrapper(inner.RemoveFirst()) :>_
    override x.Remove(i) = ImmListWrapper(inner.RemoveAt(i)) :>_
    override x.InsertRange(i, vs) = ImmListWrapper(inner.InsertRange(i,vs)) :>_
    override x.Take n = ImmListWrapper(inner.Take n) :>_
    override x.Skip n = ImmListWrapper(inner.Skip n) :>_
    override x.Item(i1,i2) = ImmListWrapper(inner.[i1, i2]):>_
    override x.CanUseHeavily name = true
    override x.Item ix= inner.[ix]
    override x.First = inner.First
    override x.FindLastIndex f = inner.FindLastIndex (f |> toFunc1) |> fromOption
    override x.Range(st,en,step) = ImmList.ofSeq [st..(step.Or 1)..en] |> wrap
    override x.Last = inner.Last
    override x.Length = inner.Length
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = ImmListWrapper(inner.Update(i, v)) :>_
    override x.Empty = ImmListWrapper(ImmList.empty):>_
    override x.Inner = inner:>_
    override x.All f = inner |> ImmList.forAll f
    override x.Any f = inner |> ImmList.exists f
    override x.Count f = inner |> ImmList.count f
    override x.Pick f = inner |> ImmList.pick f
    override x.Find f = inner |> ImmList.find f
    override x.Single() = inner.Single()
    override x.Fold initial f = inner.Aggregate(initial, toFunc2 f)
    override x.Reduce f = inner |> ImmList.reduce f
    override x.FindIndex f = inner.FindIndex f |> fromOption
    override x.FindLast (f: _ -> _) = inner |> ImmList.findLast f
    override x.ReduceBack f = inner |> ImmList.reduceBack f
    override x.FoldBack initial f = inner |> ImmList.foldBack initial f
    override x.SkipWhile f = inner |> ImmList.skipWhile f |> wrap
    override x.TakeWhile f = inner |> ImmList.takeWhile f |> wrap
    override x.Reverse() = inner.Reverse() |> wrap
    static member FromSeq s= ImmListWrapper(ImmList.ofSeq s) :> SeqWrapper<_>

type ImmVectorWrapper<'v>(inner : ImmVector<'v>)= 
    inherit SeqWrapper<'v>("ImmVector")
    let wrap x = ImmVectorWrapper x :> SeqWrapper<_>
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = ImmVectorWrapper(inner.AddLast v) :>_
    override x.SelfTest() = true
    override x.AddLastRange vs = ImmVectorWrapper(inner.AddLastRange vs) :>_
    override x.RemoveLast() = ImmVectorWrapper(inner.RemoveLast()) :>_
    override x.InsertRange(i,vs) = ImmVectorWrapper(inner.InsertRange(i,vs)) :>_
    override x.AddFirstRange vs = ImmVectorWrapper(inner.AddFirstRange vs) :> _
    override x.Take n = ImmVectorWrapper(inner.Take n) :>_
    override x.Single() = inner.Single()
    override x.FindLastIndex f = inner.FindLastIndex (f |> toFunc1) |> fromOption
    override x.Range(st,en,?step)= ImmVector.ofSeq [st..(step.Or 1)..en] |> wrap
    override x.Skip n = ImmVectorWrapper(inner.Skip n) :>_
    override x.Item(i1,i2) = ImmVectorWrapper(inner.[i1, i2]):>_
    override x.Item ix= inner.[ix]
    override x.First = inner.First
    override x.Last = inner.Last
    override x.Length = inner.Length
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = ImmVectorWrapper(inner.Update(i, v)) :>_
    override x.Empty = ImmVectorWrapper(ImmVector.empty):>_
    override x.All (f : 'v -> bool) = inner |> ImmVector.forAll f
    override x.Any f = inner |> ImmVector.exists f
    override x.Count f = inner |> ImmVector.count f
    override x.Pick f = inner |> ImmVector.pick f
    override x.Find f = inner |> ImmVector.find f
    override x.Fold initial f = inner.Aggregate(initial, toFunc2 f)
    override x.Reduce f = inner |> ImmVector.reduce f
    override x.FindIndex f = inner |> ImmVector.findIndex f
    override x.FindLast (f: _ -> _) = inner |> ImmVector.findLast f
    
    override x.ReduceBack f = inner |> ImmVector.reduceBack f
    override x.FoldBack initial f = inner |> ImmVector.foldBack initial f
    override x.SkipWhile f = inner |> ImmVector.skipWhile f |> wrap
    override x.TakeWhile f = inner |> ImmVector.takeWhile f |> wrap
    override x.Reverse() = inner.Reverse() |> wrap
    override x.Inner = inner:>_
    
    static member FromSeq s = ImmVectorWrapper(ImmVector.ofSeq (s))  :> SeqWrapper<_>

type ColWrapper<'v> with
    member x.U_all f = 
        assert_operations_equal_1(x.All, f |> noCallsAfterReturn_1 false, x .|> Seq.forall, f)
    member x.U_any f =
        assert_operations_equal_1(x.Any, f |> noCallsAfterReturn_1 true, x .|> Seq.exists, f)
    member x.U_find f =
        assert_operations_equal_1(x.Find, f |> noCallsAfterReturn_1 true, x .|> Seq.tryFind, f)
    member x.U_count f =
        let len = ref 0
        let r = assert_operations_equal_1(x.Count, f |> countCalls_1 len, x .|> Seq.countWhere, f)
        assert_eq(!len, x.Length)
        r
    member x.U_fold init f = 
        let len = ref 0
        let r = assert_operations_equal_2(x.Fold init, f |> countCalls_2 len, x .|> (init .|> Seq.fold), f)
        assert_eq(!len, x.Length)
        r
    member x.U_reduce init f =
        let len = ref 0
        let r= assert_operations_equal_2(x.Reduce,f |> countCalls_2 len, x .|> Seq.reduce, f)
        assert_eq(!len, x.Length)
        r

    member x.U_pick f = 
        assert_operations_equal_1(x.Pick, f |> noCallsAfterReturn_1_f Option.isSome , x .|> Seq.tryPick, f)

    member x.Test_predicate f=
        let find = x.U_find f
        let pick = x.U_pick (fun x -> if f x then Some x else None)
        assert_eq(find,pick)
        let cn = x.U_count f
        let foldCn = x.U_fold 0 (fun st cur -> if f cur then st + 1 else st)
        assert_eq_all [cn; foldCn]
        if find.IsSome then
            assert_true(x.U_any f)
            assert_false(x.U_all (f >> not))
            assert_true(cn >= 1)
        else
            assert_eq(cn, 0)
            assert_false(x.U_any f)
            assert_true(x.U_all (f >> not))
        find        
    member x.U_single = 
        let r = x.Single()
        assert_eq_all [r; x |> Seq.head]

type SeqWrapper<'v> with
    member x.U_get i =
        assert_false(x.IsEmpty)
        let realI = if i < 0 then i + x.Length else i
        let negI = if i < 0 then i else i - x.Length
        assert_eq(x.[realI], x.[negI])
        x.[i]
    member x.U_first =
        assert_false(x.IsEmpty)
        assert_eq(x.First, x.TryFirst.Value)
        assert_eq(x.U_get 0, x.First)
        x.First
    member x.U_last = 
        assert_false(x.IsEmpty)
        assert_eq(x.Last, x.TryLast.Value)
        assert_eq(x.Last, x.U_get -1)
        x.Last                          
    member x.U_index_not_found i =
        if not(x.IsReference) then
            assert_ex_arg_out_of_range <| fun()-> x.[i]
            assert_ex_arg_out_of_range <| fun()-> x.Update(i,Unchecked.defaultof<'v>)
            assert_ex_arg_out_of_range <| fun()-> x.Remove(i)
            assert_ex_arg_out_of_range <| fun() -> x.[i,i]
            assert_ex_arg_out_of_range <| fun() -> x.[0,i]
            if i < 0 then
                assert_ex_arg_out_of_range <| fun() -> x.Take i
                assert_ex_arg_out_of_range <| fun() -> x.Skip i
            if (i > 0 && i > x.Length) || (i < 0 && i < -x.Length-1) then   
                assert_ex_arg_out_of_range <| fun() -> x.Insert(i,Unchecked.defaultof<'v>)
                assert_ex_arg_out_of_range <| fun()-> x.InsertRange(i, [| |])
            else
                assert_no_ex <| fun () -> x.Insert(i,Unchecked.defaultof<'v>)
                assert_no_ex <| fun()-> x.InsertRange(i, [| |])
    member x.U_single = 
        let r = x.Single()
        assert_eq_all [r; x.U_first]
    member x.U_add_last v  = 
        let next = x.AddLast v
        let hasFirst = not <| x.IsEmpty
        assert_eq(next.Length, x.Length+1)
        assert_eq(next.U_last, v)
        if hasFirst then
            assert_eq(next.U_first, x.U_first)
        else
            assert_eq(next.U_first,  v)
        next
        
    member x.U_add_first v  =
        let next = x.AddFirst v
        let hasLast = not <| x.IsEmpty
        assert_eq(next.Length,x.Length+1)
        assert_eq(next.U_first, v)
        if hasLast then
            assert_eq(next.U_last, x.U_last)
        else
            assert_eq(next.U_last, v)
        next

    member x.U_add_last_range vs: SeqWrapper<'v>=
        let next = x.AddLastRange (vs |> Seq.canEnsureIterateOnce)
        let hasFirst = not <| x.IsEmpty
        let arr = vs |> Seq.asArray
        assert_eq(next.Length, x.Length + arr.Length)
        if arr.Length > 0 then
            assert_eq(next.U_last, arr.[arr.Length-1])
            assert_eq(next.U_get (x.Length), arr.[0])
            if hasFirst then
                assert_eq(next.U_first, x.U_first)
            else
                assert_eq(next.U_first, arr.[0])
        next

    member x.U_add_first_range vs  : SeqWrapper<'v>=
        let next = x.AddFirstRange(vs |> Seq.canEnsureIterateOnce)
        let hasLast = not <| x.IsEmpty
        let arr = vs |> Seq.asArray
        assert_eq(next.Length, x.Length + arr.Length)
        if arr.Length > 0 then
            assert_eq(next.U_first, arr.[0])
            assert_eq(next.U_get (arr.Length-1), arr.[arr.Length-1])
            if hasLast then
                assert_eq(next.U_last, x.U_last)
        next

    member x.U_drop_last  =
        let next = x.RemoveLast()
        assert_eq(next.Length, x.Length-1)
        //next.U_index_not_found (x.Length-1)
        if not (x.Length = 1) then
            assert_eq(next.U_last, x.U_get -2)
        else
            assert_true(next.IsEmpty)
        if next.Length = 1 then
            assert_eq(next.U_first, next.U_last)
        next

    member x.U_drop_first  =
        let next = x.RemoveFirst()
        assert_eq(next.Length, x.Length-1)
        
        if not (x.Length = 1) then
            assert_eq(next.U_first, x.U_get 1)
        else
            assert_true(next.IsEmpty)
        if next.Length = 1 then
            assert_eq(next.U_last, next.U_first)
        next

    member x.U_update i v  =
        let next = x.Update(i,v)
        assert_eq(next.Length,x.Length)
        assert_eq(next.U_get i, v)
        let next = x.Update(-x.Length+i,v)
        assert_eq(next.U_get i, v)
        let next' = next.Update(i,x.[i])
        assert_eq(next'.U_get i, x.U_get i)
        assert_eq(next'.Length, next.Length)
        next

    member x.U_insert i v  =
        let realI = if i < 0 then i + x.Length else i
        let next = x.Insert(i,v)
        let next' = x.Insert(-x.Length-1+i,v) // (x.Length) - x = -1
        assert_eq(next.Length, x.Length+1)
        assert_eq(next.Length, next'.Length)
        assert_eq(next.U_get i, next'.U_get i)
        if realI < x.Length then
            assert_eq(next.U_get (i+1), x.U_get i)
        elif realI = x.Length then
            assert_eq(next.U_last, v)
        if x.IsEmpty then
            assert_eq(next.U_first, next.U_last)
            assert_false(next.IsEmpty)
        if realI > 0 then
            assert_eq(next.U_get (i-1), next.U_get (i-1))
        elif realI = 0 then
            assert_eq(next.U_first, v)
        if x.IsEmpty then
            assert_eq(next.U_first, next.U_last)
        next'

    member x.U_remove i  = 
        let realI = if i < 0 then i + x.Length else i
        let next = x.Remove(i)
        let next' = x.Remove(-x.Length+i)
        assert_eq(next.Length,x.Length-1)
        assert_eq(next.Length,next'.Length)
       // next.U_index_not_found (x.Length-1)
        if realI < x.Length - 1 then
            assert_eq(next.U_get i, x.U_get (i+1))
            assert_eq(next'.U_get i, x.U_get (i+1))
        if realI > 0 then
            assert_eq(next.U_get (i-1), x.U_get (i-1))
            assert_eq(next'.U_get (i-1), x.U_get (i-1))
        elif x.Length = 1 then
            assert_true(next.IsEmpty)
            assert_true(next'.IsEmpty)
        else
            assert_eq(next.U_first, x.U_get 1)
            assert_eq(next'.U_first, x.U_get 1)
        next

    member x.U_take n  = 
        if n > 0 then
            assert_ex_arg_out_of_range <| fun() -> x.Take (-n)
        let next = x.Take n
        next.U_index_not_found n
        if n >= x.Length then
            assert_eq(next.Length, x.Length)
            assert_eq(next, x)
        else 
            assert_eq(next.Length, n)
            if n > 0 then
                assert_eq(next.U_last, x.U_get (n-1))
                assert_eq(next.U_first, x.U_first)
            else
                assert_true(next.IsEmpty)
        next

    member x.U_skip n  =
        if n > 0 then
            assert_ex_arg_out_of_range <| fun() -> x.Skip (-n) 
        let next = x.Skip n
        next.U_index_not_found (x.Length-n)
        if n >= x.Length then
            assert_eq(next.Length, 0)
            assert_true(next.IsEmpty)
        else
            assert_eq(next.Length, x.Length - n)
            if n > 0 then
                assert_eq(next.U_first, x.U_get n)
                assert_eq(next.U_last, x.U_last)
        next

    member x.U_slice (i1,i2)  =
        let next = x.[i1,i2]
        let next' = x.[-x.Length + i1, -x.Length + i2]
        let rI1 = if i1 < 0 then i1 + x.Length else i1
        let rI2 = if i2 < 0 then i2 + x.Length else i2
        assert_eq(next.Length, rI2 - rI1 + 1)
        assert_eq(next'.Length, rI2 - rI1 + 1)
        assert_eq(next.U_first, x.U_get rI1)
        assert_eq(next.U_last, x.U_get rI2)
        assert_eq(next'.U_first, x.U_get rI1)
        assert_eq(next'.U_last, x.U_get rI2)
        next

    member x.U_insert_range i vs  =
        let next = x.InsertRange(i,(vs |> Seq.canEnsureIterateOnce))
        let next' = if i >= x.Length then next else x.InsertRange(-x.Length-1+i, vs)
        let rI = if i < 0 then i + x.Length else i
        let arr = vs |> Seq.asArray
        assert_eq(next.Length, x.Length + arr.Length)
        assert_eq(next'.Length, x.Length + arr.Length)
        if arr.Length > 0 then
            assert_eq(next.U_get i, arr.[0])
            assert_eq(next.U_get (i+arr.Length - 1), arr.[arr.Length-1])
            assert_eq(next'.U_get i, arr.[0])
            assert_eq(next'.U_get (i+arr.Length - 1), arr.[arr.Length-1])
            if rI > 0 then
                assert_eq(next.U_first, x.U_first)
                assert_eq(next.U_get (rI-1), x.U_get (rI-1))
                assert_eq(next'.U_first, x.U_first)
                assert_eq(next'.U_get (rI-1), x.U_get (rI-1))
            else
                assert_eq(next.U_first, arr.[0])
                assert_eq(next'.U_first, arr.[0])
            if rI < x.Length then
                assert_eq(next.U_last, x.U_last)
                assert_eq(next.U_get (rI+arr.Length), x.U_get (rI))
                assert_eq(next'.U_last, x.U_last)
                assert_eq(next'.U_get (rI+arr.Length), x.U_get (rI))
        next 

    ///Expects an empty collection and verifies it has all the properties of such
    member x.U_ex_is_empty  =
        if not x.IsReference then
            assert_true(x |> Seq.isEmpty)
            assert_ex_inv_op <| fun() -> x.First
            assert_ex_inv_op <| fun() -> x.Last
            assert_ex_inv_op <| fun() -> x.Reduce(fun a b -> a)
            assert_ex_inv_op <| fun() -> x.ReduceBack(fun a b -> a)
            assert_ex_inv_op <| x.RemoveLast
            assert_ex_inv_op <| x.RemoveFirst
            assert_ex_inv_op <| x.Single
            is_none(x.TryFirst)
            is_none(x.TryLast )
            assert_eq(x.Length,0)
            assert_false(x.Any(fun x -> true))
            assert_true(x.All(fun x -> false))
            assert_eq(x.Count(fun x -> true), 0)
            
            is_none(x.Find(fun x -> true))
            is_none(x.FindIndex(fun x -> true))
            is_none(x.FindLast(fun x -> true))
            is_none(x.Pick(fun x -> Some x))
            assert_eq(x.Fold 0 (fun st cur -> 1), 0)
            assert_eq(x.FoldBack 0 (fun st cur -> 1), 0)
            assert_eq(x.Reverse(), x)
            assert_eq(x.Skip 1, x)
            assert_eq(x.Take 1, x)
            assert_eq(x.TakeWhile(fun x -> true), x)
            assert_eq(x.SkipWhile(fun x -> false), x)
            x.U_index_not_found 0 |> ignore
            x.U_index_not_found -1 |> ignore
       
    member x.U_reverse  =
        let r = x.Reverse()
        assert_eq(r.Length, x.Length)
        if x.IsEmpty then
            assert_true(r.IsEmpty)
        else
            assert_eq(x.U_first, r.U_last)
            assert_eq(x.U_last, r.U_first)
        if x.Length > 1 then
            assert_eq(x.U_get 1, r.U_get -2)
        r
    member x.U_ex_has_one_element  =
        if not x.IsReference then
            assert_eq(x.Length, 1)
            assert_eq_all [x.U_first; x.U_last; x.Single(); x.U_get 0; x.U_get -1]
            assert_eq_all [x.Take 1; x.Skip 0; x.[0,0]; x.[0,-1]; x.[-1,0]; x]

    member x.U_find_index f = 
        assert_operations_equal_1(x.FindIndex, f |> noCallsAfterReturn_1 true, x .|> Seq.tryFindIndex, f)

    member x.U_findl f  =
        assert_operations_equal_1(x.FindLast, f |> noCallsAfterReturn_1 true, x .|> Seq.tryFindLast, f)

    member x.U_findl_index f  =
        assert_operations_equal_1(x.FindLastIndex, f |> noCallsAfterReturn_1 true, x.|> Seq.tryFindLastIndex, f)

    member x.U_skipWhile f  = 
        let ix = x.U_find_index (f >> not)
        let len = ref 0
        let skip1 = x.SkipWhile (f |> countCalls_1 len |> noCallsAfterReturn_1 false)
        if ix.IsNone then
            skip1.U_ex_is_empty
            assert_eq(!len, x.Length)
        else
            assert_eq(skip1.Length, x.Length - ix.Value)
            assert_eq(x.U_get (ix.Value), skip1.U_first)
            assert_eq(skip1.U_last, x.U_last)
            assert_eq(!len, x.Length - skip1.Length + 1)
        skip1

    member x.U_takeWhile f  =
        let ix = x.U_find_index (f >> not)
        let len = ref 0
        let take = x.TakeWhile (f |> countCalls_1 len |> noCallsAfterReturn_1 false)

        if ix.IsNone then
            assert_eq(take.Length, x.Length)
            assert_eq(take.U_first, x.U_first)
            assert_eq(take.U_last, x.U_last)
            assert_eq(!len, x.Length)
        elif ix.Value = 0 then
            take.U_ex_is_empty
            assert_eq(!len, 1)
        else
            assert_eq(ix.Value, take.Length)
            assert_eq(x.U_get (ix.Value - 1), take.U_last)
            assert_eq(x.U_first, take.U_first)
            assert_eq(!len, take.Length + 1)
        take

    member x.U_foldBack init f  =
        let len = ref 0
        let r=  assert_operations_equal_2(x.FoldBack init, f |> countCalls_2 len,x .|> (Seq.foldBack init), f)
        assert_eq(!len,x.Length)
        r

    member x.U_reduceBack f =
        let len = ref 0
        let r = assert_operations_equal_2(x.ReduceBack, f |> countCalls_2 len,x .|> Seq.reduceBack, f)
        assert_eq(!len,x.Length)
        r

    member x.Test_predicate f  =
        let find, findI, findl, findlI = x.U_find f, x.U_find_index f, x.U_findl f, x.U_findl_index f
        assert_eq_all [find; findI |> Option.map (x.Item); x.U_pick (fun x -> if f x then Some x else None)]
        assert_eq_all [findl; findlI |> Option.map(x.Item)]
        let cn = x.U_count f
        let foldCn = x.U_fold 0 (fun st cur -> if f cur then st + 1 else st)
        let foldbCn = x.U_foldBack 0 (fun st cur -> if f cur then st + 1 else st)
        assert_eq_all [cn; foldCn; foldbCn]
        if find.IsSome then
            assert_true(x.U_any f)
            assert_false(x.U_all (f >> not))
            assert_true(cn >= 1)
            if cn > 1 then
                assert_neq(findI, findlI)
            else
                assert_eq(findI, findlI)
                assert_eq(find, findl)
        else
            assert_eq(cn, 0)
            assert_eq(findl, find)
            assert_eq(findI, findlI)
            assert_false(x.U_any f)
            assert_true(x.U_all (f >> not))
        find
        
    member x.Run_fold f  =
        x.U_fold, x.U_foldBack |> ignore
        
    member x.Run_reduce f  =
        x.U_reduce f, x.U_reduceBack f

