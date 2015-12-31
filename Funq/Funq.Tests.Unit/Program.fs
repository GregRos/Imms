// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Unit.Main
open Funq.Collections
open Funq.FSharp.Operators
open System.Reflection
open System
open Funq
open Funq.FSharp
open Funq.FSharp.Implementation

let tryThis<'elem> (v : FunqList<'elem>) (x : 'elem) = v <+ x
   
let test f = FunqList.empty.All f

[<EntryPoint>]
let main argv = 
    let tuple = (1, "hi", 3)
    printfn "%A" tuple
    0 // return an integer exit code
