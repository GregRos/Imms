using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid.TrieMap
{
	internal sealed class MapEmpty<TKey, TValue> : MapNode<TKey, TValue>
	{
		public MapEmpty() : base(0, 0, NodeType.Empty)
		{
		}

		public override MapNode<TKey, TValue2> Apply<TValue2>(Func<TKey, TValue, TValue2> transform)
		{
			return MapEmpty<TKey, TValue2>.Empty;
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
		}

		public override void Iter(Action<TKey, TValue> action)
		{
		}

		public override bool TryContains(HashedKey<TKey> tryKey, ref Result result)
		{
			result = Result.Success;
			return false;
		}

		public override MapNode<TKey, TValue> TryDrop(HashedKey<TKey> tryKey, ref Result result)
		{
			result = Result.KeyNotFound;
			return null;
		}

		public override TValue TryGet(HashedKey<TKey> tryKey, ref Result result)
		{
			result = Result.KeyNotFound;
			return default(TValue);
		}

		public override MapNode<TKey, TValue> TrySet(HashedKey<TKey> tryKey, TValue value, WriteBehavior behave,
		                                             ref Result result)
		{
			result = Result.Success;
			return new MapLeaf<TKey, TValue>(0, tryKey, value);
		}
	}
}