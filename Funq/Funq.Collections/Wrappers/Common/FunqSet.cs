using System.Collections.Generic;

namespace Funq {
	public static class FunqSet {
		public static FunqSet<T> Empty<T>() {
			return FunqSet<T>.Empty();
		}

		public static FunqSet<T> CreateSet<T>(this IEqualityComparer<T> eq) {
			return FunqSet<T>.Empty(eq);
		}

		public static FunqSet<T> ToFunqSet<T>(this IEnumerable<T> items, IEqualityComparer<T> eq = null) {
			return FunqSet<T>.Empty(eq).Union(items);
		}

	}
}