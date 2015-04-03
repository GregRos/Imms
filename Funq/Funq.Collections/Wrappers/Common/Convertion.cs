using System.Collections.Generic;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	/// <summary>
	///   Provides extension methods for converting between, to, and from Funq data structures.
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


		public static FunqSet<T> Wrap<T>(this HashedAvlTree<T, bool>.Node inner, IEqualityComparer<T> eq )
		{
			return new FunqSet<T>(inner, eq);
		}

		public static FunqOrderedSet<T> Wrap<T>(this OrderedAvlTree<T, bool>.Node inner, IComparer<T> eq)
		{
			return new FunqOrderedSet<T>(inner, eq);
		}
	}
}