using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Solid.Common;
using Solid.FingerTree;
using Solid.FingerTree.Iteration;
using Microsoft.FSharp.Core;
namespace Solid
{
	[DebuggerTypeProxy(typeof (FlexibleList<>.FlexibleListDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	
	public sealed class FlexibleList<T>
		: IEnumerable<T>
	{
		internal static readonly FlexibleList<T> empty = new FlexibleList<T>(FTree<Value<T>>.Empty);

		private readonly FTree<Value<T>> root;

		internal FlexibleList(FTree<Value<T>> root)
		{
			this.root = root;
		}
	
		private string FSharpFormatDisplay
		{
			get
			{
				var buildr = new StringBuilder(this.Count);
				foreach (var item in this)
				{
					buildr.Append(item);
				}
				return "";
			}
		}

		/// <summary>
		/// Gets the item at the specified index. O(logn), very fast.
		/// </summary>
		/// <param name="index">The index of the item to get. Can be negative.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public T this[int index]
		{
			get
			{
				
				index = index < 0 ? index + root.Measure : index;
				if (index == 0) return First;
				if (index == root.Measure - 1) return Last;
				if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
				var v = root.Get(index) as Value<T>;
				return v.Content;
			}
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

		/// <summary>
		///   Gets the first item in the list. O(1).
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public T First
		{
			get
			{
				if (root.Measure == 0) throw Errors.Is_empty;
				return root.Left.Content;
			}
		}

		public FlexibleList<T> Remove(int index)
		{
			return new FlexibleList<T>(root.Remove(index));
		}

		/// <summary>
		/// Returns true if the list is empty. O(1)
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
				return root.Right.Content;
			}
		}


		private string DebuggerDisplay
		{
			get
			{
				return string.Format("FlexibleList, Count = {0}", Count);
			}
		}

		/// <summary>
		///   Adds the specified item at the beginning of the list. O(1) amortized.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		public FlexibleList<T> AddFirst(T item)
		{
			return new FlexibleList<T>(root.AddLeft(new Value<T>(item)));
		}

		/// <summary>
		/// Adds the specified items, in order, to the beginning of the list. O(m)
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>

		public FlexibleList<T> AddFirstItems(params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return AddFirstRange(items);
		}

		/// <summary>
		/// Joins the specified list to the beginning of this one. O(logn), fast
		/// </summary>
		/// <param name="list">The list.</param>
		/// <returns></returns>

		public FlexibleList<T> AddFirstList(FlexibleList<T> list)
		{
			if (list == null) throw Errors.Argument_null("list");
			if (list.IsEmpty) return this;
			FTree<Value<T>> result = FTree<Value<T>>.Concat(list.root, root);
			return new FlexibleList<T>(result);
		}

		/// <summary>
		/// Joins the specified sequence to the beginning of this list. O(m)
		/// </summary>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>

		public FlexibleList<T> AddFirstRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");


			FTree<Value<T>> ftree = FTree<Value<T>>.Empty;
			foreach (T item in items)
			{
				ftree = ftree.AddRight(new Value<T>(item));
			}
			ftree = FTree<Value<T>>.Concat(ftree, root);
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		/// Adds the specified item to the end of the list. O(1) amortized.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public FlexibleList<T> AddLast(T item)
		{
			return new FlexibleList<T>(root.AddRight(new Value<T>(item)));
		}

		/// <summary>
		/// Adds several items to the end of the list. O(m)
		/// </summary>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">If the argument is null.</exception>
		public FlexibleList<T> AddLastItems(params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return AddLastRange(items);
		}

		/// <summary>
		/// Joins another list to the end of this one. O(logn), fast
		/// </summary>
		/// <param name="other">The other list.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddLastList(FlexibleList<T> other)
		{
			if (other == null) throw Errors.Argument_null("list");
			if (other.IsEmpty) return this;
			FTree<Value<T>> result = FTree<Value<T>>.Concat(root, other.root);
			return new FlexibleList<T>(result);
		}

		/// <summary>
		/// Adds a sequence of items to the end of the list. O(m)
		/// </summary>
		/// <param name="items">The sequence.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			FTree<Value<T>> ftree = root;
			foreach (T item in items)
			{
				ftree = ftree.AddRight(new Value<T>(item));
			}
			return new FlexibleList<T>(ftree);
		}

		/// <summary>
		/// Applies the accumulator function on the list, from first to last. O(n)
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="initial">The initial value.</param>
		/// <returns></returns>
		public TResult Aggregate<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		/// Applies an accumulator over the list, from last to first.  O(n)
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="initial">The initial value.</param>
		/// <returns></returns>
		public TResult AggregateBack<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			
			ForEachBack(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		/// Removes the first item from the list. O(1) amortized.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
		public FlexibleList<T> DropFirst()
		{
			if (root.Measure == 0) throw Errors.Is_empty;
			return new FlexibleList<T>(root.DropLeft());
		}

		/// <summary>
		/// Removes the last item from the list. O(1) amortized.
		/// </summary>
		/// <returns></returns>
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
			root.Iter(iterator);
		}

		/// <summary>
		/// Iterates over the list, from last to first. O(n)
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		public void ForEachBack(Action<T> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			root.IterBack(iterator);
		}

		/// <summary>
		/// Iterates over the list, from last to first, and stops if the conditional returns false. O(m)
		/// </summary>
		/// <param name="conditional">The conditional.</param>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			root.IterBackWhile(m => conditional((m as Value<T>).Content));
		}

		/// <summary>
		/// Iterates over the list, from first to last, and stops if the conditional returns false.
		/// </summary>
		/// <param name="conditional">The conditional iterator that specifies whether to proceed with iteration.</param>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			root.IterWhile(m => conditional((m as Value<T>).Content));
		}

