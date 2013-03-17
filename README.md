Solid
=====

Solid is a library of _immutable_ or _persistent_ data structures for .NET languages, mainly C# and F#. These are data structures that cannot be modified in-place. Instead, operations such as `Add`, `Remove`, `Set`, and so forth return new versions of the data structure that share some of the structure with the old one.

Right now, it consists of three usable data structures.
* `Sequence<T>` (name change pending), a high performance _double-ended queue_ data structure that offers many fast operations. This is pretty much complete, aside from additional testing.
  * Constant time access, addition, and removal from either end (`AddLast, AddFirst, DropFirst, DropLast, First, Last`).
  * Fast logarithmic time indexing (`this[index]`)
  * Slower logarithmic time concatenation, splitting, and similar (`Take, TakeLast, Split(out, out), Append, Concat(params Sequence<T>[]), Skip`
  * Slower logarithmic time insertions of various kinds (`Insert int,T, InsertRange int,IEnumerable<T>, Insert int,Sequence<T>, Set T, ...`)
  * Fast linear time reversal, filter, iteration.
  * Linear time `Select-`like transformation on the whole data structure.
  Written as a highly optmized version of [2-3 Finger Trees](http://www.soi.city.ac.uk/~ross/papers/FingerTree.pdf).
* `Vector<T>`, a high performance dynamic array-like data structure that offers fast indexing operations. This is not done yet and is pending some substantial revision (adding some deferred execution that should drastically improve performance in some cases). Implemented as a trie, similar to [Clojure's implementation](http://blog.higher-order.net/2009/02/01/understanding-clojures-persistentvector-implementation/)
* `HashMap<TKey,TValue>`, a high performance Dictionary-like key-value map that uses equality semantics (instead of comparison semantics). Supports basic `Set(Tkey,TValue), Add(Tkey,TValue), Remove(Tkey,TValue), this[Tkey]` operations. Implemented as a HAMT similar to [Clojure's implementation](http://blog.higher-order.net/2009/09/08/understanding-clojures-persistenthashmap-deftwice/). Also pending revision to support possibly deferred execution, bulk loading, and set-like operations.
