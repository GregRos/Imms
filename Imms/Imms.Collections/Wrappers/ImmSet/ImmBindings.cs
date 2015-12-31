using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	public partial class ImmSet<T> {
		protected override ISetBuilder<T, ImmSet<T>> EmptyBuilder {
			get { return new Builder(EqualityComparer, HashedAvlTree<T, bool>.Node.Empty); }
		}

		protected override ISetBuilder<T, ImmSet<T>> BuilderFrom(ImmSet<T> collection) {
			return new Builder(collection.EqualityComparer, collection.Root);
		}

		protected override bool IsCompatibleWith(ImmSet<T> other) {
			return EqualityComparer.Equals(other.EqualityComparer);
		}

		sealed class Builder : ISetBuilder<T, ImmSet<T>> {
			readonly IEqualityComparer<T> _eq;
			HashedAvlTree<T, bool>.Node _inner;
			Lineage _lineage;

			public Builder(IEqualityComparer<T> eq, HashedAvlTree<T, bool>.Node inner) {
				_eq = eq;
				_inner = inner;
				_lineage = Lineage.Mutable();
			}

			public ImmSet<T> Produce() {
				//we need to change the lineage to avoid mutating user-visible data
				_lineage = Lineage.Mutable();
				return _inner.Wrap(_eq);
			}

			public bool Add(T item) {
				var ret = _inner.Root_Add(item, true, _lineage, _eq, false);
				if (ret == null) {
					return false;
				}
				_inner = ret;
				return true;
			}

			public void AddRange(IEnumerable<T> items) {
				items.CheckNotNull("items");
				var set = items as ImmSet<T>;
				if (set != null && _eq.Equals(set.EqualityComparer)) {
					_inner = _inner.Union(set.Root, _lineage);
				} else {
					items.ForEach(x => {
						_inner = _inner.Root_Add(x, true, _lineage, _eq, false) ?? _inner;
					});
				}
			}

			public int Length {
				get { return _inner.Count; }
			}

			public bool Contains(T item) {
				return _inner.Root_Contains(item);
			}

			public bool Remove(T item) {
				var res = _inner.Root_Remove(item, _lineage);
				if (res == null) {
					return false;
				}
				_inner = res;
				return true;
			}

			public void Dispose() {
				_lineage = Lineage.Mutable();
			}
		}
	}
}