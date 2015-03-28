using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
			where TChild : Measured<TChild>
		{
			internal sealed class Compound : FTree<TChild>
			{
				internal class Enumerator : IEnumerator<Leaf<TValue>>
				{
					private readonly bool _forward;
					private int index = -1;
					private IEnumerator<Leaf<TValue>> inner;
					private readonly Compound tree;

					public Enumerator(Compound tree, bool forward)
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

				public FTree<Digit> DeepTree;
				public Digit LeftDigit;
				public Digit RightDigit;

				public Compound(Digit leftDigit, FTree<Digit> deepTree, Digit rightDigit, Lineage lineage)
					: base(leftDigit.Measure + deepTree.Measure + rightDigit.Measure, TreeType.Compound, lineage, 3)
				{
#if ASSERTS2
					leftDigit.IsNotNull();
					deepTree.IsNotNull();
					rightDigit.IsNotNull();
#endif
					_mutate(leftDigit, deepTree, rightDigit);
				}

				private FTree<TChild> CreateCheckNull(Lineage lineage, Digit left = null, FTree<Digit> deep = null, Digit right = null)
				{
					var memberPermutation = left != null ? 1 << 0 : 0;
					memberPermutation |= (deep != null && deep.Measure != 0) ? 1 << 1 : 0;
					memberPermutation |= right != null ? 1 << 2 : 0;

					switch (memberPermutation)
					{
						case 0:
							return Empty;
						case 1 << 0:
							return new Single(left, lineage);
						case 1 << 0 | 1 << 1:
							var deep_1 = deep.DropLast(lineage);
							var r_2 = deep.Right;
							return MutateOrCreate(left, deep.DropLast(lineage), deep.Right, lineage);
						case 1 << 0 | 1 << 1 | 1 << 2:
							return MutateOrCreate(left, deep, right, lineage);
						case 1 << 1 | 1 << 2:
							return MutateOrCreate(deep.Left, deep.DropFirst(lineage), right, lineage);
						case 1 << 0 | 1 << 2:
							return MutateOrCreate(left, deep, right, lineage);
						case 1 << 1:
							left = deep.Left;
							deep = deep.DropFirst(lineage);
							if (deep.Measure != 0)
							{
								right = deep.Right;
								deep = deep.DropLast(lineage);
								return MutateOrCreate(left, deep, right, lineage);
							}
							return new Single(left, lineage);
						case 1 << 2:
							return new Single(right, lineage);
						default:
							throw ImplErrors.Invalid_execution_path;
					}
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
#if ASSERTS
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

						throw Funq.Errors.Arg_out_of_range("index", index);
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
				/// <summary>
				/// This method will re-initialize this instance using the specified parameters. 
				/// </summary>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public FTree<TChild> _mutate(Digit left, FTree<Digit> deep, Digit right)
				{
					LeftDigit = left;
					DeepTree = deep;
					RightDigit = right;
					Measure = left.Measure + deep.Measure + right.Measure;
					return this;
				}
				/// <summary>
				///<para>This method can mutate the current instance and return it, or return a new instance, based on the supplied Lineage.</para>
				///<para>If the current Lineage allows mutation from the specified Lineage, the instance will be MUTATED and returned.</para>
				///<para>Otherwise, the method will return a NEW instance that is a member of the supplied Lineage. </para>
				/// </summary>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public FTree<TChild> MutateOrCreate(Digit left, FTree<Digit> deep, Digit right, Lineage lineage)
				{
					if (Lineage.AllowMutation(lineage))
					{
						LeftDigit = left;
						DeepTree = deep;
						RightDigit = right;
						Measure = left.Measure + deep.Measure + right.Measure;
						return this;
					}
					else
					{
						return new Compound(left, deep, right, lineage);
					}
				}

				public override FTree<TChild> AddFirst(TChild item, Lineage lineage)
				{
					FTree<TChild> ret;
#if ASSERTS
					var expected_size = Measure + item.Measure;
#endif
					if (LeftDigit.Size < 4)
					{
						ret = MutateOrCreate(LeftDigit.AddFirst(item, lineage), DeepTree, RightDigit, lineage);
					}
					else
					{
						var leftmost = new Digit(item, LeftDigit.First, lineage);
						var rightmost = LeftDigit.DropFirst(lineage);
						var newDeep = DeepTree.AddFirst(rightmost, lineage);
						ret =  MutateOrCreate(leftmost, newDeep, RightDigit, lineage);
					}
#if ASSERTS
					ret.Measure.Is(expected_size);
					ret.Left.Is(item);
#endif
					return ret;

				}


				public override FTree<TChild> AddLast(TChild item, Lineage lineage)
				{
#if ASSERTS
					var expected_size = Measure + item.Measure;
#endif
					FTree<TChild> ret;
					if (RightDigit.Size < 4)
					{
						ret =  MutateOrCreate(LeftDigit, DeepTree, RightDigit.AddLast(item, lineage), lineage);
					}
					else
					{
						var rightmost = new Digit(RightDigit.Fourth, item, lineage);
						var leftmost = RightDigit.DropLast(lineage);
						var newDeep = DeepTree.AddLast(leftmost, lineage);
						ret =  MutateOrCreate(LeftDigit, newDeep, rightmost, lineage);
					}
#if ASSERTS
					ret.Measure.Is(expected_size);
					ret.Right.Is(item);
#endif
					return ret;

				}


				public override FTree<TChild> DropFirst(Lineage lineage)
				{
					FTree<TChild> ret;
#if ASSERTS
					var expected = Measure - Left.Measure;
#endif

					if (LeftDigit.Size > 1)
					{
						var new_left = LeftDigit.DropFirst(lineage);
						ret =  MutateOrCreate(new_left, DeepTree, RightDigit, lineage);
					}
					else if (DeepTree.Measure > 0)
					{
						var new_left = DeepTree.Left;
						var new_deep = DeepTree.DropFirst(lineage);
						ret =  MutateOrCreate(new_left, new_deep, RightDigit, lineage);
					}
					else ret =  new Single(RightDigit, lineage);
#if ASSERTS
					ret.Measure.Is(expected);
#endif
					return ret;
				}


				public override FTree<TChild> DropLast(Lineage lineage)
				{
					FTree<TChild> ret;
#if ASSERTS
					var expected_size = Measure - Right.Measure;
#endif
					if (RightDigit.Size > 1)
					{
						var new_right = RightDigit.DropLast(lineage);
						ret= MutateOrCreate(LeftDigit, DeepTree, new_right, lineage);
					}
					else if (DeepTree.Measure > 0)
					{
						var new_right = DeepTree.Right;
						var new_deep = DeepTree.DropLast(lineage);
						ret = MutateOrCreate(LeftDigit, new_deep, new_right, lineage);
					}
					else
					{
						ret = new Single(LeftDigit, lineage);
					}
#if ASSERTS
					ret.Measure.Is(expected_size);
#endif
					return ret;
				}

				private FTree<TChild> FixLeftDigit(Lineage lineage)
				{
					if (!LeftDigit.IsFragment)
					{
						return this;
					}
					if (DeepTree.Measure == 0)
					{
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last, lineage);
						return CreateCheckNull(lineage, first, DeepTree, last);
					}
					else
					{
						var fromDeep = DeepTree.Left;
						var newDeep = DeepTree.DropFirst(lineage);
						Digit first, last;
						LeftDigit.Fuse(fromDeep, out first, out last, lineage);
						if (last == null)
						{
							return new Compound(first, newDeep, RightDigit, lineage);
						}
						return new Compound(first, newDeep.AddFirst(last, lineage), RightDigit, lineage);
					}
				}

				private FTree<TChild> FixRightDigit(Lineage lineage)
				{
					if (!RightDigit.IsFragment)
					{
						return this;
					}
					FTree<TChild> ret;
					if (DeepTree.Measure == 0)
					{
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last, lineage);
						ret = CreateCheckNull(lineage, first, DeepTree, last);
						return ret;
					}
					else
					{
						var fromDeep = DeepTree.Right;
						var newDeep = DeepTree.DropLast(lineage);
						Digit first, last;
						fromDeep.Fuse(RightDigit, out first, out last, lineage);
						if (last == null)
						{
							return new Compound(LeftDigit, newDeep, first, lineage);
						}
						ret = new Compound(LeftDigit, newDeep.AddFirst(first, lineage), last, lineage);
					}
#if ASSERTS
					ret.Measure.Is(this.Measure);
#endif
					return ret;
				}

				public override IEnumerator<Leaf<TValue>> GetEnumerator(bool forward)
				{
					return new Enumerator(this, forward);
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
#if ASSERTS
					var new_measure = Measure + 1;
					var old_value = this[index].Value;
#endif
					FTree<Digit> new_deep;
					FTree<TChild> res = null;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							Digit left_l, left_r;
							LeftDigit.Insert(index, leaf, out left_l, out left_r, lineage);
							new_deep = left_r != null ? DeepTree.AddFirst(left_r, lineage) : DeepTree;
							res = MutateOrCreate(left_l, new_deep, RightDigit, lineage);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							new_deep = DeepTree.Insert(index - LeftDigit.Measure, leaf, lineage);
							res = MutateOrCreate(LeftDigit, new_deep, RightDigit, lineage);
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							Digit right_r;
							Digit right_l;
							RightDigit.Insert(index - LeftDigit.Measure - DeepTree.Measure, leaf, out right_l, out right_r, lineage);
							new_deep = right_r != null ? DeepTree.AddLast(right_l, lineage) : DeepTree;
							right_r = right_r ?? right_l;
							res = MutateOrCreate(LeftDigit, new_deep, right_r, lineage);
							break;
					}
#if ASSERTS
					res.Measure.Is(new_measure);
					res[index].Value.Is(leaf.Value);
					res[index + 1].Value.Is(old_value);

#endif
					return res;
					throw ImplErrors.Invalid_execution_path;
				}

				public override void Iter(Action<Leaf<TValue>> action)
				{
#if ASSERTS
					action.IsNotNull();
#endif
					LeftDigit.Iter(action);
					DeepTree.Iter(action);
					RightDigit.Iter(action);
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
#if ASSERTS
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

				public override FTree<TChild> Remove(int index, Lineage lineage)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
					Digit newLeft;
					FTree<TChild> ret = this;
#if ASSERTS
					var new_measure = Measure - 1;
					var expected_at_index = index != Measure - 1 ? this[index + 1] : null;
					var expected_before_index = index != 0 ? this[index - 1] : null;
#endif
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							if (LeftDigit.IsFragment)
							{
								var fixedTree = FixLeftDigit(lineage);
								return fixedTree.Remove(index, lineage);
							}
							newLeft = LeftDigit.Remove(index, lineage);
							ret = CreateCheckNull(lineage, newLeft, DeepTree, RightDigit);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							var deep = DeepTree;
							FTree<Digit> newDeep;
							if (deep.IsFragment)
							{
								newDeep = deep.AddFirst(LeftDigit, lineage);
								newDeep = newDeep.Remove(index, lineage);
								newLeft = newDeep.Left;
								newDeep = newDeep.DropFirst(lineage);
								return new Compound(newLeft, newDeep, RightDigit, lineage);
							}
							newDeep = DeepTree.Remove(index - LeftDigit.Measure, lineage);
							ret =  CreateCheckNull(lineage, LeftDigit, newDeep, RightDigit);
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							if (RightDigit.IsFragment)
							{
								var fixedTree = FixRightDigit(lineage);
								return fixedTree.Remove(index, lineage);
							}
							var newRight = RightDigit.Remove(index - LeftDigit.Measure - DeepTree.Measure, lineage);
							ret =  CreateCheckNull(lineage, LeftDigit, DeepTree, newRight);
							break;
						default:
							throw ImplErrors.Invalid_execution_path;
					}
#if ASSERTS
					ret.Measure.Is(new_measure);
					if (expected_at_index != null) ret[index].Value.Is(expected_at_index.Value);
					if (expected_before_index != null) ret[index-1].Value.Is(expected_before_index.Value);
#endif
					return ret;

				}

				public override FTree<TChild> Reverse(Lineage lineage)
				{
					return MutateOrCreate(RightDigit.Reverse(lineage), DeepTree.Reverse(lineage), LeftDigit.Reverse(lineage), lineage);
				}


				public override void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost, Lineage lineage)
				{
					var whereIsThisIndex = WhereIsThisIndex(count);
#if ASSERTS
					count.Is(i => i <= Measure && i >= 0);
					var my_measure = Measure;
					var expected_leftmost_last = count > 0 ? this[count - 1] : null;
					var expected_leftmost_first = count > 0 ? this[0] : null;
					var expected_rightmost_last = Measure - count > 0 ? this[Measure - 1] : null;
					var expected_rightmost_first = Measure - count > 0 ? this[count] : null;
#endif

					switch (whereIsThisIndex)
					{
						case IN_START:
							leftmost = Empty;
							rightmost = this;
							break;
						case IN_MIDDLE_OF_LEFT:
							Digit left_1, left_2;
							LeftDigit.Split(count, out left_1, out left_2, lineage);
							leftmost = left_1 != null ? new Single(left_1, lineage) : Empty;
							rightmost = CreateCheckNull(lineage, left_2, DeepTree, RightDigit);
							break;
						case IN_START_OF_DEEP:
							leftmost = new Single(LeftDigit, lineage);
							rightmost = CreateCheckNull(lineage, null, DeepTree, RightDigit);
							break;
						case IN_MIDDLE_OF_DEEP:
							FTree<Digit> tree_1, tree_2;
							DeepTree.Split(count - LeftDigit.Measure, out tree_1, out tree_2, lineage);
							leftmost = CreateCheckNull(lineage, LeftDigit, tree_1);
							rightmost = CreateCheckNull(lineage, null, tree_2, RightDigit);
							break;
						case IN_START_OF_RIGHT:
							leftmost = CreateCheckNull(lineage, LeftDigit, DeepTree);
							rightmost = new Single(RightDigit, lineage);
							break;
						case IN_MIDDLE_OF_RIGHT:
							Digit right_1, right_2;
							RightDigit.Split(count - LeftDigit.Measure - DeepTree.Measure, out right_1, out right_2, lineage);
							leftmost = CreateCheckNull(lineage, LeftDigit, DeepTree, right_1);
							rightmost = CreateCheckNull(lineage, right_2);
							break;
						case IN_END:
							leftmost = this;
							rightmost = Empty;
							break;
						default:
							throw ImplErrors.Invalid_execution_path;
					}
#if ASSERTS
					leftmost.Measure.Is(count);
					rightmost.Measure.Is(Measure - count);
					if (expected_leftmost_first != null) leftmost[0].Is(expected_leftmost_first);
					if (expected_leftmost_last != null) leftmost[leftmost.Measure - 1].Is(expected_leftmost_last);
					if (expected_rightmost_first != null) rightmost[0].Is(expected_rightmost_first);
					if (expected_rightmost_last != null) rightmost[rightmost.Measure - 1].Is(expected_rightmost_last);
					
#endif

				}

				public override FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
					FTree<TChild> ret;
#if ASSERTS
					var old_size = Measure;
#endif
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							var new_left = LeftDigit.Update(index, leaf, lineage);
							ret= MutateOrCreate(new_left, DeepTree, RightDigit, lineage);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							var new_deep = DeepTree.Update(index - LeftDigit.Measure, leaf, lineage);
							ret= MutateOrCreate(LeftDigit, new_deep, RightDigit, lineage);
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							var new_right = RightDigit.Update(index - LeftDigit.Measure - DeepTree.Measure, leaf, lineage);
							ret= MutateOrCreate(LeftDigit, DeepTree, new_right, lineage);
							break;
						default:
							throw Funq.Errors.Arg_out_of_range("index", index);
					}
#if ASSERTS
					ret.Measure.Is(old_size);
					ret[index].Is(leaf);
#endif
					return ret;
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


				public override WeaklyTypedElement GetGrouping(int index) {
					switch (index) {
						case 0:
							return LeftDigit;
						case 1:
							return DeepTree;
						case 2:
							return RightDigit;
						default:
							throw Errors.Arg_out_of_range("index");
					}
				}
			}
		}
	}
}