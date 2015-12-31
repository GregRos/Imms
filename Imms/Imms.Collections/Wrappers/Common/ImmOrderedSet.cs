using System;
using System.Collections.Generic;

namespace Imms {
	public static class ImmOrderedSet {
		public static ImmOrderedSet<T> Empty<T>()
			where T : IComparable<T> {
			return ImmOrderedSet<T>.Empty(null);
		}

		public static ImmOrderedSet<T> Empty<T>(IComparer<T> cmp) {
			return ImmOrderedSet<T>.Empty(cmp);
		} 

		public static ImmOrderedSet<T> ToImmOrderedSet<T>(this IEnumerable<T> items, IComparer<T> cmp) {
			return ImmOrderedSet<T>.Empty(cmp).Union(items);
		}

		public static ImmOrderedSet<T> ToImmOrderedSet<T>(this IEnumerable<T> items)
			where T : IComparable<T> {
			return ImmOrderedSet<T>.Empty(null).Union(items);
		}

		public static ImmOrderedSet<T> CreateOrderedSet<T>(this IComparer<T> comparer) {
			return ImmOrderedSet<T>.Empty(comparer);
		}
	}
}