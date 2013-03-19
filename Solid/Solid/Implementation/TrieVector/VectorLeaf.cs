using System;
using System.Collections.Generic;
using NUnit.Framework;
using Solid.Common;
using Solid.TrieVector.Iteration;

namespace Solid.TrieVector
{
	internal sealed class VectorLeaf<T> : VectorNode<T>
	{
		private const int myBlock = (1 << 5) - 1;
		public readonly T[] Arr;

		public VectorLeaf(T[] arr) : base(0, arr.Length, arr.Length == 32)
		{
			Arr = arr;
		}


		public override T this[int index]
		{
			get
			{
				int bits = index & myBlock;
				return Arr[bits];
			}
		}

		public override VectorNode<T> Add(T item)
		{
			if (Count < 32)
			{
				var myCopy = new T[Arr.Length + 1];
				Arr.CopyTo(myCopy, 0);
				myCopy[myCopy.Length - 1] = item;
				T[] newArr = myCopy;
				return new VectorLeaf<T>(newArr);
			}
			var parentArr = new VectorNode<T>[2];
			var childArr = new T[1];
			childArr[0] = item;
			var newNode = new VectorLeaf<T>(childArr);
			parentArr[0] = this;
			parentArr[1] = newNode;
			return new VectorParent<T>(1, 33, parentArr);
		}

		public override VectorNode<T> Drop()
		{
			return new VectorLeaf<T>(Arr);
		}

		public override VectorNode<TOut> Apply<TOut>(Func<T, TOut> transform)
		{
			throw new NotImplementedException();
		}

		public override VectorNode<T> Fill(IList<T> items, int start, out int count)
		{
			items.IsNotNull();
			start.Is(i => i < Count && i > 0);
			var newArr = new T[Math.Min(Arr.Length + items.Count, 32)];
			Arr.CopyTo(newArr, 0);
			int c = 0;
			for (; c < newArr.Length; c++)
			{
				newArr[Arr.Length + c] = items[c];
			}
			count = c;
			return new VectorLeaf<T>(newArr);
		}



		public override VectorNode<T> TakeFirst(int index)
		{
			int bits = index & myBlock;
			T[] newArr = Arr.TakeFirst(bits);
			return new VectorLeaf<T>(newArr);
		}

		public override VectorNode<T> Set(int index, T value)
		{
			
			int bits = index & myBlock;
			bits.Is(i => i >= 0 && i < Count);
			var myCopy = new T[Arr.Length];
			Arr.CopyTo(myCopy, 0);
			myCopy[bits] = value;
			T[] newArr = myCopy;
			return new VectorLeaf<T>(newArr);
		}

		public override void Iter(Action<T> action)
		{
			action.IsNotNull();
			for (int i = 0; i < Arr.Length; i++)
			{
				action(Arr[i]);
			}
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return new LeafEnumerator<T>(this);
		}
	}
}