using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

#pragma warning disable 279

namespace Imms.Abstract {
	
	public abstract partial class AbstractMap<TKey, TValue, TMap>
		: AbstractIterable<KeyValuePair<TKey, TValue>, TMap, IMapBuilder<TKey, TValue, TMap>>
		where TMap : AbstractMap<TKey, TValue, TMap> {

		/// <summary>
		/// Indicates the behavior that should be used when a KVP already exists in the map.
		/// </summary>
		protected enum OverwriteBehavior {
			/// <summary>
			/// The existing value should be overwritten.
			/// </summary>
			Overwrite,
			/// <summary>
			/// An exception should be thrown.
			/// </summary>
			Throw
		}

		/// <summary>
		///     The keys contained in this map.
		/// </summary>
		public virtual IEnumerable<TKey> Keys {
			get {
				return this.Select(x => x.Key);
			}
		}

		/// <summary>
		///     The values contained in this map.
		/// </summary>
		public virtual IEnumerable<TValue> Values {
			get { return this.Select(x => x.Value); }
		}

		/// <summary>
		/// Tries to find the key-value pair with the specified key, or returns None.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		protected abstract Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key);

		/// <summary>
		/// Adds a new key-value pair to the map.
		/// </summary>
		/// <param name="kvp">The key-value pair.</param>
		/// <returns></returns>
		public TMap Add(KeyValuePair<TKey, TValue> kvp) {
			return Add(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Returns the value associated with the specified key, or None if no such key exists.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public virtual Optional<TValue> TryGet(TKey key) {
			var result = TryGetKvp(key);
			return result.IsNone ? Optional.None : result.Value.Value.AsOptional();
		}

		/// <summary>
		///     Returns true if the specified key is contained in the map.
		/// </summary>
		/// <param name="k"> The key. </param>
		/// <returns> </returns>
		public bool ContainsKey(TKey k) {
			return TryGet(k).IsSome;
		}

		/// <summary>
		///     Applies the specified accumulator on each key-value pair.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="initial">The initial.</param>
		/// <param name="accumulator">The accumulator.</param>
		public TResult Aggregate<TResult>(TResult initial, Func<TResult, TKey, TValue, TResult> accumulator) {
			accumulator.CheckNotNull("accumulator");
			return base.Aggregate(initial, (result, item) => accumulator(result, item.Key, item.Value));
		}

		/// <summary>
		///    Determines whether the specified map is compatible with this one (in terms of equality semantics, for example).
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected abstract bool IsCompatibleWith(TMap other);

		/// <summary>
		/// Determines whether this map is equal to a sequence of key-value pairs. Maps are equal if they contain the same keys, and if the values associated with them are also equal.
		/// </summary>
		/// <param name="other">The sequence of key-value pairs, understood to be a map.</param>
		/// <param name="eqComparer">Optionally, an equality comparer for determining the equality of values. If not specified, the default equality comparer for the type is used.</param>
		/// <returns></returns>
		public bool MapEquals(IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TValue> eqComparer = null) {
			eqComparer = eqComparer ?? FastEquality<TValue>.Default;
			return MapEquals(other, eqComparer.Equals); 
		}

		/// <summary>
		/// Determines whether this map is equal to a sequence of key-value pairs. Maps are equal if they contain the same keys, and if the values associated with them are also equal.
		/// </summary>
		/// <param name="other">The sequence of key-value pairs, taken to be a map.</param>
		/// <param name="comparer">A comparer for determining the equality of values.</param>
		/// <returns></returns>
		public bool MapEquals(IEnumerable<KeyValuePair<TKey, TValue>> other, IComparer<TValue> comparer) {
			comparer.CheckNotNull("comparer");
			return MapEquals(other, (a,b) => comparer.Compare(a,b) == 0);
		}

		/// <summary>
		/// Determines whether this map is equal to a sequence of key-value pairs. Maps are equal if they contain the same keys, and if the values associated with them are also equal.
		/// </summary>
		/// <param name="other">The sequence of key-value pairs.</param>
		/// <param name="equality">A function for determining equality between values.</param>
		/// <returns></returns>
		public virtual bool MapEquals(IEnumerable<KeyValuePair<TKey, TValue>> other, Func<TValue, TValue, bool> equality) {
			other.CheckNotNull("other");
			equality.CheckNotNull("equality");
			var boiler = EqualityHelper.BoilerEquality(this, other);
			if (boiler.IsSome) {
				return boiler.Value;
			}
			var map = other as TMap;
			if (map != null && IsCompatibleWith(map)) return MapEquals(map, equality);
			var tryLength = other.TryGuessLength();
			if (tryLength.IsSome && tryLength.Value < Length) {
				return false;
			}

			return other.ForEachWhile(kvp => {
				var myValue = this.TryGet(kvp.Key);
				return myValue.IsSome && equality(myValue.Value, kvp.Value);
			});
		}

		/// <summary>
		/// Determines whether this map is equal to a sequence of key-value pairs. Maps are equal if they contain the same keys, and if the values associated with them are also equal.
		/// </summary>
		/// <param name="other">The sequence of key-value pairs.</param>
		/// <param name="valueEq">A function for determining equality between values.</param>
		/// <returns></returns>
		protected virtual bool MapEquals(TMap other, Func<TValue, TValue, bool> valueEq)
		{
			other.CheckNotNull("other");
			if (Length != other.Length) {
				return false;
			}
			return other.ForEachWhile((key,value) => {
				var myValue = TryGet(key);
				if (myValue.IsNone) return false;
				return valueEq(myValue.Value, value);
			});
		}

		/// <summary>
		///     Counts the number of items that fulfill the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public int Count(Func<TKey, TValue, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return base.Count(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		/// Returns the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public TValue this[TKey key] {
			get
			{
				var result = TryGet(key);
				if (result.IsNone) {
					throw Errors.Key_not_found(key);
				}
				return result.Value;
			}
		}

		/// <summary>
		///     Iterates unconditionally over each key-value pair.
		/// </summary>
		/// <param name="act">The act.</param>
		public void ForEach(Action<TKey, TValue> act) {
			act.CheckNotNull("act");
			base.ForEach(kvp => act(kvp.Key, kvp.Value));
		}

		/// <summary>
		///     Returns true if any key-value pair matches the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public bool Any(Func<TKey, TValue, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return base.Any(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		///     Iterates over each key-value pair until the specified predicate returns false.
		/// </summary>
		/// <param name="act">The predicate.</param>
		public bool ForEachWhile(Func<TKey, TValue, bool> act) {
			act.CheckNotNull("act");
			return base.ForEachWhile(kvp => act(kvp.Key, kvp.Value));
		}

		/// <summary>
		///     Finds the first key-value pair that matches the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public Optional<KeyValuePair<TKey, TValue>> Find(Func<TKey, TValue, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return base.Find(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		///     Joins this map with another map by key, returning a map consisting of the keys present in both maps, the value of each such key being determined by the specified collision resolution function.
		/// </summary>
		/// <typeparam name="TValue2">The type of value of the second map.</typeparam>
		/// <param name="other">The other map.</param>
		/// <param name="selector">The function that determines the value associated with each key in the new map.</param>
		/// <remarks>
		/// A map join operation is an operation over maps of key-value pairs, which is analogous to an intersection operation over sets.
		///	The operation returns a new key-value map consisting of only those keys present in both maps, 
		/// with the value associated with each key being determined by the collision resolution function.
		///	The collision resolution function takes as input the key, and the values associated with that key in the two maps.
		/// This method is optimized when the input collection is a map compatible with this one.
		/// </remarks>
		/// <returns></returns>
		public TMap Join<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			Func<TKey, TValue, TValue2, TValue> selector) {
			other.CheckNotNull("other");
			selector.CheckNotNull("selector");
			var map = other as TMap;
			if (map != null && IsCompatibleWith(map)) return Join(map, (Func<TKey, TValue, TValue, TValue>) (object) selector);
			return Join_Unchecked(other, selector);
		}

			/// <summary>
		///     Joins two maps by their keys, applying the specified collision resolution function to determine the value in the
		///     return map.
		/// </summary>
		/// <typeparam name="TValue2">The type of the value in the second collection.</typeparam>
		/// <typeparam name="TRMap">The type of the return map.</typeparam>
		/// <typeparam name="TRValue">The type of the tr value.</typeparam>
		/// <param name="bFactory">A prototype instance used as a builder factory.</param>
		/// <param name="other">The other map.</param>
		/// <param name="collision">The collision resolution function.</param>
		protected internal virtual TRMap _Join<TValue2, TRMap, TRValue>(TRMap bFactory,
			IEnumerable<KeyValuePair<TKey, TValue2>> other, Func<TKey, TValue, TValue2, TRValue> collision)
			where TRMap : IBuilderFactory<IMapBuilder<TKey, TRValue, TRMap>>  {
			bFactory.CheckNotNull("bFactory");
			other.CheckNotNull("other");

			using (var builder = bFactory.EmptyBuilder) {
				other.ForEach(item => {
					var myValue = TryGet(item.Key);
					if (myValue.IsSome) {
						if (collision == null) throw Errors.Maps_not_disjoint(item.Key);
						var newValue = collision(item.Key, myValue.Value, item.Value);
						builder.Set(item.Key, newValue);
					}
				});
				return builder.Produce();
			}
		}

		private TMap Join_Unchecked<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			Func<TKey, TValue, TValue2, TValue> collision) {
			using (var builder = EmptyBuilder) {
				other.ForEach(pair => {
					var myKvp = builder.TryGetKvp(pair.Key).OrMaybe(TryGetKvp(pair.Key));
					if (myKvp.IsSome) {
						var kvp = myKvp.Value;
						var newValue = collision(kvp.Key, kvp.Value, pair.Value);
						builder.Set(kvp.Key, newValue);
					}
				});
				return builder.Produce();
			}
		}

