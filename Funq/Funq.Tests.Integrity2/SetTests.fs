namespace Funq.Tests.Integrity
open System
open System.Diagnostics
open Funq.FSharp.Implementation
open Funq.FSharp

module Seq = 
    let countWhere f sq = 
        sq |> Seq.fold (fun st x -> if f x then st + 1 else st) 0

type SetTests<'e>(items : 'e array, ?seed : int) =
    let seed = defaultArg seed (Environment.TickCount)
    let create_test iters name test = Test(iters, name, SetLike, test)
    member private x.add_check v (set :_ SetWrapper)  =
        let oldCount = set.Length
        let alreadyExists = set.Contains v
        let set = set.Add v
        assert_true(set.Contains v)
        if alreadyExists then
            assert_eq(set.Length, oldCount)
        else
            assert_eq(set.Length, oldCount + 1)
        set

    member private x.remove_check v (set :_ SetWrapper) =
        let oldCount = set.Length
        let alreadyExists = set.Contains v
        let set = set.Remove v
        assert_false(set.Contains v)
        if alreadyExists then
            assert_eq(set.Length, oldCount - 1)
        else
            assert_eq(set.Length, oldCount)
        set

    member private x.add_range_check (st,en) (set :_ SetWrapper) = 
        let slice = items.[st..en]
        let newSet = set.AddRange slice
        let mutable increase = slice |> Seq.countWhere (fun x -> not <| set.Contains x)
        assert_eq(newSet.Length, increase + set.Length)
        for item in slice do
            assert_true(newSet.Contains item)
        newSet

    member private x.remove_range_check (st,en) (set :_ SetWrapper) = 
        let slice = items.[st..en]
        let newSet = set.RemoveRange slice
        let mutable decrease = slice |> Seq.countWhere (fun x -> set.Contains x)
        assert_eq(newSet.Length, -decrease + set.Length)
        for item in slice do
            assert_false(newSet.Contains item)
        newSet


    member private x.inner_add_remove (addBias : float) (n : int) (s: SetWrapper<_>) =
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let mutable expectedCount = s.Length
        let reduced_n = n |> float |> sqrt |> int
        let addCeil = float reduced_n * addBias |> int
        let removeCeil = reduced_n |> int
        for i = 0 to reduced_n do
            let iters = r.Next(0, addCeil)
            for j = 0 to iters do
                let ix = rnd()
                s <- s |> x.add_check (items.[ix])
            
            let iters = r.Next(0, removeCeil)

            for j = 0 to iters do
                let ix = rnd()
                s <- s |> x.remove_check (items.[ix])
        s

    member private x.generate_sets num iters (s : SetWrapper<_>) seed =
        let r = Random(seed)
        let rec genList =
            function 
            | 0 -> []
            | n -> 
                let seed = r.Next()
                let bias = r.ExpDouble(30.)
                let set = SetTests(items, seed).inner_add_remove bias iters s 
                set::genList(n-1)
        genList num

    member private x.inner_intersection (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> float |> sqrt |> int
        for i = 0 to reduced_n do
            let a::b::_ = x.generate_sets 2 reduced_n s (r.Next())
            let intersect1 = a.Intersect(b)
            let intersect2 = b.Intersect(a)
            assert_eq(intersect1,intersect2)
            results <- intersect1::results
        results

    member private x.inner_union (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> float |> sqrt |> int
        for i = 0 to reduced_n do
            let a::b::_ = x.generate_sets 2 reduced_n s (r.Next())
            let union1 = a.Union(b)
            let union2 = b.Union(a)
            assert_eq(union1, union2)
            results <- union1::results
        results

    member private x.inner_except (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> float |> sqrt |> int
        for i = 0 to reduced_n do
            let a::b::_ = x.generate_sets 2 reduced_n s (r.Next())
            let except1 = a.Except(b)
            let except2 = b.Except(a)
            results <- except1::except2::results
        results
    
    member private x.inner_difference (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> float |> sqrt |> int
        for i = 0 to reduced_n do
            let a::b::_ = x.generate_sets 2 reduced_n s (r.Next())
            let dif1 = a.Difference(b)
            let dif2 = b.Difference(a)
            assert_eq(dif1, dif2)
            results <- dif1::results
        results

    member private x.inner_add_remove_range (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let rnd x = r.Next((items.Length |> float) * x |> int)
        let mx = items.Length - 1
        let reduced_n = n |> float |> sqrt |> int
        let mutable s =s
        for i = 0 to reduced_n do
            let st1,st2 = (rnd 0.5),(rnd 0.5)
            let en1,en2 = r.Next(st1, st1 + reduced_n), r.Next(st2, st2 + reduced_n)
            let en1,en2 = min en1 mx, min en2 mx
            let added = s |> x.add_range_check (st1,en1) 
            let removeped = added |> x.remove_range_check (st2,en2)
            s <- removeped
        s

    member private x.inner_many_operations (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let reduced_n = n |> float |> fun f -> Math.Pow(f, 0.3) |> int
        for i = 0 to reduced_n do
            let seed = r.Next()
            let newObj = SetTests(items, seed)
            let interSets = newObj.inner_intersection reduced_n s
            let mutable cur = s
            for item in interSets do
                cur <- cur.Union(item)
            let bias = r.ExpDouble(10.)
            let mutable copy = cur
            copy <- cur |> newObj.inner_add_remove bias reduced_n
            let difSets = newObj.inner_difference reduced_n s
            for item in difSets do
                copy <- copy.Except(item)
            cur <- cur.Union(copy)
            
            let mutable group1, group2, group3 = copy, s, cur
            let copies = Array.create (3 * reduced_n) copy
            for i = 0 to copies.Length - 1 do
                let newSeed = r.Next()
                let newObj = SetTests(items, newSeed)
                let copy = copies.[i] |> newObj.inner_add_remove bias (reduced_n * 10)
                match i % 5 with
                | 0 -> group1 <- group1.Union(copy)
                | 1 -> group2 <- group2.Intersect(copy)
                | 2 -> group3 <- group3.Difference(copy)
                | 3 -> group1 <- group1.Union(copy.Except(group2))
                | 4 -> group2 <- group2.Except(copy)
                | _ -> failwith "?"
            let cur = group1.Union(group2).Intersect(group3).Union(copy)
            results <- cur::group1::group2::group3::results
        results

    member x.add_remove iters  = create_test iters "AddRemove" (x.inner_add_remove 1.3 iters >> toList1)
    member x.intersection iters = create_test iters "Intersection" (x.inner_intersection iters)
    member x.union iters = create_test iters "Union" (x.inner_union iters)
    member x.except iters = create_test iters "Except" (x.inner_except iters)
    member x.difference iters = create_test iters "Difference" (x.inner_difference iters)
    member x.many_operations iters = create_test iters "Many Operations" (x.inner_many_operations iters)
    member x.add_remove_range iters = create_test iters "Add remove range" (x.inner_add_remove_range iters >> toList1)
