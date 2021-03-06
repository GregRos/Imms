﻿using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	/// <summary>
	///     Implements a random access list.
	/// </summary>
	/// <typeparam name="T">The type of value stored in the list.</typeparam>
	[Obsolete("Use ImmList for now.")]
	public sealed partial class ImmVector<T> : AbstractSequential<T, ImmVector<T>> {

		
		/// <summary>
		///     The empty
		/// </summary>
		static readonly ImmVector<T> empty = new ImmVector<T>(TrieVector<T>.Node.Empty);

		/// <summary>
		///     The data structure is limited by a 30-bit address space. This number returns its exact maximum capacity.
		///     When an instance exceeds this capacity an <c>InvalidOperationException</c> will be thrown.
		/// </summary>
		public const int MaxCapacity = Int32.MaxValue >> 1;

		/// <summary>
		///     The root
		/// </summary>
		internal readonly TrieVector<T>.Node Root;

		/// <summary>
		///     Initializes a new instance of the <see cref="ImmVector{T}" /> class.
		/// </summary>
		/// <param name="root">The root.</param>
		internal ImmVector(TrieVector<T>.Node root) {
			Root = root;
		}

		/// <summary>
		///     Returns the empty array.
		/// </summary>
		public new static ImmVector<T> Empty {
			get { return empty; }
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public override int Length {
			get { return Root.Length; }
		}

		/// <summary>
		///     Returns the first element in the collection.
		///     <para>⚡⚡⚡⚡⚡⚡ O(logn)</para>
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public override T First {
			get {
				if (Root.Length == 0) throw Errors.Is_empty;
				return Root[0];
			}
		}

		/// <summary>
		///     Returns true if the collection is empty.
		/// </summary>
		public override bool IsEmpty => Length == 0;

		/// <summary>
		///     Gets the last element in the collection.
		/// </summary>
		/// <value> The last. </value>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public override T Last {
			get {
				if (Root.Length == 0) throw Errors.Is_empty;
				return Root[Root.Length - 1];
			}
		}

		internal int RecursiveLength() {
			return Root.RecursiveTotalLength();
		}

		/// <summary>
		///     Returns a new collection without the specified initial number of elements. Returns empty if
		///     <paramref name="count" /> is equal or greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
		/// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public override ImmVector<T> Skip(int count) {
			count.CheckIsBetween("count", lower:0);
			if (count >= Length) return Empty;
			if (count == 0) return this;
			var arr = new T[Length - count];
			CopyTo(arr, count, 0, Length - count);
			return arr.ToImmVector();
		}

		/// <summary>
		///     Returns a range of elements. Doesn't support negative indexing.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected override ImmVector<T> GetRange(int @from, int count) {
#if ASSERTS
			@from.AssertBetween(0, Root.Length - 1);
			(from + count).AssertBetween(0, Root.Length);
