using System;
using System.Collections.Generic;

namespace Imms {
	/// <summary>
	/// Utility methods for working with <see cref="ImmOrderedSet{T}"/>.
	/// </summary>
	public static class ImmOrderedSet {
		/// <summary>
		/// Returns an empty <see cref="ImmOrderedSet{T}"/> using the default comparison semantics of the type.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <returns></returns>
		public static ImmOrderedSet<T> Empty<T>()
			where T : IComparable<T> {
			return ImmOrderedSet<T>.Empty(null);
		}

		/// <summary>
		/// Returns an empty instance of <see cref="ImmOrderedSet{T}"/> using the specified comparer.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="cmp">The comparer.</param>
		/// <returns></returns>
		public static ImmOrderedSet<T> Empty<T>(IComparer<T> cmp) {
			return ImmOrderedSet<T>.Empty(cmp);
		}

		/// <summary>
		/// Converts a sequence of elements to an <see cref="ImmOrderedSet{T}"/> using the specified comparer.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="items">The sequence to be converted.</param>
		/// <param name="cmp">The comparer.</param>
		/// <returns></returns>
		public static ImmOrderedSet<T> ToImmOrderedSet<T>(this IEnumerable<T> items, IComparer<T> cmp) {
			return ImmOrderedSet<T>.Empty(cmp).Union(items);
		}

		/// <summary>
		/// Converts a sequence of elements to an <see cref="ImmOrderedSet{T}"/> using the specified comparer.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="items">The sequence to be converted.</param>
		/// <returns></returns>
		public static ImmOrderedSet<T> ToImmOrderedSet<T>(this IEnumerable<T> items)
			where T : IComparable<T> {
			return ImmOrderedSet<T>.Empty(null).Union(items);
		}

		/// <summary>
		/// Returns an empty <see cref="ImmOrderedSet{T}"/> using the specified comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static ImmOrderedSet<T> CreateOrderedSet<T>(this IComparer<T> comparer) {
			return ImmOrderedSet<T>.Empty(comparer);
		}
	}
}