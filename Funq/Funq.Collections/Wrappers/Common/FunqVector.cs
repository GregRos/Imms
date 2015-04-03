using System.Collections.Generic;

namespace Funq.Collections {
	public static class FunqVector {
		public static FunqVector<T> Empty<T>() {
			return FunqVector<T>.Empty;
		}

		/// <summary>
		///   Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T"> The type of value. </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		public static FunqVector<T> ToFunqVector<T>(this IEnumerable<T> items)
		{
			return FunqVector<T>.Empty.AddLastRange(items);
		}

		public static FunqVector<T> FromItems<T>(params T[] xs) {
			return xs.ToFunqVector();
		}
	}
}