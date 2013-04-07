using System;
using System.Collections.Generic;
using NUnit.Framework;
using Solid.FingerTree.Iteration;

namespace Solid.FingerTree
{
	internal class Value<T> : Measured<Value<T>>
	{
		public readonly T Content;

		public Value(T content) : base(1)
		{
			Content = content;
		}

		public override Measured this[int index]
		{
			get
			{
				return this;
			}
		}

		public override bool IterBackWhile(Func<Measured, bool> action)
		{
			return action(this);
		}

		public override bool IterWhile(Func<Measured, bool> action)
		{
			return action(this);
		}

		public override IEnumerator<Measured> GetEnumerator()
		{
			return new ValueEnumerator<T>(this);
		}

		public override void Insert(int index, Measured value, out Value<T> leftmost, out Value<T> rightmost)
		{
			value.IsInstanceOf<Value<T>>();
			leftmost = value as Value<T>;
			rightmost = this;
		}


		public override void Iter(Action<Measured> action)
		{
			action(this);
		}

		public override void IterBack(Action<Measured> action)
		{
			action(this);
		}

		public override bool IsFragment
		{
			get
			{
				return false;
			}
		}

		public override Value<T> Reverse()
		{
			return this;
		}

		public override Value<T> Remove(int index)
		{
			index.Is(0);
			return null;
		}


		public override Value<T> Set(int index, Measured value)
		{
			value.IsInstanceOf<Value<T>>();
			return value as Value<T>;
		}

		public override void Split(int index, out Value<T> leftmost, out Value<T> rightmost)
		{
			leftmost = null;
			rightmost = null;
		}

		public override void Fuse(Value<T> after, out Value<T> firstRes, out Value<T> lastRes)
		{
			throw new NotImplementedException();
		}
	}
}