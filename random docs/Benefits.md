## Benefits of Persistent Collections

Before elaborating on the different collections offered by **Funq**, I'd like to make an aside about some of the benefits of persistent collections in general. There are quite a few of them, and these are just a small selection.

### Implicit copying
Immutable and persistent collection can be copied implicitly, as an instantaneous `O(1)` operation. This is because you only need to copy the reference to the collection (a simple assignment). This is similar to the way you copy a number; you simply assign a new variable to its value.

### Efficient, non-destructive concat-type operations
Persistent collections have the unique capability of implementing **Concat**, **Union**, and **Intersect** operations efficiently, and in a non-destructive fashion. Many **Funq** collections support these operations in `O(log n)` or better.

### Control over internal state
An object that stores its internal state in an immutable collection has complete control over it.

Just declaring a field `readonly` doesn't guarantee immutability if the object itself can be modified. So for example, if you expose a `readonly` field of type `List<T>`, you've allowed your internal state data to be modified.

Immutable and persistent collections don't have this drawback. You can expose them, including all the functionality they support, without risking them being modified.

### Implicit thread-safety & atomicity
Operations executed on an immutable collection are implicitly thread-safe. They are also atomic for most purposes.

This is because the only externally visible changes caused by the operation are a function return. All mutation (if any) occurs in the context of a method, and every thread by definition accesses different locals.

### Implicit history tracking, snapshots

In most cases, previous versions of persistent collections aren't explicitly required and as such are likely to go to garbage collection. However, nowadays many applications are expected to support undo, redo, and history tracking. Built-in persistence and immutability, which also means implicit snapshotting, is a simple and powerful way of implementing these operations.

More on this in the Extras section.

### Using the value abstraction on collections
One overarching theme or class of benefits that is related to all those discussed above, is the ability to treat collections as integral values. When you store an integral value, such as an integer, the value itself is both theoretically and practically immutable. That is, the number `5` itself cannot change into a `4`, although `5 - 1 = 4`, this just yields a different number.

The same principle applies to lists of things. The list `[1; 2; 3]`, when taken as a value, can't be changed to `[1; 2]` no more than the number `4` can be changed to `5`. Nevertheless, collections that aren't persistent or aren't immutable don't handle this value abstraction very well, if at all. Strings are true values, but a `List<char>` isn't.
