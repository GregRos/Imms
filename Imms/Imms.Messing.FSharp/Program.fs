// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Imms.FSharp
[<EntryPoint>]
let main argv = 
    let list = ImmSet.empty
    let items = 
        immMap {
            for i in 0 .. 10 do yield Kvp(i,i)
        }

    
    printfn "%A" argv
    0 // return an integer exit code
