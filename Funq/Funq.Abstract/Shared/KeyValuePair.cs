using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using Funq;

namespace Funq
{
	public static class Kvp
	{
		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(TKey k, TValue v)
		{
			return new KeyValuePair<TKey, TValue>(k, v);
		}

		public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(Tuple<TKey, TValue> pair) {
			return Kvp.Of(pair.Item1, pair.Item2);
		}

		public static Tuple<TKey, TValue> ToTuple<TKey, TValue>(KeyValuePair<TKey, TValue> pair) {
			return Tuple.Create(pair.Key, pair.Value);
		}
	}
}