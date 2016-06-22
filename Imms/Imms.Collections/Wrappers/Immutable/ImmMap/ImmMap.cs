using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {


		/// <summary>
	/// Immutable and persistent key-value map.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public sealed partial class ImmMap<TKey, TValue> : AbstractMap<TKey, TValue, ImmMap<TKey, TValue>> {
		private readonly IEqualityComparer<TKey> _equality;
		private readonly HashedAvlTree<TKey, TValue>.Node _root;

		internal ImmMap(HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality) {
			_root = root;
			_equality = equality;
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public override int Length {
			get { return _root.Count; }
		}

		protected override ImmMap<TKey, TValue> UnderlyingCollection
		{
			get { return this; }
		}

			/// <summary>
			///     Returns true if the collection is empty.
			/// </summary>
		public override bool IsEmpty {
			get { return _root.IsEmpty; }
		}

		/// <summary>
		/// Returns an empty <see cref="ImmMap{TKey,TValue}"/> using the specified eq comparer.
		/// </summary>
		/// <param name="equality"></param>
		/// <returns></returns>
		public new static ImmMap<TKey, TValue>  Empty(IEqualityComparer<TKey> equality = null) {
			return new ImmMap<TKey, TValue>(HashedAvlTree<TKey, TValue>.Node.Empty, equality ?? FastEquality<TKey>.Default);
		}

			/// <summary>
			/// Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
			/// </returns>
			/// <filterpriority>1</filterpriority>
			public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _root.GetEnumerator();
		}

		protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
			return _root.Root_FindKvp(key);
		}

			/// <summary>
			/// Returns the value associated with the specified key, or None if no such key exists.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <returns></returns>
			public override Optional<TValue> TryGet(TKey key) {
			return _root.Root_Find(key);
		}

		protected override ImmMap<TKey, TValue> Set(TKey k, TValue v, bool overwrite) {
			var r = _root.Root_Add(k, v, Lineage.Mutable(), _equality, overwrite);
			if (r == null && !overwrite) throw Errors.Key_exists(k);
			if (r == null) return this;
			return r.WrapMap(_equality);
		}

		/// <summary>
		///     Removes the specified key from the map, or does nothing if the key doesn't exist.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <returns></returns>
		public override ImmMap<TKey, TValue> Remove(TKey k) {
			var removed = _root.Root_Remove(k, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Merge(ImmMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Union(other._root, Lineage.Mutable(), collision).WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Join(ImmMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Intersect(other._root, Lineage.Mutable(), collision).WrapMap(_equality);
		}

			/// <summary>
			/// Removes several keys from this key-value map.
			/// </summary>
			/// <param name="keys">A sequence of keys to remove. Can be much faster if it's a set compatible with this map.</param>
			/// <returns></returns>
		public override ImmMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) {
			keys.CheckNotNull("keys");
			var set = keys as ImmSet<TKey>;
			if (set != null && _equality.Equals(set.EqualityComparer)) return _root.Except(set.Root, Lineage.Mutable()).WrapMap(_equality);
			return base.RemoveRange(keys);
		}

			/// <summary>
			///     Subtracts the key-value pairs in the specified map from this one, applying the subtraction function on each key shared between the maps.
			/// </summary>
			/// <param name="other">A sequence of key-value pairs. This operation is much faster if it's a map compatible with this one.</param>
			/// <param name="subtraction">Optionally, a subtraction function that generates the value in the resulting key-value map. Otherwise, key-value pairs are always removed.</param>
			/// <remarks>
			///	Subtraction over maps is anaologous to Except over sets. 
			///	If the subtraction function is not specified (or is null), the operation simply subtracts all the keys present in the other map from this one.
			/// If a subtraction function is supplied, the operation invokes the function on each key-value pair shared with the other map. If the function returns a value,
			/// that value is used in the return map. If the function returns None, the key is removed from the return map.
			/// </remarks>
			public override ImmMap<TKey, TValue> Subtract<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			ValueSelector<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				other.CheckNotNull("other");
			var map = other as ImmMap<TKey, TValue2>;
			if (map != null && _equality.Equals(map._equality)) return Subtract(map,subtraction);
			return base.Subtract(other, subtraction);
		}

		ImmMap<TKey, TValue> Subtract<TValue2>(ImmMap<TKey, TValue2> other,
			ValueSelector<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				other.CheckNotNull("other");
			return _root.Except(other._root, Lineage.Mutable(), subtraction).WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Subtract(ImmMap<TKey, TValue> other, ValueSelector<TKey, TValue, TValue, Optional<TValue>> subtraction = null) {
			return Subtract(other, subtraction);
		}

		protected override ImmMap<TKey, TValue> Difference(ImmMap<TKey, TValue> other) {
			other.CheckNotNull("other");
			return _root.SymDifference(other._root, Lineage.Mutable()).WrapMap(_equality);
		}


			/// <summary>
			///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
			/// </summary>
			/// <param name="function"> The function. </param>
			/// <returns> </returns>
			/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
			public override bool ForEachWhile(Func<KeyValuePair<TKey, TValue>, bool> function) {
			function.CheckNotNull("function");
			return _root.ForEachWhile((eqKey, v) => function(Kvp.Of(eqKey, v)));
		}

	}
}