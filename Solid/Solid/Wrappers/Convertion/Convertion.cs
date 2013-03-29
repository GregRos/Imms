using System;
using System.Collections.Generic;

namespace Solid
{


	public static class Convertion
	{
		/// <summary>
		/// Returns a wrapper for the IEnumerable that implicitly generates a collection of type FlexibleList.
		/// Supports a subset of LINQ methods and query syntax.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		public static DelayedList<T> DelayList<T>(this IEnumerable<T> items)
		{
			return new DelayedList<T>(items);
		}

		/// <summary>
		/// Converts a list to an array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="seq"></param>
		/// <returns></returns>
		public static T[] ToArray<T>(this FlexibleList<T> seq)
		{
			var list = new T[seq.Count];
			int index = 0;
			seq.ForEach(v =>
			            {
				            list[index] = v;
				            index++;
			            });
			return list;
		}

		/// <summary>
		/// Converts the Vector to an array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="vect"></param>
		/// <returns></returns>
		public static T[] ToArray<T>(this Vector<T> vect)
		{
			var list = new T[vect.Count];
			int index = 0;
			vect.ForEach(v =>
			             {
				             list[index] = v;
				             index++;
			             });
			return list;
		}

		/// <summary>
		/// Converts the HashMap to a Dictionary object from the standard collection library.
		/// The dictionary is created using the HashMap's equality comparer if none is specified..
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="map"></param>
		/// <param name="comparer"> </param>
		/// <returns></returns>
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this HashMap<TKey, TValue> map,
		                                                                  IEqualityComparer<TKey> comparer)
			where TKey : IEquatable<TKey>
		{
			var dict = new Dictionary<TKey, TValue>(comparer ?? map.Comparer);
			map.ForEach(dict.Add);
			return dict;
		}

		/// <summary>
		/// Creates a HashMap from a collection of KeyValuePair objects. The keys must all be unique.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="items">The collection of items.</param>
		/// <param name="comparer">
		/// The equality comparer used by the instance to determine equality. 
		/// If this parameter is not specified, the default equality comparer is used instead, which makes use of instance methods.</param>
		/// <returns></returns>
		public static HashMap<TKey, TValue> ToHashMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items,
		                                                            IEqualityComparer<TKey> comparer = null)
		{
			HashMap<TKey, TValue> hm = comparer == null
				                           ? HashMap<TKey, TValue>.Empty
				                           : HashMap<TKey, TValue>.WithComparer(comparer);
			var dict = new Dictionary<TKey, TValue>();
			foreach (var item in items)
			{
				hm = hm.Add(item.Key, item.Value);
			}
			return hm;
		}

		/// <summary>
		/// Creates a FlexibleList from a collection of items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		public static FlexibleList<T> ToFlexibleList<T>(this IEnumerable<T> items)
		{
			return FlexibleList<T>.Empty.AddLastRange(items);
		}

		/// <summary>
		/// Creates a Vector from a collection of items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		public static Vector<T> ToVector<T>(this IEnumerable<T> items)
		{
			return Vector<T>.Empty.AddLastRange(items);
		}

		public static Vector<T> ToVector<T>(this FlexibleList<T> list)
		{
			return Vector<T>.Empty.AddLastRange(list);
		}

	}
}