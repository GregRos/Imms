namespace Funq.Tests.Integrity
open System
open System.Diagnostics
open Funq.FSharp.Implementation
open Funq.FSharp
open Funq.Abstract
open ExtraFunctional
open Funq
type SeqTests(?seed : int) = 
    let create_test iters name test = Test(iters, name, ListLike, test)
    let test_of n name (test : _ -> obj list) = create_test n name test
    let seed = defaultArg seed (Environment.TickCount)

    //The next are private test methods. They are integration tests, and rely on the unit tests defined above.
    //The methods below are called as part of the test infrastructure, though the tests are exposed through the Test object and not directly.
    //I used this format because some tests call other tests (in the order of their appearance), and should do it without invoking the test infrastructure.

    (*
        For operations accepting a collection
        Collection type:
            1. array
            2. iterate once sequence
            3. ICollection
            4. ICollection<T>
            5. Collection of the same type (no structural sharing)
            6. Collection of the same type (structural sharing)
        Element count:
            1. empty
            2. 1 element
            3. 2 elements
            4. n < Length elements
            5. n > Length elements
    *)

    member private x.gen_collections n (s : SeqWrapper<int>) = 
        let r = Random(seed)

        let genData() = 
            let init = r.Next()
            fun n -> Array.init n (fun i -> i + init) |> Data.source n "Random" 
        

        let basics = genData() >> Data.basicCollections
        
        let subseq n = 
            let st, en = r.IntInterval(0, s.Length-1, n)
            let col =  s.U_slice(st, en)
            col |> seq |> Data.collection' (en - st) "Subsequence"

        let superseq n =
            let data = (genData() n).Value
            s.U_add_last_range(data) |> seq |> Data.collection' (s.Length + n) "Supersequence" 

        let sameType n =
            let data = genData() n
            s.Empty.U_add_last_range(data.Value) |> seq |> Data.collection' n "SameType"

        let values = [0; 1; 2; n]
        let basics = values |> List.map basics |> List.collect id
        let sameType = [subseq; sameType] |> List.cross_apply1 [0; 1; 2; n]
        basics @ sameType
        
    ///Operations: AddLast
    member private x.add_last (n : int) (s : SeqWrapper<int>) = 
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next()
        for i = 0 to n do
            let v = rnd()
            s <- s.U_add_last v
        s

    ///Operations: AddFirst
    member private x.add_first n (s : int SeqWrapper) = 
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next()
        for i = 0 to n do
            let v = rnd()
            s <- s.U_add_first v
        s
    ///Operations: RemoveLast
    member private x.remove_last (n : int) (s : SeqWrapper<int>) = 
        let mutable s = s
        let mutable loop = true
        let mutable i = 0
        while loop && i < n do
            match s.Length with
            | 0 -> 
                s.U_ex_is_empty |> ignore
                loop <- false
            | 1 -> s.U_ex_has_one_element |> ignore
            | _ -> s <- s.U_drop_last
            i <- i + 1
        s

    ///Operations: RemoveFirst
    member private x.remove_first n (s : int SeqWrapper) = 
        let mutable s = s
        let mutable loop = true
        let mutable i = 0
        while loop && i < n do
            match s.Length with
            | 0 -> 
                s.U_ex_is_empty |> ignore
                loop <- false
            | 1 -> s.U_ex_has_one_element |> ignore
            | _ -> s <- s.U_drop_first
            i <- i + 1
        s

    ///Operations: AddLastRange (concat and non-concat)
    member private x.add_last_range n (s : int SeqWrapper) = 
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 5
        let rnd = r.SpecialRecipe n'
        let tests() = SeqTests(r.Next(seed))
        let mutable s = s
        for i = 0 to n' do
            let cols = tests().gen_collections (rnd()) s
            for col in cols do
                s <- s.U_add_last_range (col.Value)       
        s

    ///Operations: AddFirstRange (concat and non-concat)
    member private x.add_first_range  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 5
        let rnd = r.SpecialRecipe n'
        let mutable s = s
        let mutable t = []
        let tests() = SeqTests(r.Next(seed))
        for i = 0 to n' do
            let cols = tests().gen_collections (rnd()) s
            for col in cols do
                s <- s.U_add_first_range (col.Value)
            ()
        s

    ///Operations: AddFirst, AddLast, AddFirstRange, AddLastRange
    member private x.add_first_last  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 5
        let rnd = r.SpecialRecipe n'
        let tests() = SeqTests(r.Next())
        for i = 0 to n' do
            s <- s |> tests().add_last (rnd())
            s <- s |> tests().add_first(rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().add_first_range (rnd())
        s    

    ///Operations: AddLast, AddLastRange, RemoveLast
    member private x.add_remove_last  n (s : SeqWrapper<int>) = 
        let mutable s = s        
        let r = Random(seed)
        let n' = n / 8 |> Math.intSqrt
        let rnd = r.SpecialRecipe n'
        let tests() = SeqTests(r.Next())
        for i = 0 to n' do
            s <- s |> tests().add_last (rnd())
            s <- s |> tests().remove_last (rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().remove_last (rnd())
            s <- s |> tests().remove_last ((rnd()))
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().remove_last (rnd())
            s <- s |> tests().add_last_range (rnd())
        s
    
    ///Operations: AddFirst, RemoveFirst, and AddFirstRange (with concat).
    member private x.add_remove_first  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let n' = (n / 6 |> Math.intRoot 2)
        let rnd = r.SpecialRecipe n'
        let tests() = SeqTests(r.Next())
        let mutable s = s
        for i = 0 to n' do
            s <- s |> tests().add_first (rnd())
            s <- s |> tests().add_first (rnd())
            s <- s |> tests().remove_first (rnd())
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().remove_first (rnd())
            s <- s |> tests().add_first_range (rnd())     
        s

    ///Operations: AddFirst, AddLast, RemoveFirst, RemoveLast, AddFirstRange, AddLastRange
    member private x.add_remove_first_last  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 10
        let tests() = SeqTests(r.Next())
        let rnd = r.SpecialRecipe n'
        for i = 0 to n' do
            s <- s |> tests().add_first (rnd())
            s <- s |> tests().remove_first (rnd())
            s <- s |> tests().add_last (rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().remove_first (rnd())
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().remove_first (rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().remove_last (rnd())
            s <- s |> tests().add_last_range (rnd())
        s

    ///Operations: AddLast, RemoveLast, AddLastRange, AddFirstRange
    member private x.add_remove_last_first_limited n (s : SeqWrapper<int>) =
        let mutable s = s
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 7
        let tests() = SeqTests(r.Next())
        let rnd = r.SpecialRecipe n'
        for i = 0 to n' do
            s <- s |> tests().add_last (rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().add_last_range (rnd())
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().remove_last (rnd())
            s <- s |> tests().add_last_range (rnd())
        s

    ///Operations: Indexing, AddLast
    member private x.get_index  n (s : SeqWrapper<int>) = 
        let mutable t = s.Empty
        let mutable i = 0
        while i < n do
            let mutable ix = i % (s.Length)
            t <- t.U_add_last (s.U_get ix)
            i <- i + 1
        t

    ///Operations: Update, indexing
    member private x.update  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        let mutable s = s
        for i = 0 to n do
            let ix = rnd.Next(0, s.Length)
            s <- s.U_update ix (rnd.Next(0, n))
        s

    ///Operations: Insert
    member private x.insert  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let rnd() = r.Next(0, s.Length + 1)
        let mutable s = s
        for i = 0 to n do
            s <- s.U_insert (rnd()) (rnd())
        s

    ///Operations: Take, TakeWhile
    member private x.take  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let rnd = r.SpecialRecipe n
        let n' = n |> Math.intSqrt
        let mutable m_seq = []
        for i = 0 to n' do
            let m = rnd()
            let res = s.U_take (m)
            let at = rnd()
            m_seq <- res::m_seq
        m_seq

    ///Operations: Skip, SkipWhile
    member private x.skip  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        let n' = n |> Math.intSqrt
        let rnd = rnd.SpecialRecipe n' >> fun x -> max 0 (s.Length - x)
        let mutable m_seq = []
        for i = 0 to n' do
            let res1 = s.U_skip(rnd())
            let l2=rnd()
            m_seq <- res1::m_seq
        m_seq

    ///Operations: Find(Last), Find(Last)Index, Pick, Count, Fold(Back), AddLast
    member private x.find n (s : SeqWrapper<int>) =
        let rnd = Random(seed)
        
        let n' = n |> Math.intSqrt
        let rnd = rnd.SpecialRecipe n'
        let mutable t = s.Empty
        for i = 0 to n' do
            let at = min (s.Length - 1) (rnd())
            let v = s.[at]
            let a = s.Test_predicate (fun x -> x = v)
            let b = s.Test_predicate (fun v -> true)
            let c = s.Test_predicate (fun v -> false)
            t <- [a;b;c] |> Seq.choose id |> Seq.fold (fun st cur -> st.AddLast cur) t
        t

    ///Operations: Slice
    member private x.slices  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        
        let mutable m_seq = []
        let n' = n |> Math.intSqrt
        let lenRnd = rnd.SpecialRecipe n'
        let mutable s = s
        for i = 0 to n' do
            let a, b = rnd.IntInterval(0,s.Length - 1, lenRnd())
            let slice = s.U_slice (a,b)
            m_seq <- slice::m_seq
        m_seq

    member private x.slices_concat_all n (s : SeqWrapper<int>) =
        let rnd = Random(seed)
        let mutable l = []
        let n' = n |> Math.intSqrt
        let mutable s = s
        let tests() = SeqTests(rnd.Next())
        for i = 0 to n' do
            let a, b = rnd.IntInterval(0,s.Length - 1)
            let c, d = rnd.IntInterval(0, s.Length - 1)
            let slice1, slice2 = s.U_slice(a, b), s.U_slice(c,d)
            let r = slice1.U_add_last_range slice2
            let r2 = r.U_add_first_range slice1
            let add = r2 |> tests().add_remove_last_first_limited n
            s <- add
        s

    ///Operations: Reverse
    member private x.reverse n (s : SeqWrapper<int>) =
        let rnd = Random(seed)
        let mutable l = []
        let n' = n |> Math.intSqrt
        let tests() = SeqTests(rnd.Next())
        let mutable s = s
        let lenRnd = rnd.SpecialRecipe n
        for i = 0 to n' do
            let a, b =  rnd.IntInterval(0, s.Length-1, lenRnd())
            s <- s.[a, b]
            let next = s |> tests().add_last n'
            s <- next.U_reverse
        s

    ///Operations: InsertRange 
    member private x.insert_range  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let n' = (n |> Math.intSqrt) / 5
        let rnd() = r.Next(0,n')
        let mutable s = s
        let tests() = SeqTests(r.Next())
        let nRnd = r.SpecialRecipe n'
        for i = 0 to n' do  
            let n = nRnd()
            let cols = tests().gen_collections n s
            for col in cols do
                s <- s.U_insert_range (rnd()) (col.Value)
        s
    
    ///Operations: RemoveAt
    member private x.remove_at n (s : SeqWrapper<int>) = 
        let mutable s = s
        let rnd = Random(seed)
        let mutable loop = true
        let mutable i = 0
        let n = n / 2
        while loop && i < n do
            match s.Length with
            | 0 -> 
                s.U_ex_is_empty
                loop <- false
            | 1 -> s.U_ex_has_one_element
            | _ -> s <- s.U_remove(rnd.Next(0,s.Length))
            i <- i + 1
        s

    ///Operations: Insert, RemoveAt, Update
    member private x.insert_remove_update  n (s : SeqWrapper<int>) = 
        let r = Random(seed)
        let mutable s = s
        let tests() = SeqTests(r.Next())
        let n' = (n |> Math.intSqrt) / 4
        let rnd = r.SpecialRecipe n'
        for i = 0 to n' do
            s <- s |> tests().insert (rnd())
            s <- s |> tests().remove_at (rnd())
            s <- s |> tests().update (rnd())
            s <- s |> tests().insert_range (rnd())
        s

    ///Operations: AddLast(Range), RemoveLast, AddFirstRange, InsertRange, 
    member private x.complex_add_remove_first_last_limited n (s : SeqWrapper<int>) = 
        let mutable s = s
        let lower_n = (n |> Math.intSqrt) / 8
        let r = Random(seed)
        let getIters = r.SpecialRecipe lower_n
        let tests() = SeqTests(r.Next()) 
        for i = 0 to lower_n do
            s <- s |> tests().add_remove_last (getIters())
            s <- s |> tests().add_remove_last_first_limited (getIters())
            s <- s |> tests().add_last_range(getIters())
            s <- s |> tests().add_last (getIters())
            s <- s |> tests().remove_last (getIters())
            s <- s |> tests().add_remove_last(getIters())
            s <- s |> tests().add_last_range(getIters())
            s <- s |> tests().add_remove_last(getIters())
        s

    member private x.complex_add_remove_first_last n (s : _ SeqWrapper)=
        let mutable s = s
        let lower_n = (n |> Math.intSqrt) / 6
        let r = Random(seed)
        let getIters = r.SpecialRecipe lower_n
        let tests() = SeqTests(r.Next())
        for i = 0 to lower_n do
            s <- s |> tests().add_remove_last(getIters())
            s <- s |> tests().add_remove_first(getIters())
            s <- s |> tests().add_remove_first_last(getIters())
            s <- s |> tests().add_first_range (getIters())
            s <- s |> tests().add_last_range (getIters())
            s <- s |> tests().add_last (getIters())
        s
    member private x.complex_add_last_slices_lookup_update n (s : _ SeqWrapper) = 
        let mutable s = s
        let lower_n = (n |> Math.intSqrt) / 6
        let r = Random seed
        let getIters = r.SpecialRecipe lower_n
        let mutable mSeq = []
        let tests() = SeqTests(r.Next())
        for i = 0 to lower_n do
            s <- s |> tests().complex_add_remove_first_last_limited (getIters()) 
            s <- s |> tests().update (getIters()) 
            s <- s |> tests().add_remove_last (getIters())
            s <- s |> tests().update (getIters()) 
            s <- s |> tests().add_last_range (getIters())
            let q = s |> tests().get_index (getIters())
            let take = s |> tests().take (getIters())
            mSeq <- s::q::take @ mSeq
        mSeq

    member private x.complex_add_first_last_slices_all_indexing n (s : _ SeqWrapper) = 
        let mutable s = s
        let lower_n = (n |> Math.intSqrt) / 8
        let r = Random seed
        let rnd = r.SpecialRecipe lower_n
        let mutable mSeq = []
        for i = 0 to lower_n do
            let tests() = SeqTests(r.Next())
            s <- s |> tests().complex_add_remove_first_last (rnd()) 
            s <- s |> tests().update (rnd()) 
            s <- s |> tests().add_remove_last (rnd())
            s <- s |> tests().update (rnd()) 
            s <- s |> tests().add_first_range (rnd())
            s <- s |> tests().insert_remove_update (rnd()) 
            s <- s |> tests().insert_range (rnd())
            let res = s |> tests().slices (rnd())
            s <- res.Head
            mSeq <- s::mSeq @ res
        mSeq


    member x.Add_last iters = create_test iters "AddLast" (x.add_last iters >> toList1) 
    member x.Add_first n = create_test n "AddFirst" (x.add_first n >> toList1 )
    member x.Add_first_last n = test_of n "Add first last" (x.add_first_last n >> toList1 )
    member x.Add_remove_last n = test_of n "Add remove last" (x.add_remove_last n >> toList1 )
    member x.Add_remove_first n = test_of n "Add remove first" (x.add_remove_first n >> toList1 )
    member x.Add_remove_first_last n = test_of n "Add remove first last" (x.add_remove_first_last n >> toList1 )
    member x.Add_remove_last_first_limited n = test_of n "Add remove last, first limited" (x.add_remove_last_first_limited n >> toList1 )
    member x.Add_last_range n = test_of n "Add last range" (x.add_last_range n >> toList1 )
    member x.Add_first_range n = test_of n "Add first range" (x.add_first_range n >> toList1 )
    member x.Get_index n = test_of n  "Lookup by index" (x.get_index n >> toList1)
    member x.Update n = test_of n "update by index" (x.update n >> toList1 )
    member x.Insert n = test_of n "Insert at index" (x.insert n >> toList1 )
    member x.Remove_add_last n = test_of n "Remove add" (x.remove_at n >> toList1)
    member x.Take n = test_of n "Take count" (x.take n >> List.cast )
    member x.Skip n = test_of n "Skip count" (x.skip n >> List.cast)
    member x.Slices n = test_of n "Slices" (x.slices n >> List.cast)
    member x.Insert_remove_update n = test_of n "Insert, remove, update" (x.insert_remove_update n >> toList1)
    member x.Insert_range n = test_of n "Insert range" (x.insert_range n >> toList1)
    
    member x.Complex_add_remove_last n = test_of n "Complex add/remove last sequence" (x.complex_add_remove_first_last_limited n >> toList1)
    member x.Complex_add_remove_first_last n = test_of n "Complex add/remove first/last sequence" (x.complex_add_remove_first_last n >> toList1) 
    member x.Complex_add_last_slices_indexing n = test_of n "Complex add/remove last + slice + update/indexing" (x.complex_add_last_slices_lookup_update n >> List.cast)
    member x.Complex_add_first_last_take_slices_indexing n = test_of n "Complex add/remove first/last + update/indexing + slices" (x.complex_add_first_last_slices_all_indexing n >> List.cast)
    member x.Find n = test_of n "All predicate tests" (x.find n >> toList1)
    member x.Reverse n = test_of n "Reverse" (x.reverse n >> toList1)