module Funq.Tests.Performance.Scripts
open Funq.Tests.Performance
open Funq.Tests
do()
open Funq.FSharp.Implementation
//This section contains the lists of tests each collection is assigned.
//Note that this process only constructs a list of unit → unit functions that make up the actual test/target pairs, but it doesn't run them.
//Each collection must be assigned to tests by hand, because this is the only way to allow the compiler to statically infer and generate everything.
//Also, not all collections support all operations.
let sequential(initial,iters) = 
    let src = Seqs.Numbers.length(1, 30)
    let dSize = iters / 10
    let data = src |> Data.Basic.Array (dSize)
    let sysList = Data.Sys.List dSize src
    let bulkIters = 10
    let builder = Builder.blank
    let simple_iters = [iters]
    let data_iters = [bulkIters,data]
    //List.cross_apply1 is used in case we later want to extend the argument list.
    let builder = builder.AddTarget (Data.Funq.List initial src)
    // [[Test list for Funq.FunqList]]
    let builder = 
        [Test.AddFirst; Test.AddLast; Test.DropLast; Test.DropFirst; Test.InsertRandom; 
         Test.RemoveRandom; Test.SetRandom; Test.Iter; Test.GetRandom] 
         |> List.cross_apply1 simple_iters
         |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] 
        |> List.cross_apply1 data_iters 
        |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange; Test.InsertRangeRandom] 
        |> List.cross_apply1 [bulkIters,Data.Funq.List dSize src] 
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)")
        |> builder.AddTests
    let builder = [Test.Take; Test.Skip] |> List.cross_apply1 [bulkIters] |> builder.AddTests
    let builder = 
        [Test.IterDirect] 
        |> List.cross_apply1 [1]
        |> builder.AddTests
    // [[end list]]
    // [[test list for Funq.FunqArray]]
    let builder = builder.Next().AddTarget (Data.Funq.Array initial src)
    //note that after calling Next(), a builder with an unbound generic type 'a is returned (the parameter changes)
    //calling AddTarget on a collection of the specific type allows type inference to be resolved.
    let builder = 
         [Test.AddLast; Test.DropLast; Test.Iter; Test.GetRandom; Test.SetRandom]
         |> List.cross_apply1 simple_iters
         |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 data_iters
        |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange]
        |> List.cross_apply1 [bulkIters,Data.Funq.Array dSize src]
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)")
        |> builder.AddTests
    let builder = 
        [Test.Take] 
        |> List.cross_apply1 [bulkIters]
        |> builder.AddTests
    let builder = 
        [Test.IterDirect] 
        |> List.cross_apply1 [1]
        |> builder.AddTests
    // [[end list]]
    // [[test list for System.ImmutableList]]
    let builder = builder.Next().AddTarget (Data.Sys.List initial src)
    let builder = 
        [Test.AddFirst; Test.AddLast; Test.Iter; Test.DropFirst; 
         Test.DropLast; Test.InsertRandom; Test.RemoveRandom; 
         Test.SetRandom; Test.GetRandom]
         |> List.cross_apply1 simple_iters
         |> builder.AddTests    
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange]
        |> List.cross_apply1 [bulkIters,Data.Sys.List dSize src]
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)")
        |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange]
        |> List.cross_apply1 data_iters
        |> builder.AddTests
    let builder = 
        [Test.Take; Test.Skip]
        |> List.cross_apply1 [bulkIters]
        |> builder.AddTests
    let builder = 
        [Test.IterDirect] |> List.cross_apply1 [1]
        |> builder.AddTests
    //[[end list]]
    //[[test list for FSharpx.Deque]]
    let builder = builder.Next().AddTarget (Data.FSharpx.Deque initial src)
    let builder = [Test.AddFirst; Test.AddLast; Test.Iter; Test.DropLast; Test.DropFirst] |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = [Test.AddLastRange; Test.AddFirstRange] |> List.cross_apply1 data_iters |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 [bulkIters, Data.FSharpx.Deque dSize src] 
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)") 
        |> builder.AddTests
    let builder = [Test.IterDirect] |> List.cross_apply1 [1] |> builder.AddTests
    //[[end list]]
    //[[test list for FSharpx.Vector]]
    let builder = builder.Next().AddTarget (Data.FSharpx.Vector initial src)
    let builder = 
        [Test.AddLast; Test.DropLast;Test.Iter; Test.GetRandom; Test.SetRandom] 
        |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = 
         [Test.AddLastRange] |> List.cross_apply1 data_iters |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 [bulkIters, Data.FSharpx.Vector dSize src] 
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)")
        |> builder.AddTests
    let builder = 
        [Test.IterDirect] |> List.cross_apply1 [1] |> builder.AddTests

    //[[test list of Sasa.FingerTree]]
    (* //Sasa.FingerTree is broken and unusable
    let builder = builder.Next().AddTarget (Data.Sasa.FingerTree initial src)
    let builder = 
        [Test.AddLast; Test.AddFirst; Test.Iter]
        |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 data_iters |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 [bulkIters, Data.Sasa.FingerTree dSize src] 
        |> List.chain_iter (fun test -> test?Name <- test?Name + " (concat operation)")
        |> builder.AddTests
    //[[Test list for Sasa.Vector]]
    let builder = builder.Next().AddTarget (Data.Sasa.Vector initial src)
    let builder = 
        [Test.AddLast; Test.GetRandom; Test.SetRandom; Test.Iter]
        |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = 
        [Test.AddLastRange; Test.AddFirstRange] 
        |> List.cross_apply1 data_iters |> builder.AddTests
    *)
    builder.Next().Bound
