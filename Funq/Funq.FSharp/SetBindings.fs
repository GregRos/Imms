module internal Funq.FSharp.GenSet 
#nowarn "77"
open System
open Funq
open System.Collections.Generic
let inline isEmpty s                     = (^s : (member IsEmpty : bool) s )
let inline drop k s                    = (^s : (member Drop : 'k -> 's) s, k)
let inline add v s = (^s : (member Add :'v -> 's) s, v)
type eq<'t> = IEqualityComparer<'t>
let inline map (f : 'v -> 'v2) c s  = 
    (^s : (member Select : Func<'v,'v2> * 'o -> 't) s, toFunc1 f, c)

let def : eq<'t>                            = null
let inline filter f s                       = (^s : (member Where : Func<'a, bool> -> 's) s, toFunc1 f)
let inline collect f c s                    = (^s : (member SelectMany : Func<'a, 'b seq> * 'o -> 't) s, toFunc1 f, c)
let inline iter f s                         = (^s : (member ForEach : Action<'a>  -> unit) s, toAction f)
let inline iterWhile f s                    = (^s : (member ForEachWhile : Func<'a, bool>-> bool) s, toFunc1 f)
let inline fold v f s                       = (^s : (member Aggregate : 'r * Func<'r, 'u, 'r> -> 'r) s, v, toFunc2 f)
let inline choose (f : 'a -> 'b option) c s = (^s : (member Select : Func<'a, Funq.Option<'b>> * 'o -> 't) s, (f >> toOption) |> toFunc1,c)
let inline reduce f s                       = (^s : (member Reduce : Func<'a, 'a, 'a> -> 'a) s, f)
let inline tryFind f s                      = (^s : (member Find : Func<'a, bool>  -> 'a Funq.Option) s, toFunc1 f) |> fromOption
let inline pick f s                         = (^s : (member Pick : Func<'a, 'b Funq.Option>  -> 'b Funq.Option) s, f >> toOption |> toFunc1) |> fromOption
let inline exists f s                       = (^s : (member Any : Func<'a, bool> -> bool) s, f |> toFunc1)
let inline forAll f s                       = (^s : (member All : Func<'a, bool> -> bool) s, f |> toFunc1)
let inline count f s                        = (^s : (member Count : Func<'a, bool> -> int) s, f |> toFunc1)
let inline scan r f c s                     = (^s : (member Scan : 'r * Func<'r, 'a, 'r> * 'o -> 't) s, r, toFunc2 f,c)
let inline length o                         = (^s : (member Length : int) o )
let inline maxItem s                        = (^s : (member MaxItem : 'v) s)
let inline minItem s                        = (^s : (member MinItem : 'v) s)
let inline dropMin s                        = (^s : (member DropMin : unit -> 's) s)
let inline dropMax s                        = (^s : (member DropMax : unit -> 's) s)
let inline merge f m2 m1                    = (^s : (member Merge : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline join f m2 m1                     = (^s : (member Join : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline except m2 m1                     = (^s : (member Except : 's -> 's) m1, m2)
let inline difference m2 m1                 = (^s : (member Difference : 's -> 's) m1, m2)
let inline addMany vs s                   = (^s : (member AddMany : seq<'v> -> 's) s, vs)
let inline dropMany vs s                    = (^s : (member DropMany : seq<'v> -> 's) s, vs)
let inline get k s                          = (^s : (member get_Item : 'k -> 'v) s, k)

