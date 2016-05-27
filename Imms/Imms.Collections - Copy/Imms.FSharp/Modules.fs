namespace Imms.FSharp
open Imms.FSharp
open Imms.FSharp.Implementation
open Imms
open Imms.FSharp.Operators
#nowarn "44"
///A module for working with ImmVector collections -- immutable high-performance vector collections.
module ImmVector = 
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmVector<'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmVector<'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmVector<'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmVector<'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmVector<'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmVector<'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmVector<'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmVector<'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmVector<'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmVector<'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmVector<'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmVector<'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmVector<'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmVector<'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmVector<'v>) = collection.Count(toFunc1 f)
    ///Returns the first element of the collection.
    let first (collection : ImmVector<'v>) = collection.First
    
    ///Returns the last element of the collection
    let last (collection : ImmVector<'v>) = collection.Last
    
    ///Returns the first element of the collection or None.
    let tryFirst (collection : ImmVector<'v>) = collection.TryFirst |> fromOption
    
    ///Returns the last element of the collection or None.
    let tryLast (collection : ImmVector<'v>) = collection.TryLast |> fromOption
    
    ///Returns a slice of the collection, starting from 'i1' and ending with 'i2'.
    let slice(i1, i2) (collection : ImmVector<'v>) = collection.[i1, i2]
    
    ///Iterates over the collection from end to start.
    let iterBack f (collection : ImmVector<'v>) = collection.ForEachBack(toAction f)
    
    ///Iterates over the collection from end to start while 'f' is true.
    let iterBackWhile f (collection : ImmVector<'v>) = collection.ForEachBackWhile(toFunc1 f)
    
    ///Folds over the collection from end to start.
    let foldBack initial f (collection : ImmVector<'v>) = collection.AggregateBack(initial, toFunc2 f)
    
    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    let reduceBack f (collection : ImmVector<'v>) = collection.AggregateBack(toFunc2 f)
    
    ///Returns the first 'n' elements of the collection.
    let take n (collection : ImmVector<'v>) = collection.Take(n)
    
    ///Returns the first elements of the collection until 'f' returns false.
    let takeWhile f (collection : ImmVector<'v>) = collection.TakeWhile(toFunc1 f)
    
    ///Skips over 'n' elements of the collection
    let skip n (collection : ImmVector<'v>) = collection.Skip(n)
    
    ///Skips over elements of the collection until 'f' returns false.
    let skipWhile f (collection : ImmVector<'v>) = collection.SkipWhile(toFunc1 f)
    
    let tryNth index (collection : ImmVector<'v>) = collection.TryGet index |> fromOption
    
    ///Returns the nth element of the collection.
    let nth index (collection : ImmVector<'v>) = collection.[index]
    
    ///Returns the last element of the collection that satisfies 'f'
    let findLast f (collection : ImmVector<'v>) = collection.FindLast(toFunc1 f) |> fromOption
    
    ///Returns the index of the last element of the collection that satisfies 'f'
    let findIndex f (collection : ImmVector<'v>) = collection.FindIndex(toFunc1 f) |> fromOption
    
    ///Returns the empty collection.
    let empty<'v> = ImmVector<'v>.Empty
    
    ///Returns a vector consisting of 'value'
    let ofItem value = empty.AddLast value
    
    ///Adds an element to the end of the collection.
    let addLast value (collection : ImmVector<'v>)  = collection.AddLast(value)
    
    ///Adds a sequence of elements to the end.
    let addLastRange vs (collection :ImmVector<'v>) = collection.AddLastRange(vs)
    
    ///Adds a sequence of elements to the beginning.
    let addFirstRange vs (collection : ImmVector<'v>) = collection.AddFirstRange vs
    
    ///Inserts a sequence of elements at the specified index.
    let insertRange i vs (collection : ImmVector<'v>) = collection.InsertRange(i,vs)
    
    ///Removes the last element of the collection.
    let removeLast (collection :'elem ImmVector) = collection.RemoveLast()
    
    ///Updates the value of the element at the specified index.
    let update index value (collection : ImmVector<'v>) = collection.Update(index, value)
    
    ///Maps over the collection.
    let ofSeq vs = empty.AddLastRange vs

///A module for working with ImmList collections -- immutable sequential collections supporting many operations.
module ImmList =
    ///Adds an element to the beginning of the collection.
    let addFirst value (collection:'elem ImmList) = collection.AddFirst(value)
    ///inserts an element at the specified index.
    let insert index value (collection :'elem ImmList) = collection.Insert(index,value)
    ///removes the element at the specified index.
    let removeAt index (collection :'elem ImmList) = collection.RemoveAt(index) 
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmList<'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmList<'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmList<'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmList<'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmList<'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmList<'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmList<'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmList<'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmList<'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmList<'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmList<'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmList<'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmList<'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmList<'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmList<'v>) = collection.Count(toFunc1 f)
    ///Returns the first element of the collection.
    let first (collection : ImmList<'v>) = collection.First
    
    ///Returns the last element of the collection
    let last (collection : ImmList<'v>) = collection.Last
    
    ///Returns the first element of the collection or None.
    let tryFirst (collection : ImmList<'v>) = collection.TryFirst |> fromOption
    
    ///Returns the last element of the collection or None.
    let tryLast (collection : ImmList<'v>) = collection.TryLast |> fromOption
    
    ///Returns a slice of the collection, starting from 'i1' and ending with 'i2'.
    let slice(i1, i2) (collection : ImmList<'v>) = collection.[i1, i2]
    
    ///Iterates over the collection from end to start.
    let iterBack f (collection : ImmList<'v>) = collection.ForEachBack(toAction f)
    
    ///Iterates over the collection from end to start while 'f' is true.
    let iterBackWhile f (collection : ImmList<'v>) = collection.ForEachBackWhile(toFunc1 f)
    
    ///Folds over the collection from end to start.
    let foldBack initial f (collection : ImmList<'v>) = collection.AggregateBack(initial, toFunc2 f)
    
    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    let reduceBack f (collection : ImmList<'v>) = collection.AggregateBack(toFunc2 f)
    
    ///Returns the first 'n' elements of the collection.
    let take n (collection : ImmList<'v>) = collection.Take(n)
    
    ///Returns the first elements of the collection until 'f' returns false.
    let takeWhile f (collection : ImmList<'v>) = collection.TakeWhile(toFunc1 f)
    
    ///Skips over 'n' elements of the collection
    let skip n (collection : ImmList<'v>) = collection.Skip(n)
    
    ///Skips over elements of the collection until 'f' returns false.
    let skipWhile f (collection : ImmList<'v>) = collection.SkipWhile(toFunc1 f)
    
    let tryNth index (collection : ImmList<'v>) = collection.TryGet index |> fromOption
    
    ///Returns the nth element of the collection.
    let nth index (collection : ImmList<'v>) = collection.[index]
    
    ///Returns the last element of the collection that satisfies 'f'
    let findLast f (collection : ImmList<'v>) = collection.FindLast(toFunc1 f) |> fromOption
    
    ///Returns the index of the last element of the collection that satisfies 'f'
    let findIndex f (collection : ImmList<'v>) = collection.FindIndex(toFunc1 f) |> fromOption
    
    ///Returns the empty collection.
    let empty<'v> = ImmList<'v>.Empty
    
    ///Returns a vector consisting of 'value'
    let ofItem value = empty.AddLast value
    
    ///Adds an element to the end of the collection.
    let addLast value (collection : ImmList<'v>)  = collection.AddLast(value)
    
    ///Adds a sequence of elements to the end.
    let addLastRange vs (collection :ImmList<'v>) = collection.AddLastRange(vs)
    
    ///Adds a sequence of elements to the beginning.
    let addFirstRange vs (collection : ImmList<'v>) = collection.AddFirstRange vs
    
    ///Inserts a sequence of elements at the specified index.
    let insertRange i vs (collection : ImmList<'v>) = collection.InsertRange(i,vs)
    
    ///Removes the last element of the collection.
    let removeLast (collection :'elem ImmVector) = collection.RemoveLast()
    
    ///Updates the value of the element at the specified index.
    let update index value (collection : ImmList<'v>) = collection.Update(index, value)
    
    ///Maps over the collection.
    let ofSeq vs = empty.AddLastRange vs
    
///A module for working with ImmSet collections -- immutable hash-based sets.
module ImmSet = 
    ///Returns an empty ImmSet that uses the specified equality comparer.
    let emptyWith(Eq : IEq<'elem>) = ImmSet<'elem>.Empty(Eq)
    ///Returns an empty ImmSet that uses the default equality comparer for the type.
    let empty<'v when 'v : equality> = ImmSet<'v>.Empty()
    ///Constructs an ImmSet from a sequence, using the default equality comparer for the type.
    let ofSeq (vs : seq<'k> when 'k : equality) = ImmSet.ToImmSet(vs)
    ///Constructs an ImmSet from a sequence, using the specified equality comparer.
    let ofSeqWith eq vs = ImmSet.ToImmSet(vs, eq)
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmSet<'k>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmSet<'k>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmSet<'k>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmSet<'k>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmSet<'k>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmSet<'k>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmSet<'k>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmSet<'k>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmSet<'k>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmSet<'k>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmSet<'k>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmSet<'k>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmSet<'k>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmSet<'k>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmSet<'k>) = collection.Count(toFunc1 f)
     ///Returns true if the set contains the value.
    let contains value (set : ImmSet<'k>) = set.Contains(value)
    
    ///Returns the intersection of the two sets.
    let intersect (other : _ seq) (set : ImmSet<'k>) = set.Intersect(other)
    
    ///Returns the union of the two sets.
    let union (other : _ seq) (set : ImmSet<'k>) = set.Union(other)
    
    ///Returns ImmSet<'k>' minus the elements of 'other'.
    let except (other : _ seq) (set :ImmSet<'k>) = set.Except(other)
    
    ///Returns 'other' minus the elements of ImmSet<'k>'
    let exceptInverse (other :_ seq) (set : ImmSet<'k>) = set.ExceptInverse(other)
    
    ///Returns the symmetric difference between ImmSet<'k>' and 'other'.
    let difference (other : _ seq) (set :ImmSet<'k>) = set.Difference(other)
    
    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    let relates (other : _ seq) (set :ImmSet<'k>) = set.RelatesTo(other)
    
    ///Returns true if ImmSet<'k>' is a subset of 'other'
    let isSubsetOf (other : _ seq) (set : ImmSet<'k>) = set.IsSubsetOf(other)
    
    ///Returns true if ImmSet<'k>' is a superset of 'other'
    let isSuperOf (other : _ seq) (set : ImmSet<'k>) = set.IsSupersetOf(other)
    
    ///Returns true if ImmSet<'k>' is equal to 'other'
    let isEqual (other : _ seq) (set : ImmSet<'k>) = set.SetEquals(other)
    
    ///Returns true if ImmSet<'k>' is a proper subset of 'other'
    let isProperSubsetOf (other : _ seq) (set : ImmSet<'k>) = set.IsProperSubsetOf(other)
    
    ///Returns true if ImmSet<'k>' is a proper superset of 'other'
    let isProperSuperOf (other : _ seq) (set : ImmSet<'k>) = set.IsProperSupersetOf(other)
    
    ///Returns true if ImmSet<'k>' is disjoint with (shares no elements with) other.
    let isDisjointWith (other : _ seq) (set : ImmSet<'k>) = set.IsDisjointWith(other)
///A module for working with ImmSortedSet collections -- immutable ordered sets.
module ImmSortedSet = 
    ///Returns an empty ImmSortedSet that uses the default comparer for the type.
    let empty<'k when 'k : comparison> = ImmSortedSet<'k>.Empty(null)
    ///Returns an empty ImmSortedSet that uses the specified comparer.
    let emptyWith(cm : ICmp<'elem>) = ImmSortedSet<'elem>.Empty(cm)
    ///Constructs an ImmSortedSet from a sequence, using the default comparer for the type.
    let ofSeq (vs : 'a seq when 'a : comparison) = ImmSortedSet.ToImmSortedSet(vs, null)
    ///Constructs an ImmSortedSet from a sequence, using the specified comparer.
    let ofSeqWith cmp vs = ImmSortedSet.ToImmSortedSet(vs, cmp)
    ///Returns the ith element of the set, by sort order.
    let byOrder i (set : _ ImmSortedSet) = set.ByOrder i
    ///Returns the minimal element of the set.
    let min (set : _ ImmSortedSet) = set.MinItem
    ///Returns the maximal element of the set.
    let max (set : _ ImmSortedSet) = set.MaxItem
    ///Removes the minimal element of the set.
    let removeMin (set :_ ImmSortedSet) = set.RemoveMin()
    ///Removes the maximal element of the set.
    let removeMax (set :_ ImmSortedSet) = set.RemoveMax()
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmSortedSet<'k>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmSortedSet<'k>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmSortedSet<'k>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmSortedSet<'k>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmSortedSet<'k>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmSortedSet<'k>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmSortedSet<'k>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmSortedSet<'k>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmSortedSet<'k>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmSortedSet<'k>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmSortedSet<'k>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmSortedSet<'k>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmSortedSet<'k>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmSortedSet<'k>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmSortedSet<'k>) = collection.Count(toFunc1 f)
     ///Returns true if the set contains the value.
    let contains value (set : ImmSortedSet<'k>) = set.Contains(value)
    
    ///Returns the intersection of the two sets.
    let intersect (other : _ seq) (set : ImmSortedSet<'k>) = set.Intersect(other)
    
    ///Returns the union of the two sets.
    let union (other : _ seq) (set : ImmSortedSet<'k>) = set.Union(other)
    
    ///Returns ImmSortedSet<'k>' minus the elements of 'other'.
    let except (other : _ seq) (set :ImmSortedSet<'k>) = set.Except(other)
    
    ///Returns 'other' minus the elements of ImmSortedSet<'k>'
    let exceptInverse (other :_ seq) (set : ImmSortedSet<'k>) = set.ExceptInverse(other)
    
    ///Returns the symmetric difference between ImmSortedSet<'k>' and 'other'.
    let difference (other : _ seq) (set :ImmSortedSet<'k>) = set.Difference(other)
    
    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    let relates (other : _ seq) (set :ImmSortedSet<'k>) = set.RelatesTo(other)
    
    ///Returns true if ImmSortedSet<'k>' is a subset of 'other'
    let isSubsetOf (other : _ seq) (set : ImmSortedSet<'k>) = set.IsSubsetOf(other)
    
    ///Returns true if ImmSortedSet<'k>' is a superset of 'other'
    let isSuperOf (other : _ seq) (set : ImmSortedSet<'k>) = set.IsSupersetOf(other)
    
    ///Returns true if ImmSortedSet<'k>' is equal to 'other'
    let isEqual (other : _ seq) (set : ImmSortedSet<'k>) = set.SetEquals(other)
    
    ///Returns true if ImmSortedSet<'k>' is a proper subset of 'other'
    let isProperSubsetOf (other : _ seq) (set : ImmSortedSet<'k>) = set.IsProperSubsetOf(other)
    
    ///Returns true if ImmSortedSet<'k>' is a proper superset of 'other'
    let isProperSuperOf (other : _ seq) (set : ImmSortedSet<'k>) = set.IsProperSupersetOf(other)
    
    ///Returns true if ImmSortedSet<'k>' is disjoint with (shares no elements with) other.
    let isDisjointWith (other : _ seq) (set : ImmSortedSet<'k>) = set.IsDisjointWith(other)
///A module for working with ImmMap collections -- immutable hash-based maps.
module ImmMap =
    ///Returns an empty ImmMap that uses the specified equality comparer.
    let emptyWith(eq : Eq<'key>) = ImmMap<'key,'value>.Empty(eq)
    ///Returns an empty ImmMap that uses the default equality comparer for the type.
    let empty<'key,'value when 'key : equality> = ImmMap<'key,'value>.Empty()
    ///Constructs an ImmMap from a sequence of ordered pairs, using the default equality comparer for the type.
    let ofPairsSeq (vs : seq<'key * 'value> when 'key : equality) = ImmMap.Empty() /++ (vs |> Seq.map Kvp)
    ///Constructs an ImmSet from a sequence of key-value pairs, using the default equality comparer for the type.
    let ofKvpSeq (vs : seq<Kvp<'key,'value>> when 'key : equality) = ImmMap.ToImmMap(vs)
    ///Constructs an ImmMap from a sequence of key-value pairs, using the specified equality comparer.
    let ofKvpSeqWith eq vs = ImmMap.ToImmMap(vs, eq)
    ///Constructs an ImmMap from a sequence of ordered pairs, using the specified equality comparer.
    let ofPairsSeqWith eq (vs : seq<'key * 'value>) = ImmMap.Empty(eq).AddRange(vs |> Seq.map Kvp) 
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmMap<'k, 'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmMap<'k, 'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmMap<'k, 'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmMap<'k, 'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmMap<'k, 'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmMap<'k, 'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmMap<'k, 'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmMap<'k, 'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmMap<'k, 'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmMap<'k, 'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmMap<'k, 'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmMap<'k, 'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmMap<'k, 'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmMap<'k, 'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmMap<'k, 'v>) = collection.Count(toFunc1 f)
    ///Returns true if every key-value pair satisfies the specified predicate.
    let forAllPairs (f : 'k -> 'v -> bool) (collection : ImmMap<'k, 'v>) = collection.All(toFunc2 f)
    
    ///Returns true if any key-value pair satisfies the given predicate.
    let existsPair f (map : ImmMap<'k, 'v>) = map.Any(toFunc2 f)
    
    ///Returns the first pair (in order of iteration) that fulfills the given predicate.
    let findPair f (map : ImmMap<'k, 'v>) = map.Find(toFunc2 f)
    
    ///Applies the specified function on every key-value pair, and returns the first result that isn't None.
    let pickPair (f : 'k -> 'v -> 'out option) (map : ImmMap<'k, 'v>) = map.Pick((fun a b -> f a b |> toOption) |> toFunc2) |> fromOption
    
    ///Returns the number of key-value pairs that fulfill the specified predicate.
    let countPairs (f : 'k -> 'v -> bool) (map : ImmMap<'k, 'v>) = map.Count(toFunc2 f)
    
    ///Adds a key-value pair to the map, throwing an exception if the key already exists.
    let add k v (map : ImmMap<'k, 'v>) = 
        map.Add(k, v)
    
    ///Adds a key-value pair to the map, overwriting the previous value.
    let set k v (map : ImmMap<'k, 'v>) =
        map.Set(k, v)
    
    ///Adds a sequence of key-value pairs (in the form of 2-tuples) to the map, throwing an exception if a key already exists.
    let addPairs pairs (map : ImmMap<'k, 'v>) =
        map.AddRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs (in the form of KeyValuePairs) to the map, throwing an exception if a key already exists.
    let addRange kvps (map : ImmMap<'k, 'v>) =
        map.AddRange kvps
    
    ///Adds a sequence of key-value pairs to the map (in the form of 2-tuples), overwriting previous information.
    let setPairs pairs (map : ImmMap<'k, 'v>) =
        map.SetRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs to the map (in the form of KeyValuePairs), overwriting previous information.
    let setRange kvps (map : ImmMap<'k, 'v>) =
        map.SetRange kvps
    
    ///Removes a key from the map.
    let remove k (map : ImmMap<'k, 'v>) =
        map.Remove k
    
    ///Removes a number of keys from the map.
    let removeRange ks (map : ImmMap<'k, 'v>) =
        map.RemoveRange ks
    
    ///Merges this map with the specified sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let merge kvps f (map : ImmMap<'k, 'v>) =
        map.Merge(kvps, toValSelector f)
    
    ///Joins this map with a sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let join kvps f (map : ImmMap<'k, 'v>) =
        map.Join(kvps, toValSelector f)
    
    ///Removes all the keys present in a sequence of key-value pairs, taken as another map. The value type of the map may be different.
    let minus kvps (map : ImmMap<'k, 'v>) =
        map.Subtract(kvps)
    
    ///Applies a subtraction function on each key-value pair present in both this map, and the specified other map. If the function returns None, the key is removed.
    let minusWith kvps f (map : ImmMap<'k, 'v>) =
        map.Subtract(kvps, (fun a b c -> f a b c |> toOption) |> toValSelector)
    
    let mapEquals other (map : ImmMap<'k, 'v>) =
        map.MapEquals(other)
    
    let mapEqualsWith (eq : _ IEq) other (map : ImmMap<'k, 'v>) = 
        map.MapEquals(other, eq)
    
    let mapEqualsWithCmp (cmp : _ ICmp) other (map : ImmMap<'k, 'v>) =
        map.MapEquals(other, cmp)
    
    ///Returns a new map consisting of only those key-value pairs present in exactly one map.
    let difference kvps (map : ImmMap<'k, 'v>) =
        map.Difference(kvps)
    
    ///Returns a sequence of keys.
    let keys (map : ImmMap<'k, 'v>) = map.Keys
    
    ///Returns a sequence of values.
    let values (map : ImmMap<'k, 'v>) = map.Values
    
    ///Returns the value with the specified key.
    let get key (map : ImmMap<'k, 'v>) = map.[key]
    
    ///Returns the value associated with the specified key, or None.
    let tryGet key (map : ImmMap<'k, 'v>) = map.TryGet(key) |> fromOption
    
    ///Returns true if the map contains the specified key.
    let containsKey key (map : ImmMap<'k, 'v>) = map.ContainsKey(key)
///A module for working with ImmSortedMap collections -- immutable comparison-based maps.
module ImmSortedMap = 
    ///Returns an empty ImmSortedMap that uses the default comparer for the type.
    let empty<'key, 'value when 'key : comparison> = ImmSortedMap<'key, 'value>.Empty(null)
    ///Returns an empty ImmSortedMap that uses the specified comparer.
    let emptyWith(cm : Cmp<'key>) = ImmSortedMap<'key,'value>.Empty(cm)
    ///Constructs an ImmSortedMap from a sequence of ordered pairs, using the default comparer for the type.
    let ofPairsSeq (vs : seq<'key * 'value> when 'key : comparison) = ImmSortedMap.Empty(null) /++ (vs |> Seq.map Kvp)
    ///Constructs an ImmSortedMap from a sequence of key-value pairs, using the default comparer for the type.
    let ofKvpSeq (vs : seq<Kvp<'key,'value>> when 'key : comparison) = ImmSortedMap.Empty(null) /++ (vs)
    ///Constructs an ImmSortedMap from a sequence of key-value pairs, using the specified comparer.
    let ofKvpSeqWith cmp vs = ImmSortedMap.ToImmSortedMap(vs, cmp)
    ///Constructs an ImmSortedMap from a sequence of ordered pairs, using the specified comparer.
    let ofSeqWith cmp (vs : seq<'key * 'value>) = ImmSortedMap.Empty(cmp).AddRange(vs |> Seq.map Kvp)
    ///Returns the nth element of the map, by sort order.
    let byOrder n (map : ImmSortedMap<_,_>) = map.ByOrder n |> Kvp.ToTuple
    ///Returns the minimal element of the map.
    let min (map : ImmSortedMap<_,_>) = map.MinItem |> Kvp.ToTuple
    ///Returns the maximal element of the map.
    let max (map : ImmSortedMap<_,_>) = map.MaxItem |> Kvp.ToTuple
    ///Removes the minimal element of the map.
    let removeMin (map : ImmSortedMap<_,_>) = map.RemoveMin()
    ///Removes the maximal element of the map.
    let removeMax (map : ImmSortedMap<_,_>) = map.RemoveMax()
    ///Returns true if the specified collection is empty.
    let isEmpty (collection : ImmSortedMap<'k, 'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : ImmSortedMap<'k, 'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : ImmSortedMap<'k, 'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : ImmSortedMap<'k, 'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : ImmSortedMap<'k, 'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : ImmSortedMap<'k, 'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : ImmSortedMap<'k, 'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : ImmSortedMap<'k, 'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : ImmSortedMap<'k, 'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : ImmSortedMap<'k, 'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : ImmSortedMap<'k, 'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : ImmSortedMap<'k, 'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : ImmSortedMap<'k, 'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : ImmSortedMap<'k, 'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : ImmSortedMap<'k, 'v>) = collection.Count(toFunc1 f)
    ///Returns true if every key-value pair satisfies the specified predicate.
    let forAllPairs (f : 'k -> 'v -> bool) (collection : ImmSortedMap<'k, 'v>) = collection.All(toFunc2 f)
    
    ///Returns true if any key-value pair satisfies the given predicate.
    let existsPair f (map : ImmSortedMap<'k, 'v>) = map.Any(toFunc2 f)
    
    ///Returns the first pair (in order of iteration) that fulfills the given predicate.
    let findPair f (map : ImmSortedMap<'k, 'v>) = map.Find(toFunc2 f)
    
    ///Applies the specified function on every key-value pair, and returns the first result that isn't None.
    let pickPair (f : 'k -> 'v -> 'out option) (map : ImmSortedMap<'k, 'v>) = map.Pick((fun a b -> f a b |> toOption) |> toFunc2) |> fromOption
    
    ///Returns the number of key-value pairs that fulfill the specified predicate.
    let countPairs (f : 'k -> 'v -> bool) (map : ImmSortedMap<'k, 'v>) = map.Count(toFunc2 f)
    
    ///Adds a key-value pair to the map, throwing an exception if the key already exists.
    let add k v (map : ImmSortedMap<'k, 'v>) = 
        map.Add(k, v)
    
    ///Adds a key-value pair to the map, overwriting the previous value.
    let set k v (map : ImmSortedMap<'k, 'v>) =
        map.Set(k, v)
    
    ///Adds a sequence of key-value pairs (in the form of 2-tuples) to the map, throwing an exception if a key already exists.
    let addPairs pairs (map : ImmSortedMap<'k, 'v>) =
        map.AddRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs (in the form of KeyValuePairs) to the map, throwing an exception if a key already exists.
    let addRange kvps (map : ImmSortedMap<'k, 'v>) =
        map.AddRange kvps
    
    ///Adds a sequence of key-value pairs to the map (in the form of 2-tuples), overwriting previous information.
    let setPairs pairs (map : ImmSortedMap<'k, 'v>) =
        map.SetRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs to the map (in the form of KeyValuePairs), overwriting previous information.
    let setRange kvps (map : ImmSortedMap<'k, 'v>) =
        map.SetRange kvps
    
    ///Removes a key from the map.
    let remove k (map : ImmSortedMap<'k, 'v>) =
        map.Remove k
    
    ///Removes a number of keys from the map.
    let removeRange ks (map : ImmSortedMap<'k, 'v>) =
        map.RemoveRange ks
    
    ///Merges this map with the specified sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let merge kvps f (map : ImmSortedMap<'k, 'v>) =
        map.Merge(kvps, toValSelector f)
    
    ///Joins this map with a sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let join kvps f (map : ImmSortedMap<'k, 'v>) =
        map.Join(kvps, toValSelector f)
    
    ///Removes all the keys present in a sequence of key-value pairs, taken as another map. The value type of the map may be different.
    let minus kvps (map : ImmSortedMap<'k, 'v>) =
        map.Subtract(kvps)
    
    ///Applies a subtraction function on each key-value pair present in both this map, and the specified other map. If the function returns None, the key is removed.
    let minusWith kvps f (map : ImmSortedMap<'k, 'v>) =
        map.Subtract(kvps, (fun a b c -> f a b c |> toOption) |> toValSelector)
    
    let mapEquals other (map : ImmSortedMap<'k, 'v>) =
        map.MapEquals(other)
    
    let mapEqualsWith (eq : _ IEq) other (map : ImmSortedMap<'k, 'v>) = 
        map.MapEquals(other, eq)
    
    let mapEqualsWithCmp (cmp : _ ICmp) other (map : ImmSortedMap<'k, 'v>) =
        map.MapEquals(other, cmp)
    
    ///Returns a new map consisting of only those key-value pairs present in exactly one map.
    let difference kvps (map : ImmSortedMap<'k, 'v>) =
        map.Difference(kvps)
    
    ///Returns a sequence of keys.
    let keys (map : ImmSortedMap<'k, 'v>) = map.Keys
    
    ///Returns a sequence of values.
    let values (map : ImmSortedMap<'k, 'v>) = map.Values
    
    ///Returns the value with the specified key.
    let get key (map : ImmSortedMap<'k, 'v>) = map.[key]
    
    ///Returns the value associated with the specified key, or None.
    let tryGet key (map : ImmSortedMap<'k, 'v>) = map.TryGet(key) |> fromOption
    
    ///Returns true if the map contains the specified key.
    let containsKey key (map : ImmSortedMap<'k, 'v>) = map.ContainsKey(key)
    

