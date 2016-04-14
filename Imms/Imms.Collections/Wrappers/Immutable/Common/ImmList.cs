using System.Collections.Generic;

namespace Imms {
	/// <summary>
	/// Utility and extension methods for working with the ImmList collection.
	/// </summary>
	public static class ImmList {
		/// <summary>
		/// Returns the empty ImmList for the specified element type.
		/// </summary>
		/// <typeparam name="T">The type of element to return.</typeparam>
		/// <returns></returns>
		public static ImmList<T> Empty<T>() {
			return ImmList<T>.Empty;
		}

		/// <summary>
		///     Creates a ImmList from a sequence of items.
		/// </summary>
		/// <typeparam name="T"> The type of the value. </typeparam>
		/// <param name="items"> The elements from which to create the ImmList. </param>
		/// <returns> </returns>
		public static ImmList<T> ToImmList<T>(this IEnumerable<T> items) {
			return ImmList<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		/// Creates a ImmList from a number of items.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="items">The elements from which to create the ImmList.</param>
		/// <returns></returns>
		public static ImmList<T> Of<T>(params T[] items) {
			return items.ToImmList();
		}
	}
}