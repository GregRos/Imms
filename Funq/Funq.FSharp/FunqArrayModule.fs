[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Funq.FSharp.FunqArray
open Funq.Collections
let empty<'t> = FunqArray<'t>.Empty

let inline private asArr o = o :_ FunqArray
///Generic "specialized pipe" operator for reslolving type inference.
let inline (=>) (x : _ FunqArray) f = f x
 
let ofItem v = FunqArray<'t>.Empty <+ v

let ofSeq vs = FunqArray<'t>.Empty <++ vs

let map (f : 'a -> 'b) arr = arr => GenSeq.map f
let collect f arr          = arr => GenSeq.collect f
let iter f arr             = arr => GenSeq.iter f
let iterWhile f arr        = arr => GenSeq.iterWhile f
let iterBack f arr         = arr => GenSeq.iterBack f
let iterBackWhile f arr    = arr => GenSeq.iterBackWhile f
let fold initial f arr     = arr => GenSeq.fold initial f
let foldBack initial f arr = arr => GenSeq.foldBack initial f
let reduce f arr           = arr => GenSeq.reduce f
let reduceBack f arr       = arr => GenSeq.reduceBack f
let filter f arr           = arr => GenSeq.filter f
let tryFind f arr          = arr => GenSeq.tryFind f
let tryFindIndex f arr     = arr => GenSeq.tryFindIndex f
let first arr              = arr => GenSeq.first
let tryFirst arr           = arr => GenSeq.tryFirst
let last arr               = arr => GenSeq.last
let tryLast arr            = arr => GenSeq.tryLast
let exists f arr           = arr => GenSeq.exists f
let take n arr             = arr => GenSeq.take n
let skip n arr             = arr => GenSeq.skip n
let skipWhile f arr        = arr => GenSeq.skipWhile f
let isEmpty arr            = arr => GenSeq.isEmpty
let takeWhile f arr        = arr => GenSeq.takeWhile f
let zip r l                = l => GenSeq.zip r
let count f arr            = arr => GenSeq.count f
let length arr             = arr => GenSeq.length
let scan r f arr           = arr => GenSeq.scan r f
let scanBack r f arr       = arr => GenSeq.scanBack r f
let nth n arr              = arr => GenSeq.nth n
let pick f arr             = arr => GenSeq.pick f
let forAll f arr           = arr => GenSeq.forAll f 
let slice(i1,i2) arr       = arr => GenSeq.slice(i1,i2)
let addLast v arr          = arr => GenSeq.addLast v
let addLastRange vs arr    = arr => GenSeq.addLastRange vs
