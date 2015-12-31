[<AutoOpen>]
module Funq.FSharp.MoreModules
open Funq
open Funq.FSharp
open Funq.FSharp.Implementation

module FunqList = 
    let addFirst value (collection:'elem FunqList) = collection.AddFirst(value)
    let insert index value (collection :'elem FunqList) = collection.Insert(index,value)
    let remove index (collection :'elem FunqList) = collection.RemoveAt(index)
     
module FunqSet = 
    let emptyWith(Eq : IEq<'elem>) = FunqSet<'elem>.Empty(Eq)
    let empty<'v when 'v : equality> = FunqSet<'v>.Empty()
    let ofSeq (vs : seq<'k> when 'k : equality) = FunqSet.ToFunqSet(vs)
    let ofSeqWith eq vs = FunqSet.ToFunqSet(vs, eq)

module FunqOrderedSet = 
    let empty<'k when 'k : comparison> = FunqOrderedSet<'k>.Empty(null)
    let emptyWith(cm : ICmp<'elem>) = FunqOrderedSet<'elem>.Empty(cm)
    let ofSeq (vs : 'a seq when 'a : comparison) = FunqOrderedSet.ToFunqOrderedSet(vs, null)
    let ofSeqWith cmp vs = FunqOrderedSet.ToFunqOrderedSet(vs, cmp)
    let byOrder i (set : _ FunqOrderedSet) = set.ByOrder i
    let min (set : _ FunqOrderedSet) = set.MinItem
    let max (set : _ FunqOrderedSet) = set.MaxItem
    let removeMin (set :_ FunqOrderedSet) = set.RemoveMin()
    let removeMax (set :_ FunqOrderedSet) = set.RemoveMax()

module FunqMap = 
    let emptyWith(eq : Eq<'key>) = FunqMap<'key,'value>.Empty(eq)
    let empty<'key,'value when 'key : equality> = FunqMap<'key,'value>.Empty()
    let ofSeq (vs : seq<'key * 'value> when 'key : equality) = FunqMap.ToFunqMap(vs |> Seq.map (Kvp.Of))
    let ofSeq' (vs : seq<Kvp<'key,'value>> when 'key : comparison) = FunqMap.Empty(null).op_AddRange(vs)
    let ofSeqWith eq vs = FunqMap.ToFunqMap(vs, eq)
    let ofSeqWith' eq (vs : Kvp<_,_> seq) = FunqMap.Empty(eq).op_AddRange(vs)

module FunqOrderedMap = 
    let empty<'key, 'value when 'key : comparison> = FunqOrderedMap<'key, 'value>.Empty(null)
    let emptyWith(cm : Cmp<'key>) = FunqOrderedMap<'key,'value>.Empty(cm)
    let ofSeq (vs : seq<'key * 'value> when 'key : comparison) = FunqOrderedMap.Empty(null).op_AddRange(vs)
    let ofSeq' (vs : seq<Kvp<'key,'value>> when 'key : comparison) = FunqOrderedMap.Empty(null).op_AddRange(vs)
    let ofSeqWith cmp vs = FunqOrderedMap.ToFunqOrderedMap(vs, cmp)
    let ofSeqWith' cmp (vs : Kvp<_,_> seq) = FunqOrderedMap.Empty(cmp).op_AddRange(vs)
    let byOrder i (map : FunqOrderedMap<_,_>) = map.ByOrder i |> Kvp.ToTuple
    let min (map : FunqOrderedMap<_,_>) = map.MinItem |> Kvp.ToTuple
    let max (map : FunqOrderedMap<_,_>) = map.MaxItem |> Kvp.ToTuple
    let removeMin (map : FunqOrderedMap<_,_>) = map.RemoveMin()
    let removeMax (map : FunqOrderedMap<_,_>) = map.RemoveMax()
    