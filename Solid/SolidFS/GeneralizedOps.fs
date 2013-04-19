module internal SolidFS.Gen 
let inline  isEmpty o = (^s : (member IsEmpty : bool) o )
let inline  dropLast o = (^s : (member DropLast : unit -> 's) o)
let inline  dropFirst o =(^s : (member DropFirst : unit -> 's) o )
let inline  first o = (^s : (member First : 'a) o)
let inline  last o = (^s : (member Last : 'a) o)
let inline  length o = (^s : (member Count : int) o )