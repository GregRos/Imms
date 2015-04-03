using System;
using System.Collections;
using System.Collections.Generic;

namespace Funq.Abstract
{
	public static class Comparers
	{
		public static IComparer<T> CreateComparison<T>(Func<T, T, int> comparer) {
			return Comparer<T>.Create(comparer.Invoke);
		} 

		public static IEqualityComparer<T> CreateEquality<T>(Func<T, T, bool> equals, Func<T, int> hash) {
			return new LambdaEquality<T>(equals, hash);
		}

		public static IComparer<T> KeyComparer<T, TKey>(Func<T, TKey> selector)
		where TKey : IComparable<TKey>{
			var comparer = FastComparer<TKey>.Default;
			return CreateComparison<T>((a, b) => comparer.Compare(selector(a), selector(b)));
		}

		public static IComparer<T> KeyComparer<T, TKey>(Func<T, TKey> selector, IComparer<TKey> comparer) {
			return CreateComparison<T>((a, b) => comparer.Compare(selector(a), selector(b)));
		}

		/// <summary>
		/// Returns a lexicographic sequence comparer, comparing sequences like strings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>(IComparer<T> comparer) {
			return Comparer<IEnumerable<T>>.Create((x, y) => EqualityHelper.Seq_CompareLex(x, y, comparer));
		}

		/// <summary>
		/// Returns a lexicographic sequence comparer, comparing sequences like strings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> LexSequenceComparer<T>()
			where T : IComparable<T>
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => EqualityHelper.Seq_CompareLex(x, y));
		}

		/// <summary>
		/// Returns a sequence comparer that compares sequences like you compare numbers -- length first, and then lexicographically.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> NumSequenceComparer<T>(IComparer<T> comparer)
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => EqualityHelper.Seq_CompareNum(x, y, comparer));
		}

		/// <summary>
		/// Returns a numeric sequence comparer. It doesn't compare numbers, it just compares sequences like you compare numbers.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IComparer<IEnumerable<T>> NumSequenceComparer<T>()
			where T : IComparable<T>
		{
			return Comparer<IEnumerable<T>>.Create((x, y) => EqualityHelper.Seq_CompareNum(x, y));
		} 

		/// <summary>
		/// Returns an equality comparer that compares sequences based on the order of their elements, like in a List or array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<T>> SequenceEquality<T>(IEqualityComparer<T> eq = null) {
			return CreateEquality<IEnumerable<T>>((x, y) => EqualityHelper.Seq_Equals(x, y, eq), x => EqualityHelper.Seq_HashCode(x, eq));
		}

		/// <summary>
		/// Returns an equality comparer that compares sequences like sets are compared.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static IEqualityComparer<IEnumerable<T>> SetEquality<T>(IEqualityComparer<T> eq = null)
		{
			return CreateEquality<IEnumerable<T>>((x, y) => EqualityHelper.Set_Equals(x, y, eq), x => EqualityHelper.Set_HashCode(x, eq));
		}
	}
}