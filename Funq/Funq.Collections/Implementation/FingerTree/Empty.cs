using System;
using System.Collections.Generic;
using System.Linq;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
		{
			internal sealed class EmptyTree : FTree<TChild>

			{
				public static readonly EmptyTree Instance = new EmptyTree();

				private EmptyTree()
					: base(0, TreeType.Empty, Common.Lineage.Immutable)
				{
				}

				public static EmptyTree MUTABLE_Instance
				{
					get
					{
						return new EmptyTree();
					}
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
						throw Funq.Errors.Is_empty;
					}
				}

				public override bool IsFragment
				{
					get
					{
						throw ImplErrors.Invalid_execution_path;
					}
				}

				public override TChild Left
				{
					get
					{
						throw Funq.Errors.Is_empty;
					}
				}

				public override TChild Right
				{
					get
					{
						throw Funq.Errors.Is_empty;
					}
				}

				public override FTree<TChild> AddFirst(TChild item, Lineage lineage)
				{
					return new Single(new Digit(item, lineage), lineage);
				}

				public override FTree<TChild> AddLast(TChild item, Lineage lineage)
				{
					return new Single(new Digit(item, lineage), lineage);
				}

				public override FTree<TChild> DropFirst(Lineage lineage)
				{
					throw Funq.Errors.Is_empty;
				}

				public override FTree<TChild> DropLast(Lineage lineage)
				{
					throw Funq.Errors.Is_empty;
				}

				public override IEnumerator<Leaf<TValue>> GetEnumerator(bool forward)
				{
					return Enumerable.Empty<Leaf<TValue>>().GetEnumerator();
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage)
				{
					throw ImplErrors.Invalid_execution_path;
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

				public override FTree<TChild> Remove(int index, Lineage lineage)
				{
					throw Funq.Errors.Is_empty;
				}

				public override FTree<TChild> Reverse(Lineage lineage)
				{
					return this;
				}

				public override void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost, Lineage lineage)
				{
					throw Funq.Errors.Is_empty;
				}

				public override FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage)
				{
					throw Funq.Errors.Is_empty;
				}
			}
		}

		public static FTree<Leaf<TValue>> Empty = FTree<Leaf<TValue>>.Empty;
	}
}