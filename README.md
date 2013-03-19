Solid
=====

Solid is a library of _immutable_ or _persistent_ data structures for .NET languages, mainly C# and F#. 
They are written in C# (I chose not to write them in F# mainly due to performance reasons -- F# abstracts the IL much more deeply than C# does, and makes it inconvinient to take certain (normally unwise) design choices that maximize performance. 

These are data structures that cannot be modified in-place. Instead, operations such as `Add`, `Remove`, `Set`, and so forth return new versions of the data structure that share some of the structure with the old one.

Right now, it consists of three usable data structures.
* `Sequence<T>` (name change pending), a high performance _double-ended queue_ data structure that offers many fast operations. This is pretty much complete, aside from additional testing.
  * Constant time access, addition, and removal from either end (`AddLast, AddFirst, DropFirst, DropLast, First, Last`).
  * Fast logarithmic time indexing (`this[index]`)
  * Slower logarithmic time concatenation, splitting, and similar (`Take, TakeLast, Split(out, out), Append, Concat(params Sequence<T>[]), Skip`
  * Even slower logarithmic time insertions of various kinds (`Insert int,T, InsertRange int,IEnumerable<T>, Insert int,Sequence<T>, Set T, ...`)
  * Fast linear time reversal, filter, iteration.
  * Linear time `Select-`like transformation on the whole data structure.
* Written as a highly optmized version (that is, optimized for C#) of [2-3 Finger Trees](http://www.soi.city.ac.uk/~ross/papers/FingerTree.pdf).
* `Vector<T>`, a high performance dynamic array-like data structure that offers fast indexing operations. This is not done yet and is pending some substantial revision (adding some deferred execution that should drastically improve performance in some cases). Implemented as a trie, similar to [Clojure's implementation](http://blog.higher-order.net/2009/02/01/understanding-clojures-persistentvector-implementation/)
* `HashMap<TKey,TValue>`, a high performance Dictionary-like key-value map that uses equality semantics (instead of comparison semantics). Supports basic `Set(Tkey,TValue), Add(Tkey,TValue), Remove(Tkey,TValue), this[Tkey]` operations. Implemented as a HAMT similar to [Clojure's implementation](http://blog.higher-order.net/2009/09/08/understanding-clojures-persistenthashmap-deftwice/). Also pending revision to support possibly deferred execution, bulk loading, and set-like operations.

Additional Features
===================

Besides the standard interface for manipulating the data structures, there are (or will be) a few more language-specific features. For C#, there will be an interface for decleratively constructing `Sequence<T>` collections via LINQ queries. An example follows.
```CSharp
 Sequence<T> result = from item in Enumerable.Range(0, 100).DelaySequence() where item % 2 == 0 orderby item select item
```
This is made possible by the `DelaySequence()` method which returns an object of type `DelayedSequence<T>`, for which the various query methods have been re-implemented. It will also have some caching logic (e.g. the `DelayedSequence<T>` will not needlessly construct the same data structure twice).
For F# there will exist computation expressions that work as follows.
```FSharp
 let x = Sequence {for item in {0 .. 100} -> i.ToString()}
 let y = Vector {for item in {0 .. 100} -> i}
```
