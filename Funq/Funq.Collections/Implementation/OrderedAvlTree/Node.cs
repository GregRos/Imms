using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Funq.Abstract;
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
			internal static readonly Node Empty = new Node(true);
			public TValue Value;
			public readonly TKey Key;
			public int Height;
			public Node Left = Empty;
			public Node Right = Empty;
			public readonly bool IsEmpty;
			private readonly Lineage Lineage;
			public int Count;
			internal readonly IComparer<TKey> Comparer;
			internal Node(bool isEmpty = false)
			{
				Height = 0;
				Count = 0;
				IsEmpty = isEmpty;
				Comparer = FastComparer<TKey>.Default;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node(TKey key, TValue value, Node left, Node right, IComparer<TKey> comparer, Lineage lineage) {
				Key = key;
				Value = value;
				Left = left;
				Right = right;
				Comparer = comparer;
				Lineage = lineage;
				Height = Math.Max(left.Height, right.Height) + 1;
				Count = left.Count + right.Count + 1;
			}
			
			public int Factor
			{
				get
				{
					return IsEmpty ? 0 : Left.Height - Right.Height;
				}
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node Root_Add(TKey k, TValue v, IComparer<TKey> comparer, bool overwrite, Lineage lin) {
				if (this.IsEmpty) {
					return new Node(k, v, Empty, Empty, comparer, lin);
				}
				else {
					return AvlAdd(k, v, lin, overwrite);
				}
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node NewForKvp(TKey k, TValue v, Lineage lineage)
			{
				return new Node(k, v, Empty, Empty, Comparer, lineage);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node MutateOrCreate(TValue value, Node left, Node right, Lineage lin) {
				if (Lineage.AllowMutation(lin)) {
					Value = value;
					Left = left;
					Right = right;
					Height = left.Height > right.Height ? left.Height + 1 : right.Height + 1;
					Count = 1 + left.Count + right.Count;
					return this;
				}
				else {
					return new Node(Key, value, left, right, Comparer, lin);
				}
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
			public static Node WithChildren(Node node, Node left, Node right, Lineage lineage) {
				return node.MutateOrCreate(node.Value, left, right, lineage);
			}

			/// <summary>
			/// Either constructs a new node with the specified value, or mutates the existing node.
			/// </summary>
			/// <param name="basedOn"></param>
			/// <param name="newValue"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Node WithValue(Node basedOn, TValue newValue, Lineage lineage)
			{
				if (basedOn.Lineage.AllowMutation(lineage))
				{
					basedOn.Value = newValue;
					return basedOn;
				}
				return new Node(basedOn.Key, newValue, basedOn.Left, basedOn.Right, basedOn.Comparer, lineage);
			}
			
			/// <summary>
			/// Returns the element at the specified index, ordered from smallest to largest. O(logn)
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Node IndexFromLowest(int index)
			{
				Node cur = this;

				while (!cur.IsEmpty)
				{
					if (index < Left.Count) cur = Left;
					else if (index == Left.Count) return this;
					else
					{
						cur = Right;
						index -= Left.Count + 1;
					}
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
					if (this.IsEmpty) throw Funq.Errors.Is_empty;
					var cur = this;
					for (; !cur.Right.IsEmpty; cur = cur.Right) { }
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
					if (this.IsEmpty) throw Funq.Errors.Is_empty;
					var cur = this;
					for (; !cur.Left.IsEmpty; cur = cur.Left)
					{
					}
					return cur;
				}
			}

			public Node InitializeFromNull(TKey key, TValue value, IComparer<TKey> comparer, Lineage lineage) {
				return new Node(key, value, Empty, Empty, comparer, lineage);
			}

			/// <summary>
			/// Creates a new tree from a root node and two child nodes, balancing it in the process. May mutate the root node, depending on Lineage. O(1)
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
				var oldCount = 1 + left.Count + right.Count;
#if ASSERTS
				factor.IsBetween(-2, 2);
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
				else {
					newRoot = WithChildren(this, left, right, lineage);
				}
#if ASSERTS
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
			public Node AvlAdd(TKey key, TValue value, Lineage lineage, bool overwrite)
			{
				if (this.IsEmpty)
				{
					throw new Exception();
				}
				var r = Comparer.Compare(key, Key);
				if (r < 0) {
					var newLeft = Left.IsEmpty ? NewForKvp(key, value, lineage) :Left.AvlAdd(key, value, lineage, overwrite);
					if (newLeft == null) return null;
					return AvlBalance(newLeft, Right, lineage);
				}
				if (r > 0) {
					var newRight = Right.IsEmpty ? NewForKvp(key, value, lineage) : Right.AvlAdd(key, value, lineage, overwrite);
					if (newRight == null) return null;
					return AvlBalance(Left, newRight, lineage);
				}
				return overwrite ? WithValue(this, value, lineage) : null;
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
				if (this.IsEmpty) throw ImplErrors.Invalid_execution_path;
				if (!Left.IsEmpty)
				{
					return AvlBalance(Left.ExtractMin(out min, lineage), Right, lineage);
				}
				min = this;
				return Right;
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> DebugItems
			{
				get
				{
					var walker = new TreeIterator(this);
					while (walker.MoveNext())
					{
						yield return Kvp.Of(walker.Current.Key, walker.Current.Value);
					}
				}

			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Items
			{
				get
				{
					var walker = new TreeIterator(this);
					while (walker.MoveNext())
					{
						yield return Kvp.Of(walker.Current.Key, walker.Current.Value);
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
#if ASSERTS
				IsEmpty.IsFalse();
#endif
				if (!Right.IsEmpty)
					return AvlBalance(Left, Right.ExtractMax(out max, lineage), lineage);
				max = this;
				return Left;
			}

			public bool ForEachWhileNode(Func<Node, bool> iterator) {
				if (IsEmpty) return true;
				return Left.ForEachWhileNode(iterator) && iterator(this) && Right.ForEachWhileNode(iterator);
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				var iterator = new TreeIterator(this);
				while (iterator.MoveNext()) {
					yield return Kvp.Of(iterator.Current.Key, iterator.Current.Value);
				}
			}  

			internal bool IsSupersetOf(Node other, Func<TKey, TValue, TValue, bool> isSuperOf = null)
			{
				if (other.Count > Count) return false;
				var iter = new TreeIterator(this);
				return other.ForEachWhileNode(node =>
				{
					if (!iter.MoveNext()) return false;
					var myCur = iter.Current;
					int compare = Comparer.Compare(node.Key, myCur.Key);
					if (compare < 0) return false;
					if (compare > 0) return true;
					return isSuperOf == null || isSuperOf(myCur.Key, myCur.Value, node.Value);
					
				});
			}
			/// <summary>
			/// Tries to find a value matching the specified key. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public Option<TValue> Find(TKey key)
			{
				var cur = this;
				while (!cur.IsEmpty) {
					var r = Comparer.Compare(key, cur.Key);
					if (r < 0) {
						cur = cur.Left;
					}
					else if (r > 0) {
						cur = cur.Right;
					}
					else {
						return cur.Value;
					}
				}
				return Option.None;
			}

			/// <summary>
			/// Tries to find a value matching the specified key. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public bool Contains(TKey key)
			{
				var cur = this;
				while (!cur.IsEmpty)
				{
					var r = Comparer.Compare(key, cur.Key);
					if (r < 0)
					{
						cur = cur.Left;
					}
					else if (r > 0) {
						cur = cur.Right;
					}
					else  {
						return true;
					}
				}
				return false;
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
				if (this.IsEmpty)
				{
					return OrderedAvlTree<TKey, TValue2>.Node.Empty;
				}
				var defaultNull = OrderedAvlTree<TKey, TValue2>.Node.Empty;
				
				var appliedLeft = Left.IsEmpty ? defaultNull : Left.Apply(selector, lineage);
				var appliedValue = selector(Key, Value);
				var appliedRight = Right.IsEmpty ? defaultNull : Right.Apply(selector, lineage);
				return new OrderedAvlTree<TKey, TValue2>.Node(Key, appliedValue, appliedLeft, appliedRight, Comparer, lineage);
			}

			/// <summary>
			/// Iterates over the tree, from minimum element to maximum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachWhile(Func<TKey, TValue, bool> act)
			{
				if (this.IsEmpty) return true;
				return Left.ForEachWhile(act) && act(Key, Value) && Right.ForEachWhile(act);
			}


			/// <summary>
			/// Iterates over the tree, from maximum element to minimum element.
			/// </summary>
			/// <param name="act"></param>
			/// <returns></returns>
			public bool ForEachBackWhile(Func<TKey, TValue, bool> act)
			{
				if (this.IsEmpty) return true;
				return Right.ForEachBackWhile(act) && act(Key, Value) && Left.ForEachBackWhile(act);
			}

			/// <summary>
			/// Removes the current element from the tree. O(logn)
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
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

			/// <summary>
			/// Removes an element with the specified key from the tree. O(logn)
			/// </summary>
			/// <param name="key"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public Node AvlRemove(TKey key, Lineage lineage)
			{
				if (this.IsEmpty) return null;
				int compare = Comparer.Compare(key, Key);
				if (compare  < 0) {
					Node newLeft = Left.AvlRemove(key, lineage);
					if (newLeft == null) return null;
					return AvlBalance(newLeft, Right, lineage);
				}
				if (compare > 0) {
					Node newRight = Right.AvlRemove(key, lineage);
					if (newRight == null) return null;
					return AvlBalance(Left, newRight, lineage);
				}
				return this.AvlErase(lineage);
			}
			/// <summary>
			/// Concatenates leftBranch-pivot-rightBranch and balances the result. 
			/// </summary>
			/// <param name="leftBranch"></param>
			/// <param name="pivot"></param>
			/// <param name="rightBranch"></param>
			/// <returns></returns>
			public static Node Concat(Node leftBranch, Node pivot, Node rightBranch, Lineage lineage)
			{
				//if (leftBranch.IsEmpty) return rightBranch.UnionNode(leftBranch, Lineage.Immutable, null);
				//if (rightBranch.IsEmpty) return leftBranch.UnionNode(rightBranch, Lineage.Immutable, null);
				var newFactor = leftBranch.Height - rightBranch.Height;
				var oldLeftCount = leftBranch.Count;
				var oldRightCount = rightBranch.Count;
				Node balanced;
				if (newFactor >= 2) {
					var newRight = Concat(leftBranch.Right, pivot, rightBranch, lineage);
					balanced = leftBranch.AvlBalance(leftBranch.Left, newRight, lineage);
				}
				else if (newFactor <= -2) {
					var newLeft = Concat(leftBranch, pivot, rightBranch.Left, lineage);
					balanced = rightBranch.AvlBalance(newLeft, rightBranch.Right, lineage);
				}
				else {
					balanced = WithChildren(pivot, leftBranch, rightBranch, lineage);
				}
#if ASSERTS
				AssertEx.IsTrue(balanced.Count == 1 + oldLeftCount + oldRightCount);
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
			public void Split(TKey pivot, out Node leftBranch, out Node central, out Node rightBranch, Lineage lin) {
				if (this.IsEmpty)
				{
					leftBranch = this;
					rightBranch = this;
					central = null;
					return;
				}
				var oldCount = Count;
				int compare = Comparer.Compare(pivot, Key);
				if (compare < 0) {
					Node left_l, left_r;
					this.Left.Split(pivot, out left_l, out central, out left_r, lin);
					leftBranch = left_l;
					rightBranch = Concat(left_r, this, this.Right, lin);
				}
				else if (compare > 0) {
					Node right_l, right_r;
					this.Right.Split(pivot, out right_l, out central, out right_r, lin);
					leftBranch = Concat(this.Left, this, right_l, lin);
					rightBranch = right_r;
				}
				else {
					leftBranch = this.Left;
					central = this;
					rightBranch = this.Right;
				}

#if ASSERTS
				var totalCount = leftBranch.Count + rightBranch.Count + (central == null ? 0 : 1);
				totalCount.Is(oldCount);
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
			public static Node Concat(Node left, Node right, Lineage lin)
			{
				if (left.IsEmpty) return right;
				if (right.IsEmpty) return left;
				Node central;
				left = left.ExtractMax(out central, lin);
				return Concat(left, central, right, lin);
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
			public Node Except<TValue2>(OrderedAvlTree<TKey, TValue2>.Node other, Lineage lin, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null)
			{
				if (this.IsEmpty || other.IsEmpty) return this;
				Node this_lesser, this_greater;
				Node central_node;
#if ASSERTS
				var expected = this.Pairs.Select(x => x.Key).Except(other.Pairs.Select(x => x.Key)).ToHashSet();
#endif
				this.Split(other.Key, out this_lesser, out central_node, out this_greater, lin);
				var thisLesserCount = this_lesser.Count;
				var thisGreaterCount = this_greater.Count;
				var except_lesser = this_lesser.Except(other.Left, lin, subtraction);
				var except_greater = this_greater.Except(other.Right, lin, subtraction);
				var exceptLesserCount = except_lesser.Count;
				var exceptGreaterCount = except_greater.Count;
				Node ret;
				if (central_node == null || subtraction == null) {
					ret = Concat(except_lesser, except_greater, lin);
				}
				else {
					var subtracted = subtraction(other.Key, central_node.Value, other.Value);
					if (subtracted.IsNone) {
						ret = Concat(except_lesser, except_greater, lin);
					}
					else {
						central_node = WithValue(central_node, subtracted.Value, lin);
						ret = Concat(except_lesser, central_node, except_greater, lin);
					}
				}
#if ASSERTS
				AssertEx.IsTrue(exceptLesserCount <= thisLesserCount);
				AssertEx.IsTrue(exceptGreaterCount <= thisGreaterCount);
				AssertEx.IsTrue(exceptGreaterCount + exceptLesserCount <= thisLesserCount + thisGreaterCount);
				AssertEx.IsTrue(except_greater.IsBalanced);
				var res = ret.Pairs.Select(x => x.Key).ToHashSet();
				res.SetEquals(expected).IsTrue();
#endif
				return ret;
			}

			public Node SymDifference(Node b, Lineage lin)
			{
#if ASSERTS
				var expected = this.Pairs.Select(x => x.Key).ToHashSet();
				expected.SymmetricExceptWith(b.Pairs.Select(x => x.Key));
#endif
				var ret = this.Except(b, lin).Union(b.Except(this, lin), null, lin);
#if ASSERTS
				var retSet = ret.Pairs.Select(x => x.Key).ToHashSet();
				retSet.SetEquals(expected).IsTrue();
#endif
				return ret;
			}

			public IEnumerable<KeyValuePair<TKey, TValue>> Pairs {
				get {
					return Items;
				}
			}

			public bool Debug_Intersect(List<TKey> result, Node other) {
				var kvps = new HashSet<TKey>(result);
				var list = new HashSet<TKey>();
				foreach (var item in this.Pairs)
				{
					if (other.Find(item.Key).IsSome)
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

			/// <summary>
			/// Returns the set-theoretic union, and applies a function on the values in case of a collision.  <br/>
			/// If the collision resolution function is null, it is ignored and the value is kept is arbitrary.
			/// </summary>
			/// <param name="b"></param>
			/// <param name="collision"></param>
			/// <returns></returns>
			public Node Union(Node b, Func<TKey, TValue, TValue, TValue> collision, Lineage lin)
			{
				if (IsEmpty) return b;
				if (b.IsEmpty) return this;
#if ASSERTS
				var expected = this.Pairs.Select(x => x.Key).Union(b.Pairs.Select(x => x.Key)).ToHashSet();
#endif
				Node a_lesser, a_greater;
				Node center_node;
				var oldThisCount = Count;
				var oldBCount = b.Count;
				Split(b.Key, out a_lesser, out center_node, out a_greater, lin);
				var unitedLeft = a_lesser.Union(b.Left, collision, lin);

				var unitedRight = a_greater.Union(b.Right, collision, lin);
				
				if (center_node == null || collision == null) {
					center_node = WithChildren(b, unitedLeft, unitedRight, lin);
				}
				else {
					var newValue = collision(b.Key, center_node.Value, b.Value);
					center_node = center_node.MutateOrCreate(newValue, unitedLeft, unitedRight, lin);
				}
				var concated = Concat(unitedLeft, center_node, unitedRight, lin);
#if ASSERTS
				AssertEx.IsTrue(concated.Count <= oldThisCount + oldBCount);
				AssertEx.IsTrue(concated.Count >= oldThisCount);
				AssertEx.IsTrue(concated.Count >= oldBCount);
				AssertEx.IsTrue(concated.IsBalanced);
				var result = concated.Pairs.Select(x => x.Key).ToHashSet();
				result.SetEquals(expected).IsTrue();
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
			public static Node FromSortedList(List<KeyValuePair<TKey, TValue>> sorted, int startIndex, int endIndex, IComparer<TKey> comparer, Lineage lineage)
			{
				if (startIndex > endIndex)
				{
					return Empty;
				}
				int pivotIndex = startIndex + (endIndex - startIndex) / 2;
				Node left = FromSortedList(sorted, startIndex, pivotIndex - 1, comparer, lineage);
				Node right = FromSortedList(sorted, pivotIndex + 1, endIndex, comparer, lineage);
				var pivot = sorted[pivotIndex];
				return new Node(pivot.Key, pivot.Value, left, right, comparer, lineage);
			}

			public Option<Node> ByOrder(int index) {
				if (this.IsEmpty) {
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

			public bool IsDisjoint(Node other)
			{
				var areDisjoint = true;
				var iter = other.Pairs.GetEnumerator();
				ForEachWhile((k, v) =>
				{
					if (!iter.MoveNext()) return false;
					if (Comparer.Compare(k, iter.Current.Key) == 0)
					{
						areDisjoint = false;
						return false;
					}
					return true;
				});
				return areDisjoint;
			}

			/// <summary>
			/// Returns an iterator for retrieving all the elements shared between this tree and another tree.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public IEnumerable<StructTuple<Node, Node>> IntersectElements(Node other)
			{
				if (IsEmpty || other.IsEmpty) yield break;
				var a_iterator = new TreeIterator(this);
				var b_iterator = new TreeIterator(other);
				var pivotInA = true;
				bool success = a_iterator.MoveNext();
				Node pivot = a_iterator.Current;
				while (!a_iterator.IsEnded || !b_iterator.IsEnded)
				{
					var trySeekIn = pivotInA ? b_iterator : a_iterator;
					int cmpResult;
					success = trySeekIn.SeekGreaterThan(pivot.Key, out cmpResult);
					if (!success) break;
					var maybePivot = trySeekIn.Current;
					if (cmpResult == 0)
					{
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
				var list = new List<KeyValuePair<TKey, TValue>>();
				foreach (var pair in intersection)
				{
					var newValue = collision == null ? pair.First.Value : collision(pair.First.Key, pair.First.Value, pair.Second.Value);
					list.Add(Kvp.Of(pair.First.Key, newValue));
				}
#if ASSERTS
				list.Count.Is(x => x <= Count && x <= other.Count);
				Debug_Intersect(list.Select(x => x.Key).ToList(), other);
#endif
				return FromSortedList(list, 0, list.Count - 1, Comparer, lineage);
			}
		}
	}
}
