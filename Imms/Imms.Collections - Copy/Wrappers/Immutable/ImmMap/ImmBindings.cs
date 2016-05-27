using System.Collections;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {

	public sealed partial class ImmMap<TKey, TValue>  :  IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> {
		protected override IMapBuilder<TKey, TValue, ImmMap<TKey, TValue>> EmptyBuilder {
			get { return new Builder(Empty(_equality)); }
		}

		protected override IMapBuilder<TKey, TValue, ImmMap<TKey, TValue>> BuilderFrom(ImmMap<TKey, TValue> collection) {
			return new Builder(collection);
		}

		protected override bool IsCompatibleWith(ImmMap<TKey, TValue> other) {
			return _equality.Equals(other._equality);
		}

		class Builder : IMapBuilder<TKey, TValue, ImmMap<TKey, TValue>> {
			readonly IEqualityComparer<TKey> _equality;
			HashedAvlTree<TKey, TValue>.Node _inner;
			Lineage _lineage;

			Builder(HashedAvlTree<TKey, TValue>.Node node, IEqualityComparer<TKey> equality) {
				_inner = node;
				_equality = equality;

				_lineage = GetNewLineage();
			}

			private static Lineage GetNewLineage() {

				return Lineage.Immutable;
			}

			public Builder(ImmMap<TKey, TValue> map)
				: this(map._root, map._equality) {}

			public ImmMap<TKey, TValue> Produce() {
				_lineage = GetNewLineage();
				return _inner.WrapMap(_equality);
			}

			public bool Add(KeyValuePair<TKey, TValue> item) {
				_inner = _inner.Root_Add(item.Key, item.Value, _lineage, _equality, true) ?? _inner;
				return true;
			}

			public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
				items.CheckNotNull("items");
				ImmMap<TKey, TValue> map = items as ImmMap<TKey, TValue>;
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
				_lineage = GetNewLineage();
			}
		}
	}
}