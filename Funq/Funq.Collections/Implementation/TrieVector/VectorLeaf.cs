using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	internal static partial class TrieVector<TValue>
	{
		internal sealed class Leaf : Node
		{
			internal class Enumerator : IEnumerator<TValue>
			{
				private readonly bool _forward;
				private readonly Leaf _node;
				private int index = -1;

				public Enumerator(Leaf node, bool forward)
				{
					_node = node;
					_forward = forward;
				}

				public TValue Current
				{
					get
					{
						var ix = _forward ? index : _node.ArrSize - 1 - index;
						return _node.Arr[ix];
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
					return index < _node.Length;
				}

				public void Reset()
				{
					index = -1;
				}
			}

			private const int myBlock = (1 << 5) - 1;
			public TValue[] Arr;

			public Leaf(TValue[] arr, Lineage lineage, int arrSize)
				: base(0, arrSize, arrSize == 32, lineage, arrSize)
			{
				Arr = arr;
			}

			public Leaf(TValue item, Lineage lineage)
				: this(new[] {item}, lineage, 1)
			{
			}

			private TValue[] UpdateStore(int index, TValue v, Lineage lineage)
			{
				if (Lineage.AllowMutation(lineage))
				{
					if (Arr.Length < 32) Arr = Arr.Take(32);
					Arr[index] = v;
					return Arr;
				}
				else return Arr.Update(index, v, ArrSize);
			}



			private TValue[] AddToStore(TValue v, Lineage lineage)
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

			private Node _mutate(TValue[] arr, int arrSize)
			{
				Arr = arr;
				Length = arrSize;
				ArrSize = arrSize;
				IsFull = arrSize == 32;
				return this;

			}

			private Node MutateOrCreate(int arrSize, TValue[] arr, Lineage lineage)
			{
				return Lineage.AllowMutation(lineage) ? _mutate(arr, arrSize) : new Leaf(arr, lineage, arrSize);
			}


			public override TValue this[int index]
			{
				get
				{
					return Arr[index & myBlock];
				}
			}

			public override Node Add(TValue item, Lineage lineage)
			{
				Node ret;
				var expected_length = Length + 1;
				if (ArrSize < 32)
				{
					var myCopy = AddToStore(item, lineage);
					ret = MutateOrCreate(ArrSize + 1, myCopy, lineage);
				}
				else
				{
					var newLeaf = new Leaf(item, lineage);
					ret = new Parent(this, newLeaf, lineage);
				}
#if DEBUG
				ret.Length.Is(expected_length);
				ret[ret.Length - 1].Is(item);
#endif
				return ret;
			}

			public override Node AddMany(TValue[] arr, Lineage lineage, int maxHeight, ref int start, ref int count)
			{
				var old_length = Length;
				if (count == 0) return this;
				var len = ArrSize + count > 32 ? 32 - ArrSize : count;
				var newArr = Arr.Take(ArrSize + len);
				var origCount = count;
				Array.Copy(arr, start, newArr, Length, len);
				start += len;
				count -= len;
				Node ret = new Leaf(newArr, lineage, ArrSize + len);
				if (Height < maxHeight && count > 0)
				{
					var parentArr = ArrayExt.OfItems(32, ret, Empty);
					var myParent = new Parent(1, ret.Length, parentArr, lineage, 2);
					ret = myParent.AddMany(arr, lineage, maxHeight, ref start, ref count);
				}
#if DEBUG
				ret.Length.Is(old_length + origCount - count);
				if (len != 0) ret[ret.Length - 1].Is(arr[start - 1]);
#endif
				return ret;
			}

			public override TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage)
			{
				var newArr = new TOut[ArrSize];
				for (var i = 0; i < newArr.Length; i++)
				{
					newArr[i] = transform(Arr[i]);
				}
				return new TrieVector<TOut>.Leaf(newArr, lineage, ArrSize);
			}

			public override Node Drop(Lineage lineage)
			{
#if DEBUG
				var old_length = Length;
				var old_arr_size = ArrSize;
				var before_last = Length > 1 ? Option.Some(this[Length - 2]) : Option.None;
#endif
				Node ret;
				if (Lineage.AllowMutation(lineage))
				{
					ret =  Drop_MUTATES();
				}
				else
				{
					var newArr = Arr.Take(ArrSize - 1);
					ret =  new Leaf(newArr, lineage, ArrSize - 1);
				}
#if DEBUG
				ret.Length.Is(old_length - 1);
				ret.ArrSize.Is(old_arr_size - 1);
				if (before_last.IsSome) ret[ret.Length - 1].Is(before_last.Value);
#endif
				return ret;
			}

			private Node Drop_MUTATES()
			{
				ArrSize--;
				Length--;
				return this;
			}

			public override IEnumerator<TValue> GetEnumerator(bool heading)
			{
				return new Enumerator(this, heading);
			}

			public override void Iter(Action<TValue> action)
			{
#if DEBUG
				action.IsNotNull();
#endif
				for (var i = 0; i < ArrSize; i++)
				{
					action(Arr[i]);
				}
			}

			public override void IterBack(Action<TValue> action)
			{
#if DEBUG
				action.IsNotNull();
#endif
				for (var i = ArrSize - 1; i >= 0; i--)
				{
					action(Arr[i]);
				}
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional)
			{
#if DEBUG
				conditional.IsNotNull();
#endif
				for (var i = ArrSize - 1; i >= 0; i--)
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
				for (var i = 0; i < ArrSize; i++)
				{
					if (!conditional(Arr[i]))
					{
						return false;
					}
				}
				return true;
			}

			public override bool IsParent
			{
				get
				{
					return false;
				}
			}

			public override Node Take(int index, Lineage lineage)
			{
#if DEBUG
				var expected_last = index < Length ? this[index].AsSome() : Option.None;
#endif
				var bits = index & myBlock;
				var newArr = Arr.Take(bits + 1);
				Node ret = new Leaf(newArr, lineage, bits + 1);
#if DEBUG
				ret.Length.Is(bits + 1);
				if (expected_last.IsSome) ret[ret.Length - 1].Is(expected_last.Value);
#endif
				return ret;
			}

			public override Node Update(int index, TValue value, Lineage lineage)
			{
				var bits = index & myBlock;
#if DEBUG
				var expected_length = Length;
				bits.Is(i => i >= 0 && i < Length);
#endif
				var myCopy = UpdateStore(bits, value, lineage);
				var ret =  MutateOrCreate(ArrSize, myCopy, lineage);
#if DEBUG
				ret[index].Is(value);
				ret.Length.Is(expected_length);
#endif
				return ret;
			}

			public override Node Reverse(Lineage lineage)
			{
				var newArr = new TValue[Arr.Length];
				for (int i = 0; i < ArrSize; i++)
				{
					newArr[i] = Arr[ArrSize - i - 1];
				}
				return new Leaf(newArr, lineage, ArrSize);
			}


		}
	}
}