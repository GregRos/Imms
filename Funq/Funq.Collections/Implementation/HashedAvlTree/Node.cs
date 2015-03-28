using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;
using System.Linq;
namespace Funq.Collections.Implementation
{
	public static class Ext2 {
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> seq) {
			return new HashSet<T>(seq);
		}
	}
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

			public IEqualityComparer<TKey> Eq; 
			public int Height;
			public Node Left = Null;
			public Node Right = Null;
			public int NodeCount;
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
			
			internal Node(int hash, Bucket bucket, Node left, Node right, IEqualityComparer<TKey> eq  ,Lineage lin) {
				Eq = eq;
				this.Hash = hash;
				this.Bucket = bucket;
				this.Left = left;
				this.Right = right;
				this.Lineage = lin;
				this.Height = Math.Max(left.Height, right.Height) + 1;
				this.Count = left.Count + right.Count + bucket.Count;
				NodeCount = left.Count + right.Count;
			}

			public Node NewForKvp(int hash, TKey k, TValue v,  Lineage lineage)
			{
				return new Node(hash, Bucket.FromKvp(k, v, Eq, lineage), Null, Null, Eq, lineage);
			}

			public static Node WithChildren(Node node, Node left, Node right, Lineage lineage)
			{
				if (node.Lineage.AllowMutation(lineage))
				{
					node.Left = left;
					node.Right = right;
					node.Height = Math.Max(left.Height, right.Height) + 1;
					node.Lineage = lineage;
					node.Count = node.Bucket.Count + left.Count + right.Count;
					node.NodeCount = left.Count + right.Count;
				}
				else
				{
					node = new Node(node.Hash, node.Bucket, left, right, node.Eq, lineage);
				}
				return node;
			}
			
			public Node InitializeFromNull(TKey k, TValue v, IEqualityComparer<TKey> eq, Lineage lin) {
				int hash = k.GetHashCode();
				return new Node(hash, Bucket.FromKvp(k, v, eq, lin), Null, Null, eq, lin);
			}

			public Node InitializeFromNullRange(int hash, IEnumerable<KeyValuePair<TKey, TValue>> seq, IEqualityComparer<TKey> eq,
				Lineage lin) {
				var bucket = Bucket.FromRange(seq, eq, lin);
				return new Node(hash, bucket, Null, Null, eq, lin);
			}

			public Node DropRange(int hash, LinkedList<TKey> keys, Lineage lin) {
				if (IsNull) return null;
				if (hash == Hash) {
					var newBucket = Bucket.DropRange(keys, lin);
					if (newBucket == null) return null;
					if (newBucket.IsNull) return AvlErase(lin);
					return WithBucket(this, newBucket, lin);
				}
				else if (hash < Hash) {
					var newLeft = Left.DropRange(hash, keys, lin);
					if (newLeft == null) return null;
					return WithChildren(this, newLeft, Right, lin);
				}
				else {
					var newRight = Right.DropRange(hash, keys, lin);
					if (newRight == null) return null;
					return WithChildren(this, Left, newRight, lin);
				}
			}
			
			public Node Root_Add(TKey k, TValue v, Lineage lin, bool overwrite) {
				int hash = k.GetHashCode();
				if (this.IsNull) {
					throw ImplErrors.Invalid_null_invocation;
				}
				return AvlAdd(hash, k, v, lin, overwrite);
			}
			
			public Node Root_Remove(TKey k, Lineage lin) {
				int hash = k.GetHashCode();
				return AvlRemove(hash, k, lin);
			}
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Node WithBucket(Node basedOn, Bucket newBucket, Lineage lineage)
			{
				if (basedOn.Lineage.AllowMutation(lineage))
				{
					basedOn.Bucket = newBucket;
					basedOn.Count = basedOn.Left.Count + basedOn.Right.Count + newBucket.Count;
					return basedOn;
				}
				return new Node(basedOn.Hash, newBucket, basedOn.Left, basedOn.Right, newBucket.Eq, lineage);
			}
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
#if ASSERTS
				newRoot.IsBalanced.IsTrue();
