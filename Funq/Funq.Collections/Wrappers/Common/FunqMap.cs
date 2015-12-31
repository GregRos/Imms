using System;
using System.Collections.Generic;
using System.Linq;
using Funq.Abstract;

namespace Funq {

	/// <summary>
	/// Contains utility and extension methods for working with the FunqMap[TKey, TValue] collection type.
	/// </summary>
	public static class FunqMap {
		/// <summary>
		/// Uses the specified IEqualityComparer as a factory, producing an empty FunqMap using that comparer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="eq"></param>
		/// <returns></returns>
		public static FunqMap<TKey, TValue> CreateMap<TKey, TValue>(this IEqualityComparer<TKey> eq) {
			return FunqMap<TKey, TValue>.Empty(eq);
		}

		/// <summary>
		/// Returns an empty FunqMap that uses the specified equality comparer to equate elements.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="eq">Optionally, the equality comparer used to equate keys. Otherwise, the default equality comparer is used.</param>
		/// <returns></returns>
		public static FunqMap<TKey, TValue> Empty<TKey, TValue>(IEqualityComparer<TKey> eq = null) {
			return FunqMap<TKey, TValue>.Empty(eq ?? FastEquality<TKey>.Default );
		}

		/// <summary>
		/// Converts the specified sequence of key-value pairs to a FunqMap, using the optionally specified equality comparer for determining equality between keys.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="kvps">A sequence of key-value pairs.</param>
		/// <param name="eq">An equality comparer</param>
		/// <returns></returns>
		public static FunqMap<TKey, TValue> ToFunqMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps,
			IEqualityComparer<TKey> eq = null) {
			return FunqMap<TKey, TValue>.Empty(eq).AddRange(kvps);
		}


		/// <summary>
		/// Converts the specified sequence into a FunqMap, using the key and value selectors to determine the keys and values.
		/// </summary>
		/// <typeparam name="T">The type of value contained in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of key selected.</typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="keySelector"></param>
		/// <param name="valueSelector"></param>
		/// <param name="equality"></param>
		/// <returns></returns>
		public static FunqMap<TKey, TValue> ToFunqMap<T, TKey, TValue>(this IEnumerable<T> sequence, Func<T, TKey> keySelector,
			Func<T, TValue> valueSelector, IEqualityComparer<TKey> equality ) {
			return sequence.Select(x => Kvp.Of(keySelector(x), valueSelector(x))).ToFunqMap(equality);
		}


	}
}