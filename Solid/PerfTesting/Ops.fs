///A module providing generalized inline operators.
module Benchmarks.Ops 
#nowarn"77"
open Solid
open System.Collections.Immutable
///Calls an instance AddLast method.
let inline addLast item col        = (^s : (member AddLast : 'a -> 's) col, item)
///Calls an instance AddFirst method.
let inline addFirst item col       = (^s : (member AddFirst : 'a -> 's) col, item)
///Calls an instance Count property.
let inline length col              = (^s : (member Count : int) col)
///Calls an instance DropLast method.
let inline dropLast col            = (^s : (member DropLast : unit -> 's) col)
///Calls an instance DropFirst method.
let inline dropFirst col           = (^s : (member DropFirst : unit -> 's) col)
///Calls an instance Insert method.
let inline insert index item col   = (^s : (member Insert : int * 'a -> 's) col, index, item)
///Calls an instance Remove method.
let inline remove index col        = (^s : (member Remove : int -> 's) col, index)
///Calls an instance AddLastRange method.
let inline addLastRange items col  = (^s : (member AddLastRange : 'a seq -> 's) col, items)
///Calls an instance AddFirstRange method.
let inline addFirstRange items col = (^s : (member AddFirstRange : 'a seq -> 's) col, items)
///Calls an instance get_Item method or an indexer.
let inline get index col           = (^s : (member get_Item : int -> 'a) col,index)
///Calls an instance Set method.
let inline set index v col         = (^s : (member Set : int * 'a -> 's) col, index, v)
///Calls an instance Take method.
let inline internal take n col     = (^s : (member Take : int -> ^s) col,n)
///Calls an instance InsertRange method.
let inline insertRange i data col  = (^s : (member InsertRange : int * 't -> 's) col, i, data)
///Calls an instance IsEmpty property.
let inline isEmpty col             = (^s : (member IsEmpty : bool) col ) 