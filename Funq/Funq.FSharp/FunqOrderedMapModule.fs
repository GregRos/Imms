[<ComplRep(ComplFlags.ModuleSuffix)>]
module Funq.FSharp.FunqOrderedMap
open Funq.Collections
open Funq
open System.Collections.Generic

///Generic "specialized pipe" operator for reslolving type inference.
let inline (=>) (x : FunqOrderedMap<_,_>) f = f x

let inline def<'t> = EqualityComparer<'t>.Default;

let mapValues (f : 'a -> 'b -> 'c) map       = map => GenMap.mapValues f
let mapWith c f map                          = map => GenMap.map f c 
let chooseWith c f map                       = map => GenMap.choose f c
let iter f map                               = map => GenMap.iter f
let iterWhile f map                          = map => GenMap.iterWhile f
let fold initial f map                       = map => GenMap.fold initial f
let reduce f map                             = map => GenMap.reduce f
let filter f map                             = map => GenMap.filter f
let tryFind f map                            = map => GenMap.tryFind f
let exists f map                             = map => GenMap.exists f
let isEmpty map                              = map => GenMap.isEmpty
let count f map                              = map => GenMap.count f
let length map                               = map => GenMap.length
let pick f map                               = map => GenMap.pick f
let forAll f map                             = map => GenMap.forAll f 
let add(k,v) map                             = map => GenMap.add k v
let drop k map                               = map => GenMap.drop k
let update(k,v) map                          = map => GenMap.update k v
let maxItem map                              = map => GenMap.maxItem
let minItem map                              = map => GenMap.minItem
let get k map                                = map => GenMap.get k
let tryGet k map                             = map => GenMap.tryGet k
let join f m2 m1                             = m1 => GenMap.join f m2
let merge f m2 m1                            = m1 => GenMap.merge f m2
let except m2 m1                             = m1 => GenMap.except m2
let difference m2 m1                         = m1 => GenMap.difference m2
let addMany (kvps : ('k * 'v) seq) m         = 
    let pairs                                = kvps |> Seq.map (fun (a,b) -> Kvp.Of(a,b))
    m                                        => GenMap.addMany pairs
let dropMany (ks : 'k seq) m                 = m => GenMap.dropMany ks



