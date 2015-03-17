using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	/// <summary>
	/// Container class for an hashed avl tree, which stores key-value pairs ordered by a hash value. Keys with identical hashes are placed in buckets.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal static partial class HashedAvlTree<TKey, TValue>
	{
		/// <summary>
		/// A node in the hashed AVL tree.
		/// </summary>
		internal sealed class Node
		{
			/// <summary>
			/// The singleton that represents an empty node.
			/// </summary>
			internal static readonly Node Null = new Node(true);
			/// <summary>
			/// A bucket containing key-value pairs with identical hashes.
			/// </summary>
			public Bucket Bucket;
			/// <summary>
			/// The hash of this node.
			/// </summary>
			public int Hash;
			public int Height;
			public Node Left = Null;
			public Node Right = Null;
			public readonly bool IsNull;
			public Lineage Lineage;
			public int Count;

			internal Node(bool isNull = false)
			{
				Height = 0;
				Count = 0;
				IsNull = isNull;
			}
			
			public int Factor
			{
				get
				{
					return IsNull ? 0 : Left.Height - Right.Height;
				}
			}

			internal Node(int hash, Bucket bucket, Node left, Node right, Lineage lin)
			{
				this.Hash = hash;
				this.Bucket = bucket;
				this.Left = left;
				this.Right = right;
				this.Lineage = lin;
				this.Height = Math.Max(left.Height, right.Height) + 1;
				this.Count = left.Count + right.Count + bucket.Count;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Node Construct(int hash, Bucket bucket, Node left, Node right, Lineage lin)
			{
				//return new Node(hash, bucket, left, right, lin);
				return new Node()
					   {
						   Bucket = bucket,
						   Hash = hash,
						   Left = left,
						   Right = right,
						   Lineage = lin,
						   Count = left.Count + right.Count + bucket.Count,
						   Height = Math.Max(left.Height, right.Height) + 1
					   };
			}

			public static Node NewForKvp(EquatableKey<TKey> k, TValue v, Lineage lineage)
			{
				return Construct(k.Hash, Bucket.FromKvp(k, v, lineage), Null, Null, lineage);
			}


			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Node WithChildren(Node node, Node left, Node right, Lineage lineage)
			{
				if (node.Lineage.AllowMutation(lineage))
				{
					node.Left = left;
					node.Right = right;
					node.Height = Math.Max(left.Height, right.Height) + 1;
					node.Lineage = lineage;
					node.Count = node.Bucket.Count + left.Count + right.Count;
				}
				else
				{
					node = Construct(node.Hash, node.Bucket, left, right, lineage);
				}
				return node;
			}

			public static Node WithBucket(Node basedOn, Bucket newBucket, Lineage lineage)
			{
				if (basedOn.Lineage.AllowMutation(lineage))
				{
					basedOn.Bucket = newBucket;
					return basedOn;
				}
				return Construct(basedOn.Hash, newBucket, basedOn.Left, basedOn.Right, lineage);
			}

			/// <summary>
			/// Creates a new tree and balances it.
			/// </summary>
			/// <param name="root"></param>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public static Node AvlBalance(Node root, Node left, Node right, Lineage lineage)
			{
				var factor = left.Height - right.Height;
				Node newRoot = null;
				var rFactor = right.IsNull ? 0 : right.Left.Height - right.Right.Height;
				var lFactor = left.IsNull ? 0 : left.Left.Height - left.Right.Height;
				switch (factor)
				{
					case -2:
						if (rFactor == 0 || rFactor == -1)
						{
							var newLeft = WithChildren(root, left, right.Left, lineage);
							newRoot = WithChildren(right, newLeft, right.Right, lineage);
						}
						else if (rFactor == 1)
						{
							var newLeft = WithChildren(root, left, right.Left.Left, lineage);
							var rootFrom = right.Left;
							var newRight = WithChildren(right, right.Left.Right, right.Right, lineage);
							newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
						}
						else throw ImplErrors.Invalid_execution_path;
						break;
					case 2:
						if (lFactor == 1 || lFactor == 0)
						{
							var newRight = WithChildren(root, left.Right, right, lineage);
							newRoot = WithChildren(left, left.Left, newRight, lineage);
						}
						else if (lFactor == -1)
						{
							var newRight = WithChildren(root, left.Right.Right, right, lineage);
							var rootFrom = left.Right;
							var newLeft = WithChildren(left, left.Left, left.Right.Left, lineage);
							newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
						}
						else throw ImplErrors.Invalid_execution_path;
						break;

					case 0:
					case 1:
					case -1:
						newRoot = WithChildren(root, left, right, lineage);
						break;
					default:
						throw ImplErrors.Invalid_execution_path;
				}
#if DEBUG
				newRoot.IsBalanced.IsTrue();
#endif

				return newRoot;
			}

			internal int MaxPossibleHeight
			{
				get
				{
					return this.IsNull ? 0 : (int) Math.Ceiling(2 * Math.Log(Count, 2f));
				}
			}
			public Node AvlAdd(EquatableKey<TKey> key, TValue value, Lineage lineage)
			{
				var initialCount = this.Count;
				if (this.IsNull)
				{
					return NewForKvp(key, value, lineage);
				}
				Node ret;
				var r = key.Hash.CompareTo(Hash);
				/*if (r < 0)
				{
					var newLeft = Left.AvlAdd(key, value, lineage);
					return AvlBalance(this, newLeft, Right, lineage);
				}
				else if (r == 0)
				{
					var newBucket = Bucket.Add(key, value, lineage);
					return WithBucket(this, newBucket, lineage);
				}
				else
				{
					var newRight = Right.AvlAdd(key, value, lineage);
					return AvlBalance(this, Left, newRight, lineage);
				}*/
				switch (key.Hash > Hash ? Cmp.Greater : key.Hash == Hash ? Cmp.Equal : Cmp.Lesser)
				{
					case Cmp.Lesser:
						var newLeft = Left.AvlAdd(key, value, lineage);
						ret =  AvlBalance(this, newLeft, Right, lineage);
						break;
					case Cmp.Equal:
						var newBucket = Bucket.Add(key, value, lineage);
						ret =  WithBucket(this, newBucket, lineage);
						break;
					case Cmp.Greater:
						var newRight = Right.AvlAdd(key, value, lineage);
						ret =  AvlBalance(this, Left, newRight, lineage);
						break;
					default:
						throw ImplErrors.Invalid_execution_path;
				}
#if DEBUG
				ret.Count.Is(x => x <= initialCount + 1 && x >= initialCount);
				ret.IsBalanced.IsTrue();
#endif
				return ret;
			}

			public Node ExtractMin(out Node min, Lineage lineage)
			{
				if (this.IsNull) throw Errors.Is_empty;
				var initialCount = this.Count;
				Node ret;
				if (!Left.IsNull)
				{
					 ret = AvlBalance(this, Left.ExtractMin(out min, lineage), Right, lineage);
				}
				else
				{
					min = this;
					ret = Right;
				}
#if DEBUG
				ret.IsBalanced.IsTrue();
				AssertEx.IsTrue(ret.Count + min.Count == initialCount);
#endif
				return ret;
			}

			public IEnumerable<Kvp<TKey, TValue>> Pairs
			{
				get
				{
					foreach (var bucket in Buckets)
						foreach (var item in bucket.Buckets)
						{
							yield return Kvp.Of(item.Key.Key, item.Value);
						}
				}
			}

			public IEnumerable<Bucket> Buckets
			{
				get
				{
					var walker = new TreeIterator(this);
					while (walker.MoveNext())
					{
						yield return walker.Current.Bucket;
					}
				}

			}

			public override string ToString()
			{
				return string.Join(", ", Pairs);
			}

			public Node UnionNode(Node other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision)
			{
				if (this.IsNull && other.IsNull) return Null;
				else if (this.IsNull)
				{
					return Construct(other.Hash, other.Bucket, Null, Null, lineage);
				}
				switch (other.Hash > Hash ? Cmp.Greater : other.Hash == Hash ? Cmp.Equal : Cmp.Lesser)
				{
					case Cmp.Lesser:
						var newLeft = Left.UnionNode(other, lineage, collision);
						return AvlBalance(this, newLeft, Right, lineage);
					case Cmp.Equal:
						var newBucket = Bucket.Union(other.Bucket, collision);
						return WithBucket(this, newBucket, lineage);
					case Cmp.Greater:
						var newRight = Right.UnionNode(other, lineage, collision);
						return AvlBalance(this, Left, newRight, lineage);
					default:
						throw ImplErrors.Invalid_execution_path;
				}
			}

			public static Node Concat(Node leftBranch, Bucket pivot, Node rightBranch)
			{
				var newFactor = leftBranch.Height - rightBranch.Height;
				Node balanced;
				if (newFactor >= -1 && newFactor <= 1)
				{
					balanced =  Construct(pivot.Key.Hash, pivot, leftBranch, rightBranch, Lineage.Immutable);
				}
				else if (newFactor >= 2)
				{
					var newRight = Concat(leftBranch.Right, pivot, rightBranch);
					balanced =  AvlBalance(leftBranch, leftBranch.Left, newRight, Lineage.Immutable);
				}
				else
				{
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left);
					balanced =  AvlBalance(rightBranch, newLeft, rightBranch.Right, Lineage.Immutable);
				}
#if DEBUG
				AssertEx.IsTrue(balanced.Count == 1 + leftBranch.Count + rightBranch.Count);
				AssertEx.IsTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			public void Split(int pivot, out Node leftBranch, out Bucket central, out Node rightBranch)
			{
				if (this.IsNull)
				{
					leftBranch = this;
					rightBranch = this;
					central = null;
				}
				else if (pivot > this.Hash)
				{
					Node right_l, right_r;
					this.Right.Split(pivot, out right_l, out central, out right_r);
					leftBranch = Concat(this.Left, this.Bucket, right_l);
					rightBranch = right_r;
				}
				else if (pivot == this.Hash)
				{
					leftBranch = this.Left;
					central = this.Bucket;
					rightBranch = this.Right;
				}
				else
				{
					Node left_l, left_r;
					this.Left.Split(pivot, out left_l, out central, out left_r);
					leftBranch = left_l;
					rightBranch = Concat(left_r, this.Bucket, this.Right);
				}
#if DEBUG
				var totalCount = leftBranch.Count + rightBranch.Count + (central == null ? 0 : 1);
				totalCount.Is(this.Count);
#endif
			}

			public bool IsBalanced
			{
				get
				{
					return (this.IsNull ? 0 : this.Left.Height - this.Right.Height) >= -1 && (this.IsNull ? 0 : this.Left.Height - this.Right.Height) <= 1;
				}
			}

			public static Node Concat(Node left, Node right)
			{
				if (left.IsNull) return right;
				if (right.IsNull) return left;
				Node central;
				left = left.ExtractMax(out central, Lineage.Immutable);
				return Concat(left, central.Bucket, right);
			}

			public int CountIntersection(Node b)
			{
				var count = 0;
				foreach (var pair in this.IntersectElements(b))
				{
					count += pair.First.Bucket.CountIntersection(pair.Second.Bucket);
				}
				return count;
			}

			public SetRelation RelatesTo(Node other)
			{
				var intersectionSize = CountIntersection(other);
				SetRelation relation = 0;
				if (intersectionSize == 0) relation |= SetRelation.Disjoint;
				var containsThis = intersectionSize == this.Count;
				var containedInThis = intersectionSize == other.Count;
				if (containsThis && containedInThis) relation |= SetRelation.Equal;
				else if (containsThis) relation |= SetRelation.ProperSubsetOf;
				else if (containedInThis) relation |= SetRelation.ProperSupersetOf;
				else relation |= SetRelation.None;
				return relation;
			}

			public Node Except(Node other)
			{
				if (this.IsNull || other.IsNull) return this;
				Node this_lesser, this_greater;
				Bucket central;
				this.Split(other.Hash, out this_lesser, out central, out this_greater);
				var except_lesser = this_lesser.Except(other.Left);
				var except_greater = this_greater.Except(other.Right);
				Node ret;
				if (central == null)
				{
					ret = Concat(except_lesser, except_greater);
				}
				else
				{
					var exceptBucket = central.Except(other.Bucket);
					ret = exceptBucket.IsNull ? Concat(except_lesser, except_greater) : Concat(except_lesser, exceptBucket, except_greater);
				}
#if DEBUG
				AssertEx.IsTrue(except_lesser.Count <= this_lesser.Count);
				AssertEx.IsTrue(except_greater.Count <= this_greater.Count);
				AssertEx.IsTrue(except_greater.Count + except_lesser.Count <= this_lesser.Count + this_greater.Count);
				AssertEx.IsTrue(except_greater.IsBalanced);
#endif
				return ret;
			}

			public Node SymDifference(Node b)
			{
				return b.Union(this, null).Except(b.Intersect(this, null, Lineage.Immutable));
			}


			public Node Union(Node b, Func<TKey, TValue, TValue, TValue> collision)
			{
				if (IsNull) return b;
				if (b.IsNull) return this;
				Node a_lesser, a_greater;
				Bucket center_bucket;
				Split(b.Hash, out a_lesser, out center_bucket, out a_greater);
				center_bucket = center_bucket == null ? b.Bucket : b.Bucket.Union(center_bucket, collision);
				var unitedLeft = a_lesser.Union(b.Left, collision);

				var unitedRight = a_greater.Union(b.Right, collision);
				var concated =  Concat(unitedLeft, center_bucket, unitedRight);
#if DEBUG
				AssertEx.IsTrue(concated.Count <= Count + b.Count);
				AssertEx.IsTrue(concated.Count >= Count);
				AssertEx.IsTrue(concated.Count >= b.Count);
				AssertEx.IsTrue(concated.IsBalanced);
#endif
				return concated;
			}

			public static Node FromSortedList(List<Bucket> sorted, int startIndex, int endIndex, Lineage lineage)
			{
				if (startIndex > endIndex)
				{
					return Null;
				}
				int pivotIndex = startIndex + (endIndex - startIndex) / 2;
				Node left = FromSortedList(sorted, startIndex, pivotIndex - 1, lineage);
				Node right = FromSortedList(sorted, pivotIndex + 1, endIndex, lineage);
				var pivot = sorted[pivotIndex];
				return Construct(pivot.Key.Hash, pivot, left, right, lineage);
			}

			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other)
			{

				if (IsNull || other.IsNull) yield break;
				var a_iterator = new TreeIterator(this);
				var b_iterator = new TreeIterator(other);
				var success = a_iterator.MoveNext();
				var pivotInA = true;
				Node pivot = a_iterator.Current;
				while (!a_iterator.IsEnded || !b_iterator.IsEnded)
				{
					var trySeekIn = pivotInA ? b_iterator : a_iterator;
					success = trySeekIn.SeekGreaterThan(pivot.Hash);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (maybePivot.Hash == pivot.Hash)
					{
						yield return StructTuple.Create(pivot, maybePivot);
						pivotInA = !pivotInA;
						trySeekIn = pivotInA ? a_iterator : b_iterator;
						success = trySeekIn.MoveNext();
						if (!success) break;
						pivot = trySeekIn.Current;
					}
					else
					{
						pivot = maybePivot;
						pivotInA = !pivotInA; //If the pivot was in X, it's now in Y...
					}
				}
			}

			public Node Intersect(Node other, Func<TKey, TValue, TValue, TValue> collision, Lineage lineage)
			{

				var intersection = this.IntersectElements(other);
				var list = new List<Bucket>();
				foreach (var pair in intersection)
				{
					var newBucket = pair.First.Bucket.Intersect(pair.Second.Bucket, lineage, collision);
					if (!newBucket.IsNull) list.Add(newBucket);
				}
				return FromSortedList(list, 0, list.Count - 1, lineage);
			}

			public Node ExtractMax(out Node max, Lineage lineage)
			{
				if (this.IsNull) throw ImplErrors.Invalid_execution_path;
				if (!Right.IsNull)
				{
					return AvlBalance(this, Left, Right.ExtractMax(out max, lineage), lineage);
				}
				max = this;
				return Left;
			}

			public Option<TValue> Find(EquatableKey<TKey> key)
			{
				
				var cur = this;
				while (!cur.IsNull)
				{
					switch (key.Hash < cur.Hash ? Cmp.Lesser : key.Hash == cur.Hash ? Cmp.Equal : Cmp.Greater)
					{
						case Cmp.Lesser:
							cur = cur.Left;
							break;
						case Cmp.Equal:
							return cur.Bucket.Find(key);
						case Cmp.Greater:
							cur = cur.Right;
							break;
					}
				}
				return Option.None;
			}

			public HashedAvlTree<TKey, TValue2>.Node Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				if (this.IsNull)
				{
					return HashedAvlTree<TKey, TValue2>.Node.Null;
				}
				var defaultNull = HashedAvlTree<TKey, TValue2>.Node.Null;
				var children = (Left.IsNull ? 0 : 1) << 1 | (Right.IsNull ? 0 : 1);
				var appliedLeft = Left.IsNull ? defaultNull : Left.Apply(selector, lineage);
				var appliedBucket = Bucket.Apply(selector, lineage);
				var appliedRight = Right.IsNull ? defaultNull :  Right.Apply(selector, lineage);
				return HashedAvlTree<TKey, TValue2>.Node.Construct(Hash, appliedBucket, appliedLeft, appliedRight, lineage);
			}


			public bool ForEachWhile(Func<EquatableKey<TKey>, TValue, bool> act)
			{
				if (this.IsNull) return true;
				return Left.ForEachWhile(act) && Bucket.ForEachWhile(act) && Right.ForEachWhile(act);
			}

			public Node AvlErase(Lineage lineage)
			{
				var children = (Left.IsNull ? 0 : 1) << 1 | (Right.IsNull ? 0 : 1);
				switch (children)
				{
					case 0 << 1 | 0:
						return Null;
					case 1 << 1 | 0:
						return Left;
					case 0 << 1 | 1:
						return Right;
					case 1 << 1 | 1:
						Node rep;
						Node newRight = Right;
						Node newLeft = Left;
						if (Left.Height > Right.Height)
						{
							newRight = Right.ExtractMin(out rep, lineage);
						}
						else
						{
							newLeft = Left.ExtractMax(out rep, lineage);
						}
						return AvlBalance(rep, newLeft, newRight, lineage);
					default:
						throw ImplErrors.Invalid_execution_path;
				}
			}

			public Node AvlRemove(EquatableKey<TKey> key, Lineage lineage)
			{
				if (this.IsNull) return null;
				switch (key.Hash < Hash ? Cmp.Lesser : key.Hash == Hash ? Cmp.Equal : Cmp.Greater)
				{
					case Cmp.Lesser:
						Node newLeft = Left.AvlRemove(key, lineage);
						if (newLeft == null) return null;
						return AvlBalance(this, newLeft, Right, lineage);
					case Cmp.Greater:
						Node newRight = Right.AvlRemove(key, lineage);
						if (newRight == null) return null;
						return AvlBalance(this, Left, newRight, lineage);
					case Cmp.Equal:
						var newBucket = Bucket.Remove(key, lineage);
						if (newBucket == null) return null;
						return !newBucket.IsNull ? WithBucket(this, newBucket, lineage) : this.AvlErase(lineage);
					default:
						throw ImplErrors.Invalid_execution_path;
				}
			}
		}
	}
}
