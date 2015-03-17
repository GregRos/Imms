module Funq.FSharp.Implementation.Ops
open System
#nowarn"77"
let inline isEmpty o      = (^s : (member IsEmpty : bool) o )
let inline dropLast o     = (^s : (member DropLast : unit -> 's) o)
let inline dropFirst o    = (^s : (member DropFirst : unit -> 's) o )
let inline first o        = (^s : (member First : 'a) o)
let inline last o         = (^s : (member Last : 'a) o)
let inline length o       = (^s : (member Length : int) o )
let inline getEmpty()     = (^s : (static member Empty : 's) ())

let inline getEmptyWith c = (^s : (static member Empty : 'c -> 's) c)
///Calls an instance AddLast method.
let inline addLast item col        = (^s : (member AddLast : 'a -> 's) col, item)
///Calls an instance AddFirst method.
let inline addFirst item col       = (^s : (member AddFirst : 'a -> 's) col, item)
///Calls an instance Count property.
///Calls an instance Insert method.
let inline insert index item col   = (^s : (member Insert : int * 'a -> 's) col, index, item)
///Calls an instance Remove method.
let inline remove index col        = (^s : (member Remove : int -> 's) col, index)
///Calls an instance AddLastRange method.
let inline addLastRange items col  = (^s : (member AddLastRange : 'v seq -> 's) col, items)
///Calls an instance AddFirstRange method.
let inline addFirstRange items col = (^s : (member AddFirstRange : 'v seq -> 's) col, items)
///Calls an instance get_Item method or an indexer.
let inline get k col               = (^s : (member get_Item : 'k -> 'a) col,k)
///Calls an instance Set method.
let inline set k v col             = (^s : (member Update : 'k * 'a -> 's) col, k, v)
///Calls an instance Take method.
let inline take n col     = (^s : (member Take : int -> ^s) col,n)
///Calls an instance InsertRange method.
let inline insertRange i data col  = (^s : (member InsertRange : int * 't -> 's) col, i, data)

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
let inline drop k s                    = (^s : (member Drop : 'k -> 's) s, k)

let inline filter f s                       = (^s : (member Where : Func<'a, bool> -> 's) s, toFunc1 f)
let inline collect f c s                    = (^s : (member SelectMany : Func<'a, 'b seq> * 'o -> 't) s, toFunc1 f, c)
let inline fold v f s                       = (^s : (member Aggregate : 'r * Func<'r, 'u, 'r> -> 'r) s, v, toFunc2 f)
let inline choose (f : 'a -> 'b option) c s = (^s : (member Select : Func<'a, Funq.Option<'b>> * 'o -> 't) s, (f >> toOption) |> toFunc1,c)
let inline reduce f s                       = (^s : (member Reduce : Func<'a, 'a, 'a> -> 'a) s, f)
let inline tryFind f s                      = (^s : (member Find : Func<'a, bool>  -> 'a Funq.Option) s, toFunc1 f) |> fromOption
let inline pick f s                         = (^s : (member Pick : Func<'a, 'b Funq.Option>  -> 'b Funq.Option) s, f >> toOption |> toFunc1) |> fromOption
let inline exists f s                       = (^s : (member Any : Func<'a, bool> -> bool) s, f |> toFunc1)
let inline forAll f s                       = (^s : (member All : Func<'a, bool> -> bool) s, f |> toFunc1)
let inline count f s                        = (^s : (member Count : Func<'a, bool> -> int) s, f |> toFunc1)
let inline scan r f c s                     = (^s : (member Scan : 'r * Func<'r, 'a, 'r> * 'o -> 't) s, r, toFunc2 f,c)
let inline maxItem s                        = (^s : (member MaxItem : 'v) s)
let inline minItem s                        = (^s : (member MinItem : 'v) s)
let inline dropMin s                        = (^s : (member DropMin : unit -> 's) s)
let inline dropMax s                        = (^s : (member DropMax : unit -> 's) s)
let inline merge f m2 m1                    = (^s : (member Merge : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline join f m2 m1                     = (^s : (member Join : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline difference m2 m1                 = (^s : (member Difference : 's -> 's) m1, m2)
let inline addMany vs s                   = (^s : (member AddMany : seq<'v> -> 's) s, vs)
let inline dropMany vs s                    = (^s : (member DropMany : seq<'v> -> 's) s, vs)
let inline update(i,v) o                  = (^s : (member Update : int * 'a -> 's) o, i, v)
let inline slice(i1, i2) o                = (^s : (member get_Item : int * int -> 's) o, i1, i2)
let inline map f o                        = (^s : (member Select : Func<'a,'b> -> 't) o ,toFunc1 f)

let inline iterBack f o                   = (^s : (member ForEachBack : Action<'a> -> unit) o, toAction f)
let inline iterBackWhile f o              = (^s : (member ForEachBackWhile : Func<'a, bool> -> bool) o, toFunc1 f)
let inline zip r l                        = (^s : (member Zip : 'b seq * Func<'a, 'b, 'a * 'b> -> 'r) l, r, toFunc2 (fun l r -> (l,r)))
let inline foldBack v f o                 = (^s : (member AggregateBack : 'r * Func<'r, 'u, 'r> -> 'r) o, v, toFunc2 f)

let inline tryFirst o                     = (^s : (member TryFirst : 'b Funq.Option) o) |> fromOption
let inline tryLast o                      = (^s : (member TryLast : 'b Funq.Option) o ) |> fromOption

let inline reduceBack f o                 = (^s : (member ReduceBack : Func<'a, 'a, 'a> -> 'a) o, f)

let inline tryFindIndex f o               = (^s : (member FindIndex : Func<'a, bool> -> int Funq.Option) o, toFunc1 f) |> fromOption

let inline takeWhile f o                  = (^s : (member TakeWhile : Func<'a, bool> -> 's) o, f |> toFunc1)

let inline skipWhile f o                  = (^s : (member SkipWhile : Func<'a, bool> -> 's) o, f |> toFunc1)

let inline nth n o                        = (^s : (member get_Item : int -> 'a) o, n)

let inline scanBack r f o                 = (^s : (member ScanBack : 'r * Func<'r, 'a, 'r> -> 't) o, r, toFunc2 f)

let inline insertList i vs o              = (^s : (member InsertList : int * 's -> 's) o, i, vs)