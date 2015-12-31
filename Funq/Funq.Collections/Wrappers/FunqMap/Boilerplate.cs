

namespace Funq.Collections
{
		using Funq;
	using System.Collections.Generic;
	using System;
	using Funq.Abstract;
	using Linq = System.Linq;
	
	public partial class FunqMap<TKey,TValue> : AbstractMap<TKey,TValue,FunqMap<TKey,TValue>>
	{
		private FunqMap<TKey2,TValue2> GetPrototype<TKey2,TValue2>(IEqualityComparer<TKey2> ph)
		{
			return FunqMap<TKey2, TValue2>.Empty(ph);
		}
	
		/// <summary>
		/// Applies the specified selector on each element of the collection, returning the same kind of collection with a different element time.
		/// </summary>
		/// <typeparam name="TRKey">The type of the R key.</typeparam>
		/// <typeparam name="TRValue">The type of the R value.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <param name="handler">A new equality or comparison handler for constructing the resulting map.</param>
		/// <returns></returns>
		public FunqMap<TRKey,TRValue> Select<TRKey,TRValue>(Func<KeyValuePair<TKey,TValue>, KeyValuePair<TRKey,TRValue>> selector, IEqualityComparer<TRKey> handler = null)
		{
			return base.Select(GetPrototype<TRKey,TRValue>(handler), selector);
		}
	
		/// <summary>
		/// Applies the specified selector on each element of the collection, discarding all elements for which the selector returns Option.None.
		/// </summary>
		/// <typeparam name="TRKey">The type of the R key.</typeparam>
		/// <typeparam name="TRValue">The type of the R value.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <param name="handler">A new equality or comparison handler for constructing the resulting map.</param>
		/// <returns></returns>
		public FunqMap<TRKey, TRValue> Select<TRKey, TRValue>(Func<KeyValuePair<TKey, TValue>, Optional<KeyValuePair<TRKey, TRValue>>> selector, IEqualityComparer<TRKey> handler = null)
		{
			return base.Choose(this.GetPrototype<TRKey,TRValue>(handler), selector);
		}
	
		/// <summary>
		/// Applies the specified selector on each element of the collection, discarding all elements for which the selector returns Option.None.
		/// </summary>
		/// <typeparam name="TRKey">The type of the R key.</typeparam>
		/// <typeparam name="TRValue">The type of the R value.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <param name="handler">A new equality or comparison handler for constructing the resulting map.</param>
		/// <returns></returns>
		public FunqMap<TRKey,TRValue> Select<TRKey,TRValue>(Func<TKey, TValue, KeyValuePair<TRKey,TRValue>> selector, IEqualityComparer<TRKey> handler = null)
		{
			return base.Select(this.GetPrototype<TRKey, TRValue>(handler), kvp => selector(kvp.Key, kvp.Value));
		}
	
		/// <summary>
		/// Applies the specified selector on each value of the collection, discarding all elements for which the selector returns Option.None.
		/// </summary>
		/// <typeparam name="TRValue">The type of the R value.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqMap<TKey,TRValue> SelectValues<TRValue>(Func<TKey,TValue,Optional<TRValue>> selector) {
			return base.Choose(GetPrototype<TKey, TRValue>(Equality), kvp => {
				var maybe = selector(kvp.Key, kvp.Value);
				if (maybe.IsSome) return Kvp.Of(kvp.Key, maybe.Value);
				return Optional.NoneOf<KeyValuePair<TKey, TRValue>>();
			});
		}
	
		/// <summary>
		/// Joins this map with another map using this map's equality or comparison logic, and applies the specified collision function on matching key-value pairs.
		/// </summary>
		/// <typeparam name="TValue2"></typeparam>
		/// <typeparam name="TRValue"></typeparam>
		/// <param name="other"></param>
		/// <param name="collision"></param>
		/// <returns></returns>
		public FunqMap<TKey, TRValue> Join<TValue2, TRValue>(IEnumerable<KeyValuePair<TKey, TValue2>> other, Func<TKey, TValue, TValue2, TRValue> collision){
			return base.Join(GetPrototype<TKey, TRValue>(Equality), other, collision);
		}
	
		/// <summary>
		/// Selects the many.
		/// </summary>
		/// <typeparam name="TRKey">The type of the R key.</typeparam>
		/// <typeparam name="TRValue">The type of the R value.</typeparam>
		/// <typeparam name="TProject">The type of the project.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <param name="rSelector">The r selector.</param>
		/// <param name="handler">A new equality or comparison handler for constructing the resulting map.</param>
		/// <returns></returns>
		public FunqMap<TRKey,TRValue> SelectMany<TRKey,TRValue,TProject>(Func<KeyValuePair<TKey,TValue>, IEnumerable<TProject>> selector,
																						Func<KeyValuePair<TKey,TValue>, IEnumerable<TProject>, KeyValuePair<TRKey,TRValue>> rSelector, IEqualityComparer<TRKey> handler = null)
		{
			return base.SelectMany(GetPrototype<TRKey,TRValue>(handler), selector, rSelector);
		}
	}
	
		}