# FingerTree
This is an implementation of a 2-3(-4) finger tree, but it's somewhat different from the source for many reasons.

## Differences
Firstly, I use a lot of imperative constructs and some safe mutation behind the scenes in order to improve performance. Object allocation is expensive in .NET, so it's usually better to reuse old objects rather than to construct new ones, if this can be done without affecting the data structure's overall persistence and immutability, as seen from the user's point of view. More discussion about how safe mutation is achieved can be found in other places.

Secondly, the structure of the finger tree is somewhat different.

1. The original version had a `Tree`, a `Digit (1/2/3/4)`, and `Node2/Node3` (as well as, possibly, a `Leaf`). This one just has a `Tree`, `Digit`, and `Leaf`. `Digit` is used for nodes as well as digits.
2. In order to allow other optimizations, there is a single `Digit` object with 4 fields (some of which may be empty), rather than different digit objects for each digit type. The rule of the finger tree is still obeyed for addition/removal from the ends: only `Digit` objects containing 2 or 3 elements may be found inside the deeper tree as elements. Insertion in the middle breaks this rule somewhat, allowing digits in the middle to have 4 elements.
3. In the original finger tree, `Tree` has three cases: `Empty`, `Single v`, and `Deep` where `Single v` contains a single element. In this version, `Single` contains one digit instead, which itself may contain from 1 to 4 elements. The original reason behind this was simply a misunderstanding, but it actually makes some algorithms simpler, like split and concat.

As is the case with many implementations, this one is specialized for sequences, rather than using arbitrary measures. You could transform the code so the measure is a `max` measure instead, for a priority queue, but you'd have to do it rather carefully, as the data structure wasn't written for this directly in mind.




This is a massive class, no doubt about it, and very complicated. I'm using the same class structure as for the other data structures.

`FingerTree<TValue>` is a static partial container class that contains most of the classes involved in the implementation of a finger tree containing leaf values of type `TValue`. In this structure, all finger tree classes are aware of the leaf value, even though the deeper trees don't store values of that type directly. This gets rid of some of the overhead involved in the finger tree.

 `FingerTree` shares the common `TValue` parameter among multiple classes, so I don't have to parameterize every class unnecessarily.

The container class contains `FTree<TChild>`, itself an abstract container class. It contains two kinds of things:

1. The finger tree case classes.
2. The digit class.

It also shares the `TChild` parameter, which specifies the immediate children of the data structure. For a level 1 tree, this would be `Leaf<TValue>`, giving the type `FingerTree<TValue>.FTree<Leaf<TValue>>`. For a level 2 tree, it would be `FingerTree<TValue>.FTree<Leaf<TValue>>.Digit`, or just `Digit` when I refer to it inside a level 1 tree, giving the type (again, in the level 1 tree) `FTree<Digit>`. 

In this structure, I don't use Node2/Node3, but instead just have a Digit, which can have between 1 and 4 elements. The standard finger tree rule is still obeyed: only the topmost digit can have 1 and 4 elements, while the deeper digits (those pushed inside the deep tree as elements) can have either 2 or 3. This rule is actually relaxed a little in the Insert implementation, which allows deeper digits to have 4 elements, since for Insert it really doesn't matter.

