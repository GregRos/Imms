using System;
using System.Collections;
using System.Collections.Generic;
using Solid.Common;

namespace Solid
{
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
			where TChild : Measured<TChild>
		{
			internal sealed class CompoundTree : FTree<TChild>
			{
				internal class CompoundEnumerator : IEnumerator<Leaf<TValue>>
				{
					private readonly bool _forward;
					private int index = -1;
					private IEnumerator<Leaf<TValue>> inner;
					private readonly CompoundTree tree;

					public CompoundEnumerator(CompoundTree tree, bool forward)
					{
						this.tree = tree;
						_forward = forward;
					}

					public Leaf<TValue> Current
					{
						get
						{
							return inner.Current;
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
						if (index != -1 && inner.MoveNext())
							return true;
						index++;
						var fixedNum = _forward ? index : 2 - index;
						switch (fixedNum)
						{
							case 0:
								inner = tree.LeftDigit.GetEnumerator(_forward);
								return MoveNext();
							case 1:
								inner = tree.DeepTree.GetEnumerator(_forward);
								return MoveNext();
							case 2:
								inner = tree.RightDigit.GetEnumerator(_forward);
								return MoveNext();
							default:
								return false;
						}
					}

					public void Reset()
					{
						throw new NotSupportedException();
					}
				}

				private const int
					IN_END = 6;

				private const int
					IN_MIDDLE_OF_DEEP = 3;

				private const int
					IN_MIDDLE_OF_LEFT = 1;

				private const int
					IN_MIDDLE_OF_RIGHT = 5;

				private const int
					IN_START = 0;

				private const int
					IN_START_OF_DEEP = 2;

				private const int
					IN_START_OF_RIGHT = 4;

				private const int
					OUTSIDE = 7;

				public readonly FTree<Digit> DeepTree;
				public readonly Digit LeftDigit;
				public readonly Digit RightDigit;

				public CompoundTree(Digit leftDigit, FTree<Digit> deepTree, Digit rightDigit)
					: base(leftDigit.Measure + deepTree.Measure + rightDigit.Measure, TreeType.Compound)
				{
#if DEBUG
					leftDigit.IsNotNull();
					deepTree.IsNotNull();
					rightDigit.IsNotNull();
#endif
					DeepTree = deepTree;
					RightDigit = rightDigit;
					LeftDigit = leftDigit;
				}

				private static FTree<TChild> CreateCheckNull(Digit left = null, FTree<Digit> deep = null,
				                                             Digit right = null)
				{
					var memberPermutation = left != null ? 1 << 0 : 0;
					memberPermutation |= (deep != null && deep.Measure != 0) ? 1 << 1 : 0;
					memberPermutation |= right != null ? 1 << 2 : 0;

					switch (memberPermutation)
					{
						case 0:
							return Empty;
						case 1 << 0:
							return new Single(left);
						case 1 << 0 | 1 << 1:
							var deep_1 = deep.DropRight();
							var r_2 = deep.Right;
							return new CompoundTree(left, deep.DropRight(), deep.Right);
						case 1 << 0 | 1 << 1 | 1 << 2:
							return new CompoundTree(left, deep, right);
						case 1 << 1 | 1 << 2:
							return new CompoundTree(deep.Left, deep.DropLeft(), right);
						case 1 << 0 | 1 << 2:
							return new CompoundTree(left, deep, right);
						case 1 << 1:
							left = deep.Left;
							deep = deep.DropLeft();
							if (deep.Measure != 0)
							{
								right = deep.Right;
								deep = deep.DropRight();
								return new CompoundTree(left, deep, right);
							}
							return new Single(left);
						case 1 << 2:
							return new Single(right);
						default:
							throw Errors.Invalid_execution_path;
					}
				}

				public override bool IsFragment
				{
					get
					{
						return false;
					}
				}
				public override TChild Left
				{
					get
					{
						return LeftDigit.Left;
					}
				}

				public override TChild Right
				{
					get
					{
						return RightDigit.Right;
					}
				}

				public override FTree<TChild> AddLeft(TChild item)
				{
					if (LeftDigit.Size < 4)
					{
						return new CompoundTree(LeftDigit.AddLeft(item), DeepTree, RightDigit);
					}
					Digit leftmost;
					Digit rightmost;
					LeftDigit.AddLeftSplit(item, out leftmost, out rightmost);
					var newDeep = DeepTree.AddLeft(rightmost);
					return new CompoundTree(leftmost, newDeep, RightDigit);
				}

				public override FTree<TChild> AddRight(TChild item)
				{
					if (RightDigit.Size < 4)
					{
						return new CompoundTree(LeftDigit, DeepTree, RightDigit.AddRight(item));
					}

					Digit leftmost;
					Digit rightmost;
					RightDigit.AddRightSplit(item, out leftmost, out rightmost);
					var newDeep = DeepTree.AddRight(leftmost);
					return new CompoundTree(LeftDigit, newDeep, rightmost);
				}

				public override FTree<TChild> DropLeft()
				{
					if (LeftDigit.Size > 1)
					{
						var new_left = LeftDigit.PopLeft();
						var new_measure = Measure - LeftDigit.Left.Measure;
						return new CompoundTree(new_left, DeepTree, RightDigit);
					}
					if (DeepTree.Measure > 0)
					{
						var new_left = DeepTree.Left;
						var new_deep = DeepTree.DropLeft();
						var new_measure = Measure - LeftDigit.Measure;
						return new CompoundTree(new_left, new_deep, RightDigit);
					}
					return new Single(RightDigit);
				}

				public override FTree<TChild> DropRight()
				{
					if (RightDigit.Size > 1)
					{
						var new_right = RightDigit.PopRight();
						var new_measure = Measure - RightDigit.Right.Measure;
						return new CompoundTree(LeftDigit, DeepTree, new_right);
					}
					if (DeepTree.Measure > 0)
					{
						var new_right = DeepTree.Right;
						var new_deep = DeepTree.DropRight();
						var new_measure = Measure - RightDigit.Measure;
						return new CompoundTree(LeftDigit, new_deep, new_right);
					}
					return new Single(LeftDigit);
				}

				private FTree<TChild> FixLeftDigit()
				{
					if (!LeftDigit.IsFragment)
					{
						return this;
					}
					if (DeepTree.Measure == 0)
					{
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last);
						return CreateCheckNull(first, DeepTree, last);
					}
					else
					{
						var fromDeep = DeepTree.Left;
						var newDeep = DeepTree.DropLeft();
						Digit first, last;
						LeftDigit.Fuse(fromDeep, out first, out last);
						if (last == null)
						{
							return new CompoundTree(first, newDeep, RightDigit);
						}
						return new CompoundTree(first, newDeep.AddLeft(last), RightDigit);
					}
				}

				private FTree<TChild> FixRightDigit()
				{
					if (!RightDigit.IsFragment)
					{
						return this;
					}
					if (DeepTree.Measure == 0)
					{
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last);
						return CreateCheckNull(first, DeepTree, last);
					}
					else
					{
						var fromDeep = DeepTree.Right;
						var newDeep = DeepTree.DropRight();
						Digit first, last;
						fromDeep.Fuse(RightDigit, out first, out last);
						if (last == null)
						{
							return new CompoundTree(LeftDigit, newDeep, first);
						}
						return new CompoundTree(first, newDeep.AddLeft(first), last);
					}
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
#if DEBUG
						index.Is(i => i < Measure);
#endif
						var m1 = LeftDigit.Measure;
						var m2 = DeepTree.Measure + m1;

						if (index < m1)
							return LeftDigit[index];
						if (index < m2)
							return DeepTree[index - m1];
						if (index < Measure)
							return RightDigit[index - m2];

						throw Errors.Arg_out_of_range("index");
					}
				}

