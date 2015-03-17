using System;
using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{


	public partial class FunqSet<T> : Trait_SetLike<T, FunqSet<T>>
	{
		private readonly HashedAvlTree<T, bool>.Node _root;
		private readonly IEqualityComparer<T> _equality;

		internal FunqSet(HashedAvlTree<T, bool>.Node root, IEqualityComparer<T> equality)
		{
			_root = root;
			_equality = equality;
		}

		public static FunqSet<T> Empty(IEqualityComparer<T> equality)
		{
			return new FunqSet<T>(HashedAvlTree<T, bool>.Node.Null, equality ?? EqualityComparer<T>.Default);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			foreach (var bucket in _root.Buckets)
				foreach (var item in bucket.Items)
				{
					yield return item.Key.Key;
				}
		}

		/// <summary>
		/// Adds the specified item to the set.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <exception cref="ArgumentException">Thrown if the specified item already exists.</exception>
		/// <returns></returns>
		public FunqSet<T> Add(T item) {
			if (this.Contains(item)) return this;
			return _root.AvlAdd(_equality.WrapKey(item), true, Lineage.Mutable()).WrapSet(_equality);
		}

		public override SetRelation RelatesTo(FunqSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.RelatesTo(other._root);
		}

		public override FunqSet<T> Union(FunqSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return other._root.Union(other._root, null).WrapSet(_equality);
		}

		public override FunqSet<T> Intersect(FunqSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return this._root.Intersect(other._root, null, Lineage.Immutable).WrapSet(_equality);
		}


		public override FunqSet<T> Except(FunqSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Except(other._root).WrapSet(_equality);
		}

		public override FunqSet<T> Difference(FunqSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.SymDifference(other._root).WrapSet(_equality);
		}

		/// <summary>
		/// Removes an existing item from the set.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if the specified element doesn't exist in set map.</exception>
		/// <returns></returns>
		public FunqSet<T> Drop(T item) {
			if (!this.Contains(item)) return this;
			var removed = _root.AvlRemove(_equality.WrapKey(item), Lineage.Mutable()) ?? _root;
			return removed.WrapSet(_equality);
		}

		/// <summary>
		/// Adds multiple items to the set. May overwrite.
		/// </summary>
		/// <param name="items">The items to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <returns></returns>
		public FunqSet<T> AddMany(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.AvlAdd(_equality.WrapKey(item), true, lineage);
			}
			return newRoot.WrapSet(_equality);
		}

		/// <summary>
		/// Removes many elements from the set.
		/// </summary>
		/// <param name="items">The elements to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if an element doesn't exist in the set.</exception>
		/// <returns></returns>
		public FunqSet<T> DropMany(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.AvlRemove(_equality.WrapKey(item), lineage) ?? newRoot;
			}
			return newRoot.WrapSet(_equality);
		}

		public override bool ForEachWhile(Func<T, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return _root.ForEachWhile((k, v) => iterator(k.Key));
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

		public override bool Contains(T item)
		{
			return _root.Find(_equality.WrapKey(item)).IsSome;
		}

	}
}
