module Funq.FSharp.Implementation.ModuleTypes
open System
open Funq
open Funq.Collections
open Funq.FSharp.Aliases
open System.Runtime.CompilerServices
open System.ComponentModel
open Funq.Abstract
open Funq.FSharp.Operators

type FunqModuleBase<'elem, 'seq, 'builder when 'seq :> AbstractIterable<'elem, 'seq, 'builder> and 'builder :> IterableBuilder<'elem>> internal () =
    inherit ModuleType()
    member x.isEmpty (s : 'seq) = s.IsEmpty
    member x.length (s : 'seq) = s.Length
    member x.filter f (s : 'seq) = s.Where(toFunc1 f)
    member x.iter f (s : 'seq) = s.ForEach(toAction f)  
    member x.iterWhile f (s : 'seq) = s.ForEachWhile(toFunc1 f)
    member x.fold r f (s : 'seq) = s.Aggregate(r, toFunc2 f)
    member x.reduce f (s : 'seq) = s.Reduce(toFunc2 f)
    member x.reduce' initial f (s : 'seq) = s.Reduce(toFunc1 initial, toFunc2 f)
    member x.find f (s : 'seq) = s.Find(toFunc1 f) |> fromOption
    member x.findIndex f (s : 'seq) = s.Find(toFunc1 f) |> fromOption
    member x.pick f (s : 'seq) = s.Pick(f >> toOption |> toFunc1) |> fromOption
    member x.exists f (s : 'seq) = s.Any(toFunc1 f)
    member x.forAll f (s : 'seq) = s.All(toFunc1 f)
    member x.count f (s: 'seq) = s.Count(toFunc1 f)
    
type FunqMapBase<'k, 'v, 'map when 'map :> AbstractMap<'k, 'v, 'map>> internal () =
    inherit FunqModuleBase<Kvp<'k,'v>, 'map, MapBuilder<'k,'v>>()
    static let _instance = FunqMapBase<'k,'v, 'map>()
    static member internal instance = _instance
    member x.get k (map : 'map) = map.[k]
    member x.tryGet k (map : 'map) = map.TryGet(k) |> fromOption
    member x.containsKey k (map : 'map) = map.ContainsKey(k)
    member x.containsValue (v,Eq) (map : 'map) = map.ContainsValue(v, Eq)

type FunqSetBase<'elem, 'set when 'set :> AbstractSet<'elem, 'set>> internal () =
    inherit FunqModuleBase<'elem, 'set, SetBuilder<'elem>>()
    static let _instance = FunqSetBase<'elem, 'set>()
    static member internal instance = _instance
    member x.contains v (set : 'set) = set.Contains(v)
    member x.intersect (other : 'set) (set : 'set) = set.Intersect(other)
    member x.union (other : 'set) (set : 'set) = set.Union(other)
    member x.except (other : 'set) (set : 'set) = set.Except(other)
    member x.difference (other : 'set) (set : 'set) = set.Difference(other)
    member x.relates (other : 'set) (set : 'set) = set.RelatesTo(other)

type FunqSeqModule<'elem, 'list when 'list :> AbstractSequential<'elem,'list>> internal () =
    inherit FunqModuleBase<'elem, 'list, IterableBuilder<'elem>>()
    member x.first (s : 'list) = s.First
    member x.last (s : 'list) = s.Last
    member x.tryFirst (s : 'list) = s.TryFirst |> fromOption
    member x.tryLast (s : 'list) = s.TryLast |> fromOption
    member x.slice(i1, i2) (s : 'list) = s.[i1, i2]
    member x.iterBack f (s : 'list) = s.ForEachBack(toAction f)
    member x.iterBackWhile f (s : 'list) = s.ForEachBackWhile(toFunc1 f)
    member x.foldBack r f (s : 'list) = s.AggregateBack(r, toFunc2 f)
    member x.reduceBack' initial f (s : 'list) = s.ReduceBack(toFunc1 initial, toFunc2 f)
    member x.reduceBack f (s : 'list) = s.ReduceBack(toFunc2 f)
    member x.take n (s : 'list) = s.Take(n)
    member x.takeWhile f (s : 'list) = s.TakeWhile(toFunc1 f)
    member x.skip n (s : 'list) = s.Skip(n)
    member x.skipWhile f (s : 'list) = s.SkipWhile(toFunc1 f)
    member x.nth i (s : 'list) = s.[i]
    member x.findLast f (s : 'list) = s.FindLast(toFunc1 f)
    member x.findIndex f (s : 'list) = s.FindIndex(toFunc1 f)

[<EditorBrowsable(EditorBrowsableState.Never)>]
type FunqVectorModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqVector<'elem>>()
    static let _instance  = FunqVectorModule<'elem>()
    static member internal instance = _instance
    member x.empty = FunqVector<'elem>.Empty
    member x.ofItem v= FunqVector<'elem>.Empty <+ v
    member x.cast<'elem2> (s : 'elem FunqList) = s.Cast<'elem2>()
    member x.ofType<'elem2> (s : 'elem FunqList) = s.OfType<'elem2>()
    member x.addLast v (s : _ FunqVector)  = s.AddLast(v)
    member x.addLastRange vs (s :_ FunqVector) = s.AddLastRange(vs)
    member x.dropLast (s :_ FunqVector) = s.DropLast()
    member x.update i v (s : _ FunqVector) = s.Update(i, v)
    member x.map (f : 'elem -> 'relem) (s :_ FunqVector) = s.Select<'relem>(toFunc1 f)
    member x.collect f (s :_ FunqVector) = s.SelectMany(toFunc1 f)
    member x.choose (f : 'elem -> 'relem option) (s :_ FunqVector) =
        let f' = f >> toOption |> toFunc1
        s.Select<'relem>(f')

    member x.groupBy<'key when 'key : equality> (keySelector : 'elem -> 'key) (s : 'elem FunqVector)=
        s.GroupBy(keySelector |> toFunc1)

    member x.scan (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (s : 'elem FunqVector) = 
        s.Scan(initial, toFunc2 accumulator)

    member x.scanBack (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (s : 'elem FunqVector) = 
        s.ScanBack(initial, accumulator |> toFunc2)

    member x.zip (other : 'elem2 seq) (s : 'elem FunqVector) = 
        s.Zip(other)
    

type FunqListModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqList<'elem>>()
    static let _instance  = FunqListModule<'elem>()
    static member internal instance = _instance
    member x.ofItem v= FunqList<'elem>.Empty <+ v
    member x.empty = FunqList<'elem>.Empty
    member x.addLast v (s :_ FunqList) = s.AddLast(v)
    member x.cast<'elem2> (s : 'elem FunqList) = s.Cast<'elem2>()
    member x.ofType<'elem2> (s : 'elem FunqList) = s.OfType<'elem2>()
    member x.addLastRange vs (s :_ FunqList) = s.AddLastRange(vs)
    member x.addFirst v (s:_ FunqList) = s.AddFirst(v)
    member x.addFirstRange vs (s :_ FunqList) = s.AddFirstRange(vs)
    member x.insert i v (s :_ FunqList) = s.Insert(i,v)
    member x.remove i (s :_ FunqList) = s.Remove(i)
    member x.update i v (s :_ FunqList) = s.Update(i,v)
        member x.map (f : 'elem -> 'relem) (s :_ FunqList) = s.Select<'relem>(toFunc1 f)
    member x.collect f (s :_ FunqList) = s.SelectMany(toFunc1 f)
    member x.choose (f : 'elem -> 'relem option) (s :_ FunqList) =
        let f' = f >> toOption |> toFunc1
        s.Select<'relem>(f')

    member x.groupBy<'key when 'key : equality> (keySelector : 'elem -> 'key) (s : 'elem FunqList)=
        s.GroupBy(keySelector |> toFunc1)

    member x.scan (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (s : 'elem FunqList) = 
        s.Scan(initial, toFunc2 accumulator)

    member x.scanBack (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (s : 'elem FunqList) = 
        s.ScanBack(initial, accumulator |> toFunc2)

    member x.zip (other : 'elem2 seq) (s : 'elem FunqList) = 
        s.Zip(other)
    

type FunqMapModule<'k, 'v> internal () =
    inherit FunqMapBase<'k,'v,FunqMap<'k,'v>>()
    static let _instance = FunqMapModule<'k,'v>()
    static member internal instance = _instance
    member x.empty(eq : Eq<'k>) = FunqMap<'k,'v>.Empty(eq)
   
type FunqSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqSet<'elem>>()
    static let _instance = FunqSetModule<'elem>()
    static member internal instance = _instance
    member x.empty(Eq : Eq<'elem>) = FunqSet<'elem>.Empty(Eq)

type FunqOrderedMapModule<'k, 'v> internal () =
    inherit FunqMapBase<'k,'v,FunqMap<'k,'v>>()
    static let _instance = FunqOrderedMapModule<'k,'v>()
    static member internal instance = _instance
    member x.empty(cm : Cm<'k>) = FunqOrderedMap<'k,'v>.Empty(cm)

type FunqOrderedSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqSet<'elem>>()
    static let _instance = FunqOrderedSetModule<'elem>()
    static member internal instance = _instance
    member x.empty(cm : Cm<'elem>) = FunqOrderedSet<'elem>.Empty(cm)