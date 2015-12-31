using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Funq.Abstract;

namespace Funq.Implementation {
	/// <summary>
	///     A container class for an AVL tree ordered by key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	static partial class OrderedAvlTree<TKey, TValue> {

		/// <summary>
		///     A node in the AVL tree.
		/// </summary>
		internal sealed class Node {
			/// <summary>
			///     A singleton indicating an empty note.
			/// </summary>
			internal static readonly Node Empty = new Node(true);

			readonly Lineage _lineage;
			internal readonly IComparer<TKey> Comparer;
			public readonly bool IsEmpty;
			public readonly TKey Key;
			public int Count;
			private int Height;
			public Node Left = Empty;
			public Node Right = Empty;
			public TValue Value;

			private Node(bool isEmpty = false) {
				Height = 0;
				Count = 0;
				IsEmpty = isEmpty;
				Comparer = FastComparer<TKey>.Default;
			}

			public Node(TKey key, TValue value, Node left, Node right, IComparer<TKey> comparer, Lineage lineage) {
				Key = key;
				Value = value;
				Left = left;
				Right = right;
				Comparer = comparer;
				_lineage = lineage;
				Height = Math.Max(left.Height, right.Height) + 1;
				Count = left.Count + right.Count + 1;
			}

			private int Factor {
				get { return IsEmpty ? 0 : Left.Height - Right.Height; }
			}

			/// <summary>
			///     Returns the maximum element. O(logn).
			/// </summary>
			public Node Max {
				get {
					if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
					var cur = this;
					for (; !cur.Right.IsEmpty; cur = cur.Right) {}
					return cur;
				}
			}

			/// <summary>
			///     Returns the minimum element. O(logn)
			/// </summary>
			public Node Min {
				get {
					if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
					var cur = this;
					for (; !cur.Left.IsEmpty; cur = cur.Left) {}
					return cur;
				}
			}

			/// <summary>
			///     Returns the maximum possible height for this AVL tree. O(1)
			/// </summary>
			internal int MaxPossibleHeight {
				get { return Count == 0 ? 0 : (int) Math.Ceiling(2 * Math.Log(Count, 2f)); }
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> DebugItems {
				get {
					var walker = new TreeIterator(this);
					while (walker.MoveNext()) yield return Kvp.Of(walker.Current.Key, walker.Current.Value);
				}

			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Items {
				get {
					var walker = new TreeIterator(this);
					while (walker.MoveNext()) yield return Kvp.Of(walker.Current.Key, walker.Current.Value);
				}

			}

			/// <summary>
			///     Returns true if the tree is balanced.
			/// </summary>
			public bool IsBalanced {
				get { return Factor >= -1 && Factor <= 1; }
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Pairs {
				get { return Items; }
			}

			public Node Root_Add(TKey k, TValue v, IComparer<TKey> comparer, bool overwrite, Lineage lin) {
				if (IsEmpty) return new Node(k, v, Empty, Empty, comparer, lin);
				else return AvlAdd(k, v, lin, overwrite);
			}

			private Node NewForKvp(TKey k, TValue v, Lineage lineage) {
				return new Node(k, v, Empty, Empty, Comparer, lineage);
			}

			private Node MutateOrCreate(TValue value, Node left, Node right, Lineage lin) {
				if (_lineage.AllowMutation(lin)) {
					Value = value;
					Left = left;
					Right = right;
					Height = left.Height > right.Height ? left.Height + 1 : right.Height + 1;
					Count = 1 + left.Count + right.Count;
					return this;
				} else return new Node(Key, value, left, right, Comparer, lin);
			}

			/// <summary>
			///     Either constructs a new node or mutates the specified node, depending on Lineage.
			/// </summary>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			private Node WithChildren(Node left, Node right, Lineage lineage) {
				return MutateOrCreate(Value, left, right, lineage);
			}

			/// <summary>
			///     Either constructs a new node with the specified value, or mutates the existing node.
			/// </summary>
			/// <param name="newValue"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			private Node WithValue(TValue newValue, Lineage lineage) {
				if (_lineage.AllowMutation(lineage)) {
					Value = newValue;
					return this;
				}
				return new Node(Key, newValue, Left, Right, Comparer, lineage);
			}

			public override string ToString() {
				return string.Join(", ", Items);
			}

			/// <summary>
			///     Creates a new tree from a root node and two child nodes, balancing it in the process. May mutate the root node,
			///     depending on Lineage. O(1)
			/// </summary>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			private Node AvlBalance(Node left, Node right, Lineage lineage) {
				var factor = left.Height - right.Height;
				Node newRoot;

#if ASSERTS
				var oldCount = 1 + left.Count + right.Count;
				factor.AssertBetween(-2, 2);
#endif
				if (factor == -2) {
					var rFactor = right.Factor;
#if ASSERTS
					rFactor.AssertBetween(-1, 1);
#endif

					if (rFactor == 1) {
						var newLeft = WithChildren(left, right.Left.Left, lineage);
						var rootFrom = right.Left;
						var newRight = right.WithChildren(right.Left.Right, right.Right, lineage);
						newRoot = rootFrom.WithChildren(newLeft, newRight, lineage);
					} else {
						var newLeft = WithChildren(left, right.Left, lineage);
						newRoot = right.WithChildren(newLeft, right.Right, lineage);
					}
				} else if (factor == 2) {
					var lFactor = left.Factor;
#if ASSERTS
					lFactor.AssertBetween(-1, 1);
#endif
					if (lFactor == -1) {
						var newRight = WithChildren(left.Right.Right, right, lineage);
						var rootFrom = left.Right;
						var newLeft = left.WithChildren(left.Left, left.Right.Left, lineage);
						newRoot = rootFrom.WithChildren(newLeft, newRight, lineage);
					} else {
						var newRight = WithChildren(left.Right, right, lineage);
						newRoot = left.WithChildren(left.Left, newRight, lineage);
					}
				} else newRoot = WithChildren(left, right, lineage);
#if ASSERTS
				newRoot.Factor.AssertBetween(-1, 1);
				newRoot.Count.AssertEqual(oldCount);
#endif

				return newRoot;
			}

