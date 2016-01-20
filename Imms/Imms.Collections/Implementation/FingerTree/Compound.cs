using System;

namespace Imms.Implementation {
	static partial class FingerTree<TValue> {
		internal abstract partial class FTree<TChild>
			where TChild : Measured<TChild>, new() {
			private sealed class Compound : FTree<TChild> {

				const int
					IN_END = 6;

				const int
					IN_MIDDLE_OF_DEEP = 3;

				const int
					IN_MIDDLE_OF_LEFT = 1;

				const int
					IN_MIDDLE_OF_RIGHT = 5;

				const int
					IN_START = 0;

				const int
					IN_START_OF_DEEP = 2;

				const int
					IN_START_OF_RIGHT = 4;

				const int
					OUTSIDE = 7;

				public FTree<Digit> DeepTree;
				public Digit LeftDigit;
				public Digit RightDigit;

				public Compound(Digit leftDigit, FTree<Digit> deepTree, Digit rightDigit, Lineage lineage)
					: base(leftDigit.Measure + deepTree.Measure + rightDigit.Measure, TreeType.Compound, lineage, 3) {

					leftDigit.AssertNotNull();
					deepTree.AssertNotNull();
					rightDigit.AssertNotNull();
					_mutate(leftDigit, deepTree, rightDigit);
				}

				public override Leaf<TValue> this[int index] {
					get {

						index.AssertEqual(i => i < Measure);

						var m1 = LeftDigit.Measure;
						var m2 = DeepTree.Measure + m1;

						if (index < m1) return LeftDigit[index];
						if (index < m2) return DeepTree[index - m1];
						if (index < Measure) return RightDigit[index - m2];

						throw ImplErrors.Arg_out_of_range("index", index);
					}
				}

				public override bool IsFragment {
					get { return false; }
				}

				public override TChild Left {
					get { return LeftDigit.Left; }
				}

				public override TChild Right {
					get { return RightDigit.Right; }
				}

				public override string Print() {
					return string.Format("[[{0} | {1} | {2}]]", LeftDigit.Print(), DeepTree.Print(), RightDigit.Print());
				}

				FTree<TChild> CreateCheckNull(Lineage lineage, Digit left = null, FTree<Digit> deep = null, Digit right = null) {
					var memberPermutation = left != null ? 1 << 0 : 0;
					memberPermutation |= (deep != null && deep.Measure != 0) ? 1 << 1 : 0;
					memberPermutation |= right != null ? 1 << 2 : 0;

					switch (memberPermutation) {
						case 0 << 0| 0 << 1 | 0 << 2:
							return Empty;
						case 1 << 0 | 0 << 1 | 0 << 2:
							return new Single(left, lineage);
						case 1 << 0 | 1 << 1 | 0 << 2:
							var r2 = deep.Right;
							var deep1 = deep.RemoveLast(lineage);

							return MutateOrCreate(left, deep1, r2, lineage);
						case 1 << 0 | 1 << 1 | 1 << 2:
							return MutateOrCreate(left, deep, right, lineage);
						case 0 << 0 | 1 << 1 | 1 << 2:
							return MutateOrCreate(deep.Left, deep.RemoveFirst(lineage), right, lineage);
						case 1 << 0 | 0 << 1 | 1 << 2:
							return MutateOrCreate(left, deep, right, lineage);
						case 0 << 0 | 1 << 1 | 0 << 2:
							left = deep.Left;
							deep = deep.RemoveFirst(lineage);
							if (deep.Measure != 0) {
								right = deep.Right;
								deep = deep.RemoveLast(lineage);
								return MutateOrCreate(left, deep, right, lineage);
							}
							return new Single(left, lineage);
						case 0 << 0 | 0 << 1 | 1 << 2:
							return new Single(right, lineage);
						default:
							throw ImplErrors.Invalid_execution_path("Explicitly checked all possible tree permutations.");
					}
				}
				/// <summary>
				///     This method will re-initialize this instance using the specified parameters.
				/// </summary>
				private FTree<TChild> _mutate(Digit left, FTree<Digit> deep, Digit right) {
					LeftDigit = left;
					DeepTree = deep;
					RightDigit = right;
					Measure = left.Measure + deep.Measure + right.Measure;
					return this;
				}

				/// <summary>
				///     <para>
				///         This method can mutate the current instance and return it, or return a new instance, based on the supplied
				///         Lineage.
				///     </para>
				///     <para>If the current Lineage allows mutation from the specified Lineage, the instance will be MUTATED and returned.</para>
				///     <para>Otherwise, the method will return a NEW instance that is a member of the supplied Lineage. </para>
				/// </summary>
				private FTree<TChild> MutateOrCreate(Digit left, FTree<Digit> deep, Digit right, Lineage lineage) {
					if (_lineage.AllowMutation(lineage)) {
						return _mutate(left, deep, right);
					} else return new Compound(left, deep, right, lineage);
				}

				public override FTree<TChild> AddFirst(TChild item, Lineage lineage) {
					FTree<TChild> ret;
#if ASSERTS
					var expectedSize = Measure + item.Measure;
#endif
					if (LeftDigit.Size < 4) ret = MutateOrCreate(LeftDigit.AddFirst(item, lineage), DeepTree, RightDigit, lineage);
					else {
						var leftmost = new Digit(item, LeftDigit.First, lineage);
						var rightmost = LeftDigit.RemoveFirst(lineage);
						var newDeep = DeepTree.AddFirst(rightmost, lineage);
						ret = MutateOrCreate(leftmost, newDeep, RightDigit, lineage);
					}
#if ASSERTS
					ret.Measure.AssertEqual(expectedSize);
					ret.Left.AssertEqual(item);
#endif
					return ret;

				}

				public override FTree<TChild> AddLast(TChild item, Lineage lineage) {
#if ASSERTS
					var expectedSize = Measure + item.Measure;
#endif
					FTree<TChild> ret;
					if (RightDigit.Size < 4) ret = MutateOrCreate(LeftDigit, DeepTree, RightDigit.AddLast(item, lineage), lineage);
					else {
						var rightmost = new Digit(RightDigit.Fourth, item, lineage);
						var leftmost = RightDigit.RemoveLast(lineage);
						var newDeep = DeepTree.AddLast(leftmost, lineage);
						ret = MutateOrCreate(LeftDigit, newDeep, rightmost, lineage);
					}
#if ASSERTS
					ret.Measure.AssertEqual(expectedSize);
					ret.Right.AssertEqual(item);
#endif
					return ret;

				}

				public override FTree<TChild> RemoveFirst(Lineage lineage) {
					FTree<TChild> ret;
#if ASSERTS
					var expected = Measure - Left.Measure;
#endif

					if (LeftDigit.Size > 1) {
						var newLeft = LeftDigit.RemoveFirst(lineage);
						ret = MutateOrCreate(newLeft, DeepTree, RightDigit, lineage);
					} else if (DeepTree.Measure > 0) {
						var newLeft = DeepTree.Left;
						var newDeep = DeepTree.RemoveFirst(lineage);
						ret = MutateOrCreate(newLeft, newDeep, RightDigit, lineage);
					} else ret = new Single(RightDigit, lineage);
#if ASSERTS
					ret.Measure.AssertEqual(expected);
#endif
					return ret;
				}

				public override FTree<TChild> RemoveLast(Lineage lineage) {
					FTree<TChild> ret;
#if ASSERTS
					var expectedSize = Measure - Right.Measure;
#endif
					if (RightDigit.Size > 1) {
						var newRight = RightDigit.RemoveLast(lineage);
						ret = MutateOrCreate(LeftDigit, DeepTree, newRight, lineage);
					} else if (DeepTree.Measure > 0) {
						var newRight = DeepTree.Right;
						var newDeep = DeepTree.RemoveLast(lineage);
						ret = MutateOrCreate(LeftDigit, newDeep, newRight, lineage);
					} else ret = new Single(LeftDigit, lineage);
#if ASSERTS
					ret.Measure.AssertEqual(expectedSize);
#endif
					return ret;
				}

				public override void Split(int index, out FTree<TChild> left, out TChild child, out FTree<TChild> right, Lineage lineage) {
#if ASSERTS
					var oldMeasure = Measure;
					var oldValue = this[index];

#endif
					switch (WhereIsThisIndex(index)) {
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							Digit lLeft, lRight;
							LeftDigit.Split(index, out lLeft, out child, out lRight, lineage);
							index -= lLeft == null ? 0 : lLeft.Measure;
							left = CreateCheckNull(Lineage.Immutable, lLeft);
							right = CreateCheckNull(lineage, lRight, DeepTree, RightDigit);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							index -= LeftDigit.Measure;
							FTree<Digit> mLeft, mRight;
							Digit mCenter;
							DeepTree.Split(index, out mLeft, out mCenter, out mRight, lineage);
							Digit mcLeft, mcRight;
							index -= mLeft.Measure;
							mCenter.Split(index, out mcLeft, out child, out mcRight, lineage);
							index -= mcLeft == null ? 0 : mcLeft.Measure;
							left = CreateCheckNull(Lineage.Immutable, LeftDigit, mLeft, mcLeft);
							right = CreateCheckNull(lineage, mcRight, mRight, RightDigit);
							break;
						case IN_MIDDLE_OF_RIGHT:
						case IN_START_OF_RIGHT:
							Digit rLeft, rRight;
							index -= LeftDigit.Measure + DeepTree.Measure;
							RightDigit.Split(index, out rLeft, out child, out rRight, lineage);
							index -= rLeft == null ? 0 : rLeft.Measure;
							right = CreateCheckNull(Lineage.Immutable, rRight);
							left = CreateCheckNull(lineage, LeftDigit, DeepTree, rLeft);
							break;
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Index didn't match any of the cases.");
					}
#if ASSERTS
					oldMeasure.AssertEqual(left.Measure + child.Measure + right.Measure);
					oldValue.AssertEqual(child[index]);
#endif
				}

				FTree<TChild> FixLeftDigit(Lineage lineage) {
					if (!LeftDigit.IsFragment) return this;
					if (DeepTree.Measure == 0) {
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last, lineage);
						return CreateCheckNull(lineage, first, DeepTree, last);
					} else {
						var fromDeep = DeepTree.Left;
						var newDeep = DeepTree.RemoveFirst(lineage);
						Digit first, last;
						LeftDigit.Fuse(fromDeep, out first, out last, lineage);
						if (last == null) return MutateOrCreate(first, newDeep, RightDigit, lineage);
						return MutateOrCreate(first, newDeep.AddFirst(last, lineage), RightDigit, lineage);
					}
				}

				FTree<TChild> FixRightDigit(Lineage lineage) {
					if (!RightDigit.IsFragment) return this;
					FTree<TChild> ret;
					if (DeepTree.Measure == 0) {
						Digit first, last;
						LeftDigit.Fuse(RightDigit, out first, out last, lineage);
						ret = CreateCheckNull(lineage, first, DeepTree, last);
						return ret;
					} else {
						var fromDeep = DeepTree.Right;
						var newDeep = DeepTree.RemoveLast(lineage);
						Digit first, last;
						fromDeep.Fuse(RightDigit, out first, out last, lineage);
						if (last == null) ret = MutateOrCreate(LeftDigit, newDeep, first, lineage);
						else ret = MutateOrCreate(LeftDigit, newDeep.AddLast(first, lineage), last, lineage);

					}
#if ASSERTS
					ret.Measure.AssertEqual(Measure);
#endif
					return ret;
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage) {
					var whereIsThisIndex = WhereIsThisIndex(index);
#if ASSERTS
					var newMeasure = Measure + 1;
					var oldValue = this[index].Value;
#endif
					FTree<Digit> newDeep;
					FTree<TChild> res = null;
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							Digit leftL, leftR;
							LeftDigit.Insert(index, leaf, out leftL, out leftR, lineage);
							newDeep = leftR != null ? DeepTree.AddFirst(leftR, lineage) : DeepTree;
							res = MutateOrCreate(leftL, newDeep, RightDigit, lineage);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							newDeep = DeepTree.Insert(index - LeftDigit.Measure, leaf, lineage);
							res = MutateOrCreate(LeftDigit, newDeep, RightDigit, lineage);
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							Digit rightR;
							Digit rightL;
							RightDigit.Insert(index - LeftDigit.Measure - DeepTree.Measure, leaf, out rightL, out rightR, lineage);
							newDeep = rightR != null ? DeepTree.AddLast(rightL, lineage) : DeepTree;
							rightR = rightR ?? rightL;
							res = MutateOrCreate(LeftDigit, newDeep, rightR, lineage);
							break;
					}
#if ASSERTS
					res.Measure.AssertEqual(newMeasure);
					res[index].Value.AssertEqual(leaf.Value);
					res[index + 1].Value.AssertEqual(oldValue);

#endif
					return res;

				}

