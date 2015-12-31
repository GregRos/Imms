using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Imm;
namespace Imm.Collections.Mutable
{

	public class MutableSet<T> : SetLike<T, MutableSet<T>>
	{

		private class Builder : SetBuilder<T>
		{
			private readonly HashSet<T> _inner;

			public Builder(HashSet<T> inner)
			{
				_inner = inner;
			}

			public Builder() : this(new HashSet<T>())
			{
				
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

		private readonly HashSet<T> _inner;

		private MutableSet(HashSet<T> result)
		{
			_inner = new HashSet<T>(result);
		}

		protected override SetBuilder<T> BuilderFrom(MutableSet<T> provider)
		{
			return new Builder(provider._inner);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		protected override MutableSet<T> ProviderFrom(SetBuilder<T> builder)
		{
			return new MutableSet<T>((HashSet<T>)builder.Result);
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


		public static implicit operator MutableSet<T>(HashSet<T> set)
		{
			return new MutableSet<T>(set);
		}

	}
}
