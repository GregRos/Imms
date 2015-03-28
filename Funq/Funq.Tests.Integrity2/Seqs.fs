namespace Funq.Tests.Integrity
open Funq.FSharp.Implementation
open Funq.FSharp
module Seqs = 
    let letters(min,max) = 
        Seq.initInfinite (fun _ -> Rand.String.word_like(min,max)) |> Seq.cache
    let distinctLetters(min,max) = letters(min,max) |> Seq.lazyDistinct
    let ints(min,max) = 
        Seq.initInfinite (fun _ -> Rand.Int.of_length(min,max))
    let distinctInts(min,max) = 
        ints(min,max) |> Seq.lazyDistinct
