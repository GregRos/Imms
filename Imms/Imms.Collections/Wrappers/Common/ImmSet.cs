using System.Collections.Generic;

namespace Imms {
	public static class ImmSet {
		public static ImmSet<T> Empty<T>() {
			return ImmSet<T>.Empty();
		}

		public static ImmSet<T> CreateSet<T>(this IEqualityComparer<T> eq) {
			return ImmSet<T>.Empty(eq);
		}

		public static ImmSet<T> ToImmSet<T>(this IEnumerable<T> items, IEqualityComparer<T> eq = null) {
			return ImmSet<T>.Empty(eq).Union(items);
		}

	}
}