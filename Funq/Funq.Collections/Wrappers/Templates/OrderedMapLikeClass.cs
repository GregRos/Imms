using Funq;
using System.Collections.Generic;
using System;
using Funq.Abstract;
using Linq = System.Linq;

public partial class __OrderedMapLikeClass__<TKey,TValue> : AbstractMap<TKey,TValue,__OrderedMapLikeClass__<TKey,TValue>>
{
	private __OrderedMapLikeClass__<TKey2,TValue2> GetPrototype<TKey2,TValue2>(__HandlerObject__<TKey2> ph)
	{
		return __OrderedMapLikeClass__<TKey2, TValue2>.Empty(ph);
	}

	/// <summary>
	/// Applies the specified selector on each element of the collection, returning the same kind of collection with a different element time.
	/// </summary>
	/// <typeparam name="TRKey">The type of the R key.</typeparam>
	/// <typeparam name="TRValue">The type of the R value.</typeparam>
	/// <param name="selector">The selector.</param>
	/// <param name="handler">A new equality or comparison handler for constructing the resulting map.</param>
	/// <returns></returns>
	public __OrderedMapLikeClass__<TRKey,TRValue> Select<TRKey,TRValue>(Func<KeyValuePair<TKey,TValue>, KeyValuePair<TRKey,TRValue>> selector, __HandlerObject__<TRKey> handler)
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
	public __OrderedMapLikeClass__<TRKey, TRValue> Select<TRKey, TRValue>(Func<KeyValuePair<TKey, TValue>, Optional<KeyValuePair<TRKey, TRValue>>> selector, __HandlerObject__<TRKey> handler)
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
	public __OrderedMapLikeClass__<TRKey,TRValue> Select<TRKey,TRValue>(Func<TKey, TValue, KeyValuePair<TRKey,TRValue>> selector, __HandlerObject__<TRKey> handler)
	{
		return base.Select(this.GetPrototype<TRKey, TRValue>(handler), kvp => selector(kvp.Key, kvp.Value));
	}

	/// <summary>
	/// Applies the specified selector on each value of the collection, discarding all elements for which the selector returns Option.None.
	/// </summary>
	/// <typeparam name="TRValue">The type of the R value.</typeparam>
	/// <param name="selector">The selector.</param>
	/// <returns></returns>
	public __OrderedMapLikeClass__<TKey,TRValue> SelectValues<TRValue>(Func<TKey,TValue,Optional<TRValue>> selector)
	{
		return base.Choose(GetPrototype<TKey, TRValue>(__CurrentHandler__), kvp =>
			                                                {
				                                                var maybe = selector(kvp.Key, kvp.Value);
																				if (maybe.IsSome) return Optional.Some(Kvp.Of(kvp.Key, maybe.Value));
				                                                return Optional.None;
			                                                });
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
	public __OrderedMapLikeClass__<TRKey,TRValue> SelectMany<TRKey,TRValue,TProject>(Func<KeyValuePair<TKey,TValue>, IEnumerable<TProject>> selector,
																					Func<KeyValuePair<TKey,TValue>, IEnumerable<TProject>, KeyValuePair<TRKey,TRValue>> rSelector, __HandlerObject__<TRKey> handler)
	{
		return base.SelectMany(GetPrototype<TRKey,TRValue>(handler), selector, rSelector);
	}

}
