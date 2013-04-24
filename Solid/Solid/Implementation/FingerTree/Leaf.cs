using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Solid.Common;
namespace Solid
{
	internal class Leaf<TValue> : FingerTree<TValue>.Measured<Leaf<TValue>>
	{
		private class LeafEnumerator : FingerTree<TValue>.IReusableEnumerator<Leaf<TValue>>
		{
			private Leaf<TValue> _inner;
			private bool done = false;

			public LeafEnumerator(Leaf<TValue> inner)
			{
				_inner = inner;
			}

			public Leaf<TValue> Current
			{
				get
				{
#if DEBUG
					_inner.IsNotNull();
#endif
					return _inner;
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
				if (done) return false;
				done = true;
				return true;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public void Retarget(Leaf<TValue> next)
			{
				_inner = next;
				done = false;
			}
		}

		public readonly TValue Value;

		public Leaf(TValue value)
			: base(1)
		{
			Value = value;
		}

		public override Leaf<TValue> this[int index]
		{
			get
			{
				return this;
			}
		}

		public override bool IsFragment
		{
			get
			{
				return false;
			}
		}

		public override void Fuse(Leaf<TValue> after, out Leaf<TValue> firstRes, out Leaf<TValue> lastRes)
		{
			throw new NotImplementedException();
		}

		public override FingerTree<TValue>.IReusableEnumerator<Leaf<TValue>> GetEnumerator(bool x)
		{
			return new LeafEnumerator(this);
		}

		public override void Insert(int index, Leaf<TValue> leaf, out Leaf<TValue> leftmost, out Leaf<TValue> rightmost)
		{
			leftmost = leaf;
			rightmost = this;
		}

		public override void Iter(Action<Leaf<TValue>> action)
		{
			action(this);
		}

		public override void IterBack(Action<Leaf<TValue>> action)
		{
			action(this);
		}

		public override bool IterBackWhile(Func<Leaf<TValue>, bool> action)
		{
			return action(this);
		}

		public override bool IterWhile(Func<Leaf<TValue>, bool> action)
		{
			return action(this);
		}

		public override Leaf<TValue> Remove(int index)
		{
#if DEBUG
			index.Is(0);
#endif
			return null;
		}

		public override Leaf<TValue> Reverse()
		{
			return this;
		}

		public override Leaf<TValue> Set(int index, Leaf<TValue> leaf)
		{
			return leaf;
		}

		public override void Split(int index, out Leaf<TValue> leftmost, out Leaf<TValue> rightmost)
		{
			leftmost = null;
			rightmost = null;
		}
	}
}