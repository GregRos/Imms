namespace Funq.Tests.Integrity
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open Funq.FSharp.Implementation.Compatibility
open System
open Funq.Abstract
open ExtraFunctional
open Funq
open Funq.FSharp.Implementation
open Funq.FSharp
open System.Diagnostics



[<AbstractClass>]
type SetWrapper<'v when 'v : comparison>(name : string) = 
    inherit ColWrapper<'v>(name)
    interface 'v seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator

    abstract Add : 'v -> SetWrapper<'v>
    abstract Remove : 'v -> SetWrapper<'v>
    abstract ByOrder : int -> 'v
    abstract Contains : 'v -> bool
    abstract MaxItem : 'v 
    abstract IsOrdered : bool
    abstract MinItem : 'v
    abstract IsEmpty : bool
    abstract Empty : SetWrapper<'v>
    abstract EmptyWith : KeySemantics<'v> -> SetWrapper<'v>
    abstract Union : seq<'v> -> SetWrapper<'v>
    abstract Intersect : seq<'v> -> SetWrapper<'v>
    abstract Except : seq<'v>  -> SetWrapper<'v>
    abstract Difference : seq<'v>  -> SetWrapper<'v>
    abstract ExceptInverse : seq<'v> -> SetWrapper<'v>
    abstract ByArbitraryOrder : int -> 'v
    abstract IsSupersetOf : seq<'v> -> bool
    abstract IsProperSupersetOf : seq<'v> -> bool
    abstract IsSubsetOf : seq<'v> -> bool
    abstract IsProperSubsetOf : seq<'v> -> bool
    abstract SetEquals : seq<'v> -> bool
    abstract IsDisjointWith : seq<'v> -> bool
    abstract RelatesTo : seq<'v> -> SetRelation
    default x.ByArbitraryOrder i = x.ByOrder i
    default x.ByOrder _ = raise <|  OperationNotImplemented("ByOrder")
    default x.MaxItem = raise <|  OperationNotImplemented("MaxItem")
    default x.MinItem = raise <|  OperationNotImplemented("MinItem")
    default x.Union _ = raise <|  OperationNotImplemented("Union")
    default x.Intersect _ = raise <|  OperationNotImplemented("Intersect")
    default x.Except _ = raise <|  OperationNotImplemented("Except")
    default x.Difference _ = raise <|  OperationNotImplemented("Difference")
    override x.Equals other = 
        match other with
        | :? SetWrapper<'v> as other -> 
            let eq1 = x.SetEquals(other) && other.Length = x.Length
            if x.IsOrdered && other.IsOrdered then
                eq1 && Seq.equals x other
            else
                eq1
        | _ -> failwith "Unexpected equality!"
    override x.GetHashCode() = failwith "Don't call this method"
    
