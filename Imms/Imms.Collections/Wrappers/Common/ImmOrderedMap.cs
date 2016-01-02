using System;
using System.Collections.Generic;

namespace Imms {
	/// <summary>
	/// Utility methods for ordered map.
	/// </summary>
	public static class ImmOrderedMap {
		/// <summary>
		/// Converts a sequence of key-value pairs to an ordered map. The keys must be IComparable.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="kvps"></param>
		/// <returns></returns>
		public static ImmOrderedMap<TKey, TValue> ToImmOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
			where TKey : IComparable<TKey> {
			return ImmOrderedMap<TKey, TValue>.Empty(null).AddRange(kvps);
		}

		/// <summary>
		/// Converts a sequence of key-value pairs to an ordered map, with the specified comparison semantics.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="kvps"></param>
		/// <param name="cmp"></param>
		/// <returns></returns>
		public static ImmOrderedMap<TKey, TValue> ToImmOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IComparer<TKey> cmp) {
			return ImmOrderedMap<TKey, TValue>.Empty(cmp).AddRange(kvps);
		}

		/// <summary>
		/// Returns a new empty ordered map using the specified comparer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static ImmOrderedMap<TKey, TValue> CreateOrderedMap<TKey, TValue>(this IComparer<TKey> comparer) {
			return ImmOrderedMap<TKey, TValue>.Empty(comparer);
		}

		/// <summary>
		/// Returns an empty ordered map for the specified types using default comparison semantics.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static ImmOrderedMap<TKey, TValue> Empty<TKey, TValue>()
			where TKey : IComparable<TKey> {
			return ImmOrderedMap<TKey, TValue>.Empty(null);
		}

		/// <summary>
		/// Returns an empty ordered map for the specified types using the specified comparison handler.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="cmp"></param>
		/// <returns></returns>
		public static ImmOrderedMap<TKey, TValue> Empty<TKey, TValue>(IComparer<TKey> cmp) {
			return ImmOrderedMap<TKey, TValue>.Empty(cmp);
		} 

	}
}