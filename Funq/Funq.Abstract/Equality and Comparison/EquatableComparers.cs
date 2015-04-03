using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Funq.Abstract {
	/// <summary>
	/// Equatable comparers. I'm going to save this for a future version.
	/// </summary>
	internal static class EquatableComparers {
		public static IEquatableComparer<T> LambdaComparer<T>(Expression<Func<T, T, int>> comparer)
		{
			return new ExprLambdaComparer<T>(comparer);
		}

		/// <summary>
		///   Returns a comparison handler that determines the order between objects using a key.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
		/// <param name="keyComparer"> </param>
		/// <returns> </returns>
		public static IEquatableComparer<T> KeyComparer<T, TKey>(Expression<Func<T, TKey>> selector, IComparer<TKey> keyComparer = null)
		{
			keyComparer = keyComparer ?? FastComparer<TKey>.Default;
			return new KeyComparer<T, TKey>(selector, keyComparer);
		}

		public static IEquatableEquality<T> KeyEquality<T, TKey>(Expression<Func<T, TKey>> selector,
			IEqualityComparer<TKey> keyEquality = null)
		{
			keyEquality = keyEquality ?? FastEquality<TKey>.Default;
			return new KeyEquality<T, TKey>(selector, keyEquality);
		}

		/// <summary>
		/// Creates an equality comparer using the specified equality and hash code functions.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isEqual"></param>
		/// <param name="getHashCode"></param>
		/// <returns></returns>
		public static IEquatableEquality<T> CreateEquality<T>(Expression<Func<T, T, bool>> isEqual, Expression<Func<T, int>> getHashCode)
		{
			return new ExprLambdaEquality<T>(isEqual, getHashCode);
		}
	}
}