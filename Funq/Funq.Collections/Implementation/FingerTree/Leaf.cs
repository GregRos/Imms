using System;
using System.Collections;
using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	internal class Leaf<TValue> : FingerTree<TValue>.Measured<Leaf<TValue>>,IEquatable<Leaf<TValue>>
	{
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override WeaklyTypedElement GetGrouping(int index) {
			throw ImplErrors.Invalid_null_invocation;
		}

		public override bool HasValue {
			get { return true; }
		}

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
#if ASSERTS
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

		public static implicit operator TValue (Leaf<TValue> leaf)
		{
			return leaf.Value;
		}

		public static implicit operator Leaf<TValue> (TValue v)
		{
			return new Leaf<TValue>(v);
		}

		public Leaf(TValue value)
			: base(1, Lineage.Immutable, 0)
		{
			Value = value;
		}

		public bool Equals(Leaf<TValue> other)
		{
			return Value.Equals(other.Value);
		}

		public override bool Equals(object obj)
		{
			return obj is Leaf<TValue> && Equals(obj as Leaf<TValue>);
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

		public override void Fuse(Leaf<TValue> after, out Leaf<TValue> firstRes, out Leaf<TValue> lastRes, Lineage lineage)
		{
			throw new NotImplementedException();
		}

		public override FingerTree<TValue>.IReusableEnumerator<Leaf<TValue>> GetEnumerator(bool x)
		{
			return new LeafEnumerator(this);
		}

		public override void Insert(int index, Leaf<TValue> leaf, out Leaf<TValue> leftmost, out Leaf<TValue> rightmost, Lineage lineage)
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

		public override Leaf<TValue> Remove(int index, Lineage lineage)
		{
#if ASSERTS
			index.Is(0);
#endif
			return null;
		}

		public override Leaf<TValue> Reverse(Lineage lineage)
		{
			return this;
		}

		public override void Split(int index, out Leaf<TValue> leftmost, out Leaf<TValue> rightmost, Lineage lineage)
		{
			leftmost = null;
			rightmost = null;
		}

		public override Leaf<TValue> Update(int index, Leaf<TValue> leaf, Lineage lineage)
		{
			return leaf;
		}
	}
}