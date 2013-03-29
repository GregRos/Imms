[<AutoOpen>]
module SolidFS.Expressions
open Solid

open SolidFS.Operators
type Step<'a> = 
    internal
    | Nothing
    | Just of 'a
    | Many of 'a FlexibleList

type FlexibleListBuilder internal() = 
    member __.Yield (v : 'a) = Just v
    member __.YieldFrom (vs : 'a seq) = 
        Many <| vs.ToFlexibleList()
    member __.YieldFrom (vs : 'a FlexibleList) = 
        Many vs



    member __.Run step = 
        match step with
        | Nothing -> FlexibleList.Empty
        | Just v -> XList.empty <+ v
        | Many S -> S
    member __.For (vs : 'b seq,f : 'b -> 'a) =
        let mutable Sq = FlexibleList.Empty
        vs |> Seq.map f |> Sq.AddLastRange |> Many
    member __.Combine (step1 : Step<'a>, step2 : Step<'a>) = 
        match step1,step2 with
        | Nothing, x -> x
        | x, Nothing -> x
        | Just x, Just y -> XList.empty <+ x <+ y |> Many
        | Many X, Just y -> X <+ y |> Many
        | Many X,Many Y -> X <+> Y |> Many
        | Just x, Many Y -> x +> Y |> Many

    member __.Zero() = Nothing
    member __.Delay f = f()

type VectorBuilder internal() = 
    member __.Zero() = Nothing
    member __.Yield v = Just v
    member __.YieldFrom (vs : 'a seq) = vs
    member __.For (vs : 'b seq, f : 'b -> 'a) = 
        let mutable S = XList.empty
        for item in vs do
            S <- S <+ item
        Many S
    member __.Run step = 
        match step with
        | Nothing -> XList.empty
        | Just v -> XList.ofItem v
        | Many V -> V

    
        

    member __.Combine (step1,step2) = 
        match step1,step2 with
        | Nothing, x -> x
        | x, Nothing -> x
        | Just x, Just y -> XList.empty <+ x <+ y |> Many
        | Just x, Many ys -> x +> ys |> Many
        | Many xs, Just y -> xs <+ y |> Many
        | Many xs, Many ys -> xs <+> ys |> Many
    
let vector = VectorBuilder()
let xlist = FlexibleListBuilder()
        

