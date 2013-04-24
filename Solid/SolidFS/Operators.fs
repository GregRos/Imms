[<AutoOpen>]
module Solid.FSharp.Operators
///Alias of AddFirst. Adds an item to the beginning of the collection.
let inline (+>) item col =   (^s : (member AddFirst : 'a -> 's) col,item)
///Alias of AddLast. Adds an item to the end of the collection.
let inline (<+) col item =   (^s : (member AddLast : 'a -> 's) col,item)
///Alias of AddFirstRange. Adds a sequence to the beginning of the collection.
let inline (++>) items col = (^s : (member AddFirstRange : 'a -> 's) col,items)
///Alias of AddLastRange. Adds a sequence to the end of the collection.
let inline (<++) col items = (^s : (member AddLastRange : 'b -> 's) col,items)
///Alias of AddLastList. Explicitly concatenates two collections. 
let inline (<+>) col1 col2 = (^s : (member AddLastList : 's -> 's) col1, col2)

