using System.Collections.Generic;
using System.ComponentModel;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	public partial class ImmSortedMap<TKey, TValue> {
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override IMapBuilder<TKey, TValue, ImmSortedMap<TKey, TValue>> EmptyBuilder {
			get { return new Builder(Empty(Comparer)); }
		}

		protected override bool IsCompatibleWith(ImmSortedMap<TKey, TValue> other) {
			return Comparer.Equals(other.Comparer);
		}

		class Builder : IMapBuilder<TKey, TValue, ImmSortedMap<TKey, TValue>> {
			readonly IComparer<TKey> _comparer;
			OrderedAvlTree<TKey, TValue>.Node _inner;
			Lineage _lineage;

			Builder(OrderedAvlTree<TKey, TValue>.Node inner, IComparer<TKey> comparer) {
				_inner = inner;
				_comparer = comparer;
				_lineage = Lineage.Mutable();
			}

			public Builder(ImmSortedMap<TKey, TValue> inner)
				: this(inner.Root, inner.Comparer) {}

			public ImmSortedMap<TKey, TValue> Produce() {
				_lineage = Lineage.Mutable();
				return _inner.WrapMap(_comparer);
			}

			public bool Add(KeyValuePair<TKey, TValue> item) {
				_inner = _inner.Root_Add(item.Key, item.Value, _comparer, true, _lineage) ?? _inner;
				return true;
			}

			public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
				items.CheckNotNull("items");
				var map = items as ImmSortedMap<TKey, TValue>;
				if (map != null && _comparer.Equals(map.Comparer)) {
					_inner = _inner.Union(map.Root, null, _lineage);
				} else {
					items.ForEach(x => Add(x));
				}
			}

			public int Length {
				get { return _inner.Count; }
			}

			public Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
				return _inner.FindKvp(key);
			}

			public bool Remove(TKey key) {
				var ret = _inner.AvlRemove(key, _lineage);
				if (ret == null) {
					return false;
				}
				_inner = ret;
				return true;
			}

			public Optional<TValue> TryGet(TKey k) {
				return _inner.Find(k);
			}

			public void Dispose() {
				
			}
		}
	}
}