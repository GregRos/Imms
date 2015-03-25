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
				: this(map.Root, map.Equality)
			{
				
			}

			public override void EnsureCapacity(int n)
			{
				
			}

			protected override void add(Kvp<TKey, TValue> item)
			{
				_inner = _inner.AvlAdd(item.Key.GetHashCode(), item.Key, item.Value, _lineage, true);
			}

			public override Option<TValue> Lookup(TKey k)
			{
				return _inner.Root_Find(k);
			}
		}

		protected internal override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(Equality));
			}
		}

		protected internal override FunqMap<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return (FunqMap<TKey, TValue>) builder.Result;
		}

		protected internal override MapBuilder<TKey, TValue> BuilderFrom(FunqMap<TKey, TValue> provider)
		{
			return new Builder(provider);
		}
	}
}
