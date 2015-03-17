[<ComplRep(ComplFlags.ModuleSuffix)>]
module Funq.FSharp.FunqSet
open Funq.FSharp
open Funq.Collections
open System.Collections.Generic
open Funq
(*
///Generic "specialized pipe" operator for reslolving type inference.
let inline (=>) (x : FunqSet<_>) f   = f x

let inline def<'t>                     = EqualityComparer<'t>.Default;

let map f map                          = map => GenSet.map f def 
let choose f map                       = map =>     .choose f def
let iter f map                         = map => GenSet.iter f
let iterWhile f map                    = map => GenSet.iterWhile f
let fold initial f map                 = map => GenSet.fold initial f
let reduce f map                       = map => GenSet.reduce f
let filter f map                       = map => GenSet.filter f
let tryFind f map                      = map => GenSet.tryFind f
let exists f map                       = map => GenSet.exists f
let isEmpty map                        = map => GenSet.isEmpty
let count f map                        = map => GenSet.count f
let length map                         = map => GenSet.length
let pick f map                         = map => GenSet.pick f
let forAll f map                       = map => GenSet.forAll f 
let add k map                       = map => GenSet.add k
let drop k map                         = map => GenSet.drop k
let update(k,v) map                    = map => GenSet.update k v
let mapValues (f : 'a -> 'b -> 'c) map = map => GenSet.mapValues f
let get k map                          = map => GenSet.get k
let tryGet k map                       = map => GenSet.tryGet k
let join f m2 m1                       = m1 => GenSet.join f m2
let merge f m2 m1                      = m1 => GenSet.merge f m2
let except m2 m1                       = m1 => GenSet.except m2
let difference m2 m1                   = m1 => GenSet.difference m2
let addMany (kvps : ('k * 'v) seq) m   = 
    let pairs                          = kvps |> Seq.map (fun (a,b) -> Kvp.Of(a,b))
    m                                  => GenSet.addMany pairs
let dropMany (ks : 'k seq) m           = m => GenSet.dropMany ks


*)