module SolidTests.Targets
open System
open System.Collections
open System.Collections.Generic

type MutableList<'t> = Generic.List<'t>
type MutableDict<'t> = Generic.Dictionary<'t,'t>
type FSMap<'t when 't : comparison> = Map<'t,'t>
type FSList<'t> = list<'t>
type MutableQueue<'t> = Generic.Queue<'t>
open SolidFS.Operators
open SolidFS.Expressions
open Solid
open SolidFS


type TestNotAvailableException<'a>(target : TestTarget<'a>) = 
    inherit Exception( "The test is not available for the object.")


and [<AbstractClass>] 
    TestTarget<'a>(name) as this = 
    let not_available = TestNotAvailableException(this)
    abstract member Name          : string
    abstract member AddLast       : 'a             -> TestTarget<'a>
    abstract member AddFirst      : 'a             -> TestTarget<'a>
    abstract member Length        : unit           ->  int
    abstract member DropLast      : unit           -> TestTarget<'a>
    abstract member DropFirst     : unit           -> TestTarget<'a>
    abstract member Get           : int            -> 'a
    abstract member Set           : int            -> 'a -> TestTarget<'a>
    abstract member First         : unit           -> 'a
    abstract member Last          : unit           ->   'a
    abstract member AddLastRange  : seq<'a>        -> TestTarget<'a>
    abstract member AddFirstRange : seq<'a>        -> TestTarget<'a>
    abstract member Insert        : int            -> 'a -> TestTarget<'a>
    abstract member RemoveAt      : int            -> TestTarget<'a>
    abstract member InsertRange   : int            -> seq<'a> -> TestTarget<'a>
    abstract member TakeFirst     : int            -> TestTarget<'a>
    abstract member TakeLast      : int            -> TestTarget<'a>
    abstract member Slice         : int            -> int -> TestTarget<'a>
    abstract member Concat        : TestTarget<'a> -> TestTarget<'a>
    abstract member AsSeq         : unit           -> seq<'a>
    abstract member FromSeq       : seq<'a>        -> TestTarget<'a>
    abstract member IsEmpty       : bool
    abstract member AddLastList   : TestTarget<'a> -> TestTarget<'a>
    abstract member Filter        : ('a            -> bool) -> TestTarget<'a>
    abstract member Map           : ('a            -> 'b) -> TestTarget<'b>  when 'b : equality
    abstract member Reverse       : unit           -> TestTarget<'a>
    abstract member IndexOf       : ('a            -> bool) -> int option
    abstract member Verify : unit -> bool
    default __.Name            = name
    default __.AddLast _       = raise not_available
    default __.AddFirst _      = raise not_available
    default __.Length()        = raise not_available
    default __.DropFirst ()    = raise not_available
    default __.DropLast ()     = raise not_available
    default __.Get _           = raise not_available
    default __.Set _ _         = raise not_available
    default __.First()         = raise not_available
    default __.Last()          = raise not_available
    default __.AddFirstRange _ = raise not_available
    default __.AddLastRange _  = raise not_available
    default __.Insert _ _      = raise not_available
    default __.RemoveAt _      = raise not_available
    default __.InsertRange _ _ = raise not_available
    default __.TakeFirst _     = raise not_available
    default __.TakeLast _      = raise not_available
    default __.Concat _        = raise not_available
    default __.AsSeq ()        = raise not_available
    default __.AddLastList t   = raise not_available
    default __.Reverse () = raise not_available
    default __.Slice _ _       = raise not_available
    default __.Filter _        = raise not_available
    default __.Map _           = raise not_available
    default __.IndexOf _ = raise not_available
    default __.Verify () = raise not_available

module Seq = 
    let pivot (sqs : 'a seq array) : 'a array seq = 
        let states = sqs |> Array.map (fun x -> x.GetEnumerator())
        seq {
            while (states |> Array.map (fun x -> x.MoveNext()) |> Seq.distinct |> Seq.exactlyOne) do
                yield states |> Array.map (fun x -> x.Current)
        }
    let are_the_same (items : #seq<_>) : bool = 
        items |> Seq.pairwise |> Seq.map (fun (a,b) -> a.Equals(b)) |> Seq.forall id


    
//The following class is for consistency tests, not performance tests. It shouldn't be used in benchmarks.
type TestGroup<'a when 'a : equality>(inner : 'a TestTarget array) = 
    inherit TestTarget<'a>("Group")
    static let cns x                      = TestGroup<'b>(x) :> TestTarget<'b>
    override this.AddLast v        = inner |> Array.map (fun x -> x.AddLast v) |>        cns
    override this.AddFirst v       = inner |> Array.map (fun x -> x.AddFirst v) |>       cns
    override this.DropLast()       = inner |> Array.map (fun x -> x.DropLast()) |>       cns
    override this.DropFirst()      = inner |> Array.map (fun x -> x.DropFirst()) |>      cns
    override this.AddLastRange vs  = inner |> Array.map (fun x -> x.AddLastRange vs) |>  cns
    override this.AddFirstRange vs = inner |> Array.map (fun x -> x.AddFirstRange vs) |> cns
    override this.Insert i v       = inner |> Array.map (fun x -> x.Insert i v) |>       cns
    override this.InsertRange i vs = inner |> Array.map (fun x -> x.InsertRange i vs) |> cns
    override this.Length()         = inner |> Array.map (fun x -> x.Length()) |> Seq.distinct |> Seq.exactlyOne
    override this.First()          = inner |> Array.map (fun x -> x.First()) |> Seq.distinct |> Seq.exactlyOne
    override this.Last()           = inner |> Array.map (fun x -> x.Last()) |> Seq.distinct |> Seq.exactlyOne
    override this.Get i            = inner |> Array.map (fun x -> x.Get i) |> Seq.distinct |> Seq.exactlyOne
    override this.Set i v          = inner |> Array.map (fun x -> x.Set i v) |>          cns
    override this.Filter f         = inner |> Array.map (fun x -> x.Filter f) |>         cns
    override this.Map f            = inner |> Array.map (fun x -> x.Map f) |>            cns
    override this.IndexOf f        = inner |> Array.map (fun x -> x.IndexOf f) |> Seq.distinct |> Seq.exactlyOne
    override this.IsEmpty          = inner |> Array.map (fun x -> x.IsEmpty) |> Seq.distinct |> Seq.exactlyOne
    override this.FromSeq vs       = inner |> Array.map (fun x -> x.FromSeq vs) |> cns
    override this.Verify () = inner |> Array.map (fun x -> x.AsSeq()) |> Seq.pivot |> Seq.map (fun x -> x |> Seq.are_the_same) |> Seq.forall id
    override this.AsSeq() = inner |> Array.map (fun x -> x.AsSeq()) |> Seq.pivot |> Seq.map (Seq.distinct >> Seq.exactlyOne)
    override this.TakeFirst i = inner |> Array.map (fun x -> x.TakeFirst i) |> cns
    override this.TakeLast i = inner |> Array.map (fun x -> x.TakeLast i) |> cns
    override this.Slice i1 i2 = inner |> Array.map (fun x -> x.Slice i1 i2) |> cns
module FSHARPX = 
    open FSharpx.Collections

    type TestVector<'a when 'a : equality>(inner : FSharpx.Collections.Vector<'a>) =
        inherit TestTarget<'a>("FSharpx...Vector")
        let cns x                = TestVector(x) :> TestTarget<_>
        
        override this.AddLast v  = inner |> Vector.conj v |> cns
        override this.Length ()  = inner.Length
        override this.DropLast ()= inner |> Vector.unconj|> fst |> cns
        override this.Get i      = inner.[i]
        override this.Set i v    = inner.Update(i,v) |> cns
        override this.First ()= inner.[0]
        override this.Last ()= inner.Last
        override this.AsSeq ()   = inner :> _
        override this.IsEmpty    = inner.IsEmpty
        override this.FromSeq vs = vs |> Vector.ofSeq |> cns
        override this.AddLastRange vs = 
            let mutable x = inner
            for i in vs do
                x <- x |> FSharpx.Collections.Vector.conj i
            x |> cns
        static member Empty = TestVector<'a>(FSharpx.Collections.Vector.empty) :> TestTarget<'a>

    type TestDeque<'a>(inner : FSharpx.Collections.Deque<'a>) = 
        inherit TestTarget<'a>("FSharpx...Deque")
        let cns x = TestDeque<'a>(x) :> TestTarget<_>
        override this.AddLast v = inner |> Deque.conj v |> cns
        override this.AddFirst v = inner |> Deque.cons v |> cns
        override this.DropLast() = inner |> Deque.unconj |> fst |> cns
        override this.DropFirst ()= inner |> Deque.uncons |> snd|>  cns
        override this.Length () = inner.Length
        override this.First ()= inner.Head
        
        override this.Last() = inner.Last
        override this.IsEmpty = inner.IsEmpty
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = vs |> Deque.ofSeq |> cns
        static member Empty = TestDeque(Deque.empty)  :> TestTarget<'a>
    module SkewList = FSharpx.Collections.Experimental.SkewBinaryRandomAccessList
    type SkewList<'a> = FSharpx.Collections.Experimental.SkewBinaryRandomAccessList<'a>
    type TestSkewList<'a>(inner : SkewList<'a>) = 
        inherit TestTarget<'a>("FSharpx...SkewBinaryRandomAccessList")
        let cns  x= TestSkewList<'a>(x) :> TestTarget<'a>

        override this.AddFirst v = inner |> SkewList.cons v |> cns
        override this.DropFirst() = inner |> SkewList.tail |> cns
        override this.Get i = inner |> SkewList.lookup i
        override this.Length() = inner |> SkewList.length
        override this.First() = inner |> SkewList.head
        override this.IsEmpty = inner.IsEmpty
        override this.Last() = inner |> SkewList.lookup (inner.Length() - 1)
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = vs |> SkewList.ofSeq |> cns
        static member Empty = TestSkewList<'a>(SkewList.empty())  :> TestTarget<'a>
    type RealDeque<'a> = FSharpx.Collections.Experimental.RealTimeDeque<'a>
    module RealDeque = FSharpx.Collections.Experimental.RealTimeDeque
    type TestRealDeque<'a>(inner : RealDeque<'a>) = 
        inherit TestTarget<'a>("FSharpx...RealTimeDeque")
        let cns x = TestRealDeque x :> TestTarget<'a>

        override this.AddFirst v = inner |> RealDeque.cons v |> cns
        override this.AddLast v = inner |> RealDeque.snoc v |> cns
        override this.DropFirst () = inner |> RealDeque.tail |> cns
        override this.DropLast () = inner |> RealDeque.unsnoc |> fst |> cns
        override this.Get i = inner |> RealDeque.lookup i
        
        override this.IsEmpty = inner.IsEmpty
        override this.Last () = inner |> RealDeque.last
        override this.First() = inner |> RealDeque.head
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = vs |> RealDeque.ofSeq |> cns
        static member Empty = TestRealDeque<'a>(RealDeque.empty 2)  :> TestTarget<'a>
module CORE = 
    type TestList<'a>(inner : System.Collections.Immutable.IImmutableList<'a>) = 
        inherit TestTarget<'a>("System...ImmutableList")
        static let cns x = TestList<'b>(x) :> TestTarget<'b>
        static let cns_sq (x : 'b seq) = TestList<'b>(Immutable.ImmutableList.Create<'b>(x)) :> TestTarget<'b>
        override this.AddLast v= inner.Add v |> cns
        override this.AddLastRange vs = inner.AddRange vs |> cns
        override this.AddFirst v = inner.Insert(0, v) |> cns
        override this.AddFirstRange vs = inner.InsertRange(0, vs) |> cns
        override this.Last () = inner.[inner.Count - 1]
        override this.First() = inner.[0]
        override this.Get i = inner.[i]
        override this.Set i v = inner.SetItem(i,v) |> cns
        override this.IsEmpty = inner.Count = 0
        override this.Slice i1 i2 = inner |> Seq.skip i1 |> Seq.take (i2 - i1 + 1) |> cns_sq
        override this.DropLast() = inner.RemoveAt(inner.Count - 1) |> cns
        override this.DropFirst() = inner.RemoveAt(0) |> cns
        override this.Insert i v = inner.Insert(i,v) |> cns
        override this.RemoveAt i = inner.RemoveAt i |> cns
        override this.Length ()= inner.Count
        override this.InsertRange i vs = inner.InsertRange(i,vs) |> cns
        override this.TakeFirst i = inner |> Seq.take i |> cns_sq
        override this.TakeLast i = inner |> Seq.skip (inner.Count - i) |> cns_sq
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = cns_sq vs
        override this.Map f = inner |> Seq.map f |> cns_sq
        override this.Filter f = inner |> Seq.filter f |> cns_sq
        static member Empty = TestList<'a>(Immutable.ImmutableList.Create())  :> TestTarget<'a>
    type TestStack<'a> (inner : System.Collections.Immutable.IImmutableStack<'a>) = 
        inherit TestTarget<'a>("System...ImmutableStack")
        let cns x = TestStack<'a> x :> TestTarget<_>
        override this.IsEmpty = inner.IsEmpty
        
        override this.AddFirst v = inner.Push v |> cns
        override this.DropFirst () = inner.Pop() |> cns
        override this.First ()= inner.Peek()
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = Immutable.ImmutableStack.Create<'a>(vs) |> cns
        static member Empty = TestStack<'a>(Immutable.ImmutableStack.Create())  :> TestTarget<'a>
    type TestQueue<'a> (inner : System.Collections.Immutable.IImmutableQueue<'a>) =    
        inherit TestTarget<'a>("System...ImmutableQueue")
        let cns x = TestQueue<'a> x :> TestTarget<_>
        override this.IsEmpty = inner.IsEmpty
        override this.AddLast v = inner.Enqueue v |> cns
        override this.DropFirst() = inner.Dequeue() |> cns
        override this.First() = inner.Peek()
        override this.AsSeq () = inner :> _
        override this.FromSeq vs = Immutable.ImmutableQueue.Create<'a>(vs) |> cns
        static member Empty = TestQueue<'a>(Immutable.ImmutableQueue.Create())  :> TestTarget<'a>

module FSHARP = 
    type TestFSList<'a>( inner : list<'a>) =
        inherit TestTarget<'a>("FSharp...list")
        let cns x = TestFSList<'a>(x) :> TestTarget<'a>

        override this.AddFirst v = v::inner |> cns

        override this.DropFirst ()= inner.Tail |> cns

        override this.First ()= inner.Head
        override this.IsEmpty = inner.IsEmpty


        override this.Length ()= inner |> List.length

        override this.FromSeq vs = vs |> List.ofSeq |> cns

        override this.AsSeq () = inner :> _

        static member Empty = TestFSList<'a>([])  :> TestTarget<'a>

module SOLID = 
    type TestVector<'a>(inner : Solid.Vector<'a>) = 
        inherit TestTarget<'a>("Solid...Vector")
        let cns x = TestVector<'a>(x) :> TestTarget<'a>
        
        override __.AddLast v = inner.AddLast v |> cns
        override __.First() = inner.[0]
        override __.Last() = inner.[inner.Count-1]
        override __.DropLast() = inner.DropLast() |> cns
        
        override __.Length() = inner.Count
        override __.IsEmpty = inner.Count = 0
        override __.Get i = inner.[i]
        override __.Set i v = inner.Set(i, v)|> cns
        override __.TakeFirst i = inner.Take i |> cns
        override __.AsSeq () = inner :>_
        override __.FromSeq vs = vs.ToVector() |> cns
        
        static member Empty = TestVector<'a>(Solid.Vector.Empty())
    type TestXList<'a>(inner : xlist<'a>) = 
        inherit TestTarget<'a>("Solid...Sequence")
        static let cns (x : xlist<'b>) : TestTarget<'b> = TestXList<'b>(x) :> TestTarget<'b>
        member __.Inner = inner
        override __.AddFirst v = inner.AddFirst v |> cns
        override __.AddLast v = inner.AddLast v |> cns
        override __.Get i = inner.[i]
        override __.TakeFirst i = inner.Slice(0,i-1) |> cns
        override __.Slice i1 i2 = inner.Slice(i1,i2) |> cns
        override __.IsEmpty = inner.IsEmpty
        override __.DropFirst() = inner.DropFirst() |> cns
        override __.DropLast() = inner.DropLast() |> cns
        override __.TakeLast i = inner.Slice(-i,-1) |> cns
        override __.AsSeq() = inner :>_
        override __.Length() = inner.Count
        override __.FromSeq vs = vs |> XList.ofSeq |> cns
        override __.First() = inner.First
        override __.Last() = inner.Last
        override __.Insert i v = inner.Insert(i,v) |> cns
        override __.AddLastRange vs = inner.AddLastRange vs |> cns
        override __.AddFirstRange vs = inner.AddFirstRange vs |> cns
        override __.Set i v = inner.Set(i,v) |> cns
        override __.AddLastList t = 
            let t = t :?> TestXList<'a>
            inner.AddLastList(t.Inner) |> cns
        override __.Filter f = inner |> XList.filter f |> cns
        override __.Map f = inner |> XList.map f |> cns
        override __.IndexOf f = inner |> XList.indexOf f
        override __.Reverse() = inner.Reverse() |> cns
        static member Empty = TestXList<'a>(XList.empty)




let fsharpx_deque (count : int)= FSHARPX.TestDeque.Empty.FromSeq [0 .. count]
let fsharpx_skewlist count= FSHARPX.TestSkewList.Empty.FromSeq [0 .. count]
let fsharpx_vector count = FSHARPX.TestVector.Empty.FromSeq [0 .. count]
let core_list count = CORE.TestList.Empty.FromSeq [0 .. count]
let core_queue count= CORE.TestQueue.Empty.FromSeq [0 .. count]
let core_stack count= CORE.TestStack.Empty.FromSeq [0 .. count]

let solid_vector count= SOLID.TestVector.Empty.FromSeq [0 .. count];
let solid_xlist count= SOLID.TestXList.Empty.FromSeq [0 .. count]
let core_flist count= FSHARP.TestFSList.Empty.FromSeq [0 .. count];
let fsharpx_realdeque count = FSHARPX.TestRealDeque.Empty.FromSeq [0 .. count]
let all_test_targets n= 
    [solid_xlist; solid_vector; fsharpx_deque; fsharpx_skewlist; fsharpx_vector; core_list; core_queue;core_stack;core_flist]
    |> List.map (fun f -> f n)


