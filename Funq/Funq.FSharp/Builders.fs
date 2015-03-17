[<AutoOpen>]
module Funq.FSharp.Builders
open Funq.Collections
open Funq.FSharp
open System
open Funq.FSharp.Implementation
open Funq.FSharp.Operators.Extra
open Funq.FSharp.Implementation.BuilderTypes

let private array_ops<'elem> : collection_ops<'elem, 'elem FunqList, 'elem FunqArray> = seq_build (fun list -> list.ToFunqArray())

let private list_ops<'elem> : collection_ops<'elem, 'elem FunqList, 'elem FunqList> = seq_build id

let private set_ops<'elem> eq : collection_ops<'elem, 'elem FunqSet, 'elem FunqSet> = set_build eq

let private ordered_set_ops<'elem> cm : collection_ops<'elem, 'elem FunqOrderedSet, 'elem FunqOrderedSet> = set_build cm

let private map_ops<'k, 'v> eq : collection_ops<'k * 'v, FunqMap<'k,'v>, FunqMap<'k,'v>> = map_build eq

let private ordered_map_ops<'k, 'v> cm : collection_ops<'k * 'v, FunqOrderedMap<'k,'v>, FunqOrderedMap<'k,'v>> = map_build cm

let funqList<'elem> = GenericBuilder(list_ops<'elem>)
let funqArray<'elem> = GenericBuilder(array_ops<'elem>)
let funqMapWith<'k,'v>(eq : 'k Eq) = GenericBuilder(map_ops<'k,'v>(eq))
let funqSetWith<'elem> eq = GenericBuilder(set_ops<'elem>(eq))
let funqSet<'elem when 'elem : equality> = funqSetWith<'elem>(Eq.Default)
let funqOrderedSetWith<'elem> cm = GenericBuilder(ordered_set_ops<'elem>(cm))
let funqOrderedSet<'elem when 'elem : comparison> = funqOrderedSetWith<'elem>(Cm.Default)
let funqMap<'k,'v when 'k : equality> = funqMapWith<'k,'v>(Eq.Default)
let funqOrderedMapWith<'k,'v> cm = GenericBuilder(ordered_map_ops<'k,'v>(cm))
let funqOrderedMap<'k,'v when 'k : comparison> = funqOrderedMapWith<'k,'v>(Cm.Default)