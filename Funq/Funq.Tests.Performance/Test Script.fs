module Funq.Tests.Performance.Scripts
open Funq.Tests.Performance
open Funq.Tests
do()
open Funq.FSharp.Implementation
open Funq.Tests.Performance.Test
open System.Reflection
open System.Runtime.CompilerServices
open Funq.FSharp


type BaseArgs 
    (Simple_Iterations : int, 
     Target_Size : int, 
     DataSource_Size : int, 
     Full_Iterations : int, 
     DataSource_Iterations : int
     ) = 
     interface ICloneableObject
     member val Simple_Iterations = Simple_Iterations with get,set
     member val Target_Size = Target_Size with get,set
     member val DataSource_Size = DataSource_Size with get,set
     member val Full_Iterations = Full_Iterations with get, set
     member val DataSource_Iterations = DataSource_Iterations with get, set
     
type 
    AdvancedArgs<'t>(
                    Simple_Iterations : int, 
                    Target_Size : int, 
                    DataSource_Size : int, 
                    Full_Iterations : int, 
                    DataSource_Iterations : int,
                    Generator1 : DataGenerator<'t>,
                    Generator2 : DataGenerator<'t>,
                    DropRatio : float
                    ) = 
     inherit BaseArgs(Simple_Iterations, Target_Size, DataSource_Size, Full_Iterations, DataSource_Iterations)
     member val Generator1 = Generator1 with get, set
     member val Generator2 = Generator2 with get, set
     member val DropRatio = DropRatio with get, set



