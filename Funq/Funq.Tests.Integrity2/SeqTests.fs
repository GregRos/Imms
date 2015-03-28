namespace Funq.Tests.Integrity
open System
open System.Diagnostics
open Funq.FSharp.Implementation
open Funq.FSharp

type SeqTests(?seed : int) = 
    let create_test iters name test = Test(iters, name, ListLike, test)
    let seed = defaultArg seed (Environment.TickCount)
    let test_of n name (test : _ -> #TargetMetadata list) = create_test n name test
    member private x.inner_add_last (n : int) (s : SeqWrapper<int>) = 
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next()
        let initCount = s.Length
        let initFirst = s.TryFirst|> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        for i = 0 to n do
            let v = rnd()
            s <- s.AddLast(v)
            assert_eq(s.Last, v)
            assert_eq(s.First, initFirst)
            assert_eq(s.[initCount+i], v)
            assert_eq(s.[initCount - 1], initLast)
            assert_eq(s.[-1], v)
            assert_eq(s.[-initCount - i - 1], initFirst)
            assert_eq(s.TryLast.Value,v)
            assert_eq(s.TryFirst.Value, initFirst)
            assert_eq(s.Length, initCount + i + 1)
        s
    member private x.inner_add_first n (s : int SeqWrapper) = 
        let mutable s = s
        let r = Random(seed)
        let rnd() = r.Next()
        let initCount = s.Length
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        for i = 0 to n do
            let v = rnd()
            s <- s.AddFirst(v)
            assert_eq(s.First, v)
            assert_eq(s.Last, initLast)
            assert_eq(s.[0], v)
            assert_eq(s.[initCount + i], initLast)
            assert_eq(s.[-1], initLast)
            assert_eq(s.[-initCount - i - 1], v)
            assert_eq(s.TryLast.Value,initLast)
            assert_eq(s.TryFirst.Value, v)
            assert_eq(s.Length, initCount + i + 1)
        s
    member private x.inner_add_last_range n (s : int SeqWrapper) = 
        let initCount = s.Length
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        let mutable s = s
        for i = 0 to n do
            let v = rnd()
            s <- s.AddLastRange [0 .. v]
            assert_eq(s.Last, v)
            assert_eq(s.First, initFirst)
            assert_eq(s.[initCount - 1], initLast)
            assert_eq(s.[-1], v)
            assert_eq(s.TryLast.Value,v)
            assert_eq(s.TryFirst.Value, initFirst)
        s
    member private x.inner_add_first_range  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        let mutable s = s
        for i = 0 to n do
            let v = rnd()
            s <- s.AddFirstRange [-v .. 0]
            assert_eq(s.First, -v)
            assert_eq(s.Last, initLast)
            assert_eq(s.[0], -v)
            assert_eq(s.[-1], initLast)
            assert_eq(s.TryLast.Value, initLast)
            assert_eq(s.TryFirst.Value, -v)
        s
    member private x.inner_add_first_last  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let initCount = s.Length
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        for i = 0 to n do
            let last1, first1, last2, first2 = Fun.call_4 rnd ()
            s <- s.AddLast last1
            s <- s.AddFirst first1
            s <- s.AddLast last2
            s <- s.AddFirst first2
            assert_eq(s.Last, last2)
            assert_eq(s.First, first2)
            assert_eq(s.[(initCount + 4 * (i + 1) - 1)], last2)
            assert_eq(s.[0], first2)
            assert_eq(s.[-1], last2)
            assert_eq(s.[(-initCount - 4 *  (i + 1))], first2)
            assert_eq(s.TryLast.Value,last2)
            assert_eq(s.TryFirst.Value, first2)
            assert_eq(s.Length, initCount + (i + 1)*4)
        s    

    member private x.inner_add_drop_last  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let initCount = s.Length
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        for i = 0 to n do
            let last1, last2, last3, last4 = Fun.call_4 rnd ()
            let count1, count2, count3, count4 = Fun.call_4 rnd ()
            s <- s.AddLast last1
            assert_eq(s.Last, last1)
            s <- s.AddLast (last2)
            s <- s.DropLast() 
            s <- s.AddLastRange [|0 .. count1|]
            s <- s.DropLast()
            s <- s.AddLast last3
            s <- s.AddLastRange [0]
            s <- s.AddLastRange []
            s <- s.AddLast last4
            while s.Length > initCount do
                s <- s.DropLast()
            assert_eq(s.Length, initCount)
            s <- s.AddLastRange [|0 ..count2|]
            s <- s.AddLast i
            s <- s.DropLast()
            s <- s.AddLastRange [|0 .. count3|]
        s

    member private x.inner_add_drop_first  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let initFirst = s.TryFirst |> Option.orValue 0
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        let initLast = s.TryLast |> Option.orValue 0
        let mutable s = s
        for i = 0 to n do
            let first1, first2, first3, first4 = Fun.call_4 rnd ()
            s <- s.AddFirst first1
            assert_eq(s.First, first1)
            s <- s.AddFirst first2
            s <- s.DropFirst() 
            s <- s.AddFirstRange [| -first3 .. 0 |]
            s <- s.DropFirst()
            s <- s.AddFirst first4
            assert_eq(s.First, first4)
            s <- s.AddFirstRange [-first1]
            assert_eq(s.First, -first1)
            s <- s.AddFirstRange []
            assert_eq(s.First, -first1)
            s <- s.AddFirst first2
            assert_eq(s.First, first2)
            while s.Length > initCount do
                s <- s.DropFirst()
            assert_eq(s.Length, initCount)
            s <- s.AddFirstRange [| 0 .. first1 |]
            assert_eq(s.First, 0)
            s <- s.AddFirst first2
            assert_eq(s.First, first2)
            s <- s.DropFirst()
            assert_eq(s.First, 0)
            s <- s.AddFirstRange [| 0 .. 2 * first3|]
        s

    member private x.inner_add_drop_first_last  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let initCount = s.Length
        let initFirst = s.TryFirst |> Option.orValue 0
        let initLast = s.TryLast |> Option.orValue 0
        let r = Random(seed)
        let rnd() = r.Next(0, n |> float |> sqrt |> int)
        for i = 0 to n do
            let i_lower = Math.Sqrt(float i) |> int
            let first1, first2, first3, first4 = Fun.call_4 rnd ()
            let last1, last2, last3, last4 = Fun.call_4 rnd ()
            s <- s.AddFirst first1
            s <- s.AddFirst first2
            s <- s.DropFirst() 
            s <- s.AddFirstRange [|-first3 .. 0|]
            s <- s.AddLastRange [|0 .. last1|]
            s <- s.AddLast last2
            s <- s.DropFirst()
            s <- s.AddFirst first4
            s <- s.AddFirstRange [first1]
            s <- s.AddFirstRange []
            s <- s.AddLastRange []
            s <- s.AddFirstRange [first2]
            s <- s.DropLast()
            s <- s.DropFirst()
            s <- s.AddFirst last3
            let mutable t = s
            while s.Length > initCount do
                s <- s.DropFirst()

            while t.Length > initCount do
                t <- t.DropLast()
            s <- t.AddFirstRange(s)

            assert_eq(s.Length, 2*initCount)
            s <- s.AddFirstRange [|-first3 .. 0|]
            s <- s.AddFirst -first4
            s <- s.DropLast()
            s <- s.DropFirst()
            s <- s.AddFirst -first1
            s <- s.AddLastRange [|0 .. 2*last4|]
            s <- s.AddFirstRange [|-2*first2 .. 0|]
        s


    member private x.inner_get_index  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mlist = s.Empty
        let mutable t = s.Empty
        for i in Seq.init n (fun _ -> rnd.Next(0,initCount)) do
            t <- t.AddLast(s.[i])
        t

    member private x.inner_update  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mutable s = s
        for i = 0 to n do
            let ix = rnd.Next(0, s.Length)
            s <- s.Update(ix, rnd.Next(0, n))
            s <- s.AddLastRange ((fun t -> rnd.Next(0, t)) |> Seq.init n |> Seq.cache)
            s <- s.AddLast i
            s <- s.DropLast().DropLast()
        s

    member private x.inner_insert  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mutable s = s
        for i = 0 to n do
            let ix = rnd.Next(0, s.Length)
            let old_len = s.Length
            let v = rnd.Next(0, n)
            s <- s.Insert(ix, v)
            assert_eq(s.Length, old_len + 1)
            assert_eq(s.[ix], v)
        s

    member private x.inner_insert_remove_update  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mutable s = s
        for i = 0 to n do
            let ix = rnd.Next(0, s.Length)
            let ix2 = rnd.Next(0, s.Length)
            let ix3 = rnd.Next(0, s.Length)
            let v1 = rnd.Next(0, n)
            let v2 = rnd.Next(0,n)
            let v3 = rnd.Next(0,n)
            let old_len = s.Length
            s <- s.Insert(ix, v1)
            assert_eq(s.[ix], v1)
            assert_eq(s.Length, old_len + 1)
            let expected_value = if ix2 = s.Length - 1 then None else Some <| s.[ix2 + 1]
            s <- s.Remove(ix2)
            assert_eq(s.Length, old_len)
            if expected_value.IsSome then
                assert_eq(s.[ix2], expected_value.Value)
            s <- s.Update(ix3, v3)
            assert_eq(s.[ix3], v3)
            assert_eq(s.Length, old_len)
        s

    member private x.inner_take  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mutable m_seq = []
        for i = 0 to n do
            let n = rnd.Next(0,s.Length+1)
            let res = s.Take(n)
            m_seq <- res::m_seq
        m_seq

    member private x.inner_skip  n (s : SeqWrapper<int>) = 
        let initCount = s.Length
        let rnd = Random(seed)
        let mutable m_seq = []
        for i = 0 to n do
            let res = s.Skip(rnd.Next(0, s.Length + 1))
            m_seq <- res::m_seq
        m_seq

    member private x.inner_slices  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        let mutable m_seq = []
        let mutable s = s
        for i = 0 to n do
            let upTo = rnd.Next(0, s.Length)
            let from = rnd.Next(0, upTo + 1)
            let slice = s.[from, upTo]
            m_seq <- slice::m_seq
        m_seq

    member private x.inner_concat  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        let mutable m_seq = []
        let mutable s = s
        for i = 0 to n do
            let num_a = rnd.Next(0, n)
            let num_b = rnd.Next(0, n)
            let rnd_seq = Seq.initInfinite (fun _ -> rnd.Next(0, n))
            let a = s.AddLastRange (rnd_seq |> Seq.take num_a |> Seq.cache)
            let b = s.AddLastRange (rnd_seq |> Seq.take num_b |> Seq.cache)
            m_seq <- (a.AddLastRange(b.Inner).AddFirstRange(b.Inner))::m_seq
        m_seq

    member private x.inner_insert_range  n (s : SeqWrapper<int>) = 
        let rnd = Random(seed)
        let mutable s = s
        for i = 0 to n do
            let count = rnd.Next(0,n)
            let what = Array.init count (fun _ -> rnd.Next(0, n))
            let old_length = s.Length
            
            let where = rnd.Next(0, s.Length)
            let old_value = s.[where]
            s <- s.InsertRange(where, what)
            assert_eq(s.Length, old_length + count)
            if what.Length > 0 then 
                let v = s.[where]
                let w = s.[where + 1]
                assert_eq(s.[where], what.[0])
                assert_eq(s.[where + Seq.length what - 1], what.[what.Length - 1])
            else
                assert_eq(s.[where], old_value)
        s
    
    member private x.inner_insert_concat  n (s : SeqWrapper<int>) = 
        let mutable s = s
        let rnd = Random(seed)
        for i = 0 to n do
            let num = rnd.Next(0, n)
            let old_length = s.Length
            
            let ix = rnd.Next(0, s.Length)
            let old_v = s.[ix]
            let rnd_seq = Seq.init num (fun _ -> rnd.Next(0, n)) |> Seq.cache
            let of_seq = s.Empty.AddLastRange rnd_seq
            s <- s.InsertRange(ix, of_seq.Inner)
            assert_eq(s.Length, old_length + of_seq.Length)
            if of_seq.Length > 0 then
                assert_eq(s.[ix], of_seq.[0])
                assert_eq(s.[ix + of_seq.Length - 1], of_seq.[of_seq.Length - 1])
            else
                assert_eq(s.[ix], old_v)
        s

    member private x.inner_complex_add_drop_last n (s : SeqWrapper<int>) = 
        let mutable s = s
        let lower_n = n |> float |> sqrt |> int
        let r = Random(seed)
        let rnd() = r.Next(0,lower_n)
        for i = 0 to rnd() do
            let new_test_obj() = SeqTests(r.Next())
            s <-  s |> new_test_obj().inner_add_drop_last (rnd()) |> new_test_obj().inner_add_last_range(rnd()) |> new_test_obj().inner_add_last (rnd())
            s <- s |> new_test_obj().inner_add_drop_last (rnd()) |> new_test_obj().inner_add_last_range (rnd()) |> new_test_obj().inner_add_drop_last (rnd())
        s

    member private x.inner_complex_add_drop_first_last n (s : _ SeqWrapper)=
        let mutable s = s
        let lower_n = (n |> float |> sqrt |> int) / 2
        let r = Random(seed)
        let rnd() = r.Next(0,lower_n)
        for i = 0 to rnd() do
            let new_test_obj() = SeqTests(r.Next())
            s <- s |> new_test_obj().inner_add_drop_last (rnd()) |> new_test_obj().inner_add_drop_first(rnd()) |> new_test_obj().inner_add_drop_first_last (rnd())
            s <- s |> new_test_obj().inner_add_first_range (rnd()) |> new_test_obj().inner_add_last_range (rnd()) |> new_test_obj().inner_add_last (rnd())
            s <- s |> new_test_obj().inner_add_first_last (rnd())
        s
    member private x.inner_complex_add_last_take_and_indexing n (s : _ SeqWrapper) = 
        let mutable s = s
        let lower_n = (n |> float |> sqrt |> int) / 2
        let r = Random seed
        let rnd() = r.Next(0,lower_n)
        let mutable mSeq = []
        for i = 0 to rnd() do
            let new_test_obj() = SeqTests(r.Next())
            s <- s |> new_test_obj().inner_complex_add_drop_last (rnd()) |> new_test_obj().inner_update (rnd()) |> new_test_obj().inner_add_drop_last (rnd())
            s <- s |> new_test_obj().inner_update (rnd()) |> new_test_obj().inner_add_last_range (rnd())
            let q = s |> new_test_obj().inner_get_index (rnd())
            let take = s |> new_test_obj().inner_take (rnd())
            mSeq <- s::q::take @ mSeq
        mSeq

    member private x.inner_complex_all_operations n (s : _ SeqWrapper) = 
        let mutable s = s
        let lower_n = (n |> float |> sqrt |> int) / 2
        let r = Random seed
        let rnd() = r.Next(0,lower_n)
        let mutable mSeq = []
        for i = 0 to rnd() do
            let new_test_obj() = SeqTests(r.Next())
            s <- s |> new_test_obj().inner_complex_add_drop_first_last (rnd()) |> new_test_obj().inner_update (rnd()) |> new_test_obj().inner_add_drop_last (rnd())
            s <- s |> new_test_obj().inner_update (rnd()) |> new_test_obj().inner_add_first_range (rnd())
            s <- s |> new_test_obj().inner_insert_remove_update (rnd()) |> new_test_obj().inner_insert_range (rnd())
 
            let q = s |> new_test_obj().inner_get_index (rnd())
            let take = s |> new_test_obj().inner_take (rnd())
            mSeq <- s::q::mSeq @ take
        mSeq

    member x.add_last iters = create_test iters "AddLast" (x.inner_add_last iters >> toList1) 
    member x.add_first n = create_test n "AddFirst" (x.inner_add_first n >> toList1 )
    member x.add_first_last n = test_of n "Add first last" (x.inner_add_first_last n >> toList1 )
    member x.add_drop_last n = test_of n "Add drop last" (x.inner_add_drop_last n >> toList1 )
    member x.add_drop_first n = test_of n "Add drop first" (x.inner_add_drop_first n >> toList1 )
    member x.add_drop_first_last n = test_of n "Add drop first last" (x.inner_add_drop_first_last n >> toList1 )
    member x.add_last_range n = test_of n "Add last range" (x.inner_add_last_range n >> toList1 )
    member x.add_first_range n = test_of n "Add first range" (x.inner_add_first_range n >> toList1 )
    member x.get_index n = test_of n  "Lookup by index" (x.inner_get_index n >> toList1)
    member x.update n = test_of n "update by index" (x.inner_update n >> toList1 )
    member x.insert n = test_of n "Insert at index" (x.inner_insert n >> toList1 )
    member x.take n = test_of n "Take count" (x.inner_take n )
    member x.skip n = test_of n "Skip count" (x.inner_skip n)
    member x.concat n = test_of n "Concat" (x.inner_concat n)
    member x.slices n = test_of n "Slices" (x.inner_slices n)
    member x.insert_remove_update n = test_of n "Insert, remove, update" (x.inner_insert_remove_update n >> toList1)
    member x.insert_range n = test_of n "Insert range" (x.inner_insert_range n >> toList1)
    member x.insert_range_concat n = test_of n "Insert range concat" (x.inner_insert_concat n >> toList1)
    member x.complex_add_drop_last n = test_of n "Complex add/drop last sequence" (x.inner_complex_add_drop_last n >> toList1)
    member x.complex_add_drop_first_last n = test_of n "Complex add/drop first/last sequence" (x.inner_complex_add_drop_first_last n >> toList1) 
    member x.complex_add_last_take_and_indexing n = test_of n "Complex add/drop last + update/indexing" (x.inner_complex_add_last_take_and_indexing n)
    member x.complex_add_and_take_and_indexing n = test_of n "Complex add/drop first/last + update/indexing" (x.inner_complex_all_operations n)