				public override void Iter(Action<Leaf<TValue>> action) {
#if ASSERTS
					action.AssertNotNull();
#endif
					LeftDigit.Iter(action);
					DeepTree.Iter(action);
					RightDigit.Iter(action);
				}

				public override void IterBack(Action<Leaf<TValue>> action) {
#if ASSERTS
					action.AssertNotNull();
#endif
					RightDigit.IterBack(action);
					DeepTree.IterBack(action);
					LeftDigit.IterBack(action);
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> action) {
					if (!RightDigit.IterBackWhile(action)) return false;
					if (!DeepTree.IterBackWhile(action)) return false;
					if (!LeftDigit.IterBackWhile(action)) return false;
					return true;
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> action) {
					if (!LeftDigit.IterWhile(action)) return false;
					if (!DeepTree.IterWhile(action)) return false;
					if (!RightDigit.IterWhile(action)) return false;
					return true;
				}

				public override FTree<TChild> RemoveAt(int index, Lineage lineage) {
					var whereIsThisIndex = WhereIsThisIndex(index);
					Digit newLeft;
					FTree<TChild> ret = this;
#if ASSERTS
					var newMeasure = Measure - 1;
					var expectedAtIndex = index != Measure - 1 ? this[index + 1] : null;
					var expectedBeforeIndex = index != 0 ? this[index - 1] : null;
#endif
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							if (LeftDigit.IsFragment) {
								var fixedTree = FixLeftDigit(lineage);
								ret = fixedTree.RemoveAt(index, lineage);
							} else {
								newLeft = LeftDigit.Remove(index, lineage);
								ret = CreateCheckNull(lineage, newLeft, DeepTree, RightDigit);
							}
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							var deep = DeepTree;
							FTree<Digit> newDeep;
							if (deep.IsFragment) {
								newDeep = deep.AddFirst(LeftDigit, lineage);
								newDeep = newDeep.RemoveAt(index, lineage);
								newLeft = newDeep.Left;
								newDeep = newDeep.RemoveFirst(lineage);
								ret = MutateOrCreate(newLeft, newDeep, RightDigit, lineage);
							} else {
								newDeep = DeepTree.RemoveAt(index - LeftDigit.Measure, lineage);
								ret = CreateCheckNull(lineage, LeftDigit, newDeep, RightDigit);
							}
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							if (RightDigit.IsFragment) {
								var fixedTree = FixRightDigit(lineage);
								ret = fixedTree.RemoveAt(index, lineage);
							} else {
								var newRight = RightDigit.Remove(index - LeftDigit.Measure - DeepTree.Measure, lineage);
								ret = CreateCheckNull(lineage, LeftDigit, DeepTree, newRight);
							}
							break;
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all possible index locations already.");
					}
#if ASSERTS
					ret.Measure.AssertEqual(newMeasure);
					if (expectedAtIndex != null) ret[index].Value.AssertEqual(expectedAtIndex.Value);
					if (expectedBeforeIndex != null) ret[index-1].Value.AssertEqual(expectedBeforeIndex.Value);
#endif
					return ret;

				}

