using System.Collections.Generic;

namespace Funq {
	/// <summary>
	/// Static class containing utility and extension methods for working with the FunqVector[T] class.
	/// </summary>
	public static class FunqVector {
		/// <summary>
		/// Returns the empty vector for the specified type.
		/// </summary>
		/// <typeparam name="T">The type of element contained in the vector.</typeparam>
		/// <returns></returns>
		public static FunqVector<T> Empty<T>() {
			return FunqVector<T>.Empty;
		}

		/// <summary>
		///     Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T"> The type of value. </typeparam>
		/// <param name="items"> The items used as source. </param>
		/// <returns> </returns>
		public static FunqVector<T> ToFunqVector<T>(this IEnumerable<T> items) {
			return FunqVector<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		/// Creates a Vector from a number of elements.
		/// </summary>
		/// <typeparam name="T">The type of element contained in the vector.</typeparam>
		/// <param name="items">The elements to add.</param>
		/// <returns></returns>
		public static FunqVector<T> FromItems<T>(params T[] items) {
			return items.ToFunqVector();
		}
	}
}