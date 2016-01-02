///For internal use. Implements a generic collection builder used in the Builders module.
module Imms.FSharp.Implementation.BuilderTypes
open Imms
open Imms.FSharp
open System
open Imms.FSharp.Implementation
open Imms.FSharp.Operators
type internal collection_ops<'elem, 'many, 'col> = 
    {
        left_combine : 'elem -> 'many -> 'many
        empty : unit -> 'many
        of_seq : 'elem seq -> 'many
        right_combine : 'elem -> 'many -> 'many
        concat : 'many * 'many -> 'many
        final : 'many -> 'col
    }

type GenericStep<'elem, 'col> = 
    internal
    | Nothing
    | Just of 'elem
    | Many of 'col

type GenericBuilder<'elem, 'many, 'col> internal (ops : collection_ops<'elem, 'many, 'col>) =
    member x.Yield (v : 'elem) = v |> GenericStep.Just
    member x.YieldFrom (vs : 'elem seq) = vs |> ops.of_seq |> GenericStep.Many
    member x.YieldFrom (vs : 'many) = vs |> GenericStep.Many
    member x.Run (vs : GenericStep<_,_>) = 
        match vs with
        | Nothing -> ops.empty() |> ops.final
        | Just v -> ops.empty() |> ops.left_combine v |> ops.final
        | Many vs -> vs |> ops.final
    member x.For (vs : 'elem2 seq, f : 'elem2 -> GenericStep<'elem, 'many>) = 
        let mutable newCol = ops.empty()
        for v in vs do
            match f v with
            | Nothing -> ()
            | Just v -> newCol <- newCol |> ops.right_combine v
            | Many vs -> newCol <- ops.concat(newCol, vs)
        newCol |> GenericStep.Many

    member x.Combine (col1 : GenericStep<_,_>, col2 : GenericStep<_,_>) = 
        match col1, col2 with
        | Nothing, x | x, Nothing -> x
        | Just x, Just y -> ops.empty() |> ops.right_combine x |> ops.right_combine y |> Many
        | Just x, Many ys -> ys |> ops.left_combine x |> Many
        | Many xs, Just y -> xs |> ops.right_combine y |> Many
        | Many xs, Many ys -> ops.concat(xs,ys) |> Many
    member x.Zero() = GenericStep.Nothing
    member x.Delay f = f()

let inline internal seq_build (final : 's -> 'r) : collection_ops<'elem, 's, 'r>= 
    {
        left_combine = fun elem many -> elem ^+> many
        empty = Ops.getEmpty
        of_seq = fun sq -> Ops.getEmpty() <++ sq
        right_combine = fun elem many -> many <+ elem
        concat = fun (many1, many2) -> many1 <++ many2
        final = final
    }

let inline internal set_build c : collection_ops<'elem, 's, 's> =
    {
        left_combine = fun elem many -> many /+ elem
        empty = fun () -> Ops.getEmptyWith c
        of_seq = fun sq -> Ops.union sq (Ops.getEmptyWith c)
        right_combine = fun elem many -> many /+ elem
        concat = fun (many1,many2) -> many1 /++ many2
        final = id
    }

let inline internal map_build c : collection_ops<Kvp<'k,'v>, 's, 's> =
    {
        left_combine = fun e many -> many /+ e
        empty = fun () -> (Ops.getEmptyWith c : 's)
        of_seq = fun sq -> (Ops.getEmptyWith c : 's) /++ sq
        right_combine = fun elem many -> many /+ elem
        concat = fun (many1,many2) -> many1 |> Ops.addRange many2
        final = id
    }

