namespace Funq.Tests.Performance
open Funq.Tests
#nowarn "66"
#nowarn "20"

///A module that defines various different concrete tests.
module Test = 
    open System.Collections.Immutable
    open System.Linq
    open Funq.FSharp.Implementation
    
    let inline itersOf t = ((^s : (member Iters : int) t))
    let inline dataOf t = ((^s : (member DataLoaded : 'seq) t))
    let inline dataCountOf t = ((^s : (member Count : int) t))
    
    type SimpleTest<'s>(Name : string, Group : string, Iters : int, test : SimpleTest<'s> -> 's -> unit) as x= 
        inherit Test<'s>(Name, Group, Iters)
        override x.Test s = test x s

    type DataSourceTest<'e, 's>
        (Name : string, Group : string, Iters : int, Source : DataStructure<'e>, test : DataSourceTest<'e,'s> -> 's -> unit) as x =
        inherit Test<'s>(Name, Group, Iters, Source)
        member val DataLoaded = Source.Object
        member val Iterations = Iters
        member val Count = Source.Count
        override x.Test s = test x s

    let inline simpleTest(name,group,iters,test) = 
        SimpleTest(name, group, iters, test) :> Test<_>
          
    let inline dataSourceTest(name, group,iters, dataSource : DataStructure<_>, test) =
        DataSourceTest(name, group,iters, dataSource, test) :> Test<_>

    let inline Class str = Meta("Class", str)
    let inline Desc str = Meta("Description", str)
    let rnd = System.Random()
    

    let mutable PercentileData = Seqs.Numbers.float(0., 1.).Generate |> Seq.take (pown 10 6) |> Seq.toArray

    let refreshPercentile len = 
        PercentileData <- Seqs.Numbers.float(0., 1.).Generate |> Seq.take (pown 10 6) |> Seq.toArray

    let inline AddFirst iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col
            col <- col |> Ops.addFirst 0
            for i = 1 to iters do
                col <- col |> Ops.addFirst 0

        simpleTest("AddFirst","DequeSingle",iters,test) <+ Desc("Adds arbitrary items repeatedly to the beginning of the sequence.")
    
    let inline AddLast iters = 
        let inline test t col = 
            let iters = itersOf t
            let mutable col = col |> Ops.addLast 0
            for i = 1 to iters do
                col <- col |> Ops.addLast 0
        simpleTest("AddLast", "DequeSingle",iters, test) <+ Class "DequeSingle"
    
    let inline AddLastRange(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t |> seq
            let mutable col = col
            for i = 1 to iters do
                col <- col |> Ops.addLastRange data
        dataSourceTest("AddLastRange", "DequeRange",iters, data, test) <+ Desc("Adds a collection of items to the end {iters} times.")
    
    let inline AddFirstRange(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t |> seq
            let mutable col = col
            for i = 1 to iters do
                col <- col |> Ops.addLastRange data
        dataSourceTest("AddFirstRange", "DequeRange", iters, data, test) <+ Desc("Adds a collection of items to the beginning {iters} times.")
    
    let inline DropLast iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 1 to iters do
                if col |> Ops.isEmpty then col <- col'
                col <- col |> Ops.dropLast
        simpleTest("DropLast","DequeSingle", iters, test) <+ Class "DequeSingle"
    
    let inline DropFirst iters = 
        let inline test t col' = 
            let iters = itersOf t
            let mutable col = col'
            for i = 1 to iters do
                if col |> Ops.isEmpty then col <- col'
                col <- col |> Ops.dropFirst
        simpleTest("DropFirst","DequeSingle", iters, test) <+ Class "DequeSingle"
    
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
        simpleTest("Lookup", "IndexingSingle", iters, test) <+ Desc("Randomly looks up {iters} items by index from the entire collection.")
    
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
        simpleTest("Update", "IndexingSingle", iters, test) <+ Desc("Randomly updates items by index from the entire collection.")

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
        simpleTest("Take", "Subsequence", iters, test) 
        <+ Desc("Returns a starting subsequence consisting of a random number of items (from the entire collection), {iters} times.") 
    
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
        simpleTest("Skip", "Subsequence", iters, test)
    
    let inline Iter iters = 
        let inline test t col = 
            let iters = itersOf t
            let s = col |> Ops.asSeq
            use mutable iterator = s.GetEnumerator()
            for i = 1 to iters do
                if (iterator.MoveNext() |> not) then 
                    iterator <- s.GetEnumerator()
        simpleTest("IEnumerator", "Iteration", iters, test)
    
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
        simpleTest("Iterate", "Iteration", iters, test)
    
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
        simpleTest("Insert", "IndexingSingle", iters, test)
    
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
                if col |> Ops.isEmpty then 
                    col <- col'
                    count <- (col
                             |> Ops.length
                             |> float) - 1.
        simpleTest("Remove", "IndexingSingle", iters, test)
    
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
        dataSourceTest("Insert Range", "IndexingRange", iters, data, test)
    
    let inline getAllKeys t = ((^t : (member Keys : 'a array) t))

    let inline AddKeyRandom(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                for item in data do
                    colx <- colx |> Ops.add item item
        dataSourceTest("Add", "MapSingle", iters, data, test)
    
    let inline DropKey(iters) = 
        let inline test t col = 
            let iters = itersOf t
            let keys = getAllKeys col
            let len = (keys |> Ops.length) - 1
            let pdata = PercentileData
            let mutable colx = col
            for i = 1 to iters do
                let mult = pdata.[i]
                let index = (float len) * mult |> int
                col |> Ops.drop (keys.[index])
        simpleTest("DropKey", "MapSingle", iters, test)

    let inline AddKeys(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                colx <- colx |> Ops.addMany data
        dataSourceTest("AddRange", "MapRange", iters, data, test)

    let inline DropKeys(iters, ratio) = 
        let inline test t col = 
            let iters = itersOf t
            let len = Ops.length col
            let keys = (getAllKeys col).Take (ratio * (float len) |> int)
            for i = 1 to iters do
                col |> Ops.dropMany keys
        simpleTest("DropRange", "MapRange", iters, test) <+ Meta("Ratio", ratio)

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
        simpleTest("Lookup", "MapSingle", iters, test)
    
    let inline UnionWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.union data
        dataSourceTest("Union", "SetOperation", iters, source, test) <+ Desc("Computes the set-theoretic union with another data structure {iters} times.")
    
    let inline IntersectWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.intersect data
        dataSourceTest("Intersection", "SetOperation", iters, source, test) <+ Desc("Computes the set-theoretic intersection with another data structure {iters} times.")
    
    let inline ExceptWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.except data
        dataSourceTest("Except", "SetOperation", iters, source, test) <+ Desc("Computes the set-theoretic relative complement (or Except) operation.")
    
    let inline SymDifferenceWithSet(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.symDifference data
        dataSourceTest("Difference", "SetOperation", iters, source, test) <+ Class("SetOperation") <+ Desc("Computes the set-theoretic symmetric difference operation.")
    
    let inline SetEquals(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSetEquals data
        dataSourceTest("SetEquals", "SetRelation", iters, source, test) <+ Desc("Determines if this set equals another data structure, {iters} times.");
    
    let inline ProperSuperset(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSuperset data
        dataSourceTest("IsProperSuperset", "SetRelation", iters, source, test) <+ Desc("Determines the Superset relation.");
    
    let inline ProperSubset(iters, source) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            for i = 1 to iters do
                col |> Ops.isSubset data
        dataSourceTest("IsProperSubset", "SetRelation", iters, source, test)<+ Desc("Determines the Subset relation.");
    
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
        simpleTest("Contains", "SetSingle", iters, test)<+ Desc("Determines if a random element of the set is part of it, {iters} times.");
        
    let inline AddSetItem(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                for item in data do
                    colx <- colx |> Ops.addSet item
        dataSourceTest("Add", "SetSingle", iters, data, test)

    let inline AddSetItems(iters, data : DataStructure<_>) = 
        let inline test t col = 
            let iters = itersOf t
            let data = dataOf t
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                colx <- colx |> Ops.addSetMany data
        dataSourceTest("AddRange", "SetRange", iters, data, test)

    let inline DropSetItems(iters, ratio) = 
        let inline test t col = 
            let iters = itersOf t
            let keys = getAllKeys col
            let len = Ops.length col
            let data = keys.Take ((float len) * ratio |> int)
            let mutable colx = col
            for i = 1 to iters do
                colx <- col
                colx <- colx |> Ops.dropMany data
        simpleTest("DropRange", "SetRange", iters, test) <+ Meta("Ratio", ratio)

    let inline DropSetItem(iters) = 
        let inline test t col = 
            let iters = itersOf t
            let keys = getAllKeys col
            let per = PercentileData
            let len = keys.Length - 1
            let mutable colx = col
            for i = 1 to iters do
                let mult = per.[iters]
                let index = (mult * (float len)) |> int
                col |> Ops.dropSet (keys.[index])
        simpleTest("Drop", "SetSingle", iters, test)