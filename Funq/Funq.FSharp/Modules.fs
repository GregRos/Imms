namespace Funq.FSharp
open System.Runtime.CompilerServices
open Funq.Collections
open Funq.FSharp.Implementation.ModuleTypes
open System
open Funq
open System.Collections.Generic
[<Obsolete("Shouldn't be visible")>]
[<Extension>]
type ModuleExt private () =
    [<Extension>]
    static member empty (map : FunqOrderedMapModule<'k,'v> when 'k :> IComparable<'k>) =
        FunqOrderedMap.Empty<'k,'v>()

    [<Extension>]
    static member empty (set : FunqOrderedSetModule<'k> when 'k :> IComparable<'k>) =
        FunqOrderedSet.Empty<'k>();

    [<Extension>]
    static member ofSeq (map : FunqOrderedMapModule<'k,'v> when 'k :> IComparable<'k>, vs : ('k * 'v) seq) =
        FunqOrderedMap.ToFunqOrderedMap(vs |> Seq.map (Kvp.Of))

    [<Extension>]
    static member ofSeq (set : FunqOrderedSetModule<'k> when 'k :> IComparable<'k>, vs : 'k seq) =
        FunqOrderedSet.ToFunqOrderedSet(vs)

[<AutoOpen>]
module Modules = 
    open Funq.FSharp.Implementation.ModuleTypes
    let FunqList<'v> = FunqListModule<'v>.instance
    let FunqVector<'v> = FunqVectorModule<'v>.instance
    let FunqMap<'k,'v> = FunqMapModule<'k, 'v>()
    let FunqOrderedMap<'k,'v> = FunqOrderedMapModule<'k,'v>.instance
    let FunqSet<'v> = FunqSetModule<'v>.instance
    let FunqOrderedSet<'v> = FunqOrderedSetModule<'v>.instance

    
