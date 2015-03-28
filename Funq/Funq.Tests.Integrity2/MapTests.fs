namespace Funq.Tests.Integrity
open System
open System.Diagnostics
open Funq.FSharp.Implementation
open Funq.FSharp

type MapTests<'e>(items : 'e array, ?seed : int) =
    let seed = defaultArg seed (Environment.TickCount)
    let create_test iters name test = Test(iters, name, MapLike, test)
    let floor x = x |> floor |> int
    member private x.add_check v (set :_ MapWrapper)  =
        let oldCount = set.Length
        let alreadyExists = set.Contains v
        let mutable set = set
        if alreadyExists then
            set <- set.Set(v, v)
            assert_eq(set.Length, oldCount)
            assert_eq(set.Get v, v )
        else
            set <- set.Add(v, v)
            assert_eq(set.Length, oldCount + 1)
            assert_eq(set.Get v, v)
        set

    member private x.drop_check v (set :_ MapWrapper) =
        let oldCount = set.Length
        let alreadyExists = set.Contains v
        let mutable set = set
        if alreadyExists then
            set <- set.Drop v
            assert_eq(set.Length, oldCount - 1)
        else
            assert_eq(set.Length, oldCount)
        set

    member private x.add_range_check (st,en) (set :_ MapWrapper) = 
        let slice = items.[st..en]
        let newSet = slice |> Seq.map (fun x -> x,x) |> set.AddRange
        let mutable increase = slice |> Seq.countWhere (fun x -> not <| set.Contains x)
        assert_eq(newSet.Length, increase + set.Length)
        for item in slice do
            assert_true(newSet.Contains item)
            assert_eq(newSet.Get item, item)
        newSet

    member private x.drop_range_check (st,en) (set :_ MapWrapper) = 
        let slice = items.[st..en]
        let newSet = set.DropRange slice
        let mutable decrease = slice |> Seq.countWhere (fun x -> set.Contains x)
        assert_eq(newSet.Length, -decrease + set.Length)
        for item in slice do
            assert_false(newSet.Contains item)
        newSet

    member private x.inner_add_drop (addBias : float) (n : int) (s: MapWrapper<_>) =
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let mutable expectedCount = s.Length
        let reduced_n = n |> float |> sqrt |> int
        let addCeil = float reduced_n * addBias |> int
        let dropCeil = reduced_n |> int
        for i = 0 to reduced_n do
            let iters = r.Next(0, addCeil)
            for j = 0 to iters do
                let ix = rnd()
                s <- s |> x.add_check (items.[ix])
            
            let iters = r.Next(0, dropCeil)

            for j = 0 to iters do
                let ix = rnd()
                s <- s |> x.drop_check (items.[ix])
        s

    member private x.inner_add_drop_range (n : int) (s : MapWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let rnd x = r.Next((items.Length |> float) * x |> floor)
        let mx = items.Length - 1
        let reduced_n = n |> float |> sqrt |> floor |> int
        let mutable s =s
        for i = 0 to reduced_n do
            let st1,st2 = (rnd 0.5),(rnd 0.5)
            let en1,en2 = r.Next(st1, st1 + reduced_n), r.Next(st2, st2 + reduced_n)
            let en1,en2 = min en1 mx, min en2 mx
            let added = s |> x.add_range_check (st1,en1) 
            let dropped = added |> x.drop_range_check (st2,en2)
            s <- dropped
        s

    member private x.inner_add_range (n : int) (s : MapWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let rnd x = r.Next((items.Length |> float) * x |> floor)
        let mx = items.Length - 1
        let reduced_n = n |> float |> sqrt |> floor |> int
        let mutable s =s
        for i = 0 to reduced_n do
            let st1,st2 = (rnd 0.5),(rnd 0.5)
            let en1,en2 = r.Next(st1, st1 + reduced_n), r.Next(st2, st2 + reduced_n)
            let en1,en2 = min en1 mx, min en2 mx
            let added = s |> x.add_range_check (st1,en1) 
            s <- added
        s

    member private x.inner_normal_add_drop_range (n : int) (s : MapWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let rnd x = r.Next((items.Length |> float) * x |> floor)
        let mx = items.Length - 1
        let reduced_n = n |> float |> sqrt |> floor |> int
        let mutable s =s
        for i = 0 to reduced_n do
            let st1,st2 = (rnd 0.5),(rnd 0.5)
            let en1,en2 = r.Next(st1, st1 + reduced_n), r.Next(st2, st2 + reduced_n)
            let en1,en2 = min en1 mx, min en2 mx
            let bias = r.ExpDouble(10.)
            let added = s |> x.inner_add_drop bias reduced_n
            let dropped = added |> x.drop_range_check (st2,en2)
            s <- dropped
        s

    member x.add_drop iters  = create_test iters "AddDrop" (x.inner_add_drop 1.3 iters >> toList1)
    member x.add_drop_range iters  = create_test iters "AddDropRange" (x.inner_add_drop_range iters >> toList1)
    member x.add_range iters  = create_test iters "AddRange" (x.inner_add_range iters >> toList1)
    member x.drop_range iters  = create_test iters "AddRange" (x.inner_normal_add_drop_range iters >> toList1)