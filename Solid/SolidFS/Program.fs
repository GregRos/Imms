// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Solid.Main
open Solid.Expressions
[<EntryPoint>]
let main argv = 
    let ys = Vector {for i in {0 .. 100} -> i}
    let zs = Sequence {for i in {0 .. 100} -> i}
    0 // return an integer exit code
