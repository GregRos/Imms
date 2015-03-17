using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;
using System.Linq;
namespace Funq.Collections.Implementation
{
	internal static partial class TrieVector<TValue>
	{
		internal sealed class Parent : Node
		{
			internal class Enumerator : IEnumerator<TValue>
			{
				private IEnumerator<TValue> _current;
				private readonly bool _forward;
				private readonly Parent _node;
				private int index = -1;

				public Enumerator(Parent node, bool forward)
				{
					_node = node;
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
					if (index < _node.ArrSize)
					{
						var ix = _forward ? index : _node.ArrSize - 1 - index;
						_current = _node.Arr[ix].GetEnumerator(_forward);
						return _current.MoveNext();
					}
					return false;
				}
			}

			private readonly int myBlock;
			private readonly int offs;
			public Node[] Arr;

			public Parent(int height, int length, Node[] arr, Lineage lineage, int arrSize)
				: base(height, length, arrSize == 32 && arr[arrSize - 1].IsFull, lineage, arrSize)
			{
#if DEBUG
				arr.IsNotNull();
#endif

				Arr = arr;
				offs = height * 5;
				myBlock = ((1 << 5) - 1) << offs;
#if DEBUG
				Length.Is(Arr.Take(ArrSize).Aggregate(0, (tot, cur) => cur != null ? tot + cur.Length : tot));
				var leafIndex = Array.FindIndex(arr, node => node != null && !node.IsParent);
				var parentIndex = Array.FindLastIndex(arr, node => node != null && node.IsParent);
				leafIndex.Is(i => i == -1 || i > parentIndex);
#endif
			}

			public Parent(Node first, Node second, Lineage lineage)
				: base(first.Height + 1, first.Length + second.Length, false, lineage, 2)
			{
				Arr = new[] {first, second};
				offs = Height * 5;
				myBlock = ((1 << 5) - 1) << offs;
			}

			public override TValue this[int index]
			{
				get
				{
					return Arr[(index & myBlock) >> offs][index];
				}
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private Node _mutate(int count, int arrSize, Node[] arr)
			{
				Length = count;
				ArrSize = arrSize;
				Arr = arr;
				IsFull = arrSize == 32 && arr[arrSize - 1].IsFull;
				return this;
			}

			private Node MutateOrCreate(int count, int arrSize, Node[] arr, Lineage lineage)
			{
				var ret =  Lineage.AllowMutation(lineage) ? _mutate(count, arrSize, arr) : new Parent(Height, count, arr, lineage, arrSize);
#if DEBUG
				ret.Length.Is(arr.Take(ret.ArrSize).Aggregate(0, (tot, cur) => cur != null ? tot + cur.Length : tot));
				var leafIndex = Array.FindIndex(arr, node => node != null && !node.IsParent);
				var parentIndex = Array.FindLastIndex(arr, node => node != null && node.IsParent);
				leafIndex.Is(i => i == -1 || i > parentIndex);
#endif
				return ret;
			}

			private Node[] UpdateStore(int index, Node v, Lineage lineage)
			{
				if (Lineage.AllowMutation(lineage))
				{
					if (Arr.Length < 32) Arr = Arr.Take(32);
					Arr[index] = v;
					return Arr;
				}
				else return Arr.Update(index, v, ArrSize);
			}



			private Node[] AddToStore(Node v, Lineage lineage)
			{
				if (Lineage.AllowMutation(lineage))
				{
					if (Arr.Length < 32) Arr = Arr.Take(32);
					Arr[ArrSize] = v;
					return Arr;
				}
				else
				{
					var arr = Arr.Take(ArrSize + 1);
					arr[ArrSize] = v;
					return arr;
				}
			}


			public override bool IsParent
			{
				get
				{
					return true;
				}
			}

			public override Node Add(TValue item, Lineage lineage)
			{
				int lastIndex = ArrSize - 1;
				Node lastNode = Arr[lastIndex];
				Node ret;
				var expected_length = Length + 1;
				if (!lastNode.IsFull || lastNode.Height < Height - 1)
				{
					var newNode = lastNode.Add(item, lineage);
					var myCopy = UpdateStore(lastIndex, newNode, lineage);
					ret = MutateOrCreate(Length + 1, ArrSize, myCopy, lineage);
				}
				else if (ArrSize != 32)
				{
					var newNode = new Leaf(item, lineage);
					var newArr = AddToStore(newNode, lineage);
					ret = MutateOrCreate(Length + 1, ArrSize + 1, newArr, lineage);
				}
				else
				{
					var newArr = new Node[] {this, new Leaf(item, lineage)};
					ret = new Parent(Height + 1, Length + 1, newArr, lineage, 2);
				}
#if DEBUG
				ret.Length.Is(expected_length);
				ret[ret.Length - 1].Is(item);
#endif
				return ret;
			}

			public override Node AddMany(TValue[] arr, Lineage lineage, int maxHeight, ref int start, ref int count)
			{
				if (count == 0) return this;
#if DEBUG
				var old_length = Length;
#endif
				var newArr = Arr.Take(32);
				var startCount = count;
				int i;
				for (i = ArrSize - 1; i < 32 && count > 0; i++)
				{
					newArr[i] = newArr[i] ?? Empty;
					newArr[i] = newArr[i].AddMany(arr, lineage, Height - 1, ref start, ref count);
				}
				Node ret = new Parent(Height, Length + startCount - count, newArr, lineage, i);
				if (Height < maxHeight && count != 0)
				{
					var myParent = new Parent(ret, Empty, lineage);
					ret = myParent.AddMany(arr, lineage, maxHeight, ref start, ref count);
				}
#if DEBUG
				ret.Length.Is(old_length + startCount - count);
				if (startCount - count > 0) ret[ret.Length - 1].Is(arr[start - 1]);
#endif
				return ret;
			}

			public override TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage)
			{
				var newArr = new TrieVector<TOut>.Node[ArrSize];
				for (var i = 0; i < ArrSize; i++)
				{
					newArr[i] = Arr[i].Apply(transform, lineage);
				}
				return new TrieVector<TOut>.Parent(Height, Length, newArr, lineage, ArrSize);
			}

