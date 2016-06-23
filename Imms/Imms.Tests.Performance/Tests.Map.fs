module Imms.Tests.Performance.TestCode.Map
open Imms.Tests.Performance
open ExtraFunctional
open Imms.FSharp.Implementation
open System.Linq
#nowarn "66"
#nowarn "20"
let simpleTest(t, iters, test) = simpleTest(t, "Map", iters, test)
let dataSourceTest(t, iters, data, test)= dataSourceTest(t, "Map", iters, data, test)
let inline getAllKeys t = ((^t : (member Keys : 'a array) t))

let inline AddKeyRandom(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable colx = col
        for i = 1 to iters do
            colx <- col
            for item in dataLoaded do
                colx <- colx |> Ops.add item item
    dataSourceTest("Add", iters, data, test)
    
let inline RemoveKey(iters) = 
    let inline test col = 
        let keys = getAllKeys col
        let len = (keys |> Ops.length) - 1
        let pdata = PercentileData
        let mutable colx = col
        for i = 1 to iters do
            let mult = pdata.[i]
            let index = (float len) * mult |> int
            col |> Ops.removeKey (keys.[index])
    simpleTest("RemoveKey", iters, test)

let inline AddKeys(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable colx = col
        for i = 1 to iters do
            colx <- col
            colx <- colx |> Ops.addMany dataLoaded
    dataSourceTest("AddRange", iters, data, test)

let inline RemoveKeys(iters, ratio) = 
        
    let inline test col = 
        let len = Ops.length col
        let keys = (getAllKeys col).Take (ratio * (float len) |> int)
        for i = 1 to iters do
            col |> Ops.removeMany keys
    simpleTest("RemoveRange", iters, test) <+. Meta("Ratio", ratio)

let inline GetKeyRandom iters = 
    let inline test col = 
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
    simpleTest("Lookup", iters, test)

let inline ContainsKey iters = 
    let inline test col = 
        let keys = getAllKeys col
        let len = keys.Length - 1
                    |> float
        let mutable col = col
        for i = 1 to iters do
            let mult = PercentileData.[i]
            let cur = mult * len
            let key = keys.[int cur]
            let x = col |> Ops.contains key
            ()
    simpleTest("Contains", iters, test)