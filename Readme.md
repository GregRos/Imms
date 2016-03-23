# Imms
---

**Imms** is a library of [persistent](http://en.wikipedia.org/wiki/Persistent_data_structure), and [immutable](http://en.wikipedia.org/wiki/Immutable_object) collections for the .NET framework. 

It is available on [NuGet](https://www.nuget.org/packages/Imms/). The F# integration library is available [here](https://www.nuget.org/packages/Imms.FSharp).

There are a bunch of similar libraries but, well, Imms is just a lot better than them.

1. **More Functionality:** Imms collections provide more operations than other libraries. A lot of these are unique to immutable collections, such as very fast splitting and concatenation. Others were added for the sake of completeness. For example, maps support key-based joins, ordered sets support slices and retrieval by index, and more.

2. **Higher Performance:** Imms collections typically perform as well or (quite often) a lot better than other collections.

3. **Lots of Documentation:** The public API is largely documented. Every method and type has a informative summary at the very least. There is a lot more that will be done though, especially with exception and performance information.

5. **LINQ Interface:** Imms mostly follows the much more expressive [LINQ API][LINQ API], rather than duplicating the awkward and seldom-used [traditional API][Traditional API] found in the Collections namespace.

4. **High Level of Polish:** Imms collections throw the right exceptions at the right times (no `NullReferenceExceptions` popping up at awkward moments), have a consistent and intuitive API, etc.

5. **F# Integration:** Although Imms itself is written in C# for technical reasons and is primarily targeted for that language, it comes with an optional F# integration library that contains things like module bindings, active patterns, collection builders, and everything else. The test code (exclusively in F#) was written with the aid of this library.

6. **Tons of other things** that don't fit on this list. For example, the library also provides a nifty `Optional` type, structural equality for collections, ... 

[LINQ API]:https://msdn.microsoft.com/en-us/library/system.linq.enumerable(v=vs.100).aspx#memberList

[Traditional API]:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx#idMethods

Currently, all library assemblies require .NET Framework 4.0 Client Profile, and all test assemblies require .NET Framework 4.5.1. The F# libraries require F# 3.0.

## What is an Immutable Collection
It's a collection that cannot be modified, but still support all kinds of operations, such as `Add` and `Remove`. Instead of modifying the collection, operations such as `Add` return a new version of the collection, but with the additional modifications.

They're a lot like .NET strings, where operations such as `Replace` don't modify the string, but return a new string instead.

For that matter, they're also like numbers, where `4 + 1` does not modify the number `4` (which doesn't even make sense), and instead returns a different number with our 'modification' included.

The benefit of immutability is that after `string.Replace`, the original string hasn't been changed. After `4 + 1`, the number `4` also thankfully remains the same (as far as we know).

Writing an immutable and persistent collection is pretty hard. Or it's hard to write an efficient one, at least. You can always make an immutable and persistent array by copying the whole thing every time a modification is made, and the problem there is obvious. But collections in libraries such as `Imms` make use of *structural sharing*, which makes the collections a lot more practical and efficient, and basically means that small operations still take little time.

Really, there is a lot to be said about immutable collections, or immutability in general. And a lot of people have said it, many of them much more interesting to read than I. So here are some links. Most of them don't really concern .NET, because immutability is something that works in every language.

* [Why immutable collections?](https://scott.mn/2014/04/27/why_immutable_collections/)
* [Objects Should Be Immutable](http://www.yegor256.com/2014/06/09/objects-should-be-immutable.html)
* [Java theory and practice: To mutate or not to mutate?](http://www.ibm.com/developerworks/library/j-jtp02183/)

For a more thorough discussion on Immutability, I strongly recommend Eric Lippert's blog and his (still-relevant) series about immutability and C#. It even tells you how to implement one of the data structures in this library! Well, kind of, anyway.

* [Eric Lippert: Immutability, Results Page 3](https://blogs.msdn.microsoft.com/ericlippert/tag/immutability/page/3/)

Then again, I do have a few things to say on the subject as well, so I might write something up about it myself too.

## The Collections

**Imms** provides 5 main collections, divided into 3 groups. These are "building block" type collections and can be used to implement more specialized collections in various ways (in fact, I might do so eventually, if someone else doesn't beat me to it).

### Sequentials
Sequential collections store elements in order. An example of a sequential collection is `List<T>`.

The primary sequential collection is `ImmList`, 

**Imms** provides the following sequential collections:

 1. **ImmList:** A very versatile sequential collection that supports pretty much every operation you can name, including addition/removal at either end, get and set by index, insert/remove by index, concatenation, splitting, subsequences... all of these are implemented using specialized algorithms that perform up to `O(logn)`.
 1. **ImmVector:** Offers less functionality than ImmList, but performs a lot better for most operations. It is generally recommended that you use ImmList for most purposes. ImmVector's performance approaches mutable collections for some operations.

### Sets
Sets store collections of unique elements. An example of a set is `HashSet<T>`.

**Imms** provides the following set collections:

  1. **ImmSet**, which is an equality-based set that uses hashing, similarly to `HashSet` (except that it is immutable, of course).
  2. **ImmSortedSet**, which is a comparison-based set that stores elements in order. Supports additional operations, such as retrieval by sort order index.

Both sets support the following various set-theoretic operations. These operations accept an `IEnumerable<T>`, but are actually significantly faster when the input collection is of the same type and uses the same membership semantics. The sets support the following:

1. Add, Remove, Contains, Length, ...
2. Intersect, Union, Except, (Symmetric) Difference
2. RelatesTo, which returns the set relation as an enumeration (with values such as `ProperSubsetOf`).
3. IsSetRelation-type operations, such as IsSubsetOf. 

### Maps
Maps store key-value pairs and allow for fast retrieval by the key. They are also known as dictionaries. An example of a map is `Dictionary<TKey, TValue>`.

**Imms** provides the following map collections:

1. **ImmMap**, which is equality-based and uses hashing, similarly to `Dictionary<,>` (except that it is immutable).
2. **ImmSortedMap**, which is ordered by the key. Provides additional operations, such as retrieval by sort order index.

Both maps support set-theoretic operations extended to maps. These operations `IEnumerable<T>`, but are actually significantly faster when the input collection is of the same type and uses the same key semantics. The maps support the following:

1. Add, Remove, Contains, Length, ...
2. `Join`, which is similar to the LINQ operation of the same name, and is analogous to Intersect over sets. You must provide a selector to determine the value in the result map.
3. `Merge`, which combines two maps into one. It's analogous to Union over sets. You can provide a selector to determine the value in the result map, in case of a duplicate.
4. `Subtract`, which is similar to `Except` over sets, except that you provide a selector that determines the value in the result map, and this selector may also indicate that the key-value pair should be removed.
5. `Difference`, which is similar to `Difference` over sets. No selector can be provided in this case.

## API Gotcha's
A few things you should be aware of about the API.

1. `Length` returns the length of a collection, not `Count`. `Count` is instead a method that counts the number of items in the collection (like in LINQ).
2. Collections don't have visible constructors. You must construct them using factory methods, e.g. `ImmList.Empty<int>()`.
3. Collections support negative indexing (see sections below). This means that things that used to throw exceptions can instead cause unexpected behavior.
4. `AddLast`/`AddFirst` add items to sequential collections, not `Add` alone.
5. The collections 'override' LINQ operations. To use the original LINQ operations (which are lazy and return `IEnumerable`), use `AsEnumerable`.
6. The collections are sealed.

### F# Companion Library
1. F# option type is not the same as **Imms**'s `Optional` type. Sorry.
2. Instance methods exposed by collections often take parameters of type `Func<T>`, which isn't directly compatible with F#'s function objects. To get around this, use the module bindings.
3. Some constructs in the `Imms.FSharp.Implementation` namespace are accessible, but are not meant to be used in user code, and aren't supported.

### Note about Maps and Sets
The set and map collections in this library support custom equality and comparison semantics (by accepting an `IComparer<T>` or `IEqualityComparer<T>`). This isn't as trivial as it sounds.

Remember that these collections use special algorithms for operations such as `Intersect` and `Union`. These algorithms only make sense when both collections are compatible, i.e. use the same equality or comparison semantics. Otherwise, the result will be corrupted.

To avoid dangerous and hard to track bugs, **Imms** collections only use the special algorithms if both collections use the same equi/comp handler. This is determined by calling `.Equals`. For this reason, if you plan to use a custom handler, you should either:

1. Make sure to use the same handler instance for all **Imms** collections that use that handler. This pattern is made more convenient by extension methods on handlers that lets you use them as 'factories' of collections. An example is `IComparer<T>.CreateSortedSet`.
2. Override `.Equals` on your custom handler to support functional equality.

If **Imms** decides that the comparison handlers are different, a generic implementation will be used, which can be significantly slower. That is to say, the implementation is as slow as what some other collection libraries use. ◕‿◕



## Extra Features

### Compatibility Interfaces
Imms collections implement various interfaces such as `IList<T>` and others for the sake of compatibility.

### Implicit Optimization
Although re-iterated in other parts, it is worth noting here as well. **Imms** allows you to specify an `IEnumerable<T>` for many operations. 

However, operations can often be much faster (sometimes by several orders of magnitude) if the input is a collection of the same type. When this applies, it will be noted in the description.

Milder performance benefits can also be achieved for other known collection types.

This approach is chosen because otherwise, collections would be cluttered up by many methods that essentially do the same thing.

### LINQ Implementation
**Imms** collections implement 'override' LINQ operations so that they return a collection of the same kind. This is usually very convenient. In addition, the implementation is generally much faster than the generic LINQ implementation for various reasons.

You can still use the default LINQ operations by calling `AsEnumerable`.

### Optional type
**Imms** provides an optional value type. An optional value type is used to indicate a possibly missing value. It is similar to how we sometimes us `int?` to indicate the possibility of a missing integer. 

This option type is called `Optional<T>` and has two 'states':

1. `Some(v)`, in which state the object wraps a value `v` of type `T`.
2. `None`, in which state the object indicates a missing value.

It's really very similar to a nullable type, except that it can be used on reference types as well. You can even have `Optional<Optional<T>>`. 

`Optional<T>` is a struct, which has many advantages. For example, you can always view it in the debugger, you can always call methods such as `ToString()` on it, you can always recover what missing `T` it represents, etc. It is initialized to `None` by default.

**Imms** provides a variety of methods to work with optional values, such as `Map`.

#### Use in **Imms**
The optional value type is used quite frequently. Generally, any method like `bool TryX(object,out T)` is instead written with the more elegant, `Optional<T> TryX(object)`. 

Another example is the method `Choose`, which is similar to `Select`, except that it takes a selector of the form `Func<T, Optional<TOut>>` and returning `None` indicates that the value should be ignored.

### Convenience Features
The library offers the following minor features:

#### Negative Indexing
Every operation supports negative indexes, which indicate distance from the end of the list. 

For example, `list[-1]` gets the last item of `list` and `list[-3]` gets the third one from the end. Using negative indexing, `list[-list.Length]` gets the first item.

#### Slices Indexer
Collections that support indexing allow you to get a slice of the collection using the `[int,int]` indexer. For example, the following gives you a slice starting with index `2` and stretching to the end of the list: `list[2, -1]`.
#### Sequence Equality
**Imms**'s sequential collections implement structural equality, overriding `Equals` and `GetHashCode`, as well as the `==` operator. 

For two collections to be equal, they must be of the exact same type, and must also contain the same sequence of elements. 

The equality comparer used to equate elements is the default equality comparer, and this cannot be changed. However, the `SequenceEquals` method lets you provide your own comparer.


### F# Integration
**Imms** is written primarily in C# and targets that language. But the library has a separate companion assembly, `Imms.FSharp`, that provides various extensions and modules for use with F#.

These modules were heavily used in performance and integrity testing.

Here are some example features:

2. Special F# operators for adding elements to collections, and concatenating them.
2. Module bindings for most of the instance-level operations.
3. Generic active patterns for decomposing collections in various ways.
4. Computational expressions (aka monads) for constructing **Imms** collections. You can also construct maps and sets in this way.

## Performance
To see up to date benchmarks you can go to the [benchmarks folder](https://github.com/GregRos/**Imms**/tree/master/**Imms**/**Imms**.Tests.Performance/Benchmarks). Each set of benchmarks includes charts, a CSV table, and a CSV log file with explicit information about the parameters of the benchmark. The log files are very detailed.

The benchmarking system itself is available in the namespace `Imms.Tests.Performance`. It really is a system, and the way it works is quite complicated. However, running it is quite self-explanatory. There are lots of settings you can tweak.

It's written in F#, and heavily uses (or perhaps _abuses_ is a better word) the *inline* functions feature, which basically allows performance test code to be generated implicitly, so that the human-written code is generic, but still executes with very little overhead. I'll write an article about it at some point, as it involves concepts that can be reused.

### Sequential Collections

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
	| FSharpx.Deque        | 0.448    | 1.987         | 2.229                  | 0.414   | 1.802        | 2.437                 | 0.172       | X      | X            | X                     | 0.23    | X      | X      | 0.33        | 1.208      | X     | X     | X      |
	| FSharpx.Vector       | X        | X             | X                      | 1.208   | 6.212        | 6.604                 | 0.15        | X      | X            | X                     | 0.197   | 0.282  | X      | X           | 4.381      | X     | X     | 5.983  |
	| ImmList              | 2.139    | 1.651         | 0.011                  | 2.006   | 1.834        | 0.012                 | 0.425       | 14.892 | 1.793        | 0.025                 | 0.092   | 1.548  | 10.395 | 0.948       | 0.969      | 0.007 | 0.008 | 6.6    |
	| ImmVector            | X        | 0.64          | 0.538                  | 2.504   | 0.102        | 0.273                 | 0.115       | X      | 0.378        | 0.546                 | 0.032   | 0.415  | X      | X           | 2.21       | 0.161 | 0.002 | 2.853  |
	| System.ImmutableList | 9.618    | 17.089        | 22.52                  | 9.508   | 17.148       | 22.937                | 1.612       | 11.69  | 17.947       | 23.7                  | 1.923   | 1.161  | 8.048  | 4.812       | 5.261      | 1.817 | 1.376 | 5.07   |

### Sets

#### Time Complexity
The following is the time complexity of **Imms** sets for different operations. Time complexity is much better when the two inputs are sets of the same type and with the same membership semantics.

I don't have similar data about sets from other libraries.

	| Set Operation           | Compatible                  | Naive             
	|-------------------------|-----------------------------|-------------------
	| Add, Remove, Contains   | logn                        |                   
	| IsSuperset/IsSubset/etc | min(m,n)                    | m logm + min(m, n)
	| Intersect               | min(m logn, n logm, m + n)* | m (logn + logm)   
	| Union                   | min(m logn, n logm, m + n)* | m log(m + n)      
	| Except                  | min(m logn, n logm, m + n)* | m log n           
	| Xor / Sym. Difference   | min(m logn, n logm, m + n)* | (m + n) log(m + n)
	* Heuristically, based on the original algorithms.	

The naive option is used when one of the collections is not a set. Note that in pretty much all cases, operations between two compatible sets take time proportional to the smaller of the two.

#### Benchmarks
(Note strings were used to benchmark the collections. F#'s ordered map and set force ordinal comparison for strings, which generally means that the collections can't order strings properly, but perform better)

Like I implied in the previous section, in order to properly appreciate the performance of sets you have to test them at different numbers of elements, and with different *types* of elements. Comparison-based sets don't do very well with long string-based keys, but the opposite is true for integer keys. 

In this benchmark, the input collection and the target collection both had 10,000 elements (for AddRange, etc), and the key was string based. 

	| Collection/Test           | Add     | AddRange | Contains | Difference | Except | IEnumerator | Intersection | IsProperSubset | IsProperSuperset | Iterate | Remove | RemoveRange | SetEquals | Union  |
	|---------------------------|---------|----------|----------|------------|--------|-------------|--------------|----------------|------------------|---------|--------|-------------|-----------|--------|
	| FSharp.Set                | 61.475  | 61.298   | 3.109    | 118.343    | 46.464 | 0.605       | 9.833        | 0.002          | 0.002            | 0.756   | 12.53  | 25.874      | 0.007     | 21.08  |
	| ImmSet                    | 53.953  | 21.787   | 1.773    | 21.722     | 7.295  | 0.984       | 8.105        | 0.002          | 0.001            | 0.25    | 5.979  | 9.274       | 0.005     | 10.429 |
	| ImmSortedSet             | 109.294 | 60.933   | 15.481   | 47.394     | 16.212 | 0.78        | 36.875       | 0.002          | 0.001            | 0.12    | 20.646 | 33.55       | 0.006     | 17.64  |
	| System.ImmutableSet       | 75.933  | 38.076   | 2.368    | 106.883    | 18.302 | 3.737       | 18.279       | 14.165         | 0.008            | 4.326   | 14.685 | 18.173      | 14.092    | 49.788 |
	| System.ImmutableSortedSet | 100.757 | 79.588   | 17.452   | 306.202    | 74.829 | 2.424       | 65.854       | 50.431         | 0.019            | 2.956   | 24.725 | 40.408      | 49.866    | 91.959 |

Here is another set of benchmarks in which the target collection has 100 elements but the input collection has 10,000 elements (relevant for operations with an input collection). It demonstrates how performance scales with the size of the smaller collection.

	| Collection/Test           | Add    | AddRange | Contains | Difference | Except | IEnumerator | Intersection | IsProperSubset | IsProperSuperset | Iterate | Remove | RemoveRange | SetEquals | Union  |
	|---------------------------|--------|----------|----------|------------|--------|-------------|--------------|----------------|------------------|---------|--------|-------------|-----------|--------|
	| FSharp.Set                | 47.892 | 47.515   | 1.03     | 18.364     | 17.115 | 0.61        | 3.474        | 0.002          | 0.002            | 0.01    | 3.896  | 0.096       | 0.006     | 0.395  |
	| ImmSet                    | 43.781 | 15.273   | 0.566    | 0.686      | 0.327  | 0.852       | 0.226        | 0.006          | 0.001            | 0.004   | 3.106  | 0.039       | 0.002     | 0.282  |
	| ImmSortedSet             | 75.199 | 41.317   | 7.678    | 1.572      | 0.51   | 0.747       | 0.912        | 0.009          | 0.001            | 0.001   | 10.715 | 0.146       | 0.001     | 0.613  |
	| System.ImmutableSet       | 64.296 | 32.131   | 1.36     | 47.848     | 16.171 | 3.702       | 15.645       | 16.928         | 0.008            | 0.053   | 7.665  | 0.104       | 14.281    | 43.856 |
	| System.ImmutableSortedSet | 87.077 | 57.689   | 7.742    | 126.132    | 37.829 | 2.46        | 39.116       | 75.305         | 0.011            | 0.037   | 11.961 | 0.181       | 49.086    | 0.802  |

### Maps and dictionaries


#### Benchmarks

**Technical Note:** `System.Collections.Immutable` dictionaries have a mechanism that checks whether values are equal (using the default equality comparer), and if they are, it doesn't update them. I forced this mechanism off for the purpose of this benchmark because of the way I generate data (as identical key-value pairs).

	| Collection/Test            | Add     | AddRange | IEnumerator | Iterate | Lookup | RemoveKey | RemoveRange |
	|----------------------------|---------|----------|-------------|---------|--------|-----------|-------------|
	| FSharp.Map                 | 63.886  | 63.895   | 0.648       | 0.823   | 3.266  | 13.633    | 28.199      |
	| ImmMap                     | 57.989  | 25.13    | 0.845       | 0.414   | 1.837  | 9.445     | 9.499       |
	| ImmSortedMap              | 100.089 | 68.299   | 0.664       | 0.27    | 15.43  | 22.604    | 33.056      |
	| System.ImmutableDict       | 99.958  | 47.393   | 3.819       | 4.351   | 2.702  | 23.377    | 20.237      |
	| System.ImmutableSortedDict | 101.172 | 75.326   | 2.084       | 2.674   | 15.63  | 28.808    | 42.563      |

## Possibilities
**Imms** is designed with users directly in mind. However, its combination of power and performance can provide the basis for other libraries. Here are some of the things that could be implemented using the features provided by **Imms**:

1. Mutable, observable, thread-safe collections supporting such things as implicit copying, snapshots, and history tracking (undo/redo). 
2. An immutable workflow object, composed of individual computation steps, which is also catenable.
3. Various specialized collections, such as immutable and persistent multimaps, multisets, and priority queues.
4. An immutable lazy list, with caching functionality and concatenation.
