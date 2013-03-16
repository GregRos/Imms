using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Solid.Convertion;
using Solid.TrieVector;

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
	[DebuggerTypeProxy(typeof(Vector<>.VectorDebugView))]
	public sealed class Vector<T> : IEnumerable<T>
	{
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
		internal string DebuggerDisplay
		{
			get
			{
				return string.Format("Vector, Count = {0}", this.Count);
			}
		}
		private readonly VectorNode<T> root;
		public static readonly Vector<T> empty = new Vector<T>(VectorNode<T>.Empty);
		internal Vector(VectorNode<T> root)
		{
			this.root = root;
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

		public static Vector<T> Empty
		{
			get
			{
				return empty;
			}
		}

		public Vector<T> AddLast(T item)
		{
			return this.Add(item);
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
				return root[index];
			}
		}

		/// <summary>
		/// Sets the value of the item with the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public Vector<T> Set(int index, T item)
		{
			return new Vector<T>(root.Set(index,item));
		}

		/// <summary>
		/// Adds an item to the end of the list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Vector<T> Add(T item)
		{
			return new Vector<T>(root.Add(item));
		}

		public Vector<T> AddRange(IEnumerable<T> items)
		{
			var vec = this;
			foreach (var item in items)
			{
				vec = vec.Add(item);
			}
			return vec;
		}

		/// <summary>
		/// Removes the last item from the list.
		/// </summary>
		/// <returns></returns>
		public Vector<T> Drop()
		{
			return new Vector<T>(root.Drop());
		}

		/// <summary>
		/// Retrieves a subsequence consisting of the first several items from the list.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Vector<T> Take(int count)
		{
			return new Vector<T>(root.TakeFirst(count));
		}

		/// <summary>
		/// Removes several items from the end of the list.
		/// </summary>
		/// <param name="count">The number of items to remove.</param>
		/// <returns></returns>
		public Vector<T> Drop(int count)
		{
			return new Vector<T>(root.TakeFirst(root.Count - count));
		}

		public Vector<TOut> Apply<TOut>(Func<T,TOut> transform)
		{
			return new Vector<TOut>(root.Apply(transform));
		}

		public void ForEach(Action<T> action)
		{
			root.Iter(action);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