#endif

				return newRoot;
			}

			public bool Root_Contains(TKey k)
			{
				return Root_Find(k).IsSome;
			}

			internal int MaxPossibleHeight
			{
				get
				{
					return this.IsNull ? 0 : (int) Math.Ceiling(2 * Math.Log(Count, 2f));
				}
			}
			public Node AvlAdd(int hash, TKey key, TValue value, Lineage lineage, bool overwrite)
			{
				var initialCount = this.Count;
				if (this.IsNull) {
					throw ImplErrors.Invalid_execution_path;
				}
				Node ret;
				if (hash < Hash) {
					var newLeft = Left.IsNull
						? NewForKvp(hash, key, value, lineage)
						: Left.AvlAdd(hash, key, value, lineage, overwrite);
					if (newLeft == null) return null;
					ret = AvlBalance(this, newLeft, Right, lineage);
				}
				else if (hash == Hash) {
					var newBucket = Bucket.Add(key, value, lineage, overwrite);
					if (newBucket == null) return null;
					ret = WithBucket(this, newBucket, lineage);
				}
				else {
					var newRight = Right.IsNull
						? NewForKvp(hash, key, value, lineage)
						: Right.AvlAdd(hash, key, value, lineage, overwrite);
					if (newRight == null) return null;
					ret = AvlBalance(this, Left, newRight, lineage);
				}
#if ASSERTS
				ret.Count.Is(x => x <= initialCount + 1 && x >= initialCount);
				ret.IsBalanced.IsTrue();
				ret.Root_Contains(key).IsTrue();
#endif
				return ret;
			}

			/// <summary>
			/// Use to add lots of kvps with the same hash.
			/// </summary>
			/// <param name="hash"></param>
			/// <param name="kvps"></param>
			/// <returns></returns>
			public Node AddRange(int hash, LinkedList<KeyValuePair<TKey, TValue>> kvps, Lineage lin) {
				if (IsNull) throw ImplErrors.Invalid_null_invocation;
				Node ret;
				switch (hash.CompareTo(Hash)) {
					case -1:
						Node newLeft;
						if (Left.IsNull) {
							var newBucket = Bucket.FromRange(kvps, Eq, lin);
							newLeft = new Node(hash, newBucket, Null, Null, Eq, lin);
						}
						else {
							newLeft = Left.AddRange(hash, kvps, lin);
						}
						ret =  AvlBalance(this, newLeft, Right, lin);
						break;
					case 0:
						ret = WithBucket(this, Bucket.AddRangeHashCollisions(kvps, lin), lin);
						break;
					case 1:
						Node newRight;
						if (Right.IsNull) {
							var newBucket = Bucket.FromRange(kvps, Eq, lin);
							newRight = new Node(hash, newBucket, Null, Null, Eq, lin);
						}
						else {
							newRight = Right.AddRange(hash, kvps, lin);
						}
						ret = AvlBalance(this, Left, newRight, lin);
						break;
					default:
						throw ImplErrors.Invalid_execution_path;
				}
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
#if ASSERTS
				ret.IsBalanced.IsTrue();
#endif
				return ret;
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
			{
				get
				{
					foreach (var bucket in AllBuckets)
						foreach (var item in bucket.Buckets)
						{
							yield return Kvp.Of(item.Key, item.Value);
						}
				}
			}

			public IEnumerable<Bucket> AllBuckets
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

			public Node UnionNode(Node other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision) {
				if (this.IsNull && other.IsNull) return Null;
				else if (this.IsNull) {
					return other;
				}

				if (other.Hash < Hash) {
					var newLeft = Left.UnionNode(other, lineage, collision);
					return AvlBalance(this, newLeft, Right, lineage);
				}
				else if (other.Hash == Hash) {
					var newBucket = Bucket.Union(other.Bucket, collision);
					return WithBucket(this, newBucket, lineage);
				}
				else {
					var newRight = Right.UnionNode(other, lineage, collision);
					return AvlBalance(this, Left, newRight, lineage);
				}
			}

			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch, Lineage lin)
			{
				var newFactor = leftBranch.Height - rightBranch.Height;
				Node balanced;
				var leftCount = leftBranch.Count;
				var rightCount = rightBranch.Count;
				if (newFactor >= -1 && newFactor <= 1) {
					balanced = WithChildren(pivot, leftBranch, rightBranch, lin);
				}
				else if (newFactor >= 2)
				{
					var newRight = Concat(leftBranch.Right, pivot, rightBranch, lin);
					balanced =  AvlBalance(leftBranch, leftBranch.Left, newRight, lin);
				}
				else
				{
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left, lin);
					balanced =  AvlBalance(rightBranch, newLeft, rightBranch.Right, lin);
				}
