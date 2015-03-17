namespace Funq.Tests.Performance
open Funq.Tests
#nowarn "66"
#nowarn "20"

///A module that defines various different concrete tests.
module Test = 
    open System.Collections.Immutable
    open System.Linq
    open Funq.FSharp.Implementation
    
    let inline itersOf t = ((^s : (member Iterations : int) t))
    let inline dataOf t = ((^s : (member DataLoaded : 'seq) t))
    let inline dataCountOf t = ((^s : (member Count : int) t))
    

    type SimpleTest<'s>(name : string, iters : int, test : SimpleTest<'s> -> 's -> unit, ?metadata : Meta seq) as x= 
        inherit Test<'s>(name, metadata.Or [])
        do x?Iterations <- iters
        do x?Kind <- "Simple"
        member val Iterations = iters
        override x.Test s = test x s

    type DataSourceTest<'e, 's>
        (name : string, iters : int, source : DataStructure<'e>, test : DataSourceTest<'e,'s> -> 's -> unit, ?metadata : Meta seq) as x =
        inherit Test<'s>(name, metadata.Or [])
        do  x?Iterations <- iters
            x?Source <- source.Clone
        do  x?Kind <- "Data Source"
        member val DataLoaded = source.Object
        member val Iterations = iters
        member val Count = source.Count
        override x.Test s = test x s
    let inline simpleTest(name,iters,test) = 
        SimpleTest(name, iters, test) :> Test<_>
          
    let inline dataSourceTest(name, iters, dataSource : DataStructure<_>, test) =
        DataSourceTest(name, iters, dataSource, test) :> Test<_>
    let inline Class str = Meta("Class", str)
    let inline Desc str = Meta("Description", str)
    let rnd = System.Random()
    

    let mutable PercentileData = 
        Seqs.Numbers.float (0., 1.)
        |> Data.Basic.Array(10 ^* 6)
        |> fun x -> x.Object
    
    let refreshPercentile len = 
        PercentileData <- Seqs.Numbers.float (0., 1.)
        |> Data.Basic.Array(len)
        |> fun x -> x.Object

    let inline AddFirst iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col
            col <- col |> Ops.addFirst 0
            for i = 1 to iters do
                col <- col |> Ops.addFirst 0

        simpleTest("AddFirst",iters,test) <+ Desc("Adds arbitrary items repeatedly to the beginning of the sequence.") <+ Class "DequeSingle"
    
    let inline AddLast iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col |> Ops.addLast 0
            for i = 1 to iters do
                col <- col |> Ops.addLast 0
        simpleTest("AddLast", iters, test) <+ Class "DequeSingle"
    
    let inline AddLastRange(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t |> seq
            let mutable col = col
            for i = 1 to iters do
                col <- col |> Ops.addLastRange data
        dataSourceTest("AddLastRange", iters, data, test) <+ Desc("Adds a collection of items to the end {iters} times.") <+ Class "DequeRange"
    
    let inline AddFirstRange(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t |> seq
            let mutable col = col
            for i = 1 to iters do
                col <- col |> Ops.addLastRange data
        dataSourceTest("AddFirstRange", iters, data, test) <+ Desc("Adds a collection of items to the beginning {iters} times.") <+ Class "DequeRange"
    
    let inline DropLast iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 1 to iters do
                if col |> Ops.isEmpty then col <- col'
                col <- col |> Ops.dropLast
        simpleTest("DropLast", iters, test) <+ Class "DequeSingle"
    
    let inline DropFirst iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 1 to iters do
                if col |> Ops.isEmpty then col <- col'
                col <- col |> Ops.dropFirst
        simpleTest("DropFirst", iters, test) <+ Class "DequeSingle"
    
    let inline GetRandom iters = 
        let inline test t col = 
            let count = 
                (col
                 |> Ops.length
                 |> float) - 1.
            
            let iters = itersOf t
            let pdata = PercentileData
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col
                |> Ops.get cur
                |> ignore
        simpleTest("Random Lookups", iters, test) <+ Desc("Randomly looks up {iters} items by index from the entire collection.") <+ Class "IndexingSingle"
    
    let inline SetRandom iters = 
        let inline test t col = 
            let count = 
                (col
                 |> Ops.length
                 |> float) - 1.
            
            let iters = itersOf t
            let mutable col = col
            let pdata = PercentileData
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col |> Ops.set cur 0
        simpleTest("Random Updates", iters, test) <+ Desc("Randomly updates items by index from the entire collection.") <+ Class "IndexingSingle"

    let inline Take iters = 
        let inline test t col = 
            let count = 
                col
                |> Ops.length
                |> float
            
            let iters = itersOf t
            let pdata = PercentileData
            let mutable col = col
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col |> Ops.take cur
        simpleTest("Random Take", iters, test) 
        <+ Desc("Returns a starting subsequence consisting of a random number of items (from the entire collection), {iters} times.") 
        <+ Class "IndexingRange"
    
    let inline Skip iters = 
        let inline test t col = 
            let count = 
                col
                |> Ops.length
                |> float
            
            let iters = itersOf t
            let pdata = PercentileData
            let mutable col = col
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col |> Ops.skip cur
        simpleTest("Random Skip (ending subsequence)", iters, test)
        <+ Class "IndexingRange"
    
    let inline Iter iters = 
        let inline test t col = 
            let iters = itersOf t
            let s = col |> Ops.asSeq
            use mutable iterator = s.GetEnumerator()
            for i = 1 to iters do
                if (iterator.MoveNext() |> not) then 
                    iterator <- s.GetEnumerator()
        simpleTest("Iterate using IEnumerator", iters, test)
        <+ Class "Iteration"
    
    let mutable count = 0
    let iterFor n v = 
        if count >= n then
            false
        else
            count <- count + 1
            true

    let inline IterDirect iters = 
        let inline test t col = 
            let iters = itersOf t
            for i = 1 to iters do
                col |> Ops.iter
        simpleTest("Iterate using Dedicated Method", iters, test)
        <+ Class "Iteration"
    
    let inline InsertRandom iters = 
        let inline test t col = 
            let mutable count = 
                (col
                 |> Ops.length
                 |> float) - 1.
            
            let iters = itersOf t
            let mutable col = col
            let pdata = PercentileData
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col <- col |> Ops.insert cur 0
                count <- count + 1.
        simpleTest("Insert at Random Indexes", iters, test)
        <+ Class "IndexingSingle"
    
    let inline RemoveRandom iters = 
        let inline test t col = 
            let mutable count = 
                (col
                 |> Ops.length
                 |> float) - 1.
            
            let iters = itersOf t
            let col' = col
            let mutable col = col
            let pdata = PercentileData
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col <- col |> Ops.remove cur
                count <- count - 1.
                if col |> Ops.isEmpty then col <- col'
        simpleTest("Remove at Random Indexes", iters, test)
        <+ Class "IndexingSingle"
    
    let inline InsertRangeRandom(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let mutable count = 
                (col
                 |> Ops.length
                 |> float) - 1.
            
            let iters = itersOf t
            let data = dataOf t |> seq
            let cnt = (dataCountOf t |> float) - 1.
            let mutable col = col
            let pdata = PercentileData
            for i = 1 to iters do
                let mult = pdata.[i]
                let cur = int (count * mult)
                col <- col |> Ops.insertRange cur data
                count <- count + cnt
        dataSourceTest("Insert Data Structure at Random Indexes", iters, data, test)
        <+ Class "IndexingRange"
    
    let inline AddKeyRandom(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                for item in data do
                    colx <- colx |> Ops.add item item
        dataSourceTest("Add Key Random", iters, data, test)
    
    let inline getAllKeys t = ((^t : (member Keys : 'a array) t))
    
    let inline GetKeyRandom iters = 
        let inline test t col = 
            let iters = itersOf t
            let keys = getAllKeys col
            let len = keys.Length - 1
                      |> float
            let mutable col = col
            for i = 1 to iters do
                let mult = PercentileData.[i]
                let cur = mult * len
                let key = keys.[int cur]
                let x = col |> Ops.get key
                ()
        simpleTest("Lookup Random", iters, test)
    
    let inline UnionWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.union data
        dataSourceTest("Union", iters, source, test) <+ Desc("Computes the set-theoretic union with another data structure {iters} times.")
    
    let inline IntersectWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.intersect data
        dataSourceTest("Intersection", iters, source, test) <+ Desc("Computes the set-theoretic intersection with another data structure {iters} times.")
    
    let inline ExceptWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.except data
        dataSourceTest("Except", iters, source, test) <+ Desc("Computes the set-theoretic relative complement (or Except) operation.")
    
    let inline SymDifferenceWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.symDifference data
        dataSourceTest("Symmetric Difference", iters, source, test) <+ Desc("Computes the set-theoretic symmetric difference operation.")
    
    let inline SetEquals(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSetEquals data
        dataSourceTest("SetEquals Test", iters, source, test) <+ Desc("Determines if this set equals another data structure, {iters} times.");
    
    let inline ProperSuperset(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSuperset data
        dataSourceTest("IsProperSuperset Test", iters, source, test) <+ Desc("Determines the Superset relation.");
    
    let inline ProperSubset(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSubset data
        dataSourceTest("IsProperSubset Test", iters, source, test) <+ Desc("Determines the Subset relation.");
    
    let inline Contains iters = 
        let inline test t col = 
            let iters = itersOf t
            let keys = getAllKeys col
            let len = keys.Length - 1
                      |> float
            let mutable col = col
            for i = 1 to iters do
                let mult = PercentileData.[i]
                let cur = mult * len
                let key = keys.[int cur]
                let x = col |> Ops.setContains key
                ()
        simpleTest("Contains Test", iters, test) <+ Desc("Determines if a random element of the set is part of it, {iters} times.");
        
    let inline AddSetItem(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                for item in data do
                    colx <- colx |> Ops.addSet item
        dataSourceTest("Add to Set Random", iters, data, test)

    let inline DropSetItem(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                for item in data do
                    colx <- colx |> Ops.dropSet item
        dataSourceTest("Drop to Set Random", iters, data, test)