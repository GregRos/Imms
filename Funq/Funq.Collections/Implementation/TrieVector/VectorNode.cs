using System;
using System.Collections.Generic;

namespace Funq.Implementation {
	static partial class TrieVector<TValue> {
		internal abstract partial class Node {
			static readonly Node empty = new Leaf(new TValue[0], Lineage.Immutable, 0);
			public readonly int Height;
			protected readonly Lineage Lineage;
			public int ArrSize;
			public bool IsFull;
			public int Length;

			protected Node(int height, int length, bool isFull, Lineage lineage, int arrSize) {
				Height = height;
				Length = length;
				IsFull = isFull;
				Lineage = lineage;
				ArrSize = arrSize;
			}

			public static Node Empty {
				get { return empty; }
			}

			public abstract TValue this[int index] { get; }

			/// <summary>
			///     This method is not optimized.
			/// </summary>
			public abstract bool IsParent { get; }

			public abstract Node Add(TValue item, Lineage lineage);

			public abstract Node AddRange(TValue[] arr, Lineage lineage, int maxHeight, ref int count, ref int i);

			public abstract TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage);

			public abstract Node RemoveLast(Lineage lineage);

			public abstract IEnumerator<TValue> GetEnumerator(bool forward);

			public abstract void Iter(Action<TValue> action);

			public abstract void IterBack(Action<TValue> action);

			public abstract bool IterBackWhile(Func<TValue, bool> conditional);

			public abstract bool IterWhile(Func<TValue, bool> conditional);

			public abstract int RecursiveTotalLength();

			/// <summary>
			///     WARNING THE PARAMETER CALLED 'index' IS NOT THE INDEX IN THIS NODE, BUT IN THE ENTIRE TREE!!!! <br />
			///     IF YOU DO index &lt; this.Length you will get FALSE!!!!
			/// </summary>
			/// <param name="index"></param>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public abstract Node Take(int index, Lineage lineage);

			/// <summary>
			///     Begins iterating over the trie at the specified index.
			/// </summary>
			/// <param name="index"></param>
			/// <param name="conditional"></param>
			/// <returns></returns>
			public abstract bool IterWhileFrom(int index, Func<TValue, bool> conditional);

			public abstract Node Update(int index, TValue value, Lineage lineage);

			public abstract Node Reverse(Lineage lineage);

		}
	}
}