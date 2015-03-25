using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal static class Comparison
	{
		/// <summary>
		///   Returns a comparison handler that determines the order between objects using a key.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
		/// <param name="keyComparer"> </param>
		/// <returns> </returns>
		public static IComparer<T> ByKey<T, TKey>(Func<T, TKey> selector, IComparer<TKey> keyComparer = null)
		{
			keyComparer = keyComparer ?? Comparer<TKey>.Default;
			return Comparer<T>.Create((x, y) => keyComparer.Compare(selector(x), selector(y)));
		}
	}
}