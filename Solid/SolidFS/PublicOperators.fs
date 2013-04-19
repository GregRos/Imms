[<AutoOpen>]
module SolidFS.Operators
let inline (+>) item col =   (^s : (member AddFirst : 'a -> 's) col,item)
let inline (<+) col item =   (^s : (member AddLast : 'a -> 's) col,item)
let inline (++>) items col = (^s : (member AddFirstRange : 'a -> 's) col,items)
let inline (<++) col items = (^s : (member AddLastRange : 'b -> 's) col,items)
let inline (<+>) col1 col2 = (^s : (member AddLastList : 's -> 's) col1, col2)
