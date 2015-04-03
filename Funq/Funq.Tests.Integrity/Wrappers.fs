[<AutoOpen>]
module Funq.Tests.Integrity.Wrappers
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open Funq.Collections
open Funq.FSharp.Implementation.Compatibility
open System
open Funq.Abstract
open Funq
open Funq.FSharp.Implementation

[<AbstractClass>]
type SeqWrapper<'v>(name : string) = 
    inherit TargetMetadata(name)
    interface 'v seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator
    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract AddLast : 'v -> SeqWrapper<'v>
    abstract AddFirst : 'v -> SeqWrapper<'v>
    abstract AddLastRange : 'v seq -> SeqWrapper<'v>
    abstract AddFirstRange : 'v seq -> SeqWrapper<'v>
    abstract RemoveLast : unit -> SeqWrapper<'v>
    abstract Insert : int * 'v -> SeqWrapper<'v>
    abstract RemoveFirst : unit -> SeqWrapper<'v>
    abstract Remove : int -> SeqWrapper<'v>
    abstract InsertRange : int * 'v seq -> SeqWrapper<'v>
    abstract Take : int -> SeqWrapper<'v>
    abstract Skip : int -> SeqWrapper<'v>
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
    override x.Equals other = 
        match other with
        | :? SeqWrapper<'v> as other -> Seq.equals x other
        | _ -> false
    override x.GetHashCode() = x.CompuateSeqHashCode()
    default x.AddFirst _ = raise <|  OperationNotImplemented("AddFirst")
    default x.Insert (a,b) = raise <| OperationNotImplemented("Insert")
    default x.RemoveFirst() =raise <| OperationNotImplemented("RemoveFirst")
    default x.Remove _ = raise <| OperationNotImplemented("Remove")
    default x.Skip _ = raise <| OperationNotImplemented("Skip")
    default x.Item(a,b) = raise <| OperationNotImplemented("Slice")
    default x.InsertRange(a,b) = raise <| OperationNotImplemented("InsertRange")
    default x.AddFirstRange _ = raise <| OperationNotImplemented("AddFirstRange")


type SeqReferenceWrapper<'v>(inner : ImmutableList<'v>)= 
    inherit SeqWrapper<'v>("ImmutableList")
    let negIx i = if i < 0 then inner.Count + i else i
    override x.GetEnumerator() = inner.GetEnumerator():>_
    override x.AddLast v = SeqReferenceWrapper(inner.Add v) :>_
    override x.SelfTest() = true
    override x.AddLastRange vs = SeqReferenceWrapper(inner.AddRange vs) :>_
    override x.AddFirst v = SeqReferenceWrapper(inner.Insert(0, v)) :>_
    override x.AddFirstRange vs = SeqReferenceWrapper(inner.InsertRange(0, vs)) :>_
    override x.RemoveLast() = SeqReferenceWrapper(inner.RemoveAt(inner.Count - 1)) :>_
    override x.Insert(i,v) = SeqReferenceWrapper(inner.Insert(negIx i,v)) :>_
    override x.RemoveFirst() = SeqReferenceWrapper(inner.RemoveAt(0)) :>_
    override x.Remove(i) = SeqReferenceWrapper(inner.RemoveAt(negIx i)) :>_
    override x.InsertRange(i, vs) = SeqReferenceWrapper(inner.InsertRange(negIx i,vs)) :>_
    override x.Take n = SeqReferenceWrapper(inner.GetRange(0, n)) :>_
    override x.Skip n = SeqReferenceWrapper(inner.GetRange(n, inner.Count - n)) :>_
    override x.Item(i1,i2) = SeqReferenceWrapper(inner.GetRange(i1, i2 - i1 + 1)):>_
    override x.Item ix= inner.[negIx ix]
    override x.Last = inner.[inner.Count - 1]
    override x.First = inner.[0]
    override x.Length = inner.Count
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = SeqReferenceWrapper(inner.SetItem(i, v)) :>_
    override x.Empty = SeqReferenceWrapper(ImmutableList.Empty):>_
    override x.Inner = inner:>_
    static member FromSeq s = SeqReferenceWrapper(ImmutableList.CreateRange(s)) :> SeqWrapper<_>
        

