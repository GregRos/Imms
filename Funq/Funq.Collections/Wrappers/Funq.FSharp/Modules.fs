namespace Funq.FSharp
open Funq.FSharp
open Funq.FSharp.Implementation
open Funq
module FunqVector = 
    let isEmpty (collection : FunqVector<'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqVector<'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqVector<'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqVector<'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqVector<'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqVector<'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqVector<'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqVector<'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqVector<'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqVector<'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqVector<'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqVector<'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqVector<'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqVector<'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqVector<'v>) = collection.Count(toFunc1 f)
    let first (collection : FunqVector<'v>) = collection.First
    
    ///Returns the last element of the collection
    let last (collection : FunqVector<'v>) = collection.Last
    
    ///Returns the first element of the collection or None.
    let tryFirst (collection : FunqVector<'v>) = collection.TryFirst |> fromOption
    
    ///Returns the last element of the collection or None.
    let tryLast (collection : FunqVector<'v>) = collection.TryLast |> fromOption
    
    ///Returns a slice of the collection, starting from 'i1' and ending with 'i2'.
    let slice(i1, i2) (collection : FunqVector<'v>) = collection.[i1, i2]
    
    ///Iterates over the collection from end to start.
    let iterBack f (collection : FunqVector<'v>) = collection.ForEachBack(toAction f)
    
    ///Iterates over the collection from end to start while 'f' is true.
    let iterBackWhile f (collection : FunqVector<'v>) = collection.ForEachBackWhile(toFunc1 f)
    
    ///Folds over the collection from end to start.
    let foldBack initial f (collection : FunqVector<'v>) = collection.AggregateBack(initial, toFunc2 f)
    
    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    let reduceBack f (collection : FunqVector<'v>) = collection.AggregateBack(toFunc2 f)
    
    ///Returns the first 'n' elements of the collection.
    let take n (collection : FunqVector<'v>) = collection.Take(n)
    
    ///Returns the first elements of the collection until 'f' returns false.
    let takeWhile f (collection : FunqVector<'v>) = collection.TakeWhile(toFunc1 f)
    
    ///Skips over 'n' elements of the collection
    let skip n (collection : FunqVector<'v>) = collection.Skip(n)
    
    ///Skips over elements of the collection until 'f' returns false.
    let skipWhile f (collection : FunqVector<'v>) = collection.SkipWhile(toFunc1 f)
    
    let tryNth index (collection : FunqVector<'v>) = collection.TryGet index |> fromOption
    
    ///Returns the nth element of the collection.
    let nth index (collection : FunqVector<'v>) = collection.[index]
    
    ///Returns the last element of the collection that satisfies 'f'
    let findLast f (collection : FunqVector<'v>) = collection.FindLast(toFunc1 f) |> fromOption
    
    ///Returns the index of the last element of the collection that satisfies 'f'
    let findIndex f (collection : FunqVector<'v>) = collection.FindIndex(toFunc1 f) |> fromOption
    
    ///Returns the empty vector.
    let empty<'v> = FunqVector<'v>.Empty
    
    ///Returns a vector consisting of 'value'
    let ofItem value = empty.AddLast value
    
    ///Adds an element to the end of the collection.
    let addLast value (collection : FunqVector<'v>)  = collection.AddLast(value)
    
    ///Adds a sequence of elements to the end.
    let addLastRange vs (collection :FunqVector<'v>) = collection.AddLastRange(vs)
    
    ///Adds a sequence of elements to the beginning.
    let addFirstRange vs (collection : FunqVector<'v>) = collection.AddFirstRange vs
    
    ///Inserts a sequence of elements at the specified index.
    let insertRange i vs (collection : FunqVector<'v>) = collection.InsertRange(i,vs)
    
    ///Removes the last element of the collection.
    let removeLast (collection :'elem FunqVector) = collection.RemoveLast()
    
    ///Updates the value of the element at the specified index.
    let update index value (collection : FunqVector<'v>) = collection.Update(index, value)
    
    ///Maps over the collection.
    let ofSeq vs = empty.AddLastRange vs

module FunqList = 
    let isEmpty (collection : FunqList<'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqList<'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqList<'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqList<'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqList<'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqList<'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqList<'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqList<'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqList<'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqList<'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqList<'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqList<'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqList<'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqList<'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqList<'v>) = collection.Count(toFunc1 f)
    let first (collection : FunqList<'v>) = collection.First
    
    ///Returns the last element of the collection
    let last (collection : FunqList<'v>) = collection.Last
    
    ///Returns the first element of the collection or None.
    let tryFirst (collection : FunqList<'v>) = collection.TryFirst |> fromOption
    
    ///Returns the last element of the collection or None.
    let tryLast (collection : FunqList<'v>) = collection.TryLast |> fromOption
    
    ///Returns a slice of the collection, starting from 'i1' and ending with 'i2'.
    let slice(i1, i2) (collection : FunqList<'v>) = collection.[i1, i2]
    
    ///Iterates over the collection from end to start.
    let iterBack f (collection : FunqList<'v>) = collection.ForEachBack(toAction f)
    
    ///Iterates over the collection from end to start while 'f' is true.
    let iterBackWhile f (collection : FunqList<'v>) = collection.ForEachBackWhile(toFunc1 f)
    
    ///Folds over the collection from end to start.
    let foldBack initial f (collection : FunqList<'v>) = collection.AggregateBack(initial, toFunc2 f)
    
    ///Folds over the collection from end to start, beginning by applying 'f' to the first two elements.
    let reduceBack f (collection : FunqList<'v>) = collection.AggregateBack(toFunc2 f)
    
    ///Returns the first 'n' elements of the collection.
    let take n (collection : FunqList<'v>) = collection.Take(n)
    
    ///Returns the first elements of the collection until 'f' returns false.
    let takeWhile f (collection : FunqList<'v>) = collection.TakeWhile(toFunc1 f)
    
    ///Skips over 'n' elements of the collection
    let skip n (collection : FunqList<'v>) = collection.Skip(n)
    
    ///Skips over elements of the collection until 'f' returns false.
    let skipWhile f (collection : FunqList<'v>) = collection.SkipWhile(toFunc1 f)
    
    let tryNth index (collection : FunqList<'v>) = collection.TryGet index |> fromOption
    
    ///Returns the nth element of the collection.
    let nth index (collection : FunqList<'v>) = collection.[index]
    
    ///Returns the last element of the collection that satisfies 'f'
    let findLast f (collection : FunqList<'v>) = collection.FindLast(toFunc1 f) |> fromOption
    
    ///Returns the index of the last element of the collection that satisfies 'f'
    let findIndex f (collection : FunqList<'v>) = collection.FindIndex(toFunc1 f) |> fromOption
    
    ///Returns the empty vector.
    let empty<'v> = FunqList<'v>.Empty
    
    ///Returns a vector consisting of 'value'
    let ofItem value = empty.AddLast value
    
    ///Adds an element to the end of the collection.
    let addLast value (collection : FunqList<'v>)  = collection.AddLast(value)
    
    ///Adds a sequence of elements to the end.
    let addLastRange vs (collection :FunqList<'v>) = collection.AddLastRange(vs)
    
    ///Adds a sequence of elements to the beginning.
    let addFirstRange vs (collection : FunqList<'v>) = collection.AddFirstRange vs
    
    ///Inserts a sequence of elements at the specified index.
    let insertRange i vs (collection : FunqList<'v>) = collection.InsertRange(i,vs)
    
    ///Removes the last element of the collection.
    let removeLast (collection :'elem FunqVector) = collection.RemoveLast()
    
    ///Updates the value of the element at the specified index.
    let update index value (collection : FunqList<'v>) = collection.Update(index, value)
    
    ///Maps over the collection.
    let ofSeq vs = empty.AddLastRange vs
     
module FunqSet = 
    let isEmpty (collection : FunqSet<'k>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqSet<'k>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqSet<'k>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqSet<'k>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqSet<'k>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqSet<'k>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqSet<'k>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqSet<'k>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqSet<'k>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqSet<'k>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqSet<'k>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqSet<'k>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqSet<'k>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqSet<'k>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqSet<'k>) = collection.Count(toFunc1 f)
     ///Returns true if the set contains the value.
    let contains value (set : FunqSet<'k>) = set.Contains(value)
    
    ///Returns the intersection of the two sets.
    let intersect (other : _ seq) (set : FunqSet<'k>) = set.Intersect(other)
    
    ///Returns the union of the two sets.
    let union (other : _ seq) (set : FunqSet<'k>) = set.Union(other)
    
    ///Returns FunqSet<'k>' minus the elements of 'other'.
    let except (other : _ seq) (set :FunqSet<'k>) = set.Except(other)
    
    ///Returns 'other' minus the elements of FunqSet<'k>'
    let exceptInverse (other :_ seq) (set : FunqSet<'k>) = set.ExceptInverse(other)
    
    ///Returns the symmetric difference between FunqSet<'k>' and 'other'.
    let difference (other : _ seq) (set :FunqSet<'k>) = set.Difference(other)
    
    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    let relates (other : _ seq) (set :FunqSet<'k>) = set.RelatesTo(other)
    
    ///Returns true if FunqSet<'k>' is a subset of 'other'
    let isSubsetOf (other : _ seq) (set : FunqSet<'k>) = set.IsSubsetOf(other)
    
    ///Returns true if FunqSet<'k>' is a superset of 'other'
    let isSuperOf (other : _ seq) (set : FunqSet<'k>) = set.IsSupersetOf(other)
    
    ///Returns true if FunqSet<'k>' is equal to 'other'
    let isEqual (other : _ seq) (set : FunqSet<'k>) = set.SetEquals(other)
    
    ///Returns true if FunqSet<'k>' is a proper subset of 'other'
    let isProperSubsetOf (other : _ seq) (set : FunqSet<'k>) = set.IsProperSubsetOf(other)
    
    ///Returns true if FunqSet<'k>' is a proper superset of 'other'
    let isProperSuperOf (other : _ seq) (set : FunqSet<'k>) = set.IsProperSupersetOf(other)
    
    ///Returns true if FunqSet<'k>' is disjoint with (shares no elements with) other.
    let isDisjointWith (other : _ seq) (set : FunqSet<'k>) = set.IsDisjointWith(other)

module FunqOrderedSet = 
    let isEmpty (collection : FunqOrderedSet<'k>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqOrderedSet<'k>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqOrderedSet<'k>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqOrderedSet<'k>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqOrderedSet<'k>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqOrderedSet<'k>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqOrderedSet<'k>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqOrderedSet<'k>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqOrderedSet<'k>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqOrderedSet<'k>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqOrderedSet<'k>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqOrderedSet<'k>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqOrderedSet<'k>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqOrderedSet<'k>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqOrderedSet<'k>) = collection.Count(toFunc1 f)
     ///Returns true if the set contains the value.
    let contains value (set : FunqOrderedSet<'k>) = set.Contains(value)
    
    ///Returns the intersection of the two sets.
    let intersect (other : _ seq) (set : FunqOrderedSet<'k>) = set.Intersect(other)
    
    ///Returns the union of the two sets.
    let union (other : _ seq) (set : FunqOrderedSet<'k>) = set.Union(other)
    
    ///Returns FunqOrderedSet<'k>' minus the elements of 'other'.
    let except (other : _ seq) (set :FunqOrderedSet<'k>) = set.Except(other)
    
    ///Returns 'other' minus the elements of FunqOrderedSet<'k>'
    let exceptInverse (other :_ seq) (set : FunqOrderedSet<'k>) = set.ExceptInverse(other)
    
    ///Returns the symmetric difference between FunqOrderedSet<'k>' and 'other'.
    let difference (other : _ seq) (set :FunqOrderedSet<'k>) = set.Difference(other)
    
    ///Returns the set-theoretic relation (may be more than one) between the two sets.
    let relates (other : _ seq) (set :FunqOrderedSet<'k>) = set.RelatesTo(other)
    
    ///Returns true if FunqOrderedSet<'k>' is a subset of 'other'
    let isSubsetOf (other : _ seq) (set : FunqOrderedSet<'k>) = set.IsSubsetOf(other)
    
    ///Returns true if FunqOrderedSet<'k>' is a superset of 'other'
    let isSuperOf (other : _ seq) (set : FunqOrderedSet<'k>) = set.IsSupersetOf(other)
    
    ///Returns true if FunqOrderedSet<'k>' is equal to 'other'
    let isEqual (other : _ seq) (set : FunqOrderedSet<'k>) = set.SetEquals(other)
    
    ///Returns true if FunqOrderedSet<'k>' is a proper subset of 'other'
    let isProperSubsetOf (other : _ seq) (set : FunqOrderedSet<'k>) = set.IsProperSubsetOf(other)
    
    ///Returns true if FunqOrderedSet<'k>' is a proper superset of 'other'
    let isProperSuperOf (other : _ seq) (set : FunqOrderedSet<'k>) = set.IsProperSupersetOf(other)
    
    ///Returns true if FunqOrderedSet<'k>' is disjoint with (shares no elements with) other.
    let isDisjointWith (other : _ seq) (set : FunqOrderedSet<'k>) = set.IsDisjointWith(other)

module FunqMap = 
    let isEmpty (collection : FunqMap<'k, 'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqMap<'k, 'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqMap<'k, 'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqMap<'k, 'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqMap<'k, 'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqMap<'k, 'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqMap<'k, 'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqMap<'k, 'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqMap<'k, 'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqMap<'k, 'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqMap<'k, 'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqMap<'k, 'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqMap<'k, 'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqMap<'k, 'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqMap<'k, 'v>) = collection.Count(toFunc1 f)
    ///Returns true if every key-value pair satisfies the specified predicate.
    let forAllPairs (f : 'k -> 'v -> bool) (collection : FunqMap<'k, 'v>) = collection.All(toFunc2 f)
    
    ///Returns true if any key-value pair satisfies the given predicate.
    let existsPair f (map : FunqMap<'k, 'v>) = map.Any(toFunc2 f)
    
    ///Returns the first pair (in order of iteration) that fulfills the given predicate.
    let findPair f (map : FunqMap<'k, 'v>) = map.Find(toFunc2 f)
    
    ///Applies the specified function on every key-value pair, and returns the first result that isn't None.
    let pickPair (f : 'k -> 'v -> 'out option) (map : FunqMap<'k, 'v>) = map.Pick((fun a b -> f a b |> toOption) |> toFunc2) |> fromOption
    
    ///Returns the number of key-value pairs that fulfill the specified predicate.
    let countPairs (f : 'k -> 'v -> bool) (map : FunqMap<'k, 'v>) = map.Count(toFunc2 f)
    
    ///Adds a key-value pair to the map, throwing an exception if the key already exists.
    let add k v (map : FunqMap<'k, 'v>) = 
        map.Add(k, v)
    
    ///Adds a key-value pair to the map, overwriting the previous value.
    let set k v (map : FunqMap<'k, 'v>) =
        map.Set(k, v)
    
    ///Adds a sequence of key-value pairs (in the form of 2-tuples) to the map, throwing an exception if a key already exists.
    let addPairs pairs (map : FunqMap<'k, 'v>) =
        map.AddRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs (in the form of KeyValuePairs) to the map, throwing an exception if a key already exists.
    let addRange kvps (map : FunqMap<'k, 'v>) =
        map.AddRange kvps
    
    ///Adds a sequence of key-value pairs to the map (in the form of 2-tuples), overwriting previous information.
    let setPairs pairs (map : FunqMap<'k, 'v>) =
        map.SetRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs to the map (in the form of KeyValuePairs), overwriting previous information.
    let setRange kvps (map : FunqMap<'k, 'v>) =
        map.SetRange kvps
    
    ///Removes a key from the map.
    let remove k (map : FunqMap<'k, 'v>) =
        map.Remove k
    
    ///Removes a number of keys from the map.
    let removeRange ks (map : FunqMap<'k, 'v>) =
        map.RemoveRange ks
    
    ///Merges this map with the specified sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let merge kvps f (map : FunqMap<'k, 'v>) =
        map.Merge(kvps, toFunc3 f)
    
    ///Joins this map with a sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let join kvps f (map : FunqMap<'k, 'v>) =
        map.Join(kvps, toFunc3 f)
    
    ///Removes all the keys present in a sequence of key-value pairs, taken as another map. The value type of the map may be different.
    let minus kvps (map : FunqMap<'k, 'v>) =
        map.Subtract(kvps)
    
    ///Applies a subtraction function on each key-value pair present in both this map, and the specified other map. If the function returns None, the key is removed.
    let minusWith kvps f (map : FunqMap<'k, 'v>) =
        map.Subtract(kvps, (fun a b c -> f a b c |> toOption) |> toFunc3)
    
    let mapEquals other (map : FunqMap<'k, 'v>) =
        map.MapEquals(other)
    
    let mapEqualsWith (eq : _ IEq) other (map : FunqMap<'k, 'v>) = 
        map.MapEquals(other, eq)
    
    let mapEqualsWithCmp (cmp : _ ICmp) other (map : FunqMap<'k, 'v>) =
        map.MapEquals(other, cmp)
    
    ///Returns a new map consisting of only those key-value pairs present in exactly one map.
    let difference kvps (map : FunqMap<'k, 'v>) =
        map.Difference(kvps)
    
    ///Returns a sequence of keys.
    let keys (map : FunqMap<'k, 'v>) = map.Keys
    
    ///Returns a sequence of values.
    let values (map : FunqMap<'k, 'v>) = map.Values
    
    ///Returns the value with the specified key.
    let get key (map : FunqMap<'k, 'v>) = map.[key]
    
    ///Returns the value associated with the specified key, or None.
    let tryGet key (map : FunqMap<'k, 'v>) = map.TryGet(key) |> fromOption
    
    ///Returns true if the map contains the specified key.
    let containsKey key (map : FunqMap<'k, 'v>) = map.ContainsKey(key)

module FunqOrderedMap = 
    let isEmpty (collection : FunqOrderedMap<'k, 'v>) = collection.IsEmpty
    
    ///Returns the length of the collection. O(1)
    let length (collection : FunqOrderedMap<'k, 'v>) = collection.Length
    
    ///Iterates over the collection.
    let iter f (collection : FunqOrderedMap<'k, 'v>) = collection.ForEach(toAction f)  
    
    ///Iterates over the collection with 'f' returns true, stops when it returns 'false'.
    let iterWhile f (collection : FunqOrderedMap<'k, 'v>) = collection.ForEachWhile(toFunc1 f)
    
    ///Folds over the collection, in the default iteration order.
    let fold initial f (collection : FunqOrderedMap<'k, 'v>) = collection.Aggregate(initial, toFunc2 f)
    
    ///Folds over the collection, starting by applying the function to the first two elements.
    let reduce f (collection : FunqOrderedMap<'k, 'v>) = collection.Aggregate(toFunc2 f)
    
    ///Returns the single element present in the collection, or throws an exception if there is more than one element, or if there are no elements.
    let single (collection : FunqOrderedMap<'k, 'v>) = collection.Single()
    
    ///Converts the specified collection to an array.
    let toArray (collection : FunqOrderedMap<'k, 'v>) = collection.ToArray()
    
    ///Converts the elements of the collection to a string, using the specified separator.
    let print sep (collection : FunqOrderedMap<'k, 'v>)  = collection.Print(sep, sprintf "%A" |> toFunc1)
    
    ///Converts the elements of the collection to a string using the specified function, using the specified separator.
    let printWith sep format (collection : FunqOrderedMap<'k, 'v>) = collection.Print(sep, (sprintf format) |> toFunc1)
    
    ///Finds an element that satisfies 'f', or None.
    let find f (collection : FunqOrderedMap<'k, 'v>) = collection.Find(toFunc1 f) |> fromOption
    
    ///Returns the first element from which the function returns Some, or returns None.
    let pick f (collection : FunqOrderedMap<'k, 'v>) = collection.Pick(f >> toOption |> toFunc1) |> fromOption
    
    ///Returns true if an element satisfying 'f' exists in the collection.
    let exists f (collection : FunqOrderedMap<'k, 'v>) = collection.Any(toFunc1 f)
    
    ///Returns true if all the elements in the collection satisfies 'f'.
    let forAll f (collection : FunqOrderedMap<'k, 'v>) = collection.All(toFunc1 f)
    
    ///Counts the number of elements satisfying 'f' in the collection.
    let count f (collection : FunqOrderedMap<'k, 'v>) = collection.Count(toFunc1 f)
    ///Returns true if every key-value pair satisfies the specified predicate.
    let forAllPairs (f : 'k -> 'v -> bool) (collection : FunqOrderedMap<'k, 'v>) = collection.All(toFunc2 f)
    
    ///Returns true if any key-value pair satisfies the given predicate.
    let existsPair f (map : FunqOrderedMap<'k, 'v>) = map.Any(toFunc2 f)
    
    ///Returns the first pair (in order of iteration) that fulfills the given predicate.
    let findPair f (map : FunqOrderedMap<'k, 'v>) = map.Find(toFunc2 f)
    
    ///Applies the specified function on every key-value pair, and returns the first result that isn't None.
    let pickPair (f : 'k -> 'v -> 'out option) (map : FunqOrderedMap<'k, 'v>) = map.Pick((fun a b -> f a b |> toOption) |> toFunc2) |> fromOption
    
    ///Returns the number of key-value pairs that fulfill the specified predicate.
    let countPairs (f : 'k -> 'v -> bool) (map : FunqOrderedMap<'k, 'v>) = map.Count(toFunc2 f)
    
    ///Adds a key-value pair to the map, throwing an exception if the key already exists.
    let add k v (map : FunqOrderedMap<'k, 'v>) = 
        map.Add(k, v)
    
    ///Adds a key-value pair to the map, overwriting the previous value.
    let set k v (map : FunqOrderedMap<'k, 'v>) =
        map.Set(k, v)
    
    ///Adds a sequence of key-value pairs (in the form of 2-tuples) to the map, throwing an exception if a key already exists.
    let addPairs pairs (map : FunqOrderedMap<'k, 'v>) =
        map.AddRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs (in the form of KeyValuePairs) to the map, throwing an exception if a key already exists.
    let addRange kvps (map : FunqOrderedMap<'k, 'v>) =
        map.AddRange kvps
    
    ///Adds a sequence of key-value pairs to the map (in the form of 2-tuples), overwriting previous information.
    let setPairs pairs (map : FunqOrderedMap<'k, 'v>) =
        map.SetRange(pairs |> Seq.map (Kvp.Of))
    
    ///Adds a sequence of key-value pairs to the map (in the form of KeyValuePairs), overwriting previous information.
    let setRange kvps (map : FunqOrderedMap<'k, 'v>) =
        map.SetRange kvps
    
    ///Removes a key from the map.
    let remove k (map : FunqOrderedMap<'k, 'v>) =
        map.Remove k
    
    ///Removes a number of keys from the map.
    let removeRange ks (map : FunqOrderedMap<'k, 'v>) =
        map.RemoveRange ks
    
    ///Merges this map with the specified sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let merge kvps f (map : FunqOrderedMap<'k, 'v>) =
        map.Merge(kvps, toFunc3 f)
    
    ///Joins this map with a sequence of key-value pairs, viewed as another map, using the specified function to resolve collisions.
    let join kvps f (map : FunqOrderedMap<'k, 'v>) =
        map.Join(kvps, toFunc3 f)
    
    ///Removes all the keys present in a sequence of key-value pairs, taken as another map. The value type of the map may be different.
    let minus kvps (map : FunqOrderedMap<'k, 'v>) =
        map.Subtract(kvps)
    
    ///Applies a subtraction function on each key-value pair present in both this map, and the specified other map. If the function returns None, the key is removed.
    let minusWith kvps f (map : FunqOrderedMap<'k, 'v>) =
        map.Subtract(kvps, (fun a b c -> f a b c |> toOption) |> toFunc3)
    
    let mapEquals other (map : FunqOrderedMap<'k, 'v>) =
        map.MapEquals(other)
    
    let mapEqualsWith (eq : _ IEq) other (map : FunqOrderedMap<'k, 'v>) = 
        map.MapEquals(other, eq)
    
    let mapEqualsWithCmp (cmp : _ ICmp) other (map : FunqOrderedMap<'k, 'v>) =
        map.MapEquals(other, cmp)
    
    ///Returns a new map consisting of only those key-value pairs present in exactly one map.
    let difference kvps (map : FunqOrderedMap<'k, 'v>) =
        map.Difference(kvps)
    
    ///Returns a sequence of keys.
    let keys (map : FunqOrderedMap<'k, 'v>) = map.Keys
    
    ///Returns a sequence of values.
    let values (map : FunqOrderedMap<'k, 'v>) = map.Values
    
    ///Returns the value with the specified key.
    let get key (map : FunqOrderedMap<'k, 'v>) = map.[key]
    
    ///Returns the value associated with the specified key, or None.
    let tryGet key (map : FunqOrderedMap<'k, 'v>) = map.TryGet(key) |> fromOption
    
    ///Returns true if the map contains the specified key.
    let containsKey key (map : FunqOrderedMap<'k, 'v>) = map.ContainsKey(key)
    

