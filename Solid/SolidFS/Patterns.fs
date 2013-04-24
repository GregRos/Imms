///Provides active patterns for consuming Solid collections.
[<AutoOpen>]
module Solid.FSharp.Patterns
open Solid
///Decomposes a collection by 1 element, from the end. 
let inline (|Last|_|) o = 
    if o |> Gen.isEmpty then
        None
    else
        let last = o |> Gen.last
        let rest = o |> Gen.dropLast
        Some(rest,last)

///Decomposes a collection by 2 elements, from the end.
let inline (|Last2|_|) o =
    if o |> Gen.length < 2 then
        None
    else
        let last1 = o |> Gen.last
        let rest1 = o |> Gen.dropLast
        let last2 = rest1 |> Gen.last
        let rest2 = rest1 |> Gen.dropLast
        Some(rest2, last2, last1)
///Decomposes a collection by 3 elements, from the end.
let inline (|Last3|_|) o = 
    match o with
    | Last(Last2(tail, item1,item2),item3) -> Some(tail, item1,item2,item3)
    | _ -> None
///Decomposes a collection by 4 elements, from the end.
let inline (|Last4|_|) o = 
    match o with
    | Last2(Last2(tail, item1, item2), item3, item4) -> Some(tail, item1, item2, item3, item4)
    | _ -> None
///Decomposes a collection by 1 element, from the start.
let inline (|First|_|) o =
    if o |> Gen.isEmpty then
        None
    else
        let first = o |> Gen.first
        let rest = o |> Gen.dropFirst
        Some(first,rest)
///Decomposes a collection by 2 elements, from the start.
let inline (|First2|_|) o = 
    if o |> Gen.length < 2 then
        None
    else
        let first1 = o |> Gen.first
        let rest1 = o |> Gen.dropFirst
        let first2 = rest1 |> Gen.first
        let rest2 = rest1 |> Gen.dropFirst
        Some(first1, first2, rest2)

///Decomposes a collection by 3 elements, from the start.
let inline (|First3|_|) o =
    match o with
    | First2(First(tail, item1), item2, item3) -> Some(tail,item1,item2,item3)
    | _ -> None
///Decomposes a collection by 4 elements, from the start.
let inline (|First4|_|) o =
    match o with
    | First2(First2(tail, item1, item2), item3, item4) -> Some(tail, item1,item2,item3,item4)
    | _ -> None

///Matches an empty collection.
let inline (|Nil|_|) o = 
    if o |> Gen.isEmpty then
        Some()
    else
        None

///Decomposes a collection by two elements, one from each end.
let inline (|Mid|_|) o = 
    match o with
    | First(first, Last(mid, last)) -> Some(first, mid, last)
    | _ -> None

///Decomposes a collection by four elements, two from each end.
let inline (|Mid2|_|) o =   
    match o with
    | First2(item1, item2, Last2(mid, item3, item4)) -> Some(item1,item2,mid,item3,item4)
    | _ -> None