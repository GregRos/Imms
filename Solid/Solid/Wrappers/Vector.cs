using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Solid.Common;
using Solid.TrieVector;
using System.Linq;
namespace Solid
{
	public static class Vector
	{
		/// <summary>
		/// Gets the empty Vector.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <remarks>
		/// All empty vectors of the same generic type are reference equal.
		/// Empty vectors of different generic types may or may not be reference equal.
		/// </remarks>
		public static Vector<T> Empty<T>()
		{
			return Vector<T>.Empty;
		}

		/// <summary>
		/// Creates a vector from one or more items.
		/// </summary>
		/// <typeparam name="T">The type of value the vector holds.</typeparam>
		/// <param name="items">One or more items to be part of the vector.</param>
		/// <returns></returns>
		public static Vector<T> FromItems<T>(params T[] items)
		{
			return items.ToVector();
		}
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (Vector<>.VectorDebugView))]
	public sealed class Vector<T> : IEnumerable<T>
	{
		internal static readonly Vector<T> empty = new Vector<T>(VectorNode<T>.Empty);
		private readonly VectorNode<T> root;

		internal Vector(VectorNode<T> root)
		{
			this.root = root;
		}

		/// <summary>
		/// Gets the value of the item with the specified index. O(logn); immediate.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				index = index < 0 ? index + Count : index;
				if (index >= Count || index < 0) throw Errors.Index_out_of_range;
				return root[index];
			}
		}

		/// <summary>
		/// Gets the length of the vector.
		/// </summary>
		public int Count
		{
			get
			{
				return root.Count;
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
		/// Adds the specified element to the end of the vector. O(logn), fast.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Vector<T> AddLast(T item)
		{
			return new Vector<T>(root.Add(item));
		}
		/// <summary>
		/// O(1). Gets if the vector is empty.	
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		/// <summary>
		/// Adds the specified items to the end of the vector. O(m·logn), slow.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public Vector<T> AddLastItems(params T[] items)
		{
			//TODO: Efficient bulk loading.
			if (items == null) throw Errors.Argument_null("items");
			return this.AddLastRange(items.AsEnumerable());
		}
		/// <summary>
		/// Adds a sequence of items to the end of the vector. O(m·logn), slow.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public Vector<T> AddLastRange(IEnumerable<T> items)
		{
			if (items == null) throw Errors.Argument_null("items");
			//TODO: Naive implementation. Implement with efficient bulk loading.
			Vector<T> vec = this;
			foreach (T item in items)
			{
				vec = vec.AddLast(item);
			}
			return vec;
		}
		/// <summary>
		/// Applies a selector on every element. O(n)
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="transform"></param>
		/// <returns></returns>
		public Vector<TOut> Select<TOut>(Func<T, TOut> transform)
		{
			if (transform == null) throw Errors.Argument_null("transform");
			return new Vector<TOut>(root.Apply(transform));
		}
		/// <summary>
		/// Gets the first item in the vector. O(logn), immediate.
		/// </summary>
		public T First
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[0];
			}
		}
		/// <summary>
		/// Gets the last item in the vector. O(logn), immediate.
		/// </summary>
		public T Last
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[-1];
			}
		}


		/// <summary>
		/// Removes the last item from the list. O(logn), fast.
		/// </summary>
		/// <returns></returns>
		public Vector<T> DropLast()
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Drop());
		}
		/// <summary>
		/// Returns a vector consisting of the items that fulfill the specified predicate. O(n), slow.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public Vector<T> Where(Func<T, bool> predicate)
		{
			var newVector = empty;
			this.ForEach(v =>
			             {
				             if (predicate(v))
				             {
					             newVector = newVector.AddLast(v);
				             }


			             });
			return newVector;
		}
		/// <summary>
		/// Applies an accumulator over the vector.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="accumulator"></param>
		/// <param name="initial"></param>
		/// <returns></returns>
		public TResult Aggregate<TResult>(Func<TResult, T, TResult> accumulator, TResult initial)
		{
			this.ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		
		/// <summary>
		/// Iterates over every element in the vector, from first to last. Stops if the conditional returns false.
		/// </summary>
		/// <param name="conditional"></param>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			root.IterWhile(conditional);
		}
		/// <summary>
		///Iterates over every element in the vector, from last to first.
		/// </summary>
		/// <param name="action"></param>
		public void ForEachBack(Action<T> action)
		{
			root.IterBack(action);
		}
		/// <summary>
		/// Iterates over every element in the vector, from last to first. Stops if the conditional returns false.
		/// </summary>
		/// <param name="conditional"></param>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			root.IterBackWhile(conditional);
		}

		/// <summary>
		/// Returns the index of the first item that fulfills the predicate.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public int? IndexOf(Func<T, bool> predicate)
		{
			var index = 0;
			var result = root.IterWhile(v =>
			               {
							   if (predicate(v)) return false;
				               index++;
							   return true;
			               });
			return !result ? (int?)index : null;
		}
		/// <summary>
		/// Removes several items from the end of the list. O(logn), fast.
		/// </summary>
		/// <param name="count">The number of items to remove.</param>
		/// <returns></returns>
		public Vector<T> DropLast(int count)
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Take(root.Count - count));
		}
		/// <summary>
		/// Iterates over every item in the vector, from first to last.
		/// </summary>
		/// <param name="action"></param>
		public void ForEach(Action<T> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			root.Iter(action);
		}

		/// <summary>
		/// Sets the value of the item with the specified index. O(logn), fast.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public Vector<T> Set(int index, T item)
		{
			index = index < 0 ? index + Count : index;
			if (index < 0 || index >= Count) throw Errors.Index_out_of_range;
			return new Vector<T>(root.Set(index, item));
		}

		/// <summary>
		/// Returns a vector consisting of the first several items. O(logn), fast.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Vector<T> Take(int count)
		{
			if (count < 0 || count > Count) throw Errors.Index_out_of_range;
			if (count == 0) return empty;
			return new Vector<T>(root.Take(count));
		}
		/// <summary>
		/// Returns a vector consisting of the first items that fulfill the predicate. O(m), fast.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public Vector<T> TakeWhile(Func<T, bool> predicate)
		{
			var index = this.IndexOf(predicate);
			if (!index.HasValue)
			{
				return this;
			}
			return Take((int) index + 1);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		/// <summary>
		/// Gets the empty vector.
		/// </summary>
		public static Vector<T> Empty
		{
			get
			{
				return empty;
			}
		}

		private class VectorDebugView
		{
			private readonly Vector<T> _inner;

			public VectorDebugView(Vector<T> inner)
			{
				_inner = inner;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
			public T[] Contents
			{
				get
				{
					return _inner.ToArray();
				}
			}
		}
	}
}