using System.Collections.Generic;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqOrderedSet<T>
	{
		internal sealed class Builder : SetBuilder<T>
		{
			readonly IComparer<T> _comparer;
			private OrderedAvlTree<T, bool>.Node _inner;
			Lineage _lineage;
			public override object Result
			{
				get {
					_lineage = Lineage.Mutable();
					return _inner.Wrap(_comparer);
				}
			}

			public Builder(IComparer<T> comparer, OrderedAvlTree<T, bool>.Node inner)
			{
				_comparer = comparer;
				_inner = inner;
				_lineage = Lineage.Mutable();
			}


			protected override void add(T item) {
				_inner = _inner.Root_Add(item, true, _comparer, false, _lineage) ?? _inner;
			}

			public override bool Contains(T item) {
				return _inner.Contains(item);
			}

			public override void Remove(T item) {
				bool dummy;
				_inner = _inner.AvlRemove(item, _lineage) ?? _inner;
			}
		}

		protected internal override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder(Comparer, Root);
			}
		}

		protected internal override FunqOrderedSet<T> ProviderFrom(SetBuilder<T> builder) {
			return (FunqOrderedSet<T>) builder.Result;
		}

		protected internal override SetBuilder<T> BuilderFrom(FunqOrderedSet<T> provider)
		{
			return new Builder(Comparer, Root);
		}

		protected override bool IsCompatibleWith(FunqOrderedSet<T> other)
		{
			return this.Comparer.Equals(other.Comparer);
		}
	}
}
