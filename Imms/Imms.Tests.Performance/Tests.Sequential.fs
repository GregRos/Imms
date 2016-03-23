module Imms.Tests.Performance.TestCode.Sequential
open Imms.Tests.Performance
open ExtraFunctional
open Imms.FSharp.Implementation
#nowarn "66"
#nowarn "20"

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
    simpleTest("RemoveLast", iters, test) <+. Class "DequeSingle"
    
let inline RemoveFirst iters = 
    let inline test col' = 
        let mutable col = col'
        for i = 1 to iters do
            if col |> Ops.isEmpty then col <- col'
            col <- col |> Ops.removeFirst
    simpleTest("RemoveFirst", iters, test) <+. Class "DequeSingle"
    
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

let inline Take iters = 
    let inline test col = 
        let count = 
            col
            |> Ops.length
            |> float
            
        let pdata = PercentileData
        let mutable col = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
            col |> Ops.take cur
    simpleTest("Take", iters, test) 
    <+. Desc("Returns a starting subsequence consisting of a random number of items (from the entire collection), {iters} times.") 
    
let inline Skip iters = 
    let inline test col = 
        let count = 
            col
            |> Ops.length
            |> float
            
        let pdata = PercentileData
        let mutable col = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let cur = int (count * mult)
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
    dataSourceTest("Insert Range", iters, data, test)


let inline EitherEndAccess(iters) =
    let sqrtIters = sqrt (float iters) |> int
    let testData = [|0 .. sqrtIters|]
    let inline test col =      
        for i = 1 to iters do
            let mutable colx = col
            colx <- colx |> Ops.addFirst 0
            colx <- colx |> Ops.addFirst 0
            colx <- colx |> Ops.addLast 0
            colx <- colx |> Ops.removeLast
            colx <- colx |> Ops.removeFirst

            for i = 1 to sqrtIters do
                colx <- colx |> Ops.addFirst 0

            for i = 1 to sqrtIters do
                colx <- colx |> Ops.addLast 0

            let mutable colz = colx
            for i = 1 to 2 * sqrtIters do
                colz <- colz |> Ops.removeLast

            colz <- colx
            colz <- colz |> Ops.addFirstRange testData
            for i = 1 to 2 * sqrtIters do
                colz <- colz |> Ops.removeFirst

            colz <- colz |> Ops.addLastRange testData
            colz <- colz |> Ops.addFirstRange testData
                

    simpleTest("EitherEndAccess", iters, test)

let inline FrontEndAccess(iters) =
    let sqrtIters = sqrt (float iters) |> int
    let testData = [|0 .. 4 * sqrtIters|]
    let inline test col = 
        for i = 1 to iters do
            let mutable colx = col
            colx <- colx |> Ops.addLast 0
            colx <- colx |> Ops.removeLast

            for i = 1 to sqrtIters do
                colx <- colx |> Ops.addLast 0

            let mutable colz = colx

            colz <- colx
            for i = 1 to 2 * sqrtIters do
                colz <- colz |> Ops.removeLast

            colz <- colz |> Ops.addLastRange testData

    simpleTest("FrontEndAccess", iters, test)