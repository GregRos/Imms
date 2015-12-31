using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Imm.Abstract {
	/// <summary>
	///     Equatable comparers. I'm going to save this for a future version.
	/// </summary>
	static class EquatableComparers {
		/// <summary>
		///     Compiles the expression and memoizes the result, so that future calls of this method don't need to recompile it.
		/// </summary>
		/// <typeparam name="TDelegate">The type of the delegate.</typeparam>
		/// <param name="expr">The expression.</param>
		/// <returns></returns>
		public static TDelegate MemoizeCompile<TDelegate>(Expression<TDelegate> expr) where TDelegate : class
		{
			TDelegate result;
			var success = Cache<TDelegate>.Table.TryGetValue(expr, out result);
			if (success) return result;
			var compiled = expr.Compile();
			Cache<TDelegate>.Table[expr] = compiled;
			return compiled;
		}


		static class Cache<TDelegate>
			where TDelegate : class
		{
			internal static readonly ConcurrentDictionary<Expression<TDelegate>, TDelegate> Table =
				new ConcurrentDictionary<Expression<TDelegate>, TDelegate>(ExpressionEquality.CachingInstance);
		}
		/// <summary>
		///     Determines whether the two lambda expressions are functionally/structurally equal.
		/// </summary>
		/// <typeparam name="TDelegate">The type of the delegate.</typeparam>
		/// <returns></returns>
		public static bool FunctionalEquals<TDelegate>(this Expression<TDelegate> self, Expression<TDelegate> other) {
			return ExpressionEquality.CachingInstance.Equals(self, other);
		}

		/// <summary>
		///     Returns the structural hash code representation of the specified lambda expression.
		/// </summary>
		/// <returns></returns>
		public static int FunctionalHashCode<TDelegate>(this Expression<TDelegate> self) {
			return ExpressionEquality.CachingInstance.GetHashCode(self);
		}

		
		public static IEquatableComparer<T> LambdaComparer<T>(Expression<Func<T, T, int>> comparer) {
			return new ExprLambdaComparer<T>(comparer);
		}

		/// <summary>
		///     Returns a comparison handler that determines the order between objects using a key.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
		/// <param name="keyComparer"> </param>
		/// <returns> </returns>
		public static IEquatableComparer<T> KeyComparer<T, TKey>(Expression<Func<T, TKey>> selector,
			IComparer<TKey> keyComparer = null) {
			keyComparer = keyComparer ?? FastComparer<TKey>.Default;
			return new KeyComparer<T, TKey>(selector, keyComparer);
		}

		public static IEquatableEquality<T> KeyEquality<T, TKey>(Expression<Func<T, TKey>> selector,
			IEqualityComparer<TKey> keyEquality = null) {
			keyEquality = keyEquality ?? FastEquality<TKey>.Default;
			return new KeyEquality<T, TKey>(selector, keyEquality);
		}

		/// <summary>
		///     Creates an equality comparer using the specified equality and hash code functions.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isEqual"></param>
		/// <param name="getHashCode"></param>
		/// <returns></returns>
		public static IEquatableEquality<T> CreateEquality<T>(Expression<Func<T, T, bool>> isEqual,
			Expression<Func<T, int>> getHashCode) {
			return new ExprLambdaEquality<T>(isEqual, getHashCode);
		}
	}
}