using System;
using System.Collections.Generic;

namespace Funq {
	public static class FunqOrderedMap {
		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
			where TKey : IComparable<TKey> {
			return FunqOrderedMap<TKey, TValue>.Empty(null).AddRange(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> ToFunqOrderedMap<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IComparer<TKey> cmp) {
			return FunqOrderedMap<TKey, TValue>.Empty(cmp).AddRange(kvps);
		}

		public static FunqOrderedMap<TKey, TValue> CreateOrderedMap<TKey, TValue>(this IComparer<TKey> comparer) {
			return FunqOrderedMap<TKey, TValue>.Empty(comparer);
		}

		public static FunqOrderedMap<TKey, TValue> Empty<TKey, TValue>()
			where TKey : IComparable<TKey> {
			return FunqOrderedMap<TKey, TValue>.Empty(null);
		}

		public static FunqOrderedMap<TKey, TValue> Empty<TKey, TValue>(IComparer<TKey> cmp) {
			return FunqOrderedMap<TKey, TValue>.Empty(cmp);
		} 

	}
}