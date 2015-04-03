using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	/// <summary>
	///   A sequential data structure supporting many efficient operations.
	/// </summary>
	/// <typeparam name="T"> The type of value stored in the data structure. </typeparam>
	public sealed partial class FunqList<T> : AbstractSequential<T, FunqList<T>>
	{
		private static readonly FingerTree<T>.FTree<Leaf<T>> emptyFTree = FingerTree<T>.FTree<Leaf<T>>.Empty;
		internal static readonly FunqList<T> empty = new FunqList<T>(emptyFTree);

		internal readonly FingerTree<T>.FTree<Leaf<T>> _root;
		internal FunqList(FingerTree<T>.FTree<Leaf<T>> root)
		{
			_root = root;
		}

		/// <summary>
		///   Gets the empty list.
		/// </summary>
		public static FunqList<T> Empty
		{
			get
			{
				return empty;
			}
		}

		public override FunqList<T> Skip(int count)
		{
			return this[count, -1];
		}

		/// <summary>
		///   Gets the number of items in the list.
		/// </summary>
		public override int Length
		{
			get
			{
				return _root.Measure;
			}
		}

		/// <summary>
		///   Gets the first item in the list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public override T First
		{
			get
			{
				if (_root.Measure == 0) throw Funq.Errors.Is_empty;
				return _root.Left;
			}
		}

		/// <summary>
		///   Returns true if the list is empty.
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return Length == 0;
			}
		}

		/// <summary>
		///   Gets the last item in the list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public override T Last
		{
			get
			{
				if (_root.Measure == 0) throw Funq.Errors.Is_empty;
				return _root.Right;
			}
		}

		/// <summary>
		/// Adds the specified item at the beginning of the list.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FunqList<T> AddFirst(T item)
		{
			var ret =  _root.AddFirst(item, Lineage.Immutable).Wrap();
#if ASSERTS
			ret.First.Is(item);
			if (!IsEmpty) ret.Last.Is(Last);
#endif
			return ret;
		}

		/// <summary>
		///   Joins the specified list to the beginning of this one.
		/// </summary>
		/// <param name="list"> The list to join. </param>
		/// <returns> </returns>
		private FunqList<T> AddFirstList(FunqList<T> list)
		{
			if (list == null) throw Funq.Errors.Argument_null("list");
			return list.AddLastList(this);
		}

		/// <summary>
		/// Joins the specified sequence or list to the beginning of this list.
		/// </summary>
		/// <param name="items"> The items to add. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqList<T> AddFirstRange(IEnumerable<T> items)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
			if (items is FunqList<T>)
			{
				return AddFirstList(items as FunqList<T>);
			}
			var ftree = FingerTree<T>.Empty;
			var lineage = Lineage.Mutable();
			foreach (var item in items)
			{
				ftree = ftree.AddLast(item, lineage);
			}
			ftree = ftree.AddLastList(_root, Lineage.Immutable);

			return ftree.Wrap();
		}

		/// <summary>
		///   Adds the specified item to the end of the list.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FunqList<T> AddLast(T item)
		{
			var ret =  new FunqList<T>(_root.AddLast(item, Lineage.Immutable));
#if ASSERTS
			ret.Last.Is(item);
			ret.Length.Is(Length + 1);
#endif
			return ret;
		}

		/// <summary>
		///   Adds the specified list to the end of this one.
		/// </summary>
		/// <param name="list"> The list. </param>
		/// <returns> </returns>
		private FunqList<T> AddLastList(FunqList<T> list)
		{
			if (list == null) throw Funq.Errors.Argument_null("list");
			if (list.IsEmpty) return this;
			if (this.IsEmpty) return list;
			var result = _root.AddLastList(list._root, Lineage.Immutable);
			return result.Wrap();
		}

		/// <summary>
		///   Adds a sequence of items to the end of the list.
		/// </summary>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqList<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
			if (items is FunqList<T>)
			{
				return AddLastList(items as FunqList<T>);
			}
			var lineage = Lineage.Mutable();
			var ftree = emptyFTree;
			foreach (var item in items)
			{
				ftree = ftree.AddLast(item, lineage);
			}
			return _root.AddLastList(ftree, lineage).Wrap();
		}

		/// <summary>
		///   Removes the first item from the list.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FunqList<T> RemoveFirst()
		{
			if (_root.Measure == 0) throw Funq.Errors.Is_empty;
			var ret =  _root.RemoveFirst(Lineage.Immutable).Wrap();
#if ASSERTS
			if (Length > 1) ret.First.Is(this[1]);
			else ret.Length.Is(Length - 1);
#endif
			return ret;
		}

		/// <summary>
		///   Removes the last item from the list.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FunqList<T> RemoveLast()
		{
			if (_root.Measure == 0) throw Funq.Errors.Is_empty;
			var ret =  _root.RemoveLast(Lineage.Immutable).Wrap();
#if ASSERTS
			if (ret.Length > 0) ret.Last.Is(this[-2]);
			ret.Length.Is(Length - 1);
#endif
			return ret;
		}



		/// <summary>
		///   Inserts an item at the specified index, pushing the element at the index forward. 
		/// </summary>
		/// <param name="index"> The index before which to insert the item. May be negative. </param>
		/// <param name="item"> The item to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FunqList<T> Insert(int index, T item)
		{
			index = index < 0 ? _root.Measure + index + 1 : index;
			if (index > _root.Measure || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			if (index == _root.Measure) return AddLast(item);
			if (index == 0) return AddFirst(item);
			var new_root = _root.Insert(index, item, Lineage.Mutable());
			var ret =  new_root.Wrap();
#if ASSERTS
			ret[index].Is(item);
			ret.Length.Is(Length + 1);
#endif
			return ret;
		}
		/// <summary>
		/// Returns a slice from the first index, and consisting of the specified number of elements.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public override FunqList<T> Take(int count) {
			count.IsInRange("count", 0, Length);
			return count == 0 ? empty : GetRange(0, count);
		}

		/// <summary>
		///   Inserts a list at the specified index, pushing the element at the index forward.
		/// </summary>
		/// <param name="index">The index. Can be negative. </param>
		/// <param name="list"> The list to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		private FunqList<T> InsertList(int index, FunqList<T> list)
		{
			index = index < 0 ? _root.Measure + index + 1 : index;
			if (index > _root.Measure || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			if (list == null) throw Funq.Errors.Argument_null("list");
			if (index == 0) return list.AddLastRange(this);
			if (index == _root.Measure) return AddLastRange(list);
			var lineage = Lineage.Mutable();
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			_root.Split(index, out part1, out part2, Lineage.Immutable);
			part1 = part1.AddLastList(list._root, lineage);
			var result = part1.AddLastList(part2, lineage);

			return result.Wrap();
		}

		/// <summary>
		///   Inserts a sequence at the specified index, pushing the element at the index forward.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <param name="items"> The sequence of items to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the IEnumerable is null.</exception>
		public FunqList<T> InsertRange(int index, IEnumerable<T> items)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
			index = index < 0 ? index + _root.Measure + 1 : index;
			if (index >= _root.Measure || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			if (items is FunqList<T>)
			{
				return InsertList(index, items as FunqList<T>);
			}

			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			var lineage = Lineage.Mutable();
			_root.Split(index, out part1, out part2, Lineage.Immutable);
			var xlist1 = part1.Wrap();
			var xlist2 = part2.Wrap();
			return xlist1.Length < xlist2.Length ? xlist1.AddLastRange(items).AddLastRange(xlist2) : xlist2.AddFirstRange(items).AddFirstRange(xlist1);

		}

		/// <summary>
		///   Removes the element at some index from the list.
		/// </summary>
		/// <param name="index"> The index to remove. </param>
		///<exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		/// <returns> </returns>
		public FunqList<T> Remove(int index)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index < 0 || index >= _root.Measure) throw Funq.Errors.Arg_out_of_range("index", index);
			var ret =  _root.Remove(index, Lineage.Mutable()).Wrap();
