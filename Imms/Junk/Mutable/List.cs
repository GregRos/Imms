using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Imm.Collections.Common;
using Imm;

namespace Imm.Collections.Mutable
{
	public sealed class MutableList<T> :  ListLike<T, MutableList<T>>
	{

		private class Builder : SeqBuilder<T>
		{
			private List<T> _inner;
 
			internal Builder()
			{
				_inner = new List<T>();
			}

			internal Builder(MutableList<T> list)
			{
				_inner = new List<T>(list._inner);
			}

			public override object Result
			{
				get
				{
					return new MutableList<T>(_inner);
				}
			}

			public override void EnsureCapacity(int n)
			{
				_inner.Capacity = n;
			}

			protected override void add(T item)
			{
				_inner.Add(item);

			}
		}

		private List<T> _inner;

		internal MutableList(List<T> list )
		{
			_inner = list;
		}

		public void AddLast(T item)
		{
			_inner.Add(item);
		}

		public void DropLast()
		{
			_inner.RemoveAt(_inner.Count - 1);
		}

		public void AddFirst(T item)
		{
			_inner.Insert(0, item);
		}

		protected override SeqBuilder<T> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		protected override MutableList<T> ProviderFrom(SeqBuilder<T> builder)
		{
			return new MutableList<T>((List<T>) builder.Result);
		}

		protected override SeqBuilder<T> BuilderFrom(MutableList<T> provider)
		{
			return new Builder(this);
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		public override bool IsEmpty
		{
			get
			{
				return _inner.Count == 0;
			}
		}

		public override T First
		{
			get
			{
				if (this.IsEmpty) throw Errors.Is_empty;
				return _inner[0];
			}
		}

		public override T Last
		{
			get
			{
				if (this.IsEmpty) throw Errors.Is_empty;
				return _inner[_inner.Count - 1];
			}
		}

		protected override T GetItem(int index)
		{
			return _inner[index];
		}



		public override bool ForEachBackWhile(Func<T, bool> iterator)
		{
			for (int i = _inner.Count; i >= 0; i--)
			{
				if (!iterator(_inner[i])) return false;
			}
			return true;
		}

		public static implicit operator MutableList<T>(List<T> l )
		{
			return new MutableList<T>(l);
		}

		public override MutableList<T> Where(Func<T, bool> predicate)
		{
			return _inner.FindAll(x => predicate(x));
		}

		public override int Length
		{
			get
			{
				return _inner.Count;
			}
		}

		public override MutableList<T> Skip(int count)
		{
			return _inner.GetRange(count, _inner.Count - count);
		}

		public override MutableList<T> Take(int count)
		{
			return _inner.GetRange(0, count);
		}

		protected override MutableList<T> GetRange(int from, int count)
		{
			return _inner.GetRange(from, count);
		}


	}
}
