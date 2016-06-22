module Imms.Tests.Performance.Scripts
open Imms.Tests.Performance
open ExtraFunctional
open Imms.Tests
do()
open Imms.FSharp.Implementation
open System.Reflection
open System.Runtime.CompilerServices
open Imms.FSharp
open Imms.Tests.Performance.TestCode.Sequential
open Imms.Tests.Performance.TestCode.Set
open Imms.Tests.Performance.TestCode.Map
open Imms.Tests.Performance.TestCode.General

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
                    RemoveRatio : float
                    ) = 
     inherit BaseArgs(Simple_Iterations, Target_Size, DataSource_Size, Full_Iterations, DataSource_Iterations)
     member val Generator1 = Generator1 with get, set
     member val Generator2 = Generator2 with get, set
     member val RemoveRatio = RemoveRatio with get, set


let ( <-| ) (fs : (_ -> _) list) (arg : _) = fs |> List.apply1 arg
let ( <-|| ) (fs : (_ -> _ ) list) (arg : _) = fs |> List.apply1 arg
//This section contains the lists of tests each collection is assigned.
//Note that this process only constructs a list of unit → unit functions that make up the actual test/target pairs, but it doesn't run them.
//Each collection must be assigned to tests by hand, because this is the only way to allow the compiler to statically infer and generate everything.
//Also, not all collections support all operations.
let sequential(args : BaseArgs) =
    let iters =  args.Simple_Iterations
    let src = Seqs.Numbers.length(1, 4)
    let dataSourceInit = (args.DataSource_Size,src)
    let targetInit = (args.Target_Size,src)
    let data = Data.Basic.Array dataSourceInit
    let bulkIters = args.DataSource_Iterations
    let fullIters = args.Full_Iterations
    let data_iters = (args.DataSource_Iterations,data)
    //List.cross_apply1 is used in case we later want to extend the argument list.
    let builder = Builder.blank
    // [[Test list for Imms.ImmList]]

    let builder = builder.ForTarget(Data.Imms.List targetInit)

    //it is probably best to read these as some kind of DSL for specifying which data structure gets
    //which test with how many iterations. How it's pulled off is actually extremely complicated.
    //in the end, what you get is a set of ErasedTest objects, each of which specifies a specific test
    //for a specific data structure, with all information encapsulated and invisible.

    //if you make a mistake (e.g. assign a collection a test it doesn't support) there will be a compilation error.


    let subseqParams = 
        let maxedSize = min args.Target_Size args.DataSource_Size
        let dif = args.DataSource_Size - maxedSize
        if dif = 0 then bulkIters, maxedSize else        
            let itersFactor = args.DataSource_Size / dif
            let newIters = bulkIters * itersFactor
            int newIters, maxedSize
    builder.AddTests(
        [
            [AddFirst;AddLast;RemoveLast;RemoveFirst;InsertRandom;RemoveRandom; SetRandom; Iter; GetRandom; IterDirectN; First; Last] <-| iters;
            [AddLastRange; AddFirstRange; InsertRangeRandom] <-| data_iters;
            [AddLastRange] <-|(bulkIters, Data.Imms.List dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "Concat"); 
            [InsertRangeRandom] <-|(bulkIters, Data.Imms.List dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "InsertConcat"); 
            [Take; Skip] <-| subseqParams
            [IterDirect] <-| 1
        ] |> List.collect id).Done

    let builder = builder.ForTarget(Data.Imms.Vector targetInit)

    builder.AddTests(
        [
            [AddLast;RemoveLast; SetRandom; Iter; GetRandom; IterDirectN; First; Last] <-| iters;
            [AddLastRange; AddFirstRange; InsertRangeRandom] <-| data_iters;
            [AddLastRange] <-|(bulkIters, Data.Imms.Vector dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "Concat");
            [InsertRangeRandom] <-|(bulkIters, Data.Imms.Vector dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "InsertConcat"); 
            [Take; Skip] <-| subseqParams
            [IterDirect] <-| 1
        ] |> List.collect id).Done

    // [[end list]]
    // [[test list for System.ImmutableList]]
    let builder = builder.ForTarget(Data.Sys.List targetInit)

    builder.AddTests(
        [
            [AddFirst;AddLast;RemoveLast;RemoveFirst;InsertRandom;RemoveRandom; SetRandom; Iter; GetRandom; IterDirectN; First; Last] <-| iters;
            [AddLastRange; AddFirstRange; InsertRangeRandom] <-| data_iters;
            [AddLastRange] <-|(bulkIters, Data.Sys.List dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "Concat");
            [InsertRangeRandom] <-|(bulkIters, Data.Sys.List dataSourceInit) |> List.chain_iter (fun x -> x.Name <- "InsertConcat"); 
            [Take; Skip] <-| subseqParams
            [IterDirect] <-| 1
        ] |> List.collect id).Done

    //[[end list]]
    //[[test list for FSharpx.Deque]]

    //[[end list]]
    //[[test list for FSharpx.Vector]]
    let builder = builder.ForTarget(Data.FSharpx.Vector targetInit)

    builder.AddTests(
        [
            [AddLast;RemoveLast; SetRandom; Iter; GetRandom; IterDirectN; First; Last] <-| iters;
            [AddLastRange] <-| data_iters;
            [IterDirect] <-| 1
        ] |> List.collect id).Done

    builder.Bound |> List.ofSeq

