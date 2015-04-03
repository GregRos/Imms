using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Funq.Collections.Common;
using System.Linq;
using Funq.Collections.Implementation;


namespace Funq.Collections
{
	/// <summary>
	/// Implements a random access list.
	/// </summary>
	/// <typeparam name="T">The type of value stored in the list.</typeparam>
	public sealed partial class FunqVector<T>
	{
		/// <summary>
		/// The empty
		/// </summary>
		internal static readonly FunqVector<T> empty = new FunqVector<T>(TrieVector<T>.Node.Empty);

		/// <summary>
		/// The data structure is limited by a 30-bit address space. This number returns its exact maximum capacity.
		/// When an instance exceeds this capacity an <c>InvalidOperationException</c> will be thrown.
		/// </summary>
		public static readonly int MaxCapacity = Int32.MaxValue >> 1;

		/// <summary>
		/// The root
		/// </summary>
		internal readonly TrieVector<T>.Node root;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunqVector{T}"/> class.
		/// </summary>
		/// <param name="root">The root.</param>
		internal FunqVector(TrieVector<T>.Node root)
		{
			this.root = root;
		}

		/// <summary>
		/// Returns the empty array.
		/// </summary>
		public static FunqVector<T> Empty
		{
			get
			{
				return empty;
			}
		}


		public override int Length
		{
			get
			{
				return root.Length;
			}
		}

		public override T First
		{
			get
			{
				if (Length == 0) throw Funq.Errors.Is_empty;
				return this[0];
			}
		}

		internal int RecursiveLength() {
			return root.RecursiveTotalLength();
		}

		public override FunqVector<T> Skip(int count) {
			var arr = new T[Length - count];
			CopyTo(arr, count, 0, Length - count);
			return arr.ToFunqVector();
		}

		protected override FunqVector<T> GetRange(int @from, int count) {
			var lineage = Lineage.Mutable();
			var arr = new T[count];
			CopyTo(arr, from, 0, count);
			return arr.ToFunqVector();
		}


		public FunqVector<T> InsertRange(int index, IEnumerable<T> items) {
			if (index == 0) return AddFirstRange(items);
			if (index == Length) return AddLastRange(items);
#if ASSERTS
			var oldLast = Last;
			var oldFirst = First;
#endif
			var start = root.Take(index - 1, Lineage.Immutable);
			var lineage = Lineage.Mutable();
			var len = 0;
			var s = 0;
			var oldLength = Length;
			var arrInsert = items.ToArrayFast(out len);
			var arrAfter =  new T[oldLength - index];
			CopyTo(arrAfter, index, 0, oldLength - index);
			var arrLength = len;
			if (len == 0) return this;
			if (oldLength + len >= MaxCapacity) throw Funq.Errors.Capacity_exceeded();
			start = start.AddMany(arrInsert, lineage, 6, ref s, ref len);
			len = oldLength - index;
			s = 0;
			start = start.AddMany(arrAfter, lineage, 6, ref s, ref len);
			FunqVector<T> ret = start;
#if ASSERTS
			ret.Length.Is(oldLength + arrLength);
			ret.RecursiveLength().Is(ret.Length);
			ret.Last.Is(oldLast);
			ret.First.Is(oldFirst);
			ret[index].Is(arrInsert[0]);
			ret[index + arrLength - 1].Is(arrInsert[arrLength - 1]);
#endif
			return ret;
		}

		public override void CopyTo(T[] arr, int myStart, int arrStart, int count) {
			int ix = arrStart;
			root.IterWhileFrom(myStart, item => {
				if (ix >= arrStart + count) 
						return false;
				arr[ix] = item;
				ix++;
				return true;
			});
		}

		public override bool IsEmpty
		{
			get
			{
				return Length == 0;
			}
		}

		public override T Last
		{
			get
			{
				if (Length == 0) throw Funq.Errors.Is_empty;
				return this[-1];
			}
		}

		/// <summary>
		/// Adds the specified element to the end of the vector. O(logn), fast.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>FunqVector{`0}.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure exceeds its maximum capacity.</exception>
		public FunqVector<T> AddLast(T item)
		{
#if ASSERTS
			var expected = Length + 1;
#endif
			if (root.Length >= MaxCapacity) throw Funq.Errors.Capacity_exceeded();

			FunqVector<T> ret = root.Add(item, Lineage.Immutable);
#if ASSERTS
			ret.Last.Is(item);
			ret.Length.Is(expected);
#endif
			return ret;
		}

		


		public override bool ForEachWhile(Func<T, bool> iterator)
		{
			return root.IterWhile(iterator);
		}

		public override bool ForEachBackWhile(Func<T, bool> iterator)
		{
			return root.IterBackWhile(iterator);
		}

		public override void ForEachBack(Action<T> action)
		{
			root.IterBack(action);
		}

