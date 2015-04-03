using System.Collections.Generic;

namespace Funq.Collections {
	public static class FunqMap
	{
		public static FunqMap<TKey, TValue> CreateMap<TKey, TValue>(this IEqualityComparer<TKey> eq) {
			return FunqMap<TKey, TValue>.Empty(eq);
		}

		public static FunqMap<TKey, TValue> Empty<TKey, TValue>() {
			return FunqMap<TKey, TValue>.Empty();
		}

		public static FunqMap<TKey, TValue> ToFunqMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps, IEqualityComparer<TKey> eq = null)
		{
			return FunqMap<TKey, TValue>.Empty(eq).AddRange(kvps);
		}
	}
}