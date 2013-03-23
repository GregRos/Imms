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
		/// Gets an empty Vector.
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
		/// Gets the value of the item with the specified index.
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
		/// Returns the length of the list.
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

		public Vector<T> AddLast(T item)
		{
			return new Vector<T>(root.Add(item));
		}

		public Vector<T> AddLastItems(params T[] items)
		{
			if (items == null) throw Errors.Argument_null("items");
			return this.AddLastRange(items.AsEnumerable());
		}

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

		public Vector<TOut> Select<TOut>(Func<T, TOut> transform)
		{
			if (transform == null) throw Errors.Argument_null("transform");
			return new Vector<TOut>(root.Apply(transform));
		}
		
		public T First
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[0];
			}
		}

		public T Last
		{
			get
			{
				if (Count == 0) throw Errors.Is_empty;
				return this[-1];
			}
		}


		/// <summary>
		/// Removes the last item from the list.
		/// </summary>
		/// <returns></returns>
		public Vector<T> DropLast()
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Drop());
		}

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

		public TResult Aggregate<TResult>(Func<TResult, T, TResult> accumulator, TResult initial)
		{
			this.ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		public void ForEachWhile(Func<T, bool> conditional)
		{
			root.IterWhile(conditional);
		}

		public void ForEachBack(Action<T> action)
		{
			root.IterBack(action);
		}

		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			root.IterBackWhile(conditional);
		}


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
		/// Removes several items from the end of the list.
		/// </summary>
		/// <param name="count">The number of items to remove.</param>
		/// <returns></returns>
		public Vector<T> DropLast(int count)
		{
			if (root.Count == 0) throw Errors.Is_empty;
			return new Vector<T>(root.Take(root.Count - count));
		}

		public void ForEach(Action<T> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			root.Iter(action);
		}

		/// <summary>
		/// Sets the value of the item with the specified index.
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
		/// Retrieves a subsequence consisting of the first several items from the list.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Vector<T> Take(int count)
		{
			if (count < 0 || count > Count) throw Errors.Index_out_of_range;
			if (count == 0) return empty;
			return new Vector<T>(root.Take(count));
		}

		public IEnumerator<T> GetEnumerator()
		{
			return root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

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