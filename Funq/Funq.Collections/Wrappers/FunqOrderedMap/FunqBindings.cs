using System.Collections.Generic;
using System.ComponentModel;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqOrderedMap<TKey, TValue>
	{
		internal class Builder : MapBuilder<TKey, TValue>
		{
			private OrderedAvlTree<TKey, TValue>.Node _inner;
			private Lineage _lineage;
			private readonly IComparer<TKey> _comparer; 
			public Builder(OrderedAvlTree<TKey, TValue>.Node inner, IComparer<TKey> comparer )
			{
				_inner = inner;
				_comparer = comparer;
				_lineage = Lineage.Mutable();
			}

			public Builder(FunqOrderedMap<TKey, TValue> inner)
				: this(inner.Root, inner.Comparer)
			{
			
			}
			public override object Result
			{
				get {
					_lineage = Lineage.Mutable();
					return _inner.WrapMap(_comparer);
				}
			}
			protected override void add(KeyValuePair<TKey, TValue> item) {
				_inner = _inner.Root_Add(item.Key, item.Value, _comparer, true, _lineage) ?? _inner;
			}

			public override void Remove(TKey key) {
				_inner = _inner.AvlRemove(key, _lineage) ?? _inner;
			}

			public override Option<TValue> Lookup(TKey k)
			{
				return _inner.Find(k);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(Comparer));
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal override FunqOrderedMap<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return (FunqOrderedMap<TKey, TValue>)builder.Result;

		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal override MapBuilder<TKey, TValue> BuilderFrom(FunqOrderedMap<TKey, TValue> provider)
		{
			return new Builder(provider);
		}

		protected override bool IsCompatibleWith(FunqOrderedMap<TKey, TValue> other) {
			return Comparer.Equals(other.Comparer);
		}
	}
}
