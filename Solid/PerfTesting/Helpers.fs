///A module that contains helper functions and methods.
[<AutoOpen>]
module Benchmarks.Helpers
///A module with some extra sequence processing functions.
module Seq =
    ///Cross/cartesian product over sequences.
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = seq {for item1 in seq1 do for item2 in seq2 -> item1,item2}
    ///Maps a function taking 2 parameters over a sequence of tuples.
    let mapPairs f = Seq.map (fun (a,b) -> f a b)
    
module List = 
    ///Cross/cartesian product over lists.
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = [for item1 in seq1 do for item2 in seq2 -> item1,item2]
    ///Maps a function taking 2 parameters over a list of tuples.
    let mapPairs f = List.map (fun (a,b) -> f a b)

///If some, returns the value. If none, returns the value to the right of the bar.
let inline (?|) (o : _ option) (v : _) = if o.IsNone then v else o.Value
///A class with methods for generating number sequences of various kinds.
type Series private() = 
    ///Generates an arithmetic sequence.
    static member Arith(count,?start, ?step) =
        let start = defaultArg start 0
        let step = defaultArg step 1
        seq {for i in 0 .. count -> start + i * step}
    ///Generates a geometric sequence.
    static member inline Geom (start,``base``, exp_range)  =
        seq {
            for i in exp_range do
                let cur_exp = float i
                let cur_mult = ``base`` ** cur_exp
                let res = start * cur_mult
                yield res
            }
    ///Generates a geometric sequence, and converts the elements into integers.
    static member inline GeomInt(exp_range, ?start, ?bse) =
        let start = if start.IsSome then float(start.Value) else 1.
        let bse  = if bse.IsSome then float(bse.Value) else 10.
        Series.Geom(start,bse, exp_range) |> Seq.map int