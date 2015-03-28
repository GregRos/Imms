﻿using System;
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
	public partial class FunqVector<T>
	{
		internal class Builder : IterableBuilder<T>
		{
			private TrieVector<T>.Node inner;
			private readonly Lineage lineage;

			public Builder()
				: this(FunqVector<T>.Empty)
			{
			}

			public Builder(FunqVector<T> inner)
			{
				this.inner = inner.root;
				lineage = Lineage.Mutable();
			}

		

			public override object Result
			{
				get
				{
					return new FunqVector<T>(inner);
				}
			}

			protected override void add(T item)
			{
				inner = inner.Add(item, lineage);
			}

		}

		protected internal override IterableBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		protected internal override FunqVector<T> ProviderFrom(IterableBuilder<T> builder)
		{
			return (FunqVector<T>)builder.Result;
		}

		protected internal override IterableBuilder<T> BuilderFrom(FunqVector<T> provider)
		{
			return new Builder(provider);
		}

		/// <summary>
		///   Gets the value of the item with the specified index. O(logn); immediate.
		/// </summary>
		/// <param name="index"> The index of the item to get. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		protected override T GetItem(int index)
		{
			index = index < 0 ? index + Length : index;
			if (index >= Length || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			return root[index];
			
		}
	}
}
