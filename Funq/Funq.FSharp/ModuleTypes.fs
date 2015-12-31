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

[<AbstractClass>]
type FunqModuleBase<'elem, 'collection, 'builder when 'collection :> AbstractIterable<'elem, 'collection, 'builder> and 'builder :> IIterableBuilder<'elem, 'collection>> internal () =
    inherit ModuleType()
    ///Checks if the collection is empty. O(1)
    static member isEmpty (collection : 'collection) = collection.IsEmpty

    ///Returns the length of the collection. O(1)
    static member length (collection : 'collection) = collection.Length

    ///Iterates over the collection.
    static member iter f (collection : 'collection) = collection.ForEach(toAction f)  

    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    static member iterWhile f (collection : 'collection) = collection.ForEachWhile(toFunc1 f)

    ///Folds over the collection, in the default iteration order.
    static member fold initial f (collection : 'collection) = collection.Aggregate(initial, toFunc2 f)

    ///Folds over the collection, starting by applying the function to the first two elements.
    static member reduce f (collection : 'collection) = collection.Aggregate(toFunc2 f)

    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    static member single (collection : 'collection) = collection.Single()

    ///Converts the specified collection to an array.
    static member toArray (collection : 'collection) = collection.ToArray()

    ///Converts the elements of the collection to a string, using the specified separator.
    static member print sep (collection : 'collection)  = collection.Print(sep, sprintf "%A" |> toFunc1)

    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    static member printWith sep format (collection : 'collection) = collection.Print(sep, (sprintf format) |> toFunc1)

    ///Finds an element that satisfies 'f', or None.
    static member find f (collection : 'collection) = collection.Find(toFunc1 f) |> fromOption

    ///Returns the first element from which the function returns Some, or returns None.
    static member pick f (collection : 'collection) = collection.Pick(f >> toOption |> toFunc1) |> fromOption

    ///Returns true if an element satisfying 'f' exists in the collection.
    static member exists f (collection : 'collection) = collection.Any(toFunc1 f)

    ///Returns true if all the elements in the collection satisfies 'f'.
    static member forAll f (collection : 'collection) = collection.All(toFunc1 f)

    ///Counts the number of elements satisfying 'f' in the collection.
    static member count f (collection : 'collection) = collection.Count(toFunc1 f)


[<AbstractClass>]
type FunqMapBase<'key, 'value, 'map  when 'map :> AbstractMap<'key,'value, 'map>> internal () =
    inherit FunqModuleBase<Kvp<'key,'value>, 'map, IMapBuilder<'key,'value, 'map>>()

    ///Returns true if every key-value pair satisfies the specified predicate.
    static member forAllPairs (f : 'key -> 'value -> bool) (collection : 'map) = collection.All(toFunc2 f)

    ///Returns true if any key-value pair satisfies the given predicate.
    static member existsPair f (map : 'map) = map.Any(toFunc2 f)

    ///Returns the first pair (in order of iteration) that fulfills the given predicate.
    static member findPair f (map : 'map) = map.Find(toFunc2 f)

    ///Applies the specified function on every key-value pair, and returns the first result that isn't None.
    static member pickPair (f : 'key -> 'value -> 'out option) (map : 'map) = map.Pick((fun a b -> f a b |> toOption) |> toFunc2) |> fromOption

    ///Returns the number of key-value pairs that fulfill the specified predicate.
    static member countPairs (f : 'key -> 'value -> bool) (map : 'map) = map.Count(toFunc2 f)

    ///Adds a key-value pair to the map, throwing an exception if the key already exists.
    static member add k v (map : 'map) = 
        map.Add(k, v)

    ///Adds a key-value pair to the map, overwriting the previous value.
    static member set k v (map : 'map) =
        map.Set(k, v)

    ///Adds a sequence of key-value pairs (in the form of 2-tuples) to the map, throwing an exception if a key already exists.
    static member addPairs pairs (map : 'map) =
        map.AddRange(pairs |> Seq.map (Kvp.Of))

    ///Adds a sequence of key-value pairs (in the form of KeyValuePairs) to the map, throwing an exception if a key already exists.
    static member addRange kvps (map : 'map) =
        map.AddRange kvps

    ///Adds a sequence of key-value pairs to the map (in the form of 2-tuples), overwriting previous information.
    static member setPairs pairs (map : 'map) =
        map.SetRange(pairs |> Seq.map (Kvp.Of))

    ///Adds a sequence of key-value pairs to the map (in the form of KeyValuePairs), overwriting previous information.
    static member setRange kvps (map : 'map) =
        map.SetRange kvps

    ///Removes a key from the map.
    static member remove k (map : 'map) =
        map.Remove k

    ///Removes a number of keys from the map.
    static member removeRange ks (map : 'map) =
        map.RemoveRange ks

    ///Merges this map with the specified sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    static member merge kvps f (map : 'map) =
        map.Merge(kvps, toFunc3 f)

    ///Joins this map with a sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    static member join kvps f (map : 'map) =
        map.Join(kvps, toFunc3 f)

    ///Removes all the keys present in a sequence of key-value pairs, taken as another map. The value type of the map may be different.
    static member minus kvps (map : 'map) =
        map.Subtract(kvps)

    ///Applies a subtraction function on each key-value pair present in both this map, and the specified other map. If the function returns None, the key is removed.
    static member minusWith kvps f (map : 'map) =
        map.Subtract(kvps, (fun a b c -> f a b c |> toOption) |> toFunc3)

    static member mapEquals other (map : 'map) =
        map.MapEquals(other)

    static member mapEqualsWith (eq : _ IEq) other (map : 'map) = 
        map.MapEquals(other, eq)

    static member mapEqualsWithCmp (cmp : _ ICmp) other (map : 'map) =
        map.MapEquals(other, cmp)

    ///Returns a new map consisting of only those key-value pairs present in exactly one map.
    static member difference kvps (map : 'map) =
        map.Difference(kvps)

    ///Returns a sequence of keys.
    static member keys (map : 'map) = map.Keys

    ///Returns a sequence of values.
    static member values (map : 'map) = map.Values

    ///Returns the value with the specified key.
    static member get key (map : 'map) = map.[key]

    ///Returns the value associated with the specified key, or None.
    static member tryGet key (map : 'map) = map.TryGet(key) |> fromOption

    ///Returns true if the map contains the specified key.
    static member containsKey key (map : 'map) = map.ContainsKey(key)
    
[<AbstractClass>]
type FunqSetBase<'elem, 'set when 'set :> AbstractSet<'elem, 'set>> internal () =
    inherit FunqModuleBase<'elem, 'set, ISetBuilder<'elem, 'set>>()

    ///Returns true if the set contains the value.
    static member contains value (set : 'set) = set.Contains(value)

    ///Returns the intersection of the two sets.
    static member intersect (other : _ seq) (set : 'set) = set.Intersect(other)

    ///Returns the union of the two sets.
    static member union (other : _ seq) (set : 'set) = set.Union(other)

    ///Returns 'set' minus the elements of 'other'.
    static member except (other : _ seq) (set :'set) = set.Except(other)

    ///Returns 'other' minus the elements of 'set'
    static member exceptInverse (other :_ seq) (set : 'set) = set.ExceptInverse(other)

    ///Returns the symmetric difference between 'set' and 'other'.
    static member difference (other : _ seq) (set :'set) = set.Difference(other)

    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    static member relates (other : _ seq) (set :'set) = set.RelatesTo(other)

    ///Returns true if 'set' is a subset of 'other'
    static member isSubsetOf (other : _ seq) (set : 'set) = set.IsSubsetOf(other)

    ///Returns true if 'set' is a superset of 'other'
    static member isSuperOf (other : _ seq) (set : 'set) = set.IsSupersetOf(other)

    ///Returns true if 'set' is equal to 'other'
    static member isEqual (other : _ seq) (set : 'set) = set.SetEquals(other)

    ///Returns true if 'set' is a proper subset of 'other'
    static member isProperSubsetOf (other : _ seq) (set : 'set) = set.IsProperSubsetOf(other)

    ///Returns true if 'set' is a proper superset of 'other'
    static member isProperSuperOf (other : _ seq) (set : 'set) = set.IsProperSupersetOf(other)

    ///Returns true if 'set' is disjoint with (shares no elements with) other.
    static member isDisjointWith (other : _ seq) (set : 'set) = set.IsDisjointWith(other)
    
[<AbstractClass>]
type FunqSeqModule<'elem, 'seq when 'seq :> AbstractSequential<'elem,'seq>> internal () =
    inherit FunqModuleBase<'elem, 'seq, ISequentialBuilder<'elem, 'seq>>()
    ///Returns the first element of the collection
    static member first (collection : 'seq) = collection.First

    ///Returns the last element of the collection
    static member last (collection : 'seq) = collection.Last

    ///Returns the first element of the collection or None.
    static member tryFirst (collection : 'seq) = collection.TryFirst |> fromOption

    ///Returns the last element of the collection or None.
    static member tryLast (collection : 'seq) = collection.TryLast |> fromOption

    ///Returns a slice of the collection, starting from 'i1' and ending with 'i2'.
    static member slice(i1, i2) (collection : 'seq) = collection.[i1, i2]

    ///Iterates over the collection from end to start.
    static member iterBack f (collection : 'seq) = collection.ForEachBack(toAction f)

    ///Iterates over the collection from end to start while 'f' is true.
    static member iterBackWhile f (collection : 'seq) = collection.ForEachBackWhile(toFunc1 f)

    ///Folds over the collection from end to start.
    static member foldBack initial f (collection : 'seq) = collection.AggregateBack(initial, toFunc2 f)

    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    static member reduceBack f (collection : 'seq) = collection.AggregateBack(toFunc2 f)

    ///Returns the first 'n' elements of the collection.
    static member take n (collection : 'seq) = collection.Take(n)

    ///Returns the first elements of the collection until 'f' returns false.
    static member takeWhile f (collection : 'seq) = collection.TakeWhile(toFunc1 f)

    ///Skips over 'n' elements of the collection
    static member skip n (collection : 'seq) = collection.Skip(n)

    ///Skips over elements of the collection until 'f' returns false.
    static member skipWhile f (collection : 'seq) = collection.SkipWhile(toFunc1 f)

    static member tryNth index (collection : 'seq) = collection.TryGet index |> fromOption

    ///Returns the nth element of the collection.
    static member nth index (collection : 'seq) = collection.[index]

    ///Returns the last element of the collection that satisfies 'f'
    static member findLast f (collection : 'seq) = collection.FindLast(toFunc1 f) |> fromOption

    ///Returns the index of the last element of the collection that satisfies 'f'
    static member findIndex f (collection : 'seq) = collection.FindIndex(toFunc1 f) |> fromOption

[<EditorBrowsable(EditorBrowsableState.Never)>]
type FunqVectorModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqVector<'elem>>()
    static let _instance  = FunqVectorModule<'elem>()
    static member internal instance = _instance

    ///Returns the empty vector.
    member x.empty = FunqVector<'elem>.Empty

    ///Returns a vector consisting of 'value'
    member x.ofItem value= FunqVector<'elem>.Empty <+ value

    ///Adds an element to the end of the collection.
    member x.addLast value (collection : 'elem FunqVector)  = collection.AddLast(value)

    ///Adds a sequence of elements to the end.
    member x.addLastRange vs (collection :'elem FunqVector) = collection.AddLastRange(vs)

    member x.addFirstRange vs (collection : 'elem FunqVector) = collection.AddFirstRange vs

    member x.insertRange i vs (collection : 'elem FunqVector) = collection.InsertRange(i,vs)

    ///Removes the last element of the collection.
    member x.removeLast (collection :'elem FunqVector) = collection.RemoveLast()

    ///Updates the value of the element at the specified index.
    member x.update index value (collection : 'elem FunqVector) = collection.Update(index, value)

    ///Maps over the collection.
    member x.ofSeq vs = FunqVector.ToFunqVector(vs)

type FunqListModule<'elem> internal () =
    inherit FunqSeqModule<'elem, FunqList<'elem>>()
    static let _instance  = FunqListModule<'elem>()
    static member internal instance = _instance
    member x.ofItem value= FunqList<'elem>.Empty <+ value
    member x.empty = FunqList<'elem>.Empty
    member x.addLast value (collection :'elem FunqList) = collection.AddLast(value)
    member x.addLastRange vs (collection :'elem FunqList) = collection.AddLastRange(vs)
    member x.addFirst value (collection:'elem FunqList) = collection.AddFirst(value)
    member x.addFirstRange vs (collection :'elem FunqList) = collection.AddFirstRange(vs)
    member x.insert index value (collection :'elem FunqList) = collection.Insert(index,value)
    member x.insertRange index values (collection : 'elem FunqList) = collection.InsertRange(index, values)
    member x.remove index (collection :'elem FunqList) = collection.RemoveAt(index)
    member x.update index value (collection :'elem FunqList) = collection.Update(index,value)
    member x.ofSeq vs = FunqList.ToFunqList(vs)

type FunqMapModule<'key, 'value> internal () =
    inherit FunqMapBase<'key, 'value, FunqMap<'key,'value>>()
    static let _instance = FunqMapModule<'key,'value>()
    static member internal instance = _instance
    member x.emptyWith(eq : Eq<'key>) = FunqMap<'key,'value>.Empty(eq)
    member x.empty = FunqMap<'key,'value>.Empty()
    member x.ofSeq vs = FunqMap.ToFunqMap(vs)
    member x.ofSeqWith eq vs = FunqMap.ToFunqMap(vs, eq)

type FunqSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqSet<'elem>>()
    static let _instance = FunqSetModule<'elem>()
    static member internal instance = _instance
    member x.emptyWith(Eq : Eq<'elem>) = FunqSet<'elem>.Empty(Eq)
    member x.empty = FunqSet<'elem>.Empty()
    member x.ofSeq vs = FunqSet.ToFunqSet(vs)
    member x.ofSeqWith eq vs = FunqSet.ToFunqSet(vs, eq)

type FunqOrderedMapModule<'key, 'value> internal () =
    inherit FunqMapBase<'key,'value, FunqOrderedMap<'key,'value>>()
    static let _instance = FunqOrderedMapModule<'key,'value>()
    static member internal instance = _instance
    member x.emptyWith(cm : Cmp<'key>) = FunqOrderedMap<'key,'value>.Empty(cm)
    member x.ofSeq vs = FunqOrderedMap.ToFunqOrderedMap vs
    member x.ofSeqWith cmp vs = FunqOrderedMap.ToFunqOrderedMap(vs, cmp)
    member x.byOrder i (map : FunqOrderedMap<_,_>) = map.ByOrder i |> Kvp.ToTuple
    member x.min (map : FunqOrderedMap<_,_>) = map.MinItem |> Kvp.ToTuple
    member x.max (map : FunqOrderedMap<_,_>) = map.MaxItem |> Kvp.ToTuple
    member x.removeMin (map : FunqOrderedMap<_,_>) = map.RemoveMin()
    member x.removeMax (map : FunqOrderedMap<_,_>) = map.RemoveMax()

type FunqOrderedSetModule<'elem> internal () = 
    inherit FunqSetBase<'elem, FunqOrderedSet<'elem>>()
    static let _instance = FunqOrderedSetModule<'elem>()
    static member internal instance = _instance
    member x.emptyWith(cm : Cmp<'elem>) = FunqOrderedSet<'elem>.Empty(cm)
    member x.ofSeq (vs : 'a seq when 'a : comparison) = FunqOrderedSet.ToFunqOrderedSet(vs, Cmp<'a>.Default)
    member x.ofSeqWith cmp vs = FunqOrderedSet.ToFunqOrderedSet(vs, cmp)
    member x.byOrder i (set : _ FunqOrderedSet) = set.ByOrder i
    member x.min (set : _ FunqOrderedSet) = set.MinItem
    member x.max (set : _ FunqOrderedSet) = set.MaxItem
    member x.removeMin (set :_ FunqOrderedSet) = set.RemoveMin()
    member x.removeMax (set :_ FunqOrderedSet) = set.RemoveMax()
    
