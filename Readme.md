# Funq
---
Funq is, at least primarily, a library of [persistent](http://en.wikipedia.org/wiki/Persistent_data_structure), and [immutable](http://en.wikipedia.org/wiki/Immutable_object) collections for the .NET platform.

Funq isn't the only such library out there. In fact, Microsoft have themselves written such classes, and although they haven't incorporated them into the base class library, they are available of NuGet.

However, Funq has several distinguishing and important features that make it different from pretty much any other library of this sort that has been released for .NET so far.

  1. *Performance:* Funq collections perform very well, usually far better than other collections of the same kind.
  2. *Functionality:* Funq collections provide more inherent functionality than any collection of a similar kind.
  3. *LINQ interface:* Funq collections support true collection-based operations through LINQ syntax. For example, they implement a `Select` statement that returns a collection of the same kind.
  5. *F# Integration*: Funq collections are integrated with F# in the assembly `Funq.FSharp`, which provides extension methods, module bindings, collection builders (computation expressions), and active patterns for working with the collections. 

**Note:** Funq collections can't be called *purely functional* because they involve mutations behind the scenes. This is for performance reasons, as object allocation is expensive in .NET. However, all the mutations Funq performs are invisible, in the sense that they do not change the state of any object visible to the user.

## Current Status
The collections are in an early version right now, and not fit for production use. They're definitely usable and stable, though some things like edge cases and error messages probably need to be worked out.

The collections have reached a very satisfactory level of optimization and functionality, so there is no reason to work on those for now. The following things need to be done:

1. **Design:** Some final design decisions need to be made to finalize the interface, especially the F# interface and things like operators. Also make sure to hide everything that needs to be hidden.
2. **Exceptions:** Make sure the collections throw the right exceptions at the right time.
2. **Integrity testing:** Running all kinds of operations on the collections and checking the result is correct. The collections check themselves through asserts in the code, but a systematic integrity check is required to find the remaining bugs (there are always remaining bugs). This part is mostly done.
2. **Unit testing:** Hand-testing edge cases and exception, mainly. 
2. **Documentation:** Document the entire API. Large parts of it are documented already, but not everything. In particular, the F# side is poorly documented. Internal documentation should also be added to explain how the classes work, in case people fork the project/want to work on it. Some of the code is tricky and messy.

## The Collections

### Sequential Collections
Funq provides two sequential collections:

 1. **FunqList:** Is not just a list. It's a very versatile sequential collection that efficiently implements pretty much every operation you can name, as you can see from the table below. This includes additional and removal from the ends, indexed operations (look up, update, insert, remove), concatenation (including in the middle), and slices. 

 1. **FunqVector:** This collection offers less functionality than FunqList, but performs a lot better for indexing. It supports addition and removal at the end, look up and update by index (but not insertion or removal), and a simple `Take(n)` subsequence operation. It also supports *extremely* fast bulk addition at the end (see benchmarks), as well as fast insertion at any index if the input is very large. It also supports general slices, but at much slower (though still very high) performance.

#### Complexity
Here is an overview of the time complexity of the operations offered by the different sequential collections. Note that time complexity is not always a good indicator of performance in the real world.

	| Collection/Operation | AddFirst     | AddFirstRange | Concat  | AddLast      | AddLastRange | DropFirst    | DropLast     | Insert  | InsertRange | InsertConcat | Lookup  | Remove  | Skip    | Take    | Update  |
	|----------------------|--------------|---------------|---------|--------------|--------------|--------------|--------------|---------|-------------|--------------|---------|---------|---------|---------|---------|
	| FSharpx.Deque        | O(1)         | O(m)          | X       | O(1)         | O(m)         | O(1)/O(n)    | O(1)/O(n)    | X       | X           | X            | X       | X       | X       | X       | X       |
	| FSharpx.Vector       | X            | X             | X       | O(logn)      | O(m log s)   | X            | O(logn)      | X       | X           | X            | O(logn) | X       | X       | X       | O(logn) |
	| FunqList             | O(1)/O(logn) | O(m)          | O(logn) | O(1)/O(logn) | O(m)         | O(1)/O(logn) | O(1)/O(logn) | O(logn) | O(m + logn) | O(logn)      | O(logn) | O(logn) | O(logn) | O(logn) | O(logn) |
	| FunqVector           | X            | O(m + n)      | X       | O(logn)      | O(m + logn)  | X            | O(logn)      | X       | O(m + logn) | X            | O(logn) | X       | O(n)    | O(logn) | O(logn) |
	| System.ImmutableList | O(logn)      | O(m log s)    | X       | O(logn)      | O(m log s)   | O(logn)      | O(logn)      | O(logn) | O(m log s)  | X            | O(logn) | O(logn) | O(n)    | O(n)    | O(logn) |

	X	Operation is unavailable
	/	Means that the complexity to the left is amortized. Worst case is to the right.
	m	length of the input
	s    m + n

#### Benchmarks
These are the benchmark results for the sequential collections, compared with similar collections in different libraries. These benchmarks were performed with particular settings, and different settings yield different results. 

	| Collection/Test      | AddFirst | AddFirstRange | AddFirstRange (concat) | AddLast | AddLastRange | AddLastRange (concat) | DropFirst | DropLast | IEnumerator | Insert | Insert Range | Insert Range (concat operation) | Iterate | Lookup | Remove | Skip  | Take  | Update |
	|----------------------|----------|---------------|------------------------|---------|--------------|-----------------------|-----------|----------|-------------|--------|--------------|---------------------------------|---------|--------|--------|-------|-------|--------|
	| FSharpx.Deque        | 0.438    | 0.226         | 0.322                  | 0.44    | 0.351        | 0.306                 | 0.587     | 1.343    | 0.211       | X      | X            | X                               | 0.267   | X      | X      | X     | X     | X      |
	| FSharpx.Vector       | X        | X             | 2.59                   | 1.981   | 2.411        | 3.082                 | X         | 5.147    | 0.207       | X      | X            | X                               | 0.339   | 0.333  | X      | X     | X     | 7.633  |
	| FunqList             | 2.017    | 0.514         | 0.01                   | 2.105   | 0.503        | 0.011                 | 0.917     | 0.922    | 0.53        | 40.362 | 0.469        | 0.048                           | 0.097   | 1.868  | 11.254 | 0.013 | 0.012 | 7.801  |
	| FunqVector           | X        | 0.019         | 0.09                   | 2.846   | 0.018        | 0.049                 | X         | 2.258    | 0.145       | X      | 0.321        | 0.364                           | 0.059   | 0.141  | X      | 0.28  | 0.002 | 2.745  |
	| System.ImmutableList | 11.378   | 2.473         | 3.469                  | 8.25    | 2.512        | 3.462                 | 4.115     | 16.293   | 1.726       | 29.247 | 2.319        | 3.499                           | 2.147   | 1.691  | 17.375 | 3.136 | 4.03  | 8.968  |
### Sets
Sets are collections that store unique elements. They provide methods for adding elements, removing them, and checking if they already exist in the set. They also provide set-theoretic relations (e.g. set equality, superset, subset), and set-theoretic operations, such as intersection and union. Funq provides two set-like collections.

  1. **FunqSet**, which is an equality-based set that uses hashing.
  2. **FunqOrderedSet**, which is a comparison-based set that stores elements by order. Supports extra operations, such as getting/removing the maximal element, and retrieving elements by sort order index.

It is important to note that Funq sets implement true set-theoretic operations, whereas `System.Collections.Immutable` collections simply work with `IEnumerable`, and do not even check whether the underlying type is a set, implementing everything naively. This makes set-theoretic operations sometimes prohibitively expensive. F#'s set object does implement proper set-theoretic operations, and while this shows, they still underperform when compared to Funq.

#### Time Complexity
These sets and others compared against them implement the standard operations `Contains`, `Add`, and `Drop` in `O(log n)`. The time complexity of other operations is more complicated. 

The set relation operations have worst case time complexity `O(min(n, m))`, but in practice are very fast for arbitrary sets (if implemented properly) because arbitrary sets have many different elements, or different numbers of elements. `IsDisjoint` is likely to run more slowly than other operations for dissimilar sets. Currently, there are no benchmarks involving similar sets.

Set-theoretic operations are even more complex to analyze. All Funq sets implement set-theoretic operations in `O(min(n log m, m + n))`, which basically means they always run proportionally to the smaller set. Not all tested implementations follow this rule, which means that an operation between a set with 10,000 elements and one with just 10 could potentially take much longer than you'd expect.

I haven't determined the time complexity of the other set implementations, so you'll have to make do with benchmarks.

#### Benchmarks
Like I implied in the previous section, in order to properly appreciate the performance of sets you have to test them at different numbers of elements, and with different *types* of elements. Comparison-based sets don't do very well with long string-based keys, but the opposite is true for integer keys. 

In this benchmark, the input collection and the target collection both had 10,000 elements (for AddRange, etc), and the key was string based. 

	| Collection/Test           | Add     | AddRange | Contains | Difference | Drop  | DropRange | Except | IEnumerator | Intersection | IsProperSubset | IsProperSuperset | Iterate | SetEquals | Union   |
	|---------------------------|---------|----------|----------|------------|-------|-----------|--------|-------------|--------------|----------------|------------------|---------|-----------|---------|
	| FSharp.Set                | 94.442  | 97.326   | 0.433    | 257.542    | 1.579 | 31.035    | 98.57  | 0.085       | 25.402       | 0.004          | 0.003            | 1.114   | 0.014     | 53.623  |
	| FunqSet                   | 17.508  | 13.683   | 0.245    | 58.091     | 0.908 | 13.809    | 20.723 | 0.14        | 26.207       | 0.001          | 0.001            | 0.647   | 0.008     | 26.842  |
	| FunqOrderedSet            | 23.195  | 22.213   | 0.392    | 65.955     | 1.652 | 14.734    | 24.599 | 0.113       | 36.834       | 0              | 0                | 0.526   | 0.008     | 27.301  |
	| System.ImmutableSet       | 19.517  | 15.983   | 0.351    | 211.65     | 1.499 | 20.084    | 45.941 | 0.448       | 46.125       | 33.474         | 0.021            | 5.377   | 32.773    | 93.619  |
	| System.ImmutableSortedSet | 141.901 | 138.697  | 2.721    | 634.619    | 3.549 | 50.146    | 151.79 | 0.462       | 149.724      | 141.496        | 0.035            | 4.341   | 144.966   | 187.275 |

Here is another set of benchmarks in which the target collection has 100 elements but the input collection has 10,000 elements (relevant for operations with an input collection). It demonstrates how performance scales with the size of the smaller collection.

	| Collection/Test           | AddRange | Difference | Except | Intersection | SetEquals | Union  |
	|---------------------------|----------|------------|--------|--------------|-----------|--------|
	| FSharp.Set                | 0.325    | 46.927     | 42.065 | 7.156        | 0.011     | 0.802  |
	| FunqSet                   | 0.043    | 1.486      | 0.794  | 0.578        | 0.002     | 0.677  |
	| FunqOrderedSet            | 0.062    | 1.699      | 0.345  | 0.76         | 0         | 0.741  |
	| System.ImmutableSet       | 0.077    | 93.535     | 32.445 | 32.839       | 32.428    | 92.419 |
	| System.ImmutableSortedSet | 0.403    | 227.994    | 77.285 | 76.729       | 112.748   | 1.598  |

### Maps and dictionaries
A map or dictionary is a collection that allows efficiently retrieving values using a key. Funq provides two dictionary collections:

1. FunqMap, which is equality-based and uses hashing.
2. FunqOrderedMap, which is ordered by key. Provides additional operations, such as max key-value pair, and retrieval by sort order index.

These collections support basic operations in `O(logn)`, including `ContainsKey`, `Set`, `Add`, `Remove`, etc. Their performance for these operations is similar to set-like collections, as the former use a map behind the scenes. 

In addition to these standard operations, Funq maps extend set-theoretic operations to maps, implementing the following unique operations:

1. `Merge`, which merges two maps of the same type by key. The user provides a collision resolution function to determine the value in case the maps share identical keys. It is equivalent to `Union` on sets.
2. `Join`, which joins the two maps based on their keys. The user provides a collision resolution function to determine the resulting value (similar to a Join LINQ statement).
3. `Except`, which is identical to the set-theoretic operation. The user can optionally provide a subtraction function that determines the resulting value if the maps share a key (the function returns an `Option` value).
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

## Benchmarking 

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

An option type is kind of similar to a regular reference type (with its dedicated `null` value), but it's more similar to a nullable type such as `int?`. Its primary purpose is to unambiguously indicate a possibly missing value.

1. `Option<T>` is a value type. It is initialized to `None` by default. 
2. This means that an `Option<T>` can always be the target of a method call. In particular, you can call `Equals` or `ToString` on it. A `None` value is also visualized in the debugger.
1. You can define an `Option<T>` for any `T`, including both reference types and value types. You can even have `Option<Option<int>>`.

Although F# defines its own option type, due to technical considerations I decided to implement a separate type.

### F# Integration
**Funq** itself is written primarily in C#, and targets that language. However, the library also has a separate companion assembly that that provides various extensions and modules for use with F#.

Here are some example features:

1. Extension overloads for methods that normally take `Func<T>`. The overloads take F#'s function value.
2. Special F# operators for adding elements to collections, and concatenating them.
2. Module bindings for most of the instance-level operations.
3. Generic active patterns for decomposing collections in various ways.
4. Computational expressions (aka monads) for constructing Funq collections. You can also construct maps and sets in this way.

## Funq.Abstract [Experimental]
Although at this point experimental, Funq uses a library of collection abstractions called `Funq.Abstract` that all collections inherit from. This library implements common collection operations once (such as `Where`, `Select`, and so forth) and inheriting collections reuse those implementations, or alternatively override them with specialized versions. It's also possible to abstract over similar collections using this library, something done in `Funq.FSharp` to reduce repetition, as well as during testing.

This part of the library is inspired by Scala's collection library, which uses advanced type system features such as type variables, higher-kinded types, implicit parameters, and traits. These aren't available in C#, so the library's structure is somewhat convoluted, requires a thorough explanation, and some operations need to be partially implemented by hand. In `Funq.Collections`, method stubs for operations such as `Select` are automatically generated from T4 text templates. 

The library is stand-alone and requires no dependencies.

**Funq.Abstract** offers the following collection abstractions:
 
* `AbstractIterable`, the parent class for all collections that support some form of iteration. Implements LINQ-operations such as `Any, All, Find, ForEach, Aggregate` and provides partial implementations for operations such as `Cast, Select, GroupBy`.

* `AbstractSequential`, the parent class for all sequential collections (where one element follows another in order).  Naively implements operations such as `this[int]`, `FindIndex`, `OrderBy`, and `Take`. 

* `AbstractMap`, the parent class for all map- or dictionary-like collections, offering implementations for set-like operations over maps, such as `Join` (joins by key), `Merge` (merge by key), and so forth. Also offers specialized versions of some of the operations implemented on `Trait_Iterable`.

* `AbstractSet`, the parent class of all set-like collections, offering naive implementations for standard set operations.

## Possibilities
Funq is designed with users directly in mind. However, its combination of power and performance can provide the basis for other libraries. Here are some of the things that could be implemented using the features provided by Funq:

1. Mutable, observable, thread-safe collections supporting such things as implicit copying, snapshots, and history tracking (undo/redo). 
2. An immutable workflow object, composed of individual computation steps, which is also catenable.
3. Various specialized collections, such as immutable and persistent multimaps, multisets, and priority queues.
4. An immutable lazy list, with caching functionality and concatenation.
