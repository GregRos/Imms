using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public sealed partial class FunqMap<TKey, TValue>
	{
		internal class Builder : MapBuilder<TKey, TValue>
		{
			private HashedAvlTree<TKey, TValue>.Node _inner;
			private readonly IEqualityComparer<TKey> _equality; 
			private readonly Lineage _lineage;
			public override object Result
			{
				get
				{
					return _inner.WrapMap(_equality);
				}
			}

			public Builder(HashedAvlTree<TKey, TValue>.Node node, IEqualityComparer<TKey> equality)
			{
				_inner = node;
				_equality = equality;

				_lineage = Lineage.Mutable();
			}

			public Builder(FunqMap<TKey, TValue> map)
				: this(map._root, map._equality)
			{
				
			}

			public override void EnsureCapacity(int n)
			{
				
			}

			protected override void add(Kvp<TKey, TValue> item)
			{
				_inner = _inner.AvlAdd(_equality.WrapKey(item.Key), item.Value, _lineage);
			}

			public override Option<TValue> Lookup(TKey k)
			{
				return _inner.Find(_equality.WrapKey(k));
			}
		}

		protected override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(_equality));
			}
		}

		protected override FunqMap<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return (FunqMap<TKey, TValue>) builder.Result;
		}

		protected override MapBuilder<TKey, TValue> BuilderFrom(FunqMap<TKey, TValue> provider)
		{
			return new Builder(provider);
		}
	}
}
