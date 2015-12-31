using System;
using System.Collections.Generic;

namespace Imms {
	public static class ImmOrderedMap {
		public static ImmOrderedMap<TKey, TValue> ToImmOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
			where TKey : IComparable<TKey> {
			return ImmOrderedMap<TKey, TValue>.Empty(null).AddRange(kvps);
		}

		public static ImmOrderedMap<TKey, TValue> ToImmOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IComparer<TKey> cmp) {
			return ImmOrderedMap<TKey, TValue>.Empty(cmp).AddRange(kvps);
		}

		public static ImmOrderedMap<TKey, TValue> CreateOrderedMap<TKey, TValue>(this IComparer<TKey> comparer) {
			return ImmOrderedMap<TKey, TValue>.Empty(comparer);
		}

		public static ImmOrderedMap<TKey, TValue> Empty<TKey, TValue>()
			where TKey : IComparable<TKey> {
			return ImmOrderedMap<TKey, TValue>.Empty(null);
		}

		public static ImmOrderedMap<TKey, TValue> Empty<TKey, TValue>(IComparer<TKey> cmp) {
			return ImmOrderedMap<TKey, TValue>.Empty(cmp);
		} 

	}
}