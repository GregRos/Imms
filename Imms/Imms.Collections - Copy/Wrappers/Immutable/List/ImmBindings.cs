using System.Collections;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	public partial class ImmList<T> :  IList<T>, IList, IEnumerable, IEnumerable<T> {
		protected override ISequentialBuilder<T, ImmList<T>> EmptyBuilder {
			get { return new Builder(); }
		}

		protected override ISequentialBuilder<T, ImmList<T>> BuilderFrom(ImmList<T> collection) {
			return new Builder(collection);
		}

		protected override T GetItem(int index) {
			return Root[index];
		}

		/// <summary>
		/// Adds an element to the end. Identical to <see cref="AddLast"/>.
		/// </summary>
		/// <param name="left">The list to which the item will be added.</param>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, T item) {
			return left.AddLast(item);
		}
		/// <summary>
		/// Adds an element to the start Identical to <see cref="AddFirst"/>.
		/// </summary>
		/// <param name="list">The list to which the item will be added.</param>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(T item, ImmList<T> list) {
			return list.AddFirst(item);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddLastRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="left">The list to which the item swill be added.</param>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, IEnumerable<T> items) {
			return left.AddLastRange(items);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddFirstRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="list">The list to which the item swill be added.</param>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(IEnumerable<T> items, ImmList<T> list) {
			return list.AddFirstRange(items);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddLastRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="left">The list to which the item swill be added.</param>
		/// <param name="right">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, ImmList<T> right) {
			return left.AddLastRange(right);
		}

		class Builder : ISequentialBuilder<T, ImmList<T>> {
			FingerTree<T>.FTree<Leaf<T>> _inner;
			Lineage _lineage;

			public Builder()
				: this(Empty) {}

			public Builder(ImmList<T> inner) {
				_inner = inner.Root;
				_lineage = Lineage.Mutable();
			}

			public ImmList<T> Produce() {
				_lineage = Lineage.Mutable();
				return _inner.Wrap();
			}

			public bool Add(T item) {
				_inner = _inner.AddLast(item, _lineage);
				return true;
			}

			public void AddRange(IEnumerable<T> items) {
				items.CheckNotNull("items");
				var list = items as ImmList<T>;
				if (list != null) {
					_inner = _inner.AddLastList(list.Root, _lineage);
				} else {
					int len;
					var arr = items.ToArrayFast(out len);
					int i = 0;
					var tree = FingerTree<T>.FTree<Leaf<T>>.Construct(arr, ref i, len, _lineage);
					_inner = _inner.AddLastList(tree, _lineage);
				}
			}

			public int Length {
				get { return _inner.Measure; }
			}


			public void Dispose() {
				_lineage = Lineage.Mutable();
			}
		}
	}
}