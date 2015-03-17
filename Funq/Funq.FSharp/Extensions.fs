[<AutoOpen>]
module Funq.FSharp.Extensions
open Funq
open System
open System.Runtime.CompilerServices
open Funq.FSharp.Implementation
open Funq.Abstract
type Trait_Iterable<'elem, 'seq, 'builder when 'builder :> IterableBuilder<'elem> and 'seq :> Trait_Iterable<'elem, 'seq, 'builder>> with
    member x.All f = x.All(toFunc1 f)
    member x.Any f = x.Any(toFunc1 f)
    member x.Find f = x.Find(toFunc1 f)
    member x.Where f = x.Where(toFunc1 f)
    member x.ForEach f = x.ForEach(toAction f)
    member x.ForEachWhile f = x.ForEachWhile(toFunc1 f)
    member x.Aggregate(r,f) = x.Aggregate(r, toFunc2 f)
    member x.Reduce f = x.Reduce(toFunc2 f)
    member x.Reduce (initial, f) = x.Reduce(toFunc1 initial, toFunc2 f)
    member x.Pick f = x.Pick(f >> toOption |> toFunc1) |> fromOption
    member x.Count f = x.Count(toFunc1 f)

type Trait_Sequential<'elem, 'seq when 'seq :> Trait_Sequential<'elem,'seq>> with
    member x.TryFirst = x.TryFirst |> fromOption
    member x.TryLast = x.TryLast |> fromOption
    member x.GetRange(i1, i2) = x.[i1,i2]
    member x.ForEachBack f = x.ForEachBack(toAction f)
    member x.ForEachBackWhile f = x.ForEachBackWhile(toFunc1 f)
    member x.AggregateBack(r,f) = x.AggregateBack(r, toFunc2 f)
    member x.ReduceBack f = x.ReduceBack(toFunc2 f)
    member x.TakeWhile f = x.TakeWhile(toFunc1 f)
    member x.SkipWhile f = x.SkipWhile(toFunc1 f)
    member x.FindLast f = x.FindLast(toFunc1 f)
    member x.FindIndex f = x.FindIndex(toFunc1 f)
    member x.ReduceBack (initial, f) = x.ReduceBack(toFunc1 initial, toFunc2 f)

    
let inline infer (x : 's when 's :> Trait_Sequential<'elem, 's>) = x
[<Extension>]
type FunqExtensions private () =
    [<Extension>]
    static member inline Select (list : 'list, f) = 
        (^list : (member Select : Func<'elem, 'elem2> -> 't) list, f |> toFunc1)
    [<Extension>]
    static member inline Choose (list : 'list, f : 'elem -> 'elem2 option) =
        (^list : (member Choose<'elem2> : Func<'elem, 'elem2 Funq.Option> -> 't) list, f >> toOption |> toFunc1)
    [<Extension>]
    static member inline SelectMany (list : 'list, f) =
        (^list : (member SelectMany : Func<'elem, 'elem2 seq> -> 't) list, f |> toFunc1)
    [<Extension>]
    static member inline Scan (list : 'list, r, f) = 
        (^list : (member Scan : 'r * Func<'r, 'elem, 'r> -> 't) list,r,  f |> toFunc2)
    [<Extension>]
    static member inline ScanBack (list : 'list, r, f) =
        (^list : (member ScanBack : 'r * Func<'r, 'elem, 'r> -> 't) list, r, f |> toFunc2)
    [<Extension>]
    static member inline GroupBy (list : 'list, keySelector) = 
        (^list : (member GroupBy : Func<'elem, 'key> -> 'list2) list, keySelector |> toFunc1)
    [<Extension>]
    static member inline Zip (list : 'list, other : 'elem2 seq) =
        (^ list: (member Zip : 'elem2 seq -> 'list2) list, other)
