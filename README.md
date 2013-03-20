Solid
=====

Solid is a library of _immutable_ or _persistent_ data structures for .NET languages, mainly C# and F#. 
They are written in C# (I chose not to write them in F# mainly due to performance reasons -- F# abstracts the IL much more deeply than C# does, and makes it inconvinient to take certain (normally unwise) design choices that maximize performance. 

These are data structures that cannot be modified in-place. Instead, operations such as `Add`, `Remove`, `Set`, and so forth return new versions of the data structure that share some of the structure with the old one.

The Data Structures
===================
This library consists of a small number of data structures with a wide array of capabilities. Right now it has three, but I might added a few more in the future. It is currently late alpha stage.

Sequence<T>
-----------
This is a sequential data structure that provides a wide assortment of fast operations. It is chiefly designed as a double-ended queue, and provides very fast access, addition, and removal from either end. Although it is only constant amoritized time, worst-case performance for add/remove to the ends is `O(logn)`, is very rare, and in practice does not have a substantial impact on performance. It also provides a constant-time `Count` property.

`Sequence<T>` also provides very fast indexed operations, such as get by index, set by index, and insert at index. It should support removing at an index soon as well, when I get around to it. In practice, although it is written as a sequential access data structure, its indexing performance is competitive with some data structures specifically written for indexing.

It also supports all sorts of splits, subsequences, and concatenation. These are not much more expensive than insertion in the middle, which as I've mentioned, is very fast.

Vector<T>
---------
This data structure was written exclusively to provide high-performance indexed operations. It is significantly faster than `Sequence<T>` at indexing and is also significantly faster at this than any immutable data structure written for .NET that I've seen. It also provides extremely fast `Take(int)`, which returns the first `n` items from the start of the data structure and an `O(1)` Count property.

HashMap<TKey,TValue>
--------------------
A fast key-value map that uses equality semantics, rather than comparison semantics. This data structure needs more work, although it is currently functional.
Interface
===================
The library provides a fairly uniform interface in the style of `C#`. Examples of methods are `AddLast, AddFirst, Take, Skip, Split,DropLast,DropFirst` and properties: `First, Last, Count, ...`. The data structures do not implement any particular interfaces at this time, besides `IEnumerable<int>`.

F#
--

In addition to this, an included `F#` module provides operators and functions. At present, the following operators are defined. Consider `X` to be the collection and `n` to be the input.

    X <+   n  Alias for X.AddLast(n)
    n +>   X  Alias for X.AddFirst(n)
    ns ++> X  Alias for X.AddRangeFirst(ns)
    X <++  ns Alias for X.AddRangeLast(ns)
    X <+>  Y  Alias for static Concat (X, Y)

There are also functions such as `dropf, dropl, length, filter, map, ...` that work for all collections and can be considered operators.

There are also two computation expression builders, one for `Sequence<T>` and one for `Vector<T>`. Here is an example of how to use them.
```FSharp
 let x = Sequence {for item in {0 .. 100} -> i.ToString()}
 let y = Vector {for item in {0 .. 100} -> i}
```

C#
--
Besides manipulating data structures using the direct interface, there is also a feature resembling a list comprehension. Using LINQ, you can decleratively construct a data structure in the manner you want. Take this example:
```CSharp
 var result = from item in Enumerable.Range(0, 100).DelaySequence() where item % 2 == 0 orderby item select item
```
The result of the LINQ query (the `DelaySequence()` is what provides this additional functionality) is an object of type `DelayedSequence<T>`. It is an `IEnumerable<T>`. You can perform an implicit conversion, like this:
```CSharp
 Sequence<T> result2 = result;
```
This will automatically evaluate the above LINQ query and construct a `Sequence<T>` object. It is also possible to iterate over the `DelayedSequence<T>`, like this:
```CSharp
foreach (var item in result)
{
 //do things...
}
```
Iterating over the object will also implicitly cache the `Sequence<T>` so that the implicit conversion will just return the cached `Sequence<T>`.
