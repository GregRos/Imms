module Funq.Tests.Integrity.Wrappers
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open Funq.Collections
open Funq.FSharp.Implementation.Compatibility
open System

open Funq.FSharp.Implementation
exception OperationNotImplemented of string


[<AbstractClass>]
type TestWrapper<'v>(name : string) = 
    inherit MetaContainer([Meta("Name", name)])
    interface 'v seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator
    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract AddLast : 'v -> TestWrapper<'v>
    abstract AddFirst : 'v -> TestWrapper<'v>
    abstract AddLastRange : 'v seq -> TestWrapper<'v>
    abstract AddFirstRange : 'v seq -> TestWrapper<'v>
    abstract DropLast : unit -> TestWrapper<'v>
    abstract Insert : int * 'v -> TestWrapper<'v>
    abstract DropFirst : unit -> TestWrapper<'v>
    abstract Remove : int -> TestWrapper<'v>
    abstract InsertRange : int * 'v seq -> TestWrapper<'v>
    abstract Take : int -> TestWrapper<'v>
    abstract Skip : int -> TestWrapper<'v>
    abstract member Item : int -> 'v
    abstract member Item : int * int -> TestWrapper<'v>
    abstract First : 'v
    abstract Last : 'v
    abstract TryFirst : 'v option
    abstract TryLast : 'v option
    abstract IsEmpty : bool
    abstract Update : int * 'v -> TestWrapper<'v>
    abstract Length : int
    abstract Empty : TestWrapper<'v>
    abstract Inner : seq<'v>
    default x.AddFirst _ = raise <|  OperationNotImplemented("AddFirst")
    default x.Insert (a,b) = raise <| OperationNotImplemented("Insert")
    default x.DropFirst() =raise <| OperationNotImplemented("DropFirst")
    default x.Remove _ = raise <| OperationNotImplemented("Remove")
    default x.Skip _ = raise <| OperationNotImplemented("Skip")
    default x.Item(a,b) = raise <| OperationNotImplemented("Slice")
    default x.InsertRange(a,b) = raise <| OperationNotImplemented("InsertRange")
    default x.AddFirstRange _ = raise <| OperationNotImplemented("AddFirstRange")

    
type ReferenceWrapper<'v>(inner : ImmutableList<'v>)= 
    inherit TestWrapper<'v>("ImmutableList")
    let negIx i = if i < 0 then inner.Count + i else i
    override x.GetEnumerator() = inner.GetEnumerator():>_
    override x.AddLast v = ReferenceWrapper(inner.Add v) :>_
    override x.AddLastRange vs = ReferenceWrapper(inner.AddRange vs) :>_
    override x.AddFirst v = ReferenceWrapper(inner.Insert(0, v)) :>_
    override x.AddFirstRange vs = ReferenceWrapper(inner.InsertRange(0, vs)) :>_
    override x.DropLast() = ReferenceWrapper(inner.RemoveAt(inner.Count - 1)) :>_
    override x.Insert(i,v) = ReferenceWrapper(inner.Insert(negIx i,v)) :>_
    override x.DropFirst() = ReferenceWrapper(inner.RemoveAt(0)) :>_
    override x.Remove(i) = ReferenceWrapper(inner.RemoveAt(negIx i)) :>_
    override x.InsertRange(i, vs) = ReferenceWrapper(inner.InsertRange(negIx i,vs)) :>_
    override x.Take n = ReferenceWrapper(inner.GetRange(0, n)) :>_
    override x.Skip n = ReferenceWrapper(inner.GetRange(n, inner.Count - n)) :>_
    override x.Item(i1,i2) = ReferenceWrapper(inner.GetRange(i1, i2 - i1 + 1)):>_
    override x.Item ix= inner.[negIx ix]
    override x.Last = inner.[inner.Count - 1]
    override x.First = inner.[0]
    override x.Length = inner.Count
    override x.TryFirst = if x.IsEmpty then None else x.First |> Some
    override x.TryLast = if x.IsEmpty then None else x.Last |> Some
    override x.IsEmpty = inner.IsEmpty
    override x.Update(i,v) = ReferenceWrapper(inner.SetItem(i, v)) :>_
    override x.Empty = ReferenceWrapper(ImmutableList.Empty):>_
    override x.Inner = inner:>_

