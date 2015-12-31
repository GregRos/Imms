using System;
using System.Collections.Generic;
using Imm.Abstract;
using Imm.Collections.Common;
using Imm.Collections.Implementation;

namespace Imm.Collections
{
	public partial class ImmSet<T> : Trait_SetLike<T, ImmSet<T>>
	{
		private readonly HashedAvlTree<T, bool>.Node _root;
		private readonly IEqualityComparer<T> _equality;

		internal ImmSet(HashedAvlTree<T, bool>.Node root, IEqualityComparer<T> equality)
		{
			_root = root;
			_equality = equality;
		}

		public static ImmSet<T> Empty(IEqualityComparer<T> equality)
		{
			return new ImmSet<T>(HashedAvlTree<T, bool>.Node.Null, equality ?? EqualityComparer<T>.Default);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			foreach (var bucket in _root.Buckets)
				foreach (var item in bucket.Items)
				{
					yield return item.Key;
				}
		}

		/// <summary>
		/// Adds the specified item to the set.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <exception cref="ArgumentException">Thrown if the specified item already exists.</exception>
		/// <returns></returns>
		public ImmSet<T> Add(T item) {
			if (this.Contains(item)) return this;
			var r = _root.IsNull
				? _root.FromNull(item, true, _equality, Lineage.Mutable())
				: _root.Root_Add(item, true, Lineage.Mutable());
			return r.WrapSet(_equality);
		}

		public override SetRelation RelatesTo(ImmSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.RelatesTo(other._root);
		}

		public override ImmSet<T> Union(ImmSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return other._root.Union(other._root, null).WrapSet(_equality);
		}

		public override ImmSet<T> Intersect(ImmSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return this._root.Intersect(other._root, null, Lineage.Immutable).WrapSet(_equality);
		}


		public override ImmSet<T> Except(ImmSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Except(other._root).WrapSet(_equality);
		}

		public override ImmSet<T> Difference(ImmSet<T> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.SymDifferenceForSets(other._root).WrapSet(_equality);
		}

		public override bool IsSupersetOf(ImmSet<T> other)
		{
			return _root.IsSupersetOf(other._root);
		}

		/// <summary>
		/// Removes an existing item from the set.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if the specified element doesn't exist in set map.</exception>
		/// <returns></returns>
		public ImmSet<T> Drop(T item) {
			if (!this.Contains(item)) return this;
			var removed = _root.Root_Remove(item, Lineage.Mutable()) ?? _root;
			return removed.WrapSet(_equality);
		}



		/// <summary>
		/// Adds multiple items to the set. May overwrite.
		/// </summary>
		/// <param name="items">The items to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <returns></returns>
		public ImmSet<T> AddRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.IsNull ? newRoot.FromNull(item, true, _equality, lineage) : newRoot.Root_Add(item, true, lineage);
			}
			return newRoot.WrapSet(_equality);
		}

		/// <summary>
		/// Removes many elements from the set.
		/// </summary>
		/// <param name="items">The elements to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if an element doesn't exist in the set.</exception>
		/// <returns></returns>
		public ImmSet<T> DropRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;

			foreach (var item in items)
			{
				newRoot = newRoot.Root_Remove(item, lineage) ?? newRoot;
			}
			return newRoot.WrapSet(_equality);
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

		public override bool Contains(T item)
		{
			return _root.Root_Find(item).IsSome;
		}

	}
}
