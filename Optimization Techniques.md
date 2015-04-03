## Optimization Techniques
Funq uses a number of optimization techniques to improve the performance of its collections. Funq collections implement new algorithms or designs (at least, I have never seen them implemented before) for:

1. Starting subequence for an array-mapped trie in `O(logn)`.
2. Extremely fast bulk addition to the end for an array-mapped trie (in `O(logn + m)`, but the performance is a lot better than even this implies).
2. Dedicated insertion and removal of a single element in a finger tree (the common way involves splittng and concatenting the tree; the algorithm used here is a lot more efficient).
3. Non-recursively iterating over a finger tree, which is kind of tricky because of its complex and nested type.

I will write an article about these algorithms later on. Besides these specialized algorithms, Funq uses a unique optimization feature called Lineages that can greatly improve performance for bulk operations, and allow mutable collection builders to be implemented safely and efficiently (mutable collection builders are used under the hood right now, but will be fully functional in a later version). This is a unique feature that I've never seen implemented elsewhere.

### Lineages
Fully (and confluently) persistent collections are bound by the obligation that previous versions of a collection must never undergo mutation. However, a persistent collection that is not purely functional can perform mutations on local objects, before the user has had a chance to see them (alternatively, the user will never see them at all).

Funq collections use a simple system but very effective system to know whether the user has had a chance to see a particular object (such as a particular node in a binary tree). Each user-initiated operation is performed with a reference object called a `Lineage`, and any objects created as part of that operation are annotated with that Lineage (they are said to be descended from that operation). Before the operation has returned, the user has not seen any object of that particular lineage, while the user is guaranteed to have seen any object of an older lineage.

Sometimes, instead of allocating a new object, the code can instead mutate an existing object, but only if the lineage of the operation is reference equal to the lineage of the object. After a user-initiated operation ends, the lineage used as part of that operation is discarded and is never to be used again.

Alternatively, if we wish to implement a mutable collection builder, the lineage is preserved between operations. Note that some operations such as concatenation will still require resetting the lineage, as corruption will inevitably result.

Note several things about this scheme:

1. Lineages are local objects created in the body of a method, so it's not possible for different threads to use the same lineage. Thread safety is not a concern here. Alternatively, the lineage can be stored if the object is a mutable collection builder, but in that case the collection isn't guaranteed to be thread safe.
2. Objects of older lineages can never point to objects of newer lineages. When those older objects were created or mutated, objects of newer lineages did not exist yet. For this reason, mutating objects of only the latest lineage has no effect on objects from older lineages.
3. Lineages have obvious performance benefits when dealing with multi-stage operations (e.g. `AddRange`), since in many cases nodes can be mutated instead of being allocated anew. However, they also have benefits in some single-stage operations, because they can also involve multiple steps that sometimes allocate objects needlessly (e.g. creating nodes for a binary tree, and then balancing them).
4. There is a special singleton lineage called `Lineage.Immutable` that can never be mutated. This lineage is used in some cases, such as if mutable lineages give no benefit, or if mutations are particularly dangerous in a certain operation. If the `Funq.Collections` assembly is compiled with the `NO_MUTATIONS` flag, all lineages are replaced by the immutable lineage, disabling all these naughty mutations. This is useful for debugging, and also if you're a purist.
5. Mutations are possible sources for bugs, which makes writing safe code using lineages rather tricky. However, they can never affect the persistence of the data structure, because lineages are never reused for multiple operations, so any older versions of the data structure can never be mutated. Instead, bugs will simply result in errors or corruption in the new version.  They're not different from any other kind of bug.
6. Although not currently used in this way, lineages can be used to investigate which nodes were created by which operations. This may be an interesting way to study the data structure. 