type FunqListWrapper<'v>(inner : FunqList<'v>)= 
    inherit TestWrapper<'v>("FunqList")
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = FunqListWrapper(inner.AddLast v) :>_
    override x.AddLastRange vs = FunqListWrapper(inner.AddLastRange(vs)):>_
    override x.AddFirst v = FunqListWrapper(inner.AddFirst v) :>_
    override x.AddFirstRange vs = FunqListWrapper(inner.AddFirstRange(vs)) :>_
    override x.DropLast() = FunqListWrapper(inner.DropLast()) :>_
    override x.Insert(i,v) = FunqListWrapper(inner.Insert(i,v)) :>_
    override x.DropFirst() = FunqListWrapper(inner.DropFirst()) :>_
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

type FunqVectorWrapper<'v>(inner : FunqVector<'v>)= 
    inherit TestWrapper<'v>("FunqVector")
    override x.GetEnumerator() = (inner :> 'v seq).GetEnumerator()
    override x.AddLast v = FunqVectorWrapper(inner.AddLast v) :>_
    override x.AddLastRange vs = FunqVectorWrapper(inner.AddLastRange vs) :>_
    override x.DropLast() = FunqVectorWrapper(inner.DropLast()) :>_
    override x.Take n = FunqVectorWrapper(inner.Take n) :>_
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

type CollisionFunc<'k,'v> = ('k -> 'v -> 'v -> 'v)

[<AbstractClass>]
type MapWrapper<'k, 'v>(name : string) = 
    inherit MetaContainer([Meta("Name", name)])
    interface ('k * 'v) seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator
    abstract GetEnumerator : unit -> IEnumerator<'k * 'v>
    abstract Add : 'k * 'v -> MapWrapper<'k,'v>
    abstract Get : 'k -> 'v
    abstract Set : 'k * 'v -> MapWrapper<'k,'v>
    abstract Drop : 'k -> MapWrapper<'k, 'v>
    abstract ByOrder : int -> 'k * 'v
    abstract Contains : 'k -> bool
    abstract MaxItem : 'k * 'v
    abstract MinItem : 'k * 'v
    abstract Length : int
    abstract IsEmpty : bool
    abstract DropRange : seq<'k> -> MapWrapper<'k, 'v>
    abstract Union : MapWrapper<'k,'v> * CollisionFunc<'k,'v> -> MapWrapper<'k,'v>
    abstract Intersect : MapWrapper<'k,'v> * CollisionFunc<'k,'v> -> MapWrapper<'k,'v>
    abstract Except : MapWrapper<'k,'v> -> MapWrapper<'k,'v>
    abstract Empty : MapWrapper<'k,'v>
    abstract Difference : MapWrapper<'k,'v> -> MapWrapper<'k,'v>
    default x.ByOrder _ = raise <|  OperationNotImplemented("ByOrder")
    default x.MaxItem = raise <|  OperationNotImplemented("MaxItem")
    default x.MinItem = raise <|  OperationNotImplemented("MinItem")
    default x.Union (_,_) = raise <|  OperationNotImplemented("Union")
    default x.Intersect (_,_) = raise <|  OperationNotImplemented("Intersect")
    default x.Except _ = raise <|  OperationNotImplemented("Except")
    default x.Difference _ = raise <|  OperationNotImplemented("Difference")
    default x.DropRange _ = raise <|  OperationNotImplemented("DropRange")

type FunqMapWrapper<'k,'v>(Inner : FunqMap<'k,'v>) =
    inherit MapWrapper<'k,'v>("FunqMap")
    static let wrap x = FunqMapWrapper<'k,'v>(x) :> MapWrapper<'k,'v>
    member val public Inner = Inner
    override x.GetEnumerator() = (Inner |> Seq.map (fun kvp -> kvp.Key, kvp.Value)).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Drop k = Inner.Drop k |> wrap
    override x.Contains k = Inner.ContainsKey(k)
    override x.Empty = FunqMap.Empty() |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Union(other, f) =
        let typed = other :?> FunqMapWrapper<'k,'v>
        Inner.Merge(typed.Inner, f |> toFunc3) |> wrap
    override x.Intersect(other, f) = 
        let typed = other :?> FunqMapWrapper<'k,'v>
        Inner.Join(typed.Inner, f |> toFunc3) |> wrap
    override x.Except other = 
        let typed = other :?> FunqMapWrapper<'k,'v>
        Inner.Except(typed.Inner) |> wrap
    override x.Difference other = 
        let typed = other :?> FunqMapWrapper<'k,'v>
        Inner.Difference(typed.Inner) |> wrap
    override x.DropRange vs = 
        Inner.DropRange vs |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty

type FunqOrderedMapWrapper<'k,'v when 'k :> IComparable<'k>>(Inner : FunqOrderedMap<'k,'v>) =
    inherit MapWrapper<'k,'v>("FunqMap")
    static let wrap x = FunqOrderedMapWrapper<'k,'v>(x) :> MapWrapper<'k,'v>
    member val public Inner = Inner
    override x.GetEnumerator() = (Inner |> Seq.map (fun kvp -> kvp.Key, kvp.Value)).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Drop k = Inner.Drop k |> wrap
    override x.Contains k = Inner.ContainsKey(k)
    override x.Empty = FunqOrderedMap.Empty() |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Union(other, f) =
        let typed = other :?> FunqOrderedMapWrapper<'k,'v>
        Inner.Merge(typed.Inner, f |> toFunc3) |> wrap
    override x.Intersect(other, f) = 
        let typed = other :?> FunqOrderedMapWrapper<'k,'v>
        Inner.Join(typed.Inner, f |> toFunc3) |> wrap
    override x.Except other = 
        let typed = other :?> FunqOrderedMapWrapper<'k,'v>
        Inner.Except(typed.Inner) |> wrap
    override x.Difference other = 
        let typed = other :?> FunqOrderedMapWrapper<'k,'v>
        Inner.Difference(typed.Inner) |> wrap
    override x.DropRange vs = 
        Inner.DropRange vs |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.MinItem = Inner.MinItem.Key,Inner.MinItem.Value
    override x.MaxItem = Inner.MaxItem.Key, Inner.MaxItem.Value
    override x.ByOrder i = Inner.ByOrder(i).Key, Inner.ByOrder(i).Value
type ReferenceMapWrapper<'k,'v when 'k : comparison>(Inner : ImmutableSortedDictionary<'k,'v>) = 
    inherit MapWrapper<'k,'v>("ImmutableSortedDictionary")
    static let wrap x = ReferenceMapWrapper<'k,'v>(x) :> MapWrapper<'k,'v>
    let min = lazy (Inner |> Seq.head |> fun kvp -> kvp.Key,kvp.Value)
    let  max = lazy (Inner |> Seq.last |> fun kvp -> kvp.Key,kvp.Value)
    member val Inner = Inner
    override x.GetEnumerator() = Inner |> Seq.map (fun kvp -> (kvp.Key, kvp.Value)) |> fun z -> z.GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Drop k = Inner.Remove(k) |> wrap
    override x.Contains k = Inner.ContainsKey(k)
    override x.MinItem = min.Value
    override x.MaxItem = max.Value
    override x.Empty = ImmutableSortedDictionary<'k,'v>.Empty |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.SetItem(k, v) |> wrap
    override x.Union (other, f) =
        let mutable working = x :> MapWrapper<_,_>
        let allKeys = Set.ofSeq (x |> Seq.map fst) |> Set.union (other |> Seq.map fst |> Set.ofSeq)
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
        for k, v in x do
            if other.Contains k then
                tally <- tally.Add(k, f k v (other.Get(k)))
        tally
    override x.Except other =
        x.DropRange(other |> Seq.map fst)
    override x.Difference other = 
        x.Except(other).Union(other.Except x, fun k v1 v2 -> v1) 
    override x.DropRange ks = Inner.RemoveRange(ks) |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.Length = Inner.Count
    override x.ByOrder i = x |> Seq.nth i
    
[<AbstractClass>]
type SetWrapper<'v>(name : string) = 
    inherit MetaContainer([Meta("Name", name)])
    interface 'v seq with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() = x.GetEnumerator() :> IEnumerator

    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract Add : 'v -> SetWrapper<'v>
    abstract Drop : 'v -> SetWrapper<'v>
    abstract ByOrder : int -> 'v
    abstract Contains : 'v -> bool
    abstract MaxItem : 'v 
    abstract MinItem : 'v
    abstract Length : int
    abstract IsEmpty : bool
    abstract Empty : SetWrapper<'v>
    abstract DropRange : seq<'v> -> SetWrapper<'v>
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
    default x.DropRange _ = raise <|  OperationNotImplemented("DropRange")

type ReferenceSetWrapper<'v>(Inner : ImmutableHashSet<'v>) = 
    inherit SetWrapper<'v>("ImmutableSet")
    static let wrap x = ReferenceSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Drop v = Inner.Remove v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Count
    override x.Empty = ImmutableHashSet.Create() |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.DropRange vs =
        let mutable is = Inner
        for v in vs do
            is <- is.Remove v
        is |> wrap
    override x.Union st = Inner.Union(st) |> wrap
    override x.Intersect st = Inner.Intersect(st) |> wrap
    override x.Except st = Inner.Except(st) |> wrap
    override x.Difference st = Inner.SymmetricExcept(st) |> wrap
    