#if ASSERTS
				AssertEx.IsTrue(balanced.Count == 1 + leftCount + rightCount);
				AssertEx.IsTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			public void Split(int pivot, out Node leftBranch, out Node central, out Node rightBranch, Lineage lin) {
				var myCount = Count;
				if (this.IsNull)
				{
					leftBranch = this;
					rightBranch = this;
					central = null;
				}
				else if (pivot > this.Hash)
				{
					Node right_l, right_r;
					this.Right.Split(pivot, out right_l, out central, out right_r, lin);
					leftBranch = Concat(this.Left, this, right_l, lin);
					rightBranch = right_r;
				}
				else if (pivot == this.Hash)
				{
					leftBranch = this.Left;
					central = this;
					rightBranch = this.Right;
				}
				else
				{
					Node left_l, left_r;
					this.Left.Split(pivot, out left_l, out central, out left_r, lin);
					leftBranch = left_l;
					rightBranch = Concat(left_r, this, this.Right, lin);
				}
#if ASSERTS
				var totalCount = leftBranch.Count + rightBranch.Count + (central == null ? 0 : 1);
				totalCount.Is(myCount);
#endif
			}

			public bool IsBalanced
			{
				get
				{
					return (this.IsNull ? 0 : this.Left.Height - this.Right.Height) >= -1 && (this.IsNull ? 0 : this.Left.Height - this.Right.Height) <= 1;
				}
			}

			public static Node Concat(Node left, Node right, Lineage lin)
			{
				if (left.IsNull) return right;
				if (right.IsNull) return left;
				Node central;
				left = left.ExtractMax(out central, lin);
				return Concat(left, central, right, lin);
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
			public Node Except(Node other, Lineage lin, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null )
			{
				if (this.IsNull || other.IsNull) return this;
				Node this_lesser, this_greater;
				Node central;
#if ASSERTS
				var expected = this.Pairs.Select(x => x.Key).ToHashSet();
				expected.ExceptWith(other.Pairs.Select(x => x.Key));
#endif
				this.Split(other.Hash, out this_lesser, out central, out this_greater, lin);
				var thisLesserCount = this_lesser.Count;
				var thisGreaterCount = this_greater.Count;
				var except_lesser = this_lesser.Except(other.Left, lin);
				var except_greater = this_greater.Except(other.Right, lin);
				var exceptLesserCount = except_lesser.Count;
				var exceptGreaterCount = except_greater.Count;
				Node ret;
				if (central == null)
				{
					ret = Concat(except_lesser, except_greater, lin);
				}
				else
				{
					var exceptBucket = central.Bucket.Except(other.Bucket);
					central = WithBucket(central, exceptBucket, lin);
					ret = exceptBucket.IsNull
						? Concat(except_lesser, except_greater, lin)
						: Concat(except_lesser, central, except_greater, lin);
				}
#if ASSERTS
				AssertEx.IsTrue(exceptLesserCount <= thisLesserCount);
				AssertEx.IsTrue(exceptGreaterCount <= thisGreaterCount);
				AssertEx.IsTrue(exceptGreaterCount + exceptLesserCount <= thisLesserCount + thisGreaterCount);
				AssertEx.IsTrue(ret.IsBalanced);
				var hs = ret.Pairs.Select(x => x.Key).ToHashSet();
				hs.SetEquals(expected).IsTrue();
#endif
				return ret;
			}

			public bool IsSupersetOf(Node other)
			{
				if (other.NodeCount > NodeCount) return false;
				if (other.Count > Count) return false;
				var iter = new TreeIterator(this);
				//The idea is tht we go over the nodes in order of hash, from largest to lowest.
				//If we find a node in `other` that is smaller than the current node in `this
				//This means that node doesn't exist in `this` at all, so it isn't a superset.
				return other.ForEachWhileNode(node =>
				{
					if (!iter.MoveNext()) return false;
					var cur = iter.Current;
					if (node.Hash < cur.Hash) return false;
					var result = node.Bucket.ForEachWhile((k, v) => cur.Bucket.Find(k).IsSome);
					return result;
				});
			}

			public bool IsDisjoint(Node other) {
				var areDisjoint = true;
				var iter = other.Pairs.GetEnumerator();
				ForEachWhile((k, v) => {
					if (!iter.MoveNext()) return false;
					if (Eq.Equals(k, iter.Current.Key)) {
						areDisjoint = false;
						return false;
					}
					return true;
				});
				return areDisjoint;
			}

			
			public bool ForEachWhileNode(Func<Node, bool> iter) {
				if (IsNull) return true;
				return Left.ForEachWhileNode(iter) && iter(this) && Right.ForEachWhileNode(iter);
			}

			public Node SymDifference(Node b, Lineage lin) {
#if ASSERTS
				var expected = Pairs.Select(x => x.Key).ToHashSet();
				expected.SymmetricExceptWith(b.Pairs.Select(x => x.Key));
#endif
				var ret = this.Except(b, lin).Union(b.Except(this, lin), lin);
#if ASSERTS
				ret.Pairs.Select(x => x.Key).ToHashSet().SetEquals(expected).IsTrue();
#endif
				return ret;
			}
			public Node Union(Node b, Lineage lin, Func<TKey, TValue, TValue, TValue> collision = null)
			{
				if (IsNull) return b;
				if (b.IsNull) return this;
				var myCount = Count;
				var bCount = b.Count;
#if ASSERTS
				var expected = this.Pairs.Select(x => x.Key).ToHashSet();
				var oldContents = b.Pairs.Select(x => x.Key).ToArray();
				expected.UnionWith(oldContents);
#endif
				Node a_lesser, a_greater;
				Node center_node;
				Split(b.Hash, out a_lesser, out center_node, out a_greater, lin);
				if (center_node == null) {
					center_node = b;
				}
				else {
					var newBucket = b.Bucket.Union(center_node.Bucket, collision);
					center_node = WithBucket(center_node, newBucket, lin);
				}
				var unitedLeft = a_lesser.Union(b.Left, lin, collision);

				var unitedRight = a_greater.Union(b.Right, lin, collision);
				var concated =  Concat(unitedLeft, center_node, unitedRight, lin);
#if ASSERTS
				AssertEx.IsTrue(concated.Count <= myCount + bCount);
				AssertEx.IsTrue(concated.Count >= myCount);
				AssertEx.IsTrue(concated.Count >= bCount);
				AssertEx.IsTrue(concated.IsBalanced);
				var hs = concated.Pairs.Select(x => x.Key).ToHashSet();
				hs.SetEquals(expected).IsTrue();
#endif
				return concated;
			}

			public static Node FromSortedList(List<StructTuple<int, Bucket>> sorted, int startIndex, int endIndex, Lineage lineage)
			{
				if (startIndex > endIndex)
				{
					return Null;
				}
				int pivotIndex = startIndex + (endIndex - startIndex) / 2;
				Node left = FromSortedList(sorted, startIndex, pivotIndex - 1, lineage);
				Node right = FromSortedList(sorted, pivotIndex + 1, endIndex, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.First, pivot.Second, left, right, pivot.Second.Eq, lineage);
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
				var list = new List<StructTuple<int, Bucket>>();
				foreach (var pair in intersection) {
					var newBucket = pair.First.Bucket.Intersect(pair.Second.Bucket, lineage, collision);
					if (!newBucket.IsNull) list.Add(StructTuple.Create(pair.First.Hash, newBucket));
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

			public Option<TValue> Root_Find(TKey key) {
				int hash = key.GetHashCode();
				return Find(hash, key);
			}

			public Option<TValue> Find(int hash, TKey key)
			{
				var cur = this;
				while (!cur.IsNull) {
					if (hash < cur.Hash) {
						//if (cur.Left.IsNull) Debugger.Break();
						cur = cur.Left;
					}
					else if (hash == cur.Hash) {
						return cur.Bucket.Find(key);
					}
					else {
						//if (cur.Right.IsNull) Debugger.Break();
						cur = cur.Right;
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
				return new HashedAvlTree<TKey, TValue2>.Node(Hash, appliedBucket, appliedLeft, appliedRight, Eq, lineage);
			}


			public bool ForEachWhile(Func<TKey, TValue, bool> act)
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

			public Node AvlRemove(int hash, TKey key, Lineage lineage)
			{
				if (this.IsNull) return null;
				if (hash < Hash) {
					Node newLeft = Left.AvlRemove(hash, key, lineage);
					if (newLeft == null) return null;
					return AvlBalance(this, newLeft, Right, lineage);
				}
				else if (hash > Hash) {
					Node newRight = Right.AvlRemove(hash, key, lineage);
					if (newRight == null) return null;
					return AvlBalance(this, Left, newRight, lineage);
				}
				else {
					var newBucket = Bucket.Remove(key, lineage);
					if (newBucket == null) return null;
					return !newBucket.IsNull ? WithBucket(this, newBucket, lineage) : this.AvlErase(lineage);
				}
			}
		}
	}
}
