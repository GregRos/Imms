[<AutoOpen>]
module SolidFS.Operators
open Solid
//++ Primitive Operators
// We define generalized operators usable throughout the library.
// This is preferable to separate static operators because when statically defined operators are used, 
//Type inference doesn't seem to work very well.
open System

let inline delay1 f p1 = fun () -> f p1
let inline delay2 f p1 p2 = fun () -> f  p1
let inline delay3 f p1 p2 p3 = fun () -> f p1 p2 p3
let inline delay4 f p1 p2 p3 p4 = fun () -> f p1 p2 p3 p4

type xlist<'a> = FlexibleList<'a>
let inline (+>) item col = (^s : (member AddFirst : 'a -> 's) col,item)
let inline (<+) col item = (^s : (member AddLast : 'a -> 's) col,item)
let inline (<+>) col1 col2 = (^s : (member Append : 's -> 's) col1,col2)
let inline (++>) items col = (^s : (member AddFirstRange : #seq<_> -> 's) col,items)
let inline (<++) col items = (^s : (member AddLastRange : #seq<_> -> 's) col,items)
let inline (>+<) target1 target2 = (^s : (member AddLastList : 's -> 's)target1,target2)
type 'a FlexibleList with
    member this.GetSlice(startIndex : int option, endIndex : int option) = 
        let startIndex = if startIndex.IsSome then startIndex.Value else 0
        let endIndex = if endIndex.IsSome then endIndex.Value else -1
        this.Slice(startIndex,endIndex)
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module XList = 
    let first (target : xlist<'a>)  = 
        target.First

    let last (target : xlist<'a>) = 
        target.Last

    let dropF (target : xlist<'a>) = 
        target.DropFirst()

    let dropL (target : xlist<'a>) = 
        target.DropLast()

    let nth  (index : int) (target : xlist<'a>) =
        target.[index]
       
    let set (index : int) (item : 'a) (target : xlist<'a>) = 
        target.Set(index,item)

    let insert  (index : int) (item : 'a) (target : xlist<'a>)=
        target.Insert(index,item)

    let insertRange (index : int) (items : 'a seq) (target : xlist<'a>) =
        target.InsertRange(index, items)

    let insertList (index : int) (l : xlist<'a>) (target : xlist<'a>) = 
        target.InsertList(index,l)

    let slice (startIndex : int) (endIndex : int) (target : xlist<'a>) = 
        target.Slice(startIndex,endIndex)

    let length (target:  xlist<'a>) = 
        target.Count

    let filter (f : 'a -> bool) (target : xlist<'a>) = 
        target.Where(Func<'a,bool>(f))

    let map (f : 'a -> 'b) (target : xlist<'a>) = 
        target.Select(Func<'a,'b>(f))

    let iter (f : 'a -> unit) (target : xlist<'a>) = 
        target.ForEach(Action<'a>(f))

    let take count (target : xlist<'a>) = 
        target.Slice(0,count - 1)

    let skip count (target : xlist<'a>) = 
        target.Slice(count,-1)

    let empty<'a> = xlist<'a>.Empty

    let choose (f : 'a -> 'b option) (target : xlist<'a>) = 
        let new_target = ref (empty : xlist<'b>)
        target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))

    let fold (initial : 'state) (f : 'state -> 'item -> 'state) (target : xlist<'item>) = 
        target.Aggregate(Func<'state,'item,'state>(f), initial)
        
    let foldBack (initial : 'state) (f : 'state -> 'item -> 'state) (target : xlist<'item>) = 
        target.AggregateBack(Func<_,_,_>(f), initial)

    let collect (f : _ -> seq<_>) (target :_ xlist) =
        target.SelectMany(Func<_,_>(f))

    let zip (left : #seq<_>) (right:#seq<_>) = 
        empty<_> <++ Seq.zip left right
        
    let ofSeq (xs : #seq<_>) = 
        empty<_> <++ xs

    let exists (f : _ -> bool) (l : _ xlist) =
        l.IndexOf(Func<_,bool>(f)).HasValue

    let ofItem x = empty <+ x

    let indexOf(f : _ -> bool) (l :_ xlist)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some res.Value else None

    let find(f : _ -> bool) (l :_ xlist)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some l.[res.Value] else None

    let takeWhile (f : _ -> bool) (target : _ xlist) = 
        target.TakeWhile(Func<_,bool>(f))

    let skipWhile (f : _ -> bool) (target :_ xlist) = 
        target.SkipWhile(Func<_,bool>(f))

    

module Vector = 
    let first (target : Vector<'a>)  = 
        target.First

    let last (target :  Vector<'a>) = 
        target.Last


    let dropL (target :Vector<'a>) = 
        target.DropLast()

    let nth  (index : int) (target :Vector<'a>) =
        target.[index]
       
    let set (index : int) (item : 'a) (target :Vector<'a>) = 
        target.Set(index,item)

    let length (target:  Vector<'a>) = 
        target.Count

    let filter (f : 'a -> bool) (target : Vector<'a>) = 
        target.Where(Func<'a,bool>(f))

    let map (f : 'a -> 'b) (target : Vector<'a>) = 
        target.Select(Func<'a,'b>(f))

    let iter (f : 'a -> unit) (target : Vector<'a>) = 
        target.ForEach(Action<'a>(f))

    let take count (target : Vector<'a>) = 
        target.Take(count)

    let empty<'a> = Vector<'a>.Empty

    let choose (f : 'a -> 'b option) (target :Vector<'a>) = 
        let new_target = ref (empty : Vector<'b>)
        target |> iter (fun v -> match f v with | None -> () | Some u -> new_target := (!new_target <+ u))

    let fold (initial : 'state) (f : 'state -> 'item -> 'state) (target : Vector<'item>) = 
        target.Aggregate(Func<'state,'item,'state>(f), initial)

    let zip (left : #seq<_>) (right:#seq<_>) = 
        empty<_> <++ Seq.zip left right
        
    let ofSeq (xs : #seq<_>) = 
        empty<_> <++ xs

    let exists (f : _ -> bool) (l : _ Vector) =
        l.IndexOf(Func<_,bool>(f)).HasValue

    let ofItem x = empty <+ x

    let indexOf(f : _ -> bool) (l :_ Vector)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some res.Value else None

    let find(f : _ -> bool) (l :_ Vector)=
        let res = l.IndexOf(Func<_,_>(f))
        if res.HasValue then Some l.[res.Value] else None

    