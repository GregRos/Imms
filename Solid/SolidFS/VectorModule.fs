[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Solid.FSharp.Vector 
open System
open Solid
///O(1). Gets if the Vector is empty.
let isEmpty (target:_ Vector) = 
    target.IsEmpty
///O(logn); immediate. Gets the first element.
let first (target : Vector<'a>)  = 
    target.First
///O(logn); immediate. Gets the last element.
let last (target :  Vector<'a>) = 
    target.Last

///O(logn), fast. Returns a Vector without the last element.
let dropLast (target :Vector<'a>) = 
    target.DropLast()
///O(logn); immediate. Returns the element with the specified index.
let lookup  (index : int) (target :Vector<'a>) =
    target.[index]

///O(logn), fast. Updates the element with the specified index.
let update (index,item) (target :Vector<'a>) = 
    target.Set(index,item)
///O(1). Gets the length of the Vector.
let length (target:  Vector<'a>) = 
    target.Count
///O(n). Returns a Vector containing the elements that fulfill the predicate.
let filter (f : 'a -> bool) (target : Vector<'a>) = 
    target.Where(Func<'a,bool>(f))
///O(n). Applies the specified function on every element in the Vector.
let select (f : 'a -> 'b) (target : Vector<'a>) = 
    target.Select(Func<'a,'b>(f))
///O(n). Iterates over the Vector, from first to last.
let iter (f : 'a -> unit) (target : Vector<'a>) = 
    target.ForEach(Action<'a>(f))
///O(n). Iterates over the Vector, from last to first.
let iterBack (f:_->unit) (target:_ Vector) =
    target.ForEachBack(Action<_>(f))

    

///O(logn), fast. Returns a new Vector consisting of the first several elements.
let take count (target : Vector<'a>) = 
    target.Take(count)
///Gets the empty Vector.
let empty<'a> = Vector<'a>.Empty

let choose (f : 'a -> 'b option) (target :Vector<'a>) = 
    let new_target = ref (empty : Vector<'b>)
    target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))
///Applies an accumulator over the Vector.
let fold (initial : 'state) f (target : Vector<'item>) = 
    target.Fold(initial,f)

let zip (left : #seq<_>) (right:#seq<_>) = 
    empty<_> <++ Seq.zip left right
///O(n). Constructs a Vector from a sequence.
   

let ofSeq (xs : seq<_>) = empty<_>.AddLastRange(xs)
        
    
///O(n). Checks if any item fulfilling the predicate exists in the Vector.
let exists (f : _ -> bool) (l : _ Vector) =
    l.IndexOf(Func<_,bool>(f)).HasValue

let ofItem x = empty <+ x
///O(n). Returns the index of the first item that fulfills the predicate, or None.
let indexOf(f : _ -> bool) (l :_ Vector)=
    let res = l.IndexOf(Func<_,_>(f))
    if res.HasValue then Some res.Value else None
///O(n). Returns the first item that fulfills the predicate, or None.
let find(f : _ -> bool) (l :_ Vector)=
    let res = l.IndexOf(Func<_,_>(f))
    if res.HasValue then Some l.[res.Value] else None