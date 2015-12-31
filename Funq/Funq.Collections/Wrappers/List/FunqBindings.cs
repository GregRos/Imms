using System.Collections.Generic;
using Funq.Abstract;
using Funq.Implementation;

namespace Funq {
	public partial class FunqList<T> {
		protected override ISequentialBuilder<T, FunqList<T>> EmptyBuilder {
			get { return new Builder(); }
		}

		protected override ISequentialBuilder<T, FunqList<T>> BuilderFrom(FunqList<T> collection) {
			return new Builder(collection);
		}

		protected override T GetItem(int index) {
			return Root[index];
		}

		class Builder : ISequentialBuilder<T, FunqList<T>> {
			FingerTree<T>.FTree<Leaf<T>> _inner;
			Lineage _lineage;

			public Builder()
				: this(Empty) {}

			public Builder(FunqList<T> inner) {
				_inner = inner.Root;
				_lineage = Lineage.Mutable();
			}

			public FunqList<T> Produce() {
				_lineage = Lineage.Mutable();
				return _inner.Wrap();
			}

			public bool Add(T item) {
				_inner = _inner.AddLast(item, _lineage);
				return true;
			}

			public void AddRange(IEnumerable<T> items) {
				items.CheckNotNull("items");
				var list = items as FunqList<T>;
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