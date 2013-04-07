using System;
using System.Collections.Generic;
using Solid.Common;
using Solid.FingerTree.Iteration;

namespace Solid.FingerTree
{
	internal sealed class Empty<T> : FTree<T>
		where T : Measured<T>
	{
		public static readonly Empty<T> Instance = new Empty<T>();

		private Empty() : base(0, TreeType.Empty)
		{
		}

		public override T Left
		{
			get
			{
				throw Errors.Is_empty;
			}
		}

		public override T Right
		{
			get
			{
				throw Errors.Is_empty;
			}
		}

		public override FTree<T> AddLeft(T item)
		{
			return new Single<T>(item.Measure, new Digit<T>(item, item.Measure));
		}

		public override FTree<T> AddRight(T item)
		{
			return new Single<T>(item.Measure, new Digit<T>(item, item.Measure));
		}

		public override FTree<T> DropLeft()
		{
			throw Errors.Is_empty;
		}

		public override FTree<T> DropRight()
		{
			throw Errors.Is_empty;
		}

		public override bool IterBackWhile(Func<Measured, bool> func)
		{
			return true;
		}

		public override bool IterWhile(Func<Measured, bool> func)
		{
			return true;
		}

		public override Measured Get(int index)
		{
			throw Errors.Is_empty;
		}

		public override bool IsFragment
		{
			get
			{
				throw Errors.Invalid_execution_path;
			}
		}

		public override FTree<T> Remove(int index)
		{
			throw Errors.Is_empty;
		}

		public override IEnumerator<Measured> GetEnumerator()
		{
			return EmptyEnumerator<T>.Instance;
		}

		public override FTree<T> Insert(int index, Measured value)
		{
			return AddRight(value as T);
		}

		public override void Iter(Action<Measured> action1)
		{
		}

		public override void IterBack(Action<Measured> action)
		{
		}

		public override FTree<T> Reverse()
		{
			return this;
		}

		public override FTree<T> Set(int index, Measured value)
		{
			throw Errors.Is_empty;
		}

		public override void Split(int count, out FTree<T> leftmost, out FTree<T> rightmost)
		{
			throw Errors.Is_empty;
		}
	}
}