using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;


namespace Funq.Collections.Implementation
{
	/// <summary>
	/// A container class for an AVL tree ordered by key.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal static partial class OrderedAvlTree<TKey, TValue>
	{
		/// <summary>
		/// A node in the AVL tree.
		/// </summary>
		internal sealed class Node
		{
			/// <summary>
			/// A singleton indicating an empty note.
			/// </summary>
			internal static readonly Node Null = new Node(true);
			public TValue Value;
			public ComparableKey<TKey> Key;
			public int Height;
			public Node Left = Null;
			public Node Right = Null;
			public readonly bool IsNull;
			private Lineage Lineage;
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

			public static Node Construct(ComparableKey<TKey> key, TValue value, Node left, Node right, Lineage lin)
			{
				return new Node()
				{
					Value = value,
					Key = key,
					Left = left,
					Right = right,
					Lineage = lin,
					Count = left.Count + right.Count + 1,
					Height = left.Height > right.Height ? left.Height + 1 : right.Height + 1,
				};
			}

			public static Node NewForKvp(ComparableKey<TKey> k, TValue v, Lineage lineage)
			{
				return Construct(k, v, Null, Null, lineage);
			}


			/// <summary>
			/// Either constructs a new node or mutates the specified node, depending on Lineage.
			/// </summary>
			/// <param name="node"></param>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Node WithChildren(Node node, Node left, Node right, Lineage lineage)
			{
				if (node.Lineage.AllowMutation(lineage))
				{
					node.Left = left;
					node.Right = right;
					node.Height = left.Height > right.Height ? left.Height + 1 : right.Height + 1;
					node.Lineage = lineage;
					node.Count = 1 + left.Count + right.Count;
				}
				else
				{
					node = Construct(node.Key, node.Value, left, right, lineage);
				}
				return node;
			}

			/// <summary>
			/// Either constructs a new node with the specified value, or mutates the existing node.
			/// </summary>
			/// <param name="basedOn"></param>
			/// <param name="newValue"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public static Node WithValue(Node basedOn, TValue newValue, Lineage lineage)
			{
				if (basedOn.Lineage.AllowMutation(lineage))
				{
					basedOn.Value = newValue;
					return basedOn;
				}
				return Construct(basedOn.Key, newValue, basedOn.Left, basedOn.Right, lineage);
			}
			
			/// <summary>
			/// Returns the element at the specified index, ordered from smallest to largest. O(logn)
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Node IndexFromLowest(int index)
			{
				Node cur = this;
				while (!cur.IsNull)
				{
					if (index < Left.Count) cur = Left;
					else if (index == Left.Count) return this;
					else if (index < Count)
					{
						cur = Right;
						index -= Left.Count + 1;
					}
					else return null;

				}
				return null;
			}

			/// <summary>
			/// Returns the maximum element. O(logn).
			/// </summary>
			public Node Max
			{
				get
				{
					if (this.IsNull) throw Funq.Errors.Is_empty;
					var cur = this;
					for (; !cur.Right.IsNull; cur = cur.Right) { }
					return cur; 
				}
			}

			public override string ToString()
			{
				return string.Join(", ", Items);
			}

			/// <summary>
			/// Returns the minimum element. O(logn)
			/// </summary>
			public Node Min
			{
				get
				{
					if (this.IsNull) throw Funq.Errors.Is_empty;
					var cur = this;
					for (; !cur.Left.IsNull; cur = cur.Left)
					{
					}
					return cur;
				}
			}

			/// <summary>
			/// Creates a new tree from a root node and two child nodes, balancing it in the process. May mutate the root node, depending on Lineage. O(1)
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
				var rFactor = right.Factor;
				var lFactor = left.Factor;
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
				if (newRoot.Factor > 1 || newRoot.Factor < -1)
				{
					throw ImplErrors.Invalid_execution_path;
				}
#endif

				return newRoot;
			}
			/// <summary>
			/// Returns the maximum possible height for this AVL tree. O(1)
			/// </summary>
			internal int MaxPossibleHeight
			{
				get
				{
					return Count == 0 ? 0 : (int)Math.Ceiling(2 * Math.Log(Count, 2f));
				}
			}
			/// <summary>
			/// Adds the specified key-value pair to the tree. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <param name="value"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node AvlAdd(ComparableKey<TKey> key, TValue value, Lineage lineage)
			{
				if (this.IsNull)
				{
					return NewForKvp(key, value, lineage);
				}
				switch (key.CompareTo(Key))
				{
					case Cmp.Lesser:
						var newLeft = Left.AvlAdd(key, value, lineage);
						return AvlBalance(this, newLeft, Right, lineage);
					case Cmp.Equal:
						return WithValue(this, value, lineage);
					case Cmp.Greater:
						var newRight = Right.AvlAdd(key, value, lineage);
						return AvlBalance(this, Left, newRight, lineage);
					default:
						throw ImplErrors.Invalid_execution_path;
				}

			}
			/// <summary>
			/// Removes the minimum element. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node RemoveMin(Lineage lineage)
			{
				Node dummy;
				return ExtractMin(out dummy, lineage);
			}
			/// <summary>
			/// Removes the maximum element. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node RemoveMax(Lineage lineage)
			{
				Node dummy;
				return ExtractMax(out dummy, lineage);
			}

			/// <summary>
			/// Removes the minimum element and returns it. O(logn)
			/// </summary>
			/// <param name="min"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node ExtractMin(out Node min, Lineage lineage)
			{
				if (this.IsNull) throw ImplErrors.Invalid_execution_path;
				if (!Left.IsNull)
				{
					return AvlBalance(this, Left.ExtractMin(out min, lineage), Right, lineage);
				}
				min = this;
				return Right;
			}

			public IEnumerable<Kvp<TKey, TValue>> Items
			{
				get
				{
					var walker = new TreeIterator(this);
					while (walker.MoveNext())
					{
						yield return Kvp.Of(walker.Current.Key.Key, walker.Current.Value);
					}
				}

			}
			/// <summary>
			/// Removes the maximum element and returns it. O(logn)
			/// </summary>
			/// <param name="max"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node ExtractMax(out Node max, Lineage lineage)
			{
				if (IsNull) throw ImplErrors.Invalid_execution_path;
				if (!Right.IsNull)
					return AvlBalance(this, Left, Right.ExtractMax(out max, lineage), lineage);
				max = this;
				return Left;
			}
			/// <summary>
			/// Tries to find a value matching the specified key. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public Option<TValue> Find(ComparableKey<TKey> key)
			{

				var cur = this;
				while (!cur.IsNull)
				{
					switch (key.CompareTo(cur.Key))
					{
						case Cmp.Lesser:
							cur = cur.Left;
							break;
						case Cmp.Equal:
							return Value;
						case Cmp.Greater:
							cur = cur.Right;
							break;
					}
				}
				return Option.None;
			}

			/// <summary>
			/// Applies a selector on the values of the tree, returning a new tree. O(n)
			/// </summary>
			/// <typeparam name="TValue2"></typeparam>
			/// <param name="selector"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public OrderedAvlTree<TKey, TValue2>.Node Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				if (this.IsNull)
				{
					return OrderedAvlTree<TKey, TValue2>.Node.Null;
				}
				var defaultNull = OrderedAvlTree<TKey, TValue2>.Node.Null;
				var children = (Left.IsNull ? 0 : 1) << 1 | (Right.IsNull ? 0 : 1);
				var appliedLeft = Left.IsNull ? defaultNull : Left.Apply(selector, lineage);
				var appliedValue = selector(Key, Value);
				var appliedRight = Right.IsNull ? defaultNull : Right.Apply(selector, lineage);
				return OrderedAvlTree<TKey, TValue2>.Node.Construct(Key, appliedValue, appliedLeft, appliedRight, lineage);
			}