			/// <summary>
			///     Adds the specified key-value pair to the tree. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <param name="value"></param>
			/// <param name="lineage"></param>
			/// <param name="overwrite"></param>
			/// <returns></returns>
			private Node AvlAdd(TKey key, TValue value, Lineage lineage, bool overwrite) {
				if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
				var r = Comparer.Compare(key, Key);
				if (r < 0) {
					var newLeft = Left.IsEmpty ? NewForKvp(key, value, lineage) : Left.AvlAdd(key, value, lineage, overwrite);
					if (newLeft == null) return null;
					return AvlBalance(newLeft, Right, lineage);
				}
				if (r > 0) {
					var newRight = Right.IsEmpty ? NewForKvp(key, value, lineage) : Right.AvlAdd(key, value, lineage, overwrite);
					if (newRight == null) return null;
					return AvlBalance(Left, newRight, lineage);
				}
				return overwrite ? WithValue(value, lineage) : null;
			}

			/// <summary>
			///     Removes the minimum element. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node RemoveMin(Lineage lineage) {
				Node dummy;
				return ExtractMin(out dummy, lineage);
			}

			/// <summary>
			///     Removes the maximum element. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node RemoveMax(Lineage lineage) {
				Node dummy;
				return ExtractMax(out dummy, lineage);
			}

			/// <summary>
			///     Removes the minimum element and returns it. O(logn)
			/// </summary>
			/// <param name="min"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node ExtractMin(out Node min, Lineage lineage) {
				if (IsEmpty) throw ImplErrors.Invalid_invocation("EmptyNode");
				if (!Left.IsEmpty) return AvlBalance(Left.ExtractMin(out min, lineage), Right, lineage);
				min = this;
				return Right;
			}

			/// <summary>
			///     Removes the maximum element and returns it. O(logn)
			/// </summary>
			/// <param name="max"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node ExtractMax(out Node max, Lineage lineage) {
#if ASSERTS
				IsEmpty.AssertFalse();
