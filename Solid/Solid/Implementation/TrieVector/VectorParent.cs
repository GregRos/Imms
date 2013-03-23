using System;
using System.Collections.Generic;
using NUnit.Framework;
using Solid.Common;
using Solid.TrieVector.Iteration;

namespace Solid.TrieVector
{
	
	internal sealed class VectorParent<T> : VectorNode<T>
	{
		public readonly VectorNode<T>[] Arr;

		private readonly int myBlock;
		private readonly int offs;

		public VectorParent(int height, int count, VectorNode<T>[] arr)
			: base(height, count, arr.Length == 32 && arr[arr.Length - 1].IsFull)
		{
			arr.IsNotNull();

			Arr = arr;
			offs = height*5;
			myBlock = ((1 << 5) - 1) << offs;
		}

		public override T this[int index]
		{
			get
			{
				int myIndex = index & myBlock;
				myIndex = myIndex >> offs;
				return Arr[myIndex][index];
			}
		}

		public override VectorNode<T> Add(T item)
		{
			VectorNode<T>[] newMyArr;
			if (!Arr[Arr.Length - 1].IsFull || Arr[Arr.Length - 1].Height < Height - 1)
			{
				VectorNode<T> newNode = Arr[Arr.Length - 1].Add(item);
				var myCopy = new VectorNode<T>[Arr.Length];
				Arr.CopyTo(myCopy, 0);
				myCopy[Arr.Length - 1] = newNode;
				newMyArr = myCopy;
				return new VectorParent<T>(Height, Count + 1, newMyArr);
			}
			if (Arr.Length != 32)
			{
				var newArr = new T[1];
				newArr[0] = item;
				var newNode = new VectorLeaf<T>(newArr);
				var myCopy = new VectorNode<T>[Arr.Length + 1];
				Arr.CopyTo(myCopy, 0);
				myCopy[myCopy.Length - 1] = newNode;
				newMyArr = myCopy;
				return new VectorParent<T>(Height, Count + 1, newMyArr);
			}
			newMyArr = new VectorNode<T>[2];
			var newArrLeaf = new T[1];
			newArrLeaf[0] = item;
			newMyArr[0] = this;

			newMyArr[1] = new VectorLeaf<T>(newArrLeaf);
			return new VectorParent<T>(Height + 1, Count + 1, newMyArr);
		}

		public override VectorNode<T> Drop()
		{
			if (Arr.Length == 2 && Arr[1].Count == 1)
			{
				return Arr[0];
			}
			VectorNode<T>[] newMyArr;
			if (Arr[Arr.Length - 1].Count == 1)
			{
				newMyArr = Arr.Remove();
				return new VectorParent<T>(Height, Count - 1, newMyArr);
			}
			VectorNode<T> newLast = Arr[Arr.Length - 1].Drop();
			var myCopy = new VectorNode<T>[Arr.Length];
			Arr.CopyTo(myCopy, 0);
			myCopy[Arr.Length - 1] = newLast;
			newMyArr = myCopy;
			return new VectorParent<T>(Height, Count - 1, newMyArr);
		}


		public override VectorNode<TOut> Apply<TOut>(Func<T, TOut> transform)
		{
			var newArr = new VectorNode<TOut>[Arr.Length];
			for (int i = 0; i < newArr.Length; i++)
			{
				newArr[i] = Arr[i].Apply(transform);
			}
			return new VectorParent<TOut>(Height, Count, newArr);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return new ParentEnumerator<T>(this);
		}

		public override void IterBack(Action<T> action)
		{
			for (int i = Arr.Length - 1; i >= 0; i--)
			{
				Arr[i].IterBack(action);
			}
		}

		public override bool IterWhile(Func<T, bool> conditional)
		{
			for (int i = 0; i < Arr.Length; i++)
			{
				if (!Arr[i].IterWhile(conditional))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IterBackWhile(Func<T, bool> conditional)
		{
			for (int i = Arr.Length - 1; i >= 0; i--)
			{
				if (!Arr[i].IterBackWhile(conditional))
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
				Arr[i].Iter(action);
			}
		}

		public override VectorNode<T> Set(int index, T value)
		{
			int myIndex = (index) & myBlock;
			myIndex = myIndex >> offs;
			myIndex.Is(i => i < Count && i >= 0);
			VectorNode<T> myNewNode = Arr[myIndex].Set(index, value);
			var myCopy = new VectorNode<T>[Arr.Length];
			Arr.CopyTo(myCopy, 0);
			myCopy[myIndex] = myNewNode;
			VectorNode<T>[] myNewArr = myCopy;
			return new VectorParent<T>(Height, Count, myNewArr);
		}

		public override VectorNode<T> Take(int index)
		{
			index.Is(i => i < Count && i > 0);
			int myCount = index & myBlock;
			myCount = myCount >> (offs);
			VectorNode<T>[] myArrFirst = Arr.TakeFirst(myCount + 1);
			VectorNode<T> myNewLast = Arr[myCount].Take(index);
			myArrFirst[myCount] = myNewLast;
			return new VectorParent<T>(Height, Arr[0].Count*(myCount - 1) + myNewLast.Count, myArrFirst);
		}
	}
}