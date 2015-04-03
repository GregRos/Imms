module Funq.FSharp.Implementation.ModuleTypes
open System
open Funq
open Funq.Collections
open Funq.FSharp.Aliases
open System.Runtime.CompilerServices
open System.ComponentModel
open Funq.Abstract
open Funq.FSharp.Operators
open System.Collections.Generic


[<AbstractClass>]
type FunqModuleBase<'elem, 'iterable, 'builder when 'iterable :> AbstractIterable<'elem, 'iterable, 'builder> and 'builder :> IterableBuilder<'elem>> internal () =
    inherit ModuleType()
    ///Checks if the collection is empty.
    member x.isEmpty (collection : 'iterable) = collection.IsEmpty

    ///Returns the length of the collection.
    member x.length (collection : 'iterable) = collection.Length

    ///Filters the collection using a predicate.
    member x.filter f (collection : 'iterable) = collection.Where(toFunc1 f)

    ///Iterates over the collection.
    member x.iter f (collection : 'iterable) = collection.ForEach(toAction f)  

    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    member x.iterWhile f (collection : 'iterable) = collection.ForEachWhile(toFunc1 f)

    ///Folds over the collection, in the default iteration order.
    member x.fold initial f (collection : 'iterable) = collection.Aggregate(initial, toFunc2 f)

    ///Folds over the collection, starting by applying the function to the first two elements.
    member x.reduce f (collection : 'iterable) = collection.Reduce(toFunc2 f)

    ///Folds over the collection, starting by applying the initial function to the first element.
    member x.reduce' initial f (collection : 'iterable) = collection.Reduce(toFunc1 initial, toFunc2 f)

    ///Finds an element that satisfies 'f', or None.
    member x.find f (collection : 'iterable) = collection.Find(toFunc1 f) |> fromOption

    ///Finds the position of the first element that satisfies 'f', or None.
    member x.findIndex f (collection : 'iterable) = collection.Find(toFunc1 f) |> fromOption

    ///Returns the first element from which the function returns Some, or returns None.
    member x.pick f (collection : 'iterable) = collection.Pick(f >> toOption |> toFunc1) |> fromOption

    ///Returns true if an element satisfying 'f' exists in the collection.
    member x.exists f (collection : 'iterable) = collection.Any(toFunc1 f)

    ///Returns true if all the elements in the collection satisfies 'f'.
    member x.forAll f (collection : 'iterable) = collection.All(toFunc1 f)

    ///Counts the number of elements satisfying 'f' in the collection.
    member x.count f (collection : 'iterable) = collection.Count(toFunc1 f)

[<AbstractClass>]
type FunqMapBase<'key, 'value, 'map  when 'map :> AbstractMap<'key,'value, 'map>> internal () =
    inherit FunqModuleBase<Kvp<'key,'value>, 'map, MapBuilder<'key,'value>>()
    ///Returns the value with the specified key.
    member x.get key (map : 'map) = map.[key]

    ///Returns the value associated with the specified key, or None.
    member x.tryGet key (map : 'map) = map.TryGet(key) |> fromOption

    ///Returns true if the map contains the specified key.
    member x.containsKey key (map : 'map) = map.ContainsKey(key)

    ///Returns true if the map contains the specified value.
    member x.containsValue (value,Eq) (map : 'map) = map.ContainsValue(value, Eq)
    
        
[<AbstractClass>]
type FunqSetBase<'elem, 'set when 'set :> AbstractSet<'elem, 'set>> internal () =
    inherit FunqModuleBase<'elem, 'set, SetBuilder<'elem>>()
    ///Returns true if the set contains the value.
    member x.contains value (set : 'set) = set.Contains(value)

    ///Returns the intersection of the two sets.
    member x.intersect (other : 'set) (set : 'set) = set.Intersect(other)

    ///Returns the union of the two sets.
    member x.union (other : 'set) (set : 'set) = set.Union(other)

    ///Returns 'set' minus the elements of 'other'.
    member x.except (other : 'set) (set :'set) = set.Except(other)

    ///Returns the symmetric difference between 'set' and 'other'.
    member x.difference (other : 'set) (set :'set) = set.Difference(other)

    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    member x.relates (other : 'set) (set :'set) = set.RelatesTo(other)

    ///Returns true if 'set' is a subset of 'other'
    member x.isSubsetOf (other : 'set) (set : 'set) = set.IsSubsetOf(other)

    ///Returns true if 'set' is a superset of 'other'
    member x.isSuperOf (other : 'set) (set : 'set) = set.IsSupersetOf(other)

    ///Returns true if 'set' is equal to 'other'
    member x.isEqual (other : 'set) (set : 'set) = set.SetEquals(other)

    ///Returns true if 'set' is a proper subset of 'other'
    member x.isProperSubsetOf (other : 'set) (set : 'set) = set.IsProperSubsetOf(other)

    ///Returns true if 'set' is a proper superset of 'other'
    member x.isProperSuperOf (other : 'set) (set : 'set) = set.IsProperSupersetOf(other)

    ///Returns true if 'set' is disjoint with (shares no elements with) other.
    member x.isDisjointWith (other : 'set) (set : 'set) = set.IsDisjointWith(other)
    
[<AbstractClass>]
type FunqSeqModule<'elem, 'seq when 'seq :> AbstractSequential<'elem,'seq>> internal () =
    inherit FunqModuleBase<'elem, 'seq, IterableBuilder<'elem>>()
    ///Returns the first element of the collection
    member x.first (collection : 'seq) = collection.First
    ///Returns the last element of the collection
    member x.last (collection : 'seq) = collection.Last
    ///Returns the first element of the collection or None.
    member x.tryFirst (collection : 'seq) = collection.TryFirst |> fromOption
    ///Returns the last element of the collection or None.
    member x.tryLast (collection : 'seq) = collection.TryLast |> fromOption
    member x.slice(i1, i2) (collection : 'seq) = collection.[i1, i2]
    ///Iterates over the collection from end to start.
    member x.iterBack f (collection : 'seq) = collection.ForEachBack(toAction f)
    ///Iterates over the collection from end to start while 'f' is true.
    member x.iterBackWhile f (collection : 'seq) = collection.ForEachBackWhile(toFunc1 f)
    ///Folds over the collection from end to start.
    member x.foldBack initial f (collection : 'seq) = collection.AggregateBack(initial, toFunc2 f)
    ///Folds over the collection from end to start, beginning by applying 'initial' to the last element.
    member x.reduceBack' initial f (collection : 'seq) = collection.ReduceBack(toFunc1 initial, toFunc2 f)
    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    member x.reduceBack f (collection : 'seq) = collection.ReduceBack(toFunc2 f)
    ///Returns the first 'n' elements of the collection.
    member x.take n (collection : 'seq) = collection.Take(n)
    ///Returns the first elements of the collection until 'f' returns false.
    member x.takeWhile f (collection : 'seq) = collection.TakeWhile(toFunc1 f)
    ///Skips over 'n' elements of the collection
    member x.skip n (collection : 'seq) = collection.Skip(n)
    ///Skips over elements of the collection until 'f' returns false.
    member x.skipWhile f (collection : 'seq) = collection.SkipWhile(toFunc1 f)
    ///Returns the nth element of the collection.
    member x.nth index (collection : 'seq) = collection.[index]
    ///Returns the last element of the collection that satisfies 'f'
    member x.findLast f (collection : 'seq) = collection.FindLast(toFunc1 f) |> fromOption
    ///Returns the index of the last element of the collection that satisfies 'f'
    member x.findIndex f (collection : 'seq) = collection.FindIndex(toFunc1 f) |> fromOption

[<EditorBrowsable(EditorBrowsableState.Never)>]
type FunqVectorModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqVector<'elem>>()
    static let _instance  = FunqVectorModule<'elem>()
    static member internal instance = _instance
    ///Returns the empty vector.
    member x.empty = FunqVector<'elem>.Empty
    ///Returns a vector consisting of 'value'
    member x.ofItem value= FunqVector<'elem>.Empty <+ value
    ///Casts all the elements of the collection to a different type.
    member x.cast<'elem2> (collection : 'elem FunqList) = collection.Cast<'elem2>()
    ///Returns a collection consisting of all the elements that are of the specified type.
    member x.ofType<'elem2> (collection : 'elem FunqList) = collection.OfType<'elem2>()
    ///Adds an element to the end of the collection.
    member x.addLast value (collection : _ FunqVector)  = collection.AddLast(value)
    ///Adds a sequence of elements to the end.
    member x.addLastRange vs (collection :_ FunqVector) = collection.AddLastRange(vs)
    ///Removes the last element of the collection.
    member x.removeLast (collection :_ FunqVector) = collection.RemoveLast()
    ///Updates the value of the element at the specified index.
    member x.update index value (collection : _ FunqVector) = collection.Update(index, value)
    ///Maps over the collection.
    member x.map (f : 'elem -> 'relem) (collection :_ FunqVector) = collection.Select<'relem>(toFunc1 f)

    member x.collect f (collection :_ FunqVector) = collection.SelectMany(toFunc1 f)
    member x.choose (f : 'elem -> 'relem option) (collection :_ FunqVector) =
        let f' = f >> toOption |> toFunc1
        collection.Select<'relem>(f')

    member x.groupBy<'key when 'key : equality> (keySelector : 'elem -> 'key) (collection : 'elem FunqVector)=
        collection.GroupBy(keySelector |> toFunc1)

    member x.scan (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (collection : 'elem FunqVector) = 
        collection.Scan(initial, toFunc2 accumulator)

    member x.scanBack (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (collection : 'elem FunqVector) = 
        collection.ScanBack(initial, accumulator |> toFunc2)

    member x.zip (other : 'elem2 seq) (collection : 'elem FunqVector) = 
        collection.Zip(other)
    member x.ofSeq vs = FunqVector.ToFunqVector(vs)

type FunqListModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqList<'elem>>()
    static let _instance  = FunqListModule<'elem>()
    static member internal instance = _instance
    member x.ofItem value= FunqList<'elem>.Empty <+ value
    member x.empty = FunqList<'elem>.Empty
    member x.addLast value (collection :_ FunqList) = collection.AddLast(value)
    member x.cast<'elem2> (collection : 'elem FunqList) = collection.Cast<'elem2>()
    member x.ofType<'elem2> (collection : 'elem FunqList) = collection.OfType<'elem2>()
    member x.addLastRange vs (collection :_ FunqList) = collection.AddLastRange(vs)
    member x.addFirst value (collection:_ FunqList) = collection.AddFirst(value)
    member x.addFirstRange vs (collection :_ FunqList) = collection.AddFirstRange(vs)
    member x.insert index value (collection :_ FunqList) = collection.Insert(index,value)
    member x.remove index (collection :_ FunqList) = collection.Remove(index)
    member x.update index value (collection :_ FunqList) = collection.Update(index,value)
    member x.map (f : 'elem -> 'relem) (collection :_ FunqList) = collection.Select<'relem>(toFunc1 f)
    member x.collect f (collection :_ FunqList) = collection.SelectMany(toFunc1 f)
    member x.choose (f : 'elem -> 'relem option) (collection :_ FunqList) =
        let f' = f >> toOption |> toFunc1
        collection.Select<'relem>(f')

    member x.groupBy<'key when 'key : equality> (keySelector : 'elem -> 'key) (collection : 'elem FunqList)=
        collection.GroupBy(keySelector |> toFunc1)

    member x.scan (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (collection : 'elem FunqList) = 
        collection.Scan(initial, toFunc2 accumulator)

    member x.scanBack (initial : 'relem) (accumulator : 'relem -> 'elem -> 'relem) (collection : 'elem FunqList) = 
        collection.ScanBack(initial, accumulator |> toFunc2)

    member x.zip (other : 'elem2 seq) (collection : 'elem FunqList) = 
        collection.Zip(other)
    member x.ofSeq vs = FunqList.ToFunqList(vs)

type FunqMapModule<'key, 'value> internal () =
    inherit FunqMapBase<'key, 'value, FunqMap<'key,'value>>()
    static let _instance = FunqMapModule<'key,'value>()
    static member internal instance = _instance
    member x.emptyWith(eq : Eq<'key>) = FunqMap<'key,'value>.Empty(eq)
    member x.empty = FunqMap<'key,'value>.Empty()
    member x.ofSeq vs = FunqList.Empty().AddLastRange(vs)

type FunqSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqSet<'elem>>()
    static let _instance = FunqSetModule<'elem>()
    static member internal instance = _instance
    member x.emptyWith(Eq : Eq<'elem>) = FunqSet<'elem>.Empty(Eq)
    member x.empty = FunqSet<'elem>.Empty()
    member x.ofSeq vs = FunqSet.ToFunqSet(vs)
type FunqOrderedMapModule<'key, 'value> internal () =
    inherit FunqMapBase<'key,'value, FunqOrderedMap<'key,'value>>()
    static let _instance = FunqOrderedMapModule<'key,'value>()
    static member internal instance = _instance
    member x.emptyWith(cm : Cm<'key>) = FunqOrderedMap<'key,'value>.Empty(cm)

type FunqOrderedSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqOrderedSet<'elem>>()
    static let _instance = FunqOrderedSetModule<'elem>()
    static member internal instance = _instance
    member x.emptyWith(cm : Cm<'elem>) = FunqOrderedSet<'elem>.Empty(cm)


