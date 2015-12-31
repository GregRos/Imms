namespace Funq.Tests.Performance.Wrappers
open System
open Funq.Abstract
#nowarn"20"
///A module with light-weight wrappers for the various System.Collections.Immutable classes.

module Funq = 
    open Funq
    open System.Collections.Generic
    [<Struct>]
    type FunqMap<'t> = 
        val public inner : FunqMap<'t, 't>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't[]
        new (innerp) = {inner = innerp}
        member  x.Add(k,v) = FunqMap(x.inner.Set(k,v))
        member  x.get_Item k = x.inner.TryGet(k).Value
        member  x.AddRange (sq : KeyValuePair<_,_> seq) = FunqMap(x.inner.SetRange sq)
        member  x.RemoveRange sq = FunqMap(x.inner.RemoveRange sq)
        member  x.Remove item = FunqMap(x.inner.Remove item)
        member  x.Length = x.inner.Length
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = x.inner.ForEach (a)
        member  x.Keys = x.keys
        static member  FromSeq(s : 't seq) = 
            let mutable mp = [for item in s -> Kvp.Of(item, item)].ToFunqMap() |> fun x -> FunqMap(x)
            mp.keys <- s |> Seq.toArray
            mp

    [<Struct>]
    type OrderedMap<'t when 't :> IComparable<'t>> = 
        val public inner : FunqOrderedMap<'t, 't>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't[]
        new (innerp) = {inner = innerp}
        member  x.Add(k,v) = OrderedMap<_>(x.inner.Set(k,v))
        member  x.ForEach (a : Action<_>) = x.inner.ForEach(a)
        member  x.Length = x.inner.Length
        member  x.AddRange (sq : KeyValuePair<_,_> seq) = OrderedMap<_>(x.inner.SetRange sq)
        member  x.RemoveRange sq = OrderedMap<_>(x.inner.RemoveRange sq)
        member  x.Remove item = OrderedMap(x.inner.Remove item)
        member  x.AsSeq = x.inner :> _ seq
        member  x.get_Item k = x.inner.TryGet(k).Value
        member  x.Keys = x.keys
        static member  FromSeq(s : 't seq) = 
            let mutable mp = [for item in s -> Kvp.Of(item, item)].ToFunqOrderedMap() |> fun x -> OrderedMap(x)
            mp.keys <- s |> Seq.toArray
            mp
    [<Struct>]
    type Set<'t> = 
        val public inner : FunqSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.Add(k) = Set(x.inner.Add(k))
        member  x.Contains(k) = x.inner.Contains k
        member  x.Remove(k) = Set(x.inner.Remove k)
        member  x.ForEach a = x.inner.ForEach a
        member  x.AsSeq = x.inner :> _ seq
        member  x.AddRange sq = Set(x.inner.Union sq)
        member  x.Length = x.inner.Length
        member  x.RemoveRange sq = Set(x.inner.Except sq)
        member  x.Union (o : Set<'t>) = Set(o.inner.Union x.inner)
        member  x.Intersect (s : Set<'t>) = Set(s.inner.Intersect x.inner)
        member  x.Except (s : Set<'t>) = Set(s.inner.Except x.inner)
        member  x.SymmetricDifference (s : Set<'t>) = Set(s.inner.Difference x.inner)
        member  x.IsSetEqual (s : Set<'t>) = x.inner.SetEquals(s.inner)
        member  x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member  x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member  x.Keys = x.keys
        static member  FromSeq(s : 't seq) = 
            let mutable set = Set(s.ToFunqSet())
            set.keys <- s |> Seq.toArray
            set

    type OrderedSet<'t when 't :> IComparable<'t>> = 
        val public inner : FunqOrderedSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.Add(k) = OrderedSet(x.inner.Add(k))
        member  x.Contains(k) = x.inner.Contains k
        member  x.Remove(k) = OrderedSet(x.inner.Remove k)
        member  x.ForEach (a : Action<'t>) = x.inner.ForEach a
        member  x.AsSeq = x.inner :> _ seq
        member  x.AddRange sq = OrderedSet(x.inner.Union sq)
        member  x.Length = x.inner.Length
        member  x.RemoveRange sq = OrderedSet(x.inner.Except sq)
        member  x.Union (o : OrderedSet<'t>) = OrderedSet(x.inner.Union o.inner)
        member  x.Intersect (s : OrderedSet<'t>) = OrderedSet(x.inner.Intersect s.inner)
        member  x.Except (s : OrderedSet<'t>) = OrderedSet(x.inner.Except s.inner)
        member  x.SymmetricDifference (s : OrderedSet<'t>) = OrderedSet(x.inner.Difference s.inner)
        member  x.IsSetEqual (s : OrderedSet<'t>) = x.inner.SetEquals(s.inner)
        member  x.IsProperSuperset (s : OrderedSet<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member  x.IsProperSubset (s : OrderedSet<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member  x.Keys = x.keys
        static member  FromSeq(s : 't seq) = 
            let mutable set = OrderedSet(s.ToFunqOrderedSet())
            set.keys <- s |> Seq.toArray
            set
module Sys = 
    open System.Collections.Immutable
    open System.Collections.Generic
    
    let rnd = Random()
    ///A light-weight wrapper for ImmutableList.
    [<Struct>]
    type List<'t> = 
        interface seq<'t> with
            member x.GetEnumerator() = x.inner.GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = x.inner.GetEnumerator():>IEnumerator<_>
        val public inner : ImmutableList<'t>
        new (innerp : ImmutableList<'t>) = {inner = innerp}
        member  x.AddLast v           = List(x.inner.Add(v))
        member  x.AddFirst v          = List(x.inner.Insert(0, v)) 
        member  x.AddLastRange vs     = List(x.inner.AddRange(vs)) 
        member  x.AddFirstRange vs    = List(x.inner.InsertRange(0, vs)) 
        member  x.RemoveLast()          = List(x.inner.RemoveAt(x.inner.Count - 1)) 
        member  x.RemoveFirst()         = List(x.inner.RemoveAt(0))
        member  x.Slice(a,b)           = List(x.inner.GetRange(a, a+b))
        member  x.Take a = List(x.inner.GetRange(0, a))
        member  x.Skip a = List(x.inner.GetRange(a, x.inner.Count - a))
        member  x.Insert(ix, v)       = List(x.inner.Insert(ix, v))
        member  x.InsertRange(ix, vs) = List(x.inner.InsertRange(ix, vs))
        member  x.GetEnumerator()     = (x.inner :> 't seq).GetEnumerator()
        member  x.Update(ix, v)          = List(x.inner.SetItem(ix, v))
        member  x.get_Item ix         = x.inner.[ix]
        member  x.Length = x.inner.Count
        member  x.RemoveAt i = List(x.inner.RemoveAt(i))
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.ForEach f = x.inner.ForEach(f)
        member x.AsSeq = x.inner |> seq
        static member  FromSeq (vs : 'a seq) = List(ImmutableList.CreateRange(vs))
    ///A light-weight wrapper for ImmutableQueue.
    [<Struct>]
    type Queue<'t> = 
        val public inner : ImmutableQueue<'t>
        new (innerp) = {inner = innerp}
        member  x.AddLast v = Queue(x.inner.Enqueue v)
        member  x.RemoveFirst() = Queue(x.inner.Dequeue())
        member  x.GetEnumerator() = x.inner.GetEnumerator()
        member  x.First = x.inner.Peek
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.Length = x.inner |> Seq.length
        member  x.AsSeq = x.inner |> seq
        static member  FromSeq (vs : 'a seq) = Queue(ImmutableQueue.CreateRange(vs))
    ///A light-weight wrapper for ImmutableStack
    [<Struct>]
    type Stack<'t> = 
        val public inner : ImmutableStack<'t>
        new (innerp) = {inner = innerp}
        member  x.AddFirst v=  Stack(x.inner.Push v)
        member  x.RemoveFirst() = Stack(x.inner.Pop())
        member  x.GetEnumerator() = x.inner.GetEnumerator()
        member  x.First = x.inner.Peek()
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.Length = x.inner |> Seq.length
        member  x.AsSeq = x.inner |> seq
        static member  FromSeq(vs : 'a seq) = Stack(ImmutableStack.CreateRange(vs))
    [<Struct>]
    type Dict<'t> = 
        val public inner : ImmutableDictionary<'t,'t>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member  x.Add(k,v) = Dict(x.inner.SetItem(k,v))
        member  x.Length = x.inner.Count
        member  x.Remove item = Dict(x.inner.Remove item)
        member  x.AddRange (vs : KeyValuePair<_,_> seq) = Dict(x.inner.SetItems vs)
        member  x.RemoveRange vs = Dict(x.inner.RemoveRange vs)
        member  x.get_Item k =x.inner.[k]
        member  x.Keys = x.keys
        static member  FromSeq (s : 't seq) = 
            let eqK = Eq<_>.Default
            let eqV = {new IEq<'t> with 
                            member x.Equals(a,b) = false
                            member x.GetHashCode a = rnd.Next()
                      }
            let mutable dict = Dict([for item in s -> KeyValuePair(item,item)].ToImmutableDictionary(eqK, eqV))
            dict.keys <- s |> Seq.toArray
            dict

    [<Struct>]
    type SortedDict<'t> = 
        val public inner : ImmutableSortedDictionary<'t,'t>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.Add(k,v) = SortedDict(x.inner.SetItem(k,v))
        member  x.AddRange (sq : KeyValuePair<_,_> seq) = SortedDict(x.inner.SetItems sq)
        member  x.Remove item = SortedDict(x.inner.Remove item)
        member  x.Length = x.inner.Count
        member  x.RemoveRange sq = 
            SortedDict(sq |> x.inner.RemoveRange)
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member  x.get_Item k =x.inner.[k]
        member  x.Keys = x.keys
        static member  FromSeq (s : 't seq) = 
            let cmK = Cmp<_>.Default
            let eqV = {new IEq<'t> with 
                            member x.Equals(a,b) = false
                            member x.GetHashCode a = rnd.Next()
                      }
            let mutable dict = SortedDict([for item in s -> KeyValuePair(item,item)].ToImmutableSortedDictionary(cmK, eqV))
            dict.keys <- s |> Seq.toArray
            dict

    [<Struct>]
    type Set<'t> = 
        val public inner : ImmutableHashSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.Add(k) = Set(x.inner.Add(k))
        member  x.Keys = x.keys;
        member  x.Remove k = Set(x.inner.Remove k)
        member  x.Contains(k) = x.inner.Contains k
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member  x.Length = x.inner.Count
        member  x.AddRange (s : 't seq) = Set(x.inner.Union s)
        member  x.RemoveRange (s : 't seq) = Set(x.inner.Except s)
        member  x.Union (o : Set<'t>) = Set(x.inner.Union o.inner)
        member  x.Intersect (s : Set<'t>) = Set(x.inner.Intersect s.inner)
        member  x.Except (s : Set<'t>) = Set(x.inner.Except s.inner)
        member  x.SymmetricDifference (s : Set<'t>) = Set(x.inner.SymmetricExcept s.inner)
        member  x.IsSetEqual (s : Set<'t>) = x.inner.SetEquals(s.inner)
        member  x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member  x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        static member  FromSeq(s : 't seq) = 
            let mutable set = Set(s.ToImmutableHashSet())
            set.keys <- s |> Seq.toArray
            set

    [<Struct>]
    type SortedSet<'t> = 
        val public inner : ImmutableSortedSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member  x.Add(k) = SortedSet(x.inner.Add(k))
        member  x.Remove k = SortedSet(x.inner.Remove k)
        member  x.Keys = x.keys;
        member  x.Contains(k) = x.inner.Contains k
        member  x.AddRange sq = SortedSet(x.inner.Union sq)
        member  x.RemoveRange sq = SortedSet(x.inner.Except sq)
        member  x.Length = x.inner.Count
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member  x.Union (o : SortedSet<'t>) = SortedSet(x.inner.Union o.inner)
        member  x.Intersect (s : SortedSet<'t>) = SortedSet(x.inner.Intersect s.inner)
        member  x.Except (s : SortedSet<'t>) = SortedSet(x.inner.Except s.inner)
        member  x.SymmetricDifference (s : SortedSet<'t>) = SortedSet(x.inner.SymmetricExcept s.inner)
        member  x.IsSetEqual (s : SortedSet<'t>) = x.inner.SetEquals(s.inner)
        member  x.IsProperSuperset (s : SortedSet<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member  x.IsProperSubset (s : SortedSet<'t>) = x.inner.IsProperSubsetOf(s.inner)
        static member  FromSeq(s : 't seq) = 
            let mutable set = SortedSet(s.ToImmutableSortedSet())
            set.keys <- s |> Seq.toArray
            set

        
///A module that contains light-weight wrappers for FSharpx collections.
module FSharpx = 
    module Vector' = FSharpx.Collections.Vector
    module Deque' = FSharpx.Collections.Deque
    module RanAccList' = FSharpx.Collections.RandomAccessList
    ///A light-weight wrapper for FSharpx.Vector
    [<Struct>]
    type Vector<'t when 't : equality> = 
        interface seq<'t> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        val public inner : FSharpx.Collections.Vector<'t>
        new (innerp) = {inner = innerp}
        member  x.AddLast v = Vector(x.inner.Conj v)
        member  x.get_Item i = x.inner.[i]
        member  x.Update(i,v) = Vector(x.inner.Update(i,v))
        member  x.RemoveLast() = Vector(x.inner |> Vector'.initial)
        member  x.GetEnumerator() = (x.inner :> seq<_>).GetEnumerator()
        member  x.AddLastList vs = Vector(Vector'.append x.inner vs)
        member  x.Length = x.inner.Length
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.AsSeq = x.inner |> seq
        member  x.ForEach (f : Action<_>) =
            for item in x.inner do
                f.Invoke(item)
        member  x.AddLastRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Conj v
            Vector(inner)
        static member  FromSeq(vs : 'a seq) = Vector(Vector'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.Deque
    [<Struct>]
    type Deque<'t> = 
        val public inner : FSharpx.Collections.Deque<'t>
        interface seq<'t> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        new (innerp) = {inner = innerp}
        member  x.AddLast v = Deque(x.inner.Conj v)
        member  x.AddFirst v = Deque(x.inner.Cons v)
        member  x.RemoveLast() = Deque(x.inner.Unconj |> fst)
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.RemoveFirst() = Deque(x.inner.Uncons |> snd)
        member  x.AsSeq = x.inner |> seq
        member  x.ForEach (f : Action<_>) =
            for item in x.inner do
                f.Invoke(item)
        member  x.Length = x.inner.Length
        member x.AddLastRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Conj v
            Deque(inner)
        member x.AddFirstRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Cons v
            Deque(inner)
        static member  FromSeq(vs : 'a seq) = Deque(Deque'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.RandomAccessList
    [<Struct>]    
    type RanAccList<'t when 't : equality> = 
        val public inner : FSharpx.Collections.RandomAccessList<'t>
        new (innerp) = {inner = innerp}
        member  x.AddLast v = RanAccList(x.inner.Cons v)
        member  x.get_Item i = x.inner.[i]
        member  x.Update(i,v) = RanAccList(x.inner.Update(i,v))
        member  x.RemoveLast() = RanAccList(x.inner |> RanAccList'.tail)
        member  x.AsSeq = x.inner |> seq
        member  x.Length = x.inner.Length
        member  x.IsEmpty = x.inner.IsEmpty
        static member  FromSeq(vs : 'a seq) = RanAccList(RanAccList'.ofSeq vs)

(* Sasa collections are buggy and cause problems
module Sasa = 
    [<Struct>]
    type FingerTree<'k> = 
        val public inner : Sasa.Collections.FingerTree<'k>
        val public len : int
        interface seq<'k> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        new(innerp, lenp) = {inner = innerp; len = lenp}
        member  x.AddLast v = FingerTree<_>(x.inner.Add(v), x.len + 1)
        member  x.AddFirst v = FingerTree<_>(x.inner.Push v, x.len + 1)
        member  x.RemoveLast() = FingerTree<_>(x.inner.Remove().First, x.len - 1)
        member  x.RemoveFirst() = FingerTree<_>(x.inner.Pop().First, x.len - 1)
        member  x.AsSeq = x.inner |> seq
        member  x.IsEmpty = x.inner.IsEmpty
        member  x.AddLastRange (sq : 'k seq) =
            match sq with
            | :? FingerTree<'k> as ft -> FingerTree<_>(x.inner.Concat(ft.inner), x.len + ft.len)
            | _ -> 
                let fold (st : Sasa.Collections.FingerTree<'k>) i = st.Add i
                FingerTree<_>(sq |> Seq.fold fold (x.inner), x.len + (sq |> Seq.length))
        member  x.AddFirstRange (sq : 'k seq) =
            match sq with
            | :? FingerTree<'k> as ft -> ft.AddLastRange x
            | _ -> 
                let empty = FingerTree<'k>(Sasa.Collections.FingerTree<'k>(), 0)
                let empty = empty.AddLastRange sq
                empty.AddLastRange x
        static member  FromSeq(vs : 'k seq) = FingerTree<_>(Sasa.Collections.FingerTree.Empty, 0).AddLastRange vs

    [<Struct>]
    type Vector<'k> = 
        val public inner : Sasa.Collections.Vector<'k>
        interface seq<'k> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        new(innerp) = {inner = innerp}
        member  x.AddLast v = Vector<_>(x.inner.Add v)
        member  x.AddLastRange vs = 
            let mutable v = x.inner
            for item in vs do
                v <- v.Add item
            Vector<_>(v)
        member  x.get_Item i = x.inner.Item i
        member  x.Update(i,v) = Vector<_>(x.inner.Set(i, v))
        member  x.Length = x.inner.Count
        member  x.AsSeq = x.inner |> seq
        member  x.IsEmpty = x.inner.Count = 0
        static member  FromSeq(vs : 'k seq) = Vector<_>(Sasa.Collections.Vector.Empty).AddLastRange vs
*)        
                
module FSharp =
    open System.Collections.Generic
    [<Struct>] 
    type Map<'k when 'k : comparison>= 
        val public inner : Map<'k,'k>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 'k array
        new (innerp) = {inner = innerp}  
        member  x.Add(k,v)= Map<_>(x.inner.Add(k,v))
        member  x.Remove k = Map<_>(x.inner.Remove(k))
        member  x.Contains k = x.inner.ContainsKey k
        member  x.Length = x.inner.Count
        member  x.get_Item k = x.inner.[k]
        member  x.Keys = x.keys
        member  x.RemoveRange sq = 
            let mutable map = x.inner
            for item in sq do
                map <- map.Remove item
            Map<_>(map)
        member  x.AddRange (sq : KeyValuePair<_,_> seq) =
            let mutable map = x.inner
            for item in sq do
                map <- map.Add(item.Key, item.Value)
            Map<_>(map)
        member  x.AsSeq = x.inner |> seq
        member  x.ForEach (act : Action<_>) = 
            for item in x.inner do
                act.Invoke item
        static member  FromSeq (vs : 'k seq) = 
            let arr = vs |> Seq.toArray
            let mutable m = Map.empty
            for k in arr do
                m <- m.Add(k,k)
            let mutable m = Map<_>(m)
            m.keys <- arr
            m
    [<Struct>]
    type Set<'t when 't : comparison>  =
        val public inner : Microsoft.FSharp.Collections.Set<'t>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't array
        new(innerp) = {inner=innerp}
        member  x.Add(k) = Set(x.inner.Add(k))
        member  x.Remove k = Set(x.inner.Remove k)
        member  x.Keys = x.keys
        member  x.Contains(k) = x.inner.Contains k
        member  x.Length = x.inner.Count
        member  x.AsSeq = x.inner :> _ seq
        member  x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member  x.Union (o : Set<'t>) = Set(x.inner |> Set.union o.inner)
        member  x.Intersect (s : Set<'t>) = Set(x.inner |> Set.intersect s.inner)
        member  x.Except (s : Set<'t>) = Set(Set.difference x.inner s.inner)
        member  x.SymmetricDifference (s : Set<'t>) = Set((x.inner |> Set.difference s.inner) + (s.inner |> Set.difference x.inner))
        member  x.IsSetEqual (s : Set<'t>) = x.inner.Equals(s.inner)
        member  x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member  x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member  x.AddRange vs = 
            let mutable col = x.inner
            for item in vs do
                col <- col.Add item
            Set(col)
        member  x.RemoveRange vs = 
            let mutable col = x.inner
            for item in vs do
                col <- col.Remove item
            Set(col)
        static member  FromSeq vs = 
            let mutable set = Set(vs |> Set.ofSeq)
            set.keys <- vs |> Seq.toArray
            set

