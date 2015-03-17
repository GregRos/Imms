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

	public static class FunqSet
	{

		public static FunqSet<T> ToFunqSet<T>(this IEnumerable<T> items, IEqualityComparer<T> eq = null)
		{
			return FunqSet.Simple(eq).AddMany(items);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items, IComparer<T> cmp)
		{
			return FunqSet.Ordered(cmp).AddMany(items);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items)
			where T : IComparable<T>
		{
			return FunqSet.Ordered<T>().AddMany(items);
		}

		public static FunqSet<T> Simple<T>(IEqualityComparer<T> equality = null)
		{
			equality = equality ?? EqualityComparer<T>.Default;
			return FunqSet<T>.Empty(equality);
		}

		public static FunqOrderedSet<T> Ordered<T>() 
			where T : IComparable<T>
		{
			return Ordered(Comparer<T>.Default);
		}

		public static FunqOrderedSet<T> Ordered<T>(IComparer<T> comparer)
		{
			return FunqOrderedSet<T>.Empty(comparer);
		}
	}

	public static class FunqMap
	{
		public static FunqMap<TKey, TValue> Simple<TKey, TValue>(IEqualityComparer<TKey> equality = null)
		{
			equality = equality ?? EqualityComparer<TKey>.Default;
			return FunqMap<TKey, TValue>.Empty(equality);
		}

		public static FunqMap<TKey, TValue> ToFunqMap<TKey, TValue>(this IEnumerable<Kvp<TKey, TValue>> kvps, IEqualityComparer<TKey> eq = null)
		{
			return FunqMap.Simple<TKey, TValue>().AddMany(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(this IEnumerable<Kvp<TKey, TValue>> kvps)
			where TKey : IComparable<TKey>
		{
			return FunqMap.Ordered<TKey, TValue>().AddMany(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(this IEnumerable<Kvp<TKey, TValue>> kvps, IComparer<TKey> cmp)
		{
			return FunqMap.Ordered<TKey, TValue>(cmp).AddMany(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> Ordered<TKey, TValue>()
			where TKey : IComparable<TKey>
		{
			return Ordered<TKey, TValue>(Comparer<TKey>.Default);
		} 

		public static FunqOrderedMap<TKey, TValue> Ordered<TKey, TValue>(IComparer<TKey> comparer)
		{
			return FunqOrderedMap<TKey, TValue>.Empty(comparer);
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

		internal static FunqSet<T> WrapSet<T>(this HashedAvlTree<T, bool>.Node root, IEqualityComparer<T> equality)
		{
			return new Funq.Collections.FunqSet<T>(root, equality);
		}

		internal static FunqOrderedSet<T> WrapSet<T>(this OrderedAvlTree<T, bool>.Node root, IComparer<T> comparer)
		{
			return new Funq.Collections.FunqOrderedSet<T>(root, comparer);
		}

		internal static FunqOrderedMap<TKey, TValue> WrapMap<TKey, TValue>(this OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer )
		{
			return new FunqOrderedMap<TKey, TValue>(root, comparer);
		}

	}
}