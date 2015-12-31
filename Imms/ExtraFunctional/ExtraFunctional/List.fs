module ExtraFunctional.List
open System.Collections.Generic
open System
let correlate (left : 'v list, right : 'v list) (key : 'v -> 'k when 'k : equality) =
    let dict = Dictionary()
    for item in right do
        dict.[key item] <- item

    let mutable result = []
    for item in left do
        let k = key item
        if dict.ContainsKey k then
            result <- (item,dict.[k])::result
    result

let zipAll (lsts : #seq<#seq<_>>) = 
    let cols = lsts |> Seq.toArray
    let iterators = cols |> Array.map (Seq.getEnumerator)
    [while iterators |> Array.forall (fun x -> x.MoveNext()) do
        yield iterators |> Array.mapi (fun i x -> cols.[i],x.Current) |> Array.toList]

let ofType<'input, 'output> (lst : 'input list) = 
    let filter x = 
        match x :> obj with
        | :? 'output as out -> Some out
        | _ -> None
    lst |> List.choose filter

let cast<'a, 'b> (lst : 'a list) = lst |> List.map (fun x -> x :> obj :?> 'b)

///Cross/cartesian product over lists.
let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = [for item1 in seq1 do for item2 in seq2 -> item1,item2]

let singleton x = [x]

let apply1 arg fs = fs |> List.map (Fun.apply1 arg)
let apply2 arg1 arg2 fs = fs |> List.map (Fun.apply2 arg1 arg2)

let cross_apply1 (args : _ list) (fs : _ list) = fs |> Seq.cross_apply1 args |> Seq.toList
let chain_iter f vs = 
    vs |> List.iter f
    vs

