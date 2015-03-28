using System;
using System.Collections.Generic;
using Enumerable = System.Linq.Enumerable;
#pragma warning disable 279
namespace Funq.Abstract
{
	public  abstract partial class AbstractMap<TKey, TValue, TMap>
		: AbstractIterable<KeyValuePair<TKey, TValue>, TMap, MapBuilder<TKey, TValue>>, IAnyMapLike<TKey, TValue>
		where TMap : AbstractMap<TKey, TValue, TMap>
	{
		private static readonly bool IsDifferenceImplemented;
		private static readonly bool IsExceptImplemented;
		private static readonly bool IsJoinImplemented;
		private static readonly bool IsMergeImplemented;

		static AbstractMap()
		{
			IsDifferenceImplemented = ReflectExt.DoesImplementMethod<TMap>("Difference", typeof(TMap));
			IsExceptImplemented = ReflectExt.DoesImplementMethod<TMap>("Except", typeof(TMap), typeof(Func<TKey, TValue, TValue, Option<TValue>>));
			IsJoinImplemented = ReflectExt.DoesImplementMethod<TMap>("Join", typeof(TMap), typeof(Func<TKey, TValue, TValue, TValue>));
			IsMergeImplemented = ReflectExt.DoesImplementMethod<TMap>("Merge", typeof(TMap), typeof(Func<TKey, TValue, TValue, TValue>));
		} 


		public static implicit operator TMap(AbstractMap<TKey, TValue, TMap> self)
		{
			return self as object as TMap;
		}


		/// <summary>
		/// Applies the specified accumulator on each key-value pair.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="initial">The initial.</param>
		/// <param name="accumulator">The accumulator.</param>
		public TResult Aggregate<TResult>(TResult initial, Func<TResult, TKey, TValue, TResult> accumulator)
		{
			accumulator.IsNotNull("accumulator");
			return base.Aggregate(initial, (result, item) => accumulator(result, item.Key, item.Value));
		}

		/// <summary>
		/// (Implementation) Casts the keys and the values of the map to a different type.
		/// </summary>
		/// <typeparam name="TOutMap">The type of the return provider.</typeparam>
		/// <typeparam name="TOutKey">The type of the tr key.</typeparam>
		/// <typeparam name="TOutValue">The type of the tr value.</typeparam>
		/// <param name="bFactory">The b factory.</param>
		protected internal virtual TOutMap Cast<TOutMap, TOutKey, TOutValue>(TOutMap bFactory)
			where TOutMap : IAnyMapLike<TOutKey, TOutValue>
		{
			return base.Select(bFactory, kvp => Kvp.Of(kvp.Key.Cast<TOutKey>(), kvp.Value.Cast<TOutValue>()));
		}

		public bool MapEquals(IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TValue> valueEq = null) {
			other.IsNotNull("other");
			return Equality.Map_Equate(this, other, valueEq);
		}

		public virtual bool MapEquals(TMap other, IEqualityComparer<TValue> valueEq = null) {
			other.IsNotNull("other");
			return MapEquals((IEnumerable<KeyValuePair<TKey, TValue>>) other, valueEq);

		}

		/// <summary>
		///   Returns true if the specified key is contained in the map.
		/// </summary>
		/// <param name="k"> The key. </param>
		/// <returns> </returns>
		public bool ContainsKey(TKey k)
		{
			return TryGet(k).IsSome;
		}

		/// <summary>
		/// Determines whether the map contains the specified value, using an optional non-default equality handler.
		/// </summary>
		/// <param name="find">The value.</param>
		/// <param name="eq">Optionally, an equality handler. Otherwise the default handler is used.</param>
		public bool ContainsValue(TValue find, IEqualityComparer<TValue> eq = null ) {
			eq = eq ?? FastEquality<TValue>.Default;
			return Find((k, v) => eq.Equals(v, find)).IsSome;
		}

		/// <summary>
		/// Counts the number of items that fulfill the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public int Count(Func<TKey, TValue, bool> predicate)
		{
			predicate.IsNotNull("predicate");
			return base.Count(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Iterates unconditionally over each key-value pair.
		/// </summary>
		/// <param name="act">The act.</param>
		public void ForEach(Action<TKey, TValue> act) {
			act.IsNotNull("act");
			base.ForEach(kvp => act(kvp.Key, kvp.Value));
		}
		/// <summary>
		/// Returns true if any key-value pair matches the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public bool Any(Func<TKey, TValue, bool> predicate) {
			predicate.IsNotNull("predicate");
			return base.Any(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Iterates over each key-value pair until the specified predicate returns false.
		/// </summary>
		/// <param name="act">The predicate.</param>
		public bool ForEachWhile(Func<TKey, TValue, bool> act)
		{
			act.IsNotNull("act");
			return base.ForEachWhile(kvp => act(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Applies the specified selector to each value, returning a map with the same key type but a different value type.
		/// </summary>
		/// <typeparam name="TRProvider">The type of the tr provider.</typeparam>
		/// <typeparam name="TRValue">The type of the tr value.</typeparam>
		/// <param name="bFactory">The b factory.</param>
		/// <param name="selector">The selector.</param>
		protected internal virtual TRProvider SelectValues<TRProvider, TRValue>(TRProvider bFactory, Func<TKey, TValue, TRValue> selector )
			where TRProvider : IAnyMapLike<TKey, TRValue> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			return base.Select(bFactory, kvp => Kvp.Of(kvp.Key, selector(kvp.Key, kvp.Value)));
		}

		/// <summary>
		/// Finds the first key-value pair that matches the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public Option<KeyValuePair<TKey, TValue>> Find(Func<TKey, TValue, bool> predicate)
		{
			return base.Find(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// The keys contained in this map.
		/// </summary>
		public virtual IEnumerable<TKey> Keys
		{
			get
			{
				return Enumerable.Select(this, x => x.Key);
			}
		}
		/// <summary>
		/// The values contained in this map.
		/// </summary>
		public virtual IEnumerable<TValue> Values
		{
			get
			{
				return Enumerable.Select(this, x => x.Value);
			}
		}
		/// <summary>
		/// Joins two maps by their keys, applying the specified collision resolution function to determine the value in the return map.
		/// </summary>
		/// <typeparam name="TValue2">The type of the value in the second collection.</typeparam>
		/// <typeparam name="TRMap">The type of the return map.</typeparam>
		/// <typeparam name="TRValue">The type of the tr value.</typeparam>
		/// <param name="bFactory">A prototype instance used as a builder factory.</param>
		/// <param name="other">The other map.</param>
		/// <param name="collision">The collision resolution function.</param>
		protected internal virtual TRMap Join<TValue2, TRMap, TRValue>(TRMap bFactory, IAnyMapLike<TKey, TValue2> other,Func<TKey, TValue, TValue2, TRValue> collision)
			where TRMap : IAnyMapLike<TKey, TRValue> {
			bFactory.IsNotNull("bFactory");
			other.IsNotNull("other");

			using (var builder = bFactory.EmptyBuilder)
			{
				foreach (var item in this)
				{
					var pair = other.TryGet(item.Key);
					if (pair.IsSome)
					{
						if (collision == null) {
							throw Errors.Maps_not_disjoint(item.Key);
						}
						builder.Add(item.Key, collision(item.Key, item.Value, pair.Value));
					}
				}
				return (TRMap)bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		/// Joins this map with another map.
		/// </summary>
		/// <typeparam name="TValue2">The type of value of the second map.</typeparam>
		/// <param name="other">The other map.</param>
		/// <param name="collision">The collision function that generates the value of the new map.</param>
		/// <returns></returns>
		public TMap Join<TValue2>(IAnyMapLike<TKey, TValue2> other, Func<TKey, TValue, TValue2, TValue> collision) {
			if (other is TMap && IsJoinImplemented) {
				return Join((TMap) other, (Func<TKey, TValue, TValue, TValue>) (object) collision);
			}
			return Join(this, other, collision);
		}

		/// <summary>
		/// Returns a new map without all those keys present in the specified map.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="subtraction">The subtraction function that generates the value </param>
		public virtual TMap Except<TValue2>(IAnyMapLike<TKey, TValue2> other, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null) {
			other.IsNotNull("other");
			if (other is TMap && IsExceptImplemented) {
				return Except((TMap)other, (Func<TKey, TValue, TValue, Option<TValue>>)(object)subtraction);
			}
			using (var builder = EmptyBuilder)
			{
				foreach (var item in this) {
					var res = other.TryGet(item.Key);
					if (res.IsNone) {
						builder.Add(item);
					}
					else if (subtraction != null) {
						var newValue = subtraction(item.Key, item.Value, res.Value);
						if (newValue.IsSome) {
							builder.Add(item.Key, newValue);
						}
					}
				}
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		/// Merges the two maps, applying the specified collision resolution function for every key that appears in both maps.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">The collision resolution function. If null, the maps are assumed to have no keys in common, and a collision throws an exception.</param>
		public virtual TMap Merge(IAnyMapLike<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> collision) {
			other.IsNotNull("other");
			if (other is TMap && IsMergeImplemented) {
				return Merge((TMap) other, collision);
			}
			collision = collision ?? ((k, v1, v2) =>
			                          {
				                          throw Errors.Maps_not_disjoint(k);
			                          });
			using (var builder = BuilderFrom(this))
			{
				foreach (var item in other)
				{
					var myElement = builder.Lookup(item.Key);
					builder[item.Key] = myElement.IsSome ? collision(item.Key, myElement, item.Value) : item.Value;
				}
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		/// Merges the two maps, applying the specified collision resolution function for every key that appears in both maps.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">The collision resolution function. If null, the maps are assumed to have no keys in common, and a collision throws an exception.</param>
		public virtual TMap Merge(TMap other, Func<TKey, TValue, TValue, TValue> collision)
		{
			other.IsNotNull("other");
			return this.Merge(other as IAnyMapLike<TKey, TValue>, collision);
		}

		/// <summary>
		/// Joins two maps by their keys, applying the specified collision resolution function to determine the value in the return map.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">The collision.</param>
		public virtual TMap Join(TMap other, Func<TKey, TValue, TValue, TValue> collision)
		{
			other.IsNotNull("other");
			return this.Join(this, other as IAnyMapLike<TKey, TValue>, collision);
		}

		/// <summary>
		/// Performs the Except operation, potentially removing all the keys present in the other map.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <param name="subtraction">A substraction selector that determines the new value (if any) when a collision occurs. If null, colliding keys are removed.</param>
		public virtual TMap Except(TMap other, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null)
		{
			other.IsNotNull("other");
			return this.Except(other as IAnyMapLike<TKey, TValue>);
		}

		/// <summary>
		/// Returns true if all the key-value pairs satisfy the predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public bool All(Func<TKey, TValue, bool> predicate) {
			predicate.IsNotNull("predicate");
			return base.All(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Returns a new map containing only those key-value pairs present in exactly one of the maps.
		/// </summary>
		/// <param name="other">The other.</param>
		public virtual TMap Difference(TMap other) {
			other.IsNotNull("other");
			return this.Except(other).Merge(other.Except(this), null);
		}

		/// <summary>
		/// Filters the keys and values of the map by type.
		/// </summary>
		/// <typeparam name="TRMap">The type of the return map</typeparam>
		/// <typeparam name="TRKey">The type of the return key.</typeparam>
		/// <typeparam name="TRValue">The type of the return value.</typeparam>
		/// <param name="bFactory">A prototype instance of the return map.</param>
		protected internal virtual TRMap OfType<TRMap, TRKey, TRValue>(TRMap bFactory)
			where TRMap : IAnyMapLike<TRKey, TRValue>
		{
			bFactory.IsNotNull("bFactory");
			return base.Choose(bFactory, kvp =>
			                             {
				                             var key = kvp.Key;
				                             var value = kvp.Value;
				                             if (key is TRKey && value is TRValue)
					                             return Kvp.Of(key.Cast<TRKey>(), value.Cast<TRValue>()).AsSome();
				                             return Option.None;
			                             });
		}

		/// <summary>
		/// Returns the first value for which the specified selector returns Some.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="selector">The selector.</param>
		public Option<TResult> Pick<TResult>(Func<TKey, TValue, Option<TResult>> selector)
		{
			return base.Pick(kvp => selector(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Applies the specified accumulator on each key-value pair, with the first value obtained by using the first function.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="first">The function used to retrieve the first value.</param>
		/// <param name="accumulator">The accumulator.</param>
		public TResult Reduce<TResult>(Func<TKey, TValue, TResult> first, Func<TResult, TKey, TValue, TResult> accumulator)
		{
			return base.Reduce(kvp => first(kvp.Key, kvp.Value), (r, kvp) => accumulator(r, kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="k">The key.</param>
		public TValue this[TKey k]
		{
			get
			{
				var v = TryGet(k);
				if (v.IsNone) throw Errors.Key_not_found;
				return v.Value;
			}
		}

		/// <summary>
		/// Returns the value associated with the specified key, or None if the key doesn't exist.
		/// </summary>
		/// <param name="k"> The key. </param>
		/// <returns> </returns>
		public abstract Option<TValue> TryGet(TKey k);


		/// <summary>
		/// Filters the key-value pairs using the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public TMap Where(Func<TKey, TValue, bool> predicate)
		{
			predicate.IsNotNull("predicate");
			return base.Where(kvp => predicate(kvp.Key, kvp.Value));
		}


	}
}