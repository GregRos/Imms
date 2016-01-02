using System.Collections.Generic;

namespace Imms {
	/// <summary>
	/// Utility class for working with <see cref="ImmSet{T}"/>.
	/// </summary>
	public static class ImmSet {
		/// <summary>
		/// Returns an empty <see cref="ImmSet{T}"/> that uses default equality semantics.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static ImmSet<T> Empty<T>() {
			return ImmSet<T>.Empty();
		}

		/// <summary>
		/// Returns an empty <see cref="ImmSet{T}"/> that uses this eq comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static ImmSet<T> CreateSet<T>(this IEqualityComparer<T> eq) {
			return ImmSet<T>.Empty(eq);
		}

		/// <summary>
		/// Converts the sequence to an <see cref="ImmSet{T}"/>. Optionally, you may provide an eq comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static ImmSet<T> ToImmSet<T>(this IEnumerable<T> items, IEqualityComparer<T> eq = null) {
			return ImmSet<T>.Empty(eq).Union(items);
		} 

	}
}