using System.Collections;
using System.Collections.Generic;

namespace Solid.TrieMap.Iteration
{
	internal sealed class ParentIterator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private int index;
		private IEnumerator<KeyValuePair<TKey, TValue>> currentIter;
		private KeyValuePair<TKey, TValue> curValue;
		private readonly MapParent<TKey, TValue> root;

		public ParentIterator(MapParent<TKey, TValue> root)
		{
			this.root = root;
		}

		public void Dispose()
		{
			
		}

		private bool TryNext()
		{
			index++;
			if (index < root.Arr.Length)
			{
				currentIter = root.Arr[index].GetEnumerator();
				currentIter.MoveNext();
				curValue = currentIter.Current;
				return true;
			}
			return false;
		}

		public bool MoveNext()
		{
			if (currentIter.MoveNext())
			{
				return true;
			}
			return TryNext();

		}

		public void Reset()
		{
			index = -1;
			currentIter = null;
			curValue = default(KeyValuePair<TKey,TValue>);
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				return curValue;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}