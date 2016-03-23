module Imms.Tests.Performance.TestCode.Set
open Imms.Tests.Performance
open ExtraFunctional
open Imms.FSharp.Implementation
#nowarn "66"
#nowarn "20"
open System.Linq
let inline getAllKeys t = ((^t : (member Keys : 'a array) t))
let inline UnionWithSet(iters, source : _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.union data
    dataSourceTest("Union", iters, source, test) <+. Desc("Computes the set-theoretic union with another data structure {iters} times.")
    
let inline IntersectWithSet(iters, source : _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.intersect data
    dataSourceTest("Intersection", iters, source, test) <+. Desc("Computes the set-theoretic intersection with another data structure {iters} times.")
    
let inline ExceptWithSet(iters, source : _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.except data
    dataSourceTest("Except", iters, source, test) <+. Desc("Computes the set-theoretic relative complement (or Except) operation.")
    
let inline SymDifferenceWithSet(iters, source : _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.symDifference data
    dataSourceTest("Difference", iters, source, test) <+. Class("SetOperation") <+. Desc("Computes the set-theoretic symmetric difference operation.")
    
let inline SetEquals(iters, source : _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.isSetEquals data
    dataSourceTest("SetEquals", iters, source, test) <+. Desc("Determines if this set equals another data structure, {iters} times.");
    
let inline ProperSuperset(iters, source: _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.isSuperset data
    dataSourceTest("IsProperSuperset", iters, source, test) <+. Desc("Determines the Superset relation.");
    
let inline ProperSubset(iters,source: _ DataStructure) = 
    let data = source.Object
    let inline test col = 
        for i = 1 to iters do
            col |> Ops.isSubset data
    dataSourceTest("IsProperSubset", iters, source, test)<+. Desc("Determines the Subset relation.");
    
let inline Contains iters = 
    let inline test col = 
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
    simpleTest("Contains", iters, test)<+. Desc("Determines if a random element of the set is part of it, {iters} times.");
        
let inline AddSetItem(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable colx = col
        for i = 1 to iters do
            colx <- col
            for item in dataLoaded do
                colx <- colx |> Ops.addSet item
    dataSourceTest("Add", iters, data, test)

let inline AddSetItems(iters, data : DataStructure<_>) = 
    let dataLoaded = data.Object
    let inline test col = 
        let mutable colx = col
        for i = 1 to iters do
            colx <- col
            colx <- colx |> Ops.addSetMany dataLoaded
    dataSourceTest("AddRange", iters, data, test)

let inline RemoveSetItems(iters, ratio) = 
    let inline test col = 
        let keys = getAllKeys col
        let len = Ops.length col
        let data = keys.Take ((float len) * ratio |> int)
        let mutable colx = col
        for i = 1 to iters do
            colx <- col
            colx <- colx |> Ops.removeMany data
    simpleTest("RemoveRange", iters, test) <+. Meta("Ratio", ratio)

let inline RemoveSetItem(iters) = 
    let inline test col = 
        let keys = getAllKeys col
        let per = PercentileData
        let len = keys.Length - 1
        let mutable colx = col
        for i = 1 to iters do
            let mult = per.[iters]
            let index = (mult * (float len)) |> int
            col |> Ops.removeSet (keys.[index])
    simpleTest("Remove", iters, test)

