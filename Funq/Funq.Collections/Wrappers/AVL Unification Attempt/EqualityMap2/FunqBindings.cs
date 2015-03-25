using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public sealed partial class FunqMap2<TKey, TValue>
	{
		internal class Builder : MapBuilder<TKey, TValue> {
			private FunqMap2<TKey, TValue> _inner;
			private readonly IEqualityComparer<TKey> _equality; 
			private readonly Lineage _lineage;
			public override object Result
			{
				get {
					return _inner;
				}
			}

			public Builder(FunqMap2<TKey, TValue> inner)
			{
				_inner = inner;
				_equality = inner._eq;

				_lineage = Lineage.Mutable();
			}

			public override void EnsureCapacity(int n)
			{
				
			}

			protected override void add(Kvp<TKey, TValue> item) {
				_inner = _inner.Add(item.Key, item.Value);
			}

			public override Option<TValue> Lookup(TKey k) {
				return _inner.TryGet(k);
			}
		}

		protected internal override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(_eq));
			}
		}

		protected internal override FunqMap2<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return (FunqMap2<TKey, TValue>) builder.Result;
		}

		protected internal override MapBuilder<TKey, TValue> BuilderFrom(FunqMap2<TKey, TValue> provider)
		{
			return new Builder(provider);
		}
	}
}
