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
		private readonly IComparer<TKey> _comparer;
		private readonly OrderedAvlTree<TKey, TValue>.Node _root;

		internal ImmSortedMap(OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer) {
			_root = root;
			_comparer = comparer;
		}

		protected override ImmSortedMap<TKey, TValue> UnderlyingCollection
		{
			get { return this; }
		}
		public override bool IsEmpty {
			get { return _root.IsEmpty; }
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public override int Length {
			get { return _root.Count; }
		}
		/// <summary>
		/// Returns the maximal key-value pair in this map.
		/// </summary>
		public KeyValuePair<TKey, TValue> MaxItem {
			get {
				if (_root.IsEmpty) throw Errors.Is_empty;
				var maxNode = _root.Max;
				return Kvp.Of(maxNode.Key, maxNode.Value);
			}
		}

		/// <summary>
		/// Returns the minimal key-value pair in this map.
		/// </summary>
		public KeyValuePair<TKey, TValue> MinItem {
			get {
				if (_root.IsEmpty) throw Errors.Is_empty;
				var minNode = _root.Min;
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
			var cmp = Comparers.KeyComparer<KeyValuePair<TKey, TValue>, TKey>(x => x.Key, _comparer);
			Array.Sort(arr, 0, len, cmp);
			arr.RemoveDuplicatesInSortedArray((a, b) => _comparer.Compare(a.Key, b.Key) == 0, ref len);
			var lineage = Lineage.Mutable();
			var node = OrderedAvlTree<TKey, TValue>.Node.FromSortedArray(arr, 0, len - 1, _comparer, lineage);
			var newRoot = _root.Union(node, collision, lineage);
			return newRoot.WrapMap(_comparer);
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

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _root.Items.GetEnumerator();
		}

		protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
			return _root.FindKvp(key);
		}

		public override Optional<TValue> TryGet(TKey key) {
			return _root.Find(key);
		}

		public override bool ForEachWhile(Func<KeyValuePair<TKey, TValue>, bool> function) {
			function.CheckNotNull("function");
			return _root.ForEachWhile((k, v) => function(Kvp.Of(k, v)));
		}

		protected override ImmSortedMap<TKey, TValue> Merge(ImmSortedMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Union(other._root, collision, Lineage.Mutable()).WrapMap(_comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Join(ImmSortedMap<TKey, TValue> other,
			ValueSelector<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Intersect(other._root, Lineage.Mutable(), collision).WrapMap(_comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Difference(ImmSortedMap<TKey, TValue> other) {
			return _root.SymDifference(other._root, Lineage.Mutable()).WrapMap(_comparer);
		}

		public override ImmSortedMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) {
			keys.CheckNotNull("keys");
			var set = keys as ImmSortedSet<TKey>;
			if (set != null && _comparer.Equals(set.Comparer)) return _root.Except(set.Root, Lineage.Mutable()).WrapMap(_comparer);
			return base.RemoveRange(keys);
		}

		public override ImmSortedMap<TKey, TValue> Subtract<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			ValueSelector<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
			other.CheckNotNull("other");
			var map = other as ImmSortedMap<TKey, TValue2>;
			if (map != null && _comparer.Equals(map._comparer)) {
				return _root.Except(map._root, Lineage.Mutable(), subtraction).WrapMap(_comparer);
			}
			return base.Subtract(other, subtraction);
		}

		protected override ImmSortedMap<TKey, TValue> Subtract(ImmSortedMap<TKey, TValue> other, ValueSelector<TKey, TValue, TValue, Optional<TValue>> subtraction = null) {
			return _root.Except(other._root, Lineage.Mutable(), subtraction).WrapMap(_comparer);
		}

		/// <summary>
		/// Returns the key-value pair at the specified index in the ordered map, by key order.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public KeyValuePair<TKey, TValue> ByOrder(int index) {
			index.CheckIsBetween("index", -_root.Count, _root.Count - 1);
			index = index < 0 ? index + _root.Count : index;
			var opt = _root.ByOrder(index);
			return Kvp.Of(opt.Key, opt.Value);
		}

		public override ImmSortedMap<TKey, TValue> Remove(TKey key) {
			var removed = _root.AvlRemove(key, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(_comparer);
		}

		/// <summary>
		/// Removes the maximal key-value pair from this map.
		/// </summary>
		/// <returns></returns>
		public ImmSortedMap<TKey, TValue> RemoveMax() {
			if (_root.IsEmpty) throw Errors.Is_empty;
			return _root.RemoveMax(Lineage.Mutable()).WrapMap(_comparer);
		}

		/// <summary>
		/// removes the minimal key-value pair from this map.
		/// </summary>
		/// <returns></returns>
		public ImmSortedMap<TKey, TValue> RemoveMin() {
			if (_root.IsEmpty) throw Errors.Is_empty;
			return _root.RemoveMin(Lineage.Mutable()).WrapMap(_comparer);
		}

		protected override ImmSortedMap<TKey, TValue> Set(TKey key, TValue value, bool overwrite) {
			var ret = _root.Root_Add(key, value, _comparer, overwrite, Lineage.Mutable());
			if (ret == null && !overwrite) throw Errors.Key_exists(key);
			if (ret == null) return this;
			return ret.WrapMap(_comparer);
		}
	}
}