using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Solid.Common;

namespace Solid
{
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
		{
			internal sealed class EmptyTree : FTree<TChild>

			{
				public static readonly EmptyTree Instance = new EmptyTree();

				private EmptyTree()
					: base(0, TreeType.Empty)
				{
				}

				public override bool IsFragment
				{
					get
					{
						throw Errors.Invalid_execution_path;
					}
				}

				public override TChild Left
				{
					get
					{
						throw Errors.Is_empty;
					}
				}

				public override TChild Right
				{
					get
					{
						throw Errors.Is_empty;
					}
				}

				public override FTree<TChild> AddLeft(TChild item)
				{
					return new Single(new Digit(item));
				}

				public override FTree<TChild> AddRight(TChild item)
				{
					return new Single(new Digit(item));
				}

				public override FTree<TChild> DropLeft()
				{
					throw Errors.Is_empty;
				}

				public override FTree<TChild> DropRight()
				{
					throw Errors.Is_empty;
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
						throw Errors.Is_empty;
					}
				}

				public override IEnumerator<Leaf<TValue>> GetEnumerator(bool forward)
				{
					return Enumerable.Empty<Leaf<TValue>>().GetEnumerator();
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf)
				{
					return AddRight(leaf as TChild);
				}

				public override void Iter(Action<Leaf<TValue>> action1)
				{
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> func)
				{
					return true;
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> func)
				{
					return true;
				}

				public override FTree<TChild> Remove(int index)
				{
					throw Errors.Is_empty;
				}

				public override FTree<TChild> Reverse()
				{
					return this;
				}

				public override FTree<TChild> Set(int index, Leaf<TValue> leaf)
				{
					throw Errors.Is_empty;
				}

				public override void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost)
				{
					throw Errors.Is_empty;
				}
			}
		}
	}
}