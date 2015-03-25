namespace Funq.FSharp
open Funq


module Operators = 
    open Funq.FSharp.Implementation
    ///Alias of AddFirst. Adds an item to the beginning of the collection.
    let inline (^+>) item col =  col |> Ops.addFirst item
    ///Alias of AddLast. Adds an item to the end of the collection.
    let inline (<+) col item =  col |> Ops.addLast item
    ///Alias of AddFirstRange. Adds a sequence to the beginning of the collection.
    let inline (^++>) items col = col |> Ops.addFirstRange items
    ///Alias of AddLastRange. Adds a sequence to the end of the collection.
    let inline (<++) col items = col |> Ops.addLastRange items
    ///Alias of AddLastList. Explicitly concatenates two collections. 
    let inline (<+>) col1 col2 = (^s : (member AddLastList : 's -> 's) col1, col2)

    ///Adds a sequence of elements to the specified key-based collection.
    let inline ( /++ ) set items= set |> Ops.addSetMany items
    ///Adds an element to the specified key-based collection.
    let inline ( /+ ) set item = set |> Ops.addSet item

    module Extra = 
        ///Computes the set-theoretic intersection.
        let inline ( .&. ) set1 set2 = set1 |> Ops.intersect set2
        ///Computes the set-theoretic union.
        let inline ( .|. ) set1 set2 = set1 |> Ops.union set2
        ///Computes the set-theoretic symmetric difference.
        let inline ( .^. ) set1 set2 = set1 |> Ops.symDifference set2
        ///Computes the set-theoretic complement, or 'except' operation: the left operand, minus the right operand.
        let inline ( .-. ) set1 set2 = set1 |> Ops.except set2
        ///Determines the set-theoretic relation of the right operand to the left operand.
        let inline ( <<? ) set1 set2 = set1 |> Ops.relatesTo set2
        ///Determines the set-theoretic relation of the left operand to the right operand.
        let inline ( ?>> ) set1 set2 = set2 |> Ops.relatesTo set1
    

