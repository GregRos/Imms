using System.Collections.Generic;
using Funq.Abstract;
using Funq.Implementation;

namespace Funq {
	public sealed partial class FunqMap<TKey, TValue> {
		protected override IMapBuilder<TKey, TValue, FunqMap<TKey, TValue>> EmptyBuilder {
			get { return new Builder(Empty(_equality)); }
		}

		protected override IMapBuilder<TKey, TValue, FunqMap<TKey, TValue>> BuilderFrom(FunqMap<TKey, TValue> collection) {
			return new Builder(collection);
		}

		protected override bool IsCompatibleWith(FunqMap<TKey, TValue> other) {
			return _equality.Equals(other._equality);
		}

		class Builder : IMapBuilder<TKey, TValue, FunqMap<TKey, TValue>> {
			readonly IEqualityComparer<TKey> _equality;
			HashedAvlTree<TKey, TValue>.Node _inner;
			Lineage _lineage;

			Builder(HashedAvlTree<TKey, TValue>.Node node, IEqualityComparer<TKey> equality) {
				_inner = node;
				_equality = equality;

				_lineage = Lineage.Mutable();
			}

			public Builder(FunqMap<TKey, TValue> map)
				: this(map._root, map._equality) {}

			public FunqMap<TKey, TValue> Produce() {
				_lineage = Lineage.Mutable();
				return _inner.WrapMap(_equality);
			}

			public bool Add(KeyValuePair<TKey, TValue> item) {
				_inner = _inner.Root_Add(item.Key, item.Value, _lineage, _equality, true) ?? _inner;
				return true;
			}

			public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
				items.CheckNotNull("items");
				FunqMap<TKey, TValue> map = items as FunqMap<TKey, TValue>;
				if (map != null && _equality.Equals(map._equality)) {
					_inner = _inner.Union(map._root, _lineage);
				} else {
					items.ForEach(x => Add(x));
				}
			}

			public int Length {
				get { return _inner.Count; }
			}

			public Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
				return _inner.Root_FindKvp(key);
			}

			public bool Remove(TKey key) {
				var ret = _inner.Root_Remove(key, _lineage);
				if (ret == null) {
					return false;
				} else {
					_inner = ret;
					return true;
				}
			}

			public Optional<TValue> TryGet(TKey k) {
				return _inner.Root_Find(k);
			}

			public void Dispose() {
				_lineage = Lineage.Mutable();
			}
		}
	}
}