			public override Node Drop(Lineage lineage)
			{
				Node ret;
#if DEBUG
				var expected_length = Length - 1;
				var expected_last = Length > 1 ? Option.Some(this[Length - 2]) : Option.None;
#endif
				

				if (ArrSize == 2 && Arr[1].Length == 1)
				{
					ret = Arr[0];
				}
				else if (Arr[ArrSize - 1].Length == 1)
				{
					ret = MutateOrCreate(Length - 1, ArrSize - 1, Arr.Take(ArrSize - 1), lineage);
				}
				else
				{
					var newLast = Arr[ArrSize-1].Drop(lineage);
					var newArr = UpdateStore(ArrSize - 1, newLast, lineage);
					ret =  MutateOrCreate(Length - 1, ArrSize, newArr, lineage);
				}
#if DEBUG
				ret.Length.Is(expected_length);
				if (expected_last.IsSome) ret[ret.Length - 1].Is(expected_last.Value);
#endif
				return ret;

			}

			public override IEnumerator<TValue> GetEnumerator(bool forward)
			{
				return new Enumerator(this, forward);
			}

			public override void Iter(Action<TValue> action)
			{
#if DEBUG
				action.IsNotNull();
#endif
				for (var i = 0; i < ArrSize; i++)
				{
					Arr[i].Iter(action);
				}
			}

			public override void IterBack(Action<TValue> action)
			{
				for (var i = ArrSize - 1; i >= 0; i--)
				{
					Arr[i].IterBack(action);
				}
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional)
			{
				for (var i = ArrSize - 1; i >= 0; i--)
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
				for (var i = 0; i < ArrSize; i++)
				{
					if (!Arr[i].IterWhile(conditional))
					{
						return false;
					}
				}
				return true;
			}

			public override Node Take(int index, Lineage lineage)
			{
#if DEBUG
				var expected_last = index < Length ? Option.Some(this[index]) : Option.None;
#endif
				var myIndex = index & myBlock;
				myIndex = myIndex >> (offs);
				Node ret;
				if (myIndex == 0)
				{
					ret = Arr[0].Take(index, lineage);
				}
				else
				{
					var myArrFirst = Arr.Take(myIndex + 1);
					var myNewLast = Arr[myIndex].Take(index, lineage);

					myArrFirst[myIndex] = myNewLast;
					ret = new Parent(Height, Arr[0].Length * (myIndex) + myNewLast.Length, myArrFirst, lineage, myIndex + 1);
				}

#if DEBUG
				ret.Length.Is(index + 1);
				if (expected_last.IsSome) ret[ret.Length - 1].Is(expected_last.Value);
#endif
				return ret;
			}

			public override Node Update(int index, TValue value, Lineage lineage)
			{
#if DEBUG
				var expected_length = Length;
#endif
				var myIndex = (index) & myBlock;
				myIndex = myIndex >> offs;
				var updatedChild = Arr[myIndex].Update(index, value, lineage);
				var newArr = UpdateStore(myIndex, updatedChild, lineage);
				Node ret = MutateOrCreate(Length, ArrSize, newArr, lineage);
#if DEBUG
				ret.Length.Is(expected_length);
				ret[index].Is(value);
#endif
				return ret;
			}

			public override Node Reverse(Lineage lineage)
			{
				var newArr = new Node[Arr.Length];
				for (int i = 0; i < ArrSize; i++)
				{
					newArr[i] = Arr[ArrSize - i - 1].Reverse(lineage);
				}
				return new Parent(Height, Length, newArr, lineage, ArrSize);
			}
		}
	}
}