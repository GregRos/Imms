using System.Collections;
using System.Collections.Generic;

namespace Solid.TrieVector.Iteration
{
	internal class LeafEnumerator<T> : IEnumerator<T>
	{
		private readonly VectorLeaf<T> node;
		private int index = -1;

		public LeafEnumerator(VectorLeaf<T> node)
		{
			this.node = node;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			index++;
			return index < node.Count;
		}

		public void Reset()
		{
			index = -1;
		}

		public T Current
		{
			get
			{
				return node[index];
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