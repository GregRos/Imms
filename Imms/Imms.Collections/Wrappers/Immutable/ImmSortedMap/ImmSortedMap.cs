using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	/// <summary>
	/// Immutable and persistent ordered map that uses comparison and allows looking up key-value pairs by sort order. 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public sealed partial class ImmSortedMap<TKey, TValue> : AbstractMap<TKey, TValue, ImmSortedMap<TKey, TValue>> {
		private readonly IComparer<TKey> Comparer;
		private readonly OrderedAvlTree<TKey, TValue>.Node Root;

		internal ImmSortedMap(OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer) {
			Root = root;
			Comparer = comparer;
		}

		protected override ImmSortedMap<TKey, TValue> UnderlyingCollection
		{
			get { return this; }
		}

		/// <summary>
		///     Returns true if the collection is empty.
		/// </summary>
		public override bool IsEmpty {
			get { return Root.IsEmpty; }
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public override int Length {
			get { return Root.Count; }
		}
		/// <summary>
		/// Returns the maximal key-value pair in this map.
		/// </summary>
		public KeyValuePair<TKey, TValue> MaxItem {
			get {
				if (Root.IsEmpty) throw Errors.Is_empty;
				var maxNode = Root.Max;
				return Kvp.Of(maxNode.Key, maxNode.Value);
			}
		}

		/// <summary>
		/// Returns the minimal key-value pair in this map.
		/// </summary>
		public KeyValuePair<TKey, TValue> MinItem {
			get {
				if (Root.IsEmpty) throw Errors.Is_empty;
				var minNode = Root.Min;
				return Kvp.Of(minNode.Key, minNode.Value);
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
		public override ImmSortedMap<TKey, TValue> Merge(IEnumerable<KeyValuePair<TKey, TValue>> other, ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			other.CheckNotNull("other");
			var map = other as ImmSortedMap<TKey, TValue>;
			if (map != null && IsCompatibleWith(map)) return Merge(map, collision);
			int len;
			var arr = other.ToArrayFast(out len);
			var cmp = Comparers.KeyComparer<KeyValuePair<TKey, TValue>, TKey>(x => x.Key, Comparer);
			Array.Sort(arr, 0, len, cmp);
			arr.RemoveDuplicatesInSortedArray((a, b) => Comparer.Compare(a.Key, b.Key) == 0, ref len);
			var lineage = Lineage.Mutable();
			var node = OrderedAvlTree<TKey, TValue>.Node.FromSortedArray(arr, 0, len - 1, Comparer, lineage);
			var newRoot = Root.Union(node, collision, lineage);
			return newRoot.WrapMap(Comparer);
		}

		/// <summary>
		/// Returns an empty <see cref="ImmSortedMap{TKey,TValue}"/> using the specified comparer.
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public new static ImmSortedMap<TKey, TValue> Empty(IComparer<TKey> comparer) {
			return new ImmSortedMap<TKey, TValue>(OrderedAvlTree<TKey, TValue>.Node.Empty,
				comparer ?? FastComparer<TKey>.Default);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return Root.Items.GetEnumerator();
		}

		protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
			return Root.FindKvp(key);
		}

		/// <summary>
		/// Returns the value associated with the specified key, or None if no such key exists.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public override Optional<TValue> TryGet(TKey key) {
			return Root.Find(key);
		}

		/// <summary>
		///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public override bool ForEachWhile(Func<KeyValuePair<TKey, TValue>, bool> function) {
			function.CheckNotNull("function");
			return Root.ForEachWhile((k, v) => function(Kvp.Of(k, v)));
		}

		protected override ImmSortedMap<TKey, TValue> Merge(ImmSortedMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return Root.Union(other.Root, collision, Lineage.Mutable()).WrapMap(Comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Join(ImmSortedMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return Root.Intersect(other.Root, Lineage.Mutable(), collision).WrapMap(Comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Difference(ImmSortedMap<TKey, TValue> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).WrapMap(Comparer);
		}

		/// <summary>
		/// Removes several keys from this key-value map.
		/// </summary>
		/// <param name="keys">A sequence of keys to remove. Can be much faster if it's a set compatible with this map.</param>
		/// <returns></returns>
		public override ImmSortedMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) {
			keys.CheckNotNull("keys");
			var set = keys as ImmSortedSet<TKey>;
			if (set != null && Comparer.Equals(set.Comparer)) return Root.Except(set.Root, Lineage.Mutable()).WrapMap(Comparer);
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
		public override ImmSortedMap<TKey, TValue> Subtract<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			ValueSelector<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
			other.CheckNotNull("other");
			var map = other as ImmSortedMap<TKey, TValue2>;
			if (map != null && Comparer.Equals(map.Comparer)) {
				return Root.Except(map.Root, Lineage.Mutable(), subtraction).WrapMap(Comparer);
			}
			return base.Subtract(other, subtraction);
		}

		protected override ImmSortedMap<TKey, TValue> Subtract(ImmSortedMap<TKey, TValue> other, ValueSelector<TKey, TValue, TValue, Optional<TValue>> subtraction = null) {
			return Root.Except(other.Root, Lineage.Mutable(), subtraction).WrapMap(Comparer);
		}

		/// <summary>
		/// Returns the key-value pair at the specified index in the ordered map, by key order.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">If the index is out of range</exception>
		public KeyValuePair<TKey, TValue> ByOrder(int index) {
			index.CheckIsBetween("index", -Root.Count, Root.Count - 1);
			index = index < 0 ? index + Root.Count : index;
			var opt = Root.ByOrder(index);
			return Kvp.Of(opt.Key, opt.Value);
		}

		/// <summary>
		/// Removes a key from the map, or does nothing if the key does not exist.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns></returns>
		public override ImmSortedMap<TKey, TValue> Remove(TKey key) {
			var removed = Root.AvlRemove(key, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(Comparer);
		}

		/// <summary>
		/// Removes the maximal key-value pair from this map.
		/// </summary>
		/// <returns></returns>
		public ImmSortedMap<TKey, TValue> RemoveMax() {
			if (Root.IsEmpty) throw Errors.Is_empty;
			return Root.RemoveMax(Lineage.Mutable()).WrapMap(Comparer);
		}

		/// <summary>
		/// removes the minimal key-value pair from this map.
		/// </summary>
		/// <returns></returns>
		public ImmSortedMap<TKey, TValue> RemoveMin() {
			if (Root.IsEmpty) throw Errors.Is_empty;
			return Root.RemoveMin(Lineage.Mutable()).WrapMap(Comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Set(TKey key, TValue value, bool overwrite) {
			var ret = Root.Root_Add(key, value, Comparer, overwrite, Lineage.Mutable());
			if (ret == null && !overwrite) throw Errors.Key_exists(key);
			if (ret == null) return this;
			return ret.WrapMap(Comparer);
		}

		/// <summary>
		/// Returns the index of the specified key, by sort order, or None if the key was not found.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Optional<int> IndexOf(TKey key) {
			return Root.IndexOf(key);
		} 

		/// <summary>
		/// Returns a slice of the map, bounded by an optional minimum key and an optional maximum key. The bounds are included in the result.
		/// </summary>
		/// <returns></returns>
		public ImmSortedMap<TKey, TValue> Slice(Optional<TKey> minimum = default(Optional<TKey>), Optional<TKey> maximum = default(Optional<TKey>)) {
			if (minimum.IsNone && maximum.IsNone) {
				return this;
			}
			OrderedAvlTree<TKey, TValue>.Node left, central, right;
			var currentRoot = Root;
			if (minimum.IsSome) {
				currentRoot.Split(minimum.Value, out left, out central, out right, Lineage.Immutable);
				currentRoot = right;
				if (central != null) {
					currentRoot = currentRoot.Root_Add(left.Key, left.Value, Comparer, true, Lineage.Immutable);
				}
			}
			if (maximum.IsSome) {
				currentRoot.Split(maximum.Value, out left, out central, out right, Lineage.Immutable);
				currentRoot = left;
				if (central != null) {
					currentRoot = currentRoot.Root_Add(central.Key, central.Value, Comparer, true, Lineage.Immutable);
				}
			}
			return currentRoot.WrapMap(Comparer);
		}
	}
}