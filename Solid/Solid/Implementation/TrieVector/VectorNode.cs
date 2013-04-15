using System;
using System.Collections.Generic;

namespace Solid
{
	internal static partial class TrieVector<TValue>
	{
		internal abstract class VectorNode
		{
			private static readonly VectorNode empty = new VectorLeaf(new TValue[0]);
			public readonly int Count;
			public readonly int Height;
			public readonly bool IsFull;

			protected VectorNode(int height, int count, bool isFull)
			{
				Height = height;
				Count = count;
				IsFull = isFull;
			}

			public static VectorNode Empty
			{
				get
				{
					return empty;
				}
			}

			public abstract TValue this[int index] { get; }

			public abstract VectorNode Add(TValue item);

			public abstract TrieVector<TOut>.VectorNode Apply<TOut>(Func<TValue, TOut> transform);

			public abstract VectorNode BulkLoad(TValue[] items, int startIndex, int count);

			public abstract VectorNode Drop();

			public abstract IEnumerator<TValue> GetEnumerator();

			public abstract void Iter(Action<TValue> action);

			public abstract void IterBack(Action<TValue> action);

			public abstract bool IterBackWhile(Func<TValue, bool> conditional);

			public abstract bool IterWhile(Func<TValue, bool> conditional);

			public abstract VectorNode Set(int index, TValue value);

			public abstract VectorNode Take(int index);
		}
	}
	
}