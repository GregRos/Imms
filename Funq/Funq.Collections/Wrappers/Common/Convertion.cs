using System;
using System.Collections.Generic;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public static class FunqList {
		public static FunqList<T> Empty<T>() {
			return FunqList<T>.Empty;
		}

		/// <summary>
		///   Creates a FlexibleList from a sequence of items.
		/// </summary>
		/// <typeparam name="T"> The type of the value. </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		public static FunqList<T> ToFunqList<T>(this IEnumerable<T> items)
		{
			return FunqList<T>.Empty.AddLastRange(items);
		}
	}

	public static class FunqVector {
		public static FunqVector<T> Empty<T>() {
			return FunqVector<T>.Empty;
		}

		/// <summary>
		///   Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T"> The type of value. </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		public static FunqVector<T> ToFunqVector<T>(this IEnumerable<T> items)
		{
			return FunqVector<T>.Empty.AddLastRange(items);
		}
	}

	public static class FunqSet {
		public static FunqSet<T> Empty<T>(IEqualityComparer<T> eq = null) {
			return FunqSet<T>.Empty(eq);
		}

		public static FunqSet<T> ToFunqSet<T>(this IEnumerable<T> items, IEqualityComparer<T> eq = null) {
			return FunqSet<T>.Empty(eq).AddRange(items);
		}

	}

	public static class FunqOrderedSet {

		public static FunqOrderedSet<T> Empty<T>(IComparer<T> comparer) {
			return FunqOrderedSet<T>.Empty(comparer);
		}

		public static FunqOrderedSet<T> Empty<T>()
			where T : IComparable<T>
		{
			return FunqOrderedSet<T>.Empty(null);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items, IComparer<T> cmp)
		{
			return FunqOrderedSet<T>.Empty(cmp).AddRange(items);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items)
			where T : IComparable<T>
		{
			return FunqOrderedSet<T>.Empty(null).AddRange(items);
		}
	}
	

	public static class FunqMap
	{
		public static FunqMap<TKey, TValue> Empty<TKey, TValue>(IEqualityComparer<TKey> eq = null) {
			return FunqMap<TKey, TValue>.Empty(eq);
		}

		public static FunqMap<TKey, TValue> ToFunqMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IEqualityComparer<TKey> eq = null)
		{
			return FunqMap<TKey, TValue>.Empty(eq).AddRange(kvps);
		}


		
	}

	public static class FunqOrderedMap {



		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
			where TKey : IComparable<TKey>
		{
			return FunqOrderedMap<TKey, TValue>.Empty(null).AddRange(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IComparer<TKey> cmp)
		{
			return FunqOrderedMap<TKey, TValue>.Empty(cmp).AddRange(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> Empty<TKey, TValue>(IComparer<TKey> comparer)
		{
			return FunqOrderedMap<TKey, TValue>.Empty(comparer);
		}

		public static FunqOrderedMap<TKey, TValue> Empty<TKey, TValue>()
			where TKey : IComparable<TKey>
		{
			return FunqOrderedMap<TKey, TValue>.Empty(null);
		}
	}

	/// <summary>
	///   Provides extension methods for converting between, to, and from Solid data structures.
	/// </summary>
	internal static class Convertion
	{
		internal static FunqList<T> Wrap<T>(this FingerTree<T>.FTree<Leaf<T>> tree)
		{
			return new FunqList<T>(tree);
		}

		internal static FunqMap<TKey, TValue> WrapMap<TKey, TValue>(this HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality)
		{
			return new FunqMap<TKey, TValue>(root, equality);
		}

		internal static FunqOrderedMap<TKey, TValue> WrapMap<TKey, TValue>(this OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer )
		{
			return new FunqOrderedMap<TKey, TValue>(root, comparer);
		}

	}
}