			/// <summary>
			/// Iterates over the tree, from minimum element to maximum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachWhile(Func<TKey, TValue, bool> act)
			{
				if (this.IsNull) return true;
				return Left.ForEachWhile(act) && act(Key, Value) && Right.ForEachWhile(act);
			}


			/// <summary>
			/// Iterates over the tree, from maximum element to minimum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachBackWhile(Func<TKey, TValue, bool> act)
			{
				if (this.IsNull) return true;
				return Right.ForEachBackWhile(act) && act(Key, Value) && Left.ForEachBackWhile(act);
			}

			/// <summary>
			/// Removes the current element from the tree. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
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

			/// <summary>
			/// Removes an element with the specified key from the tree. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node AvlRemove(ComparableKey<TKey> key, Lineage lineage)
			{
				if (this.IsNull) return this;
				switch (key.CompareTo(Key))
				{
					case Cmp.Lesser:
						Node newLeft = Left.AvlRemove(key, lineage);
						return AvlBalance(this, newLeft, Right, lineage);
					case Cmp.Greater:
						Node newRight = Right.AvlRemove(key, lineage);
						return AvlBalance(this, Left, newRight, lineage);
					case Cmp.Equal:
						return this.AvlErase(lineage);
					default:
						throw ImplErrors.Invalid_execution_path;
				}
			}
			/// <summary>
			/// Concatenates leftBranch-pivot-rightBranch and balances the result. 
			/// </summary>
			/// <param name="leftBranch"></param>
			/// <param name="pivot"></param>
			/// <param name="rightBranch"></param>
			/// <returns></returns>
			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch)
			{
				//if (leftBranch.IsNull) return rightBranch.UnionNode(leftBranch, Lineage.Immutable, null);
				//if (rightBranch.IsNull) return leftBranch.UnionNode(rightBranch, Lineage.Immutable, null);
				var newFactor = leftBranch.Height - rightBranch.Height;
				Node balanced;
				if (newFactor >= -1 && newFactor <= 1)
				{
					balanced = Construct(pivot.Key, pivot.Value, leftBranch, rightBranch, Lineage.Immutable);
				}
				else if (newFactor >= 2)
				{
					var newRight = Concat(leftBranch.Right, pivot, rightBranch);
					balanced = AvlBalance(leftBranch, leftBranch.Left, newRight, Lineage.Immutable);
				}
				else
				{
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left);
					balanced = AvlBalance(rightBranch, newLeft, rightBranch.Right, Lineage.Immutable);
				}
