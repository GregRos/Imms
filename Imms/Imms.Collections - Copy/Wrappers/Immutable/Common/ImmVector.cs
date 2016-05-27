using System;
using System.Collections.Generic;

namespace Imms {
	/// <summary>
	/// Static class containing utility and extension methods for working with the ImmVector[T] class.
	/// </summary>
	[Obsolete("Use ImmList for now.")]
	public static class ImmVector {
		/// <summary>
		/// Returns the empty vector for the specified type.
		/// </summary>
		/// <typeparam name="T">The type of element contained in the vector.</typeparam>
		/// <returns></returns>
		public static ImmVector<T> Empty<T>() {
			return ImmVector<T>.Empty;
		}

		/// <summary>
		///     Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T"> The type of value. </typeparam>
		/// <param name="items"> The items used as source. </param>
		/// <returns> </returns>
		public static ImmVector<T> ToImmVector<T>(this IEnumerable<T> items) {
			return ImmVector<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		/// Creates a Vector from a number of elements.
		/// </summary>
		/// <typeparam name="T">The type of element contained in the vector.</typeparam>
		/// <param name="items">The elements to add.</param>
		/// <returns></returns>
		public static ImmVector<T> FromItems<T>(params T[] items) {
			return items.ToImmVector();
		}
	}
}