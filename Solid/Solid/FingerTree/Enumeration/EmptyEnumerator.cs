using System;
using System.Collections;
using System.Collections.Generic;

namespace Solid.FingerTree
{
	internal class EmptyEnumerator<T> : IEnumerator<Measured>,IEnumerable<Measured>
	{
		private static readonly EmptyEnumerator<T> empty = new EmptyEnumerator<T>();

		private EmptyEnumerator()
		{
		}

		public static IEnumerator<Measured> Instance
		{
			get
			{
				return empty;
			}
		}

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public Measured Current
		{
			get { throw new NotSupportedException(); }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public IEnumerator<Measured> GetEnumerator()
		{
			return empty;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}