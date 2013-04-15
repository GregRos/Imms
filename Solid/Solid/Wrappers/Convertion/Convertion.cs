using System;
using System.Collections.Generic;

namespace Solid
{
	/// <summary>
	///   Provides extension methods for converting between, to, and from Solid data structures.
	/// </summary>
	public static class Convertion
	{
		/// <summary>
		///   Returns a wrapper for the IEnumerable that implicitly generates a collection of type FlexibleList.
		/// </summary>
		/// <typeparam name="T"> The type of value the flexible list will contain. </typeparam>
		/// <param name="items"> The sequence to process. </param>
		/// <returns> </returns>
		public static DelayedList<T> DelayList<T>(this IEnumerable<T> items)
		{
			return new DelayedList<T>(items);
		}





		/// <summary>
		///   Converts the HashMap to a Dictionary object from the standard collection library.
		///   The dictionary is created using the HashMap's equality comparer if none is specified..
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="map">The HashMap object.</param>
		/// <param name="comparer">The comparer to use for the dictionary.</param>
		/// <returns> </returns>
		internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this HashMap<TKey, TValue> map,
		                                                                    IEqualityComparer<TKey> comparer)
			where TKey : IEquatable<TKey>
		{
			var dict = new Dictionary<TKey, TValue>(comparer ?? map.Comparer);
			map.ForEach(dict.Add);
			return dict;
		}

		/// <summary>
		///   Creates a FlexibleList from a sequence of items.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="items">The sequence.</param>
		/// <returns> </returns>
		public static FlexibleList<T> ToFlexibleList<T>(this IEnumerable<T> items)
		{
			return FlexibleList<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		///   Creates a HashMap from a collection of KeyValuePair objects. The keys must all be unique.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="items"> The collection of items. </param>
		/// <param name="comparer"> The equality comparer used by the instance to determine equality. If this parameter is not specified, the default equality comparer is used instead, which makes use of instance methods. </param>
		/// <returns> </returns>
		internal static HashMap<TKey, TValue> ToHashMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items,
		                                                              IEqualityComparer<TKey> comparer = null)
		{
			var hm = comparer == null
				? HashMap<TKey, TValue>.empty
				: HashMap<TKey, TValue>.WithComparer(comparer);
			var dict = new Dictionary<TKey, TValue>();
			foreach (var item in items)
			{
				hm = hm.Add(item.Key, item.Value);
			}
			return hm;
		}

		/// <summary>
		///   Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="items">The sequence.</param>
		/// <returns> </returns>
		public static Vector<T> ToVector<T>(this IEnumerable<T> items)
		{
			return Vector<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		///   Converts the list into a vector.
		/// </summary>
		/// <typeparam name="T"> The type of value stored in the collection. </typeparam>
		/// <param name="list"> The list. </param>
		/// <returns> </returns>
		public static Vector<T> ToVector<T>(this FlexibleList<T> list)
		{
			return Vector<T>.Empty.AddLastRange(list);
		}
	}
}