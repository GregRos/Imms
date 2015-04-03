using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using Funq;

namespace Funq
{
	/// <summary>
	/// Utility methods for working with key-value pairs.
	/// </summary>
	public static class Kvp
	{
		/// <summary>
		/// Constructs a new key-value pair.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="k"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(TKey k, TValue v)
		{
			return new KeyValuePair<TKey, TValue>(k, v);
		}

		/// <summary>
		/// Constructs a new key-value pair from a tuple.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="pair"></param>
		/// <returns></returns>
		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(Tuple<TKey, TValue> pair) {
			return Kvp.Of(pair.Item1, pair.Item2);
		}

		/// <summary>
		/// Turns a key-value pair into a tuple.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="pair"></param>
		/// <returns></returns>
		public static Tuple<TKey, TValue> ToTuple<TKey, TValue>(KeyValuePair<TKey, TValue> pair) {
			return Tuple.Create(pair.Key, pair.Value);
		}

	}
}