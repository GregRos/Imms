using System;
using System.Collections.Generic;
using NUnit.Framework;
using Solid.Common;
using Solid.TrieVector.Iteration;

namespace Solid.TrieVector
{
	public class CountingIterator<T>
	{
		private readonly IList<T> inner;
		private int index;

		public CountingIterator(IList<T> inner)
		{
			this.inner = inner;
		}

		public int Count
		{
			get
			{
				return inner.Count - index;
			}
		}

		public bool MoveNext()
		{
			if (index < inner.Count)
			{
				index++;
				return true;
			}
			return false;
		}

		public T Current
		{
			get
			{
				return inner[index];
			}
		}



	}

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

		public override VectorNode<TOut> Apply<TOut>(Func<T, TOut> transform)
		{
			var newArr = new TOut[Arr.Length];
			Arr.CopyTo(newArr, 0);
			for (int i = 0; i < newArr.Length; i++)
			{
				newArr[i] = transform(Arr[i]);
			}
			return new VectorLeaf<TOut>(newArr);
		}

		public override VectorNode<T> Drop()
		{
			var newArr = new T[Arr.Length - 1];
			Array.Copy(Arr, 0, newArr, 0, Arr.Length - 1);
			return new VectorLeaf<T>(newArr);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return new LeafEnumerator<T>(this);
		}

		public override void IterBack(Action<T> action)
		{
			action.IsNotNull();
			for (int i = Arr.Length - 1; i >= 0; i--)
			{
				action(Arr[i]);
			}
		}

		public override bool IterWhile(Func<T, bool> conditional)
		{
			for (int i = 0; i < Arr.Length; i++)
			{
				if (!conditional(Arr[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IterBackWhile(Func<T, bool> conditional)
		{
			conditional.IsNotNull();
			for (int i = Arr.Length - 1; i >= 0; i--)
			{
				if (!conditional(Arr[i]))
				{
					return false;
				}
				
			}
			return true;
		}

		public override void Iter(Action<T> action)
		{
			action.IsNotNull();
			for (int i = 0; i < Arr.Length; i++)
			{
				action(Arr[i]);
			}
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

		public override VectorNode<T> Take(int index)
		{
			int bits = index & myBlock;
			T[] newArr = Arr.TakeFirst(bits);
			return new VectorLeaf<T>(newArr);
		}
	}
}