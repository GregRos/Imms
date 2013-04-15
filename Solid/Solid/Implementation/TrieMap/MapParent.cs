using System;
using System.Collections.Generic;
using Solid.TrieMap.Iteration;

namespace Solid.TrieMap
{
	internal sealed class MapParent<TKey, TValue> : MapNode<TKey, TValue>
	{
		private readonly int myBlock;
		private readonly int myCount;
		public readonly MapNode<TKey, TValue>[] Arr;
		public readonly uint Population;

		public MapParent(int height, int count, MapNode<TKey, TValue>[] arr, uint population) :
			base(height, count, NodeType.Parent)
		{
			Arr = arr;
			myCount = (25 - Height * 5);
			Population = population;
			myBlock = ((1 << 5) - 1) << myCount;
		}

		internal MapParent(int height, HashedKey<TKey> key1, TValue value1, HashedKey<TKey> key2, TValue value2)
			: base(height, 2, NodeType.Parent)
		{
			myCount = (25 - Height * 5);
			myBlock = ((1 << 5) - 1) << myCount;

			var localKey1 = ((key1.Hash & myBlock) >> myCount);
			var localKey2 = ((key2.Hash & myBlock) >> myCount);
			if (localKey1 == localKey2)
			{
				var subParent = new MapParent<TKey, TValue>(height + 1, key1, value1, key2, value2);
				var arr = new MapNode<TKey, TValue>[1];
				arr[0] = subParent;
				Population = 1u << localKey1;
				Arr = arr;
			}
			else
			{
				Arr = new MapNode<TKey, TValue>[2];
				MapNode<TKey, TValue> node1, node2;
				if (localKey1 < localKey2)
				{
					node1 = new MapLeaf<TKey, TValue>(Height + 1, key1, value1);
					node2 = new MapLeaf<TKey, TValue>(Height + 1, key2, value2);
				}
				else
				{
					node2 = new MapLeaf<TKey, TValue>(Height + 1, key1, value1);
					node1 = new MapLeaf<TKey, TValue>(Height + 1, key2, value2);
				}
				Population = ((1u << localKey1) | (1u << localKey2));
				Arr[0] = node1;
				Arr[1] = node2;
			}
		}

		public override MapNode<TKey, TValue2> Apply<TValue2>(Func<TKey, TValue, TValue2> transform)
		{
			var newArr = new MapNode<TKey, TValue2>[Arr.Length];
			for (var i = 0; i < Arr.Length; i++)
			{
				newArr[i] = Arr[i].Apply(transform);
			}
			return new MapParent<TKey, TValue2>(Height, Count, newArr, Population);
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new ParentIterator<TKey, TValue>(this);
		}

		public override void Iter(Action<TKey, TValue> action)
		{
			for (var i = 0; i < Arr.Length; i++)
			{
				Arr[i].Iter(action);
			}
		}

