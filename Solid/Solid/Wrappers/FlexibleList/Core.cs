using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid
{
	/// <summary>
	///   A sequential data structure supporting many efficient operations.
	/// </summary>
	/// <typeparam name="T"> The type of value stored in the data structure. </typeparam>
	public sealed partial class FlexibleList<T>
	{
		private static readonly FingerTree<T>.FTree<Leaf<T>> emptyFTree = FingerTree<T>.FTree<Leaf<T>>.Empty;
		internal static readonly FlexibleList<T> empty = new FlexibleList<T>(emptyFTree);

		private readonly FingerTree<T>.FTree<Leaf<T>> _root;

		internal FlexibleList(FingerTree<T>.FTree<Leaf<T>> root)
		{
		
			this._root = root;
		}

		/// <summary>
		///   Converts the specified array into a FlexibleList.
		/// </summary>
		/// <param name="arr"> The array. </param>
		/// <returns> </returns>
		public static implicit operator FlexibleList<T>(T[] arr)
		{
			return Empty.AddLastRange(arr);
		}

		/// <summary>
		///   Gets the empty list.
		/// </summary>
		public static FlexibleList<T> Empty
		{
			get
			{
				return empty;
			}
		}

		/// <summary>
		///   Gets the item at the specified index. O(logn), very fast.
		/// </summary>
		/// <param name="index"> The index of the item to get. Can be negative. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public T this[int index]
		{
			get
			{
				index = index < 0 ? index + _root.Measure : index;
				if (index == 0) return First;
				if (index == _root.Measure - 1) return Last;
				if (index >= _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
				var v = _root[index];
				return v.Value;
			}
		}

		/// <summary>
		///   Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get
			{
				return _root.Measure;
			}
		}

		private string FSharpFormatDisplay
		{
			get
			{
				var buildr = new StringBuilder(Count);
				foreach (var item in this)
				{
					buildr.Append(item);
				}
				return "";
			}
		}

		/// <summary>
		///   Gets the first item in the list. O(1).
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public T First
		{
			get
			{
				if (_root.Measure == 0) throw Errors.Is_empty;
				return _root.Left.Value;
			}
		}

		/// <summary>
		///   Returns true if the list is empty. O(1)
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		/// <summary>
		///   Gets the last item in the list. O(1)
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public T Last
		{
			get
			{
				if (_root.Measure == 0) throw Errors.Is_empty;
				return _root.Right.Value;
			}
		}

		/// <summary>
		///   Adds the specified item at the beginning of the list. O(1) amortized.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FlexibleList<T> AddFirst(T item)
		{
			return new FlexibleList<T>(_root.AddLeft(new Leaf<T>(item)));
		}


		/// <summary>
		/// Joins the specified list to the beginning of this one.
		/// </summary>
		/// <param name="list">The list to join.</param>
		/// <returns></returns>

		public FlexibleList<T> AddFirstList(FlexibleList<T> list)
		{
			if (list.IsEmpty) return this;
			var result = list._root.AddLeft(this._root);
			return new FlexibleList<T>(result);
		}

		/// <summary>
		///   Joins the specified sequence to the beginning of this list. O(m) for general sequences, O(logm) for lists.
		/// </summary>
		/// <param name="items"> The items to add. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>

		public FlexibleList<T> AddFirstRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			if (items is FlexibleList<T>)
			{
				return this.AddFirstList(items as FlexibleList<T>);
			}
			var ftree = emptyFTree;
			foreach (var item in items)
			{
				ftree = ftree.AddRight(new Leaf<T>(item));
			}
			ftree = ftree.AddRight(_root);
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		///   Adds the specified item to the end of the list. O(1) amortized.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FlexibleList<T> AddLast(T item)
		{
			return new FlexibleList<T>(_root.AddRight(new Leaf<T>(item)));
		}



		/// <summary>
		/// Adds the specified list to the end of this one.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <returns></returns>
	
		public FlexibleList<T> AddLastList(FlexibleList<T> list)
		{
			if (list.IsEmpty) return this;
			var result = _root.AddRight(list._root);
			return new FlexibleList<T>(result);
		}



		/// <summary>
		///   Adds a sequence of items to the end of the list.
		/// </summary>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			if (items is FlexibleList<T>)
			{
				return AddLastList(items as FlexibleList<T>);
			}
			var ftree = _root;
			foreach (var item in items)
			{
				ftree = ftree.AddRight(new Leaf<T>(item));
			}
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		///   Removes the first item from the list. O(1) amortized.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FlexibleList<T> DropFirst()
		{
			if (_root.Measure == 0) throw Errors.Is_empty;
			return new FlexibleList<T>(_root.DropLeft());
		}

		/// <summary>
		///   Removes the last item from the list. O(1) amortized.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FlexibleList<T> DropLast()
		{
			if (_root.Measure == 0) throw Errors.Is_empty;
			return new FlexibleList<T>(_root.DropRight());
		}

		/// <summary>
		///   Inserts an item immediately before the specified index. O(logn), fast.
		/// </summary>
		/// <param name="index"> The index before which to insert the item. Can be negative. </param>
		/// <param name="item"> The item to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FlexibleList<T> Insert(int index, T item)
		{
			index = index < 0 ? _root.Measure + index + 1 : index;
			if (index >= _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (index == _root.Measure) return AddLast(item);
			if (index == 0) return AddFirst(item);

			return new FlexibleList<T>(_root.Insert(index, new Leaf<T>(item)));
		}

		/// <summary>
		/// Returns a slice of the collection.
		/// </summary>
		/// <param name="from">The first index of the slice.</param>
		/// <param name="to">The last index of the slice.</param>
		/// <returns></returns>
		public FlexibleList<T> this[int from, int to]
		{
			get
			{
				return Slice(from, to);
			}
		}

		/// <summary>
		///   Inserts a list immediately before the specified index. O(logn), slow.
		/// </summary>
		/// <param name="index"> The position. Can be negative. </param>
		/// <param name="list"> The list to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FlexibleList<T> InsertList(int index, FlexibleList<T> list)
		{
			index = index < 0 ? _root.Measure + index + 1 : index;
			if (index > _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (list == null) throw Errors.Argument_null("list");
			if (index == 0) return list.AddLastRange(this);
			if (index == _root.Measure) return AddLastRange(list);
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			_root.Split(index + 1, out part1, out part2);
			part1 = part1.AddRight(list._root);
			var result = part1.AddRight(part2);

			return new FlexibleList<T>(result);
		}

		/// <summary>
		///   Inserts a sequence of items immediately before the specified index.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <param name="items"> The sequence of items to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the IEnumerable is null."/&gt;</exception>
		public FlexibleList<T> InsertRange(int index, IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			index = index < 0 ? index + _root.Measure + 1 : index;
			if (index >= _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (items is FlexibleList<T>)
			{
				return this.InsertList(index, items as FlexibleList<T>);
			}
			
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			_root.Split(index + 1, out part1, out part2);
			foreach (var item in items)
			{
				part1 = part1.AddRight(new Leaf<T>(item));
			}
			part1 = part1.AddRight(part2);
			return new FlexibleList<T>(part1);
		}

		/// <summary>
		///   Removes the element at some index from the list.
		/// </summary>
		/// <param name="index"> The index to remove. </param>
		/// <returns> </returns>
		public FlexibleList<T> Remove(int index)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index < 0 || index >= _root.Measure) throw Errors.Arg_out_of_range("index");
			return new FlexibleList<T>(_root.Remove(index));
		}

		/// <summary>
		///   Reverses the list. O(n).
		/// </summary>
		/// <returns> </returns>
		public FlexibleList<T> Reverse()
		{
			return new FlexibleList<T>(_root.Reverse());
		}

		/// <summary>
		///   Sets the value of the item at the specified index. O(logn), fast
		/// </summary>
		/// <param name="index"> The index of the item to update. </param>
		/// <param name="item"> The new value of the item. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FlexibleList<T> Set(int index, T item)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index >= _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (index == 0) return DropFirst().AddFirst(item);
			if (index == _root.Measure - 1) return DropLast().AddLast(item);
			return new FlexibleList<T>(_root.Set(index, new Leaf<T>(item)));
		}

		/// <summary>
		///   Returns sublist beginning at the start index and ending at the end index. O(logn), slow.
		/// </summary>
		/// <param name="start"> The first index of the subsequence, inclusive. </param>
		/// <param name="end"> The last index of the subsequence, inclusive. Defaults to the last index. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if one of the indices does not exist in the data structure.</exception>
		public FlexibleList<T> Slice(int start, int end = -1)
		{
			start = start < 0 ? start + _root.Measure : start;
			end = end < 0 ? end + _root.Measure : end;
			if (start < 0 || start > _root.Measure) throw Errors.Arg_out_of_range("start");
			if (end < 0 || end > _root.Measure) throw Errors.Arg_out_of_range("end");
			if (end < start) throw Errors.Arg_out_of_range("start");
			if (start == end)
			{
				var res = _root[start];
				return new FlexibleList<T>(emptyFTree.AddRight(res));
			}
			FingerTree<T>.FTree<Leaf<T>> first, last;
			_root.Split(start, out first, out last);
			last.Split(end - start + 1, out first, out last);
			return new FlexibleList<T>(first);
		}

		/// <summary>
		///   Splits the list at the specified index. The boundary element becomes part of the first list. O(logn)
		/// </summary>
		/// <param name="index"> The index at which to split. </param>
		/// <param name="first"> An output parameter that returns the first part of the sequence. </param>
		/// <param name="last"> An output parameter that returns the second part of the sequence. </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the list.</exception>
		
		internal void Split(int index, out FlexibleList<T> first, out FlexibleList<T> last)
		{
			index = index < 0 ? index + _root.Measure : index;
			if (index >= _root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			FingerTree<T>.FTree<Leaf<T>> left, right;
			_root.Split(index + 1, out left, out right);
			first = new FlexibleList<T>(left);
			last = new FlexibleList<T>(right);
		}

		/// <summary>
		///   Returns a sublist consisting of the first items for which the conditional returns true. O(logn+m)
		/// </summary>
		/// <param name="conditional"> The function. </param>
		/// <returns> </returns>
		public FlexibleList<T> TakeWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			var lastIndex = IndexOf(v => !conditional(v));
			if (lastIndex.HasValue) return Slice(0, lastIndex.Value);
			return this;
		}

		/// <summary>
		///   Converts a list to an array.
		/// </summary>
		/// <returns> </returns>
		public T[] ToArray()
		{
			var arr = new T[Count];
			CopyTo(arr, 0);
			return arr;
		}
	}
}