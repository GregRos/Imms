using System;
using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	internal static partial class TrieVector<TValue> {
		static readonly IEqualityComparer<TValue> Eq = EqualityComparer<TValue>.Default;
		internal abstract partial class Node
		{
			private static readonly Node empty = new Leaf(new TValue[0], Lineage.Immutable, 0);
			 
			public int ArrSize;
			public int Length;
			public readonly int Height;
			public bool IsFull;
			public readonly Lineage Lineage;

			protected Node(int height, int length, bool isFull, Lineage lineage, int arrSize)
			{
				Height = height;
				Length = length;
				IsFull = isFull;
				Lineage = lineage;
				ArrSize = arrSize;
			}

			public static Node Empty
			{
				get
				{
					return empty;
				}
			}

			public abstract TValue this[int index] { get; }

			public abstract Node Add(TValue item, Lineage lineage);

			public abstract Node AddMany(TValue[] arr, Lineage lineage, int maxHeight, ref int count, ref int i);

			public abstract TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage);

			public abstract Node Drop(Lineage lineage);

			public abstract IEnumerator<TValue> GetEnumerator(bool forward);

			public abstract void Iter(Action<TValue> action);

			public abstract void IterBack(Action<TValue> action);

			/// <summary>
			/// This method is not optimized.
			/// </summary>
			public abstract bool IsParent { get; }

			public abstract bool IterBackWhile(Func<TValue, bool> conditional);

			public abstract bool IterWhile(Func<TValue, bool> conditional);

			public abstract Node Take(int index, Lineage lineage);

			public abstract Node Update(int index, TValue value, Lineage lineage);

			public abstract Node Reverse(Lineage lineage);

			public IEnumerable<TValue> Iterate()
			{
				var stack = new List<Node>(32 * this.Height );
				stack.Add(this);
				while (stack.Count > 0)
				{
					var curNode = stack[stack.Count - 1];
					stack.RemoveAt(stack.Count - 1);
					var asLeaf = curNode as Leaf;
					if (asLeaf != null)
					{
						for (int i = 0; i < asLeaf.ArrSize; i++)
						{
							yield return asLeaf.Arr[i];
						}
					}
					else
					{
						var asParent = (Parent) curNode;
						for (int i = asParent.ArrSize - 1; i >= 0; i--)
						{
							stack.Add(asParent.Arr[i]);
						}
					}
				}
			}
		}
	}
}