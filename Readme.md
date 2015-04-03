# Funq
---

Funq is, at least primarily, a library of [persistent](http://en.wikipedia.org/wiki/Persistent_data_structure), and [immutable](http://en.wikipedia.org/wiki/Immutable_object) collections for the .NET platform.

Funq isn't the only such library out there. In fact, Microsoft have themselves written such classes, and although they haven't incorporated them into the base class library, they are available of NuGet.

However, Funq has several distinguishing and important features that make it different from pretty much any other library of this sort that has been released for .NET so far.

  1. *Performance:* Funq collections perform very well, usually far better than other collections of the same kind.
  2. *Functionality:* Funq collections provide more inherent functionality than any collection of a similar kind.
  3. *LINQ interface:* Funq collections support true collection-based operations through LINQ syntax. For example, they implement a `Select` statement that returns a collection of the same kind.
  5. *F# Integration*: Funq collections are integrated with F# in the assembly `Funq.FSharp`, which provides extension methods, module bindings, collection builders (computation expressions), and active patterns for working with the collections. 
  6. *Permissive License:* Available under the [MIT license](https://github.com/GregRos/Funq/blob/master/license.md).

**Note:** Funq collections can't be called *purely functional* because they involve mutations behind the scenes. This is for performance reasons, as object allocation is expensive in .NET. However, all the mutations Funq performs are invisible, in the sense that they do not change the state of any object visible to the user.

## Current Status
The collections are in an early version right now, and not fit for production use. They're definitely usable and the core functionality is stable, though some things like edge cases and error messages probably need to be worked out.

The collections have reached a very satisfactory level of optimization and functionality, so there is no reason to work on those for now. The following things need to be done:

1. **Design Decisions:** Although most design decisions have been made, I may still make some more decisions regarding the interface/functionality.
1. **Integrity testing:** Running sequences of operations on the collections and checking the result is correct against a data structure believed to be stable (such as the F# data structures). This is done for primary operations, but may still need to be done for secondary ones.
2. **Unit testing:** Hand-testing features by writing unit tests for them. This cannot detect rare structural corruption, which is a danger when working with complex recursive data structures, but they're important for making sure features work as intended, exceptions are thrown, and so forth.
2. **Documentation:** Document the entire API. Large parts of it are documented already, but not everything. In particular, the F# side is poorly documented. Internal documentation should also be added to explain how the classes work, in case people fork the project/want to work on it. Some of the code is tricky and messy.

## The Collections
To see up to date benchmarks you can go to the [benchmarks folder](https://github.com/GregRos/Funq/tree/master/Funq/Funq.Tests.Performance/Benchmarks). Each set of benchmarks includes charts, a CSV table, and a CSV log file with explicit information about the parameters of the benchmark.

### Sequential Collections

Funq provides two sequential collections:

 1. **FunqList:** Is not just a list. It's a very versatile sequential collection that efficiently implements pretty much every operation you can name, as you can see from the table below. This includes additional and removal from the ends, indexed operations (look up, update, insert, remove), concatenation (including in the middle), and slices. This data structure is implemented as a 2-3-4 finger tree. 

 1. **FunqVector:** This collection offers less functionality than FunqList, but performs a lot better for indexing. It supports addition and removal at the end, look up and update by index (but not insertion or removal), and a simple `Take(n)` subsequence operation. It also supports *extremely* fast bulk addition at the end (see benchmarks), as well as fast insertion at any index if the input is very large. It also supports general slices, but at much slower (though still very high) performance. This data structure is implemented as an array-mapped trie. 

#### Equality Semantics
Funq's sequential collections implement structural equality, overriding `Equals` and `GetHashCode`, as well as the `==` operator. For two collections to be equal, they must be of the exact same type, and must also contain the same sequence of elements. The equality comparer used to equate elements is the default equality comparer, and this cannot be changed. However, Funq provides separate comparison handlers for all kinds of sequences, and these allow you to specify a custom comparison handler for elements.

#### Complexity
Here is an overview of the time complexity of the operations offered by the different sequential collections. Note that time complexity is not always a good indicator of performance in the real world.

	| Collection/Operation | AddFirst     | AddFirstRange | Concat  | AddLast      | AddLastRange | RemoveFirst  | RemoveLast   | Insert  | InsertRange | InsertConcat | Lookup  | Remove  | Skip    | Take    | Update  |
	|----------------------|--------------|---------------|---------|--------------|--------------|--------------|--------------|---------|-------------|--------------|---------|---------|---------|---------|---------|
	| FSharpx.Deque        | O(1)         | O(m)          | X       | O(1)         | O(m)         | O(1)/O(n)    | O(1)/O(n)    | X       | X           | X            | X       | X       | X       | X       | X       |
	| FSharpx.Vector       | X            | X             | X       | O(logn)      | O(m log s)   | X            | O(logn)      | X       | X           | X            | O(logn) | X       | X       | X       | O(logn) |
	| FunqList             | O(1)/O(logn) | O(m)/?        | O(logn) | O(1)/O(logn) | O(m)/?       | O(1)/O(logn) | O(1)/O(logn) | O(logn) | O(m + logn) | O(logn)      | O(logn) | O(logn) | O(logn) | O(logn) | O(logn) |
	| FunqVector           | X            | O(m + n)      | X       | O(logn)      | O(m + logn)  | X            | O(logn)      | X       | O(m + n)    | X            | O(logn) | X       | O(n)    | O(logn) | O(logn) |
	| System.ImmutableList | O(logn)      | O(m log s)    | X       | O(logn)      | O(m log s)   | O(logn)      | O(logn)      | O(logn) | O(m log s)  | X            | O(logn) | O(logn) | O(n)    | O(n)    | O(logn) |

	X	Operation is unavailable
	/	Means that the complexity to the left is amortized. Worst case is to the right.
	?	I have no idea what the complexity is here
	n	length of the target collection
	m	length of the input collection (where applicable)
	s   m + n
	
#### Benchmarks
These are the benchmark results for the sequential collections, compared with similar collections in different libraries. These benchmarks were performed with particular settings, and different settings yield different results. 

	| Collection/Test      | AddFirst | AddFirstRange | AddFirstRange (concat) | AddLast | AddLastRange | AddLastRange (concat) | IEnumerator | Insert | Insert Range | Insert Range (concat) | Iterate | Lookup | Remove | RemoveFirst | RemoveLast | Skip  | Take  | Update |
	|----------------------|----------|---------------|------------------------|---------|--------------|-----------------------|-------------|--------|--------------|-----------------------|---------|--------|--------|-------------|------------|-------|-------|--------|
	| FSharpx.Deque        | 0.524    | 2.335         | 3.37                   | 0.47    | 2.26         | 2.734                 | 0.203       | X      | X            | X                     | 0.39    | X      | X      | 0.549       | 1.486      | X     | X     | X      |
	| FSharpx.Vector       | X        | X             | 11.472                 | 2.247   | 9.072        | 16.574                | 0.167       | X      | X            | X                     | 0.246   | 0.454  | X      | X           | 4.96       | X     | X     | 6.272  |
	| FunqList             | 2.004    | 3.672         | 0.009                  | 1.936   | 3.847        | 0.009                 | 0.471       | 20.26  | 3.66         | 0.045                 | 0.102   | 1.737  | 20.523 | 0.861       | 1.05       | 0.008 | 0.008 | 11.923 |
	| FunqVector           | X        | 0.205         | 0.318                  | 3.187   | 0.281        | 0.419                 | 0.142       | X      | 0.392        | 0.737                 | 0.029   | 0.146  | X      | X           | 2.935      | 0.116 | 0.001 | 2.959  |
	| System.ImmutableList | 12.835   | 21.376        | 34.088                 | 8.298   | 17.303       | 24.198                | 2.128       | 23.897 | 20.022       | 27.188                | 2.092   | 2.155  | 8.789  | 4.59        | 5.19       | 1.63  | 2.293 | 5.959  |

### Sets
Sets are collections that store unique elements. They provide methods for adding elements, removing them, and checking if they already exist in the set. They also provide set-theoretic relations (e.g. set equality, superset, subset), and set-theoretic operations, such as intersection and union. Funq provides two set-like collections.

  1. **FunqSet**, which is an equality-based set that uses hashing. Implemented as an AVL tree.
  2. **FunqOrderedSet**, which is a comparison-based set that stores elements by order. Supports extra operations, such as getting/removing the maximal element, and retrieving elements by sort order index. Implemented as an AVL tree.

It is important to note that Funq set-theoretic operations accept any `IEnumerable<T>` parameter, but behind the scenes they check whether the argument is also a set of the same kind, and with the same comparison handler. If so, they use a specialized operation with strong performance guarantees. Otherwise, they use a generic algorithm that works for any collection.

In contrast, `System.Collections.Immutable` sets have no such specialized algorithm, and perform equally (badly) whether the input collection is a set of the same kind or an arbitrary collection. F#'s set does have specialized algorithms, but still underperforms when compared to Funq.

#### Time Complexity
These sets and others compared against them implement the standard operations `Contains`, `Add`, and `Drop` in `O(log n)`. The time complexity of other operations is more complicated. 

The set relation operations have worst case time complexity `O(min(n, m))`, but in practice are very fast for arbitrary sets (if implemented properly) because arbitrary sets have many different elements, or different numbers of elements. `IsDisjoint` is likely to run more slowly than other operations for dissimilar sets. Currently, there are no benchmarks involving similar sets.

Set-theoretic operations are even more complex to analyze. All Funq sets implement set-theoretic operations in `O(min(n log m, m + n))`, which basically means they always run proportionally to the smaller set. Not all tested implementations follow this rule, which means that an operation between a set with 10,000 elements and one with just 10 could potentially take much longer than you'd expect.

I haven't determined the time complexity of the other set implementations, so you'll have to make do with benchmarks.

#### Benchmarks
Like I implied in the previous section, in order to properly appreciate the performance of sets you have to test them at different numbers of elements, and with different *types* of elements. Comparison-based sets don't do very well with long string-based keys, but the opposite is true for integer keys. 

In this benchmark, the input collection and the target collection both had 10,000 elements (for AddRange, etc), and the key was string based. 

	| Collection/Test           | Add    | AddRange | Contains | Difference | Except | IEnumerator | Intersection | IsProperSubset | IsProperSuperset | Iterate | Remove | RemoveRange | SetEquals | Union  |
	|---------------------------|--------|----------|----------|------------|--------|-------------|--------------|----------------|------------------|---------|--------|-------------|-----------|--------|
	| FSharp.Set                | 51.496 | 54.061   | 5.192    | 148.187    | 55.046 | 0.745       | 15.467       | 0.002          | 0.002            | 1.065   | 14.208 | 35.956      | 0.007     | 28.515 |
	| FunqSet                   | 9.679  | 7.676    | 3.763    | 30.71      | 11.241 | 1.478       | 14.005       | 0.006          | 0.006            | 0.339   | 8.032  | 14.517      | 0.006     | 13.612 |
	| FunqOrderedSet            | 14.487 | 14.156   | 6.863    | 47.256     | 12.767 | 1.618       | 19.424       | 0.014          | 0.008            | 0.309   | 16.296 | 17.667      | 0.005     | 14.333 |
	| System.ImmutableSet       | 10.367 | 8.745    | 3.138    | 112.661    | 23.747 | 3.951       | 26.111       | 17.293         | 0.008            | 4.824   | 16.571 | 20.679      | 19.04     | 54.249 |
	| System.ImmutableSortedSet | 57.695 | 57.937   | 20.884   | 344.763    | 99.322 | 2.465       | 77.411       | 58.545         | 0.014            | 3.441   | 26.427 | 44.63       | 67.019    | 97.147 |

Here is another set of benchmarks in which the target collection has 100 elements but the input collection has 10,000 elements (relevant for operations with an input collection). It demonstrates how performance scales with the size of the smaller collection.

	| Collection/Test           | Difference | Except | Intersection | IsProperSubset | IsProperSuperset | RemoveRange | SetEquals | Union  |
	|---------------------------|------------|--------|--------------|----------------|------------------|-------------|-----------|--------|
	| FSharp.Set                | 23.868     | 23.559 | 4.15         | 0.002          | 0.002            | 0.147       | 0.007     | 0.524  |
	| FunqSet                   | 0.791      | 0.438  | 0.401        | 0.006          | 0.005            | 0.047       | 0.002     | 0.351  |
	| FunqOrderedSet            | 1.009      | 0.246  | 0.383        | 0.024          | 0.002            | 0.041       | 0.002     | 0.408  |
	| System.ImmutableSet       | 64.99      | 17.645 | 17.43        | 22.886         | 0.01             | 0.081       | 16.913    | 41.338 |
	| System.ImmutableSortedSet | 131.557    | 40.526 | 41.335       | 88.106         | 0.011            | 0.184       | 58.584    | 0.931  |


### Maps and dictionaries
A map or dictionary is a collection that allows efficiently retrieving values using a key. Funq provides two dictionary collections:

1. FunqMap, which is equality-based and uses hashing. Implemented as an AVL tree.
2. FunqOrderedMap, which is ordered by key. Provides additional operations, such as max key-value pair, and retrieval by sort order index. Implemented as an AVL tree.

These collections support basic operations in `O(logn)`, including `ContainsKey`, `Set`, `Add`, `Remove`, etc. Their performance for these operations is similar to set-like collections, as the former use a map behind the scenes. 

In addition to these standard operations, Funq maps extend set-theoretic operations to maps. This comes in two forms. Firstly, `AddRange` and `RemoveRange` use the specialized `Union` and `Except` algorithms if the input collection is of the same type and with the same comparison handler as the target. There is also a `RemoveRange` overload that checks if the other collection is a *set* of the same kind (e.g. `FunqMap` checks if the other collection is `FunqSet` for example).

Other than this, there are specialized methods:
1. `Merge`, which merges two maps of the same type by key. The user provides a collision resolution function to determine the value in case the maps share identical keys. It is equivalent to `Union` on sets.
2. `Join`, which joins the two maps based on their keys. The user provides a collision resolution function to determine the resulting value (similar to a Join LINQ statement).
3. `Except`, which is identical to the set-theoretic operation. The user can optionally provide a subtraction function that determines the resulting value if the maps share a key (the function returns an `Option` value). Can be used with maps of a different value type, and also with sets that have the same key and equality/comparison semantics.
4. `Difference`, which returns the symmetric difference. No function can be given here, as the operation is too complicated.

These operations perform similarly to the set-theoretic operations on which they are based.

### Benchmarks
I should note that `System.Collections.Immutable` dictionaries have a mechanism that checks whether values are equal (using the default equality comparer), and if they are, it doesn't update them. I forced this mechanism off for the purpose of this benchmark because of the way I generate data (as identical key-value pairs).

	| Collection/Test            | Add     | AddRange | DropKey | DropRange | IEnumerator | Iterate | Lookup |
	|----------------------------|---------|----------|---------|-----------|-------------|---------|--------|
	| FSharp.Map                 | 183.877 | 193.356  | 25.077  | 37.747    | 1.541       | 1.282   | 9.441  |
	| FunqMap                    | 172.237 | 89.343   | 22.318  | 25.009    | 2.815       | 1.3     | 4.105  |
	| FunqOrderedMap             | 192.106 | 76.973   | 26.027  | 16.219    | 1.2         | 0.52    | 3.89   |
	| System.ImmutableDict       | 197.622 | 99.538   | 28.36   | 29.539    | 6.466       | 7.373   | 5.063  |
	| System.ImmutableSortedDict | 250.299 | 178.54   | 37.232  | 48.54     | 3.732       | 3.728   | 26.033 |

## Notes
### Custom comparison handlers
The set and map collections in this library support custom equality and comparison semantics (by accepting an `IComparer<T>` or `IEqualityComparer<T>`). This isn't as trivial as it sounds. Remember that these collections support specialized implementations of operations such as `Intersect` and `Union`, but these specialized implementations only make sense when both collections use the same equality/comparison semantics. Otherwise, the result will be corrupted.

In order to avoid these dangerous and hard to track bugs, Funq collections will only use specialized implementations if both collections have the same comparison handler (determined by `.Equals`). For this reason, it is recommended that if you use custom equality semantics, you should either:

1. Make sure to use the same comparer instance for all Funq collections using that comparer, and avoid creating separate but functionally identical instances of it. This pattern is enforced by extension methods allowing comparison handlers to serve as 'factories' for Funq collections. E.g. `IComparer<T>.CreateOrderedSet` creates a `FunqOrderedSet` that uses that comparison handler.
2. Override `.Equals` on your custom comparison handler for functional equality. 

If Funq decides that the comparison handlers are different, a generic implementation will be used, which doesn't carry the same performance guarantees discussed in the previous section. 

### Benchmarking 
Benchmark data is available in the form of charts (automatically generated using the Microsoft charting controls and the `FSharp.Chart` library), raw logs in the form of CSV files which contain the exact parameters and tests executed, and a CSV table for comparison (the tables in this file were generated in this way, in fact).

The benchmarking system itself is available in the namespace `Funq.Tests.Performance`. It really is a system, and the way it works is quite complicated. It's written in F#, and heavily uses (or perhaps _abuses_ is a better word) the *inline* functions feature, which basically allows performance test code to be generated implicitly, so that the human-written code is generic, but still executes with zero overhead. I'll write an article about it at some point, as it involves concepts that can be reused.

## Extra Features
### LINQ Implementation
A very common use-case when working with concrete collections, is to apply various operations on them, such as filtering, or projection. In such cases, you usually want to get a collection of the same type in return, rather than an `IEnumerable` that you then need to convert.

Standard `.NET` collections, such as `List<T>` do implement a limited set of collection-based operations. Examples are `ConvertAll`, which is similar to the `select` operator, and `FindAll` which is similar to `where`. However, these operations are seldom used for several reasons. Instead, we tend to use the various `LINQ` operators, whether as keywords or as extension methods.

**Funq** collections effectively override the LINQ operators, so that they directly return collections of the same kind as the source. They also provide a number of LINQ-like operations geared for map collections. Operations such as `Any` are implemented more efficiently as well.

### Option type
**Funq** provides its own implementation of an _option type_: a generic type `Option<T>` that indicates a possibly missing value. Option types have two effective states:

1. The `Some` state, in which the object wraps a value of type `T`
2. The `None` state, in which case the object indicates a missing value.

An option type is kind of similar to a regular reference type (with its dedicated `null` value), but it's more similar to a nullable type such as `int?`. Its primary purpose is to unambiguously indicate a possibly missing value. `null` is very bad at doing this.

1. `Option<T>` is a value type. It is initialized to `None` by default. 
2. This means that an `Option<T>` can always be the target of a method call. In particular, you can call `Equals` or `ToString` on it. A `None` value is also visualized in the debugger.
1. You can define an `Option<T>` for any `T`, including both reference types and value types. You can even have `Option<Option<int>>`.

Although F# defines its own option type, due to technical considerations I decided to implement a separate type. I really feel this is a shame, and could've been avoided if the developers of F# wrote an option type that works well in every .NET language, rather than just for themselves.

The option type is used very frequently in Funq. Any method that has a signature similar to, `bool TryX(object arg, out TResult result)` is replaced with the more elegant and convenient `Option<TResult> TryX(object arg)`. Also supported is the method `Choose` which is similar to `Select`, except that it returns an `Option<T>` and if it is equal to `None`, the element is skipped. There are other examples.

### F# Integration
**Funq** itself is written primarily in C#, and targets that language. However, the library also has a separate companion assembly that that provides various extensions and modules for use with F#.

Here are some example features:

1. Extension overloads for methods that normally take `Func<T>`. The overloads take F#'s function value.
2. Special F# operators for adding elements to collections, and concatenating them.
2. Module bindings for most of the instance-level operations.
3. Generic active patterns for decomposing collections in various ways.
4. Computational expressions (aka monads) for constructing Funq collections. You can also construct maps and sets in this way.

### Comparison Handlers
I originally wanted Funq collections to support functional equality or comparison by default. However, this is brings about many problems in practice, especially when dealing with maps and sets which have configurable comparison handlers, so I've extended this support to sequential collections alone.

Nevertheless, all collections have methods that determine equality; they simply don't use them as default equality semantics.

In addition, quite separately from anything else, Funq provides an assortment of methods and comparison handlers for comparing `IEnumerable` objects in different ways, including:

1. Sequence, set, and map equality and hash code. 
2. Sequence comparison using lexicographic comparison or number-like comparison (that is, length first, and then lexicographically).

One of the intended purposes of these objects and methods is to enable using collections as keys in dictionaries,

## Funq.Abstract [Experimental]
Although at this point experimental, Funq uses a library of collection abstractions called `Funq.Abstract` that all collections inherit from. This library implements common collection operations once (such as `Where`, `Select`, and so forth) and inheriting collections reuse those implementations, or alternatively override them with specialized versions. It's also possible to abstract over similar collections using this library, something done in `Funq.FSharp` to reduce repetition, as well as during testing.

This part of the library is inspired by Scala's collection library, which uses advanced type system features such as type variables, higher-kinded types, implicit parameters, and traits. These aren't available in C#, so the library's structure is somewhat convoluted, requires a thorough explanation, and some operations need to be partially implemented by hand. In `Funq.Collections`, method stubs for operations such as `Select` are automatically generated from T4 text templates. 

The library is stand-alone and requires no dependencies.

**Funq.Abstract** offers the following collection abstractions:
 
* `AbstractIterable`, the parent class for all collections that support some form of iteration. Implements LINQ-operations such as `Any, All, Find, ForEach, Aggregate` and provides partial implementations for operations such as `Cast, Select, GroupBy`.

* `AbstractSequential`, the parent class for all sequential collections (where one element follows another in order).  Naively implements operations such as `this[int]`, `FindIndex`, `OrderBy`, and `Take`. 

* `AbstractMap`, the parent class for all map- or dictionary-like collections, offering implementations for set-like operations over maps, such as `Join` (joins by key), `Merge` (merge by key), and so forth. Also offers specialized versions of some of the operations implemented on `AbstractIterable`.

* `AbstractSet`, the parent class of all set-like collections, offering naive implementations for standard set operations.

The library is meant to allow extension of immutable collections with common methods, and can be used separately in order to extend your own collections. However, a more complete guide will be required in order to learn how to use it properly. 

## Possibilities
Funq is designed with users directly in mind. However, its combination of power and performance can provide the basis for other libraries. Here are some of the things that could be implemented using the features provided by Funq:

1. Mutable, observable, thread-safe collections supporting such things as implicit copying, snapshots, and history tracking (undo/redo). 
2. An immutable workflow object, composed of individual computation steps, which is also catenable.
3. Various specialized collections, such as immutable and persistent multimaps, multisets, and priority queues.
4. An immutable lazy list, with caching functionality and concatenation.