		/// <summary>
		/// Gets a new enumerator that iterates over the list.
		/// </summary>
		/// <returns></returns>
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			
			return new EnumeratorWrapper<T>(root.GetEnumerator());
		}

		/// <summary>
		/// Returns the index of the first item that fulfills the specified predicate, or null.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public int? IndexOf(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			int index = 0;
			ForEachWhile(v =>
			             {
				             if (predicate(v)) return false;
				             index++;
				             return true;
			             });
			return index > Count ? null : (int?) index;
		}

		/// <summary>
		/// Inserts an item immediately before the specified index. O(logn), fast.
		/// </summary>
		/// <param name="index">The index before which to insert the item. Can be negative.</param>
		/// <param name="item">The item to insert.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FlexibleList<T> Insert(int index, T item)
		{
		
			index = index < 0 ? root.Measure + index + 1: index;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (index == root.Measure) return AddLast(item);
			if (index == 0) return AddFirst(item);


			return new FlexibleList<T>(root.Insert(index, new Value<T>(item)));
		}

		/// <summary>
		/// Inserts one or more items immediately before the specified index. 
		/// </summary>
		/// <param name="index">The position immediately before which the items are inserted.</param>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		/// <example></example>
		public FlexibleList<T> InsertItems(int index, params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return InsertRange(index, items);
		}

		/// <summary>
		/// Inserts a list immediately before the specified index. O(logn), slow.
		/// </summary>
		/// <param name="index">The position. Can be negative.</param>
		/// <param name="list">The list to insert.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index does not exist in the list.</exception>
		public FlexibleList<T> InsertList(int index, FlexibleList<T> list)
		{
			index = index < 0 ? root.Measure + index + 1 : index;
			if (index > root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (list == null) throw Errors.Argument_null("list");
			if (index == 0) return list.AddLastList(this);
			if (index == root.Measure) return AddLastList(list);
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			part1 = FTree<Value<T>>.Concat(part1, list.root);
			FTree<Value<T>> result = FTree<Value<T>>.Concat(part1, part2);

			return new FlexibleList<T>(result);
		}

		/// <summary>
		/// Inserts a sequence of items immediately before the specified index. 
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="items">The sequence of items to insert.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the IEnumerable is null."/&gt;</exception>
		public FlexibleList<T> InsertRange(int index, IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			index = index < 0 ? index + root.Measure +1: index;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (!System.Linq.Enumerable.Any(items)) return this;
			
			
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			foreach (T item in items)
			{
				part1 = part1.AddRight(new Value<T>(item));
			}
			part1 = FTree<Value<T>>.Concat(part1, part2);
			return new FlexibleList<T>(part1);
		}

		/// <summary>
		/// Reverses the list. O(n).
		/// </summary>
		/// <returns></returns>
		public FlexibleList<T> Reverse()
		{
			return new FlexibleList<T>(root.Reverse());
		}

		/// <summary>
		/// Transforms the list by applying the specified selector on every item in the list. O(n).
		/// </summary>
		/// <typeparam name="TOut">The output type of the transformation.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FlexibleList<TOut> Select<TOut>(Func<T, TOut> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			FTree<Value<TOut>> newRoot = FTree<Value<TOut>>.Empty;
			ForEach(v => newRoot.AddRight(new Value<TOut>(selector(v))));
			return new FlexibleList<TOut>(newRoot);
		}

		/// <summary>
		/// Applies the specified selector on every item in the list, flattens, and constructs a new list. O(n·m)
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FlexibleList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			FlexibleList<TResult> result = FlexibleList<TResult>.Empty;
			ForEach(v => result = result.AddLastRange(selector(v)));
			return result;
		}

		/// <summary>
		/// Checks if the specified sequence is equal to the list. O(n)
		/// </summary>
		/// <param name="other">The list list.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns></returns>
		public bool SequenceEqual(IEnumerable<T> other, IEqualityComparer<T> comparer = null)
		{
			if (other == null) throw Errors.Argument_null("list");
			comparer = comparer ?? EqualityComparer<T>.Default;

			IEnumerator<T> state = other.GetEnumerator();
			return root.IterWhile(v => state.MoveNext() ? comparer.Equals((v as Value<T>).Content, state.Current) : false);
		}

		/// <summary>
		/// Returns a sublist consisting of the first  items. O(logn), slow
		/// </summary>
		/// <param name="count">The length of the sublist.</param>
		/// <returns></returns>
		public FlexibleList<T> Take(int count)
		{
			return this.Slice(0, count - 1);
		}
		/// <summary>
		/// Returns subslist without the first several elements. O(logn),slow
		/// </summary>
		/// <param name="count">The number of elements to skip. </param>
		/// <returns></returns>
		public FlexibleList<T> Skip(int count)
		{
			return this.Slice(count, -1);
		}

		/// <summary>
		/// Sets the value of the item at the specified index. O(logn), fast
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="item"> </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		public FlexibleList<T> Set(int index, T item)
		{

			index = index < 0 ? index + root.Measure : index;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (index == 0) return DropFirst().AddFirst(item);
			if (index == root.Measure - 1) return DropLast().AddLast(item);
			return new FlexibleList<T>(root.Set(index, new Value<T>(item)));
		}

		/// <summary>
		/// Returns a list without the first elements for which the predicate returns true. O(logn+m)
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException">Thrown if the predicate is null.</exception>
		public FlexibleList<T> SkipWhile(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			int? lastIndex = IndexOf(v => !predicate(v));
			return lastIndex.HasValue ? Slice((int) lastIndex) : empty;
		}

		/// <summary>
		/// Returns sublist beginning at the start index and ending at the end index. O(logn), slow.
		/// </summary>
		/// <param name="start">The first index of the subsequence, inclusive.</param>
		/// <param name="end">The last index of the subsequence, inclusive. Defaults to the last index.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if one of the indices does not exist in the data structure.</exception>
		public FlexibleList<T> Slice(int start, int end=-1)
		{
			start = start < 0 ? start + root.Measure : start;
			end = end < 0 ? end + root.Measure : end;
			if (start < 0 || end < start || start > root.Measure || end > root.Measure) throw Errors.Index_out_of_range;
			if (start == end)
			{
				var res = root.Get(start) as Value<T>;
				return new FlexibleList<T>(FTree<Value<T>>.Empty.AddRight(res));
			}
			FTree<Value<T>> last;
			FTree<Value<T>> first;
			root.Split(start, out first, out last);
			last.Split(end - start + 1, out first, out last);
			return new FlexibleList<T>(first);
		}

		/// <summary>
		///   Splits the list at the specified index. The boundary element becomes part of the first list. O(logn)
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="first"> An output parameter that returns the first part of the sequence. </param>
		/// <param name="last"> An output parameter that returns the second part of the sequence. </param>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist in the list.</exception>
		public void Split(int index, out FlexibleList<T> first, out FlexibleList<T> last)
		{
			index = index < 0 ? index + root.Measure : index;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> left, right;
			root.Split(index + 1, out left, out right);
			first = new FlexibleList<T>(left);
			last = new FlexibleList<T>(right);
		}

		/// <summary>
		/// Returns a sublist consisting of the first items for which the predicate returns true. O(logn+m)
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public FlexibleList<T> TakeWhile(Func<T, bool> predicate)
		{
			
			if (predicate == null) throw Errors.Argument_null("predicate");
			int? lastIndex = IndexOf(v => !predicate(v));
			if (lastIndex.HasValue) return Slice(0, lastIndex.Value);
			return this;
		}
		

		/// <summary>
		/// Returns true if all the items in the list fulfill the predicate. O(m)
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public bool All(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			return root.IterWhile(m => predicate((m as Value<T>).Content));
		}

		/// <summary>
		/// Returns a list consisting of all the elements for which the predicate returns true. 
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public FlexibleList<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			FTree<Value<T>> newList = FTree<Value<T>>.Empty;
			root.Iter(m =>
			          {
				          var asValue = m as Value<T>;
				          if (predicate(asValue.Content))
				          {
					          newList = newList.AddRight(asValue);
				          }
			          });
			return new FlexibleList<T>(newList);
		}

		/// <summary>
		/// Gets the empty list.
		/// </summary>
		public static FlexibleList<T> Empty
		{
			get
			{
				return empty;
			}
		}

		public static FlexibleList<T> Concat(FlexibleList<T> seq1, FlexibleList<T> seq2)
		{
			return seq1.AddLastList(seq2);
		}

		private class FlexibleListDebugView
		{
			private readonly FlexibleList<T> _inner;

			public FlexibleListDebugView(FlexibleList<T> inner)
			{
				_inner = inner;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
			public T[] Backward
			{
				get
				{
					return _inner.Reverse().ToArray();
				}
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


			[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
			public T[] Forward
			{
				get
				{
					return _inner.ToArray();
				}
			}

			public T Last
			{
				get
				{
					return _inner.Last;
				}
			}
		}

		private class FlexibleListEnumerable
			: IEnumerable<T>
		{
			private readonly FlexibleList<T> inner;

			public FlexibleListEnumerable(FlexibleList<T> inner)
			{
				this.inner = inner;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return (inner as IEnumerable<T>).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			
			return (this as IEnumerable<T>).GetEnumerator();
		}
	}
}