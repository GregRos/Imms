[<AutoOpen>]
module SolidFS.Operators
//++ Primitive Operators
// We define generalized operators usable throughout the library.
// This is preferable to separate static operators because when statically defined operators are used, 
//Type inference doesn't seem to work very well.
open System

let inline delay1 f p1 = fun () -> f p1
let inline delay2 f p1 p2 = fun () -> f  p1
let inline delay3 f p1 p2 p3 = fun () -> f p1 p2 p3
let inline delay4 f p1 p2 p3 p4 = fun () -> f p1 p2 p3 p4
let inline iter (f : 'a -> unit) (x : 's) = (^s : (member ForEach : ('a -> unit) -> unit) x,f)
let inline iterBack (f : 'a -> unit) (x : 's) = (^s : (member ForEachBack : ('a -> unit) -> unit) x,f)

let inline empty ()               = (^s : (static member Empty : 's) ())
let inline (+>) item col          = (^s : (member AddFirst : 'a -> 's) col,item)
let inline (<+) col item          = (^s : (member AddLast : 'a -> 's) col,item)
let inline (<+>) col1 col2        = (^s : (member Append : 's -> 's) col1,col2)
let inline (++>) items col        = (^s : (member AddRangeFirst : #seq<_> -> 's) col,items)
let inline (<++) col items        = (^s : (member AddRangeLast : #seq<_> -> 's) col,items)

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
let inline isEmpty col = (^s : (member IsEmpty : bool) col)
let inline isntEmpty col = col |> isEmpty |> not
