using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqList<T>
	{
		internal class Builder : IterableBuilder<T>
		{
			private FingerTree<T>.FTree<Leaf<T>> _inner;
			private Lineage _lineage;

			public Builder()
				: this(FunqList<T>.Empty)
			{
			}

			public Builder(FunqList<T> inner)
			{
				this._inner = inner._root;
				_lineage = Lineage.Mutable();
			}


			public override object Result
			{
				get
				{
					_lineage = Lineage.Mutable();
					return _inner.Wrap();
				}
			}

			protected override void add(T item)
			{
				_inner = _inner.AddLast(item, _lineage);
			}
		}

		protected internal override IterableBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		protected internal override IterableBuilder<T> BuilderFrom(FunqList<T> provider)
		{
			return new Builder(provider);
		}

		protected internal override FunqList<T> ProviderFrom(IterableBuilder<T> builder)
		{
			return (FunqList<T>) builder.Result;
		}

		protected override T GetItem(int index)
		{
			return _root[index];
		}
	}
}
