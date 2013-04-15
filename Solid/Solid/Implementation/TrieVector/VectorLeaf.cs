using System;
using System.Collections;
using System.Collections.Generic;
using Solid.Common;
using Solid.TrieVector.Iteration;

namespace Solid
{
	internal static partial class TrieVector<TValue>
	{
		internal sealed class VectorLeaf : VectorNode
		{
			internal class LeafEnumerator : IEnumerator<TValue>
			{
				private int index = -1;
				private readonly VectorLeaf node;

				public LeafEnumerator(VectorLeaf node)
				{
					this.node = node;
				}

				public TValue Current
				{
					get
					{
						return node[index];
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					index++;
					return index < node.Count;
				}

				public void Reset()
				{
					index = -1;
				}
			}
			private const int myBlock = (1 << 5) - 1;
			public readonly TValue[] Arr;

			public VectorLeaf(TValue[] arr)
				: base(0, arr.Length, arr.Length == 32)
			{
				Arr = arr;
			}

			public override TValue this[int index]
			{
				get
				{
					var bits = index & myBlock;
					return Arr[bits];
				}
			}

			public override VectorNode Add(TValue item)
			{
				if (Count < 32)
				{
					var myCopy = new TValue[Arr.Length + 1];
					Arr.CopyTo(myCopy, 0);
					myCopy[myCopy.Length - 1] = item;
					var newArr = myCopy;
					return new VectorLeaf(newArr);
				}
				var parentArr = new VectorNode[2];
				var childArr = new TValue[1];
				childArr[0] = item;
				var newNode = new VectorLeaf(childArr);
				parentArr[0] = this;
				parentArr[1] = newNode;
				return new VectorParent(1, 33, parentArr);
			}

			public override TrieVector<TOut>.VectorNode Apply<TOut>(Func<TValue, TOut> transform)
			{
				var newArr = new TOut[Arr.Length];
				Arr.CopyTo(newArr, 0);
				for (var i = 0; i < newArr.Length; i++)
				{
					newArr[i] = transform(Arr[i]);
				}
				return new TrieVector<TOut>.VectorLeaf(newArr);
			}

			public override VectorNode BulkLoad(TValue[] data, int startIndex, int count)
			{
				var newArraySize = Math.Min(32, Arr.Length + count);
				var newArray = new TValue[newArraySize];
				var loadCount = Math.Min(32 - Arr.Length, count);
				Arr.CopyTo(newArray, 0);
				Array.Copy(data, startIndex, newArray, Arr.Length, loadCount);
				var newLeaf = new VectorLeaf(newArray);
				if (loadCount < count)
				{
					var newParentArr = new VectorNode[1];
					newParentArr[0] = newLeaf;
					var newParent = new VectorParent(1, newArray.Length, newParentArr);
					return newParent.BulkLoad(data, startIndex + loadCount, count - loadCount);
				}
				return newLeaf;
			}

			public override VectorNode Drop()
			{
				var newArr = new TValue[Arr.Length - 1];
				Array.Copy(Arr, 0, newArr, 0, Arr.Length - 1);
				return new VectorLeaf(newArr);
			}

			public override IEnumerator<TValue> GetEnumerator()
			{
				return new LeafEnumerator(this);
			}

			public override void Iter(Action<TValue> action)
			{
#if DEBUG
			action.IsNotNull();
#endif
				for (var i = 0; i < Arr.Length; i++)
				{
					action(Arr[i]);
				}
			}

			public override void IterBack(Action<TValue> action)
			{
#if DEBUG
			action.IsNotNull();
#endif
				for (var i = Arr.Length - 1; i >= 0; i--)
				{
					action(Arr[i]);
				}
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional)
			{
#if DEBUG
			conditional.IsNotNull();
#endif
				for (var i = Arr.Length - 1; i >= 0; i--)
				{
					if (!conditional(Arr[i]))
					{
						return false;
					}
				}
				return true;
			}

			public override bool IterWhile(Func<TValue, bool> conditional)
			{
				for (var i = 0; i < Arr.Length; i++)
				{
					if (!conditional(Arr[i]))
					{
						return false;
					}
				}
				return true;
			}

			public override VectorNode Set(int index, TValue value)
			{
				var bits = index & myBlock;
#if DEBUG
			bits.Is(i => i >= 0 && i < Count);
#endif
				var myCopy = new TValue[Arr.Length];
				Arr.CopyTo(myCopy, 0);
				myCopy[bits] = value;
				var newArr = myCopy;
				return new VectorLeaf(newArr);
			}

			public override VectorNode Take(int index)
			{
				var bits = index & myBlock;
				var newArr = Arr.TakeFirst(bits);
				return new VectorLeaf(newArr);
			}
		}
	}

	
}