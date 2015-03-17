module Funq.Benchmarking.Scripts
open Funq.Benchmarking

let sequential(initial,iters) = 
    let src = Seqs.Numbers.length(1, 30)
    let dSize = iters / 10
    let data = src |> Data.Basic.Array (dSize)
    let sysList = Data.Sys.List dSize src
    let bulkIters = 9
    Builder.blank 
    |> Builder.addTarget(Data.Funq.List initial src)
    |> Builder.addTestsFunc [Test.AddFirst; Test.AddLast; 
                             Test.DropLast; Test.DropFirst; Test.InsertRandom;
                             Test.RemoveRandom; Test.SetRandom; Test.Iter;
                             Test.GetRandom] [iters]
    |> Builder.addTestsFunc [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] [(bulkIters, data)]
    |> Builder.addTestsName [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] [(bulkIters,Data.Funq.List dSize src)] "(data source is identical to target)"
    |> Builder.addTestsFunc [Test.Take; Test.Skip] [bulkIters]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.next
    |> Builder.addTarget(Data.Funq.Vector initial src)
    |> Builder.addTestsFunc [Test.AddLast; Test.DropLast; Test.Iter; Test.GetRandom; Test.SetRandom] [iters]
    |> Builder.addTestsFunc [Test.AddLastRange; Test.AddFirstRange] [(bulkIters,data)]
    |> Builder.addTestsName [Test.AddLastRange; Test.AddFirstRange] [(bulkIters,Data.Funq.Vector dSize src)] "(data source is identical to target)"
    |> Builder.addTestsFunc [Test.Take] [bulkIters]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.next
    |> Builder.addTarget(Data.Sys.List initial src)
    |> Builder.addTestsFunc [Test.AddFirst; Test.AddLast; Test.Iter; Test.DropFirst; Test.DropLast;
                             Test.InsertRandom; Test.RemoveRandom; Test.SetRandom; Test.GetRandom] [iters]
    |> Builder.addTestsFunc [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] [(bulkIters,data)]
    |> Builder.addTestsFunc [Test.Take; Test.Skip] [bulkIters]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.addTestsName [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] [(bulkIters,Data.Sys.List dSize src)] "(data source is identical to target)"
    |> Builder.next
    |> Builder.addTarget(Data.FSharpx.Deque initial src)
    |> Builder.addTestsFunc [Test.AddFirst; Test.AddLast; Test.Iter; Test.DropLast; Test.DropFirst] [iters]
    |> Builder.addTestsFunc [Test.AddLastRange; Test.AddFirstRange] [(bulkIters,data)]
    |> Builder.addTestsName [Test.AddLastRange; Test.AddFirstRange] [(bulkIters,Data.FSharpx.Deque dSize src)] "(data source is identical to target)"
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.next
    |> Builder.addTarget(Data.FSharpx.Vector initial src)
    |> Builder.addTestsFunc [Test.AddLast; Test.DropLast;Test.Iter; Test.GetRandom; Test.SetRandom] [iters]
    |> Builder.addTestsName [Test.AddLastRange; Test.AddFirstRange] [(bulkIters,Data.FSharpx.Vector dSize src)] "(data source is identical to target)"
    |> Builder.addTestsFunc [Test.AddLastRange] [(bulkIters,data)]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.finish   

let mapLike(initial, iters, src) = 
    let data = src |> Data.Basic.Array (iters / 10)
    let bulkIters = 9
    Builder.blank
    |> Builder.addTarget(Data.Funq.Map initial src)
    |> Builder.addTestsFunc [Test.AddKeyRandom] [(bulkIters, data)]
    |> Builder.addTestsFunc [Test.GetKeyRandom] [iters]
    |> Builder.addTestsFunc [Test.Iter] [iters]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.next
    |> Builder.addTarget(Data.Sys.Dict initial src)
    |> Builder.addTestsFunc [Test.AddKeyRandom] [(bulkIters, data)]
    |> Builder.addTestsFunc [Test.GetKeyRandom] [iters]
    |> Builder.addTestsFunc [Test.Iter] [iters]
    |> Builder.addTestsFunc [Test.IterDirect] [0]
    |> Builder.next
    |> Builder.finish

let setLike(set1, set2, iters, src1, src2) = 
    let bulkIters = 10
    let arr = src2 |> Data.Basic.Array set2
    Builder.blank
    |> Builder.addTarget(Data.Sys.Set set1 src1)
    |> Builder.addTestsFunc [Test.UnionWithSet; Test.IntersectWithSet; Test.ExceptWithSet; Test.SymDifferenceWithSet
                             Test.ProperSubset; Test.ProperSuperset; Test.SetEquals] [(iters, Data.Sys.Set set2 src2)]
    |> Builder.addTestsFunc [Test.AddSetItem; Test.DropSetItem] [(iters, arr)] 
    |> Builder.addTestsFunc [Test.Contains] [iters]
    |> Builder.next
    |> Builder.addTarget(Data.Funq.Set set1 src1)
    |> Builder.addTestsFunc [Test.UnionWithSet; Test.IntersectWithSet; Test.ExceptWithSet; Test.SymDifferenceWithSet
                             Test.ProperSubset; Test.ProperSuperset; Test.SetEquals] [(iters, Data.Funq.Set set2 src2)]
    |> Builder.addTestsFunc [Test.AddSetItem; Test.DropSetItem] [(iters, arr)] 
    |> Builder.addTestsFunc [Test.Contains] [iters]
    |> Builder.next
    |> Builder.addTarget(Data.Sys.SortedSet set1 src1)
    |> Builder.addTestsFunc [Test.UnionWithSet; Test.IntersectWithSet; Test.ExceptWithSet; Test.SymDifferenceWithSet
                             Test.ProperSubset; Test.ProperSuperset; Test.SetEquals] [(iters, Data.Sys.Set set2 src2)]
    |> Builder.addTestsFunc [Test.AddSetItem; Test.DropSetItem] [(iters, arr)] 
    |> Builder.addTestsFunc [Test.Contains] [iters]
    |> Builder.next
    |> Builder.addTarget(Data.Funq.OrderedSet set1 src1)
    |> Builder.addTestsFunc [Test.UnionWithSet; Test.IntersectWithSet; Test.ExceptWithSet; Test.SymDifferenceWithSet
                             Test.ProperSubset; Test.ProperSuperset; Test.SetEquals] [(iters, Data.Funq.OrderedSet set2 src2)]
    |> Builder.addTestsFunc [Test.AddSetItem; Test.DropSetItem] [(iters, arr)] 
    |> Builder.addTestsFunc [Test.Contains] [iters]
    |> Builder.finish

