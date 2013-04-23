using System;
using System.Collections;
using System.Collections.Generic;
using Solid.Common;
using Solid.TrieVector.Iteration;

namespace Solid
{
	internal static partial class TrieVector<TValue>
	{
		internal sealed class VectorParent : VectorNode
		{
			internal class ParentEnumerator : IEnumerator<TValue>
			{
				private IEnumerator<TValue> _current;
				private int index = -1;
				private readonly VectorParent _node;
				private readonly bool _forward;
				public ParentEnumerator(VectorParent node, bool forward)
				{
					this._node = node;
					_forward = forward;
				}

				public TValue Current
				{
					get
					{
						return _current.Current;
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
					if (index == -1)
					{
						return TryNext();
					}
					if (_current.MoveNext())
					{
						return true;
					}
					return TryNext();
				}

				public void Reset()
				{
					index = -1;
				}

				public bool TryNext()
				{
					index++;
					if (index < _node.Arr.Length)
					{
						var ix = _forward ? index : _node.Arr.Length - 1 - index;
						_current = _node.Arr[ix].GetEnumerator(_forward);
						return _current.MoveNext();
					}
					return false;
				}
			}

			private readonly int myBlock;
			private readonly int offs;
			public readonly VectorNode[] Arr;

			public VectorParent(int height, int count, VectorNode[] arr)
				: base(height, count, arr.Length == 32 && arr[arr.Length - 1].IsFull)
			{
#if DEBUG
				arr.IsNotNull();
#endif

				Arr = arr;
				offs = height * 5;
				myBlock = ((1 << 5) - 1) << offs;
			}

			public override TValue this[int index]
			{
				get
				{
					var myIndex = index & myBlock;
					myIndex = myIndex >> offs;
					return Arr[myIndex][index];
				}
			}

			public override VectorNode Add(TValue item)
			{
				VectorNode[] newMyArr;
				if (!Arr[Arr.Length - 1].IsFull || Arr[Arr.Length - 1].Height < Height - 1)
				{
					var newNode = Arr[Arr.Length - 1].Add(item);
					var myCopy = new VectorNode[Arr.Length];
					Arr.CopyTo(myCopy, 0);
					myCopy[Arr.Length - 1] = newNode;
					newMyArr = myCopy;
					return new VectorParent(Height, Count + 1, newMyArr);
				}
				if (Arr.Length != 32)
				{
					var newArr = new TValue[1];
					newArr[0] = item;
					var newNode = new VectorLeaf(newArr);
					var myCopy = new VectorNode[Arr.Length + 1];
					Arr.CopyTo(myCopy, 0);
					myCopy[myCopy.Length - 1] = newNode;
					newMyArr = myCopy;
					return new VectorParent(Height, Count + 1, newMyArr);
				}
				newMyArr = new VectorNode[2];
				var newArrLeaf = new TValue[1];
				newArrLeaf[0] = item;
				newMyArr[0] = this;

				newMyArr[1] = new VectorLeaf(newArrLeaf);
				return new VectorParent(Height + 1, Count + 1, newMyArr);
			}

			public override TrieVector<TOut>.VectorNode Apply<TOut>(Func<TValue, TOut> transform)
			{
				var newArr = new TrieVector<TOut>.VectorNode[Arr.Length];
				for (var i = 0; i < newArr.Length; i++)
				{
					newArr[i] = Arr[i].Apply(transform);
				}
				return new TrieVector<TOut>.VectorParent(Height, Count, newArr);
			}

			public override VectorNode BulkLoad(TValue[] items, int startIndex, int count)
			{
				if (count == 0) return this;
				var oldCount = count;
				var maybeArraySize = 1 + ((count + Count - 1) >> offs);
				var realArraySize = Math.Min(32, maybeArraySize);
				var newArray = new VectorNode[realArraySize];
				Arr.CopyTo(newArray, 0);
				var lastCount = Arr[Arr.Length - 1].Count;
				var childMaxCapacity = (1 << offs);
				var lastRemainingCapacity = childMaxCapacity - lastCount;
				var newLast = Arr[Arr.Length - 1].BulkLoad(items, startIndex, Math.Min(count, lastRemainingCapacity));
				newArray[Arr.Length - 1] = newLast;
				startIndex = startIndex + lastRemainingCapacity;
				count = Math.Max(0,count - lastRemainingCapacity);

				var emptyLeaf = Empty;
				for (var i = Arr.Length; i < newArray.Length; i++)
				{
					var howManyToLoad = Math.Min(childMaxCapacity, count);
					newArray[i] = emptyLeaf.BulkLoad(items, startIndex, howManyToLoad);
					count -= howManyToLoad;
					startIndex += howManyToLoad;
				}

				var newMyself = new VectorParent(Height, Count + (oldCount - count), newArray);
				if (count > 0)
				{
					var newParentArr = new VectorNode[1];
					newParentArr[0] = newMyself;
					var newParent = (VectorNode) new VectorParent(Height + 1, newMyself.Count, newParentArr);
					newParent = newParent.BulkLoad(items, startIndex, count);
					return newParent;
				}
				return newMyself;
			}

			public override VectorNode Drop()
			{
				if (Arr.Length == 2 && Arr[1].Count == 1)
				{
					return Arr[0];
				}
				VectorNode[] newMyArr;
				if (Arr[Arr.Length - 1].Count == 1)
				{
					newMyArr = Arr.Remove();
					return new VectorParent(Height, Count - 1, newMyArr);
				}
				var newLast = Arr[Arr.Length - 1].Drop();
				var myCopy = new VectorNode[Arr.Length];
				Arr.CopyTo(myCopy, 0);
				myCopy[Arr.Length - 1] = newLast;
				newMyArr = myCopy;
				return new VectorParent(Height, Count - 1, newMyArr);
			}

			public override IEnumerator<TValue> GetEnumerator(bool forward)
			{
				return new ParentEnumerator(this,forward);
			}

			public override void Iter(Action<TValue> action)
			{
#if DEBUG
				action.IsNotNull();
#endif
				for (var i = 0; i < Arr.Length; i++)
				{
					Arr[i].Iter(action);
				}
			}

			public override void IterBack(Action<TValue> action)
			{
				for (var i = Arr.Length - 1; i >= 0; i--)
				{
					Arr[i].IterBack(action);
				}
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional)
			{
				for (var i = Arr.Length - 1; i >= 0; i--)
				{
					if (!Arr[i].IterBackWhile(conditional))
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
					if (!Arr[i].IterWhile(conditional))
					{
						return false;
					}
				}
				return true;
			}

			public override VectorNode Set(int index, TValue value)
			{
				var myIndex = (index) & myBlock;
				myIndex = myIndex >> offs;
#if DEBUG
				myIndex.Is(i => i < Count && i >= 0);
#endif
				var myNewNode = Arr[myIndex].Set(index, value);
				var myCopy = new VectorNode[Arr.Length];
				Arr.CopyTo(myCopy, 0);
				myCopy[myIndex] = myNewNode;
				var myNewArr = myCopy;
				return new VectorParent(Height, Count, myNewArr);
			}

			public override VectorNode Take(int index)
			{
#if DEBUG
				index.Is(i => i < Count && i > 0);
#endif
				var myCount = index & myBlock;
				myCount = myCount >> (offs);
				var myArrFirst = Arr.TakeFirst(myCount + 1);
				var myNewLast = Arr[myCount].Take(index);
				myArrFirst[myCount] = myNewLast;
				return new VectorParent(Height, Arr[0].Count * (myCount - 1) + myNewLast.Count, myArrFirst);
			}
		}
	}
}