type FunqListWrapper<'v>(inner : FunqList<'v>)= 
    inherit SeqWrapper<'v>("FunqList")
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = FunqListWrapper(inner.AddLast v) :>_
    override x.SelfTest() = true
    override x.AddLastRange vs = FunqListWrapper(inner.AddLastRange(vs)):>_
    override x.AddFirst v = FunqListWrapper(inner.AddFirst v) :>_
    override x.AddFirstRange vs = FunqListWrapper(inner.AddFirstRange(vs)) :>_
    override x.RemoveLast() = FunqListWrapper(inner.RemoveLast()) :>_
    override x.Insert(i,v) = FunqListWrapper(inner.Insert(i,v)) :>_
    override x.RemoveFirst() = FunqListWrapper(inner.RemoveFirst()) :>_
    override x.Remove(i) = FunqListWrapper(inner.Remove(i)) :>_
    override x.InsertRange(i, vs) = FunqListWrapper(inner.InsertRange(i,vs)) :>_
    override x.Take n = FunqListWrapper(inner.Take n) :>_
    override x.Skip n = FunqListWrapper(inner.Skip n) :>_
    override x.Item(i1,i2) = FunqListWrapper(inner.[i1, i2]):>_
    override x.Item ix= inner.[ix]
    override x.First = inner.First
    override x.Last = inner.Last
    override x.Length = inner.Length
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = FunqListWrapper(inner.Update(i, v)) :>_
    override x.Empty = FunqListWrapper(FunqList.Empty()):>_
    override x.Inner = inner:>_
    static member FromSeq s = FunqListWrapper(FunqList.ToFunqList(s)) :> SeqWrapper<_>

type FunqVectorWrapper<'v>(inner : FunqVector<'v>)= 
    inherit SeqWrapper<'v>("FunqVector")
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = FunqVectorWrapper(inner.AddLast v) :>_
    override x.SelfTest() = true
    override x.Insert(i,v) = FunqVectorWrapper(inner.Insert(i,v)):>_
    override x.AddLastRange vs = FunqVectorWrapper(inner.AddLastRange vs) :>_
    override x.RemoveLast() = FunqVectorWrapper(inner.RemoveLast()) :>_
    override x.InsertRange(i,vs) = FunqVectorWrapper(inner.InsertRange(i,vs)) :>_
    override x.AddFirstRange vs = FunqVectorWrapper(inner.AddFirstRange vs) :> _
    override x.Take n = FunqVectorWrapper(inner.Take n) :>_
    override x.Skip n = FunqVectorWrapper(inner.Skip n) :>_
    override x.Remove i = FunqVectorWrapper(inner.Remove i) :>_
    override x.Item(i1,i2) = FunqVectorWrapper(inner.[i1, i2]):>_
    override x.Item ix= inner.[ix]
    override x.First = inner.First
    override x.Last = inner.Last
    override x.Length = inner.Length
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = FunqVectorWrapper(inner.Update(i, v)) :>_
    override x.Empty = FunqVectorWrapper(FunqVector.Empty()):>_
    override x.Inner = inner:>_
    static member FromSeq s = FunqVectorWrapper(FunqVector.ToFunqVector(s))  :> SeqWrapper<_>
type CollisionFunc<'k, 'v> = ('k -> 'v -> 'v -> 'v)

