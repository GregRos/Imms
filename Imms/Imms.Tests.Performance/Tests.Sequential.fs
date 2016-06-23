module Imms.Tests.Performance.TestCode.Sequential
open Imms.Tests.Performance
open ExtraFunctional
open Imms.FSharp.Implementation
#nowarn "66"
#nowarn "20"

let simpleTest(t, iters, test) = simpleTest(t, "Seq", iters, test)
let dataSourceTest(t, iters, data, test)= dataSourceTest(t, "Seq", iters, data, test)
let inline AddFirst iters = 
    let inline test col = 
        let mutable col = col
        col <- col |> Ops.addFirst 0
        for i = 1 to iters do
            col <- col |> Ops.addFirst 0

    simpleTest("AddFirst",iters,test) <+. Desc("Adds arbitrary items repeatedly to the beginning of the sequence.")
    
let inline AddLast iters = 
    let inline test col = 
        let mutable col = col |> Ops.addLast 0
        for i = 1 to iters do
            col <- col |> Ops.addLast 0
    simpleTest("AddLast",iters, test) <+. Class "DequeSingle"
    
let inline AddLastRange(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable col = col
        for i = 1 to iters do
            col <- col |> Ops.addLastRange dataLoaded
    dataSourceTest("AddLastRange",iters, data, test) <+. Desc("Adds a collection of items to the end {iters} times.")
    
let inline AddFirstRange(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable col = col
        for i = 1 to iters do
            col <- col |> Ops.addFirstRange dataLoaded
    dataSourceTest("AddFirstRange", iters, data, test) <+. Desc("Adds a collection of items to the beginning {iters} times.")
    
let inline RemoveLast iters = 
    let inline test col' = 
        let mutable col = col'
        for i = 1 to iters do
            if col |> Ops.isEmpty then col <- col'
            col <- col |> Ops.removeLast
    simpleTest("RemoveLast", iters, test)
    
let inline First iters = 
    let inline test col = 
        for i = 1 to iters do 
            let x = col |> Ops.first
            ()
    simpleTest("First", iters, test) 

let inline Last iters = 
    let inline test col = 
        for i = 1 to iters do 
            let x = col |> Ops.last
            ()
    simpleTest("Last", iters, test) 

let inline RemoveFirst iters = 
    let inline test col' = 
        let mutable col = col'
        for i = 1 to iters do
            if col |> Ops.isEmpty then col <- col'
            col <- col |> Ops.removeFirst
    simpleTest("RemoveFirst", iters, test)
    
let inline GetRandom iters = 
    let inline test col = 
        let count = 
            (col
                |> Ops.length
                |> float) - 1.

        let pdata = PercentileData
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col
            |> Ops.get cur
            |> ignore
    simpleTest("Lookup", iters, test) <+. Desc("Randomly looks up {iters} items by index from the entire collection.")
    
let inline SetRandom iters = 
    let inline test col = 
        let count = 
            (col
                |> Ops.length
                |> float) - 1.
            
        let mutable col = col
        let pdata = PercentileData
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col |> Ops.set cur 0
    simpleTest("Update", iters, test) <+. Desc("Randomly updates items by index from the entire collection.")

let inline Take(iters, mag) = 
    let mag = float mag
    let inline test col = 
        let count = 
            col
            |> Ops.length
            |> float
            
        let pdata = PercentileData
        let mutable col = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (mag * mult)
            col |> Ops.take cur
    simpleTest("Take", iters, test) 
    <+. Desc("Returns a starting subsequence consisting of a random number of items (from the entire collection), {iters} times.") 
    
let inline Slice(iters, mag) = 
    let mag = float mag
    let inline test col = 
        let count = 
            col
            |> Ops.length
            |> float
            
        let pdata = PercentileData
        let mutable col = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let i1 = mult * count
            let maxCount = min (mag) (count - i1)
            let curCount = mult * maxCount
            col |> Ops.slice (int i1) (i1 + curCount |> int)
    simpleTest("Slice", iters, test) 
    <+. Desc("Returns a starting subsequence consisting of a random number of items (from the entire collection), {iters} times.") 

let inline Skip(iters,mag) = 
    let mag = float mag
    let inline test col = 
        let count = 
            col
            |> Ops.length
            |> float
            
        let pdata = PercentileData
        let mutable col = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (mag * mult)
            col |> Ops.skip cur
    simpleTest("Skip", iters, test)

let inline InsertRandom iters = 
    let inline test col = 
        let mutable count = 
            (col
                |> Ops.length
                |> float) - 1.
            
        let mutable col = col
        let pdata = PercentileData
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col <- col |> Ops.insert cur 0
            count <- count + 1.
    simpleTest("Insert", iters, test)
    
let inline RemoveRandom iters = 
    let inline test col = 
        let mutable count = 
            (col
                |> Ops.length
                |> float) - 1.
            
        let col' = col
        let mutable col = col
        let pdata = PercentileData
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col <- col |> Ops.removeAt cur
            count <- count - 1.
            if col |> Ops.isEmpty then 
                col <- col'
                count <- (col
                            |> Ops.length
                            |> float) - 1.
    simpleTest("Remove", iters, test)
    
let inline InsertRangeRandom(iters, data : DataStructure<_>) = 
    let cnt = data.Count
    let dataLoaded = data.Object
    let inline test col = 
        let mutable count = 
            (col
                |> Ops.length
                |> float) - 1.
            
        let cnt = (cnt|> float) - 1.
        let mutable col = col
        let pdata = PercentileData
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col <- col |> Ops.insertRange cur dataLoaded
            count <- count + cnt
    dataSourceTest("InsertRange", iters, data, test)
