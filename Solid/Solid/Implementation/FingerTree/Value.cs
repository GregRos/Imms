using System;
using System.Collections.Generic;
using NUnit.Framework;
using Solid.FingerTree.Iteration;

namespace Solid.FingerTree
{
	internal class Value<T> : Measured
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

		public override U Reverse<U>()
		{
			return this as U;
		}

		public override void IterBack(Action<Measured> action)
		{
			action(this);
		}

		public override IEnumerator<Measured> GetEnumerator()
		{
			return new ValueEnumerator<T>(this);
		}


		public override void Split<TObject>(int index, out TObject leftmost, out TObject rightmost)
		{
			leftmost = null;
			rightmost = null;
		}

		public override void Iter(Action<Measured> action)
		{
			action(this);
		}

		public override TObject Set<TObject>(int index, object value)
		{
			value.IsInstanceOf<Value<T>>();
			return value as TObject;
		}

		public override void Insert<TObject>(int index, object value, out TObject leftmost, out TObject rightmost)
		{
			value.IsInstanceOf<Value<T>>();
			leftmost = value as TObject;
			rightmost = this as TObject;
		}
	}
}