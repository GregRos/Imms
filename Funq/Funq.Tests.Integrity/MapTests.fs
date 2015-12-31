namespace Funq.Tests.Integrity
open System
open System.Diagnostics
open Funq.FSharp.Implementation
open ExtraFunctional
open Funq.FSharp
open Funq
type MapTests<'e when 'e : comparison>(items : 'e array, ?seed : int) =

    let seed = defaultArg seed (Environment.TickCount)
    let create_test iters name test = Test(iters, name, MapLike, test)
   

    let rec rnd_kvp' mx (r : Random) (s : MapWrapper<_>) =        
        let item = items.[r.Next(0, items.Length)]
        assert_true(mx >= 0)
        match s.TryGet item with
        | Some v -> item, v
        | None -> rnd_kvp' (mx - 1) r s
    
    let rnd_kvp r s = rnd_kvp' 10 r s
         
    member private x.get_kvp_slice (n : int) = 
        let r = Random(seed)
        let st1,en1 = r.IntInterval(0, items.Length, n)
        let st2 = r.Next(0, items.Length - (en1 - st1))
        let keys,values = items.[st1..en1], items.[st2..st2+(en1-st1)]
        let kvps = Seq.zip keys values |> Seq.map (Kvp.Of)
        kvps

    member private x.gen_disjoint n (map:_ MapWrapper) =
        let r = Random(seed)
        let mutable working = map.Empty
        let rnd() = r.Next(0,items.Length)
        for i = 0 to n do
            let k,v = items.[rnd()], items.[rnd()]
            if k|> map.ContainsKey |> not then 
               working <- working.U_add k v 
        working

    member private x.gen_different n (map:_ MapWrapper) =
        let r = Random(seed)
        let mutable map = map
        let mutable drop = min n (map.Length .* 0.5)

        for i = 0 to drop do
            let k, v = map |> rnd_kvp r
            map <- map.U_remove k
        let rnd() = r.Next(0,items.Length)
        while drop >= 0 do
            let k, v = items.[rnd()], items.[rnd()]
            if k |> map.ContainsKey |> not then
                map <- map.U_add k v
                drop <- drop - 1
        map

    member private x.gen_equal_keys_dif_values n (map:_ MapWrapper) =
        let r = Random(seed)
        let mutable next = map
        for i = 0 to n do
            let k, v = next |> rnd_kvp r
            let v = items.[r.Next(0,items.Length)]
            next <- next.U_add k v
        next
        
    member private x.add n (map :_ MapWrapper) =
        let mutable map = map
        let r = Random(seed)
        let rnd() = r.Next(0,items.Length)
        for i = 0 to n do
            map <- map.U_add (items.[rnd()]) (items.[rnd()])
        map

    member private x.remove n (map :_ MapWrapper) =
        let mutable map = map
        let r = Random(seed)
        let n = n /2
        let rnd() = r.Next(0,items.Length)
        for i = 0 to n do
            let k = items.[rnd()]
            map <- map.U_remove k

        for i = 0 to n do
            let k,v = map |> rnd_kvp r
            map <- map.U_remove k
        map

    member private x.add_remove n (map:_ MapWrapper) =
        let mutable map = map
        let r = Random(seed)
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items,r.Next())
        for i = 0 to n' do
            map <- map |> tests().add n'
            map <- map |> tests().remove n'
        map

    member private x.add_range n (map:_ MapWrapper) =
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items, r.Next())
        let mutable results = []
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different
            let maps = fs |> Tuple.apply2 n map
            let super,sub,disjoint,eq,dif as curResults = maps |> Tuple.mapAll (fun mp -> t.U_addRange mp)
            results <- (curResults |> Tuple.toList) @ results
            let curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_addRange mp) |> Tuple.toList
            results <- curResults @ results
            map <- dif
        results

    member private x.remove_range n (map:_ MapWrapper) = 
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items, r.Next())
        let mutable results = []
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different, tests().gen_different
            let maps = fs |> Tuple.apply2 n map |> Tuple.mapAll (fun s -> s.toSet)
            let super,sub,disjoint,eq,dif1,dif2 as curResults = maps |> Tuple.mapAll (fun set -> t.U_removeRange set)
            results <- (curResults |> Tuple.toList) @ results
            let curResults = maps |> Tuple.mapAll (Seq.toArray >> fun mp -> t.U_removeRange mp) |> Tuple.toList
            results <- curResults @ results
            map <- dif1.U_addRange dif2|> tests().add n
        results    

    member private x.merge n (map:_ MapWrapper) = 
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items, r.Next())
        let mutable results = []
        let merge k v1 v2 = if r.Next(0,2) = 0 then v1 else v2
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different
            let maps = fs |> Tuple.apply2 n map
            let super,sub,disjoint,eq,dif as curResults = maps |> Tuple.mapAll (fun set -> t.U_merge (Some merge) set)
            results <- (curResults |> Tuple.toList) @ results
            let super,sub,disjoint,eq,dif as curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_merge (Some merge) mp)
            results <- (curResults |> Tuple.toList) @ results
            map <- dif
        results  

    member private x.join n (map:_ MapWrapper) = 
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt

        let tests() = MapTests(items, r.Next())
        let mutable results = []
        let join k v1 v2 = if r.Next(0,2) = 0 then v1 else v2
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different, tests().gen_different
            let maps = fs |> Tuple.apply2 n map
            let super,sub,disjoint,eq,dif1,dif2 as curResults = maps |> Tuple.mapAll (fun set -> t.U_join (Some join) set)
            results <- (curResults |> Tuple.toList) @ results
            let curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_join (Some join) mp) |> Tuple.toList
            results <- curResults @ results
            map <- dif1 |> tests().add n
        results 

    member private x.except n (map:_ MapWrapper) = 
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items, r.Next())
        let mutable results = []
        let join k v1 v2 =
            match k.GetHashCode() % 5 with
            | 0 -> Some v1
            | 1 -> Some v2
            | _ -> None
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different, tests().gen_different
            let maps = fs |> Tuple.apply2 n map
            let super,sub,disjoint,eq,dif1,dif2 as curResults = maps |> Tuple.mapAll (fun set -> t.U_except (Some join) set)
            results <- (curResults |> Tuple.toList) @ results
            let curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_except (Some join) mp) |> Tuple.toList
            results <- curResults @ results
            let curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_except (None) mp) |> Tuple.toList
            results <- curResults @ results
            map <- dif1.U_addRange dif2 |> tests().add n
        results 

    member private x.difference n (map:_ MapWrapper) = 
        let mutable map = map
        let r = Random(seed)
        let n = n / 2
        let n' = n |> Math.intSqrt
        let tests() = MapTests(items, r.Next())
        let mutable results = []
        for i = 0 to n' do
            let t = map
            let fs = tests().add, tests().remove, tests().gen_disjoint, tests().gen_equal_keys_dif_values, tests().gen_different, tests().gen_different
            let maps = fs |> Tuple.apply2 n map
            let super,sub,disjoint,eq,dif1,dif2 as curResults = maps |> Tuple.mapAll (fun set -> t.U_difference set)
            results <- (curResults |> Tuple.toList) @ results
            let curResults = maps |> Tuple.mapAll (Seq.asArray >> fun mp -> t.U_difference  mp) |> Tuple.toList
            results <- curResults @ results
            map <- dif1 |> tests().add n
        results 


    member private x.find n (s : MapWrapper<_>) =
        let rnd = Random(seed)
        let n' = n |> Math.intSqrt
        let mutable t = []
        for i = 0 to n' do
            let rePair = s |> rnd_kvp rnd
            let r = s.Test_predicate (fun (Kvp(pair)) -> pair = rePair)
            if r.IsSome then
                t <- (r.Value.Key, r.Value.Value)::t
        t
    member x.Add iters = create_test iters "Add" (x.add iters >> toList1 >> List.cast)
    member x.Remove iters = create_test iters "Remove" (x.remove iters >> toList1 >> List.cast)
    member x.Remove_range iters = create_test iters "RemoveRange" (x.remove_range iters >> List.cast)
    member x.Add_range iters = create_test iters "AddRange" (x.add_range iters >> List.cast)
    member x.Merge iters = create_test iters "Merge" (x.merge iters >> List.cast)
    member x.Join iters = create_test iters "Join" (x.join iters >> List.cast)
    member x.Except iters = create_test iters "Except" (x.except iters >> List.cast)
    member x.Difference iters = create_test iters "Difference" (x.difference iters >> List.cast)
    member x.Add_remove iters = create_test iters "Add, Remove" (x.add_remove iters >> toList1 >> List.cast)
    member x.Find iters = create_test iters "Find" (x.find iters >> toList1 >> List.cast)







            



