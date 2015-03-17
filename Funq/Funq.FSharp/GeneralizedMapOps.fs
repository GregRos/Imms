module internal Funq.FSharp.GenMap 
#nowarn "77"
open System
open Funq
open System.Collections.Generic
let inline isEmpty m                     = (^s : (member IsEmpty : bool) m )
let inline drop k m                    = (^s : (member Drop : 'k -> 's) m, k)
let inline add k v m = (^s : (member Add :'k * 'v -> 's) m, k, v)
type eq<'t> = IEqualityComparer<'t>
let inline update k v m                 = (^s : (member Set : 'k * 'v-> 's) m, k, v)
let inline map (f : 'k -> 'v -> 'k2 * 'v2) c m  = 
    let f' = fun (Kvp(k,v)) -> f k v |> Kvp.Of
    (^s : (member Select : Func<Kvp<'k,'v>, Kvp<'k2, 'v2>> * 'o -> 't) m, toFunc1 f', c)


let inline mapValues (f : 'k -> 'v -> 'v2) m =
    (^s : (member SelectValues<'v2> : Func<'k,'v,'v2> -> 't)m, toFunc2 f)

let def : eq<'t>                            = null
let inline filter f m                       = (^s : (member Where : Func<'a, bool> -> 's) m, toFunc1 f)
let inline collect f c m                    = (^s : (member SelectMany : Func<'a, 'b seq> * 'o -> 't) m, toFunc1 f, c)
let inline iter f m                         = (^s : (member ForEach : Action<'a>  -> unit) m, toAction f)
let inline iterWhile f m                    = (^s : (member ForEachWhile : Func<'a, bool>-> bool) m, toFunc1 f)
let inline fold v f m                       = (^s : (member Aggregate : 'r * Func<'r, 'u, 'r> -> 'r) m, v, toFunc2 f)
let inline choose (f : 'a -> 'b option) c m = (^s : (member Select : Func<'a, Funq.Option<'b>> * 'o -> 't) m, (f >> toOption) |> toFunc1,c)
let inline reduce f m                       = (^s : (member Reduce : Func<'a, 'a, 'a> -> 'a) m, f)
let inline tryFind f m                      = (^s : (member Find : Func<'a, bool>  -> 'a Funq.Option) m, toFunc1 f) |> fromOption
let inline pick f m                         = (^s : (member Pick : Func<'a, 'b Funq.Option>  -> 'b Funq.Option) m, f >> toOption |> toFunc1) |> fromOption
let inline exists f m                       = (^s : (member Any : Func<'a, bool> -> bool) m, f |> toFunc1)
let inline forAll f m                       = (^s : (member All : Func<'a, bool> -> bool) m, f |> toFunc1)
let inline count f m                        = (^s : (member Count : Func<'a, bool> -> int) m, f |> toFunc1)
let inline scan r f c m                     = (^s : (member Scan : 'r * Func<'r, 'a, 'r> * 'o -> 't) m, r, toFunc2 f,c)
let inline length o                         = (^s : (member Length : int) o )
let inline maxItem m                        = (^s : (member MaxItem : 'v) m)
let inline minItem m                        = (^s : (member MinItem : 'v) m)
let inline dropMin m                        = (^s : (member DropMin : unit -> 's) m)
let inline dropMax m                        = (^s : (member DropMax : unit -> 's) m)
let inline merge f m2 m1                    = (^s : (member Merge : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline join f m2 m1                     = (^s : (member Join : 's * Func<'k, 'v, 'v, 'v> -> 's) m1, m2, toFunc3 f)
let inline except m2 m1                     = (^s : (member Except : 's -> 's) m1, m2)
let inline difference m2 m1                 = (^s : (member Difference : 's -> 's) m1, m2)
let inline addMany kvps m                   = (^s : (member AddMany : seq<Kvp<'k,'v>> -> 's) m, kvps)
let inline dropMany ks m                    = (^s : (member DropMany : seq<'k> -> 's) m, ks)
let inline get k m                          = (^s : (member get_Item : 'k -> 'v) m, k)
let inline tryGet k m                       = (^s : (member TryGet : 'k -> 'v Funq.Option) m, k) |> fromOption
let inline contains k m = (^s : (member Contains : 'k -> bool) m, k)

