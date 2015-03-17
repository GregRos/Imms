module Solid.Benchmarking.Test_Plans.Comparisons
open Solid.Benchmarking
let lowIters = 10
let initial = 0.5 * (10. ** 5.) |> int
let lowInitial = 100
let iters =0.5 * (10. ** 5.) |> int
let numbers = Seqs.Numbers.length(0, 30)
let numArray = Data.Basic.Array initial numbers
let numFlexList = Data.Solid.List initial numbers
let strings = Seqs.Strings.letters(30, 50)
    (*
let tests = 
    Builder.blank
    |> Builder.addTargets [Data.Solid.List initial numbers]
    |> Builder.addTests   [Test.AddFirst iters; Test.AddLast iters; 
                            Test.DropLast iters; Test.DropFirst iters; Test.InsertRandom iters;
                            Test.RemoveRandom iters; Test.SetRandom iters; Test.GetRandom iters; Test.Take iters]
    |> Builder.addTests   [Data.Basic.Array initial numbers |> Test.AddLastRange lowIters ]
    |> Builder.addTests   [Test.InsertRangeRandom lowIters numArray ]
    |> Builder.addTests   [numArray |> Test.InsertRangeRandom lowIters]
    |> Builder.addTests   [numArray |> Test.AddFirstRange lowIters]
    |> Builder.addTests   [Test.AddLastRange lowIters numFlexList]
    |> Builder.next
    |> Builder.addTargets [Data.Solid.Vector initial numbers]
    |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; Test.GetRandom iters; Test.SetRandom iters; Test.Take iters]
    |> Builder.addTests   [Test.AddLastRange lowIters numArray]
    |> Builder.next
    |> Builder.addTargets [Data.Sys.List initial numbers]
    |> Builder.addTests   [Test.AddFirst iters; Test.AddLast iters; Test.DropFirst iters; Test.DropLast iters;
                            Test.InsertRandom iters; Test.RemoveRandom iters; Test.SetRandom iters; Test.GetRandom iters]
    |> Builder.addTests   [Test.AddLastRange lowIters numArray ]
    |> Builder.addTests   [Test.InsertRangeRandom lowIters numArray]
    |> Builder.addTests   [Test.InsertRangeRandom lowIters numArray]
    |> Builder.next
    |> Builder.addTargets [Data.Sys.List 1000 numbers; Data.Sys.List 10000 numbers]
    |> Builder.addTests [Test.Take 1000]
    |> Builder.next
    |> Builder.addTargets [Data.Sys.Queue initial numbers]
    |> Builder.addTests   [Test.AddLast iters; Test.DropFirst iters]
    |> Builder.next
    |> Builder.addTargets [Data.Sys.Stack initial numbers]
    |> Builder.addTests   [Test.AddFirst iters; Test.AddFirst iters]
    |> Builder.next
    |> Builder.addTargets [Data.FSharpx.Vector initial numbers]
    |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; Test.GetRandom iters; Test.SetRandom iters]
    |> Builder.next
    |> Builder.addTargets [Data.FSharpx.Deque initial numbers]
    |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; 
                            Test.DropFirst iters; Test.AddFirst iters]
    |> Builder.next
    |> Builder.addTargets [Data.FSharpx.RanAccList initial numbers]
    |> Builder.addTests   [Test.AddLast iters; Test.DropLast iters; 
                            Test.GetRandom iters; Test.SetRandom iters]
    |> Builder.finish
    *)


