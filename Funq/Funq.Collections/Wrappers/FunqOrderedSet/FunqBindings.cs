using System.Collections.Generic;
using Funq.Abstract;
using Funq.Implementation;

namespace Funq {
	public partial class FunqOrderedSet<T> {
		protected override ISetBuilder<T, FunqOrderedSet<T>> EmptyBuilder {
			get { return new Builder(Comparer, OrderedAvlTree<T, bool>.Node.Empty); }
		}

		protected override ISetBuilder<T, FunqOrderedSet<T>> BuilderFrom(FunqOrderedSet<T> collection) {
			return new Builder(Comparer, collection.Root);
		}


		protected override bool IsCompatibleWith(FunqOrderedSet<T> other) {
			return Comparer.Equals(other.Comparer);
		}

		sealed class Builder : ISetBuilder<T, FunqOrderedSet<T>> {
			readonly IComparer<T> _comparer;
			OrderedAvlTree<T, bool>.Node _inner;
			Lineage _lineage;

			public Builder(IComparer<T> comparer, OrderedAvlTree<T, bool>.Node inner) {
				_comparer = comparer;
				_inner = inner;
				_lineage = Lineage.Mutable();
			}

			public FunqOrderedSet<T> Produce() {
				_lineage = Lineage.Mutable();
				return _inner.Wrap(_comparer);
			}

			public bool Add(T item) {
				var ret = _inner.Root_Add(item, true, _comparer, false, _lineage);
				if (ret == null) {
					return false;
				}
				_inner = ret;
				return true;
			}

			public void AddRange(IEnumerable<T> items) {
				items.CheckNotNull("items");
				var set = items as FunqOrderedSet<T>;
				if (set != null && _comparer.Equals(set.Comparer)) {
					_inner = _inner.Union(set.Root, null, _lineage);
				} else {
					items.ForEach(x => Add(x));
				}
			}

			public int Length {
				get { return _inner.Count; }
			}

			public bool Contains(T item) {
				return _inner.Contains(item);
			}

			public bool Remove(T item) {
				var ret = _inner.AvlRemove(item, _lineage);
				if (ret == null) {
					return false;
				}
				_inner = ret;
				return true;
			}

			public void Dispose() {
				
			}
		}
	}
}