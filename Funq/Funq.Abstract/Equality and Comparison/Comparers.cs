using System;
using System.Collections;
using System.Collections.Generic;

namespace Funq.Abstract
{
	public static class Comparers
	{
		/// <summary>
		///   Returns a comparison handler that determines the order between objects using a key.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
		/// <param name="keyComparer"> </param>
		/// <returns> </returns>
		public static IComparer<T> KeyComparer<T, TKey>(Func<T, TKey> selector, IComparer<TKey> keyComparer = null)
		{
			keyComparer = keyComparer ?? FastComparer<TKey>.Default;
			return Comparer<T>.Create((x, y) => keyComparer.Compare(selector(x), selector(y)));
		}

		/// <summary>
		/// Creates an equality comparer using the specified equality and hash code functions.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isEqual"></param>
		/// <param name="getHashCode"></param>
		/// <returns></returns>
		public static IEqualityComparer<T> CreateEquality<T>(Func<T, T, bool> isEqual, Func<T, int> getHashCode) {
			return new LambdaEquality<T>(isEqual, getHashCode);
		} 

		/// <summary>
		/// Returns a lexicographic sequence comparer, comparing sequences like strings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>(IComparer<T> comparer) {
			return Comparer<IEnumerable<T>>.Create((x, y) => Equality.Seq_CompareLex(x, y, comparer));
		}

		/// <summary>
		/// Returns a lexicographic sequence comparer, comparing sequences like strings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>()
			where T : IComparable<T>
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => Equality.Seq_CompareLex(x, y));
		}

		/// <summary>
		/// Returns a numeric sequence comparer. It doesn't compare numbers, it just compares sequences like you compare numbers.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> NumSequenceComparer<T>(IComparer<T> comparer)
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => Equality.Seq_CompareNum(x, y, comparer));
		}

		/// <summary>
		/// Returns a numeric sequence comparer. It doesn't compare numbers, it just compares sequences like you compare numbers.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> NumSequenceComparer<T>()
			where T : IComparable<T>
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => Equality.Seq_CompareNum(x, y));
		}

		/// <summary>
		/// Returns an equality comparer that compares sequences based on the order of their elements, like in a List or array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<T>> OrderEquality<T>(IEqualityComparer<T> eq = null) {
			return CreateEquality<IEnumerable<T>>((x, y) => Equality.List_Equate(x, y, eq), x => Equality.Seq_HashCode(x, eq));
		}

		/// <summary>
		/// Returns an equality comparer that compares sequences like sets are compared.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<T>> SetEquality<T>(IEqualityComparer<T> eq = null)
		{
			return CreateEquality<IEnumerable<T>>((x, y) => Equality.Set_Equate(x, y, eq), x => Equality.Set_HashCode(x, eq));
		}

		/// <summary>
		/// Returns an equality comparer that compares maps both by key and by value. 
		/// You must supply the equality comparer used for keys by hand. 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="keyEq"></param>
		/// <param name="valueEq"></param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<KeyValuePair<TKey, TValue>>> MapEquality<TKey, TValue>(IEqualityComparer<TKey> keyEq, IEqualityComparer<TValue> valueEq = null)
		{
			if (keyEq == null) {
				throw Errors.Eq_comparer_required("keyEq");
			}
			return CreateEquality<IEnumerable<KeyValuePair<TKey, TValue>>>((x, y) => Equality.Map_Equate(x, y, valueEq), x => Equality.Map_HashCode(x, keyEq, valueEq));
		}

		/// <summary>
		/// Determines set equality between sequences.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other, IEqualityComparer<T> eq = null ) {
			return Equality.Set_Equate(self, other, eq);
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
			IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TValue> vEq = null) {
			return Equality.Map_Equate(self, other, vEq);
		}



		/// <summary>
		/// Determines whether this sequence is bigger than the other using lex comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other, IComparer<T> comparer) {
			return Equality.Seq_CompareLex(self, other, comparer);
		}

		/// <summary>
		/// Determines whether this sequence is bigger than the other using lex comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int LexCompare<T>(this IEnumerable<T> self, IEnumerable<T> other)
			where T : IComparable<T>
		{
			return Equality.Seq_CompareLex(self, other);
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
			return Equality.Seq_CompareLex(self, other, comparer);
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
			return Equality.Seq_CompareLex(self, other);
		}

		public static int CompuateSeqHashCode<T>(this IEnumerable<T> self, IEqualityComparer<T> eq = null)
		{
			return Equality.Seq_HashCode(self, eq);
		}

		public static int ComputeSetHashCode<T>(this IEnumerable<T> self, IEqualityComparer<T> eq = null) {
			return Equality.Set_HashCode(self, eq);
		}

		public static int ComputeMapHashCode<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self, IEqualityComparer<TKey> keyEq,
			IEqualityComparer<TValue> valueEq = null) {
			if (keyEq == null) {
				throw Errors.Eq_comparer_required("keyEq");
			}
			return Equality.Map_HashCode(self, keyEq, valueEq);
		}
	}
}