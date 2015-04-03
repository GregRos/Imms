using System.Collections.Generic;

namespace Funq.Collections {
	public static class FunqList {
		public static FunqList<T> Empty<T>() {
			return FunqList<T>.Empty;
		}

		/// <summary>
		///   Creates a FlexibleList from a sequence of items.
		/// </summary>
		/// <typeparam name="T"> The type of the value. </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		public static FunqList<T> ToFunqList<T>(this IEnumerable<T> items)
		{
			return FunqList<T>.Empty.AddLastRange(items);
		}

		public static FunqList<T> FromItems<T>(params T[] xs)
		{
			return xs.ToFunqList();
		}
	}
}