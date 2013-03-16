using System;
using System.Collections;
using System.Collections.Generic;

namespace Solid.FingerTree
{
	internal class EnumeratorWrapper<T> : IEnumerator<T>,IEnumerable<T>
	{
		private readonly IEnumerator<Measured> inner;
		private T current;

		public EnumeratorWrapper(IEnumerator<Measured> inner)
		{
			this.inner = inner;
		}

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{

			var ret = inner.MoveNext();
			if (ret)
			{
				var tmp = inner.Current as Value<T>;
				current = tmp.Content;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public T Current
		{
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new EnumeratorWrapper<T>(inner);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}