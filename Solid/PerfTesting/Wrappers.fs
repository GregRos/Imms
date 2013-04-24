namespace Benchmarks.Wrappers
#nowarn"20"
///A module with light-weight wrappers for the various System.Collections.Immutable classes.
module Sys = 
    open System.Collections.Immutable
    ///A light-weight wrapper for ImmutableList.
    [<Struct>]
    type List<'t> = 
        val public inner : ImmutableList<'t>
        new (innerp : ImmutableList<'t>) = {inner = innerp}
        member inline x.AddLast v           = List(x.inner.Add(v))
        member inline x.AddFirst v          = List(x.inner.Insert(0, v)) 
        member inline x.AddLastRange vs     = List(x.inner.AddRange(vs)) 
        member inline x.AddFirstRange vs    = List(x.inner.InsertRange(0, vs)) 
        member inline x.DropLast()          = List(x.inner.RemoveAt(x.inner.Count - 1)) 
        member inline x.DropFirst()         = List(x.inner.RemoveAt(0))
        member inline x.Slice(a,b)           = List(x.inner.GetRange(a, a+b))
        member inline x.Take a = List(x.inner.GetRange(0, a))
        member inline x.Insert(ix, v)       = List(x.inner.Insert(ix, v))
        member inline x.InsertRange(ix, vs) = List(x.inner.InsertRange(ix, vs))
        member inline x.GetEnumerator()     = (x.inner :> 't seq).GetEnumerator()
        member inline x.Set(ix, v)          = List(x.inner.SetItem(ix, v))
        member inline x.get_Item ix         = x.inner.[ix]
        member inline x.Count = x.inner.Count
        member inline x.Remove i = List(x.inner.RemoveAt(i))
        member inline x.IsEmpty = x.inner.IsEmpty
        static member inline FromSeq (vs : 'a seq) = List(ImmutableList.Create<'a>(vs))
    ///A light-weight wrapper for ImmutableQueue.
    [<Struct>]
    type Queue<'t> = 
        val public inner : ImmutableQueue<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = Queue(x.inner.Enqueue v)
        member inline x.DropFirst() = Queue(x.inner.Dequeue())
        member inline x.GetEnumerator() = x.inner.GetEnumerator()
        member inline x.First = x.inner.Peek
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.Count = x.inner |> Seq.length
        static member inline FromSeq (vs : 'a seq) = Queue(ImmutableQueue.Create<'a>(vs))
    ///A light-weight wrapper for ImmutableStack
    [<Struct>]
    type Stack<'t> = 
        val public inner : ImmutableStack<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddFirst v=  Stack(x.inner.Push v)
        member inline x.DropFirst() = Stack(x.inner.Pop())
        member inline x.GetEnumerator() = x.inner.GetEnumerator()
        member inline x.First = x.inner.Peek()
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.Count = x.inner |> Seq.length
        static member inline FromSeq(vs : 'a seq) = Stack(ImmutableStack.Create<'a>(vs))
///A module that contains light-weight wrappers for FSharpx collections.
module FSharpx' = 
    module Vector' = FSharpx.Collections.Vector
    module Deque' = FSharpx.Collections.Deque
    module RanAccList' = FSharpx.Collections.RandomAccessList
    ///A light-weight wrapper for FSharpx.Vector
    [<Struct>]
    type Vector<'t when 't : equality> = 
        val public inner : FSharpx.Collections.Vector<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = Vector(x.inner.Conj v)
        member inline x.get_Item i = x.inner.[i]
        member inline x.Set(i,v) = Vector(x.inner.Update(i,v))
        member inline x.DropLast() = Vector(x.inner |> Vector'.initial)
        member inline x.GetEnumerator() = (x.inner :> seq<_>).GetEnumerator()
        member inline x.AddLastList vs = Vector(Vector'.append x.inner vs)
        member inline x.Count = x.inner.Length
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.AddLastRange vs =
            let mutable inner = x.inner
            for v in vs do inner <- inner.Conj v
            Vector(inner)
        static member inline FromSeq(vs : 'a seq) = Vector(Vector'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.Deque
    [<Struct>]
    type Deque<'t> = 
        val public inner : FSharpx.Collections.Deque<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = Deque(x.inner.Conj v)
        member inline x.AddFirst v = Deque(x.inner.Cons v)
        member inline x.DropLast() = Deque(x.inner.Unconj |> fst)
        member inline x.IsEmpty = x.inner.IsEmpty
        member inline x.DropFirst() = Deque(x.inner.Uncons |> snd)
        member inline x.Count = x.inner.Length
        static member inline FromSeq(vs : 'a seq) = Deque(Deque'.ofSeq vs)
    ///A light-weight wrapper for FSharpx.RandomAccessList
    [<Struct>]    
    type RanAccList<'t when 't : equality> = 
        val public inner : FSharpx.Collections.RandomAccessList<'t>
        new (innerp) = {inner = innerp}
        member inline x.AddLast v = RanAccList(x.inner.Cons v)
        member inline x.get_Item i = x.inner.[i]
        member inline x.Set(i,v) = RanAccList(x.inner.Update(i,v))
        member inline x.DropLast() = RanAccList(x.inner |> RanAccList'.tail)
        member inline x.Count = x.inner.Length
        member inline x.IsEmpty = x.inner.IsEmpty
        static member inline FromSeq(vs : 'a seq) = RanAccList(RanAccList'.ofSeq vs)
        