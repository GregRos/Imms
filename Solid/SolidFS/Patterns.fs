[<AutoOpen>]
module SolidFS.Patterns

let inline (|MatchList|_|) (len : int) o= 
    if o |> Gen.length > len then
        None
    else
        Some(List.ofSeq o)

let inline (|Last|_|) o = 
    if o |> Gen.isEmpty then
        None
    else
        let last = o |> Gen.last
        let rest = o |> Gen.dropLast
        Some(rest,last)

let inline (|Last2|_|) o =
    if o |> Gen.length < 2 then
        None
    else
        let last1 = o |> Gen.last
        let rest1 = o |> Gen.dropLast
        let last2 = rest1 |> Gen.last
        let rest2 = rest1 |> Gen.dropLast
        Some(rest2, last2, last1)

let inline (|Last3|_|) o = 
    match o with
    | Last(Last2(tail, item1,item2),item3) -> Some(tail, item1,item2,item3)
    | _ -> None

let inline (|Last4|_|) o = 
    match o with
    | Last2(Last2(tail, item1, item2), item3, item4) -> Some(tail, item1, item2, item3, item4)
    | _ -> None

let inline (|First|_|) o =
    if o |> Gen.isEmpty then
        None
    else
        let first = o |> Gen.first
        let rest = o |> Gen.dropFirst
        Some(first,rest)

let inline (|First2|_|) o = 
    if o |> Gen.length < 2 then
        None
    else
        let first1 = o |> Gen.first
        let rest1 = o |> Gen.dropFirst
        let first2 = rest1 |> Gen.first
        let rest2 = rest1 |> Gen.dropFirst
        Some(first1, first2, rest2)


let inline (|First3|_|) o =
    match o with
    | First2(First(tail, item1), item2, item3) -> Some(tail,item1,item2,item3)
    | _ -> None

let inline (|First4|_|) o =
    match o with
    | First2(First2(tail, item1, item2), item3, item4) -> Some(tail, item1,item2,item3,item4)
    | _ -> None

let inline (|Nil|_|) o = 
    if o |> Gen.isEmpty then
        Some()
    else
        None