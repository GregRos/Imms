using System;
using System.Collections.Generic;

namespace Imms.Abstract {
	/// <summary>
	///     Container static class for various methods for creating comparers and equality comparers.
	/// </summary>
	internal static class Comparers {

		/// <summary>
		///     Creates a comparer from the specified comparison function.
		/// </summary>
		/// <typeparam name="T">The type for which the comparer is created.</typeparam>
		/// <param name="comparison">The comparison function.</param>
		public static IComparer<T> CreateComparer<T>(Comparison<T> comparison) {
			return new LambdaComparer<T>(comparison);
		}

		/// <summary>
		///     Creates an equality comparer from the specified equality and hash functions.
		/// </summary>
		/// <typeparam name="T">The type for which the equality comparer is created.</typeparam>
		/// <param name="equals">The function that determines equality.</param>
		/// <param name="hash">The hash function.</param>
		/// <returns></returns>
		public static IEqualityComparer<T> CreateEqComparer<T>(Func<T, T, bool> equals, Func<T, int> hash) {
			return new LambdaEquality<T>(equals, hash);
		}

		/// <summary>
		///     Returns an IComparer implementation that compares objects by key.
		/// </summary>
		/// <typeparam name="T">The type of the comparer.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="selector">The key selector function.</param>
		/// <returns></returns>
		public static IComparer<T> KeyComparer<T, TKey>(Func<T, TKey> selector)
			where TKey : IComparable<TKey> {
			var comparer = FastComparer<TKey>.Default;
			return CreateComparer<T>((a, b) => comparer.Compare(selector(a), selector(b)));
		}

		/// <summary>
		///     Returns an IComparer implementation that compares objects by key.
		/// </summary>
		/// <typeparam name="T">The type of the comparer.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="selector">The key selector function.</param>
		/// <param name="comparer">A comparer used to compare the keys.</param>
		/// <returns></returns>
		public static IComparer<T> KeyComparer<T, TKey>(Func<T, TKey> selector, IComparer<TKey> comparer) {
			return CreateComparer<T>((a, b) => comparer.Compare(selector(a), selector(b)));
		}

		/// <summary>
		///     Returns a lexicographic sequence comparer, which compares each element according to its position, from first to
		///     last.
		/// </summary>
		/// <param name="comparer">The comparer used to compare each element of the sequence.</param>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>(IComparer<T> comparer) {
			return CreateComparer<IEnumerable<T>>((x, y) => EqualityHelper.SeqCompareLex(x, y, comparer));
		}

		/// <summary>
		///     Returns a lexicographic sequence comparer, which compares each element according to its position, from first to
		///     last.
		/// </summary>
		/// <typeparam name="T">The type of element being compared. Must be IComparable[T].</typeparam>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>()
			where T : IComparable<T> {
			return CreateComparer<IEnumerable<T>>((x, y) => EqualityHelper.SeqCompareLex(x, y));
		}

		/// <summary>
		///     Returns an equality comparer for sequences, determining equality sequentially.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq">Optionally, an equality comparer for the elements. Otherwise, the default equality comparer is used.</param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<T>> SequenceEquality<T>(IEqualityComparer<T> eq = null) {
			return CreateEqComparer<IEnumerable<T>>((x, y) => EqualityHelper.SeqEquals(x, y, eq),
				x => EqualityHelper.SeqHashCode(x, eq));
		}
	}
}