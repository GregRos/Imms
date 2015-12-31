using System;
using System.Collections.Generic;

namespace Funq.Abstract {
	/// <summary>
	///     Contains utility and extension methods for comparisons between different objects, mainly sequences or collections.
	/// </summary>
	internal static class Comparisons {

		/// <summary>
		/// Determines set equality between sequences. That is, they must contain the same elements, but the multiplicity of
		/// each element doesn't matter.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="eq">Optionally, the equality comparer used for the values. If not specified, the default equality comparer is used.</param>
		/// <returns></returns>
		public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other, IEqualityComparer<T> eq = null) {
			return EqualityHelper.SetEquals(self, other, eq);
		}

		/// <summary>
		/// Determines equality between sequences of key-value pairs. That is, they must have the same key-value pairs, but the
		/// multiplicity of each key-value pair doesn't matter.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="self">The first sequence of key-value pairs.</param>
		/// <param name="other">The second sequence of key-value pairs.</param>
		/// <param name="vEq">The equality comparer used to compare the values..</param>
		/// <returns></returns>
		public static bool MapEquals<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self,
			IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TValue> vEq = null) {
			return EqualityHelper.MapEquals(self, other, vEq);
		}

		/// <summary>
		///     Determines whether this sequence is bigger than the other using a lexicographic comparison (comparing each element
		///     based on its position, from first to last).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other, IComparer<T> comparer) {
			return EqualityHelper.SeqCompareLex(self, other, comparer);
		}

		/// <summary>
		///     Determines whether this sequence is bigger than the other using lexicographic comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other)
			where T : IComparable<T> {
			return EqualityHelper.SeqCompareLex(self, other);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using number-like comparison (length first, then
		/// lexicographically).
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="first">The first sequence, which is compared to the second sequence</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns></returns>
		public static int NumCompare<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer)
		{
			return EqualityHelper.SeqCompareNum(first, second, comparer);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using number-like comparison (length first, then
		/// lexicographically).
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="first">The first sequence, which is compared to the second sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <returns></returns>
		public static int NumCompare<T>(this IEnumerable<T> first, IEnumerable<T> second)
			where T : IComparable<T>
		{
			return EqualityHelper.SeqCompareNum(first, second);
		}

		/// <summary>
		/// Computes a hash code for a sequence.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="self">The sequence for which to compute the hash code.</param>
		/// <param name="eq">The equality comparer used to generate the hash of each element. If not specified, default equality comparer is used.</param>
		/// <returns></returns>
		public static int CompuateSeqHashCode<T>(this IEnumerable<T> self, IEqualityComparer<T> eq = null) {
			return EqualityHelper.SeqHashCode(self, eq);
		}
	}
}