[<AbstractClass>]
type MapWrapper<'k>(name : string) = 
    inherit TargetMetadata(name)
    interface IReadOnlyDictionary<'k,'k> with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() : IEnumerator = x.GetEnumerator() :>_
        member x.Count = x.Length
        member x.get_Item k = x.Get k
        member x.ContainsKey k= x.Contains k
        member x.Keys = x.Keys
        member x.Values = x.Values
        member x.TryGetValue(k, v) = 
            if x.Contains k then
                v <- x.Get k
                true
            else
                false
    abstract GetEnumerator : unit -> IEnumerator<Kvp<'k,'k>>
    abstract Add : 'k * 'k -> MapWrapper<'k>
    abstract Get : 'k -> 'k
    abstract AddRange : ('k * 'k) seq -> MapWrapper<'k>
    abstract Set : 'k * 'k -> MapWrapper<'k>
    abstract Remove : 'k -> MapWrapper<'k>
    abstract ByOrder : int -> Kvp<'k,'k>
    abstract Contains : 'k -> bool
    abstract MaxItem : 'k * 'k
    abstract MinItem : 'k * 'k
    abstract IsOrdered : bool
    abstract IsEmpty : bool
    abstract RemoveRange : seq<'k> -> MapWrapper<'k>
    abstract Union : MapWrapper<'k> * CollisionFunc<'k,'k> -> MapWrapper<'k>
    abstract Intersect : MapWrapper<'k> * CollisionFunc<'k,'k> -> MapWrapper<'k>
    abstract Except : MapWrapper<'k> -> MapWrapper<'k>
    abstract Empty : MapWrapper<'k>
    abstract Difference : MapWrapper<'k> -> MapWrapper<'k>
    member x.Keys = x |> Seq.map (Kvp.ToTuple >> fst)
    member x.Values = x |> Seq.map (Kvp.ToTuple >> snd)
    default x.ByOrder _ = raise <|  OperationNotImplemented("ByOrder")
    default x.MaxItem = raise <|  OperationNotImplemented("MaxItem")
    default x.MinItem = raise <|  OperationNotImplemented("MinItem")
    default x.Union (_,_) = raise <|  OperationNotImplemented("Union")
    default x.Intersect (_,_) = raise <|  OperationNotImplemented("Intersect")
    default x.Except _ = raise <|  OperationNotImplemented("Except")
    default x.Difference _ = raise <|  OperationNotImplemented("Difference")
    default x.RemoveRange _ = raise <|  OperationNotImplemented("RemoveRange")
    override x.Equals other = 
        match other with
        | :? MapWrapper<'k> as other -> 
            let eq1 = x.MapEquals other
            if x.IsOrdered && other.IsOrdered then
                eq1 && Seq.equalsWith (fun (a : Kvp<_,_>) (b : Kvp<_,_>) -> obj.Equals(a.Key,b.Key) && obj.Equals(a.Value,b.Value)) x other
            else eq1
        | _ -> false

    override x.GetHashCode() = x.ComputeMapHashCode(Eq.Default)
type FunqMapWrapper<'k when 'k : equality>(Inner : FunqMap<'k,'k>) =
    inherit MapWrapper<'k>("FunqMap")
    static let wrap x = FunqMapWrapper<'k>(x) :> MapWrapper<'k>
    member val public Inner = Inner
    override x.GetEnumerator() = (Inner :> _ seq).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Remove k = Inner.Remove k |> wrap
    override x.IsOrdered = false
    override x.AddRange vs = Inner.SetRange vs |> wrap
    override x.SelfTest() = 
        x |> Seq.distinctBy (fun y -> y.Key) |> Seq.length = x.Length
    override x.Contains k = Inner.ContainsKey(k)
    override x.Empty = FunqMap.Empty() |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Union(other, f) =
        let typed = other :?> FunqMapWrapper<'k>
        Inner.Merge(typed.Inner, f |> toFunc3) |> wrap
    override x.Intersect(other, f) = 
        let typed = other :?> FunqMapWrapper<'k>
        Inner.Join<'k>(typed.Inner, f |> toFunc3) |> wrap
    override x.Except other = 
        let typed = other :?> FunqMapWrapper<'k>
        Inner.Except(typed.Inner) |> wrap
    override x.Difference other = 
        let typed = other :?> FunqMapWrapper<'k>
        Inner.Difference(typed.Inner) |> wrap
    override x.RemoveRange vs = 
        Inner.RemoveRange vs |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty
    static member FromSeq s = FunqMapWrapper(FunqMap.ToFunqMap(s))  :> MapWrapper<_>

