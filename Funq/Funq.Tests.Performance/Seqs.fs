namespace Funq.Tests.Performance
open Funq.FSharp.Implementation
module Seqs = 
    module Strings = 
        let letters(min,max)= 
            let s = seq {while true do yield Rand.String.word_like(min,max)} |> Seq.cache
            DataGenerator("Strings of Letters by Length",s)
                <+ Meta("Range", (min,max))
    module Numbers = 
        let length(min,max) = 
            let s = seq {while true do yield Rand.Int.of_length(min,max)} |> Seq.cache
            DataGenerator("Integers by Length", s) <+ Meta("Range", (min,max))
        let float(min,max) = 
            let s = seq {while true do yield Rand.Float.in_range(min,max)} |> Seq.cache
            DataGenerator("Floating Point by Length", s) <+ Meta("Range", (min,max))