#endif
				if (!Right.IsEmpty) return AvlBalance(Left, Right.ExtractMax(out max, lineage), lineage);
				max = this;
				return Left;
			}

			public bool ForEachWhileNode(Func<Node, bool> iterator) {
				if (IsEmpty) return true;
				return Left.ForEachWhileNode(iterator) && iterator(this) && Right.ForEachWhileNode(iterator);
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
				var iterator = new TreeIterator(this);
				while (iterator.MoveNext()) yield return Kvp.Of(iterator.Current.Key, iterator.Current.Value);
			}

			internal bool IsSupersetOf(Node other) {
				if (other.Count > Count) return false;
				var iter = new TreeIterator(this);
				return other.ForEachWhileNode(node => {
					if (!iter.MoveNext()) return false;
					var myCur = iter.Current;
					var compare = Comparer.Compare(node.Key, myCur.Key);
					if (compare > 0) {
						while ((compare = Comparer.Compare(node.Key, iter.Current.Key)) > 0) {
							if (!iter.MoveNext()) {
								return false;
							}
						}
					}
					return compare == 0;

				});
			}

			public Optional<TValue> Find(TKey key) {
				var cur = this;
				while (!cur.IsEmpty)
				{
					var r = Comparer.Compare(key, cur.Key);
					if (r < 0) cur = cur.Left;
					else if (r > 0) cur = cur.Right;
					else return cur.Value;
				}
				return Optional.None;
			}

			/// <summary>
			///     Tries to find a value matching the specified key. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public Optional<KeyValuePair<TKey, TValue>> FindKvp(TKey key) {
				var cur = this;
				while (!cur.IsEmpty) {
					var r = Comparer.Compare(key, cur.Key);
					if (r < 0) cur = cur.Left;
					else if (r > 0) cur = cur.Right;
					else return Kvp.Of(cur.Key, cur.Value);
				}
				return Optional.None;
			}

			/// <summary>
			///     Tries to find a value matching the specified key. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public bool Contains(TKey key) {
				var cur = this;
				while (!cur.IsEmpty) {
					var r = Comparer.Compare(key, cur.Key);
					if (r < 0) cur = cur.Left;
					else if (r > 0) cur = cur.Right;
					else return true;
				}
				return false;
			}

			/// <summary>
			///     Applies a selector on the values of the tree, returning a new tree. O(n)
			/// </summary>
			/// <typeparam name="TValue2"></typeparam>
			/// <param name="selector"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public OrderedAvlTree<TKey, TValue2>.Node Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage) {
				if (IsEmpty) return OrderedAvlTree<TKey, TValue2>.Node.Empty;
				var defaultNull = OrderedAvlTree<TKey, TValue2>.Node.Empty;

				var appliedLeft = Left.IsEmpty ? defaultNull : Left.Apply(selector, lineage);
				var appliedValue = selector(Key, Value);
				var appliedRight = Right.IsEmpty ? defaultNull : Right.Apply(selector, lineage);
				return new OrderedAvlTree<TKey, TValue2>.Node(Key, appliedValue, appliedLeft, appliedRight, Comparer, lineage);
			}

			/// <summary>
			///     Iterates over the tree, from minimum element to maximum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachWhile(Func<TKey, TValue, bool> act) {
				if (IsEmpty) return true;
				return Left.ForEachWhile(act) && act(Key, Value) && Right.ForEachWhile(act);
			}

			/// <summary>
			///     Iterates over the tree, from maximum element to minimum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachBackWhile(Func<TKey, TValue, bool> act) {
				if (IsEmpty) return true;
				return Right.ForEachBackWhile(act) && act(Key, Value) && Left.ForEachBackWhile(act);
			}

			/// <summary>
			///     Removes the current element from the tree. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
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

			/// <summary>
			///     Removes an element with the specified key from the tree. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node AvlRemove(TKey key, Lineage lineage) {
				if (IsEmpty) return null;
				var compare = Comparer.Compare(key, Key);
				if (compare < 0) {
					var newLeft = Left.AvlRemove(key, lineage);
					if (newLeft == null) return null;
					return AvlBalance(newLeft, Right, lineage);
				}
				if (compare > 0) {
					var newRight = Right.AvlRemove(key, lineage);
					if (newRight == null) return null;
					return AvlBalance(Left, newRight, lineage);
				}
				return AvlErase(lineage);
			}

			/// <summary>
			///     Concatenates leftBranch-pivot-rightBranch and balances the result.
			/// </summary>
			/// <param name="leftBranch"></param>
			/// <param name="pivot"></param>
			/// <param name="rightBranch"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch, Lineage lineage) {
				//if (leftBranch.IsEmpty) return rightBranch.UnionNode(leftBranch, Lineage.Immutable, null);
				//if (rightBranch.IsEmpty) return leftBranch.UnionNode(rightBranch, Lineage.Immutable, null);
				var newFactor = leftBranch.Height - rightBranch.Height;
				var oldLeftCount = leftBranch.Count;
				var oldRightCount = rightBranch.Count;
				Node balanced;
				if (newFactor >= 2) {
					var newRight = Concat(leftBranch.Right, pivot, rightBranch, lineage);
					balanced = leftBranch.AvlBalance(leftBranch.Left, newRight, lineage);
				} else if (newFactor <= -2) {
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left, lineage);
					balanced = rightBranch.AvlBalance(newLeft, rightBranch.Right, lineage);
				} else balanced = pivot.WithChildren(leftBranch, rightBranch, lineage);
