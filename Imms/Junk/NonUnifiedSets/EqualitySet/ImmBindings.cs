using System.Collections.Generic;
using Imm.Abstract;
using Imm.Collections.Common;
using Imm.Collections.Implementation;

namespace Imm.Collections
{
	public partial class ImmSet<T>
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

			public Builder(ImmSet<T> map)
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

		protected override ImmSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return (ImmSet<T>)builder.Result;
		}

		protected override SetBuilder<T> BuilderFrom(ImmSet<T> provider)
		{
			return new Builder(provider);
		}
	}
}