		public override void ForEach(Action<T> action)
		{
			root.Iter(action);
		}
		/// <summary>
		/// Very, very inefficient. Shouldn't be used.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		internal FunqVector<T> Remove(int index)
		{
			if (index == 0) return Skip(1);
			if (index == Length - 1) return RemoveLast();
			var lineage = Lineage.Mutable();
			var take = root.Take(index - 1, lineage);
			var arr = new T[Length - index - 1];
			CopyTo(arr, index + 1, 0, Length - index - 1);
			var len = arr.Length;
			var s = 0;
			var ret = take.AddMany(arr, lineage, 6, ref s, ref len);
			return ret;
		}

		/// <summary>
		/// Very inefficient.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		internal FunqVector<T> Insert(int index, T value) {
			if (index == Length) return AddLast(value);
			if (index == 0) return AddFirstRange(new[] {value});
			var lineage = Lineage.Mutable();
			var take = root.Take(index - 1, lineage);
			take = take.Add(value, lineage);
			var arr = new T[Length - index];
			CopyTo(arr, index, 0, Length - index);
			var len = arr.Length;
			var s = 0;
			var ret = take.AddMany(arr, lineage, 6, ref s, ref len);
#if ASSERTS
			ret.Length.Is(Length + 1);
#endif
			return ret;
		}

		/// <summary>
		/// Adds a sequence of items to the end of the collection.
		/// </summary>
		/// <param name="items">A sequence of items to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		/// <remarks>This member performs a lot better when the specified sequence is an array.</remarks>
		public FunqVector<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
#if ASSERTS
			var expected = Length;
			var old_last = TryLast;
#endif
			var len = 0;
			var s = 0;
			var arr = items.ToArrayFast(out len);
			var old_len = len;
			if (root.Length + len >= MaxCapacity) throw Funq.Errors.Capacity_exceeded();
			FunqVector<T> ret = root.AddMany(arr, Lineage.Mutable(), 6, ref s, ref len);
#if ASSERTS
			ret.Length.Is(expected + old_len);
			if (arr.Length > 0 && old_len > 0) ret.Last.Is(arr[old_len - 1]);
			else if (old_last.IsSome) ret.Last.Is(old_last.Value);
#endif
			return ret;
		}
		/// <summary>
		/// Adds a sequence of elements to the beginning of the collection. Isn't optimized.
		/// </summary>
		/// <param name="items">The sequence.</param>
		/// <exception cref="ArgumentNullException">Thrown if the sequence is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		/// <returns></returns>
		public FunqVector<T> AddFirstRange(IEnumerable<T> items)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");

			var asFunqArray = items as FunqVector<T>;
			if (asFunqArray != null)
			{
				return asFunqArray.AddLastRange(this);
			}

			FunqVector<T> ret = empty.AddLastRange(items).AddLastRange(this);
			return ret;
		}

		/// <summary>
		/// Copies data from an array..
		/// </summary>
		/// <param name="arr">The arr.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="count">The count.</param>
		/// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		public FunqVector<T> CopyFrom(T[] arr, int startIndex, int count)
		{
			if (arr == null) throw Funq.Errors.Argument_null("arr");
			if (root.Length + arr.Length >= MaxCapacity) throw Funq.Errors.Capacity_exceeded();
			var lineage = Lineage.Mutable();

			var ret = root.AddMany(arr, lineage, 6, ref startIndex, ref count);
			return ret;
		}

		/// <summary>
		/// Removes the last item from the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public FunqVector<T> RemoveLast()
		{
			if (root.Length == 0) throw Funq.Errors.Is_empty;
#if ASSERTS
			var expected = Length - 1;
#endif
			var lineage = Lineage.Immutable;
			FunqVector<T> ret =  root.RemoveLast(lineage);
#if ASSERTS
			ret.Length.Is(expected);
			if (Length > 1) ret.Last.Is(this[-2]);
#endif
			return ret;
		}

		/// <summary>
		/// Removes several items from the end of the list. O(logn)
		/// </summary>
		/// <param name="count">The number of items to remove.</param>
		/// <returns>FunqVector{`0}.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if there aren't enough elements.</exception>
		public FunqVector<T> RemoveLast(int count)
		{
			if (root.Length < count) throw Errors.Arg_out_of_range("count");
			return this.Take(Length - count);
		}

		/// <summary>
		/// Sets the value of the item with the specified index.
		/// </summary>
		/// <param name="index">The index of the item to update.</param>
		/// <param name="item">The new value of the item</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the data structure.</exception>
		public FunqVector<T> Update(int index, T item)
		{
#if ASSERTS
			var expected_length = Length;
#endif
			index = index < 0 ? index + Length : index;
			if (index < 0 || index >= Length) throw Funq.Errors.Arg_out_of_range("index", index);
			var lineage = Lineage.Immutable;
			var ret = root.Update(index, item, lineage);
#if ASSERTS
			ret[index].Is(item);
			ret.Length.Is(expected_length);
#endif
			return ret;
		}

		public override FunqVector<T> Take(int count)
		{
			if (count < 0 || count > Length) throw Funq.Errors.Arg_out_of_range("count", count);
			if (count == 0) return empty;
			var ret = root.Take(count - 1, Lineage.Immutable);
			return ret;
		}
	}
}