				public override FTree<TChild> Reverse(Lineage lineage) {
					return MutateOrCreate(RightDigit.Reverse(lineage), DeepTree.Reverse(lineage), LeftDigit.Reverse(lineage), lineage);
				}


				public override FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage) {
					var whereIsThisIndex = WhereIsThisIndex(index);
					FTree<TChild> ret;
#if ASSERTS
					var oldSize = Measure;
#endif
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_LEFT:
							var newLeft = LeftDigit.Update(index, leaf, lineage);
							ret = MutateOrCreate(newLeft, DeepTree, RightDigit, lineage);
							break;
						case IN_START_OF_DEEP:
						case IN_MIDDLE_OF_DEEP:
							if (DeepTree.Measure == 0) goto case IN_START_OF_RIGHT;
							var newDeep = DeepTree.Update(index - LeftDigit.Measure, leaf, lineage);
							ret = MutateOrCreate(LeftDigit, newDeep, RightDigit, lineage);
							break;
						case IN_START_OF_RIGHT:
						case IN_MIDDLE_OF_RIGHT:
							var newRight = RightDigit.Update(index - LeftDigit.Measure - DeepTree.Measure, leaf, lineage);
							ret = MutateOrCreate(LeftDigit, DeepTree, newRight, lineage);
							break;
						default:
							throw ImplErrors.Arg_out_of_range("index", index);
					}
