using System.Collections.Generic;
using Imms.Implementation;

namespace Imms {
	/// <summary>
	///     Provides extension methods for converting between, to, and from Imm data structures.
	/// </summary>
	static class Convertion {
		internal static ImmList<T> Wrap<T>(this FingerTree<T>.FTree<Leaf<T>> tree) {
			return new ImmList<T>(tree);
		}

		internal static ImmMap<TKey, TValue> WrapMap<TKey, TValue>(this HashedAvlTree<TKey, TValue>.Node root,
			IEqualityComparer<TKey> equality) {
			return new ImmMap<TKey, TValue>(root, equality);
		}

		internal static ImmOrderedMap<TKey, TValue> WrapMap<TKey, TValue>(this OrderedAvlTree<TKey, TValue>.Node root,
			IComparer<TKey> comparer) {
			return new ImmOrderedMap<TKey, TValue>(root, comparer);
		}

		public static ImmSet<T> Wrap<T>(this HashedAvlTree<T, bool>.Node inner, IEqualityComparer<T> eq) {
			return new ImmSet<T>(inner, eq);
		}

		public static ImmOrderedSet<T> Wrap<T>(this OrderedAvlTree<T, bool>.Node inner, IComparer<T> eq) {
			return new ImmOrderedSet<T>(inner, eq);
		}

		public static ImmVector<T> Wrap<T>(this TrieVector<T>.Node inner) {
			return new ImmVector<T>(inner);
		}
	}
}