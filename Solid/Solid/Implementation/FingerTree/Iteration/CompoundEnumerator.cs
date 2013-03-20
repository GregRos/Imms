using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Solid.FingerTree.Iteration
{
	internal class CompoundEnumerator<T> : IEnumerator<Measured>
		where T : Measured<T>
	{
		private readonly Compound<T> tree;
		private IEnumerator<Measured> inner;
		private int index=-1;

		public CompoundEnumerator(Compound<T> tree)
		{
			tree.IsNotNull();
			this.tree = tree;
			
		}

		private bool TryNext()
		{
			index++;
			switch (index)
			{
				case 0:
					inner = tree.LeftDigit.GetEnumerator();
					inner.MoveNext();
					return true;
				case 1:
					inner = tree.DeepTree.GetEnumerator();
					if (inner.MoveNext())
						return true;
					return TryNext();
				case 2:
					inner = tree.RightDigit.GetEnumerator();
					inner.MoveNext();
					return true;
				default:
					return false;
			}
		}

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			
			if (index == -1) return TryNext();
			if (inner.MoveNext())
				return true;
			else
				return TryNext();
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public Measured Current
		{
			get
			{
				return inner.Current;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}