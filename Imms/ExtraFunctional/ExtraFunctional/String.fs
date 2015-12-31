namespace ExtraFunctional
open ExtraFunctional.Fun
open System.Text.RegularExpressions

module String = 
    open ExtraFunctional
    open System

    let toUpperInv (str : string) = str.ToUpperInvariant()
    let toLowerInv (str : string) = str.ToLowerInvariant()

    let contains caseSensitive what (where : string) = 
        if caseSensitive then 
            where.Contains(what) 
        else 
            where.ToUpperInvariant().Contains(what.ToUpperInvariant())

    let replace (find : string) replace (where : string) = 
        where.Replace(find, replace)

    let tryFirstIndex what where = 
        match toLowerInv(what).IndexOf(where |> toLowerInv) with
        | -1 -> None
        | i -> Some i

    let trim (str : string) = str.Trim()
    let trimAny chars (str : string) = str.Trim(chars |> Seq.asArray)
    let trimAnyStart chars (str : string) = str.TrimStart(chars |> Seq.asArray)
    let trimAnyEnd chars (str : string) = str.TrimEnd(chars |> Seq.asArray)
    
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

    let split (delim : string seq) (str : string) = str.Split(delim |> Seq.toArray, StringSplitOptions.None)
    let containsAny caseSensitive what (where : string) = 
        what |> Seq.exists (contains caseSensitive @? where)
    let containsAll caseSensitive what (where : string) = what |> Seq.forall (contains caseSensitive @? where)
    let ofChars chars = String(chars |> Seq.asArray) : string

    let substring (str : string) (st,en) = str.[st .. en]

    let take (str : string) n = str.[0 .. n - 1]

    let takeWhile f (str : string)  = str |> Seq.takeWhile (fun s -> f s) |> ofChars
    
    let skipWhile f (str : string)  = str |> Seq.skipWhile (fun s -> f s) |> ofChars

    let skip (str : string) n = str.[n .. ]

    let initChars count (f : int -> char) = String.init count (f >> string)
    let tryIndexOf caseSensitive (what : string) (where : string) = 
        let comparison = if caseSensitive then StringComparison.InvariantCulture else StringComparison.InvariantCultureIgnoreCase
        match where.IndexOf(what, comparison) with
        | -1 -> None
        | i -> Some i
    
    module Patterns = 
        type FloatingPointMetadata = {
            Raw : string
            LeadingZeroes : int
            Precision : int
        }

        let (|Split|) delimeters target = 
            target |> split delimeters

        let (|Substring|_|) what where = 
            let find = tryIndexOf true what where
            match find with
            | None -> None
            | Some i -> where.[i .. i + what.Length - 1] |> Some

        let (|StartsWith|_|) what (where : string) = 
            if where.StartsWith(what) then Some() else None
            
        let (|EndsWith|_|) what (where : string) =
            if where.EndsWith(what) then Some() else None

        let (|Double|_|) (str : string) = 
            let str = str |> trim
            let yes, n = Double.TryParse str
            if yes then 
                let leadingZeroes = str |> takeWhile (function '0' -> true | _ -> false) |> String.length
                let precision = 
                    match str.IndexOf('.') with
                    | -1 -> 0
                    | i -> str.Length - i - 1
                Some (n, {Raw = str; LeadingZeroes = leadingZeroes; Precision = precision})
            else None

        let (|Int32|_|) str = 
            let str = str |> trim
            let yes, n = Int32.TryParse str
            if yes then Some n else None

        let (|Regex|_|) (pattern : Regex) input = 
            let m = pattern.Match(input)
            if m.Success then Some m else None
        

