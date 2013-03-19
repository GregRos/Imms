using System;
using System.Collections.Generic;
using System.Linq;
using Solid.FingerTree;

namespace Solid.TrieMap
{
	internal sealed class MapEmpty<TKey,TValue> : MapNode<TKey,TValue>
	{
		public MapEmpty() : base(0,0,NodeType.Empty)
		{
		}

		public override TValue TryGet(HashedKey<TKey> tryKey, out Result result)
		{
			result = Result.KeyNotFound;
			return default(TValue);
		}

		public override MapNode<TKey, TValue> TrySet(HashedKey<TKey> tryKey, TValue value, WriteBehavior behave, out Result result)
		{
			result = Result.Success;
			return new MapLeaf<TKey, TValue>(0, tryKey, value);
		}

		public override MapNode<TKey, TValue> TryDrop(HashedKey<TKey> tryKey, out Result result)
		{
			result = Result.KeyNotFound;
			return null;
		}

		public override bool TryContains(HashedKey<TKey> tryKey, out Result result)
		{
			result = Result.Success;
			return false;
		}

		public override void Iter(Action<TKey, TValue> action)
		{
			
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
		}

		public override MapNode<TKey, TValue2> Apply<TValue2>(Func<TKey, TValue, TValue2> transform)
		{
			return MapEmpty<TKey, TValue2>.Empty;
		}
	}
}