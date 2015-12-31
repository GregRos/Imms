using System;
using System.Collections.Generic;
using System.Diagnostics;
using Funq.Abstract;
using Funq.Implementation;

namespace Funq {
	/// <summary>
	///     A sequential data structure supporting many efficient operations.
	/// </summary>
	/// <typeparam name="T"> The type of value stored in the data structure. </typeparam>
	public sealed partial class FunqList<T> : AbstractSequential<T, FunqList<T>> {
		private readonly FingerTree<T>.FTree<Leaf<T>> Root;

		internal FunqList(FingerTree<T>.FTree<Leaf<T>> root) {
			Root = root;
		}

		/// <summary>
		///     Gets the empty list.
		/// </summary>
		public new static FunqList<T> Empty {
			get { return empty; }
		}

		/// <summary>
		///     Gets the number of items in the list.
		/// </summary>
		public override int Length {
			get { return Root.Measure; }
		}

		/// <summary>
		///     Gets the first item in the list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public override T First {
			get {
				if (Root.IsEmpty) throw Errors.Is_empty;
				return Root.Left;
			}
		}

		protected override FunqList<T> UnderlyingCollection
		{
			get { return this; }
		}

		/// <summary>
		///     Returns true if the list is empty.
		/// </summary>
		public override bool IsEmpty {
			get { return Root.Measure == 0; }
		}

		/// <summary>
		///     Gets the last item in the list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public override T Last {
			get {
				if (Root.IsEmpty) throw Errors.Is_empty;
				return Root.Right;
			}
		}

		internal override Tuple<FunqList<T>, FunqList<T>> Split(int atIndex) {
			atIndex.CheckIsBetween("atIndex", -Root.Measure-1, Root.Measure);
			atIndex = atIndex < 0 ? atIndex + Root.Measure + 1 : atIndex;
			if (atIndex == 0) {
				return Tuple.Create(Empty, this);
			}
			if (atIndex == Root.Measure) {
				return Tuple.Create(this, Empty);
			}
			FingerTree<T>.FTree<Leaf<T>> left, right;
			Root.Split(atIndex, out left, out right, Lineage.Immutable);
			return Tuple.Create(left.Wrap(), right.Wrap());
		}

		public override FunqList<T> Skip(int count) {
			count.CheckIsBetween("count", 0);
			return count >= Root.Measure ? Empty : this[count, -1];
		}

		/// <summary>
		///     Adds the specified item at the beginning of the list.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FunqList<T> AddFirst(T item) {
			var ret = Root.AddFirst(item, Lineage.Immutable).Wrap();
#if ASSERTS
			ret.First.AssertEqual(item);
			if (!IsEmpty) ret.Last.AssertEqual(Last);
#endif
			return ret;
		}

		/// <summary>
		///     Joins the specified list to the beginning of this one.
		/// </summary>
		/// <param name="list"> The list to join. </param>
		/// <returns> </returns>
		FunqList<T> AddFirstList(FunqList<T> list) {
			return list.AddLastList(this);
		}

		/// <summary>
		///     Joins the specified sequence or list to the beginning of this list.
		/// </summary>
		/// <param name="items"> The items to add. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqList<T> AddFirstRange(IEnumerable<T> items) {
			items.CheckNotNull("items");
			var list = items as FunqList<T>;
			if (list != null) return AddFirstList(list);
			var lineage = Lineage.Mutable();
			int len;
			var arr = items.ToArrayFast(out len);
			var index = 0;
			var tree = FingerTree<T>.FTree<Leaf<T>>.Construct(arr, ref index, len, lineage);
			return tree.AddLastList(Root, Lineage.Immutable).Wrap();
		}

		/// <summary>
		///     Adds the specified item to the end of the list.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FunqList<T> AddLast(T item) {
			var ret = new FunqList<T>(Root.AddLast(item, Lineage.Immutable));
