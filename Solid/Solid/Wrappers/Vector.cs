using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

		public Vector<T> AddLastRange(IEnumerable<T> items)
		{
			Vector<T> vec = this;
			foreach (T item in items)
			{
				vec = vec.AddLast(item);
			}
			return vec;
		}

		public Vector<TOut> Apply<TOut>(Func<T, TOut> transform)
		{
			return new Vector<TOut>(root.Apply(transform));
		}

		/// <summary>
		/// Removes the last item from the list.
		/// </summary>
		/// <returns></returns>
		public Vector<T> DropLast()
		{
			return new Vector<T>(root.Drop());
		}

		/// <summary>
		/// Removes several items from the end of the list.
		/// </summary>
		/// <param name="count">The number of items to remove.</param>
		/// <returns></returns>
		public Vector<T> DropLast(int count)
		{
			return new Vector<T>(root.TakeFirst(root.Count - count));
		}

		public void ForEach(Action<T> action)
		{
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
			return new Vector<T>(root.Set(index, item));
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