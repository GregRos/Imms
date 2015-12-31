namespace Funq.FSharp
open System.Runtime.CompilerServices
open Funq.Collections
open Funq.FSharp.Implementation.ModuleTypes
open System
open Funq
open System.Collections.Generic
open Funq
[<Obsolete("Shouldn't be visible")>]
[<Extension>]
type ModuleExt private () =
    [<Extension>]
    static member empty (map : FunqOrderedMapModule<'k,'v> when 'k : comparison) =
        FunqOrderedMap.Empty<'k,'v>(null)

    [<Extension>]
    static member empty (set : FunqOrderedSetModule<'k> when 'k : comparison) =
        FunqOrderedSet.Empty<'k>(null);

    [<Extension>]
    static member ofSeq (map : FunqOrderedMapModule<'k,'v> when 'k : comparison, vs : ('k * 'v) seq) =
        FunqOrderedMap.ToFunqOrderedMap(vs |> Seq.map (Kvp.Of), null)

    [<Extension>]
    static member ofSeq (set : FunqOrderedSetModule<'k> when 'k : comparison, vs : 'k seq) =
        FunqOrderedSet.ToFunqOrderedSet(vs, null)


    
