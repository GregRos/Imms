# C# Documentation

## Primary Collections
`Funq` offers the following collections:

1. Sequential: `FunqList`, `FunqVector`
2. Set-like: `FunqSet`, `FunqOrderedSet`
3. Map-like: `FunqMap`, `FuneOrderedMap`

### Creating a collection

Sequential collections are easy to create. They are singletons.

	var list = FunqList<int>.Empty
	var list = FunqList.Empty<int>();
	
For equality maps and sets you can *optionally* supply an `IEqualityComparer<T>`. Otherwise, the default one is used.

	var myEquality = ...//my custom equality handler
	var set = FunqSet<int>.Empty(); //default equality
	var set = FunqSet.Empty();
	var set = FunqSet.Empty(myEquality); //custom equality
	
For comparison-based maps, the type has to be `IComparable<T>` or else you have to supply a custom comparison handler, in the same way:
	
	var set = FunqOrderedSet<int>.Empty() //type is IComparable.
	var set = FunqOrderedSet<MyCustomType>.Empty(customComparer) //not IComparable
	
#### Important Note
Optimized `Intersection/Union/...` operations are only used if both types use the same comparison handler, as determined by using `.Equals(object)`. Typically, this is `this.Comparison.Equals(other.Comparison)`. If the comparers are found to be different, a generic non-optimized algorithm will be used.

If you want to enjoy both hard performance guarantees and use a custom comparison handler, make sure you do one (or more) of the following:

1. Override the `Equals(object)` method on your handler for functional equality.
2. Use the same handler instance to construct all you collections.

Option (2) is reinforced by another way to construct collections requiring comparison handlers: by invoking extension methods on the handlers themselves, treating them as collection factories. This reinforced the idea that collections descended from the same comparer can use the optimized operations.

	var set = myCustomEquality.CreateSet();
	var set = myCustomComparer.CreateSet();
	
### General Operations
The collections support a wide variety of operations commonly implemented by LINQ. Examples include `Aggregate`, `Any`, `Where`, and so forth. These methods are implemented using direct iteration over the underlying collection, which is typically at least one order of magnitude faster