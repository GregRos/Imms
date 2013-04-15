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
	/// <typeparam name="T">The type of value storedi n the list. </typeparam>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (Vector<>.VectorDebugView))]
	public class Vector<T> : IList<T>
	{
		private class VectorDebugView
		{
			private readonly Vector<T> _inner;

			public VectorDebugView(Vector<T> inner)
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

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] zContents
			{
				get
				{
					return _inner.ToArray();
				}
			}
		}

		internal static readonly Vector<T> empty = new Vector<T>(TrieVector<T>.VectorNode.Empty);

		/// <summary>
		///   The data structure is limited by a 30-bit address space. This number returns its exact maximum capacity. 
		///   When an instance exceeds this capacity an <c>InvalidOperationException</c> will be thrown.
		/// </summary>
		public static readonly int MaxCapacity = Int32.MaxValue >> 1;

		private readonly TrieVector<T>.VectorNode root;

		internal Vector(TrieVector<T>.VectorNode root)
		{
			this.root = root;
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

		int IList<T>.IndexOf(T item)
		{
			var found = this.IndexOf(v => item.Equals(v));
			if (found.HasValue)
			{
				return found.Value;
			}
			throw Errors.Arg_out_of_range("item");
		}

		void IList<T>.Insert(int index, T item)
		{
			throw Errors.Collection_readonly;
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
		///   Gets the value of the item with the specified index. O(logn); immediate.
		/// </summary>
		/// <param name="index">The index of the item to get.</param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		public T this[int index]
		{
			get
			{
				index = index < 0 ? index + Count : index;
				if (index >= Count || index < 0) throw Errors.Arg_out_of_range("index");
				return root[index];
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
			return this.IndexOf(v => item.Equals(v)).HasValue;
		}
		/// <summary>
		/// Copies the entire collection to the specified array, starting at some index.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="arrayIndex">.</param>
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
		///   Gets the length of the vector.
		/// </summary>
		public int Count
		{
			get
			{
				return root.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("Vector, Count = {0}", Count);
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
		/// <param name="item">The item to add.</param>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure exceeds its maximum capacity.</exception>
		public Vector<T> AddLast(T item)
		{
			if (root.Count >= MaxCapacity) throw Errors.Capacity_exceeded;
			return new Vector<T>(root.Add(item));
		}

		/// <summary>
		/// Converts a Vector to an array.
		/// </summary>
		/// <returns></returns>
		public T[] ToArray()
		{
			var list = new T[this.Count];
			var index = 0;
			this.ForEach(v =>
			{
				list[index] = v;
				index++;
			});
			return list;
		}

		/// <summary>
		///   <para> Highly optimized. Adds a sequence of items to the end of the collection. O(n). </para>
		/// </summary>
		/// <param name="items">A sequence of items to add.</param>
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
			if (root.Count >= MaxCapacity) throw Errors.Capacity_exceeded;
			int len = 0;
			T[] arr = items.ToArrayFast(out len);
			return new Vector<T>(root.BulkLoad(arr, 0, len));
		}

		/// <summary>
		///   Applies an accumulator over the collection.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value, or the default value for the type. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult Aggregate<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Applies the acummulator function over every element in the collection, from last to first.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		public TResult AggregateBack<TResult>(Func<TResult, T, TResult> accumulator, TResult initial = default(TResult))
		{
			ForEachBack(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Removes the last item from the collection. O(logn), fast.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public Vector<T> DropLast()
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Drop());
		}

		/// <summary>
		///   Removes several items from the end of the list. O(logn), fast.
		/// </summary>
		/// <param name="count"> The number of items to remove. </param>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the data structure is empty.</exception>
		public Vector<T> DropLast(int count)
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Take(root.Count - count));
		}

		/// <summary>
		///   Iterates over every item in the vector, from first to last.
		/// </summary>
		/// <param name="action"> The function to apply on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEach(Action<T> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			root.Iter(action);
		}

		/// <summary>
		///   Iterates over every element in the vector, from last to first.
		/// </summary>
		/// <param name="conditional"> The function applied on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachBack(Action<T> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			root.IterBack(conditional);
		}

		/// <summary>
		///   Iterates over every element in the vector, from last to first. Stops if the function returns false.
		/// </summary>
		/// <param name="conditional"> The function applied on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			root.IterBackWhile(conditional);
		}

		/// <summary>
		///   Iterates over every element in the collection, from first to last. Stops if the function returns false.
		/// </summary>
		/// <param name="conditional">The function used for iterating over the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			root.IterWhile(conditional);
		}

		/// <summary>
		///   Returns the enumerator that allows iterating over the collection.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Returns the index of the first item that fulfills the predicate, or null.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public int? IndexOf(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = 0;
			var result = root.IterWhile(v =>
			                            {
				                            
											if (predicate(v)) return false;
				                            index++;
				                            return true;
			                            });
			return !result ? (int?) index : null;
		}

		/// <summary>
		///   Projects each element using the specified selector. O(n), fast.
		/// </summary>
		/// <typeparam name="TOut"> The projected type of each element. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		/// <exception cref="OutOfMemoryException">Thrown if the system runs out of memory.</exception>
		public Vector<TOut> Select<TOut>(Func<T, TOut> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			return new Vector<TOut>(root.Apply(selector));
		}

		/// <summary>
		///   Sets the value of the item with the specified index. O(logn), fast.
		/// </summary>
		/// <param name="index">The index of the item to update.</param>
		/// <param name="item">The new value of the item</param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index doesn't exist in the data structure.</exception>
		public Vector<T> Set(int index, T item)
		{
			index = index < 0 ? index + Count : index;
			if (index < 0 || index >= Count) throw Errors.Arg_out_of_range("index");
			return new Vector<T>(root.Set(index, item));
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
			return new Vector<T>(root.Take(count));
		}

		/// <summary>
		///   Returns a vector consisting of the first items that fulfill the predicate. O(m), fast.
		/// </summary>
		/// <param name="predicate">The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public Vector<T> TakeWhile(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = IndexOf(predicate);
			if (!index.HasValue)
			{
				return this;
			}
			return Take((int) index + 1);
		}

		/// <summary>
		///   Filters the collection using the specified predicate. O(n), fast.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		/// <exception cref="OutOfMemoryException">Thrown if the system runs out of memory.</exception>
		public Vector<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = 0;
			var array = new T[Count];
			var newVector = empty;
			ForEach(v =>
			        {
				        if (predicate(v))
				        {
					        array[index] = v;
					        index++;
				        }
			        });
			return new Vector<T>(TrieVector<T>.VectorNode.Empty.BulkLoad(array, 0, index));
		}
	}
}