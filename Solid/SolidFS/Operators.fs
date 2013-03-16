[<AutoOpen>]
module Solid.Operators
//++ Primitive Operators
// We define generalized operators usable throughout the library.
// This is preferable to separate static operators because when statically defined operators are used, 
//Type inference doesn't seem to work very well.
open System

let inline iter (f : 'a -> unit) (x : 's) = (^s : (member ForEach : ('a -> unit) -> unit) x,f)
let inline iterBack (f : 'a -> unit) (x : 's) = (^s : (member ForEachBack : ('a -> unit) -> unit) x,f)

let inline empty ()               = (^s : (static member Empty : 's) ())
let inline (+>) item col          = (^s : (member AddFirst : 'a -> 's) col,item)
let inline (<+) col item          = (^s : (member AddLast : 'a -> 's) col,item)
let inline (<+>) col1 col2        = (^s : (static member Concat : 's * 's -> 's) col1,col2)
let inline (++>) items col        = (^s : (member AddFirstRange : #seq<_> -> 's) col,items)
let inline (<++) col items        = (^s : (member AddLastRange : #seq<_> -> 's) col,items)

//++ Advanced Operators
//The following operators deal with adding or removing items from a data structure.

///Returns the first elements of the data structure
let inline take n col            = (^s : (member Take : int -> ^s) col,n)
///Returns the last elements of the data structure
let inline takel n col            = (^s : (member TakeLast : int -> ^s) col,n)
///Splits the data structure at the specified index.
let inline split n col            = (^s : (member Split : int -> ^s * ^s) col,n)

///Gets the first element of the data structure
let inline first col              = (^s : (member First : 'a) col)
///Gets the last element of the data structure
let inline last col               = (^s : (member Last : 'a) col)
///Gets the element with the specified index from the data structure
let inline get i col              = (^s : (member Item : int -> ^a) col,i)
///Sets the element with the specified index from the data structure
let inline set i v col            = (^s : (member Set : ^k * 'a -> 's) col,i,v)
///Removes the first element from the data structure.
let inline dropf col              = (^s : (member DropFirst : ^s) col)
///Removes the last element from the data structure.
let inline dropl col              = (^s : (member DropLast : ^s) col)

let inline fromSeq (sq : #seq<_>) = (^s : (static member FromSeq : seq<'a> -> 's) sq)
///Reverses the data structure.
let inline reverse col            = (^s : (member Reverse : ^s) col)
///Gets the length of the data structure.
let inline length col             = (^s : (member Length : int) col)
//++ Derived Operators
// These operators use the above operators to apply functions on data structure, filter, etc.

let inline splice index inner outer = 
    let part1,part2 = outer |> split index
    let part1 = part1 <+> inner
    let closed = part1 <+> part2
    closed

let inline droplMany n target = 
    let len = target |> length
    let first = target |> take (len - n)
    first

let inline dropfMany n target = 
    let len = target |> length
    let last = target |> takel (len - n)
    last

let inline insertManyAt index (sq : seq<'a>) (target : ^s) = 
    let ins  = (sq |> fromSeq)
    target |> splice index ins

let inline removeAt index target = 
    let part1,part2 = target |> split index
    let part1 = part1 |> dropl
    part1 <+> part2

let inline removeManyAt index count target= 
    let part1,part2 = target |> split index
    let part1 = part1 |> droplMany count
    part1 <+> part2


///Applies the specified transformation on the data structure.
let inline map (f : 'a -> 'b) col = 
    let refRoot = ref (empty() : 'b)
    let mapIter x = 
        let x = x |> f
        refRoot := (!refRoot <+ x)
    col |> iter mapIter
    !refRoot

///Filters the data structure using the specified predicate.
let inline filter (f : 'a -> bool) (col : 's) : 's =
    let refRoot = ref (empty() : ^s)
    let filterIter (x : 'a) = 
        if f x then refRoot := !refRoot <+ x 
    col |> iter filterIter
    !refRoot

///Applies a a function on the data structure, keeping all elements which return Some, and dropping all those that return None.
let inline choose (f : _ -> _ option) col = 
    let refRoot = ref (empty() : 'b)
    let chooseIter x = 
        match f x with
        | None -> ()
        | Some y -> refRoot := !refRoot + y
    col |> iter chooseIter
    !refRoot

///Constructs a linked list from the elements of this data structure
let inline toList col = 
    let refList = ref []
    let listIter x = 
        refList := x :: !refList
    col |> iter listIter
    !refList

///Folds over the data structure using the specified function.
let inline fold (foldr : _-> _ -> _) (start : _) col = 
    let refState = ref start
    let foldIter x = 
        refState := foldr (refState.Value) x
    col |> iter foldIter
    !refState

///Folds over the data structure backwards, using the specified function.
let inline foldBack (foldr :_ -> _ -> _) (start :_) col = 
    let refState = ref start
    let foldbIter x = 
        refState := foldr (refState.Value) x 
    col |> iterBack foldbIter
    !refState

///Returns true if the specified predicate is true for all items in the data structure.
let inline forall (pred : _ -> bool) col = 
    col |> fold (fun cur item -> cur && pred(item)) true

///Returns true if the specified predicate is true for any item in the data structure.
let inline forany (pred : _ -> bool) col = 
    col |> fold (fun cur item -> cur || pred(item)) false

///Counts the number of items for which the specified predicate is true.
let inline count (pred : _ -> bool) col = 
    col |> fold (fun cur item -> if pred(item) then cur + 1 else cur) 0

let inline toArray col = 
    let l = col |> length
    let arr = Array.zeroCreate l
    for i,item in Seq.zip {0 .. l} col do
        arr.[i] <- item
    arr

