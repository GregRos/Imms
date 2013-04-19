[<AutoOpen>]
module SolidFS.VectorOverloads
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

    member this.SelectMany f = 
        this.SelectMany(toFunc1 f)