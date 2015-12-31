 ///A module that contains helper functions and methods.
[<AutoOpen>]
module Funq.FSharp.Implementation.Helpers
open System.CodeDom.Compiler
open System
open System.Collections.Generic
open Funq.FSharp
open System.Diagnostics

type IndentedTextWriter with
    member x.Push() = x.Indent <- x.Indent + 1
    member x.Pop() = x.Indent <- x.Indent - 1    

type Option<'v> with
    member x.Map f = x |> Option.map f
    member x.OrMaybe y = if x.IsSome then x else y
    member x.Or(v : 'v) = match x with Some v -> v | None -> v
    
type Ref<'v> with
    member x.On f = x.Value <- f(x.Value)
       
///The "placeholder" operator. Partially invokes a function, skipping the first parameter and letting you specify the second one.
let (@?) (f : 'a -> 'b -> 'c) (b : 'b) = fun a -> f a b

///The double "placeholder" operator. Partially invokes a function, skipping the first two parameters and letting you specify the third one.
let (@?@?) (f : 'a -> 'b -> 'c -> 'd) (c : 'c) = fun a b -> f a b c

///The triple "placeholder" operator. Partially invokes a function, skipping the first three parameters and letting you specify the fourth one.
let (@?@?@?) (f : 'a -> 'b -> 'c -> 'd -> 'e) (d : 'd) = fun a b c -> f a b c d

module Fun = 
    let apply1 a f = f a
    let apply2 a b f = f a b

    let compose1 g f = f >> g
    let compose2 g f = fun a b -> (f a b) |> g
    let compose3 g f = fun a b c -> (f a b c) |> g
    let compose4 g f = fun a b c d-> (f a b c d) |> g
    let compose5 g f = fun a b c d e -> (f a b c d e) |> g

    ///Takes a function taking an integer (assumed to be an index) and a value, and returns a function without the integer.
    ///The integer value that is supplied is the number of times that function instance is called.
    let indexed (f : int -> 'v -> 'out) : 'v -> 'out = 
            let i = ref 0
            fun x -> 
                let r=  f !i x
                i := !i + 1
                r

///The 2-composition operator. Composes a curried function taking two arguments (left operand) with another function (right operand).
let (.>>) f g = f |> Fun.compose2 g

///The 3-composition operator. Composes a curried function taking three arguments (left operand) with another function (right operand).
let (..>>) f g = f |> Fun.compose3 g

///The 4-composition operator. Composes a curried function taking four arguments (left operand) with another function (right operand).
let (...>>) f g = f |> Fun.compose4 g

///The 5-composition operator. Composes a curried function taking five arguments (left operand) with another function (right operand).
let (....>>) f g = f |> Fun.compose5 g

module Math = 
    let root n x = Math.Pow(float x, 1./(float n))
    let intRoot n x = x |> root n |> int
    let intSqrt x = x |> intRoot 2

module Seq =
    type IterationControl<'v> = 
    | Stop
    | Yield of 'v
    | Ignore
    let getEnumerator (a :_ seq) = a.GetEnumerator()

    let rev (s :_ seq) =
        let mutable list = []
        for item in s do list <- item::list
        list :> _ seq

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
    let toArrayDirty (s : 'v seq) =
        match s with
        | :? array<'v> as arr -> arr
        | _ -> s |> Seq.toArray

    let equalsWith areEqual a b = 
        let rec iter (it1 :_ Iter) (it2 : _ Iter) = 
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

             
    ///Similar to Seq.choose, except that three types of indicators may be returned: Yield and Ignore, which function as Some and None respectively, and Stop which stops iteration.
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
    ///You can specify a maximum number of duplicates in a row before iteration stops. Otherwise, may get stuck in an infinite loop if given an infinite sequence.
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
    ///Stops iterating if it has iterated over 10,000,000 duplicate elements in a row. Use lazyDistinct' if you don't like this behavior.
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
    let sections n  (s : 't seq)= 
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
    
module List = 
    let correlate (left : 'v list, right : 'v list) (key : 'v -> 'k when 'k : equality) =
        let dict = Dictionary()
        for item in right do
            dict.[key item] <- item

        let mutable result = []
        for item in left do
            let k = key item
            if dict.ContainsKey k then
                result <- (item,dict.[k])::result
        result

    let zipAll (lsts : #seq<#seq<_>>) = 
        let cols = lsts |> Seq.toArray
        let iterators = cols |> Array.map (Seq.getEnumerator)
        [while iterators |> Array.forall (fun x -> x.MoveNext()) do
            yield iterators |> Array.mapi (fun i x -> cols.[i],x.Current) |> Array.toList]

    let ofType<'input, 'output> (lst : 'input list) = 
        let filter x = 
            match x :> obj with
            | :? 'output as out -> Some out
            | _ -> None
        lst |> List.choose filter

    let cast<'a, 'b> (lst : 'a list) = lst |> List.map (fun x -> x :> obj :?> 'b)

    ///Cross/cartesian product over lists.
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = [for item1 in seq1 do for item2 in seq2 -> item1,item2]

    let singleton x = [x]

    let apply1 arg fs = fs |> List.map (Fun.apply1 arg)
    let apply2 arg1 arg2 fs = fs |> List.map (Fun.apply2 arg1 arg2)

    let cross_apply1 (args : _ list) (fs : _ list) = fs |> Seq.cross_apply1 args |> Seq.toList
    let chain_iter f vs = 
        vs |> List.iter f
        vs

module String = 
    let contains caseSensitive what (where : string) = 
        if caseSensitive then 
            where.Contains(what) 
        else 
            where.ToUpperInvariant().Contains(what.ToUpperInvariant())

    let joinWith delim toString (items : #seq<_>) = String.Join(delim, items |> Seq.map toString)
    let join delim (items : #seq<_>) = joinWith delim (sprintf "%O") items
    let joinWordsWith delim1 delim2 f (items : #seq<_>) = 
        let arr = items |> Seq.toArray
        match arr.Length with
        | 0 -> ""
        | 1 -> arr.[0] |> f
        | n -> 
            let mutable joined = ""
            for i = 0 to n - 2 do
                joined <- joined + (f arr.[i]) + delim1
            joined + delim2 + (f arr.[n-1])
    let split (delim : string seq) (str : string) = str.Split(delim |> Seq.toArray, StringSplitOptions.RemoveEmptyEntries)
    let containsAny caseSensitive what (where : string) = 
        what |> Seq.exists (contains caseSensitive @? where)
    let containsAll caseSensitive what (where : string) = what |> Seq.forall (contains caseSensitive @? where)
    let ofChars chars = String(chars |> Seq.toArrayDirty)
    let initChars count f = String.init count (f >> string)

type String with
    member x.ContainsAny caseSensitive strs = 
        x |> String.containsAny caseSensitive strs
    member x.ContainsAll caseSensitive strs = 
        x |> String.containsAll caseSensitive strs

module Option = 
    let orValue (v : 'v) = function Some u -> u | None -> v
    let cast<'a,'b> (opt : 'a option) : 'b option = opt |> Option.map (fun a -> a :> obj :?> 'b)
    let asNull (opt : 'a option) = if opt.IsSome then opt.Value else null

module Chars = 
    let alphanumeric = ['A' .. 'Z'] @ ['a' .. 'z'] @ ['0' .. '9']

type Random with
    member x.ExpDouble max = 
        let scale = x.NextDouble() * 2. - 1.
        let log = log max
        exp (scale * log)
    member x.IntInterval(mn,mx,?maxLen) = 
        let st = x.Next(mn, mx + 1)
        let maxEn = if maxLen.IsNone then mx + 1 else min (mx + 1) (mn + maxLen.Value)
        let en = x.Next(st, maxEn)
        st,en
    member x.IntByLength(min,max) =
        let min = pown 2 min
        let max = pown 2 max
        x.Next(min,max)
    member x.ElementOf(sq) = 
        let arr = sq |> Seq.toArrayDirty
        arr.[x.Next(0, arr.Length)]
    member x.String len  =
        fun (charPool : #seq<char>) ->
            let charPool = charPool |> Seq.toArrayDirty
            let str = String.initChars len (fun i -> x.ElementOf charPool)
            str
    member x.String (min,max) : (#seq<char> -> string) = 
        x.Next(min,max) |> x.String
    member x.Double(min,max) = 
        min + x.NextDouble() * (max - min)

[<AutoOpen>]
module Type = 
    type Type with 
        member x.JustTypeName() =
            let name = x.Name
            let indexOf = name.IndexOf("`", StringComparison.InvariantCulture)
            if indexOf < 0 then name else name.Substring(0, indexOf)
        member x.PrettyName() = 
            if x.GetGenericArguments().Length = 0 then
                x.Name
            else
                let args = x.GetGenericArguments()
                let name = x.JustTypeName()
                sprintf "%s<%s>" name (args |> Seq.map (fun x -> x.PrettyName()) |> fun strs -> String.Join(",", strs))
    
