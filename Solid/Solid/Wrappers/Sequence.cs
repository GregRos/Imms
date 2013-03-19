using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Solid.Common;
using Solid.FingerTree;
using Solid.FingerTree.Iteration;

namespace Solid
{
	public static class Sequence
	{
		public static Sequence<T> Concat<T>(params Sequence<T>[] seqs)
		{
			Sequence<T> result = Sequence<T>.Empty;
			foreach (var seq in seqs)
			{
				result = result.Append(seq);
			}
			return result;
		}

		/// <summary>
		///   Gets an empty DelaySequence.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		/// <remarks>
		///   All empty DelaySequence objects of the same generic type are guaranteed to be reference equal.
		///   Empty DelaySequence objects of different generic types may or may not be reference equal."/>
		/// </remarks>
		public static Sequence<T> Empty<T>()
		{
			return Sequence<T>.Empty;
		}

		/// <summary>
		///   Creates a DelaySequence from a number of items.
		/// </summary>
		/// <typeparam name="T"> The type of the sequence. </typeparam>
		/// <param name="items"> The items from which to construct the sequence. </param>
		/// <returns> </returns>
		public static Sequence<T> FromItems<T>(params T[] items)
		{
			return Sequence<T>.Empty.AddRangeLast(items);
		}
	}

