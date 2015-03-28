using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqSet<T>
	{
		internal sealed class Builder : SetBuilder<T>
		{
			private FunqSet<T> _inner;
			public override object Result
			{
				get {
					return _inner;
				}
			}

			public Builder(FunqSet<T> inner)
			{
				_inner = inner;
			}
			protected override void add(T item) {
				_inner = _inner;
			}

			public override bool Contains(T item) {
				return _inner.Contains(item);
			}

			public override void Remove(T item) {
				bool dummy;
				_inner = _inner.Drop(item);
			}
		}

		protected internal override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(_inner.Equality));
			}
		}

		protected internal override FunqSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return (FunqSet<T>)builder.Result;
		}

		protected internal override SetBuilder<T> BuilderFrom(FunqSet<T> provider)
		{
			return new Builder(provider);
		}
	}
}
