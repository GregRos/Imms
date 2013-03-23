using System.Collections;
using System.Collections.Generic;

namespace Solid.TrieVector.Iteration
{
	internal class ParentEnumerator<T> : IEnumerator<T>
	{
		private readonly VectorParent<T> node;
		private IEnumerator<T> current;
		private int index = -1;

		public ParentEnumerator(VectorParent<T> node)
		{
			this.node = node;
		}

		public bool TryNext()
		{
			index++;
			if (index < node.Arr.Length)
			{
				current = node.Arr[index].GetEnumerator();
				return current.MoveNext();
			}
			return false;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index == -1)
			{
				return TryNext();
			}
			if (current.MoveNext())
			{
				return true;
			}
			return TryNext();
		}

		public void Reset()
		{
			index = -1;
		}

		public T Current
		{
			get
			{
				return current.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}
	}
}