let mapLike(args : AdvancedArgs<_>) =
    let iters = args.Simple_Iterations
    let full_iters = args.Full_Iterations
    let dataSourceInit = (args.DataSource_Size, args.Generator2)
    let targetInit = (args.Target_Size,args.Generator1)
    let data = Data.Basic.Array dataSourceInit
    let dataKvp = 
        args.Generator2.Map (fun x -> System.Collections.Generic.KeyValuePair(x, x)) |> fun x -> Data.Basic.Array(args.DataSource_Size, x)
    let bulkIters = args.DataSource_Iterations
    let removeRatio = args.RemoveRatio
    let inline standard_tests() =
        [
            [GetKeyRandom; Iter; RemoveKey; IterDirectN] <-| iters;
            [AddKeyRandom] <-| (bulkIters, data);
            [IterDirect] <-| full_iters;
            [AddKeys] <-| (bulkIters, dataKvp);
            [RemoveKeys] <-| (bulkIters, removeRatio)
        ] |> List.collect id
    
    let builder = Builder.blank
    builder
        .ForTarget(Data.Imms.Map targetInit)
        .AddTests(standard_tests())
        
        .ForTarget(Data.Sys.Dict targetInit)
        .AddTests(standard_tests())
        
        .ForTarget(Data.FSharp.Map targetInit)
        .AddTests(standard_tests())

        .ForTarget(Data.Sys.SortedDict targetInit)
        .AddTests(standard_tests())

        .ForTarget(Data.Imms.OrderedMap targetInit)
        .AddTests(standard_tests())
        .Done

    builder.Bound |> List.ofSeq

let setLike(args : AdvancedArgs<_>) = //(set1, set2, iters, src1, src2) = 
    let bulkIters = args.DataSource_Iterations
    let iters = args.Simple_Iterations
    let full_iters = args.Full_Iterations
    let dataSourceInit = (args.DataSource_Size, args.Generator2)
    let targetInit = (args.Target_Size,args.Generator1)
    let removeRatio = args.RemoveRatio
    let arr = Data.Basic.Array dataSourceInit
    let bulkInit = bulkIters, arr
    let inline elementTests() = 
        [
            [AddSetItem] <-| (bulkIters, arr);
            [RemoveSetItem; Contains; Iter; IterDirectN] <-| iters;
        ] |> List.collect id


    let inline dataTests f tag = 
        [
            [UnionWithSet; IntersectWithSet; ExceptWithSet; SymDifferenceWithSet] <-| (bulkIters, f dataSourceInit)
        ] |> List.collect id |> List.chain_iter (fun test -> test.Name <- tag + "_" + test.Name)

    let builder = 
        Builder.blank
         .ForTarget(Data.Sys.Set targetInit)
         .AddTests(elementTests())
         .AddTests(dataTests Data.Sys.Set "Set")
         .AddTests(dataTests Data.Basic.Array "Array")

         .ForTarget(Data.Sys.SortedSet targetInit)
         .AddTests(elementTests())
         .AddTests(dataTests Data.Sys.SortedSet "Set")
         .AddTests(dataTests Data.Basic.Array "Array")
        
         .ForTarget(Data.FSharp.Set targetInit)
         .AddTests(elementTests())
         .AddTests(dataTests Data.FSharp.Set "Set")
         .AddTests(dataTests Data.Basic.Array "Array")

         .ForTarget(Data.Imms.Set targetInit)
         .AddTests(elementTests())
         .AddTests(dataTests Data.Imms.Set "Set")
         .AddTests(dataTests Data.Basic.Array "Array")

         .ForTarget(Data.Imms.OrderedSet targetInit)
         .AddTests(elementTests())
         .AddTests(dataTests Data.Imms.OrderedSet "Set")
         .AddTests(dataTests Data.Basic.Array "Array")

        
    builder.Done
    builder.Bound |> List.ofSeq 

    