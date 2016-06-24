module ExtraFunctional.Seq
open ExtraFunctional
open System.Collections.Generic
open System.Collections
open System.Linq
type IterationControl<'v> = 
| Stop
| Yield of 'v
| Ignore

let getEnumerator (a :_ seq) = a.GetEnumerator()

let rev (s :_ seq) =
    let mutable list = []
    for item in s do list <- item::list
    list :> _ seq

let startsWith (what :_ seq) (input :_ seq) = 
    use whatIter = what.GetEnumerator()
    use inputIter = input.GetEnumerator()
    let rec f() =
        match whatIter.MoveNext(), inputIter.MoveNext() with
        | false, _ -> true
        | true, false -> false
        | true, true -> whatIter.Current = inputIter.Current && f()
    f()

let endsWith (what:'a seq) (input:'a seq when 'a : equality) =
    let whatList = what |> List.ofSeq
    input.Take(whatList.Length).SequenceEqual(whatList)

let takeLast count (input :_ seq) =
    use iter = input.GetEnumerator()
    let mList = List<_>()
    while iter.MoveNext() do
        mList.Add(iter.Current)
        if mList.Count > count then
            mList.RemoveAt 0
    mList |> seq

let tryFindLast f (s :_ seq) =
    let found = ref None
    let f' x = 
        if f x then 
            found := Some x
            false
        else
            true
    s |> rev |> Seq.forall(f') |> ignore
    found.Value


let tryFindLastIndex f (s:_ seq) =
    let found = ref None
    let mutable lst = []
    for item in s do lst <- item::lst
    let len = lst.Length
    let f' i x = 
        if f x then
            found := Some i
            false
        else true
    lst |> Seq.forall (Fun.indexed  f') |> ignore
    found.Value.Map(fun x -> len - 1 - x)

let toObservableList sq = 
    ObservableList(sq : _ seq)

let foldBack init f (s:_ seq) =
    s |> rev |> Seq.fold f init

let reduceBack f (s:_ seq) =
    let fold st cur = 
        match st with
        | Some v -> f v cur |> Some
        | None -> Some cur
    s |> foldBack None fold |> Option.get

let tryLast (s :_ seq) = 
    if s |> Seq.isEmpty then None else s |> Seq.last |> Some

///Converts a sequence to an array, but can take shortcuts (such as returning the argument if it's already an array) that make it a bad idea to modify or store the resulting array.
let asArray (s : 'v seq) =
    match s with
    | :? array<'v> as arr -> arr
    | _ -> s |> Seq.toArray

let equalsWith areEqual a b = 
    let rec iter (it1 : IEnumerator<_>) (it2 : IEnumerator<_>) = 
        match it1.MoveNext(), it2.MoveNext() with
        | true, true ->
                let eq = areEqual(it1.Current)(it2.Current)
                if not eq then 
                    let blah = 4
                    ()
                eq && iter it1 it2
        | false, false -> true
        | _ -> 
            false
    iter (a |> getEnumerator) (b |> getEnumerator)
let equals a b = equalsWith (fun a b -> obj.Equals(a,b)) a b

             
///Similar to Seq.choose, except that four types of indicators may be returned: Yield and Ignore, which function as Some and None respectively, and Stop which stops iteration.
let chooseCtrl f sq = 
    seq {
        use iterator = sq |> getEnumerator
        let keep = ref true
        while !keep && iterator.MoveNext() do
            match f iterator.Current with
            | Stop -> keep := false
            | Yield v -> yield v
            | Ignore -> ()
    }

let scanReduce f sq = 
    sq |> Seq.scan (fun st cur -> match st with Some(v) -> f v cur |> Some | None -> cur |> Some) None |> Seq.skip 1 |> Seq.map (Option.get)

///Similar to Seq.fold, except that three types of indicators may be returned: Yield, which updates the state, Ignore which does not, and Stop which stops iteration altogether and returns the latest state.
let foldCtrl state f sq = 
    let state = ref state
    let fold cur = 
        match f !state cur with
        | Stop -> false
        | Yield v -> 
            state := v
            true
        | Ignore -> true
    sq |> Seq.forall fold


///Similar to seq.distinct, but works lazily, allowing it to be used on infinite sequences. The performance is reduced as a result.
///You can specify a maximum number of duplicates in a row before iteration stops. Otherwise, may get stuck in an infinite loop 
let lazyDistinct' (maxDuplicates : int option) sq = 
    seq {
        let mSet = System.Collections.Generic.HashSet<_>()
        let duplicates = ref 0
        let choose item = 
            if maxDuplicates.IsSome && !duplicates >= maxDuplicates.Value then
                Stop
            elif mSet.Contains item then
                duplicates.On((+) 1)
                Ignore
            else 
                mSet.Add(item) |> ignore
                duplicates := 0
                Yield item
        yield! (sq |> chooseCtrl choose)
    }
        
///Similar to seq.distinct, but works lazily, allowing it to be used on infinite sequences. The performance is reduced as a result.
///Throws an exception if it has iterated over 1,000,000 duplicate elements in a row. Use lazyDistinct' if you don't like this behavior.
let lazyDistinct sq = lazyDistinct' (pown 10 7 |> Some) sq

///Cross/cartesian product over sequences, returned in order of 
let cross (seq2 : #seq<_>) (seq1 : #seq<_>) = 
    seq {
        let seq2 = seq2 |> Seq.cache
        for item1 in seq1 do for item2 in seq2 -> item1,item2
    }

let ofType<'t> (s : System.Collections.IEnumerable) = 
        seq {
            for item in s do
                match item with
                | :? 't as typed -> yield typed
                | _ -> ()
        }

///Cuts the sequence up into sections of the specified size, each returned as a sequence.
let sections n  (s : 't seq) = 
    seq {
        let m_list = System.Collections.Generic.List()
        for item in s do
            if m_list.Count >= n then
                yield m_list |> seq
                m_list.Clear()
            m_list.Add(item)
        if m_list.Count > 0 then
            yield m_list |> seq
    }

///Tries to get the single element in the sequence. If there are no elements, returns None. If there is more than one element, throws an exception.
let trySingle (sq : _ seq) = 
    let mutable v = None
    match sq |> Seq.length with
    | 0 -> None
    | 1 -> sq |> Seq.head |> Some
    | _ -> invalidArg "sq" "The sequence has more than one element!"

///When given a sequence of functions 'fs', applies those functions to 'arg', returning a sequence of the results.
let apply1 arg fs = fs |> Seq.map (Fun.apply1 arg)
///When given a sequence of arguments and a sequence of functions, cross-applies all functions to all arguments,
///returning a sequence ordered by 
let cross_apply1 args fs = fs |> cross args |> Seq.map (fun (f,arg) -> f arg)

module Patterns = 
    
    let (|Equals|_|) (input:_ seq) (what:_ seq) =
        if input |> equals what then Some() else None

    let (|First|_|) (input:_ seq) count =
        let take = input.Take(count) |> Seq.toList
        if take.Length = count then Some take else None


    let (|StartsWith|_|) (input : _ seq) (what : _ seq) = 
        if input |> startsWith what then Some() else None

    let (|EndsWith|_|) (input:_ seq) (what:_ seq) =
        if input |> endsWith what then Some() else None

    let (|Cast|_|) input = 
        let orig = input |> Seq.toList
        let typed = orig |> List.choose (fun x -> match x :> obj with :? 't as x -> Some x | _ -> None)
        if orig.Length = typed.Length then Some typed else None