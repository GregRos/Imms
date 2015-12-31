module Imms.Tests.Integrity.Target
open System
let rnd1 = Random()
let seed = rnd1.Next()
let lst n =
    let rnd = Random(seed)
    [for i in 0 .. n -> rnd.Next()]
module Imms = 
    let List n = ImmListWrapper<_>.FromSeq (lst n)
    let Vector n= ImmVectorWrapper<_>.FromSeq (lst n)
    let Set = ImmSetWrapper<_>.FromSeq 
    let OrderedSet = ImmOrderedSetWrapper<_>.FromSeq 
    let Map s = ImmMapWrapper<_>.FromSeq s
    let OrderedMap s = ImmOrderedMapWrapper<_>.FromSeq s

module Sys = 
    let List n= SeqReferenceWrapper<_>.FromSeq (lst n)
    let OrderedSet = ReferenceSetWrapper<_>.FromSeq
    let OrderedMap = MapReferenceWrapper<_>.FromSeq