#if ASSERTS
				AssertEx.AssertTrue(balanced.Count == 1 + oldLeftCount + oldRightCount);
				AssertEx.AssertTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			/// <summary>
			///     Splits the tree into a left subtree where all elements are smallest than the pivot, <br />
			///     a right subtree where all elements are larger than the pivot, and a central value equal to the pivot (if one
			///     exists)
			/// </summary>
			/// <param name="pivot"></param>
			/// <param name="leftBranch"></param>
			/// <param name="central"></param>
			/// <param name="rightBranch"></param>
			/// <param name="lin"></param>
			public void Split(TKey pivot, out Node leftBranch, out Node central, out Node rightBranch, Lineage lin) {
				if (IsEmpty) {
					leftBranch = this;
					rightBranch = this;
					central = null;
					return;
				}
				var oldCount = Count;
				var compare = Comparer.Compare(pivot, Key);
				if (compare < 0) {
					Node leftL, leftR;
					Left.Split(pivot, out leftL, out central, out leftR, lin);
					leftBranch = leftL;
					rightBranch = Concat(leftR, this, Right, lin);
				} else if (compare > 0) {
					Node rightL, rightR;
					Right.Split(pivot, out rightL, out central, out rightR, lin);
					leftBranch = Concat(Left, this, rightL, lin);
					rightBranch = rightR;
				} else {
					leftBranch = Left;
					central = this;
					rightBranch = Right;
				}

#if ASSERTS
				var totalCount = leftBranch.Count + rightBranch.Count + (central == null ? 0 : 1);
				totalCount.AssertEqual(oldCount);
#endif
			}

			/// <summary>
			///     Concantenates a left subtree and a right subtree.
			/// </summary>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lin"></param>
			/// <returns></returns>
			public static Node Concat(Node left, Node right, Lineage lin) {
				if (left.IsEmpty) return right;
				if (right.IsEmpty) return left;
				Node central;
				left = left.ExtractMax(out central, lin);
				return Concat(left, central, right, lin);
			}

			/// <summary>
			///     Counts the number of elements shared between this tree and the specified tree, without actually evaluating the
			///     intersection. O(min(m+n, mlogn, nlogm))
			/// </summary>
			/// <param name="b"></param>
			/// <returns></returns>
			public int CountIntersection(Node b) {
				var count = 0;
				foreach (var pair in IntersectElements(b)) count += 1;
				return count;
			}

			/// <summary>
			///     Returns the set-theoretic relation between this tree and another tree. O(min(m+n, nlogm)), where n ≤ m.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public SetRelation Relation(Node other) {
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

			/// <summary>
			///     Removes the keys of the other tree from this tree. Keys are ignored. <br />
			///     Corresponds to a set theoretic relative complement: this ∖ other.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="lin"></param>
			/// <param name="subtraction"></param>
			/// <returns></returns>
			public Node Except<TValue2>(OrderedAvlTree<TKey, TValue2>.Node other, Lineage lin,
				Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				if (IsEmpty || other.IsEmpty) return this;
				Node thisLesser, thisGreater;
				Node centralNode;
				Split(other.Key, out thisLesser, out centralNode, out thisGreater, lin);
				var thisLesserCount = thisLesser.Count;
				var thisGreaterCount = thisGreater.Count;
				var exceptLesser = thisLesser.Except(other.Left, lin, subtraction);
				var subtracted = centralNode != null && subtraction != null
					? subtraction(other.Key, centralNode.Value, other.Value) : Optional.None;
				var exceptGreater = thisGreater.Except(other.Right, lin, subtraction);
				
				var exceptLesserCount = exceptLesser.Count;
				var exceptGreaterCount = exceptGreater.Count;
				Node ret;
				if (subtracted.IsNone) {
					ret = Concat(exceptLesser, exceptGreater, lin);
				} else {
					centralNode = centralNode.WithValue(subtracted.Value, lin);
					ret = Concat(exceptLesser, centralNode, exceptGreater, lin);
				}
#if ASSERTS
				AssertEx.AssertTrue(exceptLesserCount <= thisLesserCount);
				AssertEx.AssertTrue(exceptGreaterCount <= thisGreaterCount);
				AssertEx.AssertTrue(exceptGreaterCount + exceptLesserCount <= thisLesserCount + thisGreaterCount);
				AssertEx.AssertTrue(exceptGreater.IsBalanced);
#endif
				return ret;
			}

			public Node SymDifference(Node b, Lineage lin) {
#if ASSERTS
				var expected = Pairs.Select(x => x.Key).ToSortedSet(Comparer);
				expected.SymmetricExceptWith(b.Pairs.Select(x => x.Key));
#endif
				var ret = Except(b, lin).Union(b.Except(this, lin), null, lin);
#if ASSERTS
				var retSet = ret.Pairs.Select(x => x.Key).ToSortedSet(Comparer);
				retSet.SetEquals(expected).AssertTrue();
#endif
				return ret;
			}

			private bool Debug_Intersect(List<TKey> result, Node other) {
				var kvps = new HashSet<TKey>(result);
				var list = new HashSet<TKey>();
				foreach (var item in Pairs) if (other.Find(item.Key).IsSome) list.Add(item.Key);
				var ret = kvps.SetEquals(list);
				if (!ret) Debugger.Break();
				return ret;
			}

			bool CanMutateBy(Lineage lin) {
				return _lineage.AllowMutation(lin);
			}

			/// <summary>
			///     Returns the set-theoretic union, and applies a function on the values in case of a collision.  <br />
			///     If the collision resolution function is null, the value of 'b' is used.
			/// </summary>
			/// <param name="b"></param>
			/// <param name="collision"></param>
			/// <param name="lin"></param>
			/// <returns></returns>
			public Node Union(Node b, Func<TKey, TValue, TValue, TValue> collision, Lineage lin) {
				if (IsEmpty) return b;
				if (b.IsEmpty) return this;
#if ASSERTS
				var expected = Pairs.Select(x => x.Key).Union(b.Pairs.Select(x => x.Key)).ToSortedSet(Comparer);
#endif
				Node aLesser, aGreater;
				Node centerNode;
				var oldThisCount = Count;
				var oldBCount = b.Count;
				Split(b.Key, out aLesser, out centerNode, out aGreater, lin);
				var bLeft = b.Left;
				var bRight = b.Right;
				var unitedLeft = aLesser.Union(bLeft, collision, lin);
				var newValue = collision == null || centerNode == null ? b.Value : collision(b.Key, centerNode.Value, b.Value);
				var unitedRight = aGreater.Union(bRight, collision, lin);
				if (centerNode == null) centerNode = b.WithChildren(unitedLeft, unitedRight, lin);
				else {
					var chosenNode = b.CanMutateBy(lin) ? b : centerNode;
					centerNode = chosenNode.MutateOrCreate(newValue, unitedLeft, unitedRight, lin);
				}
				var concated = Concat(unitedLeft, centerNode, unitedRight, lin);
#if ASSERTS
				AssertEx.AssertTrue(concated.Count <= oldThisCount + oldBCount);
				AssertEx.AssertTrue(concated.Count >= oldThisCount);
				AssertEx.AssertTrue(concated.Count >= oldBCount);
				AssertEx.AssertTrue(concated.IsBalanced);
				var result = concated.Pairs.Select(x => x.Key).ToSortedSet(Comparer);
				result.SetEquals(expected).AssertTrue();
#endif
				return concated;
			}

			/// <summary>
			///     Constructs a tree from a sorted list.
			/// </summary>
			/// <param name="sorted"></param>
			/// <param name="startIndex"></param>
			/// <param name="endIndex"></param>
			/// <param name="comparer"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public static Node FromSortedList(List<KeyValuePair<TKey, TValue>> sorted, int startIndex, int endIndex,
				IComparer<TKey> comparer, Lineage lineage) {
				if (startIndex > endIndex) return Empty;
				var pivotIndex = startIndex + (endIndex - startIndex) / 2;
				var left = FromSortedList(sorted, startIndex, pivotIndex - 1, comparer, lineage);
				var right = FromSortedList(sorted, pivotIndex + 1, endIndex, comparer, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.Key, pivot.Value, left, right, comparer, lineage);
			}

			public static Node FromSortedArray(KeyValuePair<TKey, TValue>[] sorted, int startIndex, int endIndex,
				IComparer<TKey> comparer, Lineage lineage) {
				if (startIndex > endIndex) return Empty;
				var pivotIndex = startIndex + (endIndex - startIndex) / 2;
				var left = FromSortedArray(sorted, startIndex, pivotIndex - 1, comparer, lineage);
				var right = FromSortedArray(sorted, pivotIndex + 1, endIndex, comparer, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.Key, pivot.Value, left, right, comparer, lineage);
			}

			public static Node FromSortedArraySet(TKey[] sorted, int startIndex, int endIndex, IComparer<TKey> comparer,
				Lineage lineage) {
				if (startIndex > endIndex) return Empty;
				var pivotIndex = startIndex + (endIndex - startIndex) / 2;
				var left = FromSortedArraySet(sorted, startIndex, pivotIndex - 1, comparer, lineage);
				var right = FromSortedArraySet(sorted, pivotIndex + 1, endIndex, comparer, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot, default(TValue), left, right, comparer, lineage);
			}

			public Node ByOrder(int index) {
				for (Node cur = this; !cur.IsEmpty;) {
					var left = cur.Left.Count;
					if (index < left) cur = cur.Left;
					else if (index == left) return cur;
					else {
						cur = cur.Right;
						index -= left + 1;
					}
				}
				throw ImplErrors.Arg_out_of_range("index", index);
			}

			public bool IsDisjoint(Node other) {
				var areDisjoint = true;
				var iter = other.Pairs.GetEnumerator();
				ForEachWhile((k, v) => {
					if (!iter.MoveNext()) return false;
					if (Comparer.Compare(k, iter.Current.Key) == 0) {
						areDisjoint = false;
						return false;
					}
					return true;
				});
				return areDisjoint;
			}

			/// <summary>
			///     Returns an iterator for retrieving all the elements shared between this tree and another tree.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other) {
				if (IsEmpty || other.IsEmpty) yield break;
				var aIterator = new TreeIterator(this);
				var bIterator = new TreeIterator(other);
				var pivotInA = true;
				var success = aIterator.MoveNext();
				var pivot = aIterator.Current;
				while (!aIterator.IsEnded || !bIterator.IsEnded) {
					var trySeekIn = pivotInA ? bIterator : aIterator;
					int cmpResult;
					success = trySeekIn.SeekGreaterThan(pivot.Key, out cmpResult);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (cmpResult == 0) {
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

			/// <summary>
			///     Returns the set-thereotic intersection between the two trees, and applies the collision resolution function on each
			///     shared key-value pair. O(min(m+n,nlogm)) where n ≤ m. <br />
			///     If the collision resolution function is null, it is ignored and the value is kept is arbitrary.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="lineage"></param>
			/// <param name="collision"></param>
			/// <returns></returns>
			public Node Intersect(Node other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision = null) {
				var intersection = IntersectElements(other);
				var list = new List<KeyValuePair<TKey, TValue>>();
				foreach (var pair in intersection) {
					var newValue = collision == null
						? pair.First.Value : collision(pair.First.Key, pair.First.Value, pair.Second.Value);
					list.Add(Kvp.Of(pair.First.Key, newValue));
				}
#if ASSERTS
				list.Count.AssertEqual(x => x <= Count && x <= other.Count);
				Debug_Intersect(list.Select(x => x.Key).ToList(), other);
#endif
				return FromSortedList(list, 0, list.Count - 1, Comparer, lineage);
			}
		}
	}
}