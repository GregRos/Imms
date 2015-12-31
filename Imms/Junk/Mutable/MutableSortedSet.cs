using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Imm;

namespace Imm.Collections.Mutable
{
	public class MutableSortedSet<T> : SetLike<T, MutableSortedSet<T>>
	{
		private class Builder : SetBuilder<T>
		{
			private readonly SortedSet<T> _inner;

			public Builder() : this(new SortedSet<T>())
			{
			}

			public Builder(SortedSet<T> inner)
			{
				_inner = inner;
			}

			public override object Result
			{
				get
				{
					return _inner;
				}
			}

			public override void EnsureCapacity(int n)
			{

			}

			protected override void add(T item)
			{
				_inner.Add(item);
			}

			public override bool Contains(T item)
			{
				return _inner.Contains(item);
			}

			public override void Remove(T item)
			{
				_inner.Remove(item);
			}
		}
		protected override SetBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		private readonly SortedSet<T> _inner;

		public MutableSortedSet(SortedSet<T> inner)
		{
			_inner = inner;
		}

		protected override SetBuilder<T> BuilderFrom(MutableSortedSet<T> provider)
		{
			return new Builder(provider._inner);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		protected override MutableSortedSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return new MutableSortedSet<T>((SortedSet<T>)builder.Result);
		}

		public override bool Contains(T item)
		{
			return _inner.Contains(item);
		}

		public override int Length
		{
			get
			{
				return _inner.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return _inner.Count == 0;
			}
		}

		
	}
}