#if DEBUG
				AssertEx.IsTrue(balanced.Count == 1 + leftBranch.Count + rightBranch.Count);
				AssertEx.IsTrue(balanced.IsBalanced);
#endif
				return balanced;
			}

			/// <summary>
			/// Splits the tree into a left subtree where all elements are smallest than the pivot, <br/>
			///  a right subtree where all elements are larger than the pivot, and a central value equal to the pivot (if one exists)
			/// </summary>
			/// <param name="pivot"></param>
			/// <param name="leftBranch"></param>
			/// <param name="central"></param>
			/// <param name="rightBranch"></param>
			public void Split(ComparableKey<TKey> pivot, out Node leftBranch, out Option<TValue> central, out Node rightBranch)
			{
				if (this.IsNull)
				{
					leftBranch = this;
					rightBranch = this;
					central = Option.None;
					return;
				}
				switch (pivot.CompareTo(this.Key))
				{
					case Cmp.Lesser:
						Node left_l, left_r;
						this.Left.Split(pivot, out left_l, out central, out left_r);
						leftBranch = left_l;
						rightBranch = Concat(left_r, this, this.Right);
						break;
					case Cmp.Greater:
						Node right_l, right_r;
						this.Right.Split(pivot, out right_l, out central, out right_r);
						leftBranch = Concat(this.Left, this, right_l);
						rightBranch = right_r;
						break;
					case Cmp.Equal:
						leftBranch = this.Left;
						central = this.Value;
						rightBranch = this.Right;
						break;
					default:
						throw ImplErrors.Invalid_execution_path;
				}
#if DEBUG
				var totalCount = leftBranch.Count + rightBranch.Count + (central.IsNone ? 0 : 1);
				totalCount.Is(this.Count);
#endif
			}

			/// <summary>
			/// Returns true if the tree is balanced.
			/// </summary>
			public bool IsBalanced
			{
				get
				{
					return this.Factor >= -1 && this.Factor <= 1;
				}
			}

			/// <summary>
			/// Concantenates a left subtree and a right subtree. 
			/// </summary>
			/// <param name="left"></param>
			/// <param name="right"></param>
			/// <returns></returns>
			public static Node Concat(Node left, Node right)
			{
				if (left.IsNull) return right;
				if (right.IsNull) return left;
				Node central;
				left = left.ExtractMax(out central, Lineage.Immutable);
				return Concat(left, central, right);
			}

			/// <summary>
			/// Counts the number of elements shared between this tree and the specified tree, without actually evaluating the intersection. O(min(m+n, mlogn, nlogm))
			/// </summary>
			/// <param name="b"></param>
			/// <returns></returns>
			public int CountIntersection(Node b)
			{
				var count = 0;
				foreach (var pair in this.IntersectElements(b))
				{
					count += 1;
				}
				return count;
			}

			/// <summary>
			/// Returns the set-theoretic relation between this tree and another tree. O(min(m+n, nlogm)), where n ≤ m.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public SetRelation Relation(Node other)
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

			/// <summary>
			/// Removes the keys of the other tree from this tree. Keys are ignored. <br/>
			/// Corresponds to a set theoretic relative complement: this ∖ other.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public Node Except(Node other)
			{
				if (this.IsNull || other.IsNull) return this;
				Node this_lesser, this_greater;
				Option<TValue> central_opt;
				this.Split(other.Key, out this_lesser, out central_opt, out this_greater);
				var except_lesser = this_lesser.Except(other.Left);
				var except_greater = this_greater.Except(other.Right);
				Node ret;
				ret = Concat(except_lesser, except_greater);
#if DEBUG
				AssertEx.IsTrue(except_lesser.Count <= this_lesser.Count);
				AssertEx.IsTrue(except_greater.Count <= this_greater.Count);
				AssertEx.IsTrue(except_greater.Count + except_lesser.Count <= this_lesser.Count + this_greater.Count);
				AssertEx.IsTrue(except_greater.IsBalanced);
#endif
				return ret;
			}

			/// <summary>
			/// Returns the set-theoretic symmetric difference between the two trees. 
			/// </summary>
			/// <param name="b"></param>
			/// <returns></returns>
			public Node SymDifference(Node b)
			{
				return b.Union(this, null).Except(b.Intersect(this, null, Lineage.Immutable));
			}

			/// <summary>
			/// Returns the set-theoretic union, and applies a function on the values in case of a collision.  <br/>
			/// If the collision resolution function is null, it is ignored and the value is kept is arbitrary.
			/// </summary>
			/// <param name="b"></param>
			/// <param name="collision"></param>
			/// <returns></returns>
			public Node Union(Node b, Func<TKey, TValue, TValue, TValue> collision)
			{
				if (IsNull) return b;
				if (b.IsNull) return this;
				Node a_lesser, a_greater;
				Option<TValue> center_val_opt;
				Split(b.Key, out a_lesser, out center_val_opt, out a_greater);
				var unitedLeft = a_lesser.Union(b.Left, collision);

				var unitedRight = a_greater.Union(b.Right, collision);
				var center_val = center_val_opt.IsNone || collision == null ? b.Value : collision(b.Key, center_val_opt, b.Value);
				var newCenter = Construct(b.Key, center_val, unitedLeft, unitedRight, Lineage.Immutable);
				var concated = Concat(unitedLeft, newCenter, unitedRight);
#if DEBUG
				AssertEx.IsTrue(concated.Count <= Count + b.Count);
				AssertEx.IsTrue(concated.Count >= Count);
				AssertEx.IsTrue(concated.Count >= b.Count);
				AssertEx.IsTrue(concated.IsBalanced);
#endif
				return concated;
			}

			/// <summary>
			/// Constructs a tree from a sorted list.
			/// </summary>
			/// <param name="sorted"></param>
			/// <param name="startIndex"></param>
			/// <param name="endIndex"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public static Node FromSortedList(List<Kvp<ComparableKey<TKey>, TValue>> sorted, int startIndex, int endIndex, Lineage lineage)
			{
				if (startIndex > endIndex)
				{
					return Null;
				}
				int pivotIndex = startIndex + (endIndex - startIndex) / 2;
				Node left = FromSortedList(sorted, startIndex, pivotIndex - 1, lineage);
				Node right = FromSortedList(sorted, pivotIndex + 1, endIndex, lineage);
				var pivot = sorted[pivotIndex];
				return Construct(pivot.Key, pivot.Value, left, right, lineage);
			}

			public Option<Node> ByOrder(int index) {
				if (this.IsNull) {
					return Option.None;
				}
				if (index < Left.Count) {
					return Left.ByOrder(index);
				}
				if (index == Left.Count) {
					return this;
				}
				return Right.ByOrder(index - Left.Count - 1);
			}

			/// <summary>
			/// Returns an iterator for retrieving all the elements shared between this tree and another tree.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other)
			{

				if (IsNull || other.IsNull) yield break;
				var a_iterator = new TreeIterator(this);
				var b_iterator = new TreeIterator(other);
				var pivotInA = true;
				bool success = a_iterator.MoveNext();
				Node pivot = a_iterator.Current;
				while (!a_iterator.IsEnded || !b_iterator.IsEnded)
				{
					var trySeekIn = pivotInA ? b_iterator : a_iterator;
					Cmp cmpResult;
					success = trySeekIn.SeekGreaterThan(pivot.Key, out cmpResult);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (cmpResult == Cmp.Equal)
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

			/// <summary>
			/// Returns the set-thereotic intersection between the two trees, and applies the collision resolution function on each shared key-value pair. O(min(m+n,nlogm)) where n ≤ m. <br/>
			/// If the collision resolution function is null, it is ignored and the value is kept is arbitrary.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="collision"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node Intersect(Node other, Func<TKey, TValue, TValue, TValue> collision, Lineage lineage)
			{
				var intersection = this.IntersectElements(other);
				var list = new List<Kvp<ComparableKey<TKey>, TValue>>();
				foreach (var pair in intersection)
				{
					var newValue = collision == null ? pair.First.Value : collision(pair.First.Key, pair.First.Value, pair.Second.Value);
					list.Add(Kvp.Of(pair.First.Key, newValue));
				}
				return FromSortedList(list, 0, list.Count - 1, lineage);
			}
		}
	}
}