#if ASSERTS
			ret.Last.AssertEqual(item);
			ret.Root.Measure.AssertEqual(Root.Measure + 1);
#endif
			return ret;
		}

		/// <summary>
		///     Adds the specified list to the end of this one.
		/// </summary>
		/// <param name="list"> The list. </param>
		/// <returns> </returns>
		FunqList<T> AddLastList(FunqList<T> list) {
			if (list.IsEmpty) return this;
			if (IsEmpty) return list;
			var result = Root.AddLastList(list.Root, Lineage.Immutable);
			return result.Wrap();
		}

		/// <summary>
		///     Adds a sequence of items to the end of the list.
		/// </summary>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqList<T> AddLastRange(IEnumerable<T> items) {
			items.CheckNotNull("items");
			var list = items as FunqList<T>;
			if (list != null) return AddLastList(list);
			var lineage = Lineage.Mutable();
			int len;
			var arr = items.ToArrayFast(out len);
			if (len == 0) return this;
			var index = 0;
			var tree = FingerTree<T>.FTree<Leaf<T>>.Construct(arr, ref index, len, lineage);
			return Root.AddLastList(tree, Lineage.Immutable).Wrap();
		}

		/// <summary>
		///     Removes the first item from the list.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FunqList<T> RemoveFirst() {
			if (Root.IsEmpty) throw Errors.Is_empty;
			var ret = Root.RemoveFirst(Lineage.Immutable).Wrap();
#if ASSERTS
			if (Root.Measure > 1) ret.First.AssertEqual(this[1]);
			else ret.Root.Measure.AssertEqual(Root.Measure - 1);
#endif
			return ret;
		}

		/// <summary>
		///     Removes the last item from the list.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FunqList<T> RemoveLast() {
			if (Root.IsEmpty) throw Errors.Is_empty;
			var ret = Root.RemoveLast(Lineage.Immutable).Wrap();
#if ASSERTS
			if (ret.Root.Measure > 0) ret.Last.AssertEqual(this[-2]);
			ret.Root.Measure.AssertEqual(Root.Measure - 1);
#endif
			return ret;
		}

		/// <summary>
		///     Inserts an item at the specified index, pushing the element at the index forward.
		/// </summary>
		/// <param name="index"> The index before which to insert the item. May be negative. </param>
		/// <param name="item"> The item to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FunqList<T> Insert(int index, T item) {
			index.CheckIsBetween("index", -Root.Measure-1, Root.Measure);
			index = index < 0 ? Root.Measure + index + 1: index;
			if (index == Root.Measure) return AddLast(item);
			if (index == 0) return AddFirst(item);
			var newRoot = Root.Insert(index, item, Lineage.Mutable());
			var ret = newRoot.Wrap();
#if ASSERTS
			ret[index].AssertEqual(item);
			ret.Root.Measure.AssertEqual(Root.Measure + 1);
