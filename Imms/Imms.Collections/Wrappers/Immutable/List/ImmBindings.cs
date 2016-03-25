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