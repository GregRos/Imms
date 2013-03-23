using System;
using System.Collections.Generic;

namespace Solid.TrieMap
{
	internal abstract class MapNode<TKey, TValue>
	{
		protected const int MaxHeight = 6;
		public readonly int Count;
		public readonly int Height;
		public readonly int Kind;
		private static readonly MapNode<TKey, TValue> empty = new MapEmpty<TKey, TValue>();

		protected MapNode(int height, int count, int kind)
		{
			Height = height;
			Count = count;
			Kind = kind;
		}

		public abstract MapNode<TKey, TValue2> Apply<TValue2>(Func<TKey, TValue, TValue2> transform);

		public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

		public abstract void Iter(Action<TKey, TValue> action);

		public abstract bool TryContains(HashedKey<TKey> tryKey, out Result result1);

		public abstract MapNode<TKey, TValue> TryDrop(HashedKey<TKey> hashedKey, out Result result1);

		/// <summary>
		/// Tries to get a Content from the map using a hashed key.
		/// </summary>
		/// <param name="tryKey">The key.</param>
		/// <param name="result">An output parameter that returns the status code of the operation.</param>
		/// <returns>The Content, or null if no key was found.</returns>
		public abstract TValue TryGet(HashedKey<TKey> tryKey, out Result result);

		/// <summary>
		/// Tries to associate a key
		/// </summary>
		/// <param name="tryKey"></param>
		/// <param name="tryValue"></param>
		/// <param name="writeBehavior"></param>
		/// <param name="result1"></param>
		/// <returns></returns>
		public abstract MapNode<TKey, TValue> TrySet(HashedKey<TKey> tryKey, TValue tryValue, WriteBehavior writeBehavior,
		                                             out Result result1);

		public MapNode<TKey, TValue> Union(MapNode<TKey, TValue> one, MapNode<TKey, TValue> other,
		                                   List<KeyValuePair<TKey, TValue>> collisions)
		{
			int code = one.Kind << 3 | other.Kind;

			MapNode<TKey, TValue> output;
			Result outcome;
			switch (code)
			{
				case NodeType.Empty << 3 | NodeType.Parent:
				case NodeType.Empty << 3 | NodeType.Leaf:
				case NodeType.Empty << 3 | NodeType.Empty:
					return other;
				case NodeType.Parent << 3 | NodeType.Empty:
				case NodeType.Leaf << 3 | NodeType.Empty:
					return one;
				case NodeType.Leaf << 3 | NodeType.Parent:
					var oneAsLeaf = one as MapLeaf<TKey, TValue>;
					output = other.TrySet(oneAsLeaf.MyKey, oneAsLeaf.MyValue, WriteBehavior.OnlyCreate, out outcome);
					switch (outcome)
					{
						case Result.HashCollision:
							collisions.Add(new KeyValuePair<TKey, TValue>(oneAsLeaf.MyKey.Key, oneAsLeaf.MyValue));
							return other;
						case Result.KeyExists:
							return other;
						case Result.Success:
							return output;
						default:
							throw Invalid_execution_path;
					}

				case NodeType.Parent << 3 | NodeType.Leaf:
					var twoAsLeaf = other as MapLeaf<TKey, TValue>;
					output = one.TrySet(twoAsLeaf.MyKey, twoAsLeaf.MyValue, WriteBehavior.Any, out outcome);
					switch (outcome)
					{
						case Result.HashCollision:
							collisions.Add(new KeyValuePair<TKey, TValue>(twoAsLeaf.MyKey.Key, twoAsLeaf.MyValue));
							return one;
						case Result.Success:
							return output;
						default:
							throw Invalid_execution_path;
					}

				case NodeType.Parent << 3 | NodeType.Parent:
					var parent1 = one as MapParent<TKey, TValue>;
					var parent2 = other as MapParent<TKey, TValue>;
					MapParent<TKey, TValue> result = parent1.UnionInPlace(parent2, collisions);
					return result;
				default:
					throw Invalid_execution_path;
			}
		}

		public static MapNode<TKey, TValue> Empty
		{
			get
			{
				return empty;
			}
		}

		private static InvalidOperationException Invalid_execution_path
		{
			get
			{
				return new InvalidOperationException("Invalid");
			}
		}

		public static MapNode<TKey, TValue> Intersect(MapNode<TKey, TValue> one, MapNode<TKey, TValue> other,
		                                              List<KeyValuePair<TKey, TValue>> collisions)
		{
			throw new NotImplementedException();
		}
	}
}