		private uint PopCount(uint bitmap)
		{
			unchecked
			{
				bitmap = bitmap - ((bitmap >> 1) & 0x55555555u);
				bitmap = ((bitmap >> 2) & 0x33333333u) + (bitmap & 0x33333333u);
				bitmap = (((bitmap + (bitmap >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
				return bitmap;
			}
		}

		public override bool TryContains(HashedKey<TKey> tryKey, ref Result result)
		{
			var localKey = (((tryKey.Hash & myBlock) >> myCount));
			if ((Population & (1u << localKey)) == 0u)
			{
				result = Result.Success;
				return false;
			}
			var population = Population;
			unchecked
			{
				population = (population & ((1u << localKey) - 1));
				population = population - ((population >> 1) & 0x55555555u);
				population = ((population >> 2) & 0x33333333u) + (population & 0x33333333u);
				population = (((population + (population >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
			}
			return Arr[population].TryContains(tryKey, ref result);
		}

		public override MapNode<TKey, TValue> TryDrop(HashedKey<TKey> tryKey, ref Result result)
		{
			var localKey = (((tryKey.Hash & myBlock) >> myCount));
			if ((Population & (1u << localKey)) == 0u)
			{
				result = Result.KeyNotFound;
				return null;
			}
			var population = Population;
			unchecked
			{
				population = (population & ((1u << localKey) - 1));
				population = population - ((population >> 1) & 0x55555555u);
				population = ((population >> 2) & 0x33333333u) + (population & 0x33333333u);
				population = (((population + (population >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
			}
			var newNode = Arr[population].TryDrop(tryKey, ref result);
			MapNode<TKey, TValue>[] myCopy;
			switch (result)
			{
				case Result.Success:
					myCopy = new MapNode<TKey, TValue>[Arr.Length];
					Arr.CopyTo(myCopy, 0);
					myCopy[population] = newNode;
					return new MapParent<TKey, TValue>(Height, Count - 1, myCopy, Population);
				case Result.TurnedEmpty:
					if (Arr.Length == 1)
					{
						result = Result.TurnedEmpty;
						return null;
					}
					myCopy = new MapNode<TKey, TValue>[Arr.Length - 1];
					Array.Copy(Arr, 0, myCopy, 0, population);
					Array.Copy(Arr, population + 1, myCopy, population, myCopy.Length - population);
					var newPop = (Population & ~((1u << localKey) - 1));
					result = Result.Success;
					return new MapParent<TKey, TValue>(Height, Count - 1, myCopy, newPop);
				default:
					return null;
			}
		}

		public override TValue TryGet(HashedKey<TKey> tryKey, ref Result result)
		{
			var localKey = ((tryKey.Hash & myBlock) >> myCount);
			if ((Population & (1u << localKey)) == 0u)
			{
				result = Result.KeyNotFound;
				return default(TValue);
			}

			var population = Population;
			unchecked
			{
				//This code block has been inlined for performance reasons. It counts the set bits.
				population = (population & ((1u << localKey) - 1));
				population = population - ((population >> 1) & 0x55555555u);
				population = ((population >> 2) & 0x33333333u) + (population & 0x33333333u);
				population = (((population + (population >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
			}
			return Arr[population].TryGet(tryKey, ref result);
		}

		public override MapNode<TKey, TValue> TrySet(HashedKey<TKey> tryKey, TValue tryValue, WriteBehavior behave,
		                                             ref Result result)
		{
			var localKey = (((tryKey.Hash & myBlock) >> myCount));

			MapNode<TKey, TValue> newNode;
			if ((Population & (1u << localKey)) == 0u)
			{
				if (behave == WriteBehavior.OnlyOverwrite)
				{
					result = Result.KeyNotFound;
					return null;
				}
				newNode = new MapLeaf<TKey, TValue>(Height + 1, tryKey, tryValue);
				var population = Population;
				unchecked
				{
					population = (population & ((1u << localKey) - 1));
					population = population - ((population >> 1) & 0x55555555u);
					population = ((population >> 2) & 0x33333333u) + (population & 0x33333333u);
					population = (((population + (population >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
				}
				var newArr = new MapNode<TKey, TValue>[Arr.Length + 1];
				Array.Copy(Arr, 0, newArr, 0, population);
				Array.Copy(Arr, population, newArr, population + 1, Arr.Length - population);
				newArr[population] = newNode;
				var newPop = (Population | ((1u << localKey)));
				result = Result.Success;
				return new MapParent<TKey, TValue>(Height, Count + 1, newArr, newPop);
			}
			var population1 = Population;
			unchecked
			{
				population1 = (population1 & ((1u << localKey) - 1));
				population1 = population1 - ((population1 >> 1) & 0x55555555u);
				population1 = ((population1 >> 2) & 0x33333333u) + (population1 & 0x33333333u);
				population1 = (((population1 + (population1 >> 4)) & 0xF0F0F0Fu) * 0x1010101u) >> 24;
			}
			var oldNode = Arr[population1];
			newNode = oldNode.TrySet(tryKey, tryValue, behave, ref result);
			switch (result)
			{
				case Result.Success:
					var newArr = new MapNode<TKey, TValue>[Arr.Length];
					Arr.CopyTo(newArr, 0);
					newArr[population1] = newNode;
					return new MapParent<TKey, TValue>(Height, Count - oldNode.Count + newNode.Count, newArr, Population);
				default:
					return null;
			}
		}

		internal MapParent<TKey, TValue> UnionInPlace(MapParent<TKey, TValue> other,
		                                              List<KeyValuePair<TKey, TValue>> collisions)
		{
			throw new NotImplementedException();
		}
	}
}