using System.Collections;
using System.Collections.Generic;

namespace Solid.TrieMap.Iteration
{
	
	internal sealed class LeafIterator<TKey,TValue> : IEnumerator<KeyValuePair<TKey,TValue>>
	{
		private readonly MapLeaf<TKey, TValue> inner;
		private bool started;

		public LeafIterator(MapLeaf<TKey, TValue> inner)
		{
			this.inner = inner;
		}

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			if (started) return false;
			started = true;
			return true;
		}

		public void Reset()
		{
			started = false;
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get { return new KeyValuePair<TKey, TValue>(inner.MyKey.Key, inner.MyValue); }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}
