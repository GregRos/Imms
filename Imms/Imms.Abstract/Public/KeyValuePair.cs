using System;
using System.Collections.Generic;

namespace Imms {
	/// <summary>
	///     Utility methods for working with key-value pairs.
	/// </summary>
	public static class Kvp {

		/// <summary>
		/// Constructs a new key-value pair.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="k">The key.</param>
		/// <param name="v">The value.</param>
		/// <returns></returns>
		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(TKey k, TValue v) {
			return new KeyValuePair<TKey, TValue>(k, v);
		}

		/// <summary>
		/// Constructs a new key-value pair from a 2-tuple.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="pair">The 2-tuple.</param>
		/// <returns></returns>
		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(Tuple<TKey, TValue> pair) {
			return Of(pair.Item1, pair.Item2);
		}

		/// <summary>
		/// Turns a key-value pair into a tuple.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="pair">The key-value pair.</param>
		/// <returns></returns>
		public static Tuple<TKey, TValue> ToTuple<TKey, TValue>(KeyValuePair<TKey, TValue> pair) {
			return Tuple.Create(pair.Key, pair.Value);
		}
	}
}