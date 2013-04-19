[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SolidFS.XList
open Solid
open System
///O(1). Gets the first element.
let first (target : FlexibleList<'a>)  = 
    target.First
///O(1). Gets the last element.
let last (target : FlexibleList<'a>) = 
    target.Last
///O(1) amortized. Returns a list without the first element.
let dropFirst (target : FlexibleList<'a>) = 
    target.DropFirst()

///O(1) amortized. returns a list without the last element.
let dropLast (target : FlexibleList<'a>) = 
    target.DropLast()
///O(logn), very fast. Returns the element with the specified index.
let lookup  (index : int) (target : FlexibleList<'a>) =
    target.[index]
///O(logn), fast. Updates the element with the specified index.
let update (index : int) (item : 'a) (target : FlexibleList<'a>) = 
    target.Set(index,item)
///O(logn), fast. Inserts an element immediately before the specified index.
let insert  (index : int) (item : 'a) (target : FlexibleList<'a>)=
    target.Insert(index,item)
///O(logn + m). Inserts a sequence of elements immediately before the specified index.
let insertSeq (index : int) (items : 'a seq) (target : FlexibleList<'a>) =
    target.InsertRange(index, items)
///O(logn), slow. Returns a slice starting at one index and ending at the other index.
let slice (startIndex : int,endIndex : int) (target : FlexibleList<'a>) = 
    target.Slice(startIndex,endIndex)
///O(1). Returns the length of the list.
let length (target:  FlexibleList<'a>) = 
    target.Count
///O(n). Returns a list containing only those elements that fulfill the specified predicate.
let filter (f : 'a -> bool) (target : FlexibleList<'a>) = 
    target.Where(f)
///O(n). Applies a function on every element in the list.
let select (f : 'a -> 'b) (target : FlexibleList<'a>) = 
    target.Select(f)
///O(n). Iterates over every element in the list.
let iter (f : 'a -> unit) (target : FlexibleList<'a>) = 
    target.ForEach(Action<'a>(f))
///O(logn), med. Returns a sublist containing the first N elements.
let take count (target : FlexibleList<'a>) = 
    target.Slice(0,count - 1)
///O(logn), med. Returns a sublist without the first N elements.
let skip count (target : FlexibleList<'a>) = 
    target.Slice(count,-1)
///O(1). Gets the empty list.
let empty<'a> = FlexibleList<'a>.Empty

///O(1). Gets if the list is empty.
let isEmpty (target :_ FlexibleList)=
    target.IsEmpty

///O(n).
let choose (f : 'a -> 'b option) (target : FlexibleList<'a>) = 
    let new_target = ref (empty : FlexibleList<'b>)
    target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))
///O(n). Applies an accumulator on every element, from first to last.
let fold (initial : 'state) (f : 'state -> 'item -> 'state) (target : FlexibleList<'item>) = 
    target.Fold(initial, f)
///O(n). Applies an accumulator on every element, from last to first.
let foldBack (initial : 'state) (f : 'state -> 'item -> 'state) (target : FlexibleList<'item>) = 
    target.FoldBack(initial, f)
///O(n). Applies a function on every element, flattens the results, and constructs a new list. O(n·m)
let collect f (target :_ FlexibleList) =
    target.SelectMany(f)

///O(n). 
let zip (left : FlexibleList<_>) (right:FlexibleList<_>) = 
    empty<_> <++ Seq.zip left right
///O(n). Constructs a list from a sequence.
let ofSeq (xs : #seq<_>) = 
    xs.ToFlexList()
///O(n), fast. Returns true if any element fulfills the predicate.
let exists f (l : _ FlexibleList) =
    l.IndexOf(Func<_,_>(f)).HasValue
///O(1). Returns a list consisting of a single item.
let ofItem x = empty <+ x
///O(n), fast. Returns the index of the first element that fulfills a predicate, or None.

///O(logn + m). Returns the first elements that fulfill the predicate.
let takeWhile (f : _ -> bool) (target : _ FlexibleList) = 
    target.TakeWhile(Func<_,bool>(f))
///O(logn + m). Returns a list without the first elements that fulfill the predicate.
let skipWhile (f : _ -> bool) (target :_ FlexibleList) = 
    target.SkipWhile(Func<_,bool>(f))
///O(n). Reverses the list.
let rev (target:_ FlexibleList) = 
    target.Reverse()
let remove i (target:_ FlexibleList) = 
    target.Remove(i) 
