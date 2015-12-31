namespace ExtraFunctional
open System
[<AutoOpen>]
module RandomExt = 
    type Random with
        member x.ExpDouble max = 
            let scale = x.NextDouble() * 2. - 1.
            let log = log max
            exp (scale * log)
        member x.IntInterval(mn,mx,?maxLen) = 
            let st = x.Next(mn, mx + 1)
            let maxEn = if maxLen.IsNone then mx + 1 else min (mx + 1) (st + maxLen.Value)
            let en = x.Next(st, maxEn)
            st,en
        member x.IntByLength(min,max) =
            let min = pown 2 min
            let max = pown 2 max
            x.Next(min,max)
        member x.ElementOf(sq) = 
            let arr = sq |> Seq.asArray
            arr.[x.Next(0, arr.Length)]
        member x.String len  =
            fun (charPool : #seq<char>) ->
                let charPool = charPool |> Seq.asArray
                let str = String.initChars len (fun i -> x.ElementOf charPool)
                str
        member x.String (min,max) : (#seq<char> -> string) = 
            x.Next(min,max) |> x.String
        member x.Double(min,max) = 
            min + x.NextDouble() * (max - min)