type ReferenceSetWrapper<'v when 'v : comparison>(Inner : Set<ComparableKey<'v>>, Ordering : FunqOrderedSet<ComparableKey<'v>>, Comparer : KeySemantics<'v>) = 
    inherit SetWrapper<'v>("FSharpSet")    
    static let wrap cmp ord x  = ReferenceSetWrapper<'v>(x, ord, cmp) :> SetWrapper<'v>
    static let unwrapKey (x : ComparableKey<_>) = x.Value
    let wrapInst ord x = wrap Comparer ord x 
    let wrapKey x = Comparer.Wrap x
    member val Inner = Inner
    member val Ordering = Ordering
    override x.GetEnumerator() = (Inner :> seq<_>) |> Seq.map (fun x -> x.Value) |> Seq.getEnumerator
    override x.Add v = 
        let v = wrapKey v
        if Inner.Contains v then x:>_ else Inner.Add v |> wrapInst (Ordering.Add v)
    override x.IsReference = true
    override x.Remove v = 
        let v = wrapKey v
        if Inner.Contains v then Inner.Remove v |> wrapInst (Ordering.Remove v) else x:>_
    override x.Contains v = Inner.Contains (wrapKey v)
    override x.Length = Ordering.Length
    override x.SelfTest() = true
    override x.Empty = Set.empty |> wrapInst (FunqOrderedSet.empty)
    override x.IsEmpty = Inner.IsEmpty
    override x.EmptyWith cmp = ReferenceSetWrapper(Set.empty, FunqOrderedSet.empty, cmp) :> _
    override x.IsOrdered = true
    override x.Union st = 
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.union other.Inner x.Inner |> wrapInst (Ordering.Union other.Ordering)
        | _ -> 
            let st = st |> Seq.map wrapKey |> Set.ofSeq
            Set.union st (x.Inner) |> wrapInst (Ordering.Union st)
    override x.Intersect st =
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.intersect other.Inner x.Inner |> wrapInst (Ordering.Intersect (other.Inner))
        | _ ->
            let st = st |> Seq.map wrapKey |> Set.ofSeq
            Set.intersect st (x.Inner) |> wrapInst (Ordering.Intersect st)
    override x.Except st =
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.difference x.Inner other.Inner |> wrapInst (Ordering.Except (other.Inner))
        | _ -> 
            let st = st |> Seq.map wrapKey |> Set.ofSeq
            Set.difference (x.Inner) st |> wrapInst (Ordering.Except st)
    override x.Difference st =
        let st = st |> Seq.asArray
        x.Except(st).Union(x.ExceptInverse(st))
    override x.ExceptInverse sq =
        match sq with
        | :? ReferenceSetWrapper<'v> as other -> Set.difference (other.Inner) (Inner) |> wrapInst (Ordering.ExceptInverse other.Inner)
        | _ -> 
            let st = sq |> Seq.map wrapKey |> Set.ofSeq
            Set.difference st Inner |> wrapInst (Ordering.ExceptInverse st)
    override x.ByOrder i = 
        let o = Ordering.ByOrder i
        assert_true(Inner.Contains o)
        o.Value
    override x.MinItem = 
        let mn = Inner.MinimumElement
        assert_eq(mn, Ordering.MinItem)
        mn.Value
    override x.MaxItem = 
        let mx = Inner.MaximumElement
        assert_eq(mx, Ordering.MaxItem)
        mx.Value
    override x.All f = Inner |> Seq.forall (unwrapKey >> f)
    override x.Any f = Inner |> Seq.exists (unwrapKey >> f)
    override x.Count f = Inner |> Seq.fold (unwrapKey >>. fun st cur -> if f cur then st + 1 else st) 0
    override x.Pick f = Inner |> Seq.tryPick (unwrapKey >> f)
    override x.Find f = Inner |> Seq.tryFind (unwrapKey >> f) |> Option.map unwrapKey
    override x.Fold initial f = Inner |> Seq.fold (unwrapKey >>. f) initial
    override x.Reduce f = Inner |> Seq.reduce (fun st cur -> f (unwrapKey st) (unwrapKey cur) |> wrapKey) |> unwrapKey 
    override x.IsSubsetOf vs = Inner.IsSubsetOf (vs |> Seq.map (wrapKey)|> Set.ofSeq)
    override x.IsProperSubsetOf vs = Inner.IsProperSubsetOf (vs |> Seq.map wrapKey |> Set.ofSeq)
    override x.IsSupersetOf vs = Inner.IsSupersetOf (vs |> Seq.map wrapKey |>  Set.ofSeq)
    override x.IsProperSupersetOf vs = Inner.IsProperSupersetOf (vs |> Seq.map wrapKey |> Set.ofSeq)
    override x.SetEquals vs = Inner.Equals(vs |> Set.ofSeq)
    override x.IsDisjointWith vs = 
        Inner |> Set.intersect (vs |> Seq.map wrapKey |> Set.ofSeq) |> Set.count = 0
    override x.RelatesTo vs = 
        let vs = vs |> Seq.map (wrapKey) |> Set.ofSeq
        if Inner.IsEmpty && vs.IsEmpty then SetRelation.Equal ||| SetRelation.Disjoint
        elif Inner.IsEmpty then SetRelation.ProperSubsetOf ||| SetRelation.Disjoint
        elif vs.IsEmpty then SetRelation.ProperSupersetOf ||| SetRelation.Disjoint
        else
            let intersect = Inner |> Set.intersect vs
            let len, myCount, vsCount = intersect.Count, Inner.Count, vs.Count
            if len = 0 then SetRelation.Disjoint
            elif len = myCount && len = vsCount then SetRelation.Equal
            elif len = myCount then SetRelation.ProperSubsetOf
            elif len = vsCount then SetRelation.ProperSupersetOf
            else SetRelation.None    

    static member FromSeqWith (cmp : KeySemantics<_>) s  = 
        let arr = s |> Seq.map (cmp.Wrap) |> Seq.toArray
        arr  |> Set.ofSeq |> wrap cmp (arr |> Seq.disableIterateOnce |> FunqOrderedSet.ofSeq) 
        
    static member FromSeq s  = 
        let dSemantics = defaultKeySemantics
        let arr = s |> Seq.map (dSemantics.Wrap) |> Seq.toArray
        arr |> Set.ofSeq |> wrap (defaultKeySemantics) (arr |> Seq.disableIterateOnce |> FunqOrderedSet.ofSeq) 



type FunqSetWrapper<'v when 'v : comparison>(Inner : FunqSet<'v>, Ordering : FunqOrderedSet<'v>) = 
    inherit SetWrapper<'v>("FunqSet")
    static let wrap ord x  = FunqSetWrapper<'v>(x, ord) :> SetWrapper<'v>
    let unwrap (vs : 'v seq) = 
        match vs with
        | :? FunqSetWrapper<'v> as wrapped -> wrapped.Inner :> _ seq
        | sq -> sq
    member val Inner = Inner
    member val Ordering = Ordering
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap (Ordering.Add v)
    override x.Remove v = Inner.Remove v |> wrap (Ordering.Remove v)
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Length
    override x.EmptyWith eq = FunqSet.emptyWith eq |> wrap (FunqOrderedSet.emptyWith eq)
    override x.Empty = FunqSet.empty|> wrap (FunqOrderedSet.empty)
    override x.ByArbitraryOrder i = Ordering.ByOrder i
    override x.SelfTest() = 
        let distinctCount = x |> Seq.distinct |> Seq.smartLength
        distinctCount = x.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.IsOrdered = false
    override x.Union other = 
        let res = other |> unwrap |> Inner.Union
        res |> wrap (Ordering.Union (other |> Seq.disableIterateOnce))
    override x.Intersect other = 
        let res = other |> unwrap |> Inner.Intersect
        res |> wrap (Ordering.Intersect (other |> Seq.disableIterateOnce))
    override x.Except other = 
        let res = other |> unwrap |> Inner.Except
        res |> wrap (Ordering.Except (other |> Seq.disableIterateOnce))
    override x.Difference other = 
        let res = other |> unwrap |> Inner.Difference
        res |> wrap (Ordering.Difference (other |> Seq.disableIterateOnce))
    override x.ExceptInverse other = 
        let res = other |> unwrap |> Inner.ExceptInverse
        res |> wrap (Ordering.ExceptInverse (other |> Seq.disableIterateOnce))
    override x.All f = Inner |> FunqSet.forAll f
    override x.Any f = Inner |> FunqSet.exists f
    override x.Count f = Inner |> FunqSet.count f
    override x.Pick f = Inner |> FunqSet.pick f
    override x.Find f = Inner |> FunqSet.find f
    override x.Fold initial f = Inner |> FunqSet.fold initial f
    override x.Reduce f = Inner |> FunqSet.reduce f
    override x.IsSubsetOf vs = vs |> unwrap |> Inner.IsSubsetOf
    override x.IsProperSubsetOf vs = vs |> unwrap |> Inner.IsProperSubsetOf
    override x.IsSupersetOf vs = vs |> unwrap |> Inner.IsSupersetOf
    override x.IsProperSupersetOf vs = vs |> unwrap |> Inner.IsProperSupersetOf
    override x.IsDisjointWith vs = vs |> unwrap |> Inner.IsDisjointWith
    override x.RelatesTo vs = vs |> unwrap |> Inner.RelatesTo
    override x.SetEquals vs = vs |> unwrap |> Inner.SetEquals
    static member FromSeqWith cmp s = FunqSet.ofSeqWith cmp s |> wrap (s |> Seq.disableIterateOnce |> FunqOrderedSet.ofSeqWith cmp)
    static member FromSeq s = FunqSet.ofSeq(s) |> wrap (FunqOrderedSet.ofSeq (s |> Seq.disableIterateOnce))
    

type FunqOrderedSetWrapper<'v when 'v : comparison>(Inner : FunqOrderedSet<'v>) = 
    inherit SetWrapper<'v>("FunqOrderedSet")
    static let wrap x = FunqOrderedSetWrapper<'v>(x) :> SetWrapper<'v>
    let unwrap (vs : 'v seq) = 
        match vs with
        | :? FunqOrderedSetWrapper<'v> as wrapped -> wrapped.Inner :> _ seq
        | sq -> sq
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Remove v = Inner.Remove v |> wrap
    override x.Contains v = Inner.Contains v
    override x.IsOrdered = true
    override x.Length = Inner.Length
    override x.Empty = FunqOrderedSet.empty|> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.SelfTest() =     
        let mutable i = 0
        let mutable okay = true
        let distinctCount = x |> Seq.distinct |> Seq.smartLength
        if not <| (distinctCount = x.Length) then false
        else
            for item in x do
                let atIndex = x.ByOrder i
                okay <- okay && obj.Equals(atIndex, item)
                i <- i + 1
            okay
    override x.Union other = other |> unwrap |> Inner.Union |> wrap
    override x.EmptyWith cmp = FunqOrderedSet.emptyWith cmp |> wrap
    override x.Intersect other = other |> unwrap |> Inner.Intersect |> wrap
    override x.Except other = other |> unwrap |> Inner.Except |> wrap
    override x.Difference other = other |> unwrap |> Inner.Difference |> wrap
    override x.ExceptInverse other = other |> unwrap |> Inner.ExceptInverse |> wrap
    override x.IsSubsetOf vs = vs |> unwrap |> Inner.IsSubsetOf
    override x.IsProperSubsetOf vs = vs |> unwrap |> Inner.IsProperSubsetOf
    override x.IsSupersetOf vs = vs |> unwrap |> Inner.IsSupersetOf
    override x.IsProperSupersetOf vs = vs |> unwrap |> Inner.IsProperSupersetOf
    override x.IsDisjointWith vs = vs |> unwrap |> Inner.IsDisjointWith
    override x.RelatesTo vs = vs |> unwrap |> Inner.RelatesTo
    override x.SetEquals vs = vs |> unwrap |> Inner.SetEquals
    override x.MinItem = Inner.MinItem
    override x.MaxItem = Inner.MaxItem
    override x.ByOrder i= Inner.ByOrder i
    override x.All f = Inner |> FunqOrderedSet.forAll f
    override x.Any f = Inner |> FunqOrderedSet.exists f
    override x.Count f = Inner |> FunqOrderedSet.count f
    override x.Pick f = Inner |> FunqOrderedSet.pick f
    override x.Find f = Inner |> FunqOrderedSet.find f
    override x.Fold initial f = Inner |> FunqOrderedSet.fold initial f
    override x.Reduce f = Inner |> FunqOrderedSet.reduce f
    static member FromSeq s = FunqOrderedSet.ofSeq s |> wrap
    static member FromSeqWith cmp s = FunqOrderedSet.ofSeqWith cmp s |> wrap

type SetWrapper<'v when 'v : comparison> with
    member x.U_add v =
        let next = x.Add v
        assert_true(next.Contains v)
        if x.Contains v then
            assert_eq(x.Length, next.Length)
        else
            assert_eq(next.Length, x.Length + 1)
        next

    member x.U_remove v =
        let next = x.Remove v
        assert_false(next.Contains v)
        if x.Contains v then
            assert_eq(x.Length - 1, next.Length)
        else
            assert_eq(x.Length, next.Length)
        next

    member x.U_union (sq : _ seq)  = 
        let next = x.Union (sq |> Seq.canEnsureIterateOnce)
        let mutable increase = sq |> Seq.countWhere (x.Contains >> not)
        assert_eq(next.Length, increase + x.Length)
        for item in sq do
            assert_true(next.Contains item)
        next

    member x.U_except sq = 
        let next = x.Except (sq |> Seq.canEnsureIterateOnce)
        let mutable decrease = sq |> Seq.countWhere (x.Contains)
        assert_eq(next.Length, -decrease + x.Length)
        for item in sq do
            assert_false(next.Contains item)
        next

    member x.U_intersect sq=
        let next = x.Intersect (sq |> Seq.canEnsureIterateOnce)
        let count = sq |> Seq.countWhere (x.Contains)
        assert_eq(next.Length, count)
        next

    member x.U_except_inverse sq =
        let next = x.ExceptInverse((sq |> Seq.canEnsureIterateOnce))
        let count = sq |> Seq.countWhere (x.Contains >> not)
        assert_eq(next.Length, count)
        next

    member x.U_difference sq  =
        let next = x.Difference (sq |> Seq.canEnsureIterateOnce)
        let sqSet = sq |> Set.ofSeq
        let count1 = sqSet |> Seq.countWhere (x.Contains >> not)
        let count2 = x |> Seq.countWhere (sqSet.Contains >> not)
        assert_eq(next.Length, count1+ count2)
        next

    member x.U_isSupersetOf sq  =
        let r = x.IsSupersetOf (sq |> Seq.canEnsureIterateOnce)
        assert_eq(sq |> Seq.forall (x.Contains), r)
        r

    member x.U_isProperSupersetOf (sq : _ seq) =
        let r = x.IsProperSupersetOf (sq |> Seq.canEnsureIterateOnce)
        let len = sq |> Seq.smartLength
        if r then
            assert_true(sq |> Seq.forall (x.Contains))
            assert_true(len < x.Length)
            assert_true(x.U_isSupersetOf sq)
            assert_false(x.U_isProperSubsetOf sq)
        else
            assert_false(len > x.Length && sq |> Seq.forall (x.Contains))
        r

    member x.U_isSubsetOf (sq : _ seq)=
        let r = x.IsSubsetOf (sq |> Seq.canEnsureIterateOnce)
        let sq = sq |> Set.ofSeq
        let r2 = x.All(sq.Contains)
        assert_eq(r, r2)
        r

    member x.U_isProperSubsetOf (sq : _ seq) =
        let r = x.IsProperSubsetOf (sq |> Seq.canEnsureIterateOnce)
        let sq = sq |> Set.ofSeq
        let r2 = x |> Seq.forall (sq.Contains)
        let len = sq.Count
        if r then assert_true(x.U_isSubsetOf sq)
        assert_eq(r, r2 && len > x.Length)
        if r then assert_false(x.U_isProperSupersetOf sq)
        r

    member x.U_isDisjointWith (sq : _ seq) =
        let r = x.IsDisjointWith (sq |> Seq.canEnsureIterateOnce)
        let r2 = sq |> Seq.forall (x.Contains >> not)
        assert_eq(r, r2)
        r

    member x.U_isEqual sq  =
        let r = x.SetEquals (sq |> Seq.canEnsureIterateOnce)
        let len = sq |> Seq.smartLength
        let r2 = sq |> Seq.forall (x.Contains)
        assert_eq(r2 && len = x.Length, r)
        if r then
            assert_true(x.U_isSupersetOf sq)
            assert_true(x.U_isSubsetOf sq)
            assert_false(x.U_isProperSubsetOf sq)
            assert_false(x.U_isProperSupersetOf sq)
        r

    member x.U_relatesTo sq  =
        let r = x.RelatesTo (sq |> Seq.canEnsureIterateOnce)
        let isDisjoint = x.U_isDisjointWith sq
        let isPrSubset = x.U_isProperSubsetOf sq
        let isPrSuperset = x.U_isProperSupersetOf sq
        let isEq = x.U_isEqual sq
        if SetRelation.Disjoint |> r.HasFlag then assert_true(isDisjoint) else assert_false(isDisjoint)
        if SetRelation.ProperSubsetOf |> r.HasFlag then assert_true(isPrSubset) else assert_false(isPrSubset)
        if SetRelation.ProperSupersetOf |> r.HasFlag then assert_true(isPrSuperset) else assert_false(isPrSuperset)
        if SetRelation.Equal |> r.HasFlag then assert_true(isEq) else assert_false(isEq)
        r
