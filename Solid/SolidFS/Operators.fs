[<AutoOpen>]
module SolidFS.Operators
open Solid
//++ Primitive Operators
// We define generalized operators usable throughout the library.
// This is preferable to separate static operators because when statically defined operators are used, 
//Type inference doesn't seem to work very well.
open System

let inline delay1 f p1 = fun () -> f p1
let inline delay2 f p1 p2 = fun () -> f  p1
let inline delay3 f p1 p2 p3 = fun () -> f p1 p2 p3
let inline delay4 f p1 p2 p3 p4 = fun () -> f p1 p2 p3 p4

type xlist<'a> = FlexibleList<'a>
let inline (+>) item col = (^s : (member AddFirst : 'a -> 's) col,item)
let inline (<+) col item = (^s : (member AddLast : 'a -> 's) col,item)
let inline (++>) items col = (^s : (member AddFirstRange : #seq<_> -> 's) col,items)
let inline (<++) col items = (^s : (member AddLastRange : #seq<_> -> 's) col,items)
let inline (<+>) target1 target2 = (^s : (member AddLastList : 's -> 's)target1,target2)
type 'a FlexibleList with
    member this.GetSlice(startIndex : int option, endIndex : int option) = 
        let startIndex = if startIndex.IsSome then startIndex.Value else 0
        let endIndex = if endIndex.IsSome then endIndex.Value else -1
        this.Slice(startIndex,endIndex)

let (|Nil|Cons|) (l :_ xlist) = 
    if l.IsEmpty then Nil else Cons(l.First,l.DropFirst())

let (|Nil|Conj|) (l :_ xlist)=
    if l.IsEmpty then Nil else Conj(l.DropLast(), l.Last)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module XList = 
    ///Joins a sequence of elements to the beginning of the list.
    let consSeq hs (t :_ xlist) =
        t.AddFirstRange hs
    ///Adds another list to the beginning of this one.
    let consList (other :_ xlist) (target :_ xlist) = 
        target.AddFirstList(other)

    ///Adds an item to the beginning of the list.
    let cons h (t : _ xlist) =
        t.AddFirst(h)

    ///Adds an item to the end of the list.
    let conj h (t :_ xlist)=
        t.AddLast(h)

    ///Joins a sequence of elements to the end of the list.
    let conjSeq hs (t :_ xlist)=
        t.AddLastRange hs

    ///Joins another list to the end of this one.
    let conjList (t1 :_ xlist) (target:_ xlist)=
        target.AddLastList(t1)

    ///O(1). Gets the first element.
    let first (target : xlist<'a>)  = 
        target.First
    ///O(1). Gets the last element.
    let last (target : xlist<'a>) = 
        target.Last
    ///O(1) amortized. Returns a list without the first element.
    let tail (target : xlist<'a>) = 
        target.DropFirst()
    ///O(1) amortized. returns a list without the last element.
    let initial (target : xlist<'a>) = 
        target.DropLast()
    ///O(logn), fast. Returns the element with the specified index.
    let lookup  (index : int) (target : xlist<'a>) =
        target.[index]
    ///O(logn). Updates the element with the specified index.
    let update (index : int) (item : 'a) (target : xlist<'a>) = 
        target.Set(index,item)
    ///O(logn). Inserts an element immediately before the specified index.
    let insert  (index : int) (item : 'a) (target : xlist<'a>)=
        target.Insert(index,item)
    ///O(logn + m). Inserts a sequence of elements immediately before the specified index.
    let insertSeq (index : int) (items : 'a seq) (target : xlist<'a>) =
        target.InsertRange(index, items)
    ///O(logn). Inserts a list immediately before the specified index.
    let insertList (index : int) (l : xlist<'a>) (target : xlist<'a>) = 
        target.InsertList(index,l)
    ///O(logn). Returns a slice starting at one index and ending at the other index.
    let slice (startIndex : int) (endIndex : int) (target : xlist<'a>) = 
        target.Slice(startIndex,endIndex)
    ///O(1). Returns the length of the list.
    let length (target:  xlist<'a>) = 
        target.Count
    ///O(n). Returns a list containing only those elements that fulfill the specified predicate.
    let filter (f : 'a -> bool) (target : xlist<'a>) = 
        target.Where(Func<'a,bool>(f))
    ///O(n). Applies a function on every element in the list.
    let map (f : 'a -> 'b) (target : xlist<'a>) = 
        target.Select(Func<'a,'b>(f))
    ///O(n). Iterates over every element in the list.
    let iter (f : 'a -> unit) (target : xlist<'a>) = 
        target.ForEach(Action<'a>(f))
    ///O(logn). Returns a sublist containing the first N elements.
    let take count (target : xlist<'a>) = 
        target.Slice(0,count - 1)
    ///O(logn). Returns a sublist without the first N elements.
    let skip count (target : xlist<'a>) = 
        target.Slice(count,-1)
    ///O(1). Gets the empty list.
    let empty<'a> = xlist<'a>.Empty
    ///O(n).
    let choose (f : 'a -> 'b option) (target : xlist<'a>) = 
        let new_target = ref (empty : xlist<'b>)
        target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))
    ///O(n). Applies an accumulator on every element, from first to last.
    let fold (initial : 'state) (f : 'state -> 'item -> 'state) (target : xlist<'item>) = 
        target.Aggregate(Func<'state,'item,'state>(f), initial)
    ///O(n). Applies an accumulator on every element, from last to first.
    let foldBack (initial : 'state) (f : 'state -> 'item -> 'state) (target : xlist<'item>) = 
        target.AggregateBack(Func<_,_,_>(f), initial)
    ///O(n). Applies a function on every element, flattens the results, and constructs a new list. O(n·m)
    let collect (f : _ -> seq<_>) (target :_ xlist) =
        target.SelectMany(Func<_,_>(f))

    ///O(n). 
    let zip (left : xlist<_>) (right:xlist<_>) = 
        empty<_> <++ Seq.zip left right
    ///O(n). Constructs a list from a sequence.
    let ofSeq (xs : #seq<_>) = 
        empty<_> <++ xs
    ///O(n), fast. Returns true if any element fulfills the predicate.
    let exists (f : _ -> bool) (l : _ xlist) =
        l.IndexOf(Func<_,bool>(f)).HasValue
    ///O(1). Returns a list consisting of a single item.
    let ofItem x = empty <+ x
    ///O(n),fast. Returns the index of the first element that fulfills a predicate, or None.
    let indexOf(f : _ -> bool) (l :_ xlist)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some res.Value else None
    ///O(n), fast. Returns the first element that fulfills a predicate, or None.
    let find(f : _ -> bool) (l :_ xlist)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some l.[res.Value] else None
    ///O(logn + m). Returns the first elements that fulfill the predicate.
    let takeWhile (f : _ -> bool) (target : _ xlist) = 
        target.TakeWhile(Func<_,bool>(f))
    ///O(logn + m). Returns a list without the first elements that fulfill the predicate.
    let skipWhile (f : _ -> bool) (target :_ xlist) = 
        target.SkipWhile(Func<_,bool>(f))

    
[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector = 
    ///O(1). Gets the first element.
    let first (target : Vector<'a>)  = 
        target.First
    ///O(1). Gets the last element.
    let last (target :  Vector<'a>) = 
        target.Last

    ///O(logn), fast. Returns a vector without the last element.
    let initial (target :Vector<'a>) = 
        target.DropLast()
    ///Fast. Returns the element with the specified index.
    let lookup  (index : int) (target :Vector<'a>) =
        target.[index]
    
    let update (index : int) (item : 'a) (target :Vector<'a>) = 
        target.Set(index,item)
    ///O(1). Gets the length of the list.
    let length (target:  Vector<'a>) = 
        target.Count
    ///O(n). 
    let filter (f : 'a -> bool) (target : Vector<'a>) = 
        target.Where(Func<'a,bool>(f))

    let map (f : 'a -> 'b) (target : Vector<'a>) = 
        target.Select(Func<'a,'b>(f))

    let iter (f : 'a -> unit) (target : Vector<'a>) = 
        target.ForEach(Action<'a>(f))

    let take count (target : Vector<'a>) = 
        target.Take(count)

    let empty<'a> = Vector<'a>.Empty

    let choose (f : 'a -> 'b option) (target :Vector<'a>) = 
        let new_target = ref (empty : Vector<'b>)
        target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))

    let fold (initial : 'state) (f : 'state -> 'item -> 'state) (target : Vector<'item>) = 
        target.Aggregate(Func<'state,'item,'state>(f), initial)

    let zip (left : #seq<_>) (right:#seq<_>) = 
        empty<_> <++ Seq.zip left right
        
    let ofSeq (xs : #seq<_>) = 
        empty<_> <++ xs

    let exists (f : _ -> bool) (l : _ Vector) =
        l.IndexOf(Func<_,bool>(f)).HasValue

    let ofItem x = empty <+ x

    let indexOf(f : _ -> bool) (l :_ Vector)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some res.Value else None

    let find(f : _ -> bool) (l :_ Vector)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some l.[res.Value] else None

    