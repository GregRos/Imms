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
			private HashedAvlTree<T, bool>.Node _inner;
			private readonly Lineage _lineage;
			private readonly IEqualityComparer<T> _equality; 
			public override object Result
			{
				get
				{
					return _inner.WrapSet(_equality);
				}
			}

			public Builder(HashedAvlTree<T, bool>.Node node, IEqualityComparer<T> equality)
			{
				_equality = equality;
				_inner = node;
				_lineage = Lineage.Mutable();
			}

			public Builder(FunqSet<T> map)
				: this(map._root, map._equality)
			{

			}

			public override void EnsureCapacity(int n)
			{

			}

			protected override void add(T item)
			{
				_inner = _inner.AvlAdd(item.GetHashCode(), item, true, Lineage.Mutable());
			}

			public override bool Contains(T item)
			{
				return _inner.Root_Find(item).IsSome;
			}

			public override void Remove(T item) {
				bool dummy;
				_inner = _inner.AvlRemove(item.GetHashCode(), item, Lineage.Mutable());
			}
		}

		protected override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(_equality));
			}
		}

		protected override FunqSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return (FunqSet<T>)builder.Result;
		}

		protected override SetBuilder<T> BuilderFrom(FunqSet<T> provider)
		{
			return new Builder(provider);
		}
	}
}
