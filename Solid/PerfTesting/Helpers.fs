[<AutoOpen>]
module Benchmarks.Helpers
module NumericLiteralZ = 
    let mutable ExpMultiplier = 0.3
    let mutable Base = 10.
    let inline FromZero() = 0
    let inline FromOne() = 1
    let inline FromInt32 n =
        let exp = n * ExpMultiplier
        let b = Base
        let num = exp ** b
        int num

module Seq =
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = seq {for item1 in seq1 do for item2 in seq2 -> item1,item2}
    let mapPairs f = Seq.map (fun (a,b) -> f a b)
    
module List = 
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = [for item1 in seq1 do for item2 in seq2 -> item1,item2]
    let mapPairs f = List.map (fun (a,b) -> f a b)

let inline (?|) (o : _ option) (v : _) = if o.IsNone then v else o.Value

type Series private() = 
    static member Arith(count,?start, ?step) =
        let start = defaultArg start 0
        let step = defaultArg step 1
        seq {for i in 0 .. count -> start + i * step}
    static member inline Geom (start,``base``, exp_range)  =
        seq {
            for i in exp_range do
                let cur_exp = float i
                let cur_mult = ``base`` ** cur_exp
                let res = start * cur_mult
                yield res
            }

    static member inline GeomInt(exp_range, ?start, ?bse) =
        let start = if start.IsSome then float(start.Value) else 1.
        let bse  = if bse.IsSome then float(bse.Value) else 10.
        Series.Geom(start,bse, exp_range) |> Seq.map int