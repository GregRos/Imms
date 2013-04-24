using System;
using System.Collections.Generic;
using Solid.Common;

namespace Solid
{
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
		{
			internal sealed class Single : FTree<TChild>
			{
				public readonly Digit CenterDigit;

				public Single(Digit centerDigit)
					: base(centerDigit.Measure, TreeType.Single)
				{
					CenterDigit = centerDigit;
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
						var r = CenterDigit[index];
						return r;
					}
				}

				public override bool IsFragment
				{
					get
					{
						return CenterDigit.IsFragment;
					}
				}

				public override TChild Left
				{
					get
					{
						return CenterDigit.Left;
					}
				}

				public override TChild Right
				{
					get
					{
						return CenterDigit.Right;
					}
				}

				public override FTree<TChild> MUTATES_AddLeft(TChild item)
				{
					
					if (CenterDigit.Size < 4)
					{
						this.CenterDigit.MUTATES_AddLeft(item);
						this.Measure += item.Measure;
						return this;
					}
					var leftmost = new Digit(item, CenterDigit.First);
					var rightmost = new Digit(CenterDigit.Second, CenterDigit.Third, CenterDigit.Fourth);
					return new CompoundTree(leftmost, FTree<Digit>.Empty, rightmost);
				}

				public override FTree<TChild> MUTATES_AddRight(TChild item)
				{
					if (CenterDigit.Size < 4)
					{
						this.CenterDigit.MUTATES_AddRight(item);
						this.Measure += item.Measure;
						return this;
					}
					var leftmost = new Digit(CenterDigit.First, CenterDigit.Second, CenterDigit.Third);
					var rightmost = new Digit(CenterDigit.Fourth, item);
					return new CompoundTree(leftmost, FTree<Digit>.Empty, rightmost);
				}

				public override FTree<TChild> AddLeft(TChild item)
				{
					if (CenterDigit.Size < 4)
					{
						return new Single(CenterDigit.AddLeft(item));
					}
					var leftmost = new Digit(item, CenterDigit.First);
					var rightmost = new Digit(CenterDigit.Second, CenterDigit.Third, CenterDigit.Fourth);
					return new CompoundTree(leftmost, FTree<Digit>.Empty, rightmost);
				}

				public override FTree<TChild> AddRight(TChild item)
				{
					if (CenterDigit.Size < 4)
					{
						return new Single(CenterDigit.AddRight(item));
					}
					var leftmost = new Digit(CenterDigit.First, CenterDigit.Second, CenterDigit.Third);
					var rightmost = new Digit(CenterDigit.Fourth, item);
					return new CompoundTree(leftmost, FTree<Digit>.Empty, rightmost);
				}

				public override FTree<TChild> DropLeft()
				{
					if (CenterDigit.Size > 1)
					{
						var newDigit = CenterDigit.PopLeft();
						return new Single(newDigit);
					}
					return Empty;
				}

				public override FTree<TChild> DropRight()
				{
					if (CenterDigit.Size > 1)
					{
						var newDigit = CenterDigit.PopRight();
						return new Single(newDigit);
					}
					return Empty;
				}

				public override IEnumerator<Leaf<TValue>> GetEnumerator(bool forward)
				{
					return CenterDigit.GetEnumerator(forward);
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf)
				{
					Digit leftmost, rightmost;
					CenterDigit.Insert(index, leaf, out leftmost, out rightmost);
					if (rightmost == null)
					{
						return new Single(leftmost);
					}
					return new CompoundTree(leftmost, FTree<Digit>.Empty, rightmost);
				}

				public override void Iter(Action<Leaf<TValue>> action)
				{
					CenterDigit.Iter(action);
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
					CenterDigit.IterBack(action);
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> func)
				{
					return CenterDigit.IterBackWhile(func);
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> func)
				{
					return CenterDigit.IterWhile(func);
				}

				public override FTree<TChild> Remove(int index)
				{
#if DEBUG
					CenterDigit.IsFragment.Is(false);
#endif

					var res = CenterDigit.Remove(index);
					if (res == null)
					{
						return Empty;
					}
					return new Single(res);
				}

				public override FTree<TChild> Reverse()
				{
					return new Single(CenterDigit.Reverse());
				}

				public override FTree<TChild> Set(int index, Leaf<TValue> leaf)
				{
					return new Single(CenterDigit.Set(index, leaf));
				}

				public override void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost)
				{
					Digit left_digit;
					Digit right_digit;
					CenterDigit.Split(count, out left_digit, out right_digit);
					leftmost = left_digit != null ? new Single(left_digit) : Empty;
					rightmost = right_digit != null ? new Single(right_digit) : Empty;
				}
			}
		}
	}
}