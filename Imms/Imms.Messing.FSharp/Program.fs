// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.


[<EntryPoint>]
let main argv = 
    let list = ImmSet.empty
    let items = immMap {
            for i in 0 .. 10 do yield Kvp(i,i)
        }

    let list = immList {
            for i in 0 .. 10 do yield i
        }

    let a = ImmList.ofItem 1
    let b = ImmList.ofItem 2
    let c = 1 + b + 2

    printfn "%A" argv
    0 // return an integer exit code