#endif
			var arr = new T[count];
			CopyTo(arr, from, 0, count);
			return arr.ToImmVector();
		}

		/// <summary>
		/// Inserts a sequence of elements at the specified index.
		/// </summary>
		/// <param name="index">The index at which to insert. The element at this index is pushed forward.</param>
		/// <param name="items">The items to insert. Faster if an array or a known collection type.</param>
		/// <returns></returns>
		public ImmVector<T> InsertRange(int index, IEnumerable<T> items) {
			index.CheckIsBetween("index", -Root.Length-1, Root.Length);
			items.CheckNotNull("items");
			index = index < 0 ? index + Root.Length + 1: index;
			if (index == 0) return AddFirstRange(items);
			if (index == Length) return AddLastRange(items);
#if ASSERTS
			var oldLast = Last;
			var oldFirst = First;
#endif
			var start = Root.Take(index - 1, Lineage.Immutable);
			var lineage = Lineage.Mutable();
			var len = 0;
			var s = 0;
			var oldLength = Length;
			var arrInsert = items.ToArrayFast(out len);
			if (len == 0) {
				return this;
			}
			var arrAfter = new T[oldLength - index];
			CopyTo(arrAfter, index, 0, oldLength - index);
			var arrLength = len;
			if (len == 0) return this;
			if (oldLength + len >= MaxCapacity) throw Errors.Capacity_exceeded();
			start = start.AddRange(arrInsert, lineage, 6, ref s, ref len);
			len = oldLength - index;
			s = 0;
			start = start.AddRange(arrAfter, lineage, 6, ref s, ref len);
			ImmVector<T> ret = start;
#if ASSERTS
			ret.Length.AssertEqual(oldLength + arrLength);
			ret.RecursiveLength().AssertEqual(ret.Length);
			ret.Last.AssertEqual(oldLast);
			ret.First.AssertEqual(oldFirst);
			ret[index].AssertEqual(arrInsert[0]);
			ret[index + arrLength - 1].AssertEqual(arrInsert[arrLength - 1]);
#endif
			return ret;
		}

		/// <summary>
		///     Copies a range of elements from the collection to the specified array.
		/// </summary>
		/// <param name="arr"> The array. </param>
		/// <param name="myStart"> The index of the collection at which to start copying. May be negative.</param>
		/// <param name="arrStart">The index of the array at which to start copying. May be negative.</param>
		/// <param name="count"> The number of items to copy. Must be non-negative.</param>
		/// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     Thrown if the array isn't long enough, or one of the parameters is
		///     invalid.
		/// </exception>
		public override void CopyTo(T[] arr, int myStart, int arrStart, int count) {
			arr.CheckNotNull("arr");
			if (myStart != 0 || count != 0) {
				myStart.CheckIsBetween("myStart", -Root.Length, Root.Length - 1);
			}
			if (arrStart != 0 || count != 0) {
				arrStart.CheckIsBetween("arrStart", -arr.Length, arr.Length - 1);
			}
			
			myStart = myStart < 0 ? myStart + Length : myStart;
			arrStart = arrStart < 0 ? arr.Length + arrStart : arrStart;

			(myStart + count).CheckIsBetween("count", 0, Root.Length);
			(arrStart + count).CheckIsBetween("count", 0, arr.Length);
			var ix = arrStart;
			
			Root.IterWhileFrom(myStart, item => {
				if (ix >= arrStart + count) return false;
				arr[ix] = item;
				ix++;
				return true;
			});
		}

		/// <summary>
		///     Adds the specified element to the end of the vector. O(logn), fast.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>ImmVector{`0}.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure exceeds its maximum capacity.</exception>
		public ImmVector<T> AddLast(T item) {
#if ASSERTS
			var expected = Length + 1;
#endif
			if (Root.Length >= MaxCapacity) throw Errors.Capacity_exceeded();

			ImmVector<T> ret = Root.Add(item, Lineage.Immutable);
#if ASSERTS
			ret.Last.AssertEqual(item);
			ret.Length.AssertEqual(expected);
#endif
			return ret;
		}

		/// <summary>
		///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public override bool ForEachWhile(Func<T, bool> function) {
			return Root.IterWhile(function);
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from last to first, until it returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function null.</exception>
		public override bool ForEachBackWhile(Func<T, bool> function) {
			return Root.IterBackWhile(function);
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from last to first.
		/// </summary>
		/// <param name="action"> The action. </param>
		/// <exception cref="ArgumentNullException">Thrown if the delegate is null.</exception>
		public override void ForEachBack(Action<T> action) {
			Root.IterBack(action);
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from first to last.
		/// </summary>
		/// <param name="action"> The action. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public override void ForEach(Action<T> action) {
			Root.Iter(action);
		}
		
		/// <summary>
		///     Provided for testing purposes only. Is too inefficient to be used in practice.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private ImmVector<T> RemoveAt(int index) {
			index = index < 0 ? index + Length : index;
			if (index == 0) return Skip(1);
			if (index == Length - 1) return RemoveLast();
			index.CheckIsBetween("index", 0, Length-1);
			var lineage = Lineage.Mutable();
			var take = Root.Take(index - 1, lineage);
			var arr = new T[Length - index - 1];
			CopyTo(arr, index + 1, 0, Length - index - 1);
			var len = arr.Length;
			var s = 0;
			var ret = take.AddRange(arr, lineage, 6, ref s, ref len);
			return ret;
		}

		/// <summary>
		///     Provided for testing purposes only. Is too inefficient to be used in practice.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ImmVector<T> Insert(int index, T value) {
			index = index < 0 ? index + Length + 1 : index;
			if (index == Length) return AddLast(value);
			if (index == 0) return AddFirstRange(new[] { value });
			var lineage = Lineage.Mutable();
			var take = Root.Take(index - 1, lineage);
			take = take.Add(value, lineage);
			var arr = new T[Length - index];
			CopyTo(arr, index, 0, Length - index);
			var len = arr.Length;
			var s = 0;
			var ret = take.AddRange(arr, lineage, 6, ref s, ref len);
