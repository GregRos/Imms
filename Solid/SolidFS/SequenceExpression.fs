module SolidFS.Expressions
open Solid

open SolidFS.Operators
type Step<'a> = 
    internal
    | Nothing
    | Just of 'a
    | Many of 'a Sequence

type SequenceBuilder internal() = 
    member __.Yield (v : 'a) = Just v
    member __.YieldFrom (vs : 'a seq) = 
        Many <| vs.ToSequence()
    member __.YieldFrom (vs : 'a Sequence) = 
        Many vs

    member __.Run step = 
        match step with
        | Nothing -> Sequence.Empty()
        | Just v -> empty() <+ v
        | Many S -> S
    member __.For (vs : 'b seq,f : 'b -> 'a) =
        let mutable Sq = Sequence.Empty()
        vs |> Seq.map f |> Sq.AddRangeLast |> Many
    member __.Combine (step1 : Step<'a>, step2 : Step<'a>) = 
        match step1,step2 with
        | Nothing, x -> x
        | x, Nothing -> x
        | Just x, Just y -> empty() <+ x <+ y |> Many
        | Many X, Just y -> X <+ y |> Many
        | Many X,Many Y -> X <+> Y |> Many
        | Just x, Many Y -> x +> Y |> Many

    member __.Zero = Nothing
    member __.Delay f = f()

type VectorBuilder internal() = 
    member __.Yield v = Just v
    member __.YieldFrom (vs : 'a seq) = vs
    member __.For (vs : 'b seq, f : 'b -> 'a) = 
        let mutable S = empty()
        for item in vs do
            S <- S <+ item
        Many S
    member __.Run step = 
        match step with
        | Nothing -> empty()
        | Just v -> empty() <+ v
        | Many V -> V.ToVector()

    member __.Combine (step1,step2) = 
        match step1,step2 with
        | Nothing, x -> x
        | x, Nothing -> x
        | Just x, Just y -> empty() <+ x <+ y |> Many
        | Just x, Many ys -> x +> ys |> Many
        | Many xs, Just y -> xs <+ y |> Many
        | Many xs, Many ys -> xs <+> ys |> Many
    
let Vector = VectorBuilder()
let Sequence = SequenceBuilder()
        