#if ASSERTS
					ret.Measure.AssertEqual(oldSize);
					ret[index].AssertEqual(leaf);
#endif
					return ret;
				}

				/// <summary>
				///     Returns 0 if index is 0 (meaning, empty).
				///     Returns 1 if index is in left digit
				///     Returns 2 if index encompasses the left digit.
				///     Returns 3 if he index is in the deep tree.
				///     Returns 4 if the index encompasses the left digit + deep tree
				///     Returns 5 if the index is in the right digit.
				///     Returns 6 if the index encompasses the entire tree.
				///     Returns 7 if the index is outside the tree.
				/// </summary>
				/// <param name="index"> </param>
				/// <returns> </returns>
				int WhereIsThisIndex(int index) {
					if (index == 0) return IN_START;
					if (index < LeftDigit.Measure) return IN_MIDDLE_OF_LEFT;
					if (index == LeftDigit.Measure) return DeepTree.Measure == 0 ? IN_START_OF_RIGHT : IN_START_OF_DEEP;
					if (index < LeftDigit.Measure + DeepTree.Measure) return IN_MIDDLE_OF_DEEP;
					if (index == LeftDigit.Measure + DeepTree.Measure) return IN_START_OF_RIGHT;
					if (index < Measure) return IN_MIDDLE_OF_RIGHT;
					if (index == Measure) return IN_END;
					return OUTSIDE;
				}

				public override FingerTreeElement GetChild(int index) {
					switch (index) {
						case 0:
							return LeftDigit;
						case 1:
							return DeepTree;
						case 2:
							return RightDigit;
						default:
							throw ImplErrors.Arg_out_of_range("index", index);
					}
				}
			}
		}
	}
}