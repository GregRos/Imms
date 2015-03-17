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
			private FingerTree<T>.FTree<Leaf<T>> inner;
			private readonly Lineage lineage;

			public Builder()
				: this(FunqList<T>.Empty)
			{
			}

			public Builder(FunqList<T> inner)
			{
				this.inner = inner._root;
				lineage = Lineage.Mutable();
			}


			public override object Result
			{
				get
				{
					return inner.Wrap();
				}
			}

			protected override void add(T item)
			{
				inner = inner.AddLast(item, lineage);
			}

			public override void EnsureCapacity(int n)
			{

			}
		}

		protected override IterableBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		protected override IterableBuilder<T> BuilderFrom(FunqList<T> provider)
		{
			return new Builder(provider);
		}

		protected override FunqList<T> ProviderFrom(IterableBuilder<T> builder)
		{
			return (FunqList<T>) builder.Result;
		}

		protected override T GetItem(int index)
		{
			return _root[index];
		}
	}
}
