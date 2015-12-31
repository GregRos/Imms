// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open ExtraFunctional
open System
let rnd = Random()
let getResult n = 
    let space = [|for i in 0 .. 3*n -> ((float n) / sqrt (float (i + 1))) |> int |] 
    let totals = space |> Seq.scan (fun (sum,_) cur -> sum + cur, cur) (0, 0) |> Seq.skip 1 |> Seq.toArray
    let last = (totals.Length - 1) |> Array.get totals |> fst
    fun () -> 
        let r = rnd.Next(0, last)
        let ix = r |> Array.binSearchBy (fst) @? totals
        let res = totals.[ix] |> snd
        let mr = totals.[ix] |> fst
        let z = 5
        ix

[<EntryPoint>]
let main argv = 
    
    let rnd = Random()
    let get = getResult 10
    let list = [for n in 1000 .. 1000 do
                    let get = getResult n
                    for i in 0 .. 100 do yield get()] |> List.map (int)
    printfn "%s" (list |> String.join ", ")
    let avg = list |> List.averageBy (float)
    printfn "%f" avg
    printfn "Variance: %f" (list |> Math.variance)
    printfn "%A" argv
    0 // return an integer exit code