type FunqOrderedMapWrapper<'k when 'k : equality>(Inner : FunqOrderedMap<'k, 'k>) =
    inherit MapWrapper<'k>("FunqOrderedMap")
    static let wrap x = FunqOrderedMapWrapper<'k>(x) :> MapWrapper<'k>
    member val public Inner = Inner
    override x.GetEnumerator() = (Inner :>_ seq).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Remove k = Inner.Remove k |> wrap
    override x.Contains k = Inner.ContainsKey(k)
    override x.AddRange vs = Inner.SetRange vs |> wrap
    override x.IsOrdered = true
    override x.Empty = FunqOrderedMap<'k,'k>.Empty(Cmp.Default) |> wrap
    override x.Get k = Inner.[k]
    override x.SelfTest() = 
        let mutable i = 0
        let mutable okay = true
        let distinctKeyCount = x |> Seq.distinctBy (fun y -> y.Key) |> Seq.length
        if distinctKeyCount <> x.Length then false
        else
            for item in x do
                let atIndex = x.ByOrder i
                okay <- okay && obj.Equals(item.Key, atIndex.Key) && obj.Equals(item.Value, atIndex.Value)
                i <- i + 1
            okay
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Union(other, f) =
        let typed = other :?> FunqOrderedMapWrapper<'k>
        Inner.Merge(typed.Inner, f |> toFunc3) |> wrap
    override x.Intersect(other, f) = 
        let typed = other :?> FunqOrderedMapWrapper<'k>
        Inner.Join(typed.Inner, f |> toFunc3) |> wrap
    override x.Except other = 
        let typed = other :?> FunqOrderedMapWrapper<'k>
        Inner.Except(typed.Inner) |> wrap
    override x.Difference other = 
        let typed = other :?> FunqOrderedMapWrapper<'k>
        Inner.Difference(typed.Inner) |> wrap
    override x.RemoveRange vs = 
        Inner.RemoveRange vs |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.MinItem = Inner.MinItem.Key,Inner.MinItem.Value
    override x.MaxItem = Inner.MaxItem.Key, Inner.MaxItem.Value
    override x.ByOrder i = Inner.ByOrder(i)
    static member FromSeq s = 
        FunqOrderedMapWrapper(FunqOrderedMap.ToFunqOrderedMap(s, Cmp.Default)) :> MapWrapper<_>

type MapReferenceWrapper<'k when 'k : comparison>(Inner : Map<'k, 'k>) = 
    inherit MapWrapper<'k>("ImmutableSortedDictionary")
    static let wrap x = MapReferenceWrapper<'k>(x) :> MapWrapper<'k>
    let min = lazy (Inner |> Seq.head |> fun kvp -> kvp.Key,kvp.Value)
    let  max = lazy (Inner |> Seq.last |> fun kvp -> kvp.Key,kvp.Value)
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :>_ seq).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Remove k = Inner.Remove(k) |> wrap
    override x.Contains k = Inner.ContainsKey(k)
    override x.MinItem = min.Value
    override x.SelfTest() = true
    override x.IsOrdered = true
    override x.MaxItem = max.Value
    override x.AddRange vs = 
        let mutable mp = x.Inner
        for item in vs do
            mp <- mp.Add item
        mp |> wrap
    override x.Empty = Map.empty |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.Add(k, v) |> wrap
    override x.Union (other, f) =
        let mutable working = x :> MapWrapper<_>
        let allKeys = x.Keys.ToImmutableSortedSet().Union(other.Keys)
        for k in allKeys do
            match x.Contains k, other.Contains k with
            | true, true -> 
                let myV = x.Get(k)
                let othV = other.Get(k)
                working <- working.Add(k, f k myV othV)
            | true, false ->
                working <- working.Add(k, x.Get(k))
            | false, true -> 
                working <- working.Add(k, other.Get(k))
            | _ -> failwith "What"
        working
    override x.Intersect (other, f) =
        let mutable tally = x.Empty
        for k, v in x |> Seq.map (Kvp.ToTuple) do
            if other.Contains k then
                tally <- tally.Add(k, f k v (other.Get(k)))
        tally
    override x.Except(other) =
        x.RemoveRange(other.Keys)
    override x.Difference other = 
        x.Except(other).Union(other.Except x, fun k v1 v2 -> v1) 
    override x.RemoveRange ks = 
        let mutable x = x.Inner
        for item in ks do
            x <- x.Remove item
        x |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.Length = Inner.Count
    override x.ByOrder i = x |> Seq.nth i
    static member FromSeq s = s |> Seq.map (Kvp.ToTuple) |> Map.ofSeq |> wrap
