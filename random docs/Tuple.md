# ExtraFunctional
Library of extensions and utility functions for F#. Includes some extensions to `Seq`, `List`, `Array`, etc. Also a Tuple module for generically working with tuples of any length (currently tuples of lengths 2 to 10 are supported), which is explained here:

## Tuple Module
This library features a tuple module that provides generic operations on some tuple types. These operations are similar in character to those provided over lists and sequences. The motivation for the module is, in general, having a way to manipulate tuple objects of arbitrary arity. The module allows treating tuples as special lists of constant length, consisting of elements of specific types, with all of this being enforced during compilation time.

Here is a small example of the sorts of functionality offered by the library (type annotations for clarity only):

	let mixedType : int * string * obj * float = (1, "hello", obj(), "world")
	let intItem : int = mixedType |> Tuple.item0
	
	//gets the 2nd item (index 1):
	let stringItem : string = mixedType |> Tuple.item S1 
	
	//maps the 0th item to string:
	let mixedType : string * string * obj * string = 
		mixedType |> Tuple.mapItem S0 (sprintf "%d") 
	
	//maps the last item to string as well, using a different sort of function:
	let allString : string * string * string * string = 
		mixedType |> Tuple.mapItem3 (sprintf "%O")
		
	//With tuples where all the elements are of the same type,
	//other things are possible:
	
	let thirdItem = allString |> Tuple.get 3
	
	let allInt : int * int * int * int = 
		allString |> Tuple.mapAll (fun x -> x.GetHashCode())
		
	let sum : int = allInt |> Tuple.sum
		
	let errorProducing : int * int = (1, 2)
	//this results in a compilation error.
	let res = errorProducing |> Tuple.item S2
	
### Notes
The functions offered by the library is pretty simple in principle, but the implementation involves a lot of repetitive code (a specialized implementation must be written for tuples of every arity) and type-fu. 

Basically, using normal means, it's impossible to write a module such as the one offered here. The different tuple types don't have any inheritance relation or any other kind of relation. Another impediment is the fact that in this case*, curried overloaded methods aren't possible, so it's not possible to make the function `Tuple.mapAll` with the signature the user expects (so that it can be used with the `|>` operator or similar).

In order accomplish what it does, the library resorts to some rather convoluted type-fu. An important result of all this stuff, is that each user-visible function corresponds to a single `inline` `let` binding with generic argument and return types. There are a number of advantages, disadvantages, and general gotchas you should be aware of:

### Advantages
1. Everything is checked at compilation time, if possible. For example, it's illegal to call `Tuple.item2` on a 2-element tuple, because index `2` corresponds a 3rd element. In more general terms, it's not possible to call these functions on non-tuple types.
2. Everything has a high degree of optimization. There is no method call, type checking/casting, or object allocation overhead except in limited cases. In a sense, the compiler takes care of a lot of the work before the program runs.
3. Function return types depend on argument types in meaningful ways. For example, the return type of `Tuple.item0` is the type of the 1st element in the tuple (for tuples of any length).
4. You can use these functions to write your own utility functions, and although the will have to be declared `inline`, they will also be generic and work for any compatible tuple type.
5. You can use these functions as function values without type annotations in most cases, and you can supply function arguments without type annotations as well.

#### Disadvantages
1. The return type of the function depends on the argument types in ways that are rather obvious, but cannot be articulated as part of the signature. Generic type names are stylized to meaningfully communicate this information, so you won't usually notice this, though. Here is an example:

		mapAll : (f : 'in -> 'out) -> tuple:'('in * ... * 'in) -> '('out * ... * 'out)
	The expressions `'('in * ... * 'in)` and `'('out * ... * 'out)` are actually the names of simple type parameters meant to communicate the dependence of the return type on the function and tuple types. They are defined using the ` `` ` construct, which allows special characters in names.
	
2. Unfortunately, the naming scheme in (1) hides information from the user (the fact that these expressions are just type parameters really isn't obvious), and since the abstraction is pretty thin, the user will have to deal with this reality eventually. 
	
1. Most errors produced by the library will be overload resolution errors, and can be a little too verbose. They also refer to members that aren't meant to be user-visible, so they might be a bit confusing.
2. The signatures of most functions (as visible in visual studio) state that they require members that aren't meant to be user-visible, but the signatures of which depend on the argument types supplied by the user. Types the user supplies aren't meant to fulfill those requirements, which may be confusing.

	For example, most functions state that they require the member `IsTuple` (which sort of conveyes one of the types must be a tuple), but also functions like `Map`, `Fold`, `Item`, and so forth.
3. Each function has to be eventually implemented separately for each tuple kind (e.g. for 2-tuples, 3-tuples, and so forth). While in practice the code is automatically generated from a text template, the potential for small bugs and inconsistencies is still there.
4. Overload resolution and the code generation required for inline members can take a toll on the compiler, although I employ a number of tricks to keep the impact to a minimum.
5. Because of the previous two items, only tuples of up to a certain length are supported.
 
### Static Integers
A static integer is a type like any other that is meant (conceptually) to represent a literal integer value which is known at compile time. While in principle this concept could be extended to other types, such as booleans, currently only integers are supported.

These are useful because they allow you to have functions (as this library does) with return types that seem to depend on the numeric value of the argument, such as the function `Tuple.item k tuple` that gets the item at index `k` in `tuple`, the return type being dependent upon the value of `k` (as well as the type `tuple`). The version of the function that is used is determined by overload resolution, which discriminates based on the type of `k`.

Static integers are written `S{n}` where `{n}` refers to some number, e.g. `S0`, `S1`, `S2` (the `S` stands for *static*). Because they are individually defined types, only a limited number of these is available. They aren't directly related (in the sense of an inheritance relation), as they don't use polymorphism (which works at runtime) but overloading resolution (which is determined at compile time). They do implement the interface `IStaticInt`, which also exposes a `Value` property returning the integer value represented by that type.

Usage examples can be seen in the first example above. Here are some additional examples:

	let tuple = (1, "hi", obj())
	let item0 = tuple |> Tuple.item S0
	let item1 = tuple |> Tuple.item S1
	
	//Sets the item at index 0:
	let tuple = tuple |> Tuple.setItem S0 "Hey there"
	
	//Maps the item at index 1 (and only that item, leaving the others unchanged). 
	//Yes, type inference magic means you don't have to use type annotations, even here!
	let tuple = tuple |> Tuple.mapItem S1 (fun x -> x.ToUpper())

You can use the static integers for your own purposes, but their use is limited, because the only mechanism that can support them is overload resolution. 

Typically, functions that accept a static integer have related functions that, instead, are named differently, e.g. `Tuple.item0` and so forth. However, error messages will frequently refer to types such as `S0` and `S1` so you should be aware of what they are.

Although its usage is limited, you can get the static integer representing the length of a tuple type using the function `Tuple.staticLength`. Remember that its return type is actually dependent on the type of tuple passed to it.
	
	   