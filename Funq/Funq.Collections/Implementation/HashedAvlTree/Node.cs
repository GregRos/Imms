using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Funq.Abstract;

namespace Funq.Implementation {

	/// <summary>
	///     Container class for an hashed avl tree, which stores key-value pairs ordered by a hash value. Keys with identical
	///     hashes are placed in buckets.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	static partial class HashedAvlTree<TKey, TValue> {
		/// <summary>
		///     A node in the hashed AVL tree.
		/// </summary>
		internal sealed class Node {
			/// <summary>
			///     The singleton that represents an empty node.
			/// </summary>
			internal static readonly Node Empty = new Node(true);

			public readonly bool IsEmpty;

			/// <summary>
			///     A bucket containing key-value pairs with identical hashes.
			/// </summary>
			private Bucket Bucket;

			public int Count;
			private readonly IEqualityComparer<TKey> Eq;

			/// <summary>
			///     The hash of this node.
			/// </summary>
			public readonly int Hash;

			private int Height;
			public Node Left = Empty;
			private Lineage Lineage;
			private int NodeCount;
			public Node Right = Empty;

			private Node(bool isEmpty = false) {
				Height = 0;
				Count = 0;
				IsEmpty = isEmpty;
				Eq = FastEquality<TKey>.Default;
			}

			private Node(int hash, Bucket bucket, Node left, Node right, IEqualityComparer<TKey> eq, Lineage lin) {
				Eq = eq;
				Hash = hash;
				Bucket = bucket;
				Left = left;
				Right = right;
				Lineage = lin;
				Height = Math.Max(left.Height, right.Height) + 1;
				Count = left.Count + right.Count + bucket.Count;
				NodeCount = left.Count + right.Count;
			}

			private int Factor {
				get { return IsEmpty ? 0 : Left.Height - Right.Height; }
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
					return total / div;
				}
			}

			internal int MaxPossibleHeight {
				get { return IsEmpty ? 0 : (int) Math.Ceiling(2 * Math.Log(Count, 2f)); }
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Pairs {
				get {
					foreach (var bucket in AllBuckets)
						foreach (var item in bucket.Buckets) yield return Kvp.Of(item.Key, item.Value);
				}
			}

			public IEnumerable<Bucket> AllBuckets {
				get {
					var walker = new TreeIterator(this);
					while (walker.MoveNext()) yield return walker.Current.Bucket;
				}

			}

			private bool IsBalanced {
				get { return (IsEmpty ? 0 : Left.Height - Right.Height) >= -1 && (IsEmpty ? 0 : Left.Height - Right.Height) <= 1; }
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
				var iterator = new TreeIterator(this);
				while (iterator.MoveNext()) {
					var bucket = iterator.Current.Bucket;
					while (!bucket.IsEmpty) {
						yield return Kvp.Of(bucket.Key, bucket.Value);
						bucket = bucket.Next;
					}
				}
			}

			public Bucket ByArbitraryOrder(int index) {
				if (IsEmpty) {
					throw ImplErrors.Arg_out_of_range("index", index);
				}
				if (index < Left.Count) {
					return Left.ByArbitraryOrder(index);
				}
				if (index < Left.Count + Bucket.Count) {
					return Bucket.ByIndex(index - Left.Count);
				}
				return Right.ByArbitraryOrder(index - Left.Count - Bucket.Count);

			}

			public Node NewForKvp(int hash, TKey k, TValue v, Lineage lineage) {
				return new Node(hash, Bucket.FromKvp(k, v, Eq, lineage), Empty, Empty, Eq, lineage);
			}

			public static Node WithChildren(Node node, Node left, Node right, Lineage lineage) {
				if (node.Lineage.AllowMutation(lineage)) {
					node.Left = left;
					node.Right = right;
					node.Height = Math.Max(left.Height, right.Height) + 1;
					node.Lineage = lineage;
					node.Count = node.Bucket.Count + left.Count + right.Count;
					node.NodeCount = left.Count + right.Count;
				} else node = new Node(node.Hash, node.Bucket, left, right, node.Eq, lineage);
				return node;
			}

			public Node Root_Add(TKey k, TValue v, Lineage lin, IEqualityComparer<TKey> eq, bool overwrite) {
				if (IsEmpty) {
					var hash = eq.GetHashCode(k);
					return new Node(hash, Bucket.FromKvp(k, v, eq, lin), Empty, Empty, eq, lin);
				} else {
					var hash = Eq.GetHashCode(k);
					return AvlAdd(hash, k, v, lin, overwrite);
				}
			}

			public Node Root_Remove(TKey k, Lineage lin) {
				if (IsEmpty) return this;
				var hash = Eq.GetHashCode(k);
				return AvlRemove(hash, k, lin);
			}

			public Node WithBucket(Bucket newBucket, Lineage lineage) {
				if (Lineage.AllowMutation(lineage)) {
					Bucket = newBucket;
					Count = Left.Count + Right.Count + newBucket.Count;
					return this;
				}
				return new Node(Hash, newBucket, Left, Right, newBucket.Eq, lineage);
			}

	
			/// <summary>
			///     Creates a new tree and balances it.
			/// </summary>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			private Node AvlBalance(Node left, Node right, Lineage lineage) {
				var factor = left.Height - right.Height;
				Node newRoot = null;
#if ASSERTS
				factor.AssertBetween(-2, 2);
				int oldCount = Bucket.Count + left.Count + right.Count;
#endif

				if (factor == -2) {
					var rFactor = right.Factor;
#if ASSERTS
					rFactor.AssertBetween(-1, 1);
#endif
					if (rFactor == 1) {
						var newLeft = WithChildren(this, left, right.Left.Left, lineage);
						var rootFrom = right.Left;
						var newRight = WithChildren(right, right.Left.Right, right.Right, lineage);
						newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
					} else {
						var newLeft = WithChildren(this, left, right.Left, lineage);
						newRoot = WithChildren(right, newLeft, right.Right, lineage);
					}
				} else if (factor == 2) {
					var lFactor = left.Factor;
#if ASSERTS
					lFactor.AssertBetween(-1, 1);
#endif
					if (lFactor == -1) {
						var newRight = WithChildren(this, left.Right.Right, right, lineage);
						var rootFrom = left.Right;
						var newLeft = WithChildren(left, left.Left, left.Right.Left, lineage);
						newRoot = WithChildren(rootFrom, newLeft, newRight, lineage);
					} else {
						var newRight = WithChildren(this, left.Right, right, lineage);
						newRoot = WithChildren(left, left.Left, newRight, lineage);
					}
				} else newRoot = WithChildren(this, left, right, lineage);
#if ASSERTS
				newRoot.IsBalanced.AssertTrue();
				newRoot.Count.AssertEqual(oldCount);
#endif

				return newRoot;
			}

