# Imms
---

Imms is a library of [persistent](http://en.wikipedia.org/wiki/Persistent_data_structure), and [immutable](http://en.wikipedia.org/wiki/Immutable_object) collections for the .NET framework. It is available on [NuGet](https://www.nuget.org/packages/ImmCollections/).

However, Imms has several distinguishing features that make it different from pretty much any other library of this sort that has been released for .NET so far.

  1. *Performance:* Imms collections perform very well, usually far better than other collections of the same kind. 
  2. *Functionality:* Imms collections provide more inherent functionality than any collection of a similar kind.
  3. *LINQ interface:* Imms collections support true collection-based operations through LINQ syntax. For example, they implement a `Select` statement that returns a collection of the same kind.
  5. *F# Integration*: Imms collections are integrated with F# in the assembly `Imms.FSharp`, which provides extension methods, module bindings, collection builders (computation expressions), and active patterns for working with the collections. 
  6. *Permissive License:* Available under the [MIT license](https://github.com/GregRos/Imms/blob/master/license.md).
  7. *Zero dependencies*: Imms assemblies have zero dependencies.

**Note:** Imms collections can't be called *purely functional* because they involve mutations behind the scenes. However, no user-visible instance is ever mutated.

Currently, all library assemblies require .NET Framework 4.0 Client Profile, and all test assemblies require .NET Framework 4.5.1. The F# libraries require F# 3.0.

## Current Status
The collections are in beta. 

## The Collections
To see up to date benchmarks you can go to the [benchmarks folder](https://github.com/GregRos/Imms/tree/master/Imms/Imms.Tests.Performance/Benchmarks). Each set of benchmarks includes charts, a CSV table, and a CSV log file with explicit information about the parameters of the benchmark.

### Sequential Collections

Imms provides two sequential collections:

 1. **ImmList:** A very versatile sequential collection that  supports pretty much every operation you can name (see the table below). Implemented as a 2-3-4 finger tree. 

 1. **ImmVector:** Offers less functionality than ImmList, but performs a lot better for most operations. It is generally recommended that you use ImmList for most purposes. 

#### Equality Semantics
Imms's sequential collections implement structural equality, overriding `Equals` and `GetHashCode`, as well as the `==` operator. 

For two collections to be equal, they must be of the exact same type, and must also contain the same sequence of elements. 

The equality comparer used to equate elements is the default equality comparer, and this cannot be changed. However, the `SequenceEquals` method lets you provide your own comparer.

#### Complexity
Here is an overview of the time complexity of the operations offered by the different sequential collections. Note that time complexity is not always a good indicator of performance in the real world.

	| Collection/Operation | AddFirst     | AddFirstRange | Concat  | AddLast      | AddLastRange | RemoveFirst  | RemoveLast   | Insert  | InsertRange | InsertConcat | Lookup  | Remove  | Skip    | Take    | Update  |
	|----------------------|--------------|---------------|---------|--------------|--------------|--------------|--------------|---------|-------------|--------------|---------|---------|---------|---------|---------|
	| FSharpx.Deque        | O(1)         | O(m)          | X       | O(1)         | O(m)         | O(1)/O(n)    | O(1)/O(n)    | X       | X           | X            | X       | X       | X       | X       | X       |
	| FSharpx.Vector       | X            | X             | X       | O(logn)      | O(m log s)   | X            | O(logn)      | X       | X           | X            | O(logn) | X       | X       | X       | O(logn) |
	| ImmList              | O(1)/O(logn) | O(m)/?        | O(logn) | O(1)/O(logn) | O(m)/?       | O(1)/O(logn) | O(1)/O(logn) | O(logn) | O(m + logn) | O(logn)      | O(logn) | O(logn) | O(logn) | O(logn) | O(logn) |
	| ImmVector            | X            | O(m + n)      | X       | O(logn)      | O(m + logn)  | X            | O(logn)      | X       | O(m + n)    | X            | O(logn) | X       | O(n)    | O(logn) | O(logn) |
	| System.ImmutableList | O(logn)      | O(m log s)    | X       | O(logn)      | O(m log s)   | O(logn)      | O(logn)      | O(logn) | O(m log s)  | X            | O(logn) | O(logn) | O(n)    | O(n)    | O(logn) |

	X	Operation is unavailable
	/	Means that the complexity to the left is amortized. Worst case is to the right.
	?	I have no idea what the complexity is here
	n	length of the target collection
	m	length of the input collection (where applicable)
	s   m + n
	
#### Benchmarks
These are the benchmark results for the sequential collections, compared with similar collections in different libraries. Different benchmark settings can yield somewhat different results. 

	| Collection/Test      | AddFirst | AddFirstRange | AddFirstRange (concat) | AddLast | AddLastRange | AddLastRange (concat) | IEnumerator | Insert | Insert Range | Insert Range (concat) | Iterate | Lookup | Remove | RemoveFirst | RemoveLast | Skip  | Take  | Update |
	|----------------------|----------|---------------|------------------------|---------|--------------|-----------------------|-------------|--------|--------------|-----------------------|---------|--------|--------|-------------|------------|-------|-------|--------|
	| FSharpx.Deque        | 0.524    | 2.335         | 3.37                   | 0.47    | 2.26         | 2.734                 | 0.203       | X      | X            | X                     | 0.39    | X      | X      | 0.549       | 1.486      | X     | X     | X      |
	| FSharpx.Vector       | X        | X             | 11.472                 | 2.247   | 9.072        | 16.574                | 0.167       | X      | X            | X                     | 0.246   | 0.454  | X      | X           | 4.96       | X     | X     | 6.272  |
	| ImmList              | 2.004    | 3.672         | 0.009                  | 1.936   | 3.847        | 0.009                 | 0.471       | 20.26  | 3.66         | 0.045                 | 0.102   | 1.737  | 20.523 | 0.861       | 1.05       | 0.008 | 0.008 | 11.923 |
	| ImmVector            | X        | 0.205         | 0.318                  | 3.187   | 0.281        | 0.419                 | 0.142       | X      | 0.392        | 0.737                 | 0.029   | 0.146  | X      | X           | 2.935      | 0.116 | 0.001 | 2.959  |
	| System.ImmutableList | 12.835   | 21.376        | 34.088                 | 8.298   | 17.303       | 24.198                | 2.128       | 23.897 | 20.022       | 27.188                | 2.092   | 2.155  | 8.789  | 4.59        | 5.19       | 1.63  | 2.293 | 5.959  |

### Sets
Imms provides two set-like collections.

  1. **ImmSet**, which is an equality-based set that uses hashing. Implemented as an AVL tree.
  2. **ImmOrderedSet**, which is a comparison-based set that stores elements in order. Supports extra operations, such as getting/removing the maximal element, and retrieving elements by sort order index. Implemented as an AVL tree.

Imms set-theoretic operations accept any `IEnumerable<T>`, but they perform much better if the input happens to be a collection of the same type that uses the same membership semantics.

Set-theoretic operations between sets of the same type are much faster for Imms sets than any other set.

#### Time Complexity
The following is the time complexity of Imms sets for different operations. Time complexity is much better when the two inputs are sets of the same type and with the same membership semantics.

I don't have similar data about sets from other libraries.

	| Set Operation           | Compatible                 | Naive             
	|-------------------------|----------------------------|-------------------
	| Add, Remove, Contains   | logn                       |                   
	| IsSuperset/IsSubset/etc | min(m,n)                   | m logm + min(m, n)
	| Intersect               | min(m logn, n logm, m + n) | m (logn + logm)   
	| Union                   | min(m logn, n logm, m + n) | m log(m + n)      
	| Except                  | min(m logn, n logm, m + n) | m log n           
	| Xor / Sym. Difference   | min(m logn, n logm, m + n) | (m + n) log(m + n) 

The naive option is used when one of the collections is not a set. Note that in pretty much all cases, operations between two compatible sets take time proportional to the smaller of the two.

#### Benchmarks
Like I implied in the previous section, in order to properly appreciate the performance of sets you have to test them at different numbers of elements, and with different *types* of elements. Comparison-based sets don't do very well with long string-based keys, but the opposite is true for integer keys. 

In this benchmark, the input collection and the target collection both had 10,000 elements (for AddRange, etc), and the key was string based. 

	| Collection/Test           | Add    | AddRange | Contains | Difference | Except | IEnumerator | Intersection | IsProperSubset | IsProperSuperset | Iterate | Remove | RemoveRange | SetEquals | Union  |
	|---------------------------|--------|----------|----------|------------|--------|-------------|--------------|----------------|------------------|---------|--------|-------------|-----------|--------|
	| FSharp.Set                | 51.496 | 54.061   | 5.192    | 148.187    | 55.046 | 0.745       | 15.467       | 0.002          | 0.002            | 1.065   | 14.208 | 35.956      | 0.007     | 28.515 |
	| ImmSet                    | 9.679  | 7.676    | 3.763    | 30.71      | 11.241 | 1.478       | 14.005       | 0.006          | 0.006            | 0.339   | 8.032  | 14.517      | 0.006     | 13.612 |
	| ImmOrderedSet             | 14.487 | 14.156   | 6.863    | 47.256     | 12.767 | 1.618       | 19.424       | 0.014          | 0.008            | 0.309   | 16.296 | 17.667      | 0.005     | 14.333 |
	| System.ImmutableSet       | 10.367 | 8.745    | 3.138    | 112.661    | 23.747 | 3.951       | 26.111       | 17.293         | 0.008            | 4.824   | 16.571 | 20.679      | 19.04     | 54.249 |
	| System.ImmutableSortedSet | 57.695 | 57.937   | 20.884   | 344.763    | 99.322 | 2.465       | 77.411       | 58.545         | 0.014            | 3.441   | 26.427 | 44.63       | 67.019    | 97.147 |

Here is another set of benchmarks in which the target collection has 100 elements but the input collection has 10,000 elements (relevant for operations with an input collection). It demonstrates how performance scales with the size of the smaller collection.

	| Collection/Test           | Difference | Except | Intersection | IsProperSubset | IsProperSuperset | RemoveRange | SetEquals | Union  |
	|---------------------------|------------|--------|--------------|----------------|------------------|-------------|-----------|--------|
	| FSharp.Set                | 23.868     | 23.559 | 4.15         | 0.002          | 0.002            | 0.147       | 0.007     | 0.524  |
	| ImmSet                    | 0.791      | 0.438  | 0.401        | 0.006          | 0.005            | 0.047       | 0.002     | 0.351  |
	| ImmOrderedSet             | 1.009      | 0.246  | 0.383        | 0.024          | 0.002            | 0.041       | 0.002     | 0.408  |
	| System.ImmutableSet       | 64.99      | 17.645 | 17.43        | 22.886         | 0.01             | 0.081       | 16.913    | 41.338 |
	| System.ImmutableSortedSet | 131.557    | 40.526 | 41.335       | 88.106         | 0.011            | 0.184       | 58.584    | 0.931  |


### Maps and dictionaries
A map or dictionary is a collection that allows efficiently retrieving values using a key. Imms provides two dictionary collections:

1. ImmMap, which is equality-based and uses hashing. Implemented as an AVL tree.
2. ImmOrderedMap, which is ordered by key. Provides additional operations, such as max key-value pair, and retrieval by sort order index. Implemented as an AVL tree.

The performance of these collections is identical to the performance of sets (except that in the case of hash-based maps, performance is average over all inputs).

Imms maps also provide operations between maps, such as Merge, Join, Except, and Difference which are analogous to the same operations over sets, except that in some cases you are allowed to supply a value selector to determine the value in the resulting map.

1. `Merge`, which merges two maps of the same type by key. The user provides a collision resolution function to determine the value in case the maps share identical keys. It is equivalent to `Union` on sets.
2. `Join`, which joins the two maps based on their keys. The user provides a collision resolution function to determine the resulting value (similar to a Join LINQ statement).
3. `Except`, which is identical to the set-theoretic operation. The user can provide a subtraction function that determines the resulting value if the maps share a key. Can be used with maps of a different value type.
4. `Difference`, which returns the symmetric difference. No function can be given here, as the operation is too complicated.

These operations perform similarly to the set-theoretic operations on which they are based.

#### Benchmarks

**Technical Note:** `System.Collections.Immutable` dictionaries have a mechanism that checks whether values are equal (using the default equality comparer), and if they are, it doesn't update them. I forced this mechanism off for the purpose of this benchmark because of the way I generate data (as identical key-value pairs).

	| Collection/Test            | Add     | AddRange | DropKey | DropRange | IEnumerator | Iterate | Lookup |
	|----------------------------|---------|----------|---------|-----------|-------------|---------|--------|
	| FSharp.Map                 | 183.877 | 193.356  | 25.077  | 37.747    | 1.541       | 1.282   | 9.441  |
	| ImmMap                     | 172.237 | 89.343   | 22.318  | 25.009    | 2.815       | 1.3     | 4.105  |
	| ImmOrderedMap              | 192.106 | 76.973   | 26.027  | 16.219    | 1.2         | 0.52    | 3.89   |
	| System.ImmutableDict       | 197.622 | 99.538   | 28.36   | 29.539    | 6.466       | 7.373   | 5.063  |
	| System.ImmutableSortedDict | 250.299 | 178.54   | 37.232  | 48.54     | 3.732       | 3.728   | 26.033 |

## Notes
### Custom comparison handlers
The set and map collections in this library support custom equality and comparison semantics (by accepting an `IComparer<T>` or `IEqualityComparer<T>`). This isn't as trivial as it sounds.

Remember that these collections support specialized implementations of operations such as `Intersect` and `Union`, but these specialized implementations only make sense when both collections use the same equality/comparison semantics. Otherwise, the result will be corrupted.

In order to avoid these dangerous and hard to track bugs, Imms collections will only use specialized implementations if both collections have the same comparison handler (determined by `.Equals`). For this reason, it is recommended that if you use custom equality semantics, you should either:

1. Make sure to use the same comparer instance for all Imms collections using that comparer, and avoid creating separate but functionally identical instances of it. This pattern is enforced by extension methods allowing comparison handlers to serve as 'factories' for Imms collections. E.g. `IComparer<T>.CreateOrderedSet` creates a `ImmOrderedSet` that uses that comparison handler.
2. Override `.Equals` on your custom comparison handler for functional equality. 

If Imms decides that the comparison handlers are different, a generic implementation will be used, which doesn't carry the same performance guarantees discussed in the previous section. 

### Benchmarking 
Benchmark data is available in the form of charts (automatically generated using the Microsoft charting controls and the `FSharp.Chart` library), raw logs in the form of CSV files which contain the exact parameters and tests executed, and a CSV table for comparison (the tables in this file were generated in this way, in fact).

The benchmarking system itself is available in the namespace `Imms.Tests.Performance`. It really is a system, and the way it works is quite complicated. 

It's written in F#, and heavily uses (or perhaps _abuses_ is a better word) the *inline* functions feature, which basically allows performance test code to be generated implicitly, so that the human-written code is generic, but still executes with zero overhead. I'll write an article about it at some point, as it involves concepts that can be reused.

## Extra Features
### LINQ Implementation
A very common use-case when working with concrete collections, is to apply various operations on them, such as filtering, or projection. In such cases, you usually want to get a collection of the same type in return, rather than an `IEnumerable` that you then need to convert.

Standard `.NET` collections, such as `List<T>` do implement a limited set of collection-based operations. Examples are `ConvertAll`, which is similar to the `select` operator, and `FindAll` which is similar to `where`. However, these operations are seldom used for several reasons. Instead, we tend to use the various `LINQ` operators, whether as keywords or as extension methods.

**Imms** collections effectively override the LINQ operators, so that they directly return collections of the same kind as the source. They also provide a number of LINQ-like operations geared for map collections. Operations such as `Any` are implemented more efficiently as well.

### Optional type
**Imms** provides its own implementation of an _optional value_: a generic type `Optional<T>` that indicates a possibly missing value. Option types have two effective states:

1. The `Some` state, in which the object wraps a value of type `T`
2. The `None` state, in which case the object indicates a missing value.

An option type is kind of similar to a regular reference type (with its dedicated `null` value), but it's more similar to a nullable type such as `int?`.

1. `Optional<T>` is a value type. It is initialized to `None` by default. 
2. This means that an `Optional<T>` can always be the target of a method call. In particular, you can call `Equals` or `ToString` on it. A `None` value is also visualized in the debugger.
1. You can define an `Optional<T>` for any `T`, including both reference types and value types. You can even have `Optional<Optional<int>>`.

Although F# defines its own option type, due to technical considerations I decided to implement a separate type. 

The option type is used very frequently in Imms. Any method that has a signature similar to, `bool TryX(object arg, out TResult result)` is replaced with the more elegant and convenient `Optional<TResult> TryX(object arg)`. 

Also supported is the method `Choose` which is similar to `Select`, except that it returns an `Option<T>` and if it is equal to `None`, the element is skipped. There are other examples.

### F# Integration
**Imms** itself is written primarily in C#, and targets that language. However, the library also has a separate companion assembly that that provides various extensions and modules for use with F#.

Here are some example features:

1. Extension overloads for methods that normally take `Func<T>`. The overloads take F#'s function value.
2. Special F# operators for adding elements to collections, and concatenating them.
2. Module bindings for most of the instance-level operations.
3. Generic active patterns for decomposing collections in various ways.
4. Computational expressions (aka monads) for constructing Imms collections. You can also construct maps and sets in this way.

### Comparison Handlers
I originally wanted Imms collections to support functional equality or comparison by default. However, this is brings about many problems in practice, especially when dealing with maps and sets which have configurable comparison handlers, so I've extended this support to sequential collections alone.

Nevertheless, all collections have methods that determine equality; they simply don't use them as default equality semantics.

## Imms.Abstract
Imms uses a library of collection abstractions called `Imms.Abstract` that all collections inherit from.

This library implements common collection operations once (such as `Where`, `Select`, and so forth) and inheriting collections reuse those implementations, or alternatively override them with specialized versions. It's also possible to abstract over similar collections using this library.

This part of the library is inspired by Scala's collection library, which uses advanced type system features such as type variables, higher-kinded types, implicit parameters, and traits. 

These aren't available in C#, so the library's structure is somewhat convoluted, requires a thorough explanation, and some operations need to be partially implemented by hand. In `Imms.Collections`, method stubs for operations such as `Select` are automatically generated from T4 text templates. 

The library is stand-alone and has no dependencies.

**Imms.Abstract** offers the following collection abstractions:
 
* `AbstractIterable`, the parent class for all collections that support some form of iteration. Implements LINQ-operations such as `Any, All, Find, ForEach, Aggregate` and provides partial implementations for operations such as `Cast, Select, GroupBy`.

* `AbstractSequential`, the parent class for all sequential collections (where one element follows another in order).  Naively implements operations such as `this[int]`, `FindIndex`, `OrderBy`, and `Take`. 

* `AbstractMap`, the parent class for all map- or dictionary-like collections, offering implementations for set-like operations over maps, such as `Join` (joins by key), `Merge` (merge by key), and so forth. Also offers specialized versions of some of the operations implemented on `AbstractIterable`.

* `AbstractSet`, the parent class of all set-like collections, offering naive implementations for standard set operations.

The library is meant to allow extension of immutable collections with common methods, and can be used separately in order to extend your own collections. However, a more complete guide will be required in order to learn how to use it properly. 

## Possibilities
Imms is designed with users directly in mind. However, its combination of power and performance can provide the basis for other libraries. Here are some of the things that could be implemented using the features provided by Imms:

1. Mutable, observable, thread-safe collections supporting such things as implicit copying, snapshots, and history tracking (undo/redo). 
2. An immutable workflow object, composed of individual computation steps, which is also catenable.
3. Various specialized collections, such as immutable and persistent multimaps, multisets, and priority queues.
4. An immutable lazy list, with caching functionality and concatenation.
