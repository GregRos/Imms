namespace Imms.Tests.Integrity
open System
open System.Diagnostics
open Imms.FSharp.Implementation
open Imms.FSharp
open Imms
open ExtraFunctional

type SetTests<'e when 'e : comparison>(items : 'e array,  ?seed : int) =
    let seed = defaultArg seed (Environment.TickCount)
    let create_test iters name test = Test(iters, name, SetLike, test)

    let rnd_slice n (r : Random) = 
        let st = r.Next(0, items.Length)
        let en = r.Next(st, min (items.Length) (st+n))
        items.[st..en]

    let rnd_element (r : Random) (s : SetWrapper<_>) = 
        let ix = r.Next(0, s.Length)
        s.ByArbitraryOrder ix

    member private x.add (n : int) (s : SetWrapper<_>) =
        let mutable t = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        for i = 0 to n do
            t <- t.U_add (items.[rnd()])
        while t.Length = s.Length do
            t <- t.U_add (items.[rnd()])
        t

    member private x.remove (errRate : float) (n : int)  (s : SetWrapper<_>) =
        let mutable t = s
        let r = Random(seed)
        let mutable i = 0
        let rnd() = r.Next(0, items.Length)
        while i < n && (not <| t.IsEmpty) do
            if r.NextDouble() < errRate then
                t <- t.U_remove (items.[rnd()])
            else
                t <- t.U_remove (rnd_element r t)
            i <- i + 1
        t
        
    (*
        Lengths:
            *   0, 1, 2
            *   > Length
            *   < Length
            *   = Length
        Set qualities:
            *   (Proper) subset
            *   (Proper) superset
            *   Disjoint
            *   Equal
            *   None of the above
        Collections:
            *   Array
            *   ICollection<T>
            *   ICollection
            *   Same type as source:
                *   Structural sharing
                *   No structural sharing
                *   Different equality comparer
                *   Same equality comparer
         Other:
            *   Contains duplicates
            *   Doesn't contain duplicates
    *)

    

    member private x.gen_collections n (s : SetWrapper<_>) = 
        let r = Random(seed)

        let randomData n =
            let st,en = r.IntInterval(0, items.Length, n)
            items.[st .. en] |> Data.source n "RandomData"

        let subset n =
            let mlist = MList.empty
            for i = 0 to n do
                mlist.Add(s.ByArbitraryOrder(r.Next(0, s.Length)))
            mlist |> Data.source n "Subset"

        let disjoint n = 
            let ml = MList.empty
            for i = 0 to n do
                let ix = r.Next(0, items.Length)
                if items.[ix] |> s.Contains then
                    ml.Add(items.[ix])
            ml |> Data.source (ml.Count) "Disjoint"

        let superset n =
            let mutable list = s |> MList.ofSeq
            for i = 0 to n do
                let ix = r.Next(0, items.Length)
                list.Add(items.[ix])
            list |> Data.source (list.Count) "Superset"

        let values = [0; 1; 2; n]
        let basics = [randomData; subset; disjoint] |> List.cross_apply1 values
        basics
    member private x.gen_disjoint_to (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let r=  Random(seed)
        let rnd() = r.Next(items.Length)
        let mutable i = 0
        let mutable d = s.Empty
        while i < n || d.IsEmpty do
            let v = items.[rnd()]
            if s.Contains v |> not then d <- d.U_add v
            i <- i + 1
        d

    member private x.gen_equal_set (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let mutable added = []
        for i = 0 to n do
            let v = items.[rnd()]
            if s.Contains v then 
                s <- s.U_add v 
            else 
                s <- s.U_add v
                added <- v::added
        for item in added do
            s <- s.U_remove item
        s

    member private x.gen_different_set (n : int) (s : SetWrapper<_>) =
        let mutable t = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let mutable reduced = min (t.Length .* 0.3) n
        for i = 0 to reduced do
            let v = t |> rnd_element r
            t <- t.U_remove v 

        while reduced >= 0 do
            let v = items.[rnd()]
            if (float (s.Length) / float (items.Length)) > 0.5 then
                reduced <- 0
            if (s.Contains v || t.Contains v) |> not then 
                t <- t.U_add v
                reduced <- reduced - 1

        t

    member private x.set_relation (n : int) (s : SetWrapper<_>) =
        let r = Random(seed)
        let n' = n |> Math.intSqrt
        let tests() = SetTests(items,r.Next())
        for i = 0 to n' do
            let empty1 = s.Empty
            let empty2 = empty1 |> tests().gen_equal_set n'
            let super = s |> tests().add n'
            let sub = s |> tests().remove 0.5 n'
            let eq = s |> tests().gen_equal_set n' 
            let none = s |> tests().gen_different_set n'
            let d = s |> tests().gen_disjoint_to n'
            assert_eq(s.U_relatesTo sub, SetRelation.ProperSupersetOf)
            assert_eq(s.U_relatesTo eq, SetRelation.Equal)
            assert_eq(s.U_relatesTo d, SetRelation.Disjoint)
            assert_eq(s.U_relatesTo none, SetRelation.None)
            assert_eq(s.U_relatesTo super, SetRelation.ProperSubsetOf)
            assert_eq(none.U_relatesTo empty2, SetRelation.Disjoint ||| SetRelation.ProperSupersetOf)
            assert_eq(empty1.U_relatesTo empty2, SetRelation.Disjoint ||| SetRelation.Equal)
            assert_eq(empty2.U_relatesTo none, SetRelation.Disjoint ||| SetRelation.ProperSubsetOf)
        ()

    member private x.add_remove (addBias : float) (n : int) (s: SetWrapper<_>) =
        let old = s
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next(items.Length)
        let reduced_n = n |> Math.intSqrt
        let addCeil = float reduced_n * addBias |> int
        let removeCeil = reduced_n |> int
        let tests() = SetTests(items,r.Next())
        for i = 0 to reduced_n do
            let iters = r.Next(0, addCeil)
            s <- s |> tests().add iters
            let iters = r.Next(0, removeCeil)
            s <- s |> tests().remove 0.5 iters
        s
            
    member private x.intersection_union (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> Math.intSqrt
        let tests() = SetTests(items,r.Next())
        for i = 0 to reduced_n do
            let t = s
            let fs = tests().remove 1., tests().add, tests().gen_disjoint_to, tests().gen_equal_set, tests().gen_different_set, tests().gen_different_set
            let sets = fs |> Tuple.apply2 n s
            let (iSub, iSuper, iDisjoint, iEq, iNone1, iNone2) as curResults = sets |> Tuple.mapAll (fun x -> t.U_intersect x)
            results <- (curResults |> Tuple.toList) @ results
            if s.IsReference then 
                results <- (curResults |> Tuple.toList) @ results
            else
                let curResults = sets |> Tuple.mapAll (fun x -> t.U_intersect (x |> Seq.asArray)) |> Tuple.toList
                results <- curResults @ results

            s <- iNone1|> tests().add n
        results  
          
    member private x.union (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> Math.intSqrt
        let tests() = SetTests(items,r.Next())
        for i = 0 to reduced_n do
            let t = s
            let fs = tests().remove 1., tests().add, tests().gen_disjoint_to, tests().gen_equal_set, tests().gen_different_set, tests().gen_different_set
            let sets = fs |> Tuple.apply2 n s
            let (uSub, iSuper, uDisjoint, uEq, uNone1, uNone2) as curResults = sets |> Tuple.mapAll (fun x -> t.U_union x)
            results <- (curResults |> Tuple.toList) @ results
            if s.IsReference then 
                results <- (curResults |> Tuple.toList) @ results
            else
                let curResults = sets |> Tuple.mapAll (fun x -> t.U_union (x |> Seq.asArray)) |> Tuple.toList
                results <- curResults @ results
            s <- uNone1
        results

    member private x.except_union (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> Math.intSqrt
        let tests() = SetTests(items,r.Next())
        for i = 0 to reduced_n do
            let t = s
            let fs = tests().remove 1., tests().add, tests().gen_disjoint_to, tests().gen_equal_set, tests().gen_different_set, tests().gen_different_set
            let sets = fs |> Tuple.apply2 n s
            let (uSub, iSuper, uDisjoint, uEq, uNone1, uNone2) as curResults = sets |> Tuple.mapAll (fun x -> t.U_except x)
            results <- (curResults |> Tuple.toList) @ results
            if s.IsReference then 
                results <- (curResults |> Tuple.toList) @ results
            else
                let curResults = sets |> Tuple.mapAll (fun x -> t.U_except (x |> Seq.asArray)) |> Tuple.toList
                results <- curResults @ results
            s <- uNone1.U_union uNone2 |> tests().add n
        results
    
    member private x.difference (n : int) (s : SetWrapper<_>) =
        let mutable s = s
        let mutable results = []
        let r = Random(seed)
        let reduced_n = n |> Math.intSqrt
        let tests() = SetTests(items,r.Next())
        for i = 0 to reduced_n do
            let t = s
            let fs = tests().remove 1., tests().add, tests().gen_disjoint_to, tests().gen_equal_set, tests().gen_different_set, tests().gen_different_set
            let sets = fs |> Tuple.apply2 n s
            let (uSub, iSuper, uDisjoint, uEq, uNone1, uNone2) as curResults = sets |> Tuple.mapAll (fun x -> t.U_difference x)
            results <- (curResults |> Tuple.toList) @ results
            if s.IsReference then 
                results <- (curResults |> Tuple.toList) @ results
            else
                let curResults = sets |> Tuple.mapAll (fun x -> t.U_difference (x |> Seq.asArray)) |> Tuple.toList
                results <- curResults @ results
            s <-  uNone1 |> tests().add n
        results

    member private x.find n (s : SetWrapper<_>) =
        let rnd = Random(seed)
        let n' = n |> Math.intSqrt
        let mutable t = []
        for i = 0 to n' do
            let at = rnd.Next(0, s.Length)
            let re = s |> rnd_element rnd
            let r = s.Test_predicate (fun x -> x = re)
            if r.IsSome then
                t <- r.Value::t
        t

    member private x.complex_many_operations (n : int) (s : SetWrapper<_>) =
        let mutable results = []
        let r = Random(seed)
        let tests() = SetTests(items, r.Next())
        let reduced_n = n |> float |> fun f -> Math.Pow(f, 0.3) |> int
        for i = 0 to reduced_n do
            
            let interSets = tests().intersection_union reduced_n s
            let mutable cur = s
            for item in interSets do
                cur <- cur.Union(item)
            let bias = r.ExpDouble(10.)
            let mutable copy = cur
            copy <- cur |> tests().add_remove bias reduced_n
            let difSets = tests().difference reduced_n s
            for item in difSets do
                copy <- copy.Except(item)
            cur <- cur.Union(copy)
            
            let mutable group1, group2, group3 = copy, s, cur
            let copies = Array.create (3 * reduced_n) copy
            for i = 0 to copies.Length - 1 do
                let copy = copies.[i] |> tests().add_remove bias (reduced_n * 10)
                match i % 5 with
                | 0 -> group1 <- group1.U_union copy
                | 1 -> group2 <- group2.U_union copy
                | 2 -> group3 <- group3.U_difference copy
                | 3 -> group1 <- group1.U_union copy
                | 4 -> group2 <- group2.U_except copy
                | _ -> failwith "?"
            let cur = copy |> group1.U_union |> group2.U_intersect |> group3.U_except
            results <- cur::group1::group2::group3::results
        results

    member x.Add_remove iters  = create_test iters "AddRemove" (x.add_remove 1.3 iters >> toList1 >> List.cast)
    member x.Intersection iters = create_test iters "Intersection" (x.intersection_union iters >> List.cast)
    member x.Union iters = create_test iters "Union" (x.union iters >> List.cast)
    member x.Except iters = create_test iters "Except" (x.except_union iters >> List.cast)
    member x.Difference iters = create_test iters "Difference" (x.difference iters >> List.cast)
    member x.Many_operations iters = create_test iters "Many Operations" (x.complex_many_operations iters >> List.cast)
    member x.Set_relation iters = create_test iters "Set relation" (x.set_relation iters >> toList1 >> List.cast)
    member x.Find iters = create_test iters "Find" (x.find iters >> toList1 >> List.cast)