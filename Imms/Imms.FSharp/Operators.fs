namespace Imms.FSharp
open Imms
open Imms.Abstract
module Operators = 
    open Imms.FSharp.Implementation

    ///Alias of AddFirst. Adds an item to the beginning of the collection.
    let inline (^+>) (item : 'element) (col : ^sequential) = col |> Ops.addFirst item

    ///Alias of AddLast. Adds an item to the end of the collection.
    let inline (<+) (col : ^sequential) (item : 'element) =  col |> Ops.addLast item

    ///Alias of AddFirstRange. Adds a sequence to the beginning of the collection.
    let inline (^++>) (items : 'element seq) (col : ^sequential) = col |> Ops.addFirstRange items

    ///Alias of AddLastRange. Adds a sequence to the end of the collection.
    let inline (<++) (col : ^sequential) (items : 'element seq) = col |> Ops.addLastRange items

    ///Adds a sequence of elements to a set, or a sequence of KeyValuePairs or 2-tuples to a map.
    let inline ( /++ ) (col : ^setOrMap) (vs : 'element seq) = col |> Ops.op_AddRange vs

    ///Adds an element to a set, or adds a KeyValuePair or 2-tuple to a map.
    let inline ( /+ ) (col : ^setOrMap) (v : 'element) = col |> Ops.op_Add v
    
    ///Removes an element from a set, or a key from a map.
    let inline ( /-) (col : ^setOrMap) (v : 'element) = col |> Ops.op_Remove v

    ///Removes a sequence of elements or keys from a set or map.
    let inline (/--) (col : ^setOrMap) (vs : 'element seq) = col |> Ops.op_RemoveRange vs

    module Extra = 
        ///Computes the set-theoretic intersection.
        let inline ( .&. ) set1 set2 = set1 |> Ops.intersect set2
        ///Computes the set-theoretic union.
        let inline ( .|. ) set1 set2 = set1 |> Ops.union set2
        ///Computes the set-theoretic symmetric difference.
        let inline ( .^. ) set1 set2 = set1 |> Ops.difference set2
        ///Computes the set-theoretic complement, or 'except' operation: the left operand, minus the right operand.
        let inline ( .-. ) set1 set2 = set1 |> Ops.except set2
        ///Determines the set-theoretic relation of the right operand to the left operand.
        let inline ( <<? ) set1 set2 = set1 |> Ops.relatesTo set2
        ///Determines the set-theoretic relation of the left operand to the right operand.
        let inline ( ?>> ) set1 set2 = set2 |> Ops.relatesTo set1
    

