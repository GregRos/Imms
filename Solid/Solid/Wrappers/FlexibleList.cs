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
	[DebuggerTypeProxy(typeof (FlexibleList<>.FlexibleListDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class FlexibleList<T>
		: IList<T>
	{

		private class FinalEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<Leaf<T>> inner;

			public FinalEnumerator(IEnumerator<Leaf<T>> inner)
			{
				this.inner = inner;
			}

			public void Dispose()
			{
				
			}

			public bool MoveNext()
			{
				return inner.MoveNext();
			}

			public void Reset()
			{
				
			}

			public T Current
			{
				get
				{
					return inner.Current.Value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}
		}
		
		/// <summary>
		///   Converts a list to an array.
		/// </summary>
		/// <returns> </returns>
		public  T[] ToArray()
		{
			var arr = new T[this.Count];
			this.CopyTo(arr,0);
			return arr;
		}
		private class FlexibleListDebugView
		{
			private readonly FlexibleList<T> _inner;

			public FlexibleListDebugView(FlexibleList<T> inner)
			{
				_inner = inner;
			}

			public int Count
			{
				get
				{
					return _inner.Count;
				}
			}

			public T First
			{
				get
				{
					return _inner.First;
				}
			}

			public T Last
			{
				get
				{
					return _inner.Last;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Zitems
			{
				get
				{
					return _inner.ToArray();
				}
			}
		}

		private static readonly FingerTree<T>.FTree<Leaf<T>> emptyFTree = FingerTree<T>.FTree<Leaf<T>>.Empty;
		internal static readonly FlexibleList<T> empty = new FlexibleList<T>(emptyFTree);
		private readonly FingerTree<T>.FTree<Leaf<T>> root;

		internal FlexibleList(FingerTree<T>.FTree<Leaf<T>> root)
		{
			this.root = root;
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
		///   Concatenates one list to the end of the other.
		/// </summary>
		/// <param name="list1"> The first list. </param>
		/// <param name="list2"> The second . </param>
		/// <returns> </returns>
		public static FlexibleList<T> Concat(FlexibleList<T> list1, FlexibleList<T> list2)
		{
			return list1.AddLastList(list2);
		}

		void IList<T>.RemoveAt(int index)
		{
			throw Errors.Collection_readonly;
		}

		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw Errors.Collection_readonly;
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
				index = index < 0 ? index + root.Measure : index;
				if (index == 0) return First;
				if (index == root.Measure - 1) return Last;
				if (index >= root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
				var v = root[index];
				return v.Value;
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<T>.Clear()
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<T>.Contains(T item)
		{
			return this.IndexOf(x => item.Equals(x)).HasValue;
		}
		/// <summary>
		/// Copies the collection into the specified array..
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="arrayIndex">The index at which to start copying.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.ForEach(v =>
			             {
				             array[arrayIndex] = v;
				             arrayIndex++;
			             });
			
		}

		bool ICollection<T>.Remove(T item)
		{
			throw Errors.Collection_readonly;
		}

		/// <summary>
		/// Converts the specified array into a FlexibleList.
		/// </summary>
		/// <param name="arr">The array.</param>
		/// <returns></returns>
		public static implicit operator FlexibleList<T> (T[] arr)
		{
			return Empty.AddLastRange(arr);
		}


		/// <summary>
		///   Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get
			{
				return root.Measure;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns an enumerator for iterating over the collection first element to last.
		/// </summary>
		public IEnumerable<T> Forward
		{
			get
			{
				return this;
			}
		}
		/// <summary>
		/// Returns an enumerator for iterating over the collection from last to first.
		/// </summary>
		public IEnumerable<T> Backward
		{
			get
			{
				return new EnumerableProxy<T>(() => new FinalEnumerator(root.GetEnumerator(false)));
			}
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("FlexibleList, Count = {0}", Count);
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
				if (root.Measure == 0) throw Errors.Is_empty;
				return root.Left.Value;
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
				if (root.Measure == 0) throw Errors.Is_empty;
				return root.Right.Value;
			}
		}

		/// <summary>
		///   Adds the specified item at the beginning of the list. O(1) amortized.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FlexibleList<T> AddFirst(T item)
		{
			return new FlexibleList<T>(root.AddLeft(new Leaf<T>(item)));
		}

		/// <summary>
		/// Adds the specified items, in order, to the beginning of the list. O(m)
		/// </summary>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argumnet is null.</exception>
		public FlexibleList<T> AddFirstItems(params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return AddFirstRange(items);
		}

		///<summary>
		///  Joins the specified list to the beginning of this one. O(logn), fast
		///</summary>
		///<param name="list"> The list. </param>
		///<returns> </returns>
		///<exception cref="ArgumentNullException">Thrown if the argumnet is null.</exception>
		public FlexibleList<T> AddFirstList(FlexibleList<T> list)
		{
			if (list == null) throw Errors.Argument_null("list");
			if (list.IsEmpty) return this;
			var result = list.root.AddLeft(list.root);
			return new FlexibleList<T>(result);
		}

		/// <summary>
		///   Joins the specified sequence to the beginning of this list. O(m)
		/// </summary>
		/// <param name="items"> The items to add. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddFirstRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			if (items is FlexibleList<T>) return this.AddFirstList(items as FlexibleList<T>);
			var ftree = emptyFTree;
			foreach (var item in items)
			{
				ftree = ftree.AddRight(new Leaf<T>(item));
			}
			ftree = ftree.AddRight(root);
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		///   Adds the specified item to the end of the list. O(1) amortized.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FlexibleList<T> AddLast(T item)
		{
			return new FlexibleList<T>(root.AddRight(new Leaf<T>(item)));
		}

		/// <summary>
		///   Adds several items to the end of the list. O(m)
		/// </summary>
		/// <param name="items"> The items to add. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">If the argument is null.</exception>
		public FlexibleList<T> AddLastItems(params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return AddLastRange(items);
		}

		/// <summary>
		///   Joins another list to the end of this one. O(logn), fast
		/// </summary>
		/// <param name="other"> The other list. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddLastList(FlexibleList<T> other)
		{
			if (other == null) throw Errors.Argument_null("list");
			if (other.IsEmpty) return this;
			var result = root.AddRight(other.root);
			return new FlexibleList<T>(result);
		}

		/// <summary>
		///   Adds a sequence of items to the end of the list. O(m)
		/// </summary>
		/// <param name="items"> The sequence. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			if (items is FlexibleList<T>) return this.AddLastList(items as FlexibleList<T>);
			var ftree = root;
			foreach (var item in items)
			{
				ftree = ftree.AddRight(new Leaf<T>(item));
			}
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		///   Applies the accumulator function on the list, from first to last. O(n)
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult Aggregate<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Applies an accumulator over the list, from last to first.  O(n)
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult AggregateBack<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEachBack(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		/// Returns true if all the items in the list fulfill the conditional. O(m)
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public bool All(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			return root.IterWhile(m => predicate(m.Value));
		}

		/// <summary>
		///   Removes the first item from the list. O(1) amortized.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FlexibleList<T> DropFirst()
		{
			if (root.Measure == 0) throw Errors.Is_empty;
			return new FlexibleList<T>(root.DropLeft());
		}

		/// <summary>
		///   Removes the last item from the list. O(1) amortized.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FlexibleList<T> DropLast()
		{
			if (root.Measure == 0) throw Errors.Is_empty;
			return new FlexibleList<T>(root.DropRight());
		}

		/// <summary>
		/// Iterates over the list, from first to last. O(n)
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEach(Action<T> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			root.Iter(x => iterator(x.Value));
		}

		/// <summary>
		/// Iterates over the list, from last to first. O(n)
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachBack(Action<T> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			root.IterBack(x => iterator(x.Value));
		}

		/// <summary>
		///   Iterates over the list, from last to first, and stops if the conditional returns false. O(m)
		/// </summary>
		/// <param name="conditional"> The conditional. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			root.IterBackWhile(m => conditional(m.Value));
		}

		///<summary>
		///  Iterates over the list, from first to last, and stops if the conditional returns false.
		///</summary>
		///<param name="conditional"> The conditional iterator that specifies whether to proceed with iteration. </param>
		///<exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			root.IterWhile(m => conditional(m.Value));
		}

		/// <summary>
		///   Gets a new enumerator that iterates over the list.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<T> GetEnumerator()
		{
			var enumerator = root.GetEnumerator(true);
			for (; enumerator.MoveNext();)
			{
				yield return enumerator.Current.Value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<T>).GetEnumerator();
		}

		/// <summary>
		///   Returns the index of the first item that fulfills the specified conditional, or null.
		/// </summary>
		/// <param name="predicate"> The conditional. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public int? IndexOf(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var index = 0;
			ForEachWhile(v =>
			             {
				             if (predicate(v)) return false;
				             index++;
				             return true;
			             });
			return index > Count ? null : (int?) index;
		}

		int IList<T>.IndexOf(T item)
		{
			var res = this.IndexOf(v => item.Equals(v));
			if (res.HasValue) return res.Value;
			throw Errors.Arg_out_of_range("item");
		}

		void IList<T>.Insert(int index, T item)
		{
			throw Errors.Collection_readonly;
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
			index = index < 0 ? root.Measure + index + 1 : index;
			if (index >= root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (index == root.Measure) return AddLast(item);
			if (index == 0) return AddFirst(item);

			return new FlexibleList<T>(root.Insert(index, new Leaf<T>(item)));
		}

		/// <summary>
		///   Inserts one or more items immediately before the specified index.
		/// </summary>
		/// <param name="index"> The position immediately before which the items are inserted. </param>
		/// <param name="items"> The items. </param>
		/// <returns> </returns>
		/// <example>
		/// </example>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> InsertItems(int index, params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return InsertRange(index, items);
		}

		/// <summary>
		///   Inserts a list immediately before the specified index. O(logn), slow.
		/// </summary>
		/// <param name="index"> The position. Can be negative. </param>
		/// <param name="list"> The list to insert. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		public FlexibleList<T> InsertList(int index, FlexibleList<T> list)
		{
			index = index < 0 ? root.Measure + index + 1 : index;
			if (index > root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (list == null) throw Errors.Argument_null("list");
			if (index == 0) return list.AddLastList(this);
			if (index == root.Measure) return AddLastList(list);
			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			part1 = part1.AddRight(list.root);
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
			index = index < 0 ? index + root.Measure + 1 : index;
			if (index >= root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (!Enumerable.Any(items)) return this;

			FingerTree<T>.FTree<Leaf<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
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
			index = index < 0 ? index + root.Measure : index;
			if (index < 0 || index >= root.Measure) throw Errors.Arg_out_of_range("index");
			return new FlexibleList<T>(root.Remove(index));
		}

		/// <summary>
		///   Reverses the list. O(n).
		/// </summary>
		/// <returns> </returns>
		public FlexibleList<T> Reverse()
		{
			return new FlexibleList<T>(root.Reverse());
		}

		/// <summary>
		///   Transforms the list by applying the specified selector on every item in the list. O(n).
		/// </summary>
		/// <typeparam name="TOut"> The output type of the transformation. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<TOut> Select<TOut>(Func<T, TOut> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			var newRoot = FlexibleList<TOut>.emptyFTree;
			ForEach(v => newRoot.AddRight(new Leaf<TOut>(selector(v))));
			return new FlexibleList<TOut>(newRoot);
		}

		///<summary>
		///  Applies the specified selector on every item in the list, flattens, and constructs a new list. O(n·m)
		///</summary>
		///<typeparam name="TResult"> The type of the result. </typeparam>
		///<param name="selector"> The selector. </param>
		///<returns> </returns>
		///<exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			var result = FlexibleList<TResult>.Empty;
			ForEach(v => result = result.AddLastRange(selector(v)));
			return result;
		}

		///<summary>
		///  Checks if the specified sequence is equal to the list. O(n)
		///</summary>
		///<param name="other"> The list list. </param>
		///<param name="comparer"> The comparer. </param>
		///<returns> </returns>
		///<exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public bool SequenceEqual(IEnumerable<T> other, IEqualityComparer<T> comparer = null)
		{
			if (other == null) throw Errors.Argument_null("list");
			comparer = comparer ?? EqualityComparer<T>.Default;

			var state = other.GetEnumerator();
			return root.IterWhile(v => state.MoveNext() ? comparer.Equals((v as Leaf<T>).Value, state.Current) : false);
		}

		/// <summary>
		///   Sets the value of the item at the specified index. O(logn), fast
		/// </summary>
		/// <param name="index">The index of the item to update.</param>
		/// <param name="item">The new value of the item.</param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FlexibleList<T> Set(int index, T item)
		{
			index = index < 0 ? index + root.Measure : index;
			if (index >= root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			if (index == 0) return DropFirst().AddFirst(item);
			if (index == root.Measure - 1) return DropLast().AddLast(item);
			return new FlexibleList<T>(root.Set(index, new Leaf<T>(item)));
		}

		/// <summary>
		///   Returns subslist without the first several elements. O(logn),slow
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
		/// <returns> </returns>
		public FlexibleList<T> Skip(int count)
		{
			return Slice(count, -1);
		}

		/// <summary>
		///   Returns a list without the first elements for which the conditional returns true. O(logn+m)
		/// </summary>
		/// <param name="predicate">The function </param>
		/// <returns> </returns>
		/// <exception cref="NullReferenceException">Thrown if the conditional is null.</exception>
		public FlexibleList<T> SkipWhile(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var lastIndex = IndexOf(v => !predicate(v));
			return lastIndex.HasValue ? Slice((int) lastIndex) : empty;
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
			start = start < 0 ? start + root.Measure : start;
			end = end < 0 ? end + root.Measure : end;
			if (start < 0 || start > root.Measure) throw Errors.Arg_out_of_range("start");
			if (end < 0 || end > root.Measure) throw Errors.Arg_out_of_range("end");
			if (end < start) throw Errors.Arg_out_of_range("start");
			if (start == end)
			{
				var res = root[start];
				return new FlexibleList<T>(emptyFTree.AddRight(res));
			}
			FingerTree<T>.FTree<Leaf<T>> first, last;
			root.Split(start, out first, out last);
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
		public void Split(int index, out FlexibleList<T> first, out FlexibleList<T> last)
		{
			index = index < 0 ? index + root.Measure : index;
			if (index >= root.Measure || index < 0) throw Errors.Arg_out_of_range("index");
			FingerTree<T>.FTree<Leaf<T>> left, right;
			root.Split(index + 1, out left, out right);
			first = new FlexibleList<T>(left);
			last = new FlexibleList<T>(right);
		}

		/// <summary>
		///   Returns a sublist consisting of the first  items. O(logn), slow
		/// </summary>
		/// <param name="count"> The length of the sublist. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified count is outside of bounds.</exception>
		public FlexibleList<T> Take(int count)
		{
			return Slice(0, count - 1);
		}

		/// <summary>
		///   Returns a sublist consisting of the first items for which the conditional returns true. O(logn+m)
		/// </summary>
		/// <param name="conditional">The function.</param>
		/// <returns> </returns>
		public FlexibleList<T> TakeWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			var lastIndex = IndexOf(v => !conditional(v));
			if (lastIndex.HasValue) return Slice(0, lastIndex.Value);
			return this;
		}

		/// <summary>
		///   Returns a list consisting of all the elements for which the conditional returns true.
		/// </summary>
		/// <param name="predicate"> The conditional used to filter the list. </param>
		/// <returns> </returns>
		public FlexibleList<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var newList = emptyFTree;
			root.Iter(leaf =>
			          {
				          if (predicate(leaf.Value))
				          {
					          newList = newList.AddRight(leaf);
				          }
			          });
			return new FlexibleList<T>(newList);
		}
	}
}