[<AbstractClass>]
type SetWrapper<'v>(name : string) = 
    inherit TargetMetadata(name)
    interface 'v seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator

    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract Add : 'v -> SetWrapper<'v>
    abstract AddRange : 'v seq -> SetWrapper<'v>
    abstract Remove : 'v -> SetWrapper<'v>
    abstract ByOrder : int -> 'v
    abstract Contains : 'v -> bool
    abstract MaxItem : 'v 
    abstract IsOrdered : bool
    abstract MinItem : 'v
    abstract IsEmpty : bool
    abstract Empty : SetWrapper<'v>
    abstract RemoveRange : seq<'v> -> SetWrapper<'v>
    abstract Union : SetWrapper<'v> -> SetWrapper<'v>
    abstract Intersect : SetWrapper<'v> -> SetWrapper<'v>
    abstract Except : SetWrapper<'v> -> SetWrapper<'v>
    abstract Difference : SetWrapper<'v> -> SetWrapper<'v>
    default x.ByOrder _ = raise <|  OperationNotImplemented("ByOrder")
    default x.MaxItem = raise <|  OperationNotImplemented("MaxItem")
    default x.MinItem = raise <|  OperationNotImplemented("MinItem")
    default x.Union _ = raise <|  OperationNotImplemented("Union")
    default x.Intersect _ = raise <|  OperationNotImplemented("Intersect")
    default x.Except _ = raise <|  OperationNotImplemented("Except")
    default x.Difference _ = raise <|  OperationNotImplemented("Difference")
    default x.RemoveRange _ = raise <|  OperationNotImplemented("RemoveRange")
    override x.Equals other = 
        match other with
        | :? SetWrapper<'v> as other -> 
            let eq1 = x.SetEquals(other) && other.Length = x.Length
            if x.IsOrdered && other.IsOrdered then
                eq1 && Seq.equals x other
            else
                eq1
        | _ -> failwith "Unexpected equality!"
    override x.GetHashCode() = x.ComputeSetHashCode()
    
(*
type SetReferenceWrapper<'v>(Inner : ImmutableHashSet<'v>) = 
    inherit SetWrapper<'v>("ImmutableSet")
    static let wrap x = SetReferenceWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Remove v = Inner.Remove v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Count
    override x.Empty = ImmutableHashSet.Create() |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.RemoveRange vs =
        let mutable is = Inner
        for v in vs do
            is <- is.Remove v
        is |> wrap
    override x.Union st = Inner.Union(st) |> wrap
    override x.Intersect st = Inner.Intersect(st) |> wrap
    override x.Except st = Inner.Except(st) |> wrap
    override x.Difference st = Inner.SymmetricExcept(st) |> wrap
    static member FromSeq s = ImmutableHashSet.CreateRange(s) |> wrap
