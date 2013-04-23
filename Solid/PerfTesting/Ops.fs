module Benchmarks.Ops 
#nowarn"77"
open Solid
open System.Collections.Immutable
let inline addLast item col              = (^s : (member AddLast         : 'a                   -> 's) col, item)
let inline addFirst item col             = (^s : (member AddFirst      : 'a             -> 's) col, item)
let inline length col                    = (^s : (member Count       : int) col)
let inline dropLast col                  = (^s : (member DropLast      : unit -> 's) col)
let inline dropFirst col                 = (^s : (member DropFirst      : unit                  -> 's) col)
let inline insert index item  col        = (^s : (member Insert      : int * 'a             -> 's) col, index, item)
let inline remove index col              = (^s : (member Remove      : int                  -> 's)  col, index)
let inline addLastRange items col        = (^s : (member AddLastRange    : 'a seq               -> 's) col, items)
let inline addFirstRange items col = (^s : (member AddFirstRange : 'a seq         -> 's) col, items)
let inline get index col =                 (^s : (member get_Item : int -> 'a) col,index)
let inline set index v col  = (^s : (member Set : int * 'a -> 's) col, index, v)
let inline internal take n col = (^s : (member Take : int -> ^s) col,n)
let inline insertRange i data col = (^s : (member InsertRange : int * 't -> 's) col, i, data)
let inline isEmpty col = (^s : (member IsEmpty : bool) col )   