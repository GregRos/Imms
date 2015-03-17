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
		internal readonly OrderedAvlTree<TKey, TValue>.Node _root;
		private readonly IComparer<TKey> _comparer; 
		
		public static FunqOrderedMap<TKey, TValue> Empty(IComparer<TKey> comparer)
		{
			return new FunqOrderedMap<TKey, TValue>(OrderedAvlTree<TKey, TValue>.Node.Null, comparer ?? Comparer<TKey>.Default);
		}

		internal FunqOrderedMap(OrderedAvlTree<TKey, TValue>.Node root, IComparer<TKey> comparer )
		{
			_root = root;
			_comparer = comparer;
		}

		public FunqOrderedMap<TKey, TValue> Add(Tuple<TKey, TValue> pair)
		{
			if (this.ContainsKey(pair.Item1)) throw Funq.Errors.Key_exists;
			return _root.AvlAdd(_comparer.WrapKey(pair.Item1), pair.Item2, Lineage.Mutable()).WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TValue> Add(Kvp<TKey, TValue> pair)
		{
			if (this.ContainsKey(pair.Key)) throw Funq.Errors.Key_exists;
			return _root.AvlAdd(_comparer.WrapKey(pair.Key), pair.Value, Lineage.Mutable()).WrapMap(_comparer);
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
		{
			return _root.Items.GetEnumerator();
		}

		public override Option<TValue> TryGet(TKey k)
		{
			return _root.Find(_comparer.WrapKey(k));
		}

		public override bool IsEmpty
		{
			get
			{
				return _root.IsNull;
			}
		}

		public override bool ForEachWhile(System.Func<Kvp<TKey, TValue>, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return _root.ForEachWhile((k, v) => Kvp.Of(k, v).Pipe(iterator));
		}

		public override int Length
		{
			get
			{
				return _root.Count;
			}
		}

		public override FunqOrderedMap<TKey, TValue> Merge(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null || collision == null) throw Errors.Is_null;
			return _root.Union(other._root, collision).WrapMap(_comparer);
		}

		public override FunqOrderedMap<TKey, TValue> Join(FunqOrderedMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null || collision == null) throw Errors.Is_null;
			return _root.Intersect(other._root, collision, Lineage.Immutable).WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TRValue> SelectValues<TRValue>(Func<TKey, TValue, TRValue> selector)
		{
			return _root.Apply(selector, Lineage.Immutable).WrapMap(_comparer);
		}


		public override FunqOrderedMap<TKey, TValue> Except(FunqOrderedMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Except(other._root).WrapMap(_comparer);
		}

		public override FunqOrderedMap<TKey, TValue> Difference(FunqOrderedMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.SymDifference(other._root).WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TValue> AddMany(IEnumerable<Kvp<TKey, TValue>> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var map = this;
			foreach (var item in items)
			{
				map = map.Add(item.Key, item.Value);
			}
			return map;
		}

		public FunqOrderedMap<TKey, TValue> AddMany(IEnumerable<Tuple<TKey, TValue>> tuples)
		{
			return this.AddMany(tuples.Select(x => (Kvp<TKey, TValue>) x));
		}

		public FunqOrderedMap<TKey, TValue> DropMany(IEnumerable<TKey> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var map = this;
			foreach (var item in items)
			{
				map = map.Drop(item);
			}
			return map;
		} 

		public Kvp<TKey, TValue> MaxItem
		{
			get
			{
				if (_root.IsNull) throw Errors.Is_empty;
				var maxNode = _root.Max;
				return Kvp.Of(maxNode.Key.Key, maxNode.Value);
			}
		}

		public Kvp<TKey, TValue> MinItem
		{
			get
			{
				if (_root.IsNull) throw Errors.Is_empty;
				var minNode = _root.Min;
				return Kvp.Of(minNode.Key.Key, minNode.Value);
			}
		}

		public Kvp<TKey, TValue> ByOrder(int index) {
			if (_root.IsNull) throw Errors.Is_empty;
			var opt = _root.ByOrder(index);
			if (opt.IsNone) {
				throw Errors.Arg_out_of_range("index", index);
			}
			else {
				return Kvp.Of(opt.Value.Key.Key, opt.Value.Value);
			}
		}

		public FunqOrderedMap<TKey, TValue> Drop(TKey key)
		{
			var removed = _root.AvlRemove(_comparer.WrapKey(key), Lineage.Mutable());
			if (removed == null) return this;

			return removed.WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TValue> DropMax()
		{
			if (_root.IsNull) throw Errors.Is_empty;
			return _root.RemoveMax(Lineage.Mutable()).WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TValue> DropMin()
		{
			if (_root.IsNull) throw Errors.Is_empty;
			return _root.RemoveMin(Lineage.Mutable()).WrapMap(_comparer);
		}

		public FunqOrderedMap<TKey, TValue> Add(TKey key, TValue value)
		{
			if (this.ContainsKey(key)) throw Funq.Errors.Key_exists;
			return _root.AvlAdd(_comparer.WrapKey(key), value, Lineage.Mutable()).WrapMap(_comparer);
		}
		public FunqOrderedMap<TKey, TValue> Set(TKey key, TValue value)
		{
			return _root.AvlAdd(_comparer.WrapKey(key), value, Lineage.Mutable()).WrapMap(_comparer);
		}
	}
}