//This section contains the lists of tests each collection is assigned.
//Note that this process only constructs a list of unit → unit functions that make up the actual test/target pairs, but it doesn't run them.
//Each collection must be assigned to tests by hand, because this is the only way to allow the compiler to statically infer and generate everything.
//Also, not all collections support all operations.
let sequential(args : BaseArgs) =
    let iters =  args.Simple_Iterations
    let src = Seqs.Numbers.length(1, 4)
    let src2 = src
    let initial = args.Target_Size
    let dSize = args.DataSource_Size
    let dataSourceInit = (dSize,src2)
    let targetInit = (initial,src)
    let data = Data.Basic.Array dataSourceInit
    
    let fullIters = args.Full_Iterations
    let bulkIters = args.DataSource_Iterations
    let builder = Builder.blank
    let simple_iters = [iters]
    let data_iters = [bulkIters,data]
    //List.cross_apply1 is used in case we later want to extend the argument list.
    let builder = builder.AddTarget (Data.Funq.List targetInit)
    // [[Test list for Funq.FunqList]]
    let builder = 
        [AddFirst;AddLast;DropLast;DropFirst;InsertRandom;RemoveRandom; SetRandom; Iter; GetRandom] 
         |> List.apply1 iters
         |> builder.AddTests

    let builder = 
        [AddLastRange; AddFirstRange; InsertRangeRandom] 
        |> List.cross_apply1 data_iters 
        |> builder.AddTests

    let builder = 
        [AddLastRange; AddFirstRange; InsertRangeRandom] 
        |> List.cross_apply1 [bulkIters,Data.Funq.List dataSourceInit] 
        |> List.chain_iter (fun test -> test.Name <- test.Name + " (concat operation)")
        |> builder.AddTests

    let builder = [Take; Skip] |> List.cross_apply1 [bulkIters] |> builder.AddTests

    let builder = 
        [IterDirect] 
        |> List.cross_apply1 [1]
        |> builder.AddTests

    // [[end list]]

    // [[test list for Funq.FunqArray]]
    let builder = builder.Next().AddTarget (Data.Funq.Vector targetInit)
    //note that after calling Next(), a builder with an unbound generic type 'a is returned (the parameter changes)
    //calling AddTarget on a collection of the specific type allows type inference to be resolved.
    let builder = 
         [AddLast; DropLast; Iter; GetRandom; SetRandom]
         |> List.cross_apply1 simple_iters
         |> builder.AddTests

    let builder = 
        [AddLastRange; AddFirstRange;InsertRangeRandom] 
        |> List.cross_apply1 data_iters
        |> builder.AddTests

    let builder = 
        [AddLastRange;AddFirstRange;InsertRangeRandom]
        |> List.cross_apply1 [bulkIters,Data.Funq.Vector dataSourceInit]
        |> List.chain_iter (fun test -> test.Name <- test.Name + " (concat operation)")
        |> builder.AddTests

    let builder = 
        [Take; Skip] 
        |> List.cross_apply1 [bulkIters]
        |> builder.AddTests

    let builder = 
        [IterDirect] 
        |> List.cross_apply1 [fullIters]
        |> builder.AddTests

    // [[end list]]
    // [[test list for System.ImmutableList]]
    let builder = builder.Next().AddTarget (Data.Sys.List targetInit)
    let builder = 
        [AddFirst; AddLast; Iter; DropFirst; 
         DropLast; InsertRandom; RemoveRandom; 
         SetRandom; GetRandom]
         |> List.cross_apply1 simple_iters
         |> builder.AddTests    
    let builder = 
        [AddLastRange; AddFirstRange; InsertRangeRandom]
        |> List.cross_apply1 [bulkIters,Data.Sys.List dataSourceInit]
        |> List.chain_iter (fun test -> test.Name <- test.Name + " (concat operation)")
        |> builder.AddTests
    let builder = 
        [AddLastRange; AddFirstRange; InsertRangeRandom]
        |> List.cross_apply1 data_iters
        |> builder.AddTests
    let builder = 
        [Take; Skip]
        |> List.cross_apply1 [bulkIters]
        |> builder.AddTests
    let builder = 
        [IterDirect] |> List.cross_apply1 [fullIters]
        |> builder.AddTests
    //[[end list]]
    //[[test list for FSharpx.Deque]]
    let builder = builder.Next().AddTarget (Data.FSharpx.Deque targetInit)
    let builder = [AddFirst; AddLast; Iter; DropLast; DropFirst] |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = [AddLastRange; AddFirstRange] |> List.cross_apply1 data_iters |> builder.AddTests
    let builder = 
        [AddLastRange; AddFirstRange] 
        |> List.cross_apply1 [bulkIters, Data.FSharpx.Deque dataSourceInit] 
        |> List.chain_iter (fun test -> test.Name <- test.Name + " (concat operation)") 
        |> builder.AddTests
    let builder = [IterDirect] |> List.cross_apply1 [fullIters] |> builder.AddTests
    //[[end list]]
    //[[test list for FSharpx.Vector]]
    let builder = builder.Next().AddTarget (Data.FSharpx.Vector targetInit)
    let builder = 
        [AddLast; DropLast;Iter; GetRandom; SetRandom] 
        |> List.cross_apply1 simple_iters |> builder.AddTests
    let builder = 
         [AddLastRange] |> List.cross_apply1 data_iters |> builder.AddTests
    let builder = 
        [AddLastRange; AddFirstRange] 
        |> List.cross_apply1 [bulkIters, Data.FSharpx.Vector dataSourceInit] 
        |> List.chain_iter (fun test -> test.Name <- test.Name + " (concat operation)")
        |> builder.AddTests
    let builder = 
        [IterDirect] |> List.cross_apply1 [fullIters] |> builder.AddTests

    builder.Next().Bound
