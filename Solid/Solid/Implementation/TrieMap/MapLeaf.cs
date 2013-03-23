using System;
using System.Collections.Generic;
using Solid.TrieMap.Iteration;

namespace Solid.TrieMap
{
	/// <summary>
	/// A node associated with a key-Content pair.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal sealed class MapLeaf<TKey, TValue> : MapNode<TKey, TValue>
	{
		public readonly HashedKey<TKey> MyKey;
		public readonly TValue MyValue;

		public MapLeaf(int height, HashedKey<TKey> hashKey, TValue value)
			: base(height, 1, NodeType.Leaf)
		{
			MyKey = hashKey;
			MyValue = value;
		}

		public override MapNode<TKey, TValue2> Apply<TValue2>(Func<TKey, TValue, TValue2> transform)
		{
			return new MapLeaf<TKey, TValue2>(Height, MyKey, transform(MyKey.Key, MyValue));
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new LeafIterator<TKey, TValue>(this);
		}

		public override void Iter(Action<TKey, TValue> action)
		{
			action(MyKey.Key, MyValue);
		}

		public override bool TryContains(HashedKey<TKey> tryKey, out Result result)
		{
			if (tryKey.Hash != MyKey.Hash)
			{
				result = Result.Success;
				return false;
			}
			if (MyKey.Comparer.Equals(tryKey.Key, MyKey.Key))
			{
				result = Result.Success;
				return true;
			}
			result = Result.HashCollision;
			return false;
		}

		public override MapNode<TKey, TValue> TryDrop(HashedKey<TKey> tryKey, out Result result)
		{
			if (tryKey.Hash != MyKey.Hash)
			{
				result = Result.KeyNotFound;
				return null;
			}
			if (MyKey.Comparer.Equals(tryKey.Key, MyKey.Key))
			{
				result = Result.TurnedEmpty;
				return null;
			}
			result = Result.HashCollision;
			return null;
		}


		public override TValue TryGet(HashedKey<TKey> tryKey, out Result result)
		{
			if (tryKey.Hash != MyKey.Hash)
			{
				result = Result.KeyNotFound;
				return default(TValue);
			}
			if (MyKey.Comparer.Equals(MyKey.Key, tryKey.Key))
			{
				result = Result.Success;
				return MyValue;
			}
			result = Result.HashCollision;
			return default(TValue);
		}

		public override MapNode<TKey, TValue> TrySet(HashedKey<TKey> tryKey, TValue tryValue, WriteBehavior behave,
		                                             out Result result)
		{
			if (tryKey.Hash != MyKey.Hash && Height < MaxHeight)
			{
				if (behave == WriteBehavior.OnlyOverwrite)
				{
					result = Result.KeyNotFound;
					return null;
				}
				result = Result.Success;

				return new MapParent<TKey, TValue>(Height, MyKey, MyValue, tryKey, tryValue);
			}
			if (MyKey.Comparer.Equals(tryKey.Key, MyKey.Key))
			{
				if (behave == WriteBehavior.OnlyCreate)
				{
					result = Result.KeyExists;
					return null;
				}
				result = Result.Success;
				return new MapLeaf<TKey, TValue>(Height, tryKey, tryValue);
			}
			result = Result.HashCollision;
			return null;
		}
	}
}