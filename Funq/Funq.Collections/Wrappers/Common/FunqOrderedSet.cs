using System;
using System.Collections.Generic;

namespace Funq.Collections {
	public static class FunqOrderedSet {
		public static FunqOrderedSet<T> Empty<T>()
			where T : IComparable<T>
		{
			return FunqOrderedSet<T>.Empty(null);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items, IComparer<T> cmp)
		{
			return FunqOrderedSet<T>.Empty(cmp).Union(items);
		}

		public static FunqOrderedSet<T> ToFunqOrderedSet<T>(this IEnumerable<T> items)
			where T : IComparable<T>
		{
			return FunqOrderedSet<T>.Empty(null).Union(items);
		}

		public static FunqOrderedSet<T> CreateOrderedSet<T>(this IComparer<T> comparer) {
			return FunqOrderedSet<T>.Empty(comparer);
		}
	}
}