		/// <summary>
		/// Removes several keys from this key-value map.
		/// </summary>
		/// <param name="keys">The keys to remove. </param>
		/// <returns></returns>
		public virtual TMap RemoveRange(IEnumerable<TKey> keys) {
			keys.CheckNotNull("other");
			if (IsEmpty) return this;
			using (var builder = BuilderFrom(this)) {
				var len = Length;
				keys.ForEachWhile(item => {
					if (builder.Remove(item)) {
						len--;
					}
					return len > 0;
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Subtracts the key-value pairs in the specified map from this one, applying the subtraction function on each key shared between the maps.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <param name="subtraction">Optionally, a subtraction function that generates the value in the resulting key-value map. Otherwise, key-value pairs are always removed.</param>
		/// <remarks>
		///	Subtraction over maps is anaologous to Except over sets. 
		///	If the subtraction function is not specified (or is null), the operation simply subtracts all the keys present in the other map from this one.
		/// If a subtraction function is supplied, the operation invokes the function on each key-value pair shared with the other map. If the function returns a value,
		/// that value is used in the return map. If the function returns None, the key is removed from the return map.
		/// </remarks>
		public virtual TMap Subtract<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
			if (subtraction == null && ReferenceEquals(other, this)) {
				return Empty;
			}
			other.CheckNotNull("other");

			var map = other as TMap;
			if (map != null && IsCompatibleWith(map)) return Subtract(map, (Func<TKey, TValue, TValue, Optional<TValue>>) (object) subtraction);

			return Subtract_Unchecked(other, subtraction);
		}

		TMap Subtract_Unchecked<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
			other.CheckNotNull("other");
			using (var builder = BuilderFrom(this)) {
				var len = Length;
				other.ForEachWhile(item => {
					if (subtraction == null) {
						if (builder.Remove(item.Key)) {
							len--;
						}
					}
					else {
						var tryGet = builder.TryGetKvp(item.Key).Map(x => x.Value);
						if (tryGet.IsNone) return len > 0;
						var newValue = subtraction(item.Key, tryGet.Value, item.Value);
						if (newValue.IsSome) {
							builder.Set(item.Key, newValue.Value);
						} else {
							if (builder.Remove(item.Key)) {
								len--;
							}
						}
					}
					return len > 0;
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Merges the two maps, applying the selector function for keys appearing in both maps.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">
		///     The collision resolution function. If null, the values in the other map overwrite the values in this map.
		/// </param>
		/// <remarks>
		/// The merge operation is analogous to a union operation over sets. 
		/// 
		/// This operation returns all key-value pairs present in either map. If a key is shared between both maps, the collision resolution function is applied to determine the value in the result map.
		/// </remarks>
		public virtual TMap Merge(IEnumerable<KeyValuePair<TKey, TValue>> other, Func<TKey, TValue, TValue, TValue> collision = null) {
			other.CheckNotNull("other");
			if (collision == null && ReferenceEquals(this, other)) {
				return this;
			}
			var map = other as TMap;
			if (map != null && IsCompatibleWith(map)) return Merge(map, collision);
			return Merge_Unchecked(other, collision);
		}

		/// <summary>
		/// Adds a new key-value pair to the map, possibly overwriting any previous value.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		/// <param name="behavior">The overwrite behavior requested.</param>
		/// <returns></returns>
		protected abstract TMap Set(TKey key, TValue value, OverwriteBehavior behavior);

		/// <summary>
		/// Adds a new key-value pair to the map, overwriting any previous value.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		/// <returns></returns>
		public TMap Set(TKey key, TValue value) {
			return Set(key, value, OverwriteBehavior.Overwrite);
		}

		/// <summary>
		/// Adds a new key-value pair to the map, throwing an exception if a pair with this key already exists.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		/// <returns></returns>
		public TMap Add(TKey key, TValue value) {
			return Set(key, value, OverwriteBehavior.Throw);
		}

		/// <summary>
		/// Removes a key from the map.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns></returns>
		public abstract TMap Remove(TKey key);

		/// <summary>
		///     Adds a sequence of key-value pairs to the map, throwing an exception on collision.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">
		///     Thrown if the map already contains one of the keys, or if there are duplicate keys
		///     in the sequence.
		/// </exception>
		public TMap AddRange(IEnumerable<KeyValuePair<TKey, TValue>> other) {
			other.CheckNotNull("other");
			return Merge(other, (k, v1, v2) => { throw Errors.Key_exists(k); });
		}

		/// <summary>
		///     Adds a sequence of key-value pairs to the map, overwriting old data on collision.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public TMap SetRange(IEnumerable<KeyValuePair<TKey, TValue>> other) {
			other.CheckNotNull("other");
			return Merge(other);
		}

		TMap Merge_Unchecked(IEnumerable<KeyValuePair<TKey, TValue>> other,
			Func<TKey, TValue, TValue, TValue> collision = null) {
			other.CheckNotNull("other");
			using (var builder = BuilderFrom(this)) {
				other.ForEach(item => {
					if (collision == null) builder.Set(item.Key, item.Value);
					else {
						var myElement = builder.TryGetKvp(item.Key).Map(x => x.Value);
						builder.Set(item.Key, myElement.IsSome ? collision(item.Key, myElement.Value, item.Value) : item.Value);
					}
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Merges the two maps, applying the specified collision resolution function for every key that appears in both maps.
		///     <br />
		///     Override this operation to implement it efficiently.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">
		///     Optionally, a collision resolution function. If null, data in the other map overwrites data in the current map.
		/// </param>
		protected virtual TMap Merge(TMap other, Func<TKey, TValue, TValue, TValue> collision = null) {
			other.CheckNotNull("other");
			return Merge_Unchecked(other, collision);
		}

		/// <summary>
		///     Joins two maps by their keys, applying the specified collision resolution function to determine the value in the
		///     return map.
		///     Override this operation to implement it efficiently.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <param name="collision">The collision.</param>
		protected virtual TMap Join(TMap other, Func<TKey, TValue, TValue, TValue> collision = null) {
			other.CheckNotNull("other");
			return Join_Unchecked(other, collision);
		}

		/// <summary>
		///     Performs the Except operation, potentially removing all the keys present in the other map.
		///     Override this operation to implement it efficiently.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <param name="subtraction">
		///     A substraction selector that determines the new value (if any) when a collision occurs. If
		///     null, colliding keys are removed.
		/// </param>
		protected virtual TMap Subtract(TMap other, Func<TKey, TValue, TValue, Optional<TValue>> subtraction = null) {
			other.CheckNotNull("other");
			return Subtract_Unchecked(other, subtraction);
		}

		/// <summary>
		///     Returns true if all the key-value pairs satisfy the predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public bool All(Func<TKey, TValue, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return base.All(kvp => predicate(kvp.Key, kvp.Value));
		}

		/// <summary>
		///     Returns a map consisting of all the key-value pairs where the key is contained in exactly one map.
		/// </summary>
		/// <param name="other">The a sequence of key-value pairs, understood to be a map..</param>
		/// <returns></returns>
		public virtual TMap Difference(IEnumerable<KeyValuePair<TKey, TValue>> other) {
			other.CheckNotNull("other");
			var map = other as TMap;
			if (map != null && IsCompatibleWith(map)) return Difference(map);
			var otherMap = this.ToIterable(other);
			return Subtract(otherMap).Merge(otherMap.Subtract(this));
		}

		/// <summary>
		///     Returns a new map containing only those key-value pairs present in exactly one of the maps.
		/// </summary>
		/// <param name="other">The other.</param>
		protected virtual TMap Difference(TMap other) {
			other.CheckNotNull("other");
			return Subtract(other).Merge(other.Subtract(this));
		}

		/// <summary>
		///     Returns the first value for which the specified selector returns Some, or None if this doesn't happen.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="selector">The selector.</param>
		public Optional<TResult> Pick<TResult>(Func<TKey, TValue, Optional<TResult>> selector) {
			selector.CheckNotNull("selector");
			return base.Pick(kvp => selector(kvp.Key, kvp.Value));
		}

	}


}