*)
type ReferenceSetWrapper<'v when 'v : comparison>(Inner : Set<'v>) = 
    inherit SetWrapper<'v>("ImmutableSortedSet")    
    static let wrap x = ReferenceSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Remove v = Inner.Remove v |> wrap
    override x.AddRange s= 
        let mutable x = x.Inner
        for item in s do
            x <- x.Add item
        x |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Count
    override x.SelfTest() = true
    override x.Empty = Set.empty |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.IsOrdered = true
    override x.RemoveRange vs =
        let mutable is = Inner
        for v in vs do
            is <- is.Remove v
        is |> wrap
    override x.Union st = 
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.union other.Inner x.Inner |> wrap

    override x.Intersect st =
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.intersect other.Inner x.Inner |> wrap
    override x.Except st =
        match st with
        | :? ReferenceSetWrapper<'v> as other -> Set.difference x.Inner other.Inner |> wrap
    override x.Difference st =
        match st with
        | :? ReferenceSetWrapper<'v> as other ->
            let a = Set.difference x.Inner other.Inner
            let b = Set.difference other.Inner x.Inner
            Set.union a b |> wrap
    override x.ByOrder i = Inner |> Seq.nth i
    override x.MinItem = Inner.MinimumElement
    override x.MaxItem = Inner.MaximumElement
    static member FromSeq s = s |> Set.ofSeq |> wrap

type FunqSetWrapper<'v when 'v : comparison>(Inner : FunqSet<'v>) = 
    inherit SetWrapper<'v>("FunqSet")
    static let wrap x = FunqSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.AddRange vs = Inner.Union vs |> wrap
    override x.Remove v = Inner.Remove v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Length
    override x.Empty = FunqSet.Empty()|> wrap
    override x.SelfTest() = 
        let distinctCount = x |> Seq.distinct |> Seq.length
        distinctCount = x.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.IsOrdered = false
    override x.RemoveRange vs = Inner.Except vs |> wrap
    override x.Union other = 
        match other with
        | :? FunqSetWrapper<'v> as wraped -> Inner.Union(wraped.Inner) |> wrap
    override x.Intersect other = 
        match other with
        | :? FunqSetWrapper<'v> as wraped -> Inner.Intersect(wraped.Inner) |> wrap
    override x.Except other = 
        match other with
        | :? FunqSetWrapper<'v> as wraped -> Inner.Except(wraped.Inner) |> wrap
    override x.Difference other = 
        match other with
        | :? FunqSetWrapper<'v> as wraped -> Inner.Difference(wraped.Inner) |> wrap
    static member FromSeq s = FunqSet.ToFunqSet(s) |> wrap

type FunqOrderedSetWrapper<'v when 'v : comparison and 'v :> IComparable<'v>>(Inner : FunqOrderedSet<'v>) = 
    inherit SetWrapper<'v>("FunqOrderedSet")
    static let wrap x = FunqOrderedSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Remove v = Inner.Remove v |> wrap
    override x.AddRange vs = Inner.Union vs |> wrap
    override x.Contains v = Inner.Contains v
    override x.IsOrdered = true
    override x.Length = Inner.Length
    override x.Empty = FunqOrderedSet.Empty<'v>()|> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.SelfTest() =     
        let mutable i = 0
        let mutable okay = true
        let distinctCount = x |> Seq.distinct |> Seq.length
        if not <| (distinctCount = x.Length) then false
        else
            for item in x do
                let atIndex = x.ByOrder i
                okay <- okay && obj.Equals(atIndex, item)
                i <- i + 1
            okay
    override x.RemoveRange vs = Inner.Except vs |> wrap
    override x.Union other = 
        match other with
        | :? FunqOrderedSetWrapper<'v> as wraped -> Inner.Union(wraped.Inner) |> wrap
    override x.Intersect other = 
        match other with
        | :? FunqOrderedSetWrapper<'v> as wraped -> Inner.Intersect(wraped.Inner) |> wrap
    override x.Except other = 
        match other with
        | :? FunqOrderedSetWrapper<'v> as wraped -> Inner.Except(wraped.Inner) |> wrap
    override x.Difference other = 
        match other with
        | :? FunqOrderedSetWrapper<'v> as wraped -> Inner.Difference(wraped.Inner) |> wrap
    override x.MinItem = Inner.MinItem
    override x.MaxItem = Inner.MaxItem
    override x.ByOrder i= Inner.ByOrder i
    static member FromSeq s = FunqOrderedSet.ToFunqOrderedSet(s, Cmp.Default) |> wrap