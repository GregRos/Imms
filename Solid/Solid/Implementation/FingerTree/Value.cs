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
			this.Content = content;
		}

		public override Measured this[int index]
		{
			get
			{
				return this;
			}
		}

		public override Value<T> Reverse()
		{
			return this;
		}

		public override void IterBack(Action<Measured> action)
		{
			action(this);
		}

		public override IEnumerator<Measured> GetEnumerator()
		{
			return new ValueEnumerator<T>(this);
		}


		public override void Split(int index, out Value<T> leftmost, out Value<T> rightmost)
		{
			leftmost = null;
			rightmost = null;
		}

		public override void Iter(Action<Measured> action)
		{
			action(this);
		}

		public override Value<T> Set(int index, Measured value)
		{
			value.IsInstanceOf<Value<T>>();
			return value as Value<T>;
		}

		public override void Insert(int index, object value, out Value<T> leftmost, out Value<T> rightmost)
		{
			value.IsInstanceOf<Value<T>>();
			leftmost = value as Value<T>;
			rightmost = this;
		}
	}
}