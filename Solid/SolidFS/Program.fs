// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module SolidFS.Main
open SolidFS.Expressions





[<EntryPoint>]
let main argv = 
    let y = XList.ofSeq [0 .. 5]
    let x = y.[0 .. 3]
    let ys = vector {for i in {0 .. 100} -> i}
    let zs = XList.ofSeq {0 .. 100}
    printfn "%A" zs
    0 // return an integer exit code
