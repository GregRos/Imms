using System;
using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqOrderedSet<T> : Trait_SetLike<T, FunqOrderedSet<T>>
	{
		private readonly OrderedAvlTree<T, bool>.Node _root;
		private readonly IComparer<T> _comparer;

		internal FunqOrderedSet(OrderedAvlTree<T, bool>.Node root, IComparer<T> comparer )
		{
			_root = root;
			_comparer = comparer;
		}

		public static FunqOrderedSet<T> Empty(IComparer<T> comparer)
		{
			return new FunqOrderedSet<T>(OrderedAvlTree<T, bool>.Node.Null, comparer ?? Comparer<T>.Default);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			foreach (var item in _root.Items)
				yield return item.Key;
		}

		public FunqOrderedSet<T> Add(T item)
		{
			if (this.Contains(item)) return this;
			var res = _root.IsNull
				? _root.FromNull(item, true, _comparer, Lineage.Mutable())
				: _root.AvlAdd(item, true, Lineage.Mutable());
			return res.WrapSet(_comparer);
		}

		public FunqOrderedSet<T> Drop(T item)
		{
			var removed = _root.AvlRemove(item, Lineage.Mutable()) ?? _root;
			return removed.WrapSet(_comparer);
		}

		public T ByOrder(int index) {
			var result = _root.ByOrder(index);
			if (result.IsNone) {
				throw Errors.Arg_out_of_range("index", index);
			}
			return result.Value.Key;
		}

		public override bool IsSupersetOf(FunqOrderedSet<T> other) {
			if (other.Length > Length) return false;
			return _root.IsSupersetOf(other._root, null);
		}

		public FunqOrderedSet<T> AddRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.IsNull ? newRoot.FromNull(item, true, _comparer, lineage) : newRoot.AvlAdd(item, true, lineage);
			}
			return newRoot.WrapSet(_comparer);
		}

		public FunqOrderedSet<T> DropMany(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.AvlRemove(item, lineage) ?? newRoot;
			}
			return newRoot.WrapSet(_comparer);
		}

		public override bool ForEachWhile(Func<T, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return _root.ForEachWhile((k, v) => iterator(k));
		}

		public override int Length
		{
			get
			{
				return _root.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return _root.IsNull;
			}
		}

		public T MinItem
		{
			get
			{
				if (_root.IsNull) throw Errors.Is_empty;
				return _root.Min.Key;
			}
		}

		public T MaxItem
		{
			get
			{
				if (_root.IsNull) throw Errors.Is_empty;
				return _root.Max.Key;
			}
		}



		public override FunqOrderedSet<T> Union(FunqOrderedSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Union(other._root, null).WrapSet(_comparer);
		}

		public override FunqOrderedSet<T> Intersect(FunqOrderedSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Intersect(other._root, null, Lineage.Immutable).WrapSet(_comparer);
		}

		public override SetRelation RelatesTo(FunqOrderedSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Relation(other._root);
		}

		public override FunqOrderedSet<T> Except(FunqOrderedSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Except(other._root).WrapSet(_comparer);
		}

		public override FunqOrderedSet<T> Difference(FunqOrderedSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.SymDifference(other._root).WrapSet(_comparer);
		}

		public FunqOrderedSet<T> DropMax()
		{
			if (_root.IsNull) throw Errors.Is_empty;
			return _root.RemoveMax(Lineage.Mutable()).WrapSet(_comparer);
		}

		public FunqOrderedSet<T> DropMin()
		{
			if (_root.IsNull) throw Errors.Is_empty;
			return _root.RemoveMin(Lineage.Mutable()).WrapSet(_comparer);
		}

		public override bool Contains(T item)
		{
			return _root.Find(item).IsSome;
		}
	}
}
