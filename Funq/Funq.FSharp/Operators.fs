namespace Funq.FSharp
open Funq

[<AutoOpen>]
module Operators = 
    ///Alias of AddFirst. Adds an item to the beginning of the collection.
    let inline (^+>) item col =   (^s : (member AddFirst : 'a -> 's) col,item)
    ///Alias of AddLast. Adds an item to the end of the collection.
    let inline (<+) col item =   (^s : (member AddLast : 'a -> 's) col,item)
    ///Alias of AddFirstRange. Adds a sequence to the beginning of the collection.
    let inline (^++>) items col = (^s : (member AddFirstRange : 'a -> 's) col,items)
    ///Alias of AddLastRange. Adds a sequence to the end of the collection.
    let inline (<++) col items = (^s : (member AddLastRange : 'b -> 's) col,items)
    ///Alias of AddLastList. Explicitly concatenates two collections. 
    let inline (<+>) col1 col2 = (^s : (member AddLastList : 's -> 's) col1, col2)

    ///Adds a sequence of elements to the specified key-based collection.
    let inline ( /++ ) set items= (^s : (member AddMany :'v seq -> 's) set, items)
    ///Adds an element to the specified key-based collection.
    let inline ( /+ ) set item = (^s : (member Add : 'v -> 's) set, item)

    module Extra = 
        ///Computes the set-theoretic intersection.
        let inline ( .&. ) set1 set2 = (^s : (member Intersect : 't -> 's) set1, set2)
        ///Computes the set-theoretic union.
        let inline ( .|. ) set1 set2 = (^s : (member Union : 't -> 's) set1, set2)
        ///Computes the set-theoretic symmetric difference.
        let inline ( .^. ) set1 set2 = (^s : (member Difference : 't -> 's) set1, set2)
        ///Computes the set-theoretic complement, or 'except' operation: the left operand, minus the right operand.
        let inline ( .-. ) set1 set2 = (^s : (member Except : 't -> 's) set1, set2)
        ///Determines the set-theoretic relation of the right operand to the left operand.
        let inline ( <<? ) set1 set2 = (^s : (member RelatesTo : 't -> SetRelation) set1, set2)
        ///Determines the set-theoretic relation of the left operand to the right operand.
        let inline ( ?>> ) set1 set2 = (^s : (member RelatesTo : 't -> SetRelation) set2, set1)
    