type ReferenceOrderedSetWrapper<'v>(Inner : ImmutableSortedSet<'v>) = 
    inherit SetWrapper<'v>("ImmutableSortedSet")
    static let wrap x = ReferenceOrderedSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Drop v = Inner.Remove v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Count
    override x.Empty = ImmutableSortedSet.Create() |> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.DropRange vs =
        let mutable is = Inner
        for v in vs do
            is <- is.Remove v
        is |> wrap
    override x.Union st = Inner.Union(st) |> wrap
    override x.Intersect st = Inner.Intersect(st) |> wrap
    override x.Except st = Inner.Except(st) |> wrap
    override x.Difference st = Inner.SymmetricExcept(st) |> wrap
    override x.ByOrder i = Inner.Item(i)
    override x.MinItem = Inner.Min
    override x.MaxItem = Inner.Max
    
type FunqSetWrapper<'v when 'v : comparison>(Inner : FunqSet<'v>) = 
    inherit SetWrapper<'v>("FunqSet")
    static let wrap x = FunqSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Drop v = Inner.Drop v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Length
    override x.Empty = FunqSet.Empty(Eq.Default)|> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.DropRange vs = Inner.DropRange vs |> wrap
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

type FunqOrderedSetWrapper<'v when 'v : comparison>(Inner : FunqOrderedSet<'v>) = 
    inherit SetWrapper<'v>("FunqSet")
    static let wrap x = FunqOrderedSetWrapper<'v>(x) :> SetWrapper<'v>
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :> seq<_>).GetEnumerator()
    override x.Add v = Inner.Add v |> wrap
    override x.Drop v = Inner.Drop v |> wrap
    override x.Contains v = Inner.Contains v
    override x.Length = Inner.Length
    override x.Empty = FunqOrderedSet.Empty(Cm.Default)|> wrap
    override x.IsEmpty = Inner.IsEmpty
    override x.DropRange vs = Inner.DropRange vs |> wrap
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
    override x.MinItem = Inner.MinItem
    override x.MaxItem = Inner.MaxItem
    override x.ByOrder i= Inner.ByOrder i
(*
    
    abstract GetEnumerator : unit -> IEnumerator<'v>
    abstract AddLast : 'v -> TestWrapper<'v>
    abstract AddFirst : 'v -> TestWrapper<'v>
    abstract AddLastRange : 'v seq -> TestWrapper<'v>
    abstract AddFirstRange : 'v seq -> TestWrapper<'v>
    abstract DropLast : unit -> TestWrapper<'v>
    abstract Insert : int * 'v -> TestWrapper<'v>
    abstract DropFirst : unit -> TestWrapper<'v>
    abstract Remove : int -> TestWrapper<'v>
    abstract InsertRange : int * 'v seq -> TestWrapper<'v>
    abstract Take : int -> TestWrapper<'v>
    abstract Skip : int -> TestWrapper<'v>
    abstract member Item : int -> 'v
    abstract member Item : int * int -> TestWrapper<'v>
    abstract First : 'v
    abstract Last : 'v
    abstract TryFirst : 'v option
    abstract TryLast : 'v option
    abstract IsEmpty : bool
    abstract Update : int * 'v -> TestWrapper<'v>
    abstract Length : int
    abstract Empty : TestWrapper<'v>
    abstract Inner : seq<'v>
    default x.AddFirst _ = raise <|  OperationNotImplemented("AddFirst")
    default x.Insert (a,b) = raise <| OperationNotImplemented("Insert")
    default x.DropFirst() =raise <| OperationNotImplemented("DropFirst")
    default x.Remove _ = raise <| OperationNotImplemented("Remove")
    default x.Skip _ = raise <| OperationNotImplemented("Skip")
    default x.Item(a,b) = raise <| OperationNotImplemented("Slice")
    default x.InsertRange(a,b) = raise <| OperationNotImplemented("InsertRange")
    default x.AddFirstRange _ = raise <| OperationNotImplemented("AddFirstRange")
    *)