#if ASSERTS
			ret.Length.AssertEqual(Length + 1);
#endif
			return ret;
		}

		/// <summary>
		///     Adds a sequence of items to the end of the collection.
		/// </summary>
		/// <param name="items">A sequence of items to add. Faster if the sequence is an array or a known collection type.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		/// <remarks>This member performs a lot better when the specified sequence is an array.</remarks>
		public ImmVector<T> AddLastRange(IEnumerable<T> items) {
			items.CheckNotNull("items");
#if ASSERTS
			var expected = Length;
			var oldLast = TryLast;
#endif
			var len = 0;
			var s = 0;
			if (IsEmpty) {
				var asVector = items as ImmVector<T>;
				if (asVector != null) {
					return asVector;
				}
			}
			var arr = items.ToArrayFast(out len);
			var oldLen = len;
			if (Root.Length + len >= MaxCapacity) throw Errors.Capacity_exceeded();
			ImmVector<T> ret = Root.AddRange(arr, Lineage.Mutable(), 6, ref s, ref len);
#if ASSERTS
			ret.Length.AssertEqual(expected + oldLen);
			if (arr.Length > 0 && oldLen > 0) ret.Last.AssertEqual(arr[oldLen - 1]);
			else if (oldLast.IsSome) ret.Last.AssertEqual(oldLast.Value);
#endif
			return ret;
		}

		/// <summary>
		///     Adds a sequence of elements to the beginning of the collection.
		/// </summary>
		/// <param name="items">The sequence. Faster if the sequence is also a vector, an array, or a known collection type.</param>
		/// <exception cref="ArgumentNullException">Thrown if the sequence is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		/// <returns></returns>
		public ImmVector<T> AddFirstRange(IEnumerable<T> items) {
			items.CheckNotNull("items");

			var ret = empty.AddLastRange(items).AddLastRange(this);
			return ret;
		}

		/// <summary>
		///     Copies data from an array..
		/// </summary>
		/// <param name="arr">The arr.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="count">The count.</param>
		/// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		private ImmVector<T> CopyFrom(T[] arr, int startIndex, int count) {
			arr.CheckNotNull("arr");
			startIndex.CheckIsBetween("startIndex", -arr.Length, arr.Length - 1);
			startIndex = startIndex < 0 ? arr.Length + startIndex : startIndex;
			count.CheckIsBetween("count", 0, arr.Length - startIndex);
			
			if (Root.Length + arr.Length >= MaxCapacity) throw Errors.Capacity_exceeded();
			var lineage = Lineage.Mutable();

			var ret = Root.AddRange(arr, lineage, 6, ref startIndex, ref count);
			return ret;
		}

		/// <summary>
		///     Removes the last item from the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public ImmVector<T> RemoveLast() {
			if (Root.Length == 0) throw Errors.Is_empty;
#if ASSERTS
			var expected = Length - 1;
#endif
			var lineage = Lineage.Immutable;
			ImmVector<T> ret = Root.RemoveLast(lineage);
#if ASSERTS
			ret.Length.AssertEqual(expected);
			if (Length > 1) ret.Last.AssertEqual(this[-2]);
#endif
			return ret;
		}

		/// <summary>
		///     Sets the value of the item with the specified index.
		/// </summary>
		/// <param name="index">The index of the item to update.</param>
		/// <param name="item">The new value of the item</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the data structure.</exception>
		public ImmVector<T> Update(int index, T item) {
			index.CheckIsBetween("index", -Root.Length, Root.Length - 1);
#if ASSERTS
			var expectedLength = Length;
#endif  
			index = index < 0 ? index + Length : index;
			var lineage = Lineage.Immutable;
			var ret = Root.Update(index, item, lineage);
#if ASSERTS
			ret[index].AssertEqual(item);
			ret.Length.AssertEqual(expectedLength);
#endif
			return ret;
		}

		protected override ImmVector<T> UnderlyingCollection
		{
			get { return this; }
		}

		/// <summary>
		///     Returns a subsequence consisting of the specified number of elements. Returns empty if <paramref name="count" /> is
		///     greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements.. </param>
		/// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public override ImmVector<T> Take(int count) {
			count.CheckIsBetween("count", lower:0);
			if (count == 0) return Empty;
			if (count >= Length) return this;
			var ret = Root.Take(count - 1, Lineage.Immutable);
			return ret;
		}
	}
}