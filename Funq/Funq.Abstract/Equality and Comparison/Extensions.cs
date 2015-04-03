using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	public static class Comparisons
	{
		/// <summary>
		/// Determines set equality between sequences.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other, IEqualityComparer<T> eq = null)
		{
			return EqualityHelper.Set_Equals(self, other, eq);
		}

		/// <summary>
		/// Determines equality between sequences of key-value pairs.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="vEq"></param>
		/// <returns></returns>
		public static bool MapEquals<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self,
			IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TValue> vEq = null)
		{
			return EqualityHelper.Map_Equals(self, other, vEq);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using lex comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other, IComparer<T> comparer)
		{
			return EqualityHelper.Seq_CompareLex(self, other, comparer);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using lex comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other)
			where T : IComparable<T>
		{
			return EqualityHelper.Seq_CompareLex(self, other);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using number-like comparison (length first, then lexicographically)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int NumCompare<T>(this IEnumerable<T> self, IEnumerable<T> other, IComparer<T> comparer)
		{
			return EqualityHelper.Seq_CompareLex(self, other, comparer);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using number-like comparison (length first, then lexicographically)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static int NumCompare<T>(this IEnumerable<T> self, IEnumerable<T> other)
			where T : IComparable<T>
		{
			return EqualityHelper.Seq_CompareLex(self, other);
		}

		public static int CompuateSeqHashCode<T>(this IEnumerable<T> self, IEqualityComparer<T> eq = null)
		{
			return EqualityHelper.Seq_HashCode(self, eq);
		}

		public static int ComputeSetHashCode<T>(this IEnumerable<T> self, IEqualityComparer<T> eq = null)
		{
			return EqualityHelper.Set_HashCode(self, eq);
		}

		public static int ComputeMapHashCode<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self, IEqualityComparer<TKey> keyEq,
			IEqualityComparer<TValue> valueEq = null)
		{
			if (keyEq == null)
			{
				throw Errors.Eq_comparer_required("keyEq");
			}
			return EqualityHelper.Map_HashCode(self, keyEq, valueEq);
		}
	}
}
