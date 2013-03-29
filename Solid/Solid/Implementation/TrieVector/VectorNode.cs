using System;
using System.Collections.Generic;

namespace Solid.TrieVector
{
	internal abstract class VectorNode<T>
	{
		public readonly int Count;
		public readonly int Height;
		public readonly bool IsFull;
		private static readonly VectorNode<T> empty = new VectorLeaf<T>(new T[0]);

		protected VectorNode(int height, int count, bool isFull)
		{
			Height = height;
			Count = count;
			IsFull = isFull;
		}

		public abstract T this[int index]
		{
			get;
		}

		public abstract VectorNode<T> Add(T item);

		public abstract VectorNode<TOut> Apply<TOut>(Func<T, TOut> transform);

		public abstract VectorNode<T> Drop();

		public abstract IEnumerator<T> GetEnumerator();


		public abstract void IterBack(Action<T> action);

		public abstract bool IterWhile(Func<T, bool> conditional);

		public abstract bool IterBackWhile(Func<T, bool> conditional);

		public abstract VectorNode<T> BulkLoad(T[] items, int startIndex, int count);

		public abstract void Iter(Action<T> action);

		public abstract VectorNode<T> Set(int index, T value);

		public abstract VectorNode<T> Take(int index);

		public static VectorNode<T> Empty
		{
			get
			{
				return empty;
			}
		}
	}
}