#endif
			return ret;
		}

		/// <summary>
		///     Returns a slice from the first index, and consisting of the specified number of elements.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public override FunqList<T> Take(int count) {
			count.CheckIsBetween("count", 0);
			if (count >= Root.Measure) return this;
			return count == 0 ? empty : GetRange(0, count);
		}

		/// <summary>
		///     Inserts a list at the specified index, pushing the element at the index forward.
		/// </summary>
		/// <param name="index">The index. Can be negative. </param>
		/// <param name="list"> The list to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		FunqList<T> InsertList(int index, FunqList<T> list) {
			index.CheckIsBetween("index", -Root.Measure-1, Root.Measure);
			list.CheckNotNull("list");
			index = index < 0 ? Root.Measure + index + 1: index;
			if (index == 0) return list.AddLastRange(this);
			if (index == Root.Measure) return AddLastRange(list);
			var lineage = Lineage.Mutable();
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			Root.Split(index, out part1, out part2, Lineage.Immutable);
			part1 = part1.AddLastList(list.Root, Lineage.Immutable);
			var result = part1.AddLastList(part2, Lineage.Immutable);

			return result.Wrap();
		}

		/// <summary>
		///     Inserts a sequence at the specified index, pushing the element at the index forward.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <param name="items"> The sequence of items to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the IEnumerable is null.</exception>
		public FunqList<T> InsertRange(int index, IEnumerable<T> items) {
			items.CheckNotNull("items");
			index.CheckIsBetween("index", -Root.Measure-1, Root.Measure);
			index = index < 0 ? index + Length + 1 : index;
			if (index == 0) return AddFirstRange(items);
			if (index == Root.Measure) return AddLastRange(items);
			var list = items as FunqList<T>;
			if (list != null) return InsertList(index, list);
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			
			Root.Split(index, out part1, out part2, Lineage.Immutable);
			int len;
			var i = 0;
			var arr = items.ToArrayFast(out len);
			var lineage = Lineage.Mutable();
			var middle = FingerTree<T>.FTree<Leaf<T>>.Construct(arr, ref i, len, lineage);
			return part1.AddLastList(middle, Lineage.Immutable).AddLastList(part2, Lineage.Immutable).Wrap();
		}

		/// <summary>
		///     Removes the element at some index from the list.
		/// </summary>
		/// <param name="index"> The index to remove. </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		/// <returns> </returns>
		public FunqList<T> RemoveAt(int index) {
			index.CheckIsBetween("index", -Root.Measure, Root.Measure-1);
			index = index < 0 ? index + Root.Measure : index;
			var ret = Root.RemoveAt(index, Lineage.Mutable()).Wrap();
#if ASSERTS
			if (Root.Measure > index + 1)
			{
				ret[index].AssertEqual(this[index + 1]);
			}
			ret.Root.Measure.AssertEqual(Root.Measure - 1);
#endif
			return ret;
		}

		/// <summary>
		///     Reverses the list. O(n).
		/// </summary>
		/// <returns> </returns>
		public override FunqList<T> Reverse() {
			return Root.Reverse(Lineage.Immutable).Wrap();
		}

		/// <summary>
		///     Sets the value of the item at the specified index.
		/// </summary>
		/// <param name="index"> The index of the item to update. </param>
		/// <param name="item"> The new value of the item. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FunqList<T> Update(int index, T item) {
			index.CheckIsBetween("index", -Root.Measure, Root.Measure - 1);
			index = index < 0 ? index + Root.Measure : index;
			if (index == 0) return RemoveFirst().AddFirst(item);
			if (index == Root.Measure - 1) return RemoveLast().AddLast(item);
			var ret = Root.Update(index, item, Lineage.Mutable()).Wrap();
#if ASSERTS
			ret[index].AssertEqual(item);
			ret.Root.Measure.AssertEqual(Root.Measure);
#endif
			return ret;
		}

		protected override FunqList<T> GetRange(int start, int count) {
			start.CheckIsBetween("start", 0, Root.Measure-1);
			(start + count).CheckIsBetween("count", 0, Root.Measure);
			if (count == 0) return empty;
			if (count == 1) {
				var res = Root[start];
				return EmptyFTree.AddLast(res, Lineage.Immutable).Wrap();
			}
			FingerTree<T>.FTree<Leaf<T>> first, last;
			Root.Split(start, out first, out last, Lineage.Immutable);
			last.Split(count, out first, out last, Lineage.Immutable);
			return first.Wrap();
		}

		public override void ForEach(Action<T> action) {
			action.CheckNotNull("action");
			Root.Iter(leaf => action(leaf.Value));
		}

		public override void ForEachBack(Action<T> action) {
			action.CheckNotNull("action");
			Root.IterBack(leaf => action(leaf.Value));
		}

		static readonly FingerTree<T>.FTree<Leaf<T>> EmptyFTree = FingerTree<T>.FTree<Leaf<T>>.Empty;
		static readonly FunqList<T> empty = new FunqList<T>(EmptyFTree);

	}
}