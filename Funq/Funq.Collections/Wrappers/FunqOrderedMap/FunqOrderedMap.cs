using System;
using System.Collections.Generic;
using System.Linq;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public sealed partial class FunqOrderedMap<TKey, TValue> : Trait_MapLike<TKey, TValue, FunqOrderedMap<TKey, TValue>>
	{
		internal readonly OrderedAvlTree<TKey, TValue>.Node Root;
		internal readonly IComparer<TKey> Comparer;

		static FunqOrderedMap() {

		} 
		
		public static FunqOrderedMap<TKey, TValue> Empty(IComparer<TKey> comparer)
		{
			return new FunqOrderedMap<TKey, TValue>(OrderedAvlTree<TKey, TValue>.Node.Null, comparer ?? FastComparer<TKey>.Value);
		}

		internal FunqOrderedMap(OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer )
		{
			Root = root;
			Comparer = comparer;
		}

		public FunqOrderedMap<TKey, TValue> Add(Tuple<TKey, TValue> pair) {
			return Add(pair.Item1, pair.Item2);
		}

		public FunqOrderedMap<TKey, TValue> Add(Kvp<TKey, TValue> pair) {
			return Add(pair.Key, pair.Value);
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
		{
			return Root.Items.GetEnumerator();
		}

		public override Option<TValue> TryGet(TKey k)
		{
			return Root.Find(k);
		}

		public override bool IsEmpty
		{
			get
			{
				return Root.IsNull;
			}
		}

		public override bool ForEachWhile(System.Func<Kvp<TKey, TValue>, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return Root.ForEachWhile((k, v) => Kvp.Of(k, v).Pipe(iterator));
		}

		public override int Length
		{
			get
			{
				return Root.Count;
			}
		}

		public override FunqOrderedMap<TKey, TValue> Merge(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Union(other.Root, collision, Lineage.Mutable()).WrapMap(Comparer);
		}

		public override FunqOrderedMap<TKey, TValue> Join(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Intersect(other.Root, collision, Lineage.Mutable()).WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TRValue> SelectValues<TRValue>(Func<TKey, TValue, TRValue> selector)
		{
			return Root.Apply(selector, Lineage.Mutable()).WrapMap(Comparer);
		}

		public override FunqOrderedMap<TKey, TValue> Difference(FunqOrderedMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return Root.SymDifference(other.Root, Lineage.Mutable()).WrapMap(Comparer);
		}

		public override FunqOrderedMap<TKey, TValue> Except(FunqOrderedMap<TKey, TValue> other, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Except(other.Root, Lineage.Mutable(), subtraction).WrapMap(Comparer);
		}

		internal FunqOrderedMap<TKey, TValue> AddRange(IEnumerable<Kvp<TKey, TValue>> items, OverwriteBehavior behavior) {
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = Root;

			foreach (var item in items) {
				var newerRoot = newRoot.IsNull
					? newRoot.InitializeFromNull(item.Key, item.Value, Comparer, lineage)
					: newRoot.AvlAdd(item.Key, item.Value, lineage, behavior == OverwriteBehavior.Overwrite);

				if (newerRoot == null)
				{
					if (behavior == OverwriteBehavior.Throw)
					{
						throw Errors.Key_exists;
					}
				}
				else
				{
					newRoot = newerRoot;
				}
			}
			return newRoot.WrapMap(Comparer);
		} 

		public FunqOrderedMap<TKey, TValue> AddRange(IEnumerable<Kvp<TKey, TValue>> items) {
			return AddRange(items, OverwriteBehavior.Overwrite);
		}

		public FunqOrderedMap<TKey, TValue> AddRange(IEnumerable<Tuple<TKey, TValue>> tuples)
		{
			return this.AddRange(tuples.Select(x => (Kvp<TKey, TValue>) x));
		}

		public FunqOrderedMap<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> tuples)
		{
			return this.AddRange(tuples.Select(x => (Kvp<TKey, TValue>)x));
		}

		public FunqOrderedMap<TKey, TValue> DropRange(IEnumerable<TKey> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var map = Root;
			var wasModified = false;
			foreach (var item in items) {
				var newMap = map.AvlRemove(item, lineage);
				if (newMap != null) {
					map = newMap;
					wasModified = true;
				}
			}
			return wasModified ? map.WrapMap(Comparer) : this;
		} 

		public Kvp<TKey, TValue> MaxItem
		{
			get
			{
				if (Root.IsNull) throw Errors.Is_empty;
				var maxNode = Root.Max;
				return Kvp.Of(maxNode.Key, maxNode.Value);
			}
		}

		public Kvp<TKey, TValue> MinItem
		{
			get
			{
				if (Root.IsNull) throw Errors.Is_empty;
				var minNode = Root.Min;
				return Kvp.Of(minNode.Key, minNode.Value);
			}
		}

		public Kvp<TKey, TValue> ByOrder(int index) {
			if (Root.IsNull) throw Errors.Is_empty;
			var opt = Root.ByOrder(index);
			if (opt.IsNone) {
				throw Errors.Arg_out_of_range("index", index);
			}
			else {
				return Kvp.Of(opt.Value.Key, opt.Value.Value);
			}
		}

		public FunqOrderedMap<TKey, TValue> Drop(TKey key)
		{
			var removed = Root.AvlRemove(key, Lineage.Immutable);
			if (removed == null) return this;
			return removed.WrapMap(Comparer);
		}


		public FunqOrderedMap<TKey, TValue> DropMax()
		{
			if (Root.IsNull) throw Errors.Is_empty;
			return Root.RemoveMax(Lineage.Mutable()).WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TValue> DropMin()
		{
			if (Root.IsNull) throw Errors.Is_empty;
			return Root.RemoveMin(Lineage.Mutable()).WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TValue> Add(TKey key, TValue value)
		{
			return Set(key, value, OverwriteBehavior.Throw);
		}

		public FunqOrderedMap<TKey, TValue> Set(TKey key, TValue value) {
			return Set(key, value, OverwriteBehavior.Overwrite);
		}

		internal FunqOrderedMap<TKey, TValue> Set(TKey key, TValue value, OverwriteBehavior behavior) {
			var ret = Root.IsNull
				? Root.InitializeFromNull(key, value, Comparer, Lineage.Mutable())
				: Root.AvlAdd(key, value, Lineage.Mutable(), behavior == OverwriteBehavior.Overwrite);
			if (ret == null && behavior == OverwriteBehavior.Throw) throw Errors.Key_exists;
			if (ret == null) return null;
			return ret.WrapMap(Comparer);
		}
	}
}
