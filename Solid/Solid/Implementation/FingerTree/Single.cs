using System;
using System.Collections.Generic;

namespace Solid.FingerTree
{
	internal sealed class Single<T> : FTree<T>
		where T : Measured<T>
	{
		public readonly Digit<T> CenterDigit;

		public Single(int measure, Digit<T> centerDigit) : base(measure, TreeType.Single)
		{
			CenterDigit = centerDigit;
		}

		public override T Left
		{
			get
			{
				return CenterDigit.Left;
			}
		}

		public override T Right
		{
			get
			{
				return CenterDigit.Right;
			}
		}

		public override FTree<T> AddLeft(T item)
		{
			if (CenterDigit.Size < 4)
			{
				return new Single<T>(Measure + item.Measure, CenterDigit.AddLeft(item));
			}
			Digit<T> leftmost;
			Digit<T> rightmost;
			CenterDigit.AddLeftSplit(item, out leftmost, out rightmost);
			return new Compound<T>(Measure + item.Measure, leftmost, Empty<Digit<T>>.Instance, rightmost);
		}

		public override FTree<T> AddRight(T item)
		{
			if (CenterDigit.Size < 4)
			{
				return new Single<T>(Measure + item.Measure, CenterDigit.AddRight(item));
			}
			Digit<T> leftmost;
			Digit<T> rightmost;
			CenterDigit.AddRightSplit(item, out leftmost, out rightmost);
			return new Compound<T>(Measure + item.Measure, leftmost, Empty<Digit<T>>.Instance, rightmost);
		}

		public override FTree<T> DropLeft()
		{
			if (CenterDigit.Size > 1)
			{
				Digit<T> newDigit = CenterDigit.PopLeft();
				return new Single<T>(newDigit.Measure, newDigit);
			}
			return Empty<T>.Instance;
		}

		public override FTree<T> DropRight()
		{
			if (CenterDigit.Size > 1)
			{
				Digit<T> newDigit = CenterDigit.PopRight();
				return new Single<T>(newDigit.Measure, newDigit);
			}
			return Empty<T>.Instance;
		}

		public override bool IterBackWhile(Func<Measured, bool> func)
		{
			return CenterDigit.IterBackWhile(func);
		}

		public override bool IterWhile(Func<Measured, bool> func)
		{
			return CenterDigit.IterWhile(func);
		}

		public override Measured Get(int index)
		{
			Measured r = CenterDigit[index];
			return r;
		}

		public override IEnumerator<Measured> GetEnumerator()
		{
			return CenterDigit.GetEnumerator();
		}

		public override FTree<T> Insert(int index, Measured value)
		{
			Digit<T> leftmost, rightmost;
			CenterDigit.Insert(index, value, out leftmost, out rightmost);
			if (rightmost == null)
			{
				return new Single<T>(Measure + 1, leftmost);
			}
			return new Compound<T>(Measure + 1, leftmost, Empty<Digit<T>>.Instance, rightmost);
		}

		public override void Iter(Action<Measured> action)
		{
			CenterDigit.Iter(action);
		}

		public override void IterBack(Action<Measured> action)
		{
			CenterDigit.IterBack(action);
		}

		public override FTree<T> Reverse()
		{
			return new Single<T>(Measure, CenterDigit.ReverseDigit());
		}

		public override FTree<T> Set(int index, Measured value)
		{
			return new Single<T>(Measure, CenterDigit.Set(index, value));
		}

		public override void Split(int count, out FTree<T> leftmost, out FTree<T> rightmost)
		{
			Digit<T> left_digit;
			Digit<T> right_digit;
			CenterDigit.Split(count, out left_digit, out right_digit);
			leftmost = left_digit != null ? (FTree<T>) new Single<T>(count, left_digit) : Empty<T>.Instance;
			rightmost = right_digit != null ? (FTree<T>) new Single<T>(Measure - count, right_digit) : Empty<T>.Instance;
		}
	}
}