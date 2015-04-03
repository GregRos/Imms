using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public sealed partial class FunqOrderedMap<TKey, TValue> : AbstractMap<TKey, TValue, FunqOrderedMap<TKey, TValue>>
	{
		internal readonly OrderedAvlTree<TKey, TValue>.Node Root;
		internal readonly IComparer<TKey> Comparer;

		static FunqOrderedMap() {

		} 
		
		public static FunqOrderedMap<TKey, TValue> Empty(IComparer<TKey> comparer)
		{
			return new FunqOrderedMap<TKey, TValue>(OrderedAvlTree<TKey, TValue>.Node.Empty, comparer ?? FastComparer<TKey>.Default);
		}

		internal FunqOrderedMap(OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer )
		{
			Root = root;
			Comparer = comparer;
		}

		protected override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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
				return Root.IsEmpty;
			}
		}

		public override bool ForEachWhile(System.Func<KeyValuePair<TKey, TValue>, bool> iterator)
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

		protected override FunqOrderedMap<TKey, TValue> Merge(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision = null) {
			if (other == null) throw Errors.Is_null;
			return Root.Union(other.Root, collision, Lineage.Mutable()).WrapMap(Comparer);
		}

		protected override FunqOrderedMap<TKey, TValue> Join(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Intersect(other.Root, collision, Lineage.Mutable()).WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TRValue> SelectValues<TRValue>(Func<TKey, TValue, TRValue> selector)
		{
			return Root.Apply(selector, Lineage.Mutable()).WrapMap(Comparer);
		}

		protected override FunqOrderedMap<TKey, TValue> Difference(FunqOrderedMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return Root.SymDifference(other.Root, Lineage.Mutable()).WrapMap(Comparer);
		}

		public override FunqOrderedMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
		{
			var set = keys as FunqOrderedSet<TKey>;
			if (set != null && Comparer.Equals(set.Comparer))
			{
				return Root.Except(set.Root, Lineage.Mutable()).WrapMap(Comparer);
			}
			return base.RemoveRange(keys);
		}

		public override FunqOrderedMap<TKey, TValue> Except<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null)
		{
			var map = other as FunqOrderedMap<TKey, TValue2>;
			if (map != null && Comparer.Equals(map.Comparer))
			{
				return Root.Except(map.Root, Lineage.Mutable(), subtraction).WrapMap(Comparer);
			}
			return base.Except(other, subtraction);
		}

		protected override FunqOrderedMap<TKey, TValue> Except(FunqOrderedMap<TKey, TValue> other, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null)
		{
			throw new Exception("Should never be called.");
		}

		public KeyValuePair<TKey, TValue> MaxItem
		{
			get
			{
				if (Root.IsEmpty) throw Errors.Is_empty;
				var maxNode = Root.Max;
				return Kvp.Of(maxNode.Key, maxNode.Value);
			}
		}

		public KeyValuePair<TKey, TValue> MinItem
		{
			get
			{
				if (Root.IsEmpty) throw Errors.Is_empty;
				var minNode = Root.Min;
				return Kvp.Of(minNode.Key, minNode.Value);
			}
		}

		public KeyValuePair<TKey, TValue> ByOrder(int index) {
			index = index < 0 ? index + Length : index;
			index.IsInRange("index", 0, Length - 1);			
			var opt = Root.ByOrder(index);
			if (opt.IsNone) {
				throw Errors.Arg_out_of_range("index", index);
			}
			else {
				return Kvp.Of(opt.Value.Key, opt.Value.Value);
			}
		}

		public FunqOrderedMap<TKey, TValue> Remove(TKey key)
		{
			var removed = Root.AvlRemove(key, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TValue> RemoveMax()
		{
			if (Root.IsEmpty) throw Errors.Is_empty;
			return Root.RemoveMax(Lineage.Mutable()).WrapMap(Comparer);
		}

		public FunqOrderedMap<TKey, TValue> RemoveMin()
		{
			if (Root.IsEmpty) throw Errors.Is_empty;
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
			var ret = Root.Root_Add(key, value, Comparer, behavior == OverwriteBehavior.Overwrite, Lineage.Mutable());
			if (ret == null && behavior == OverwriteBehavior.Throw) throw Errors.Key_exists;
			if (ret == null) return null;
			return ret.WrapMap(Comparer);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqOrderedMap<TKey, TValue> op_Add(Tuple<TKey, TValue> item)
		{
			return Add(item.Item1, item.Item2);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqOrderedMap<TKey, TValue> op_AddRange(IEnumerable<Tuple<TKey, TValue>> items) {
			return AddRange(items.Select(t => Kvp.Of(t)));
		}
	}
}