let mapLike(initial, iters, src) = 
    let data = src |> Data.Basic.Array (iters / 10)
    let bulkIters = 9
    
    let simple_args = [iters]
    let data_args = [bulkIters, data]
    let inline standard_tests() =
        let a = [Test.GetKeyRandom; Test.Iter] |> List.cross_apply1 simple_args
        let b = [Test.AddKeyRandom] |> List.cross_apply1 data_args
        let c = [Test.IterDirect] |> List.cross_apply1 [1]
        a @ b @ c
    
    let builder = 
        Builder.blank
        |> Builder.addTarget (Data.Funq.Map initial src) 
        |> Builder.addTests (standard_tests())
        |> Builder.next
    let builder = 
        builder |> Builder.addTarget (Data.Sys.Dict initial src)
        |> Builder.addTests (standard_tests())
        |> Builder.next

    builder |> Builder.finish

let setLike(set1, set2, iters, src1, src2) = 
    let bulkIters = 10
    let arr = src2 |> Data.Basic.Array set2
    let builder = Builder.blank |> Builder.addTarget (Data.Sys.Set set1 src1)

    let inline standard_tests makeSet= 
        let a = 
            [Test.UnionWithSet; Test.IntersectWithSet; 
             Test.ExceptWithSet; Test.SymDifferenceWithSet; 
             Test.ProperSubset; Test.ProperSuperset; Test.SetEquals]
             |> List.cross_apply1 [iters, makeSet set2 src2]
        let b = 
            [Test.AddSetItem; Test.DropSetItem] |> List.cross_apply1 [iters, arr] 
        let c = [Test.Contains] |> List.cross_apply1 [iters]
        a @ b @ c
    let builder = 
        Builder.blank 
        |> Builder.addTarget (Data.Sys.Set set1 src1)
        |> Builder.addTests (standard_tests <| Data.Sys.Set) |> Builder.next
        |> Builder.addTarget (Data.Funq.Set set1 src1)
        |> Builder.addTests (standard_tests <| Data.Funq.Set) |> Builder.next
        |> Builder.addTarget (Data.Funq.OrderedSet set1 src1)
        |> Builder.addTests (standard_tests <| Data.Funq.OrderedSet) |> Builder.next
        |> Builder.addTarget (Data.Sys.SortedSet set1 src1)
        |> Builder.addTests (standard_tests <| Data.Sys.SortedSet) |> Builder.next
    builder |> Builder.finish


    