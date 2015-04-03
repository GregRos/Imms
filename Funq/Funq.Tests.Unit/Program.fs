// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Unit.Main
open Funq.Collections
open Funq.FSharp.Operators
open System.Reflection
open System

let tryThis<'elem> (v : FunqList<'elem>) (x : 'elem) = v <+ x
   
[<EntryPoint>]
let main argv = 

  
    printfn "%A" argv
    0 // return an integer exit code
