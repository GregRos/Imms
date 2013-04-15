module Wrapper
open Solid
open Solid.Common;
open System.Collections.Generic
open System.Collections
open System



type XList<'value>(root : FlexibleList<'value>)= 
    
    static let empty = XList<_>(FlexibleList<'value>.Empty)
    static let cns r = XList<'b>(r)
      
    interface seq<'value> with
        member this.GetEnumerator() : IEnumerator<'value> =
            root.GetEnumerator()
        member this.GetEnumerator() : IEnumerator = root.GetEnumerator():>_

    interface IList<'value> with
        member this.Count = this.Length
        member this.Add _ = raise Errors.Collection_readonly
        member this.RemoveAt _ = raise Errors.Collection_readonly
        member this.IsReadOnly = true
        member this.CopyTo (arr, index) = 
            root.CopyTo(arr, index)
        member this.IndexOf i = (root :> IList<'value>).IndexOf(i)
        member this.Contains i = (root :> IList<'value>).Contains(i)
        member this.Clear () = raise Errors.Collection_readonly
        member this.Insert(_,_) = raise Errors.Collection_readonly
        member this.Remove _ = raise Errors.Collection_readonly
        member this.get_Item i = this.[i]
        member this.set_Item(a,b) = raise Errors.Collection_readonly
    static member OfSeq vs = 
        FlexibleList.Empty.AddLastRange(vs) |> cns

    member internal this.Root = root  
       
    member this.Cons v = 
        root.AddFirst v

    member this.Conj v = 
        root.AddLast v |> cns

    member this.ConsList (other : XList<'value>) = 
        other.Root |> root.AddFirstList |> cns

    member this.ConjList (other : XList<'value>) = 
        other.Root |> root.AddLastList |> cns

    member this.ConsSeq (s : seq<'value>) = 
        s |> root.AddFirstRange |> cns

    member this.ConjSeq (s : seq<'value>) = 
        s |> root.AddLastRange |> cns
    member this.Length = root.Count
    member this.Item index = 
        root.[index]
    member this.Update index value = 
        root.Set(index,value)
    member this.Insert index value = 
        root.Insert(index,value)
    member this.GetSlice(fIndex, lIndex)= 
        match fIndex,lIndex with
        | None,None -> this
        | Some s, None -> root.Slice(s, -1) |> cns
        | Some s, Some e -> root.Slice(s,e) |> cns
        | None, Some e -> root.Slice(0, e) |> cns

    member this.InsertSeq index s = 
        root.InsertRange(index, s) |> cns

    member this.InsertList index (l : XList<_>) = 
        root.InsertList(index, l.Root) |> cns

    member this.Remove index = 
        root.Remove(index) |> cns

    member this.Take n = 
        root.Take(n) |> cns

    member this.Skip n = 
        root.Skip(n) |> cns

    member this.TakeWhile f = 
        root.TakeWhile(Func<_,_>(f))

    member this.SkipWhile f = 
        root.SkipWhile(Func<_,_>(f))

    member this.Iter f = 
        root.ForEach(Action<_>(f))

    member this.IterBack f = 
        root.ForEach(Action<_>(f))

    member this.IterWhile f = 
        root.ForEachWhile(Func<_,_>(f))

    member this.IterBackWhile f = 
        root.ForEachBackWhile(Func<_,_>(f))

    member this.Fold f = 
        root.Aggregate(Func<_,_,_>(f))

    member this.FoldBack f = 
        root.AggregateBack(Func<_,_,_>(f))

    member this.Filter f = 
        root.Where(Func<_,_>(f)) |> cns

    member this.Map f = 
        root.Select(Func<_,_>(f)) |> cns

    member this.Uncons = 
        root.DropFirst() |> cns

    member this.Unconj = 
        root.DropLast() |> cns

    member this.Head = 
        root.First 

    member this.Last = 
        root.Last

    member this.IndexOf p = 
        match root.IndexOf(p) with
        | n when n.HasValue -> Some <| n.Value
        | _ -> None

    member this.Rev() = root.Reverse() |> cns

    member this.Split i =
        let mutable part1 : FlexibleList<_> = null
        let mutable part2 : FlexibleList<_> = null
        root.Split(i, &part1, &part2)
        part1,part2

type Vector<'value>(root : FastList<'value>) = 
    static let cns x = Vector<'value>(x)
    interface seq<'value> with
        member this.GetEnumerator() : IEnumerator<'value> = root.GetEnumerator()
        member this.GetEnumerator() : IEnumerator = root.GetEnumerator():>_

    interface IList<'value> with
        member this.Count = this.Length
        member this.Add _ = raise Errors.Collection_readonly
        member this.RemoveAt _ = raise Errors.Collection_readonly
        member this.IsReadOnly = true
        member this.CopyTo (arr, index) = 
            root.CopyTo(arr, index)
        member this.IndexOf i = (root :> IList<'value>).IndexOf(i)
        member this.Contains i = (root :> IList<'value>).Contains(i)
        member this.Clear () = raise Errors.Collection_readonly
        member this.Insert(_,_) = raise Errors.Collection_readonly
        member this.Remove _ = raise Errors.Collection_readonly
        member this.get_Item i = this.[i]
        member this.set_Item(a,b) = raise Errors.Collection_readonly

    member val internal Root = root
    
    member __.Conj v = root.AddLast v |> cns

    member __.ConjSeq vs = root.AddLastRange vs |> cns

    member __.Unconj = root.DropLast() |> cns

    member __.Item i = root.[i]

    member __.Last = root.Last

    member __.Head = root.First

    member __.Filter f = root.Where(Func<_,_>(f)) |> cns

    member __.Map f = root.Select(Func<_,_>(f)) |> cns

    member __.Fold f = root.Aggregate(Func<_,_,_>(f)) |> cns

    member __.Iter f = root.ForEach(Action<_>(f))

    member __.IterBack f = root.ForEachBack(Action<_>(f))

    member __.IterWhile f = root.ForEachWhile(Func<_,_>(f))

    member __.IterBackWhile f = root.ForEachBackWhile(Func<_,_>(f))

    member __.Take n = root.Take n |> cns

    member __.UnconjN n = root.DropLast(n) |> cns

    member __.FoldBack f = root.AggregateBack(Func<_,_,_>(f))

    member __.TakeWhile f = root.TakeWhile(Func<_,_>(f)) |> cns
    
    member __.Length = root.Count

    member __.IndexOf f = 
        match root.IndexOf(Func<_,_>(f)) with
        | n when n.HasValue -> Some(n.Value)
        | _ -> None





        
        
        
    