				public override IEnumerator<Leaf<TValue>> GetEnumerator(bool forward)
				{
					return new CompoundEnumerator(this, forward);
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
					int new_measure = Measure + 1;
					FTree<Digit> new_deep;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							Digit left_l, left_r;
							LeftDigit.Insert(index, leaf, out left_l, out left_r);
							new_deep = left_r != null ? DeepTree.AddLeft(left_r) : DeepTree;
							return new CompoundTree(left_l, new_deep, RightDigit);
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							new_deep = DeepTree.Insert(index - LeftDigit.Measure, leaf);
							return new CompoundTree(LeftDigit, new_deep, RightDigit);
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							Digit right_l, right_r;
							RightDigit.Insert(index - LeftDigit.Measure - DeepTree.Measure, leaf, out right_l, out right_r);
							new_deep = right_r != null ? DeepTree.AddRight(right_l) : DeepTree;
							right_r = right_r ?? right_l;
							return new CompoundTree(RightDigit, new_deep, right_r);
					}
					throw Errors.Invalid_execution_path;
				}

				public override void Iter(Action<Leaf<TValue>> action)
				{
#if DEBUG
					action.IsNotNull();
#endif
					LeftDigit.Iter(action);
					DeepTree.Iter(action);
					RightDigit.Iter(action);
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
#if DEBUG
					action.IsNotNull();
#endif
					RightDigit.IterBack(action);
					DeepTree.IterBack(action);
					LeftDigit.IterBack(action);
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> action)
				{
					if (!RightDigit.IterBackWhile(action)) return false;
					if (!DeepTree.IterBackWhile(action)) return false;
					if (!LeftDigit.IterBackWhile(action)) return false;
					return true;
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> action)
				{
					if (!LeftDigit.IterWhile(action)) return false;
					if (!DeepTree.IterWhile(action)) return false;
					if (!RightDigit.IterWhile(action)) return false;
					return true;
				}

				public override FTree<TChild> Remove(int index)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
					Digit newLeft;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							if (LeftDigit.IsFragment)
							{
								var fixedTree = FixLeftDigit();
								return fixedTree.Remove(index);
							}
							newLeft = LeftDigit.Remove(index);
							return CreateCheckNull(newLeft, DeepTree, RightDigit);
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							var deep = DeepTree;
							FTree<Digit> newDeep;
							if (deep.IsFragment)
							{
								newDeep = deep.AddLeft(LeftDigit);
								newDeep = newDeep.Remove(index);
								newLeft = newDeep.Left;
								newDeep = newDeep.DropLeft();
								return new CompoundTree(newLeft, newDeep, RightDigit);
							}
							newDeep = DeepTree.Remove(index - LeftDigit.Measure);
							return CreateCheckNull(LeftDigit, newDeep, RightDigit);
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							if (RightDigit.IsFragment)
							{
								var fixedTree = FixRightDigit();
								return fixedTree.Remove(index);
							}
							var newRight = RightDigit.Remove(index - LeftDigit.Measure - DeepTree.Measure);
							return CreateCheckNull(LeftDigit, DeepTree, newRight);
						default:
							throw Errors.Invalid_execution_path;
					}
					
				}

				public override FTree<TChild> Reverse()
				{
					var first = LeftDigit.Reverse();
					var deep = DeepTree.Reverse();
					var last = RightDigit.Reverse();
					return new CompoundTree(last, deep, first);
				}

				public override FTree<TChild> Set(int index, Leaf<TValue> leaf)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							var new_left = LeftDigit.Set(index, leaf);
							return new CompoundTree(new_left, DeepTree, RightDigit);
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							var new_deep = DeepTree.Set(index - LeftDigit.Measure, leaf);
							return new CompoundTree(LeftDigit, new_deep, RightDigit);
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							var new_right = RightDigit.Set(index - LeftDigit.Measure - DeepTree.Measure, leaf);
							return new CompoundTree(LeftDigit, DeepTree, new_right);
					}
					throw Errors.Arg_out_of_range("index");
				}

				public override void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost)
				{
					var whereIsThisIndex = WhereIsThisIndex(count);
#if DEBUG
					count.Is(i => i < Measure && i >= 0);
#endif

					switch (whereIsThisIndex)
					{
						case IN_START:
							leftmost = Empty;
							rightmost = this;
							return;
						case IN_MIDDLE_OF_LEFT:
							Digit left_1, left_2;
							LeftDigit.Split(count, out left_1, out left_2);
							leftmost = left_1 != null ? new Single(left_1) : Empty;
							rightmost = CreateCheckNull(left_2, DeepTree, RightDigit);
							return;
						case IN_START_OF_DEEP:
							leftmost = new Single(LeftDigit);
							rightmost = CreateCheckNull(null, DeepTree, RightDigit);
							return;
						case IN_MIDDLE_OF_DEEP:
							FTree<Digit> tree_1, tree_2;
							DeepTree.Split(count - LeftDigit.Measure, out tree_1, out tree_2);
							leftmost = CreateCheckNull(LeftDigit, tree_1);
							rightmost = CreateCheckNull(null, tree_2, RightDigit);
							return;
						case IN_START_OF_RIGHT:
							leftmost = CreateCheckNull(LeftDigit, DeepTree);
							rightmost = new Single(RightDigit);
							return;
						case IN_MIDDLE_OF_RIGHT:
							Digit right_1, right_2;
							RightDigit.Split(count - LeftDigit.Measure - DeepTree.Measure, out right_1, out right_2);
							leftmost = CreateCheckNull(LeftDigit, DeepTree, right_1);
							rightmost = CreateCheckNull(right_2);
							return;
						case IN_END:
							leftmost = this;
							rightmost = Empty;
							return;
					}
					throw Errors.Invalid_execution_path;
				}

				/// <summary>
				///   Returns 0 if index is 0 (meaning, empty).
				///   Returns 1 if index is in left digit
				///   Returns 2 if index encompasses the left digit.
				///   Returns 3 if he index is in the deep tree.
				///   Returns 4 if the index encompasses the left digit + deep tree
				///   Returns 5 if the index is in the right digit.
				///   Returns 6 if the index encompasses the entire tree.
				///   Returns 7 if the index is outside the tree.
				/// </summary>
				/// <param name="index"> </param>
				/// <returns> </returns>
				private int WhereIsThisIndex(int index)
				{
					if (index == 0) return IN_START;
					if (index < LeftDigit.Measure) return IN_MIDDLE_OF_LEFT;
					if (index == LeftDigit.Measure) return IN_START_OF_DEEP;
					if (index < LeftDigit.Measure + DeepTree.Measure) return IN_MIDDLE_OF_DEEP;
					if (index == LeftDigit.Measure + DeepTree.Measure) return IN_START_OF_RIGHT;
					if (index < Measure) return IN_MIDDLE_OF_RIGHT;
					if (index == Measure) return IN_END;
					return OUTSIDE;
				}

				//+ Implementation
				//  This function creates an FTree<TChild> of the right type, depending on which digits are null.
				//  The tree cannot be null, but can be empty.
			}
		}
	}
}