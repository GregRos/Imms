///A module providing generalized inline operators.
module Funq.Benchmarking.Opsx 
#nowarn"77"
open Funq
open System
open System.Collections.Immutable
open System
///Calls an instance AddLast method.
let inline addLast item col        = (^s : (member AddLast : 'a -> 's) col, item)
///Calls an instance AddFirst method.
let inline addFirst item col       = (^s : (member AddFirst : 'a -> 's) col, item)
///Calls an instance Count property.
let inline length col              = (^s : (member Length : int) col)
///Calls an instance DropLast method.
let inline dropLast col            = (^s : (member DropLast : unit -> 's) col)
///Calls an instance DropFirst method.
let inline dropFirst col           = (^s : (member DropFirst : unit -> 's) col)
///Calls an instance Insert method.
let inline insert index item col   = (^s : (member Insert : int * 'a -> 's) col, index, item)
///Calls an instance Remove method.
let inline remove index col        = (^s : (member Remove : int -> 's) col, index)
///Calls an instance AddLastRange method.
let inline addLastRange items col  = (^s : (member AddLastRange : 'v -> 's) col, items)
///Calls an instance AddFirstRange method.
let inline addFirstRange items col = (^s : (member AddFirstRange : 'v -> 's) col, items)
///Calls an instance get_Item method or an indexer.
let inline get k col               = (^s : (member get_Item : 'k -> 'a) col,k)
///Calls an instance Set method.
let inline set k v col             = (^s : (member Update : 'k * 'a -> 's) col, k, v)
///Calls an instance Take method.
let inline internal take n col     = (^s : (member Take : int -> ^s) col,n)
///Calls an instance InsertRange method.
let inline insertRange i data col  = (^s : (member InsertRange : int * 't -> 's) col, i, data)
///Calls an instance IsEmpty property.
let inline isEmpty col             = (^s : (member IsEmpty : bool) col ) 
let inline iterWhile f col         = (^s : (member ForEachWhile : Func<'v, bool> -> 't) col, Func<'v, bool>(f))
let inline contains k col          = (^s : (member ContainsKey : 'k -> bool) col, k)
let inline setContains k col       = (^s : (member Contains : 'k -> bool) col, k)
let inline add k v col             = (^s : (member Add : 'k * 'v -> 's) col,k,v)
let inline addSet k col            = (^s : (member Add : 'k -> 's) col, k)
let inline addSetMany ks col       = (^s : (member AddMany : 'k seq -> 's) col, ks)
let inline dropSet k col           = (^s : (member Drop : 'k -> 's) col, k)
let inline union o col             = (^s : (member Union : 't -> 's) col, o)
let inline intersect o col         = (^s : (member Intersect : 't -> 's) col, o)
let inline except o col            = (^s : (member Except : 't -> 's) col, o)
let inline symDifference o col     = (^s : (member SymmetricDifference : 't -> 's) col, o)
let inline isSetEquals o col       = (^s : (member IsSetEqual : 't -> bool) col, o)
let inline isSuperset o col        = (^s : (member IsProperSuperset : 't -> bool) col, o)
let inline isSubset o col          = (^s : (member IsProperSubset : 't -> bool) col, o)
let inline isDisjoint o col        = (^s : (member IsDisjoint : 's -> bool) col, o)
let inline ofSeq q                 = (^s : (static member FromSeq : 't -> 's) q)
let inline asSeq q                 = (^s : (member AsSeq : 't seq) q)
let inline skip n col              = (^s : (member Skip : int -> 's) col, n)

let inline iter col                = (^s : (member ForEach : Action<'a> -> unit) col, Action<'a>(ignore))