			public bool Root_Contains(TKey k) {
				return Root_Find(k).IsSome;
			}

			private Node AvlAdd(int hash, TKey key, TValue value, Lineage lineage, bool overwrite) {
#if ASSERTS
				var initialCount = Count;
#endif
				if (IsEmpty) throw ImplErrors.Invalid_invocation("Empty Node");
				Node ret;
				if (hash < Hash) {
					var newLeft = Left.IsEmpty
						? NewForKvp(hash, key, value, lineage)
						: Left.AvlAdd(hash, key, value, lineage, overwrite);
					if (newLeft == null) return null;
					ret = AvlBalance(newLeft, Right, lineage);
				} else if (hash > Hash) {
					var newRight = Right.IsEmpty
						? NewForKvp(hash, key, value, lineage)
						: Right.AvlAdd(hash, key, value, lineage, overwrite);
					if (newRight == null) return null;
					ret = AvlBalance(Left, newRight, lineage);
				} else {
					var newBucket = Bucket.Add(key, value, lineage, overwrite);
					if (newBucket == null) return null;
					ret = WithBucket(newBucket, lineage);
				}


#if ASSERTS
				ret.Count.AssertEqual(x => x <= initialCount + 1 && x >= initialCount);
				ret.IsBalanced.AssertTrue();
				ret.Root_Contains(key).AssertTrue();
				//ret.AllBuckets.Count(b => b.Find(key).IsSome).Is(1);
#endif
				return ret;
			}

			private Node ExtractMin(out Node min, Lineage lineage) {
				if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
				Node ret;
				if (!Left.IsEmpty) ret = AvlBalance(Left.ExtractMin(out min, lineage), Right, lineage);
				else {
					min = this;
					ret = Right;
				}
#if ASSERTS
				ret.IsBalanced.AssertTrue();
#endif
				return ret;
			}