	[DebuggerTypeProxy(typeof (Sequence<>.SequenceDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public sealed class Sequence<T>
		: IEnumerable<T>
	{
		internal static readonly Sequence<T> empty = new Sequence<T>(FTree<Value<T>>.Empty);

		private readonly FTree<Value<T>> root;

		internal Sequence(FTree<Value<T>> root)
		{
			this.root = root;
		}

		/// <summary>
		///   Gets the item with the specified index from the DelaySequence.
		/// </summary>
		/// <param name="index"> The index of the item to get. </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a low-moderate impact on performance. O(logn).
		/// </remarks>
		[Pure]
		public T this[int index]
		{
			get
			{
				if (index == 0) return First;
				if (index == root.Measure - 1) return Last;
				if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
				var v = root.Get(index) as Value<T>;
				return v.Content;
			}
		}

		/// <summary>
		///   Gets the number of items in the DelaySequence.
		/// </summary>
		/// <remarks>
		///   This member has no impact on performance.
		/// </remarks>
		public int Count
		{
			get
			{
				return root.Measure;
			}
		}

		/// <summary>
		///   Gets the first item in the DelaySequence.
		/// </summary>
		/// <remarks>
		///   This member has no impact on performance.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public T First
		{
			get
			{
				if (root.Measure == 0) throw Errors.Is_empty;
				return root.Left.Content;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		/// <summary>
		///   Gets the last item in the DelaySequence.
		/// </summary>
		/// <remarks>
		///   This member has no impact on performance.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
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
				return string.Format("DelaySequence, Count = {0}", Count);
			}
		}

		/// <summary>
		///   Adds the specified item at the beginning of the DelaySequence.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		/// <remarks>
		///   This member has a slight impact on performance. O(1) amortized.
		/// </remarks>
		public Sequence<T> AddFirst(T item)
		{
			return new Sequence<T>(root.AddLeft(new Value<T>(item)));
		}

		/// <summary>
		///   Adds the specified item at the end of the DelaySequence.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		/// <remarks>
		///   This member has a slight impact on performance. O(1) amortized.
		/// </remarks>
		public Sequence<T> AddLast(T item)
		{
			return new Sequence<T>(root.AddRight(new Value<T>(item)));
		}

		/// <summary>
		///   Adds a sequence of items at the beginning of the collection.
		/// </summary>
		/// <param name="items"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <remarks>
		/// </remarks>
		public Sequence<T> AddRangeFirst(IEnumerable<T> items)
		{
			if (!items.Any()) return this;
			if (items == null) throw Errors.Argument_null("items");
			FTree<Value<T>> ftree = FTree<Value<T>>.Empty;
			foreach (T item in items)
			{
				ftree = ftree.AddRight(new Value<T>(item));
			}
			ftree = FTree<Value<T>>.Concat(ftree, root);
			return new Sequence<T>(ftree);
		}

		/// <summary>
		///   Adds a sequence of items after the end of the collection.
		/// </summary>
		/// <param name="items"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <remarks>
		///   This member iterates over the Inner, but does not need to rebuild much of the data structure.
		/// </remarks>
		public Sequence<T> AddRangeLast(IEnumerable<T> items)
		{
			if (!items.Any()) return this;
			if (items == null) throw Errors.Argument_null("items");
			FTree<Value<T>> ftree = root;
			foreach (T item in items)
			{
				ftree = ftree.AddRight(new Value<T>(item));
			}
			return new Sequence<T>(ftree);
		}

		/// <summary>
		///   Appends a sequence to the end of this one.
		/// </summary>
		/// <param name="other"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <remarks>
		///   This member has a moderate impact on performance. O(logn).
		/// </remarks>
		public Sequence<T> Append(Sequence<T> other)
		{
			if (other == null) throw Errors.Argument_null("other");
			if (other.IsEmpty) return this;
			FTree<Value<T>> result = FTree<Value<T>>.Concat(root, other.root);
			return new Sequence<T>(result);
		}

		/// <summary>
		///   Applies a transformation on every item in the DelaySequence.
		/// </summary>
		/// <typeparam name="TOut"> The output type of the transformation. </typeparam>
		/// <param name="transform"> </param>
		/// <returns> </returns>
		/// <remarks>
		///   This member has a substantial impact on performance. O(n).
		/// </remarks>
		public Sequence<TOut> Apply<TOut>(Func<T, TOut> transform)
		{
			if (transform == null) throw Errors.Argument_null("transform");
			FTree<Value<TOut>> newRoot = FTree<Value<TOut>>.Empty;
			ForEach(v => newRoot.AddRight(new Value<TOut>(transform(v))));
			return new Sequence<TOut>(newRoot);
		}

		/// <summary>
		///   Removes the first item from the collection.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		/// <remarks>
		///   This member has a slight on performance. O(1) amortized.
		/// </remarks>
		public Sequence<T> DropFirst()
		{
			if (root.Measure == 0) throw Errors.Is_empty;
			return new Sequence<T>(root.DropLeft());
		}

		/// <summary>
		///   Removes the last item from the DelaySequence.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		/// <remarks>
		///   This member has a slight impact on performance. O(1) amortized.
		/// </remarks>
		public Sequence<T> DropLast()
		{
			if (root.Measure == 0) throw Errors.Is_empty;
			return new Sequence<T>(root.DropRight());
		}


		/// <summary>
		///   Applies a function on every member of the collection, starting from the first element.
		/// </summary>
		/// <param name="forEach"> </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <remarks>
		///   This member iterates over the entire DelaySequence and may have a substantial impact on performance..
		/// </remarks>
		public void ForEach(Action<T> forEach)
		{
			if (forEach == null) throw Errors.Argument_null("forEach");
			root.Iter(forEach);
		}

		/// <summary>
		///   Applies a function on every member of the collection, starting from the last element.
		/// </summary>
		/// <param name="forEach"> </param>
		/// <remarks>
		///   This member iterates over the entire DelaySequence and may have a substantial impact on performance.
		/// </remarks>
		public void ForEachBack(Action<T> forEach)
		{
			if (forEach == null) throw Errors.Argument_null("forEach");
			root.IterBack(forEach);
		}

		/// <summary>
		///   Inserts an item at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="item"> </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a moderate-high impact on performance.
		/// </remarks>
		public Sequence<T> Insert(int index, T item)
		{
			if (index == root.Measure - 1) return AddLast(item);
			if (index == 0) return AddFirst(item);
			;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			part1 = part1.AddRight(new Value<T>(item));
			FTree<Value<T>> result = FTree<Value<T>>.Concat(part1, part2);
			return new Sequence<T>(result);
		}


		/// <summary>
		///   Inserts a collection at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="other"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <remarks>
		/// </remarks>
		public Sequence<T> Insert(int index, Sequence<T> other)
		{
			if (index == 0) return other.Append(this);
			if (index == root.Measure - 1) return Append(other);
			if (other == null) throw Errors.Argument_null("other");
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			part1 = FTree<Value<T>>.Concat(part1, other.root);
			FTree<Value<T>> result = FTree<Value<T>>.Concat(part1, part2);

			return new Sequence<T>(result);
		}

		/// <summary>
		///   Inserts a sequence of items at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="items"> </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the IEnumerable is null."/></exception>
		/// <remarks>
		///   This member has (at least) a moderate-high impact on performance.
		/// </remarks>
		public Sequence<T> InsertRange(int index, IEnumerable<T> items)
		{
			if (!items.Any()) return this;
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (items == null) throw Errors.Argument_null("items");
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			foreach (T item in items)
			{
				part1 = part1.AddRight(new Value<T>(item));
			}
			part1 = FTree<Value<T>>.Concat(part1, part2);
			return new Sequence<T>(part1);
		}

		/// <summary>
		///   Reverses the DelaySequence.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		///   This member has a substantial impact on performance. O(n)
		/// </remarks>
		public Sequence<T> Reverse()
		{
			return new Sequence<T>(root.Reverse());
		}

		/// <summary>
		///   Sets the item at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="item"> </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a moderate-high impact on performance.
		/// </remarks>
		public Sequence<T> Set(int index, T item)
		{
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			if (index == 0) return DropFirst().AddFirst(item);
			if (index == root.Measure - 1) return DropLast().AddLast(item);
			FTree<Value<T>> part1, part2;
			root.Split(index + 1, out part1, out part2);
			part1 = part1.DropRight();
			part1 = part1.AddRight(new Value<T>(item));
			part1 = FTree<Value<T>>.Concat(part1, part2);
			return new Sequence<T>(part1);
		}

		public Sequence<T> Skip(int count)
		{
			return TakeLast(Count - count);
		}

		/// <summary>
		///   Returns a subsequence beginning at the specified index, and consisting of the specified number of items.
		/// </summary>
		/// <param name="index"> The start of the subsequence. </param>
		/// <param name="count"> The length of the subsequence. </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a moderate impact on performance. O(logn).
		/// </remarks>
		public Sequence<T> Slice(int index, int count)
		{
			if (index + count >= root.Measure || index < 0 || count < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> left, right;
			if (index == 0) return Take(count);
			if (index + count == root.Measure) return Take(count);

			root.Split(index + count + 1, out left, out right);
			left.Split(count, out left, out right);
			return new Sequence<T>(left);
		}

		/// <summary>
		///   Splits the DelaySequence at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="first"> An output parameter that returns the first part of the sequence. </param>
		/// <param name="last"> An output parameter that returns the second part of the sequence. </param>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist in the collection.</exception>
		/// <remarks>
		///   This member has a moderate impact on performance. O(logn).
		/// </remarks>
		public void Split(int index, out Sequence<T> first, out Sequence<T> last)
		{
			if (index >= root.Measure || index < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> left, right;
			root.Split(index + 1, out left, out right);
			first = new Sequence<T>(left);
			last = new Sequence<T>(right);
		}

		/// <summary>
		///   Returns a subsequence beginning from the first item, and consisting of the specified number of items.
		/// </summary>
		/// <param name="count"> The size of the subsequence. </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a moderate impact on performance. O(logn)
		/// </remarks>
		public Sequence<T> Take(int count)
		{
			if (count == 0) return Empty;
			if (count == root.Measure) return this;
			if (count > root.Measure || count < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> left, right;
			root.Split(count, out left, out right);
			return new Sequence<T>(left);
		}

		/// <summary>
		///   Returns a subsequence beginning from the last item, and consisting of the specified number of items.
		/// </summary>
		/// <param name="count"> The size of the subsequence. </param>
		/// <returns> </returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index doesn't exist.</exception>
		/// <remarks>
		///   This member has a moderate impact on performance. O(logn)
		/// </remarks>
		public Sequence<T> TakeLast(int count)
		{
			if (count == 0) return Empty;
			if (count == root.Measure) return this;
			if (count > root.Measure || count < 0) throw Errors.Index_out_of_range;
			FTree<Value<T>> left, right;
			root.Split(count, out left, out right);
			return new Sequence<T>(right);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new EnumeratorWrapper<T>(root.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new EnumeratorWrapper<T>(root.GetEnumerator());
		}

		public static Sequence<T> Empty
		{
			get
			{
				return empty;
			}
		}

		public static Sequence<T> Concat(Sequence<T> seq1, Sequence<T> seq2)
		{
			return seq1.Append(seq2);
		}


		private class SequenceDebugView
		{
			private readonly Sequence<T> _inner;

			public SequenceDebugView(Sequence<T> inner)
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
	}
}