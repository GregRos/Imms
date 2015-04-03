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
			readonly IEqualityComparer<T> _eq;
			private HashedAvlTree<T, bool>.Node _inner;
			Lineage _lineage;
			public override object Result
			{
				get {
					//we need to change the lineage to avoid mutating user-visible data
					_lineage = Lineage.Mutable();
					return _inner.Wrap(_eq);
				}
			}

			public Builder(IEqualityComparer<T> eq, HashedAvlTree<T, bool>.Node inner) {
				_eq = eq;
				_inner = inner;
				_lineage = Lineage.Mutable();
			}

			protected override void add(T item) {
				_inner = _inner.Root_Add(item, true, _lineage, _eq, false) ?? _inner;
			}

			public override bool Contains(T item) {
				return _inner.Root_Contains(item);
			}

			public override void Remove(T item) {
				_inner = _inner.Root_Remove(item, _lineage) ?? _inner;
			}
		}

		protected internal override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder(EqualityComparer, Root);
			}
		}

		protected internal override FunqSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return (FunqSet<T>)builder.Result;
		}

		protected internal override SetBuilder<T> BuilderFrom(FunqSet<T> provider)
		{
			return new Builder(EqualityComparer, provider.Root);
		}

		protected override bool IsCompatibleWith(FunqSet<T> other)
		{
			return EqualityComparer.Equals(other.EqualityComparer);
		}
	}
}
