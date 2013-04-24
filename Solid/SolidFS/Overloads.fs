[<AutoOpen>]
module Solid.FSharp.Overloads
open Solid
type 'v Vector with
    member this.Fold(initial, f) = 
        this.Fold(initial, toFunc2 f)

    member this.FoldBack(initial, f) = 
        this.FoldBack(initial, toFunc2 f)

    member this.ForEach f= 
        this.ForEach(toAction f)

    member this.ForEachBack f = 
        this.ForEachBack(toAction f)

    member this.ForEachWhile f =
        this.ForEachWhile(toFunc1 f)

    member this.ForEachBackWhile f = 
        this.ForEachBackWhile(toFunc1 f)

    member this.TakeWhile f = 
        this.TakeWhile(toFunc1 f)

    member this.Select f= 
        this.Select(toFunc1 f)

    member this.Where f = 
        this.Where(toFunc1 f)

    member this.SelectMany (f : 'v -> 'u seq) = 
        this.SelectMany(toFunc1 f)

type 'v FlexibleList with
    member this.IndexOf (f : 'v -> bool) = 
        match this.IndexOf(f) with
        | n when n.HasValue -> Some (n.Value)
        | _ -> None
    member this.Where (f : 'v -> bool) = 
        this.Where(f |> toFunc1)

    member this.Select f = 
        this.Select(toFunc1 f)

    member this.Fold(initial,f) = 
        this.Fold(initial, toFunc2 f)

    member this.ForEach f = 
        this.ForEach(toAction f)

    member this.ForEachWhile f =
        this.ForEachWhile(toFunc1 f)

    member this.ForEachBack f = 
        this.ForEachBack(toAction f)

    member this.ForEachBackWhile f=
        this.ForEachBackWhile(toFunc1 f)

    member this.FoldBack(initial, f) =
        this.FoldBack(initial, toFunc2 f)

    member this.TakeWhile f =
        this.TakeWhile(toFunc1 f)

    member this.SkipWhile f = 
        this.SkipWhile(toFunc1 f)

    member this.SelectMany (f : 'v -> 'u seq) = 
        this.SelectMany(toFunc1 f)