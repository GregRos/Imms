namespace Funq.Tests.Performance.Wrappers
open System
open Funq.Abstract
#nowarn"20"
///A module with light-weight wrappers for the various System.Collections.Immutable classes.

module Funq = 
    open Funq.Collections
    open Funq
    open System.Collections.Generic
    [<Struct>]
    type FunqMap<'t> = 
        val public inner : FunqMap<'t, 't>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't[]
        new (innerp) = {inner = innerp}
        member inline x.Add(k,v) = FunqMap(x.inner.Set(k,v))
        member inline x.get_Item k = x.inner.TryGet(k).Value
        member inline x.AddRange (sq : KeyValuePair<_,_> seq) = FunqMap(x.inner.SetRange sq)
        member inline x.RemoveRange sq = FunqMap(x.inner.RemoveRange sq)
        member inline x.Remove item = FunqMap(x.inner.Remove item)
        member inline x.Length = x.inner.Length
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = x.inner.ForEach (a)
        member inline x.Keys = x.keys
        static member inline FromSeq(s : 't seq) = 
            let mutable mp = [for item in s -> Kvp.Of(item, item)].ToFunqMap() |> fun x -> FunqMap(x)
            mp.keys <- s |> Seq.toArray
            mp

    [<Struct>]
    type OrderedMap<'t when 't :> IComparable<'t>> = 
        val public inner : FunqOrderedMap<'t, 't>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't[]
        new (innerp) = {inner = innerp}
        member inline x.Add(k,v) = OrderedMap<_>(x.inner.Set(k,v))
        member inline x.ForEach (a : Action<_>) = x.inner.ForEach(a)
        member inline x.Length = x.inner.Length
        member inline x.AddRange (sq : KeyValuePair<_,_> seq) = OrderedMap<_>(x.inner.SetRange sq)
        member inline x.RemoveRange sq = OrderedMap<_>(x.inner.RemoveRange sq)
        member inline x.Remove item = OrderedMap(x.inner.Remove item)
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.get_Item k = x.inner.TryGet(k).Value
        member inline x.Keys = x.keys
        static member inline FromSeq(s : 't seq) = 
            let mutable mp = [for item in s -> Kvp.Of(item, item)].ToFunqOrderedMap() |> fun x -> OrderedMap(x)
            mp.keys <- s |> Seq.toArray
            mp
    [<Struct>]
    type Set<'t> = 
        val public inner : FunqSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member inline x.Add(k) = Set(x.inner.Add(k))
        member inline x.Contains(k) = x.inner.Contains k
        member inline x.Remove(k) = Set(x.inner.Remove k)
        member inline x.ForEach a = x.inner.ForEach a
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.AddRange sq = Set(x.inner.Union sq)
        member inline x.Length = x.inner.Length
        member inline x.RemoveRange sq = Set(x.inner.Except sq)
        member inline x.Union (o : Set<'t>) = Set(o.inner.Union x.inner)
        member inline x.Intersect (s : Set<'t>) = Set(s.inner.Intersect x.inner)
        member inline x.Except (s : Set<'t>) = Set(s.inner.Except x.inner)
        member inline x.SymmetricDifference (s : Set<'t>) = Set(s.inner.Difference x.inner)
        member inline x.IsSetEqual (s : Set<'t>) = x.inner.SetEquals(s.inner)
        member inline x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member inline x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member inline x.Keys = x.keys
        static member inline FromSeq(s : 't seq) = 
            let mutable set = Set(s.ToFunqSet())
            set.keys <- s |> Seq.toArray
            set

    type OrderedSet<'t when 't :> IComparable<'t>> = 
        val public inner : FunqOrderedSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member inline x.Add(k) = OrderedSet(x.inner.Add(k))
        member inline x.Contains(k) = x.inner.Contains k
        member inline x.Remove(k) = OrderedSet(x.inner.Remove k)
        member inline x.ForEach (a : Action<'t>) = x.inner.ForEach a
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.AddRange sq = OrderedSet(x.inner.Union sq)
        member inline x.Length = x.inner.Length
        member inline x.RemoveRange sq = OrderedSet(x.inner.Except sq)
        member inline x.Union (o : OrderedSet<'t>) = OrderedSet(x.inner.Union o.inner)
        member inline x.Intersect (s : OrderedSet<'t>) = OrderedSet(x.inner.Intersect s.inner)
        member inline x.Except (s : OrderedSet<'t>) = OrderedSet(x.inner.Except s.inner)
        member inline x.SymmetricDifference (s : OrderedSet<'t>) = OrderedSet(x.inner.Difference s.inner)
        member inline x.IsSetEqual (s : OrderedSet<'t>) = x.inner.SetEquals(s.inner)
        member inline x.IsProperSuperset (s : OrderedSet<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member inline x.IsProperSubset (s : OrderedSet<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member inline x.Keys = x.keys
        static member inline FromSeq(s : 't seq) = 
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
        member inline x.AddLast v           = List(x.inner.Add(v))
        member inline x.AddFirst v          = List(x.inner.Insert(0, v)) 
        member inline x.AddLastRange vs     = List(x.inner.AddRange(vs)) 
        member inline x.AddFirstRange vs    = List(x.inner.InsertRange(0, vs)) 
        member inline x.RemoveLast()          = List(x.inner.RemoveAt(x.inner.Count - 1)) 
        member inline x.RemoveFirst()         = List(x.inner.RemoveAt(0))
        member inline x.Slice(a,b)           = List(x.inner.GetRange(a, a+b))
        member inline x.Take a = List(x.inner.GetRange(0, a))
        member inline x.Skip a = List(x.inner.GetRange(a, x.inner.Count - a))
        member inline x.Insert(ix, v)       = List(x.inner.Insert(ix, v))
        member inline x.InsertRange(ix, vs) = List(x.inner.InsertRange(ix, vs))
        member inline x.GetEnumerator()     = (x.inner :> 't seq).GetEnumerator()
        member inline x.Update(ix, v)          = List(x.inner.SetItem(ix, v))
        member inline x.get_Item ix         = x.inner.[ix]
        member inline x.Length = x.inner.Count
        member inline x.Remove i = List(x.inner.RemoveAt(i))
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.ForEach f = x.inner.ForEach(f)
        member x.AsSeq = x.inner |> seq
        static member inline FromSeq (vs : 'a seq) = List(ImmutableList.CreateRange(vs))
    ///A light-weight wrapper for ImmutableQueue.
    [<Struct>]
    type Queue<'t> = 
        val public inner : ImmutableQueue<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = Queue(x.inner.Enqueue v)
        member inline x.RemoveFirst() = Queue(x.inner.Dequeue())
        member inline x.GetEnumerator() = x.inner.GetEnumerator()
        member inline x.First = x.inner.Peek
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.Length = x.inner |> Seq.length
        member inline x.AsSeq = x.inner |> seq
        static member inline FromSeq (vs : 'a seq) = Queue(ImmutableQueue.CreateRange(vs))
    ///A light-weight wrapper for ImmutableStack
    [<Struct>]
    type Stack<'t> = 
        val public inner : ImmutableStack<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddFirst v=  Stack(x.inner.Push v)
        member inline x.RemoveFirst() = Stack(x.inner.Pop())
        member inline x.GetEnumerator() = x.inner.GetEnumerator()
        member inline x.First = x.inner.Peek()
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.Length = x.inner |> Seq.length
        member inline x.AsSeq = x.inner |> seq
        static member inline FromSeq(vs : 'a seq) = Stack(ImmutableStack.CreateRange(vs))
    [<Struct>]
    type Dict<'t> = 
        val public inner : ImmutableDictionary<'t,'t>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member inline x.Add(k,v) = Dict(x.inner.SetItem(k,v))
        member inline x.Length = x.inner.Count
        member inline x.Remove item = Dict(x.inner.Remove item)
        member inline x.AddRange (vs : KeyValuePair<_,_> seq) = Dict(x.inner.SetItems vs)
        member inline x.RemoveRange vs = Dict(x.inner.RemoveRange vs)
        member inline x.get_Item k =x.inner.[k]
        member inline x.Keys = x.keys
        static member inline FromSeq (s : 't seq) = 
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
        member inline x.Add(k,v) = SortedDict(x.inner.SetItem(k,v))
        member inline x.AddRange (sq : KeyValuePair<_,_> seq) = SortedDict(x.inner.SetItems sq)
        member inline x.Remove item = SortedDict(x.inner.Remove item)
        member inline x.Length = x.inner.Count
        member inline x.RemoveRange sq = 
            SortedDict(sq |> x.inner.RemoveRange)
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member inline x.get_Item k =x.inner.[k]
        member inline x.Keys = x.keys
        static member inline FromSeq (s : 't seq) = 
            let cmK = Cm<_>.Default
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
        member inline x.Add(k) = Set(x.inner.Add(k))
        member inline x.Keys = x.keys;
        member inline x.Remove k = Set(x.inner.Remove k)
        member inline x.Contains(k) = x.inner.Contains k
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member inline x.Length = x.inner.Count
        member inline x.AddRange (s : 't seq) = Set(x.inner.Union s)
        member inline x.RemoveRange (s : 't seq) = Set(x.inner.Except s)
        member inline x.Union (o : Set<'t>) = Set(x.inner.Union o.inner)
        member inline x.Intersect (s : Set<'t>) = Set(x.inner.Intersect s.inner)
        member inline x.Except (s : Set<'t>) = Set(x.inner.Except s.inner)
        member inline x.SymmetricDifference (s : Set<'t>) = Set(x.inner.SymmetricExcept s.inner)
        member inline x.IsSetEqual (s : Set<'t>) = x.inner.SetEquals(s.inner)
        member inline x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member inline x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        static member inline FromSeq(s : 't seq) = 
            let mutable set = Set(s.ToImmutableHashSet())
            set.keys <- s |> Seq.toArray
            set

    [<Struct>]
    type SortedSet<'t> = 
        val public inner : ImmutableSortedSet<'t>
        [<DefaultValueAttribute>]
        val mutable public keys : 't array
        new (innerp) = {inner = innerp}
        member inline x.Add(k) = SortedSet(x.inner.Add(k))
        member inline x.Remove k = SortedSet(x.inner.Remove k)
        member inline x.Keys = x.keys;
        member inline x.Contains(k) = x.inner.Contains k
        member inline x.AddRange sq = SortedSet(x.inner.Union sq)
        member inline x.RemoveRange sq = SortedSet(x.inner.Except sq)
        member inline x.Length = x.inner.Count
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member inline x.Union (o : SortedSet<'t>) = SortedSet(x.inner.Union o.inner)
        member inline x.Intersect (s : SortedSet<'t>) = SortedSet(x.inner.Intersect s.inner)
        member inline x.Except (s : SortedSet<'t>) = SortedSet(x.inner.Except s.inner)
        member inline x.SymmetricDifference (s : SortedSet<'t>) = SortedSet(x.inner.SymmetricExcept s.inner)
        member inline x.IsSetEqual (s : SortedSet<'t>) = x.inner.SetEquals(s.inner)
        member inline x.IsProperSuperset (s : SortedSet<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member inline x.IsProperSubset (s : SortedSet<'t>) = x.inner.IsProperSubsetOf(s.inner)
        static member inline FromSeq(s : 't seq) = 
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
        member inline x.AddLast v = Vector(x.inner.Conj v)
        member inline x.get_Item i = x.inner.[i]
        member inline x.Update(i,v) = Vector(x.inner.Update(i,v))
        member inline x.RemoveLast() = Vector(x.inner |> Vector'.initial)
        member inline x.GetEnumerator() = (x.inner :> seq<_>).GetEnumerator()
        member inline x.AddLastList vs = Vector(Vector'.append x.inner vs)
        member inline x.Length = x.inner.Length
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.AsSeq = x.inner |> seq
        member inline x.ForEach (f : Action<_>) =
            for item in x.inner do
                f.Invoke(item)
        member inline x.AddLastRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Conj v
            Vector(inner)
        static member inline FromSeq(vs : 'a seq) = Vector(Vector'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.Deque
    [<Struct>]
    type Deque<'t> = 
        val public inner : FSharpx.Collections.Deque<'t>
        interface seq<'t> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = Deque(x.inner.Conj v)
        member inline x.AddFirst v = Deque(x.inner.Cons v)
        member inline x.RemoveLast() = Deque(x.inner.Unconj |> fst)
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.RemoveFirst() = Deque(x.inner.Uncons |> snd)
        member inline x.AsSeq = x.inner |> seq
        member inline x.ForEach (f : Action<_>) =
            for item in x.inner do
                f.Invoke(item)
        member inline x.Length = x.inner.Length
        member inline x.AddLastRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Conj v
            Deque(inner)
        member inline x.AddFirstRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Cons v
            Deque(inner)
        static member inline FromSeq(vs : 'a seq) = Deque(Deque'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.RandomAccessList
    [<Struct>]    
    type RanAccList<'t when 't : equality> = 
        val public inner : FSharpx.Collections.RandomAccessList<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = RanAccList(x.inner.Cons v)
        member inline x.get_Item i = x.inner.[i]
        member inline x.Update(i,v) = RanAccList(x.inner.Update(i,v))
        member inline x.RemoveLast() = RanAccList(x.inner |> RanAccList'.tail)
        member inline x.AsSeq = x.inner |> seq
        member inline x.Length = x.inner.Length
        member inline x.IsEmpty = x.inner.IsEmpty
        static member inline FromSeq(vs : 'a seq) = RanAccList(RanAccList'.ofSeq vs)

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
        member inline x.AddLast v = FingerTree<_>(x.inner.Add(v), x.len + 1)
        member inline x.AddFirst v = FingerTree<_>(x.inner.Push v, x.len + 1)
        member inline x.RemoveLast() = FingerTree<_>(x.inner.Remove().First, x.len - 1)
        member inline x.RemoveFirst() = FingerTree<_>(x.inner.Pop().First, x.len - 1)
        member inline x.AsSeq = x.inner |> seq
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.AddLastRange (sq : 'k seq) =
            match sq with
            | :? FingerTree<'k> as ft -> FingerTree<_>(x.inner.Concat(ft.inner), x.len + ft.len)
            | _ -> 
                let fold (st : Sasa.Collections.FingerTree<'k>) i = st.Add i
                FingerTree<_>(sq |> Seq.fold fold (x.inner), x.len + (sq |> Seq.length))
        member inline x.AddFirstRange (sq : 'k seq) =
            match sq with
            | :? FingerTree<'k> as ft -> ft.AddLastRange x
            | _ -> 
                let empty = FingerTree<'k>(Sasa.Collections.FingerTree<'k>(), 0)
                let empty = empty.AddLastRange sq
                empty.AddLastRange x
        static member inline FromSeq(vs : 'k seq) = FingerTree<_>(Sasa.Collections.FingerTree.Empty, 0).AddLastRange vs

    [<Struct>]
    type Vector<'k> = 
        val public inner : Sasa.Collections.Vector<'k>
        interface seq<'k> with
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator() :> System.Collections.IEnumerator
            member x.GetEnumerator() = (x.inner :> _ seq).GetEnumerator():>System.Collections.Generic.IEnumerator<_>
        new(innerp) = {inner = innerp}
        member inline x.AddLast v = Vector<_>(x.inner.Add v)
        member inline x.AddLastRange vs = 
            let mutable v = x.inner
            for item in vs do
                v <- v.Add item
            Vector<_>(v)
        member inline x.get_Item i = x.inner.Item i
        member inline x.Update(i,v) = Vector<_>(x.inner.Set(i, v))
        member inline x.Length = x.inner.Count
        member inline x.AsSeq = x.inner |> seq
        member inline x.IsEmpty = x.inner.Count = 0
        static member inline FromSeq(vs : 'k seq) = Vector<_>(Sasa.Collections.Vector.Empty).AddLastRange vs
*)        
                
module FSharp =
    open System.Collections.Generic
    [<Struct>] 
    type Map<'k when 'k : comparison>= 
        val public inner : Map<'k,'k>
        [<DefaultValueAttribute(false)>]
        val mutable public keys : 'k array
        new (innerp) = {inner = innerp}  
        member inline x.Add(k,v)= Map<_>(x.inner.Add(k,v))
        member inline x.Remove k = Map<_>(x.inner.Remove(k))
        member inline x.Contains k = x.inner.ContainsKey k
        member inline x.Length = x.inner.Count
        member inline x.get_Item k = x.inner.[k]
        member inline x.Keys = x.keys
        member inline x.RemoveRange sq = 
            let mutable map = x.inner
            for item in sq do
                map <- map.Remove item
            Map<_>(map)
        member inline x.AddRange (sq : KeyValuePair<_,_> seq) =
            let mutable map = x.inner
            for item in sq do
                map <- map.Add(item.Key, item.Value)
            Map<_>(map)
        member inline x.AsSeq = x.inner |> seq
        member inline x.ForEach (act : Action<_>) = 
            for item in x.inner do
                act.Invoke item
        static member inline FromSeq (vs : 'k seq) = 
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
        member inline x.Add(k) = Set(x.inner.Add(k))
        member inline x.Remove k = Set(x.inner.Remove k)
        member inline x.Keys = x.keys
        member inline x.Contains(k) = x.inner.Contains k
        member inline x.Length = x.inner.Count
        member inline x.AsSeq = x.inner :> _ seq
        member inline x.ForEach (a : Action<_>) = 
            for item in x.inner do a.Invoke(item)
        member inline x.Union (o : Set<'t>) = Set(x.inner |> Set.union o.inner)
        member inline x.Intersect (s : Set<'t>) = Set(x.inner |> Set.intersect s.inner)
        member inline x.Except (s : Set<'t>) = Set(Set.difference x.inner s.inner)
        member inline x.SymmetricDifference (s : Set<'t>) = Set((x.inner |> Set.difference s.inner) + (s.inner |> Set.difference x.inner))
        member inline x.IsSetEqual (s : Set<'t>) = x.inner.Equals(s.inner)
        member inline x.IsProperSuperset (s : Set<'t>) = x.inner.IsProperSupersetOf(s.inner)
        member inline x.IsProperSubset (s : Set<'t>) = x.inner.IsProperSubsetOf(s.inner)
        member inline x.AddRange vs = 
            let mutable col = x.inner
            for item in vs do
                col <- col.Add item
            Set(col)
        member inline x.RemoveRange vs = 
            let mutable col = x.inner
            for item in vs do
                col <- col.Remove item
            Set(col)
        static member inline FromSeq vs = 
            let mutable set = Set(vs |> Set.ofSeq)
            set.keys <- vs |> Seq.toArray
            set

