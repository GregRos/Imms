using System;
using System.Collections.Generic;

namespace Solid.TrieVector
{
	internal abstract class VectorNode<T>
	{
		private readonly static VectorNode<T> empty = new VectorLeaf<T>(new T[0]);
		public readonly int Height;
		public readonly bool IsFull;
		public readonly int Count;

		protected VectorNode(int height, int count, bool isFull)
		{
			Height = height;
			Count = count;
			IsFull = isFull;
		}

		public abstract T this[int index] { get; }
		public static VectorNode<T> Empty
		{
			get
			{
				return empty;
			}
		}
		public abstract VectorNode<T> Add(T item);
		public abstract VectorNode<T> Drop();
		public abstract VectorNode<T> TakeFirst(int index);
		public abstract VectorNode<T> Set(int index, T value);

		public abstract void Iter(Action<T> action);

		public abstract VectorNode<TOut> Apply<TOut>(Func<T, TOut> transform);
		public abstract VectorNode<T> Fill(IList<T> items, int start, out int count);
		public abstract IEnumerator<T> GetEnumerator();
	}
}