let mapLike(args : AdvancedArgs<_>) =
    let initial = args.Target_Size
    let iters = args.Simple_Iterations
    let src1 = args.Generator1
    let src2 = args.Generator2
    let dSize = args.DataSource_Size
    let full_iters = args.Full_Iterations
    let dataSourceInit = (dSize, src2)
    let targetInit = (initial,src1)
    let data = Data.Basic.Array dataSourceInit
    let dataKvp = src2.Map (fun x -> System.Collections.Generic.KeyValuePair(x, x)) |> fun x -> Data.Basic.Array(dSize, x)
    let bulkIters = args.DataSource_Iterations
    let dropRatio = args.DropRatio
    let simple_args = [iters]
    let data_args = [bulkIters, data]
    let inline standard_tests() =
        let a = [GetKeyRandom; Iter; DropKey] |> List.cross_apply1 simple_args
        let b = [AddKeyRandom] |> List.cross_apply1 data_args
        let c = [IterDirect] |> List.cross_apply1 [full_iters]
        let d = [AddKeys]   |> List.cross_apply1 [bulkIters, dataKvp]
        let e = [DropKeys] |> List.cross_apply1 [bulkIters, dropRatio]
        a @ b @ c @ d @ e
    
    let builder = 
        Builder.blank
        |> Builder.addTarget (Data.Funq.Map targetInit) 
        |> Builder.addTests (standard_tests())
        |> Builder.next
    let builder = 
        builder |> Builder.addTarget (Data.Sys.Dict targetInit)
        |> Builder.addTests (standard_tests())
        |> Builder.next
    let builder = 
        builder |> Builder.addTarget (Data.FSharp.Map targetInit)
        |> Builder.addTests (standard_tests())
        |> Builder.next
    let builder =
        builder |> Builder.addTarget (Data.Sys.SortedDict targetInit)
        |> Builder.addTests (standard_tests())
        |> Builder.next
    let builder = 
        builder |> Builder.addTarget (Data.Funq.OrderedMap targetInit)
        |> Builder.addTests (standard_tests())
        |> Builder.next
    

    builder |> Builder.finish

let setLike(args : AdvancedArgs<_>) = //(set1, set2, iters, src1, src2) = 
    let bulkIters = args.DataSource_Iterations
    let iters = args.Simple_Iterations
    let full_iters = args.Full_Iterations
    let src1 = args.Generator1
    let src2 = args.Generator2
    let set1 = args.Target_Size
    let set2 = args.DataSource_Size
    let dataSourceInit = (set2,src2)
    let targetInit = (set1,src1)
    let dropRatio = args.DropRatio
    let arr = Data.Basic.Array targetInit
    let inline standard_tests makeSet= 
        let a = 
            [UnionWithSet; IntersectWithSet; 
             ExceptWithSet; SymDifferenceWithSet;
             ProperSubset; ProperSuperset; SetEquals]
             |> List.cross_apply1 [bulkIters, makeSet dataSourceInit]
        let b = 
            [AddSetItem] |> List.cross_apply1 [bulkIters, arr] 
        let c = [Contains; DropSetItem; Iter] |> List.cross_apply1 [iters]
        let d = [AddSetItems] |> List.cross_apply1 [bulkIters, arr]
        let e = [DropSetItems] |> List.cross_apply1 [bulkIters, dropRatio]
        let f = [IterDirect] |> List.cross_apply1 [full_iters]

        a @ b @ c @ d @ e @ f
    let builder = 
        Builder.blank 
        |> Builder.addTarget (Data.Sys.Set targetInit)
        |> Builder.addTests (standard_tests <| Data.Sys.Set) 
        |> Builder.next
        |> Builder.addTarget (Data.FSharp.Set targetInit)
        |> Builder.addTests (standard_tests <| Data.FSharp.Set)
        |> Builder.next
        |> Builder.addTarget (Data.Funq.Set targetInit)
        |> Builder.addTests (standard_tests <| Data.Funq.Set) |> Builder.next
        |> Builder.addTarget (Data.Funq.OrderedSet targetInit)
        |> Builder.addTests (standard_tests <| Data.Funq.OrderedSet) |> Builder.next
        |> Builder.addTarget (Data.Sys.SortedSet targetInit)
        |> Builder.addTests (standard_tests <| Data.Sys.SortedSet) |> Builder.next
    builder |> Builder.finish


    