#if ASSERTS
			if (Length > index + 1)
			{
				ret[index].Is(this[index + 1]);
			}
			ret.Length.Is(Length - 1);
			for (int i = 0; i < index; i++) {
				this[i].Is(ret[i]);
			}
			for (int i = index; i < ret.Length; i++) {
				this[i+1].Is(ret[i]);
			}
#endif
			return ret;
		}

		/// <summary>
		///   Reverses the list. O(n).
		/// </summary>
		/// <returns> </returns>
		public override  FunqList<T> Reverse()
		{
			return _root.Reverse(Lineage.Immutable).Wrap();
		}

		/// <summary>
		///   Sets the value of the item at the specified index.
		/// </summary>
		/// <param name="index"> The index of the item to update. </param>
		/// <param name="item"> The new value of the item. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FunqList<T> Update(int index, T item)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index >= _root.Measure || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			if (index == 0) return RemoveFirst().AddFirst(item);
			if (index == _root.Measure - 1) return RemoveLast().AddLast(item);
			var ret = _root.Update(index, item, Lineage.Mutable()).Wrap();
#if ASSERTS
			ret[index].Is(item);
			ret.Length.Is(Length);
#endif
			return ret;
		}

		
		protected override FunqList<T> GetRange(int start, int count)
		{
			if (start < 0 || start >= _root.Measure) throw Funq.Errors.Arg_out_of_range("start", start);
			if (count < 0 || start + count > _root.Measure) throw Funq.Errors.Arg_out_of_range("count", count);
			if (count == 0) return empty;
			if (count == 1)
			{
				var res = _root[start];
				return emptyFTree.AddLast(res, Lineage.Immutable).Wrap();
			}
			FingerTree<T>.FTree<Leaf<T>> first, last;
			_root.Split(start, out first, out last, Lineage.Immutable);
			last.Split(count, out first, out last, Lineage.Immutable);
			return first.Wrap();
		}

		/// <summary>
		///   Splits the list at the specified index. The boundary element becomes part of the first list.
		/// </summary>
		/// <param name="index"> The index at which to split. </param>
		/// <param name="first"> An output parameter that returns the first part of the sequence. </param>
		/// <param name="last"> An output parameter that returns the second part of the sequence. </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the list.</exception>
		internal void Split(int index, out FunqList<T> first, out FunqList<T> last)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index >= _root.Measure || index < 0) throw Funq.Errors.Arg_out_of_range("index", index);
			FingerTree<T>.FTree<Leaf<T>> left, right;
			_root.Split(index + 1, out left, out right, Lineage.Immutable);
			first = left.Wrap();
			last = right.Wrap();
		}

	}
}