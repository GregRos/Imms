///Provides active patterns for consuming Imm collections and other types. Auto-opened.
[<AutoOpen>]
module Imms.FSharp.Patterns
open Imms
open Imms.FSharp.Implementation

///Compatibility active pattern for matching Imms's Optional type.
let (|ImmSome|ImmNone|) (optional : Imms.Optional<'T>) = 
    if optional.IsSome then 
        ImmSome(optional.Value)
    else
        ImmNone
///Decomposes a collection by 1 element, from the end. 
let inline (|Last1|_|) o = 
    if o |>Ops.isEmpty then
        None
    else
        let last = o |> Ops.last
        let rest = o |> Ops.removeLast
        Some(rest,last)


///Decomposes a collection by 2 elements, from the end.
let inline (|Last2|_|) o =
    if o |>Ops.length < 2 then
        None
    else
        let last1 = o |> Ops.last
        let rest1 = o |>Ops.removeLast
        let last2 = rest1 |> Ops.last
        let rest2 = rest1 |>Ops.removeLast
        Some(rest2, last2, last1)

///Decomposes a collection by 3 elements, from the end.
let inline (|Last3|_|) o = 
    match o with
    | Last1(Last2(tail, item1,item2),item3) -> Some(tail, item1,item2,item3)
    | _ -> None

///Decomposes a collection by 4 elements, from the end.
let inline (|Last4|_|) o = 
    match o with
    | Last2(Last2(tail, item1, item2), item3, item4) -> Some(tail, item1, item2, item3, item4)
    | _ -> None
///Decomposes a collection by 1 element, from the start.
let inline (|First1|_|) o =
    if o |>Ops.isEmpty then
        None
    else
        let first = o |> Ops.first
        let rest = o |>Ops.removeFirst
        Some(first,rest)
///Decomposes a collection by 2 elements, from the start.
let inline (|First2|_|) o = 
    if o |>Ops.length < 2 then
        None
    else
        let first1 = o |> Ops.first
        let rest1 = o |>Ops.removeFirst
        let first2 = rest1 |> Ops.first
        let rest2 = rest1 |>Ops.removeFirst
        Some(first1, first2, rest2)

///Decomposes a collection by 3 elements, from the start.
let inline (|First3|_|) o =
    match o with
    | First2(item1, item2, First1(item3, rest)) -> Some(item1, item2, item3, rest)
    | _ -> None
///Decomposes a collection by 4 elements, from the start.
let inline (|First4|_|) o =
    match o with
    | First2(item1, item2, First2(item3, item4, rest)) -> Some(item1,item2,item3,item4,rest)
    | _ -> None

///Matches an empty collection.
let inline (|Nil|_|) o = 
    if o |>Ops.isEmpty then
        Some()
    else
        None

///Decomposes a collection by two elements, one from each end.
let inline (|Mid|_|) o = 
    match o with
    | First1(first, Last1(mid, last)) -> Some(first, mid, last)
    | _ -> None

///Decomposes a collection by four elements, two from each end.
let inline (|Mid2|_|) o =   
    match o with
    | First2(item1, item2, Last2(mid, item3, item4)) -> Some(item1,item2,mid,item3,item4)
    | _ -> None