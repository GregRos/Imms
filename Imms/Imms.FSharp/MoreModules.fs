[<AutoOpen>]
module Imms.FSharp.MoreModules
open Imms
open Imms.FSharp
open Imms.FSharp.Implementation

module ImmList = 
    let addFirst value (collection:'elem ImmList) = collection.AddFirst(value)
    let insert index value (collection :'elem ImmList) = collection.Insert(index,value)
    let remove index (collection :'elem ImmList) = collection.RemoveAt(index)
     
module ImmSet = 
    let emptyWith(Eq : IEq<'elem>) = ImmSet<'elem>.Empty(Eq)
    let empty<'v when 'v : equality> = ImmSet<'v>.Empty()
    let ofSeq (vs : seq<'k> when 'k : equality) = ImmSet.ToImmSet(vs)
    let ofSeqWith eq vs = ImmSet.ToImmSet(vs, eq)

module ImmOrderedSet = 
    let empty<'k when 'k : comparison> = ImmOrderedSet<'k>.Empty(null)
    let emptyWith(cm : ICmp<'elem>) = ImmOrderedSet<'elem>.Empty(cm)
    let ofSeq (vs : 'a seq when 'a : comparison) = ImmOrderedSet.ToImmOrderedSet(vs, null)
    let ofSeqWith cmp vs = ImmOrderedSet.ToImmOrderedSet(vs, cmp)
    let byOrder i (set : _ ImmOrderedSet) = set.ByOrder i
    let min (set : _ ImmOrderedSet) = set.MinItem
    let max (set : _ ImmOrderedSet) = set.MaxItem
    let removeMin (set :_ ImmOrderedSet) = set.RemoveMin()
    let removeMax (set :_ ImmOrderedSet) = set.RemoveMax()

module ImmMap = 
    let emptyWith(eq : Eq<'key>) = ImmMap<'key,'value>.Empty(eq)
    let empty<'key,'value when 'key : equality> = ImmMap<'key,'value>.Empty()
    let ofSeq (vs : seq<'key * 'value> when 'key : equality) = ImmMap.ToImmMap(vs |> Seq.map (Kvp.Of))
    let ofSeq' (vs : seq<Kvp<'key,'value>> when 'key : comparison) = ImmMap.Empty(null).op_AddRange(vs)
    let ofSeqWith eq vs = ImmMap.ToImmMap(vs, eq)
    let ofSeqWith' eq (vs : Kvp<_,_> seq) = ImmMap.Empty(eq).op_AddRange(vs)

module ImmOrderedMap = 
    let empty<'key, 'value when 'key : comparison> = ImmOrderedMap<'key, 'value>.Empty(null)
    let emptyWith(cm : Cmp<'key>) = ImmOrderedMap<'key,'value>.Empty(cm)
    let ofSeq (vs : seq<'key * 'value> when 'key : comparison) = ImmOrderedMap.Empty(null).op_AddRange(vs)
    let ofSeq' (vs : seq<Kvp<'key,'value>> when 'key : comparison) = ImmOrderedMap.Empty(null).op_AddRange(vs)
    let ofSeqWith cmp vs = ImmOrderedMap.ToImmOrderedMap(vs, cmp)
    let ofSeqWith' cmp (vs : Kvp<_,_> seq) = ImmOrderedMap.Empty(cmp).op_AddRange(vs)
    let byOrder i (map : ImmOrderedMap<_,_>) = map.ByOrder i |> Kvp.ToTuple
    let min (map : ImmOrderedMap<_,_>) = map.MinItem |> Kvp.ToTuple
    let max (map : ImmOrderedMap<_,_>) = map.MaxItem |> Kvp.ToTuple
    let removeMin (map : ImmOrderedMap<_,_>) = map.RemoveMin()
    let removeMax (map : ImmOrderedMap<_,_>) = map.RemoveMax()
    