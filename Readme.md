# Funq
---
Funq is, at least primarily, a library of [persistent](http://en.wikipedia.org/wiki/Persistent_data_structure), and [immutable](http://en.wikipedia.org/wiki/Immutable_object) collections for the .NET platform.

Funq isn't the only such library out there. In fact, Microsoft have themselves written such classes, and although they haven't incorporated them into the base class library, they are available of NuGet.

However, Funq has several distinguishing and important features that make it different from pretty much any other library of this sort that has been released for .NET so far.

  1. *Performance:* Funq collections perform very well, often much better than collections of the same kind. 
  1. *Functionality:* All Funq collections provide more inherent functionality than any collection of a similar kind.
  1. *LINQ interface:* Funq collections support true collection-based operations through LINQ syntax. For example, they implement a `Select` statement that returns a collection of the same kind.
  1. *Operator overloading*: Funq collections meaningfully overload operators such as `+`, in order to allow for better integration, and to encourage using the value abstraction for collections (more on this later).

## The Collections

### FunqList
**FunqList** is a very versatile sequential collection that efficiently implements pretty much every operation you can name. Here is a selection:

  1. You can insert/remove/peek at items from either end. This is `O(1)`.
  2. You can look up, update, insert, and remove items by index. This is `O(log n)`.
  3. You can slice, split, and concat multiple collection. This is also `O(log n)`. 

Despite this flexibilty, it tends to perform as well as similar collections with fewer features.

### FunqVector

This collection serves as a high-performance, indexed collection.

FunqVector provides only a small subset of the operations provided by **FunqList**, but its performance is pretty incredible to compensate.

Note that although most of the operations are `O(log n)`, asymptotic complexity isn't very meaningful in this case. You should look at the actual benchmarks to see the performance advantages. 

  1. Add and remove items from the end, like a `List<T>`. This is `O(log n)`.
  2. Lookup and update items based on index. This is also `O(log n)`.
  3. Highly optimized bulk addition of many elements at one end. This is `O(m)`.
  4. A `Take(m)` operation that returns a starting subsequence. This is `O(log n)`,

### Set-like collections
**Funq** provides two set-like collections.

  1. **FunqSet**, which is an equality-based set that uses hashng.
  2. **FunqOrderedSet**, which is a comparison-based set that stores elements by order. 

In addition to standard operations, such as `Contains` and `Add`, these collections also support *set operations*, such as **Union**, and **Intersection**. This is actually a major feature of set-like collections. These operations used specialized, highly tuned algorithms that have optimal performance, in a very specific sense.

#### Set operations

All set operations follow some design guidelines

All operations run at worst case at something similar to `O(min(n log m, m + n))`, where `m` is the size of the larger set. This means that if `n ≈ m`, then they take `O(n)`. If `n ≪ m`, then they take `O(log m)`. The algorithm is always the same though, so there is no hard cut off point.

There are also additional optimizations.

Here is a partial list of operations:

1. Union and Intersection
2. Difference and Except
3. `RelatesTo`, which returns the set-theoretic relation between two sets as an enumeration value, such as `ProperSubset`, `Equals`, or `Disjoint`.

In addition, the **FunqOrderedSet** supports some extra operations due to its additional structure.

1. Get the minimum or maximum element.
2. Remove the minimum or maximum element.
3. Get the `n`th element, by sort order.
3. Iterate over the collection from largest to smallest, or vice versa. 

### Maps and dictionaries
**Funq** provides two map or dictionary-like collections.

1. **FunqMap**, which is equality-based and uses hashing.
2. **FunqOrderedMap**, which is ordered by key.

In addition to common operations such as `ContainsKey` and `Add(key,value)`, **Funq** maps support special map operations that are similar to the set operations discussed previously, and have similar performance characteristics.

1. `Merge`, which combines two maps like the `Union` operation. You can provide a collision function that handles values in case of a collision.
2. `Join`, which is similar to `Intersect`.
3. `Except` and `Difference`, which work based on key.

## Benchmarks
As I've already mentioned, one of the key features of the **Funq** library is very high performance. This claim wouldn't mean much unless there was quite a lot of benchmarking to back it up. Happily, there is!

Benchmark data is available in the form of charts (automatically generated using the Microsoft charting controls and the `FSharp.Chart` library), or as raw data in the form of CSV files. The raw form provides a lot more information, but is less viewer-friendly.

The benchmarking system itself is available in the namespace `Funq.Tests.Performance`. It really is a system, and the way it works is quite complicated. It's written in F#, and heavily uses (or perhaps _abuses_ is a better word) the *inline* functions feature, which basically allows performance test code to be generated implicitly, so that the human-written code is generic, but still executes with zero overhead.

### Benchmark targets and functionality overview
There aren't that many relevant libraries and collections to compare Funq against. The primary competitors are the data structures provided by the free functional programming library, **FSharpx**, those provided by the Microsoft **System.Collections.Immutable**, and those provided by the language **F#**. More libraries may be added in the future, and the collections may be improved if they're not up to par.

#### Sequential collections
Only meaningful comparisons were made to collections from those libraries. In particular, collections with highly limited functionality, such as `ImmutableStack`, and `FSharpList` were not compared against collections with general functionality. You may safely assume, however, that they perform much better in their roles.

##### Complexity
Here is an overview of all the benchmark targets, their supported operations, and the asymptotic time complexity of each. If there is a slash `/`, it means that the time complexity is amortized. The complexity to the right of the slash is the worst case.

Note that time complexity isn't always a good indicator of the performance of an algorithm in the real world.
	
	| Collection/Operation | Library | AddFirst      | AddLast       | DropFirst | DropLast  | Length | Slices      | Concat        | Insert   | InsertConcat | Remove   | Lookup   | Update   |
	|----------------------|---------|---------------|---------------|-----------|-----------|--------|-------------|---------------|----------|--------------|----------|----------|----------|
	| FSharpx.Deque        | FSharpx | O(1)          | O(1)          | O(1)/O(n) | O(1)/O(n) | O(n)   | X           | X             | X        | X            | X        | X        | X        |
	| FSharpx.Vector       | FSharpx | X             | O(log n)      | X         | O(log n)  | O(1)   | X           | X             | X        | X            | X        | O(log n) | O(log n) |
	| Funq.FunqList        | Funq    | O(1)/O(log n) | O(1)/O(log n) | O(1)      | O(1)      | O(1)   | O(log n)    | O(log n)      | O(log n) | O(log n)     | O(log n) | O(log n) | O(log n) |
	| Funq.FunqVector      | Funq    | X             | O(log n)      | X         | O(log n)  | O(1)   | O(log n)*   | O(min(n,m)    | X        | X            | X        | O(log n) | O(log n) |
	| System.ImmutableList | BCL     | O(log n)      | O(log n)      | O(log n)  | O(log n)  | O(1)   | O(n log n)? | O(m log n)?   | O(log n) | X            | O(log n) | O(log n) | O(log n) |
	
	*   Limited to Take(n); e.g. starting subsequence
	?   Assumed due to emprical data. Actual implementation unknown.

##### Benchmark Results
The following are the benchmark results for the data structures tested.

	| Collection/Operation | AddFirst | AddFirstRange | AddFirstRange (concat) | AddLast | AddLastRange | AddLastRange (concat) | DropFirst | DropLast | Insert  | Insert Range | Insert Concat | Iterate using Dedicated Method | Iterate using IEnumerator | Lookup | Skip    | Take   | Update | Remove  |
	|----------------------|----------|---------------|------------------------|---------|--------------|-----------------------|-----------|----------|---------|--------------|---------------|--------------------------------|---------------------------|--------|---------|--------|--------|---------|
	| FSharpx.Deque        | 8.125    | 6.684         | 8.465                  | 9.813   | 8            | 9.502                 | 3.186     | 13.32    |         |              |               | 2.101                          | 2.017                     |        |         |        |        |         |
	| FSharpx.Vector       |          |               | 39.323                 | 24.945  | 33.376       | 36.295                |           | 40.341   |         |              |               | 1.907                          | 1.477                     | 14.079 |         |        | 79.566 |         |
	| FunqVector           |          | 0.383         | 0.939                  | 30.929  | 0.257        | 0.864                 |           | 26.376   |         |              |               | 0.224                          | 1.465                     | 2.777  |         | 0.008  | 29.039 |         |
	| FunqList             | 42.87    | 11.003        | 0.034                  | 42.532  | 8.74         | 0.031                 | 8.041     | 7.888    | 199.089 | 13.345       | 0.104         | 0.778                          | 18.453                    | 18.043 | 0.037   | 0.03   | 71.861 | 122.291 |
	| System.ImmutableList | 68.163   | 38.968        | 60.509                 | 75.594  | 37.644       | 66.857                | 37.147    | 36.065   | 189.346 |              |               | 36.513                         | 12.83                     | 14.26  | 105.903 | 44.912 | 58.311 | 100.059 |


Funq also provides a few other features, besides the core collection library.

###### Benchmark Procedure
Benchmarks are divided into several types:
1. **Simple**: These involve single operations on single elements, such as `AddLast`, which adds one element to the end. For these, 50,000 iterations were performed. The result of each operation is conserved, so the collection is filled or emptied.
2. **Data Structure:** These involve inserting data structures. For these, arrays consisting of 5,000 elements were used, and the operation was iterated 10 times. The result of the operation is conserved. If the operation is a concat operation, a data structure of the same type as the original is used.
3. **Subsequence:** These involve taking a starting/ending subsequence. The length of the subsequence is determined randomly. The result is *not* conserved.

If an index is required, it is determined randomly depending on the current length of the collection. This is done by generating random floating point data beforehand, so the same random data is used for all tests.

#### Map-like Collections
To be continued

#### Set-like Collections
To be continued

## Extra Features
### LINQ Implementation
A very common use-case when working with concrete collections, is to apply various operations on them, such as filtering, or projection. In such cases, you usually want to get a collection of the same type in return, rather than an `IEnumerable` that you then need to convert.

Standard `.NET` collections, such as `List<T>` do implement a limited set of collection-based operations. Examples are `ConvertAll`, which is similar to the `select` operator, and `FindAll` which is similar to `where`. However, these operations are seldom used for several reasons. Instead, we tend to use the various `LINQ` operators, whether as keywords or as extension methods.

**Funq** collections effectively override the LINQ operators, so that they directly return collections of the same kind as the source. They also provide a number of LINQ-like operations geared for map collections.

### Option type
**Funq** provides its own implementation of an _option type_: a generic type `Option<T>` that indicates a possibly missing value. Option types have two effective states:

1. The `Some` state, in which the object wraps a value of type `T`
2. The `None` state, in which case the object indicates a missing value.

An option type is kind of similar to a regular reference type (with its dedicated `null` value), but it's more similar to a nullable type such as `int?`. Its primary purpose is to unambiguously indicate a possibly missing value, whereas `null` for reference types doesn't really do the job.  

1. `Option<T>` is a value type. It is initialized to `None` by default. 
2. This means that an `Option<T>` can always be the target of a method call. In particular, you can call `Equals` or `ToString` on it. A `None` value is also visualized in the debugger.
1. You can define an `Option<T>` for any `T`, including both reference types and value types. You can even have `Option<Option<int>>`.

### F# Integration
**Funq** itself is written primarily in C#, and targets that language. However, the library also has a separate companion assembly that that provides various extensions and modules for use with F#.

Here are some example features:

1. Extension overloads for methods that normally take `Func<T>`. The overloads take F#'s function value.
2. Special F# operators for adding elements to collections, and concatenating them.
2. Module bindings for most of the instance-level operations.
3. Generic active patterns for decomposing collections in various ways.
4. Computational expressions (aka monads) for constructing **Funq** collections. You can also construct maps and sets in this way.

## Funq.Abstract [Experimental]
Although at this point experimental, **Funq** uses a library of collection abstractions called **Funq.Abstract** that all collections inherit from. This library implements common collection operations once (such as `Where`, `Select`, and so forth) and inheriting collections reuse those implementations, or alternatively override them with specialized versions. It's also possible to abstract over similar collections using this library, something done in **Funq.FSharp** to reduce repetition.

The library is inspired by Scala's collection library, which uses advanced type system features, such as type variables, higher kinds, implicit parameters, and traits. These aren't present in C#, so the library's structure is very convoluted, requires a thorough explanation, and some operations need to be partially implemented by hand. In **Funq.Collections**, some operations are implemented using the T4 text templating system.

The library is stand-alone and requires no dependencies, apart from **Funq.Shared** (which will be eventually integrated into other libraries).

**Funq.Abstract** offers the following collection abstractions:
 
* `Trait_Iterable`, the parent class for all collections that support some form of iteration. Implements LINQ-operations such as `Any, All, Find, ForEach, Aggregate` and provides partial implementations for operations such as `Cast, Select, GroupBy`.

* `Trait_Sequential`, the parent class for all sequential collections (where one element follows another in order). Implements operations such as `Indexer`, `FindIndex`, `OrderBy`, and `Take`. 

* `Trait_MapLike`, the parent class for all map- or dictionary-like collections, offering implementations for set-like operations over maps, such as `Join` (joins by key), `Merge` (merge by key), and so forth. Also offers specialized versions of some of the operations implemented on `Trait_Iterable`.

* `Trait_SetLike`, the parent class of all set-like collections, offering naive implementations for standard set operations.


## Possibilities
**Funq** is designed to be used by the user. However, another possibility is using **Funq**'s unique combination of power and performance in order to implement other libraries.

Here are some of the things that could be implemented using the features provided by **Funq**.

1. Mutable, observable, thread-safe collections supporting such things as implicit copying, snapshots, and history tracking (undo/redo). 
2. An immutable workflow object, composed of individual computation steps, which is also catenable.
3. Various specialized collections, such as immutable and persistent multimaps, multisets, and priority queues.
4. An immutable lazy list, with caching functionality and catentation.
