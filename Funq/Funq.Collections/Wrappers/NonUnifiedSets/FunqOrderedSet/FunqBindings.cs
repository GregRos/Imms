using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqOrderedSet<T>
	{
		internal class Builder : SetBuilder<T>
		{
			private OrderedAvlTree<T, bool>.Node _inner;
			private readonly Lineage _lineage;
			private readonly IComparer<T> _comparer; 
			public override object Result
			{
				get
				{
					return _inner.WrapSet(_comparer);
				}
			}


			public Builder(OrderedAvlTree<T, bool>.Node node, IComparer<T> comparer)
			{
				_comparer = comparer;
				_inner = node;
				_lineage = Lineage.Mutable();
			}

			public Builder(FunqOrderedSet<T> map)
				: this(map._root, map._comparer)
			{

			}

			public override void EnsureCapacity(int n)
			{

			}

			protected override void add(T item)
			{
				_inner = _inner.AvlAdd(item, true, Lineage.Mutable());
			}

			public override bool Contains(T item)
			{
				return _inner.Find(item).IsSome;
			}

			public override void Remove(T item)
			{
				_inner = _inner.AvlRemove(item, Lineage.Mutable());
			}
		}

		protected override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder(Empty(_comparer));
			}
		}

		protected override FunqOrderedSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return (FunqOrderedSet<T>)builder.Result;
		}

		protected override SetBuilder<T> BuilderFrom(FunqOrderedSet<T> provider)
		{
			return new Builder(provider);
		}
	}
}
