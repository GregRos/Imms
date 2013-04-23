namespace Benchmarks
#nowarn"66"
#nowarn"20"
module Test = 
    open Solid
    open System.Collections.Immutable
    open System
    open System.Linq
    let inline itersOf t = (^s : (member Iterations : int) t)
    let inline dataOf t = (^s : (member DataLoaded : 'seq) t) 
        
    //Here we use an object expression to instantiate a test parameterized with iterations.
    //Note that the test function itself is external.
    let inline NewIterTest name test iterations extra_params=
        { TestWithIterations.Name = name; Test = test; Iterations = iterations; Metadata = extra_params}:>ITest<_>


 
    let rnd = Random()
    let PercentileData = [|for i = 0. to 10. ** 6. do yield rnd.NextDouble()|]
    
    let inline Many (tests : #seq<'a -> #ITest<'s>>) pars= 
        pars |> List.cross tests |> List.mapPairs (fun test par -> test par)


    let inline AddFirst iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col
            col <- col |> Ops.addFirst 0
            for i = 0 to iters do col <- col |> Ops.addFirst 0 
            
        {
            TestWithIterations.Name = "AddFirst"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline AddLast iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col |> Ops.addLast 0
            for i = 0 to iters do col <- col |> Ops.addLast 0
            
        {
            TestWithIterations.Name = "AddLast"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>
    
    let inline AddLastRange(iters)(data : DataSource) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable col = col
            for i = 0 to iters do col <- col |> Ops.addLastRange data 
            
        {
            TestWithData.Name = "AddLastRange"
            Test = test
            DataLoaded = data.Generate
            Metadata = []
            Iterations = iters
            Data = data
        }:>ITest<_>
    let inline AddFirstRange(iters)(data : DataSource) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable col = col
            for i = 0 to iters do col <- col |> Ops.addLastRange data
            
        {
            TestWithData.Name = "AddFirstRange"
            Test = test
            DataLoaded = data.Generate
            Metadata = []
            Iterations = iters
            Data = data
        }:>ITest<_>
        
    let inline DropLast iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 0 to iters do
                if col |> Ops.isEmpty then 
                    col <- col'
                col <- col |> Ops.dropLast
            
        {
            TestWithIterations.Name = "DropLast"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>
        
    let inline DropFirst iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 0 to iters do 
                if col |> Ops.isEmpty then 
                    col <- col'
                col <- col |> Ops.dropFirst
            
        {
            TestWithIterations.Name = "DropFirst"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline GetRandom iters = 
        let inline test t col = 
            let count = (col |> Ops.length |> float) - 1.
            let iters = itersOf t
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.get cur |> ignore
            
        {
            TestWithIterations.Name = "Get Random"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>



    let inline SetRandom iters = 
        let inline test t col = 
            let count = (col |> Ops.length |> float) - 1.
            let iters = itersOf t
            let mutable col = col
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.set cur 0
            

        {
            TestWithIterations.Name = "Set Random"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline Take iters = 
        let inline test t col = 
            let count = col |> Ops.length |> float
            let iters = itersOf t
            let mutable col = col
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.take cur

        {
            TestWithIterations.Name = "Take Random"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline InsertRandom iters = 
        let inline test t col = 
            let count = (col |> Ops.length |> float) - 1.
            let iters = itersOf t
            let mutable col = col
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.insert cur 0
            
                
        {
            TestWithIterations.Name = "Insert Random"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline RemoveRandom iters = 
        let inline test t col = 
            let count = (col |> Ops.length |> float) - 1.
            let iters = itersOf t
            let mutable col = col
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.remove cur
            
        {
            TestWithIterations.Name = "Remove Random"
            Test = test
            Metadata = []
            Iterations = iters
        }:>ITest<_>

    let inline InsertRangeRandom iters (data : DataSource) = 
        let inline test t col = 
            let count = (col |> Ops.length |> float) - 1.
            let iters = itersOf t
            let data = dataOf t
            let mutable col = col
            for i = 0 to iters do
                let mult = PercentileData.[i]
                let cur = int(count * mult)
                col |> Ops.insertRange cur data
            

        {
            TestWithData.Name = "Insert Range Random"
            Test = test
            DataLoaded = data.Generate
            Metadata = []
            Data = data
            Iterations = iters
        }:>ITest<_>
    

            

    