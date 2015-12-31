using System.Collections.Generic;

namespace Funq {
	/// <summary>
	/// Utility and extension methods for working with the FunqList collection.
	/// </summary>
	public static class FunqList {
		/// <summary>
		/// Returns the empty FunqList for the specified element type.
		/// </summary>
		/// <typeparam name="T">The type of element to return.</typeparam>
		/// <returns></returns>
		public static FunqList<T> Empty<T>() {
			return FunqList<T>.Empty;
		}

		/// <summary>
		///     Creates a FunqList from a sequence of items.
		/// </summary>
		/// <typeparam name="T"> The type of the value. </typeparam>
		/// <param name="items"> The elements from which to create the FunqList. </param>
		/// <returns> </returns>
		public static FunqList<T> ToFunqList<T>(this IEnumerable<T> items) {
			return FunqList<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		/// Creates a FunqList from a number of items.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="items">The elements from which to create the FunqList.</param>
		/// <returns></returns>
		public static FunqList<T> FromItems<T>(params T[] items) {
			return items.ToFunqList();
		}
	}
}