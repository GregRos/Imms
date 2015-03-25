using System.Collections.Generic;
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
			private readonly Lineage _lineage;
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
				get
				{
					return _inner.WrapMap(_comparer);
				}
			}

			public override void EnsureCapacity(int n)
			{
				
			}

			protected override void add(Kvp<TKey, TValue> item)
			{
				_inner = _inner.AvlAdd(item.Key, item.Value, _lineage, true);
			}

			public override Option<TValue> Lookup(TKey k)
			{
				return _inner.Find(k);
			}
		}

		protected internal override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(Comparer));
			}
		}

		protected internal override FunqOrderedMap<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return (FunqOrderedMap<TKey, TValue>)builder.Result;

		}

		protected internal override MapBuilder<TKey, TValue> BuilderFrom(FunqOrderedMap<TKey, TValue> provider)
		{
			return new Builder(provider);
		}
	}
}
