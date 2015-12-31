using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {



	public sealed partial class ImmMap<TKey, TValue> : AbstractMap<TKey, TValue, ImmMap<TKey, TValue>> {
		private readonly IEqualityComparer<TKey> _equality;
		private readonly HashedAvlTree<TKey, TValue>.Node _root;

		internal ImmMap(HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality) {
			_root = root;
			_equality = equality;
		}

		public override int Length {
			get { return _root.Count; }
		}

		protected override ImmMap<TKey, TValue> UnderlyingCollection
		{
			get { return this; }
		}

		public override bool IsEmpty {
			get { return _root.IsEmpty; }
		}

		public new static ImmMap<TKey, TValue>  Empty(IEqualityComparer<TKey> equality = null) {
			return new ImmMap<TKey, TValue>(HashedAvlTree<TKey, TValue>.Node.Empty, equality ?? FastEquality<TKey>.Default);
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _root.GetEnumerator();
		}

		internal KeyValuePair<TKey, TValue> ByArbitraryOrder(int index)
		{
			index.CheckIsBetween("index", -_root.Count, _root.Count - 1);
			index = index < 0 ? index + _root.Count : index;
			return _root.ByArbitraryOrder(index).AsKvp;
		}

		protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
			return _root.Root_FindKvp(key);
		}

		public override Optional<TValue> TryGet(TKey key) {
			return _root.Root_Find(key);
		}

		protected override ImmMap<TKey, TValue> Set(TKey k, TValue v, OverwriteBehavior behavior) {
			var r = _root.Root_Add(k, v, Lineage.Mutable(), _equality, behavior == OverwriteBehavior.Overwrite);
			if (r == null && behavior == OverwriteBehavior.Throw) throw Errors.Key_exists(k);
			if (r == null) return this;
			return r.WrapMap(_equality);
		}

		/// <summary>
		///     Removes the specified key from the map.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <exception cref="KeyNotFoundException">Thrown if the specified key doesn't exist in the map.</exception>
		/// <returns></returns>
		public override ImmMap<TKey, TValue> Remove(TKey k) {
			if (_root.IsEmpty) throw Errors.Is_empty;
			var removed = _root.Root_Remove(k, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Merge(ImmMap<TKey, TValue> other,
			Func<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Union(other._root, Lineage.Mutable(), collision).WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Join(ImmMap<TKey, TValue> other,
			Func<TKey, TValue, TValue, TValue> collision = null) {
			return _root.Intersect(other._root, Lineage.Mutable(), collision).WrapMap(_equality);
		}

		public override ImmMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) {
			keys.CheckNotNull("keys");
			var set = keys as ImmSet<TKey>;
			if (set != null && _equality.Equals(set.EqualityComparer)) return _root.Except(set.Root, Lineage.Mutable()).WrapMap(_equality);
			return base.RemoveRange(keys);
		}

		public override ImmMap<TKey, TValue> Subtract<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other,
			Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				other.CheckNotNull("other");
			var map = other as ImmMap<TKey, TValue2>;
			if (map != null && _equality.Equals(map._equality)) return Except(map,subtraction);
			return base.Subtract(other, subtraction);
		}

		ImmMap<TKey, TValue> Except<TValue2>(ImmMap<TKey, TValue2> other,
			Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				other.CheckNotNull("other");
			return _root.Except(other._root, Lineage.Mutable(), subtraction).WrapMap(_equality);
		}

		protected override ImmMap<TKey, TValue> Subtract(ImmMap<TKey, TValue> other, Func<TKey, TValue, TValue, Optional<TValue>> subtraction = null) {
			return Except(other, subtraction);
		}

		protected override ImmMap<TKey, TValue> Difference(ImmMap<TKey, TValue> other) {
			other.CheckNotNull("other");
			return _root.SymDifference(other._root, Lineage.Mutable()).WrapMap(_equality);
		}


		public ImmMap<TKey, TValue> Add(Tuple<TKey, TValue> pair) {
			pair.CheckNotNull("pair");
			return Add(pair.Item1, pair.Item2);
		}

		public override bool ForEachWhile(Func<KeyValuePair<TKey, TValue>, bool> function) {
			function.CheckNotNull("function");
			return _root.ForEachWhile((eqKey, v) => function(Kvp.Of(eqKey, v)));
		}

	}
}