			public override string ToString() {
				return string.Format("({0}, {1}): {2}", Hash, Bucket, string.Join(", ", Pairs));

			}

			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch, Lineage lin) {
				var newFactor = leftBranch.Height - rightBranch.Height;
				Node balanced;
#if ASSERTS
				var leftCount = leftBranch.Count;
				var rightCount = rightBranch.Count;
#endif
				if (newFactor >= 2) {
					var newRight = Concat(leftBranch.Right, pivot, rightBranch, lin);
					balanced = leftBranch.AvlBalance(leftBranch.Left, newRight, lin);
				} else if (newFactor <= -2) {
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left, lin);
					balanced = rightBranch.AvlBalance(newLeft, rightBranch.Right, lin);
				} else balanced = WithChildren(pivot, leftBranch, rightBranch, lin);
#if ASSERTS
				AssertEx.AssertTrue(balanced.Count == 1 + leftCount + rightCount);
				AssertEx.AssertTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			public void Split(int pivot, out Node leftBranch, out Node central, out Node rightBranch, Lineage lin) {
				var myCount = Count;
				if (IsEmpty) {
					leftBranch = this;
					rightBranch = this;
					central = null;
				} else if (pivot > Hash) {
					Node rightL, rightR;
					Right.Split(pivot, out rightL, out central, out rightR, lin);
					leftBranch = Concat(Left, this, rightL, lin);
					rightBranch = rightR;
				} else if (pivot < Hash) {
					Node leftL, leftR;
					Left.Split(pivot, out leftL, out central, out leftR, lin);
					leftBranch = leftL;
					rightBranch = Concat(leftR, this, Right, lin);
				} else {
					leftBranch = Left;
					central = this;
					rightBranch = Right;
				}
#if ASSERTS
				var totalCount = leftBranch.Count + rightBranch.Count + (central == null ? 0 : 1);
				totalCount.AssertEqual(myCount);
#endif
			}

			public static Node Concat(Node left, Node right, Lineage lin) {
				if (left.IsEmpty) return right;
				if (right.IsEmpty) return left;
				Node central;
				left = left.ExtractMax(out central, lin);
				return Concat(left, central, right, lin);
			}

			public int CountIntersection(Node b) {
				var count = 0;
				foreach (var pair in IntersectElements(b)) count += pair.First.Bucket.CountIntersection(pair.Second.Bucket);
				return count;
			}

			public SetRelation RelatesTo(Node other) {
				var intersectionSize = CountIntersection(other);
				SetRelation relation = 0;
				if (intersectionSize == 0) relation |= SetRelation.Disjoint;
				var containsThis = intersectionSize == Count;
				var containedInThis = intersectionSize == other.Count;
				if (containsThis && containedInThis) relation |= SetRelation.Equal;
				else if (containsThis) relation |= SetRelation.ProperSubsetOf;
				else if (containedInThis) relation |= SetRelation.ProperSupersetOf;
				else relation |= SetRelation.None;
				return relation;
			}

			public Node Except<TValue2>(HashedAvlTree<TKey, TValue2>.Node other, Lineage lin,
				Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				if (IsEmpty || other.IsEmpty) return this;
				if (ReferenceEquals(this, other) && subtraction == null) {
					return Empty;
				}
				Node thisLesser, thisGreater;
				Node central;
				Split(other.Hash, out thisLesser, out central, out thisGreater, lin);
				var thisLesserCount = thisLesser.Count;
				var thisGreaterCount = thisGreater.Count;
				var exceptLesser = thisLesser.Except(other.Left, lin,subtraction);
				var exceptBucket = central == null ? null : central.Bucket.Except(other.Bucket, lin, subtraction);
				var exceptGreater = thisGreater.Except(other.Right, lin, subtraction);
				var exceptLesserCount = exceptLesser.Count;
				var exceptGreaterCount = exceptGreater.Count;
				Node ret;
				if (exceptBucket == null || exceptBucket.IsEmpty) {
					ret = Concat(exceptLesser, exceptGreater, lin);
				} else {
					ret = Concat(exceptLesser, central.WithBucket(exceptBucket, lin), exceptGreater, lin);
				}
#if ASSERTS
				AssertEx.AssertTrue(exceptLesserCount <= thisLesserCount);
				AssertEx.AssertTrue(exceptGreaterCount <= thisGreaterCount);
				AssertEx.AssertTrue(exceptGreaterCount + exceptLesserCount <= thisLesserCount + thisGreaterCount);
				AssertEx.AssertTrue(ret.IsBalanced);
#endif
				return ret;
			}

			public bool IsSupersetOf(Node other) {
				if (other.NodeCount > NodeCount) return false;
				if (other.Count > Count) return false;
				var iter = new TreeIterator(this);
				//The idea is tht we go over the nodes in order of hash, from smallest to largest.
				//If we find a node in `other` that is smaller than the current node in `this
				//This means that node doesn't exist in `this` at all, so it isn't a superset.
				return other.ForEachWhileNode(node => {
					if (!iter.MoveNext()) return false;
					var cur = iter.Current;
					if (node.Hash > cur.Hash) {
						while (node.Hash > iter.Current.Hash) {
							if (!iter.MoveNext()) {
								return false;
							}
						}
						cur = iter.Current;
					}
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
				var expected = Pairs.Select(x => x.Key).ToHashSet(Eq);
				expected.SymmetricExceptWith(b.Pairs.Select(x => x.Key));
#endif
				var ret = Except(b, lin).Union(b.Except(this, lin), lin);
#if ASSERTS
				ret.Pairs.Select(x => x.Key).ToHashSet(Eq).SetEquals(expected).AssertTrue();
#endif
				return ret;
			}

			public Node Union(Node b, Lineage lin, Func<TKey, TValue, TValue, TValue> collision = null) {
				if (IsEmpty) return b;
				if (b.IsEmpty) return this;
				if (ReferenceEquals(this, b) && collision == null) {
					return this;
				}
				var myCount = Count;
				var bCount = b.Count;
#if ASSERTS
				var expected = Pairs.Select(x => x.Key).ToHashSet(Eq);
				var oldContents = b.Pairs.Select(x => x.Key).ToArray();
				expected.UnionWith(oldContents);
#endif
				Node aLesser, aGreater;
				Node centerNode;
				Split(b.Hash, out aLesser, out centerNode, out aGreater, lin);
				var unitedLeft = aLesser.Union(b.Left, lin, collision);
				if (centerNode == null) centerNode = b;
				else {
					var newBucket = centerNode.Bucket.Union(b.Bucket, collision, lin);
					centerNode = centerNode.WithBucket(newBucket, lin);
				}
				var unitedRight = aGreater.Union(b.Right, lin, collision);
				var concated = Concat(unitedLeft, centerNode, unitedRight, lin);
#if ASSERTS
				AssertEx.AssertTrue(concated.Count <= myCount + bCount);
				AssertEx.AssertTrue(concated.Count >= myCount);
				AssertEx.AssertTrue(concated.Count >= bCount);
				AssertEx.AssertTrue(concated.IsBalanced);
				var hs = concated.Pairs.Select(x => x.Key).ToHashSet(Eq);
				hs.SetEquals(expected).AssertTrue();
#endif
				return concated;
			}

			public static Node FromSortedList(List<StructTuple<int, Bucket>> sorted, int startIndex, int endIndex,
				Lineage lineage) {
				if (startIndex > endIndex) return Empty;
				var pivotIndex = startIndex + (endIndex - startIndex) / 2;
				var left = FromSortedList(sorted, startIndex, pivotIndex - 1, lineage);
				var right = FromSortedList(sorted, pivotIndex + 1, endIndex, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.First, pivot.Second, left, right, pivot.Second.Eq, lineage);
			}

			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other) {

				if (IsEmpty || other.IsEmpty) yield break;
				var aIterator = new TreeIterator(this);
				var bIterator = new TreeIterator(other);
				var success = aIterator.MoveNext();
				var pivotInA = true;
				var pivot = aIterator.Current;
				while (!aIterator.IsEnded || !bIterator.IsEnded) {
					var trySeekIn = pivotInA ? bIterator : aIterator;
					success = trySeekIn.SeekGreaterThan(pivot.Hash);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (maybePivot.Hash == pivot.Hash) {
						yield return pivotInA ? StructTuple.Create(pivot, maybePivot) : StructTuple.Create(maybePivot, pivot);
						pivotInA = !pivotInA;
						trySeekIn = pivotInA ? aIterator : bIterator;
						success = trySeekIn.MoveNext();
						if (!success) break;
						pivot = trySeekIn.Current;
					} else {
						pivot = maybePivot;
						pivotInA = !pivotInA; //If the pivot was in X, it's now in Y...
					}
				}
			}

			private bool Debug_Intersect(List<TKey> result, Node other) {
				var kvps = result.ToHashSet(Eq);
				var list = new HashSet<TKey>(Eq);
				foreach (var item in Pairs) if (other.Root_Contains(item.Key)) list.Add(item.Key);
				var ret = kvps.SetEquals(list);
				if (!ret) Debugger.Break();
				return ret;
			}

			public Node Intersect(Node other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision = null) {

				var intersection = IntersectElements(other);
				var list = new List<StructTuple<int, Bucket>>();
#if ASSERTS
				var hsMe = AllBuckets.Select(x => x.Key).ToHashSet(Eq);
				var hsOther = other.AllBuckets.Select(x => x.Key).ToHashSet(Eq);
				hsMe.IntersectWith(hsOther);
#endif
				foreach (var pair in intersection) {
					var newBucket = pair.First.Bucket.Intersect(pair.Second.Bucket, lineage, collision);
					if (!newBucket.IsEmpty) list.Add(StructTuple.Create(pair.First.Hash, newBucket));
				}
#if ASSERTS
				
				var duplicates = list.Select(x => new {x, count = list.Count(y => y.First.Equals(x.First))}).OrderByDescending(x => x.count);
				list.Count.AssertEqual(x => x <= Count && x <= other.Count);
				Debug_Intersect(list.SelectMany(x => x.Second.Buckets.Select(y => y.Key)).ToList(), other);
#endif
				return FromSortedList(list, 0, list.Count - 1, lineage);
			}

			public Node ExtractMax(out Node max, Lineage lineage) {
				if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
				if (!Right.IsEmpty) return AvlBalance(Left, Right.ExtractMax(out max, lineage), lineage);
				max = this;
				return Left;
			}

			public Optional<TValue> Root_Find(TKey key) {
				var hash = key.GetHashCode();
				return Find(hash, key);
			}

			public Optional<KeyValuePair<TKey, TValue>> Root_FindKvp(TKey key)
			{
				var hash = key.GetHashCode();
				return FindKvp(hash, key);
			}

			public Optional<TValue> Find(int hash, TKey key)
			{
				var cur = this;
				while (!cur.IsEmpty)
				{
					if (hash < cur.Hash)
					{
						cur = cur.Left;
					}
					else if (hash > cur.Hash) cur = cur.Right;
					else return cur.Bucket.Find(key);
				}
				return Optional.None;
			}

			public Optional<KeyValuePair<TKey, TValue>> FindKvp(int hash, TKey key) {
				var cur = this;
				while (!cur.IsEmpty) {
					if (hash < cur.Hash) {
						cur = cur.Left;
					} else if (hash > cur.Hash) cur = cur.Right;
					else return cur.Bucket.FindKvp(key);
				}
				return Optional.None;
			}

			public HashedAvlTree<TKey, TValue2>.Node Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage) {
				if (IsEmpty) return HashedAvlTree<TKey, TValue2>.Node.Empty;
				var defaultNull = HashedAvlTree<TKey, TValue2>.Node.Empty;
				var appliedLeft = Left.IsEmpty ? defaultNull : Left.Apply(selector, lineage);
				var appliedBucket = Bucket.Apply(selector, lineage);
				var appliedRight = Right.IsEmpty ? defaultNull : Right.Apply(selector, lineage);
				return new HashedAvlTree<TKey, TValue2>.Node(Hash, appliedBucket, appliedLeft, appliedRight, Eq, lineage);
			}

			public bool ForEachWhile(Func<TKey, TValue, bool> act) {
				if (IsEmpty) return true;
				return Left.ForEachWhile(act) && Bucket.ForEachWhile(act) && Right.ForEachWhile(act);
			}

			private Node AvlErase(Lineage lineage) {
				var leftEmpty = Left.IsEmpty;
				var rightEmpty = Right.IsEmpty;
				if (leftEmpty) return Right;
				if (rightEmpty) return Left;
				Node rep;
				var newRight = Right;
				var newLeft = Left;
				if (Left.Height > Right.Height) newRight = Right.ExtractMin(out rep, lineage);
				else newLeft = Left.ExtractMax(out rep, lineage);
				return rep.AvlBalance(newLeft, newRight, lineage);
			}

			public Node AvlRemove(int hash, TKey key, Lineage lineage) {
				Node ret;
				var oldCount = Count;
				if (IsEmpty) return null;
				if (hash < Hash) {
					var newLeft = Left.AvlRemove(hash, key, lineage);
					if (newLeft == null) return null;
					ret = AvlBalance(newLeft, Right, lineage);
				} else if (hash > Hash) {
					var newRight = Right.AvlRemove(hash, key, lineage);
					if (newRight == null) return null;
					ret = AvlBalance(Left, newRight, lineage);
				} else {
					var newBucket = Bucket.Remove(key, lineage);
					if (newBucket == null) return null;
					ret = !newBucket.IsEmpty ? WithBucket(newBucket, lineage) : AvlErase(lineage);
				}
#if ASSERTS
				ret.Count.AssertEqual(oldCount - 1);
#endif
				return ret;
			}
		}
	}
}