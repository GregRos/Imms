module Wrapper
open Solid.FingerTree
open System.Collections.Generic
open System.Collections
open System

module internal Errors = 
    let Argument_null name = 
        ArgumentException(name)
    let Index_out_of_range() = 
        IndexOutOfRangeException()







[<Compil
module internal Verify = 
    let inline notNull name arg = 
        if obj.ReferenceEquals(arg, null) then
            raise(Errors.Argument_null(name))
    let inline index len index = 
        let index = if index < 0 then index + len else index
        if index < 0 || index >= len then
            raise(Errors.Index_out_of_range())
        else
            index
type FlexibleList<'value>(root : FTree<Value<'value>>)= 
    
    static let empty = FlexibleList<_>(FTree<Value<'value>>.Empty)
    static let cns r = FlexibleList<'b>(r)
      
    


    interface seq<'value> with
        member this.GetEnumerator() : IEnumerator<'value> =
            new Solid.FingerTree.Iteration.EnumeratorWrapper<'value>(root.GetEnumerator()):>_
        member this.GetEnumerator() : IEnumerator = (this :> seq<'value>).GetEnumerator():>_

    static member OfSeq vs = 
        let mutable ftree = FTree<Value<'value>>.Empty
        for v in vs do
            ftree <- ftree.AddRight(Value(v))
        FlexibleList<_>(ftree)

    member internal this.Root = root  
       
    member this.Cons v = 
        this.Root.AddLeft(Value(v)) |> cns

    member this.Conj v = 
        this.Root.AddRight(Value(v)) |> cns

    member this.ConsList (other : FlexibleList<'value>) = 
        Verify.notNull "other" other
        FTree<_>.Concat(other.Root, root) |> cns

    member this.ConjList (other : FlexibleList<'value>) = 
        Verify.notNull "other" other
        FTree<_>.Concat(root, other.Root) |> cns

    member this.ConsSeq (sequence : seq<'value>) = 
        Verify.notNull "sequence" sequence
        let asList = FlexibleList.OfSeq(sequence)
        this.ConsList asList

    member this.ConjSeq (sequence : seq<'value>) = 
        Verify.notNull "sequence" sequence
        let mutable ftree = FTree<_>.Empty
        for v in sequence do
            ftree <- ftree.AddRight(Value(v))
        ftree |> cns
    
    member this.Length = root.Measure
    member this.Item index = 
        let index = index |> Verify.index root.Measure
        ((root.Get index) :?> Value<'value>).Content
    member this.Update index value = 
        let index = index |> Verify.index root.Measure
        root.Set(index, Value(value)) |> cns
    member this.Insert index value = 
        let index = index |> Verify.index root.Measure
        root.Insert(index,Value(value)) |> cns
    member this.Slice startIndex endIndex = 
        let startIndex = startIndex |> Verify.index root.Measure
        let endIndex = endIndex |> Verify.index root.Measure
        if startIndex > endIndex then
            raise(Errors.Index_out_of_range())
        if startIndex = endIndex then
            let res = (root.Get(startIndex) :?> Value<'value>).Content
            empty.Conj(res)
        else
            let mutable split1 : FTree<_> = null
            let mutable split2 : FTree<_> = null
            root.Split(startIndex, &split1, &split2)
            split2.Split(endIndex - startIndex, &split1, &split2)
            split1 |> cns

    member this.InsertSeq 
        
        
    