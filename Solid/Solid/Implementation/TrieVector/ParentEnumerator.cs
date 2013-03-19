using System.Collections;
using System.Collections.Generic;

namespace Solid.TrieVector
{
	internal class ParentEnumerator<T> : IEnumerator<T>
	{
		private readonly VectorParent<T> node;
		private int index = -1;
		private IEnumerator<T> current;

		public ParentEnumerator(VectorParent<T> node)
		{
			this.node = node;
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

		public bool TryNext()
		{
			index++;
			if (index < node.Count)
			{
				current = node.Arr[index].GetEnumerator();
				return current.MoveNext();
			}
			return false;
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
			get { return Current; }
		}
	}
}