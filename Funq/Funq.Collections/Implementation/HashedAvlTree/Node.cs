using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Funq.Abstract;
using Funq.Collections.Common;
using System.Linq;
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
			internal static readonly Node Empty = new Node(true);
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
			public Node Left = Empty;
			public Node Right = Empty;
			public int NodeCount;
			public readonly bool IsEmpty;
			public Lineage Lineage;
			public int Count;

			internal Node(bool isEmpty = false)
			{
				Height = 0;
				Count = 0;
				IsEmpty = isEmpty;
				Eq = FastEquality<TKey>.Default;
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
				var iterator = new TreeIterator(this);
				while (iterator.MoveNext())
				{
					var bucket = iterator.Current.Bucket;
					while (!bucket.IsEmpty)
					{
						yield return Kvp.Of(bucket.Key, bucket.Value);
						bucket = bucket.Next;
					}
				}
			}  
			public int Factor
			{
				get
				{
					return IsEmpty ? 0 : Left.Height - Right.Height;
				}
			}

			public double CollisionMetric {
				get {
					if (IsEmpty) return 0;
					var total = 0.0;
					var div = 0;
					if (!Left.IsEmpty) {
						total += Left.CollisionMetric;
						div++;
					}
					total += Bucket.Count;
					div++;
					if (!Right.IsEmpty) {
						total += Right.CollisionMetric;
						div++;
					}
					return total/((double)div);
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
				return new Node(hash, Bucket.FromKvp(k, v, Eq, lineage), Empty, Empty, Eq, lineage);
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
			
			
			public Node Root_Add(TKey k, TValue v, Lineage lin, IEqualityComparer<TKey> eq, bool overwrite) {
				if (this.IsEmpty) {
					int hash = eq.GetHashCode(k);
					return new Node(hash, Bucket.FromKvp(k, v, eq, lin), Empty, Empty, eq,lin);
				}
				else {
					int hash = Eq.GetHashCode(k);
					return AvlAdd(hash, k, v, lin, overwrite);
				}
			}
	
			public Node Root_Remove(TKey k, Lineage lin) {
				if (IsEmpty) return this;
				int hash = Eq.GetHashCode(k);
				return AvlRemove(hash, k, lin);
			}
		
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
			public Node AvlBalance(Node left, Node right, Lineage lineage)
			{
				var factor = left.Height - right.Height;
				Node newRoot = null;
#if ASSERTS
				factor.IsBetween(-2, 2);
				int oldCount = Bucket.Count + left.Count + right.Count;
#endif
				
				if (factor == -2) {
					var rFactor = right.Factor;
#if ASSERTS
					rFactor.IsBetween(-1, 1);
#endif
					if (rFactor == 1) {
						var newLeft = WithChildren(this, left, right.Left.Left, lineage);
						var rootFrom = right.Left;
						var newRight = WithChildren(right, right.Left.Right, right.Right, lineage);
						newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
					}
					else {
						var newLeft = WithChildren(this, left, right.Left, lineage);
						newRoot = WithChildren(right, newLeft, right.Right, lineage);
					}
				}
				else if (factor == 2) {
					var lFactor = left.Factor;
#if ASSERTS
					lFactor.IsBetween(-1, 1);
#endif
					if (lFactor == -1) {
						var newRight = WithChildren(this, left.Right.Right, right, lineage);
						var rootFrom = left.Right;
						var newLeft = WithChildren(left, left.Left, left.Right.Left, lineage);
						newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
					}
					else {
						var newRight = WithChildren(this, left.Right, right, lineage);
						newRoot = WithChildren(left, left.Left, newRight, lineage);
					}
				}
				else newRoot = WithChildren(this, left, right, lineage);
#if ASSERTS
				newRoot.IsBalanced.IsTrue();
				newRoot.Count.Is(oldCount);
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
					return this.IsEmpty ? 0 : (int) Math.Ceiling(2 * Math.Log(Count, 2f));
				}
			}
			public Node AvlAdd(int hash, TKey key, TValue value, Lineage lineage, bool overwrite)
			{
#if ASSERTS
				var initialCount = Count;
#endif
				if (this.IsEmpty) {
					throw ImplErrors.Invalid_execution_path;
				}
				Node ret;
				if (hash < Hash) {
					var newLeft = Left.IsEmpty
						? NewForKvp(hash, key, value, lineage)
						: Left.AvlAdd(hash, key, value, lineage, overwrite);
					if (newLeft == null) return null;
					ret = AvlBalance(newLeft, Right, lineage);
				}
				else if (hash > Hash) {
					var newRight = Right.IsEmpty
						? NewForKvp(hash, key, value, lineage)
						: Right.AvlAdd(hash, key, value, lineage, overwrite);
					if (newRight == null) return null;
					ret = AvlBalance(Left, newRight, lineage);
				}
				else {
					var newBucket = Bucket.Add(key, value, lineage, overwrite);
					if (newBucket == null) return null;
					ret = WithBucket(this, newBucket, lineage);
				}

				
#if ASSERTS
				ret.Count.Is(x => x <= initialCount + 1 && x >= initialCount);
				ret.IsBalanced.IsTrue();
				ret.Root_Contains(key).IsTrue();
				ret.AllBuckets.Count(b => b.Find(key).IsSome).Is(1);
#endif
				return ret;
			}
		
			public Node ExtractMin(out Node min, Lineage lineage)
			{
				if (this.IsEmpty) throw Errors.Is_empty;
				var initialCount = this.Count;
				Node ret;
				if (!Left.IsEmpty)
				{
					 ret = AvlBalance(Left.ExtractMin(out min, lineage), Right, lineage);
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

			public override string ToString() {
				return string.Format("({0}, {1}): {2}", Hash, Bucket, string.Join(", ", Pairs));
				
			}

			public Node UnionNode(Node other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision) {
				if (this.IsEmpty && other.IsEmpty) return Empty;
				else if (this.IsEmpty) {
					return other;
				}

				if (other.Hash < Hash) {
					var newLeft = Left.UnionNode(other, lineage, collision);
					return AvlBalance(newLeft, Right, lineage);
				}
				if (other.Hash > Hash) {
					var newRight = Right.UnionNode(other, lineage, collision);
					return AvlBalance(Left, newRight, lineage);
				}
				var newBucket = Bucket.Union(other.Bucket, collision, lineage);
				return WithBucket(this, newBucket, lineage);
			}

			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch, Lineage lin)
			{
				var newFactor = leftBranch.Height - rightBranch.Height;
				Node balanced;
#if ASSERTS
				var leftCount = leftBranch.Count;
				var rightCount = rightBranch.Count;
#endif
				if (newFactor >= 2) {
					var newRight = Concat(leftBranch.Right, pivot, rightBranch, lin);
					balanced = leftBranch.AvlBalance(leftBranch.Left, newRight, lin);
				}
				else if (newFactor <= -2) {
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left, lin);
					balanced = rightBranch.AvlBalance(newLeft, rightBranch.Right, lin);
				} else {
					balanced = WithChildren(pivot, leftBranch, rightBranch, lin);
				}
#if ASSERTS
				AssertEx.IsTrue(balanced.Count == 1 + leftCount + rightCount);
				AssertEx.IsTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			public void Split(int pivot, out Node leftBranch, out Node central, out Node rightBranch, Lineage lin) {
				var myCount = Count;
				if (this.IsEmpty)
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
				else if (pivot < Hash) {
					Node left_l, left_r;
					this.Left.Split(pivot, out left_l, out central, out left_r, lin);
					leftBranch = left_l;
					rightBranch = Concat(left_r, this, this.Right, lin);
				}
				else
				{
					leftBranch = this.Left;
					central = this;
					rightBranch = this.Right;
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
					return (this.IsEmpty ? 0 : this.Left.Height - this.Right.Height) >= -1 && (this.IsEmpty ? 0 : this.Left.Height - this.Right.Height) <= 1;
				}
			}

			public static Node Concat(Node left, Node right, Lineage lin)
			{
				if (left.IsEmpty) return right;
				if (right.IsEmpty) return left;
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

		

			public Node Except<TValue2>(HashedAvlTree<TKey, TValue2>.Node other, Lineage lin, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null)
			{
				if (this.IsEmpty || other.IsEmpty) return this;
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
					var exceptBucket = central.Bucket.Except(other.Bucket, lin, subtraction);
					
					ret = exceptBucket.IsEmpty
						? Concat(except_lesser, except_greater, lin)
						: Concat(except_lesser, WithBucket(central, exceptBucket, lin), except_greater, lin);
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
				if (IsEmpty) return true;
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
				if (IsEmpty) return b;
				if (b.IsEmpty) return this;
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
					var newBucket = b.Bucket.Union(center_node.Bucket, collision, lin);
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
					return Empty;
				}
				int pivotIndex = startIndex + (endIndex - startIndex) / 2;
				Node left = FromSortedList(sorted, startIndex, pivotIndex - 1, lineage);
				Node right = FromSortedList(sorted, pivotIndex + 1, endIndex, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.First, pivot.Second, left, right, pivot.Second.Eq, lineage);
			}

			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other)
			{

				if (IsEmpty || other.IsEmpty) yield break;
				var a_iterator = new TreeIterator(this);
				var b_iterator = new TreeIterator(other);
				var success = a_iterator.MoveNext();
				var pivotInA = true;
				Node pivot = a_iterator.Current;
				while (!a_iterator.IsEnded || !b_iterator.IsEnded) {
					var trySeekIn = pivotInA ? b_iterator : a_iterator;
					success = trySeekIn.SeekGreaterThan(pivot.Hash);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (maybePivot.Hash == pivot.Hash) {
						yield return pivotInA ? StructTuple.Create(pivot, maybePivot) : StructTuple.Create(maybePivot, pivot);						
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

			public IEnumerable<Node> DebugNodes {
				get {
					var iter = new TreeIterator(this);
					while (iter.MoveNext()) {
						yield return iter.Current;
					}
				}
			}

			public bool Debug_Intersect(List<TKey> result, Node other)
			{
				var kvps = new HashSet<TKey>(result);
				var list = new HashSet<TKey>();
				foreach (var item in this.Pairs)
				{
					if (other.Root_Contains(item.Key))
					{
						list.Add(item.Key);
					}
				}
				var ret = kvps.SetEquals(list);
				if (!ret)
				{
					Debugger.Break();
				}
				return ret;
			}


			public Node Intersect(Node other, Func<TKey, TValue, TValue, TValue> collision, Lineage lineage) {

				var intersection = this.IntersectElements(other);
				var list = new List<StructTuple<int, Bucket>>();
				foreach (var pair in intersection) {
					var newBucket = pair.First.Bucket.Intersect(pair.Second.Bucket, lineage, collision);
					if (!newBucket.IsEmpty) list.Add(StructTuple.Create(pair.First.Hash, newBucket));
				}
#if ASSERTS
				
				var duplicates = list.Select(x => new {x, count = list.Count(y => y.First.Equals(x.First))}).OrderByDescending(x => x.count);
				list.Count.Is(x => x <= Count && x <= other.Count);
				Debug_Intersect(list.SelectMany(x => x.Second.Buckets.Select(X => X.Key)).ToList(), other);
#endif
				return FromSortedList(list, 0, list.Count - 1, lineage);
			}

			public Node ExtractMax(out Node max, Lineage lineage)
			{
				if (this.IsEmpty) throw ImplErrors.Invalid_execution_path;
				if (!Right.IsEmpty)
				{
					return AvlBalance(Left, Right.ExtractMax(out max, lineage), lineage);
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
				while (!cur.IsEmpty) {
					if (hash < cur.Hash) {
						//if (cur.Left.IsEmpty) Debugger.Break();
						cur = cur.Left;
					}
					else if (hash > cur.Hash) {
						cur = cur.Right;
					}
					else{
						return cur.Bucket.Find(key);
					}
				}
				return Option.None;
			}

			public HashedAvlTree<TKey, TValue2>.Node Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				if (this.IsEmpty)
				{
					return HashedAvlTree<TKey, TValue2>.Node.Empty;
				}
				var defaultNull = HashedAvlTree<TKey, TValue2>.Node.Empty;
				var children = (Left.IsEmpty ? 0 : 1) << 1 | (Right.IsEmpty ? 0 : 1);
				var appliedLeft = Left.IsEmpty ? defaultNull : Left.Apply(selector, lineage);
				var appliedBucket = Bucket.Apply(selector, lineage);
				var appliedRight = Right.IsEmpty ? defaultNull :  Right.Apply(selector, lineage);
				return new HashedAvlTree<TKey, TValue2>.Node(Hash, appliedBucket, appliedLeft, appliedRight, Eq, lineage);
			}


			public bool ForEachWhile(Func<TKey, TValue, bool> act)
			{
				if (this.IsEmpty) return true;
				return Left.ForEachWhile(act) && Bucket.ForEachWhile(act) && Right.ForEachWhile(act);
			}

			public Node AvlErase(Lineage lineage)
			{
				var leftEmpty = Left.IsEmpty;
				var rightEmpty = Right.IsEmpty;
				if (leftEmpty) {
					return Right;
				}
				if (rightEmpty) {
					return Left;
				}
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
				return rep.AvlBalance(newLeft, newRight, lineage);
			}

			public Node AvlRemove(int hash, TKey key, Lineage lineage) {
				Node ret;
				int oldCount = Count;
				if (this.IsEmpty) return null;
				if (hash < Hash) {
					Node newLeft = Left.AvlRemove(hash, key, lineage);
					if (newLeft == null) return null;
					ret= AvlBalance(newLeft, Right, lineage);
				}
				else if (hash > Hash) {
					Node newRight = Right.AvlRemove(hash, key, lineage);
					if (newRight == null) return null;
					ret= AvlBalance(Left, newRight, lineage);
				}
				else {
					var newBucket = Bucket.Remove(key, lineage);
					if (newBucket == null) return null;
					ret= !newBucket.IsEmpty ? WithBucket(this, newBucket, lineage) : this.AvlErase(lineage);
				}
#if ASSERTS
				ret.Count.Is(oldCount - 1);
#endif
				return ret;
			}
		}
	}
}
