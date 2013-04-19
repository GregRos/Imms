using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Solid.Common;
using Solid.TrieVector;
using System.Linq;

namespace Solid
{
	/// <summary>
	///   Implements a random access list.
	/// </summary>
	/// <typeparam name="T"> The type of value stored in the list. </typeparam>
	
	public partial class Vector<T>
	{
		internal static readonly Vector<T> empty = new Vector<T>(TrieVector<T>.VectorNode.Empty);

		/// <summary>
		///   The data structure is limited by a 30-bit address space. This number returns its exact maximum capacity. 
		///   When an instance exceeds this capacity an <c>InvalidOperationException</c> will be thrown.
		/// </summary>
		public static readonly int MaxCapacity = Int32.MaxValue >> 1;

		private readonly TrieVector<T>.VectorNode _root;

		internal Vector(TrieVector<T>.VectorNode root)
		{
			this._root = root;
		}

		/// <summary>
		///   Gets the empty vector.
		/// </summary>
		public static Vector<T> Empty
		{
			get
			{
				return empty;
			}
		}

		/// <summary>
		///   Gets the value of the item with the specified index. O(logn); immediate.
		/// </summary>
		/// <param name="index"> The index of the item to get. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		public T this[int index]
		{
			get
			{
				index = index < 0 ? index + Count : index;
				if (index >= Count || index < 0) throw Errors.Arg_out_of_range("index");
				return _root[index];
			}
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public int Count
		{
			get
			{
				return _root.Count;
			}
		}

		/// <summary>
		///   Gets the first item in the vector. O(logn), immediate.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public T First
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[0];
			}
		}

		/// <summary>
		///   O(1). Gets if the vector is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		/// <summary>
		///   Gets the last item in the vector. O(logn), immediate.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public T Last
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[-1];
			}
		}

		/// <summary>
		///   Adds the specified element to the end of the vector. O(logn), fast.
		/// </summary>
		/// <param name="item"> The item to add. </param>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure exceeds its maximum capacity.</exception>
		public Vector<T> AddLast(T item)
		{
			if (_root.Count >= MaxCapacity) throw Errors.Capacity_exceeded;
			return new Vector<T>(_root.Add(item));
		}

		/// <summary>
		/// Copies data from an array..
		/// </summary>
		/// <param name="arr">The arr.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="count">The count.</param>
		/// <returns></returns>
		public Vector<T> CopyFrom(T[] arr, int startIndex, int count)
		{
			if (_root.Count + arr.Length >= MaxCapacity) throw Errors.Capacity_exceeded;
			return new Vector<T>(_root.BulkLoad(arr, startIndex, count));
		}

		/// <summary>
		/// Projects each item into a sequence, and flattens the sequences.
		/// </summary>
		/// <typeparam name="TOut">The type of the output.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public Vector<TOut> SelectMany<TOut>(Func<T, IEnumerable<TOut>> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			var result = Vector<TOut>.Empty;
			ForEach(v => result = result.AddLastRange(selector(v)));
			return result;
		}

		/// <summary>
		///   <para> Highly optimized. Adds a sequence of items to the end of the collection. O(n). </para>
		/// </summary>
		/// <param name="items"> A sequence of items to add. </param>
		/// <returns> </returns>
		/// <remarks>
		///   This member performs a lot better when the specified sequence is an array.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <exception cref="OutOfMemoryException">Thrown if the system runs out of memory.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the collection exceeds its capacity.</exception>
		public Vector<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");

			var len = 0;
			var arr = items.ToArrayFast(out len);
			if (_root.Count + len >= MaxCapacity) throw Errors.Capacity_exceeded;
			return new Vector<T>(_root.BulkLoad(arr, 0, len));
		}

		/// <summary>
		///   Copies the entire collection to the specified array, starting at some index.
		/// </summary>
		/// <param name="array"> The array. </param>
		/// <param name="arrayIndex"> . </param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			ForEach(v =>
			        {
				        array[arrayIndex] = v;
				        arrayIndex++;
			        });
		}

		/// <summary>
		///   Removes the last item from the collection. O(logn), fast.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public Vector<T> DropLast()
		{
			if (_root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(_root.Drop());
		}

		/// <summary>
		///   Removes several items from the end of the list. O(logn), fast.
		/// </summary>
		/// <param name="count"> The number of items to remove. </param>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public Vector<T> DropLast(int count)
		{
			if (_root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(_root.Take(_root.Count - count));
		}

		/// <summary>
		///   Sets the value of the item with the specified index. O(logn), fast.
		/// </summary>
		/// <param name="index"> The index of the item to update. </param>
		/// <param name="item"> The new value of the item </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the data structure.</exception>
		public Vector<T> Set(int index, T item)
		{
			var x = new[] {0};

			index = index < 0 ? index + Count : index;
			if (index < 0 || index >= Count) throw Errors.Arg_out_of_range("index");
			return new Vector<T>(_root.Set(index, item));
		}

		/// <summary>
		///   Returns a collection consisting of the first several items. O(logn), fast.
		/// </summary>
		/// <param name="count"> The number of items. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the count is invalid.</exception>
		public Vector<T> Take(int count)
		{
			if (count < 0 || count > Count) throw Errors.Arg_out_of_range("count");
			if (count == 0) return empty;
			return new Vector<T>(_root.Take(count));
		}

		/// <summary>
		/// Returns an enumerable for iterating over the vector, from last to first.
		/// </summary>
		public IEnumerable<T> Backward
		{
			get
			{
				return new EnumerableProxy<T>(() => _root.GetEnumerator(false));
			}
		}

		/// <summary>
		///   Converts a Vector to an array.
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