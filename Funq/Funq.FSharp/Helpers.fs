///A module that contains helper functions and methods.
[<AutoOpen>]
module Funq.FSharp.Implementation.Helpers
open System.CodeDom.Compiler
open System
///A module with some extra sequence processing functions.
let inline (^*) (a : int) (b : int) = pown a b


type IndentedTextWriter with
    member x.Push() = x.Indent <- x.Indent + 1
    member x.Pop() = x.Indent <- x.Indent - 1    

module MList = 
    open System.Collections.Generic
    let ofList fsList = 
        let mList = List()
        mList.AddRange(fsList)
        mList
    let empty<'a> = List<'a>()
    let length (l : _ List) = l.Count

module Fun = 
    let apply1 a f = f a
    let apply2 a b f = f a b
    let call_2 f arg= f arg,f arg
    let call_3 f arg= f arg,f arg, f arg
    let call_4 f arg = f arg,f arg,f arg,f arg
    let delay1 f arg = fun () -> f arg
    let delay2 f arg1 arg2 = fun () -> f arg1 arg2
    let delay3 f arg1 arg2 arg3 = fun () -> f arg1 arg2 arg3

module String = 
    let contains what (where : string) = where.Contains(what)
    let contains_insensitive (what : string) (where : string) = where.ToUpperInvariant().Contains(what.ToUpperInvariant())

module Seq =
    let print (separator : string) (sq : #seq<'a>) = String.Join(separator, sq)

    let printWith separator (toString : 'b -> string) (sq : #seq<'b>)  = String.Join(separator, sq |> Seq.map toString)
       
    let printWordsWith f sq =
        let arr = sq |> Seq.toArray
        match arr.Length with
        | 0 -> ""
        | 1 -> arr.[0] |> f
        | n -> 
            let mutable joined = ""
            for i = 0 to n - 2 do
                joined <- joined + (f arr.[i]) + ", "
            joined + "and " + (f arr.[n-1])
      
    let printWords (sq : #seq<'b>) = printWordsWith (fun v -> v.ToString()) sq      
        
    ///Cross/cartesian product over sequences.
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = seq {for item1 in seq1 do for item2 in seq2 -> item1,item2}
    ///Maps a function taking 2 parameters over a sequence of tuples.
    let mapPairs f = Seq.map (fun (a,b) -> f a b)
    let ofType<'t> (s : System.Collections.IEnumerable) = 
            seq {
                for item in s do
                    match item with
                    | :? 't as typed -> yield typed
                    | _ -> ()
            
            }

    let takeIter  n  (s : 't seq)= 
        let m_list = System.Collections.Generic.List()
        seq {
            for item in s do
                if m_list.Count >= n then
                    yield m_list |> seq
                    m_list.Clear()
                m_list.Add(item)
            if m_list.Count > 0 then
                yield m_list |> seq
        }

    let singleOrNone (s : _ seq) = 
        let mutable v = None
        match s |> Seq.length with
        | 0 -> None
        | 1 -> s |> Seq.head |> Some
        | _ -> failwith "More than 1 element"
    let apply1 arg fs = fs |> Seq.map (Fun.apply1 arg)
    let cross_apply1 args fs = cross fs args |> Seq.map (fun (f,arg) -> f arg)
    
type 'a Microsoft.FSharp.Core.Option with
    member x.Or(v : 'a) = match x with Some v -> v | None -> v
    member x.Cast<'t>() = x |> Option.map (fun x -> x :> obj :?> 't)
    member x.OfType<'t>() = x |> Option.bind (fun v -> if v :> obj :? 't then Some(v :> obj :?> 't) else None)

type 'a List with
    member x.OfType<'t>() = x |> List.filter (fun x -> x :> obj :? 't) |> List.map (fun x -> x :> obj :?> 't)

module Array = 
    let private shuffleRnd (rnd : Random) (arr : _ array) = 
        for i = arr.Length - 1 downto 1 do
            let j = rnd.Next(0, arr.Length)
            let tmp = arr.[i]
            arr.[i] <- arr.[j]
            arr.[j] <- tmp

    let shuffle arr = shuffleRnd (Random()) arr
    let shuffleSeed seed arr = shuffleRnd (Random(seed)) arr
module List = 
    ///Cross/cartesian product over lists.
    let cross (seq1 : #seq<_>) (seq2 : #seq<_>) = [for item1 in seq1 do for item2 in seq2 -> item1,item2]
    ///Maps a function taking 2 parameters over a list of tuples.
    let mapPairs f = List.map (fun (a,b) -> f a b)

    let shuffleSeed seed lst = 
        let arr = lst |> List.toArray
        Array.shuffle arr
        arr |> Array.toList

    let shuffle lst = 
        let arr = lst |> List.toArray
        Array.shuffle arr
        arr |> Array.toList 

    let addPairs (a,b) lst = (a,b)::lst
    let apply1 arg fs = fs |> Seq.map (Fun.apply1 arg) |> Seq.toList
    let cross_apply1 (args : _ list) (fs : _ list) = fs |> Seq.cross_apply1 args |> Seq.toList
    let chain_iter f vs = 
        vs |> List.iter f
        vs

module Option = 
    let orValue (v : 'v) = function Some u -> u | None -> v
    let cast (opt : 'a option) : 'b option = opt |> Option.map (fun a -> a :> obj :?> 'b)

///A class with methods for generating number sequences of various kinds.
[<RequireQualifiedAccessAttribute>]
module Rand = 
    open System
    open System.Collections.Generic
    let private inner = Random()
    module Int = 
        let positive () = inner.Next()
        let of_length(min,max) =
            let min = pown 2 min
            let max = pown 2 max
            inner.Next(min,max)
        let in_range(min,max) = 
            inner.Next(min,max)

    module Float = 
        let basic() = inner.NextDouble()
        let in_range(min,max) = 
            min + inner.NextDouble() * (max - min)

    module String = 
        let mutable word_chars = [ 'a' .. 'z' ] @ ['A' .. 'Z'] |> List.toArray
        let mutable digit_chars = ['0' .. '9'] |> List.toArray
        let by_char_pool (min,max) (chars : char array) = 
            let len = Int.in_range(min,max)
            let mutable str = []
            for i = 0 to len do
                let ix = Int.in_range(0, chars.Length)
                str <- chars.[ix]::str
            String.Join("", str)
        let word_like (min,max) = by_char_pool (min,max) word_chars
        let digits (min,max) = by_char_pool(min,max) digit_chars
    
    type Seq<'v>(s : 'v seq) =
        member x.random_slices(min,max) = 
            let cache = List()
            seq {
                let n = Int.in_range(min,max)
                let next = ref n
                for item in s do
                    cache.Add(item)
                    if (cache.Count >= !next) then 
                        yield cache.ToArray()
                        next := Int.in_range(min,max)
                        cache.Clear()
                if (cache.Count > 0) then yield cache.ToArray()
            }   

///The parent type of fake modules used to implement generic modules/inheritance.
type ModuleType() = 
    inherit obj()
    //Obsolete because we don't want them visible.
    [<Obsolete("This is a module type.")>]
    override x.Equals o = base.Equals o
    [<Obsolete("This is a module type.")>]
    override x.GetHashCode() = base.GetHashCode()
    [<Obsolete("This is a module type.")>]
    override x.ToString() = base.ToString()
    [<Obsolete("This is a module type.")>]
    member x.GetType() = base.GetType()

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
    