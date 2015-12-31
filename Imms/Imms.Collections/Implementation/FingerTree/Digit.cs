using System;
using System.Linq;

namespace Imms.Implementation {
	static partial class FingerTree<TValue> {
		abstract partial class FTree<TChild> where TChild : Measured<TChild>, new() {
			//Calling a constructor on a generic type is a very expensive operation!			
			/// <summary>
			///     Used as a sort of hack because generic types can't have static methods.
			///     Basically, used to call ConstructTop and ConstructThis.
			/// </summary>
			static readonly TChild ExampleChild = new TChild();

			internal sealed partial class Digit : Measured<Digit> {

				const int
					IN_END = 8;

				const int
					IN_MIDDLE_OF_1 = 1;

				const int
					IN_MIDDLE_OF_2 = 3;

				const int
					IN_MIDDLE_OF_3 = 5;

				const int
					IN_MIDDLE_OF_4 = 7;

				const int
					IN_START = 0;

				const int
					IN_START_OF_2 = 2;

				const int
					IN_START_OF_3 = 4;

				const int
					IN_START_OF_4 = 6;

				const int
					OUTSIDE = 9;

				public TChild First;
				public TChild Fourth;
				public TChild Second;
				public int Size;
				public TChild Third;

				public Digit(TChild one, Lineage lineage)
					: base(one.Measure, lineage, 1, ExampleChild.Nesting + 1) {
#if ASSERTS

					one.AssertNotNull();
					one.IsFragment.AssertFalse();
#endif
					First = one;
					Size = 1;
				}

				public Digit(TChild one, TChild two, Lineage lineage)
					: base(one.Measure + two.Measure, lineage, 2, ExampleChild.Nesting + 1) {
#if ASSERTS

					one.AssertNotNull();
					two.AssertNotNull();
					new[]{two.IsFragment}.Any(x => x).AssertFalse(); //one can be a fragment as a hack used as part of RemoveAt
#endif
					First = one;
					Second = two;
					Size = 2;
				}

				public Digit(TChild one, TChild two, TChild three, Lineage lineage)
					: base(one.Measure + two.Measure + three.Measure, lineage, 3, ExampleChild.Nesting + 1) {
#if ASSERTS
					AssertEx.AssertAreNotNull(one, two, three);
					new[] { one.IsFragment, two.IsFragment, three.IsFragment }.Any(x => x).AssertFalse();
#endif

					First = one;
					Second = two;
					Third = three;
					Size = 3;
				}

				public Digit(TChild one, TChild two, TChild three, TChild four, Lineage lineage)
					: base(one.Measure + two.Measure + three.Measure + four.Measure, lineage, 4, ExampleChild.Nesting + 1) {
#if ASSERTS
					AssertEx.AssertAreNotNull(one, two, three, four);
					new[] { one.IsFragment, two.IsFragment, three.IsFragment, four.IsFragment }.Any(x => x).AssertFalse();
#endif
					First = one;
					Second = two;
					Third = three;
					Fourth = four;
					Size = 4;
				}

				public Digit()
					: base(0, Lineage.Immutable, 0, ExampleChild.Nesting + 1) {}

				public override Leaf<TValue> this[int index] {
					get {
#if ASSERTS
						index.AssertEqual(i => i < Measure);
#endif
						var m1 = First.Measure;
						if (index < m1) return First[index];
						var m2 = Second.Measure + m1;
						if (index < m2) return Second[index - m1];
						var m3 = m2 + Third.Measure;
						if (index < m3) return Third[index - m2];
						return Fourth[index - m3];
					}
				}

				/// <summary>
				///     Returns if the digit size is 1. Digit sizes of 1 cannot appear deep in the finger tree.
				/// </summary>
				public override bool IsFragment {
					get { return Size == 1; }
				}

				public TChild Left {
					get { return First; }
				}

				public TChild Right {
					get {
						switch (Size) {
							case 1:
								return First;
							case 2:
								return Second;
							case 3:
								return Third;
							case 4:
								return Fourth;
						}
						throw ImplErrors.Bad_digit_size(Size);
					}

				}

				public Digit ConstructMult(TValue[] arr, ref int index, int mult, Lineage lin) {
					var c = ExampleChild;
					switch (mult) {
						case 1:
							return new Digit(c.Construct3(arr, ref index, lin), lin);
						case 2:
							return new Digit(c.Construct3(arr, ref index, lin), c.Construct3(arr, ref index, lin), lin);
						case 3:
							return new Digit(c.Construct3(arr, ref index, lin), c.Construct3(arr, ref index, lin),
								c.Construct3(arr, ref index, lin), lin);
						case 4:
							return new Digit(c.Construct3(arr, ref index, lin), c.Construct3(arr, ref index, lin),
								c.Construct3(arr, ref index, lin), c.Construct3(arr, ref index, lin), lin);
						default:
							throw ImplErrors.Bad_digit_size(mult);
					}
				}

				public override string Print() {
					string[] rest;
					switch (Size) {
						case 1:
							rest = new[] { First.Print() };
							break;
						case 2:
							rest = new[] { First.Print(), Second.Print() };
							break;
						case 3:
							rest = new[] { First.Print(), Second.Print(), Third.Print() };
							break;
						case 4:
							rest = new[] { First.Print(), Second.Print(), Third.Print(), Fourth.Print() };
							break;
						default:
							throw ImplErrors.Bad_digit_size(Size);
					}
					var joined = String.Join(", ", rest);
					return Measure == Size ? Measure.ToString() : string.Format("{2} {0}: ({1})", Size, Measure, joined);
				}

				public override Digit Construct3(TValue[] arr, ref int index, Lineage lin) {
					var child = ExampleChild;
					return new Digit(child.Construct3(arr, ref index, lin), child.Construct3(arr, ref index, lin),
						child.Construct3(arr, ref index, lin), lin);
				}



				public override FingerTreeElement GetChild(int index) {
					switch (index) {
						case 0:
							return First;
						case 1:
							return Second;
						case 2:
							return Third;
						case 3:
							return Fourth;
						default:
							throw ImplErrors.Arg_out_of_range("index", index);
					}
				}

				//This method is used when we don't know the exact size of the digit we want to create.
				public Digit CreateCheckNull(Lineage lineage, TChild item1 = null, TChild item2 = null, TChild item3 = null,
					TChild item4 = null) {
					var itemsPresent = item1 != null ? 1 : 0;
					itemsPresent |= item2 != null ? 2 : 0;
					itemsPresent |= item3 != null ? 4 : 0;
					itemsPresent |= item4 != null ? 8 : 0;
					Digit res;
					switch (itemsPresent) {
						case 0 << 0 | 0 << 1 | 0 << 2 | 0 << 3:
							res = null;
							break;
						case 1 << 0 | 0 << 1 |  0 << 2 | 0 << 3:
							res = MutateOrCreate(item1, lineage);
							break;
						case 0 << 0 | 1 << 1 | 0 << 2 | 0 << 3:
							res = MutateOrCreate(item2, lineage);
							break;
						case 0 << 0 | 0 << 1 | 1 << 2 | 0 << 3:
							res = MutateOrCreate(item3, lineage);
							break;
						case 0 << 0 | 0 << 1 | 0 << 2 | 1 << 3:
							res = MutateOrCreate(item4, lineage);
							break;
						case 1 << 0 | 1 << 1 | 0 << 2 | 0 << 3:
							res = MutateOrCreate(item1, item2, lineage);
							break;
						case 1 << 0 | 1 << 1 | 1 << 2 | 0 << 3:
							res = MutateOrCreate(item1, item2, item3, lineage);
							break;
						case 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item2, item3, item4, lineage);
							break;
						case 0 << 0 | 1 << 1 | 1 << 2 | 0 << 3:
							res = MutateOrCreate(item2, item3, lineage);
							break;
						case 0 << 0 | 1 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item2, item3, item4, lineage);
							break;
						case 0 << 0 | 0 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item3, item4, lineage);
							break;
						case 1 << 0 | 0 << 1 | 1 << 2 | 0 << 3:
							res = MutateOrCreate(item1, item3, lineage);
							break;
						case 1 << 0 | 0 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item3, item4, lineage);
							break;
						case 0 << 0 | 1 << 1 | 0 << 2 | 1 << 3:
							res = MutateOrCreate(item2, item4, lineage);
							break;
						case 1 << 0 | 0 << 1 | 0 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item4, lineage);
							break;
						case 1 << 0 | 1 << 1 | 0 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item2, item4, lineage);
							break;
						default:
							throw ImplErrors.Invalid_execution_path("Checked all digit permutations already.");
					}

					return res;
				}

				public void Fuse(Digit other, out Digit leftmost, out Digit middle, out Digit rightmost, Lineage lineage) {
#if ASSERTS
					other.AssertNotNull();
#endif
					var match = (Size << 3) | other.Size;

					switch (match) {
						case 1 << 3 | 1:
							leftmost = MutateOrCreate(First, other.First, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 2:
							leftmost = MutateOrCreate(First, other.First, other.Second, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 3:
							leftmost = MutateOrCreate(First, other.First, other.Second, other.Third, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 4:
							leftmost = new Digit(First, other.First, lineage);
							middle = MutateOrCreate(other.Second, other.Third, other.Fourth, lineage);
							rightmost = null;
							return;
						case 2 << 3 | 1:
							leftmost = MutateOrCreate(First, Second, other.First, lineage);
							middle = null;
							rightmost = null;
							return;
						case 2 << 3 | 2:
						case 2 << 3 | 3:
						case 3 << 3 | 2:
						case 3 << 3 | 3:
							leftmost = this;
							middle = other;
							rightmost = null;
							return;
						case 2 << 3 | 4:
							leftmost = new Digit(First, Second, other.First, lineage);
							middle = MutateOrCreate(other.Second, other.Third, other.Fourth, lineage);
							rightmost = null;
							return;
						case 3 << 3 | 4:
							leftmost = this;
							middle = new Digit(other.First, other.Second, lineage);
							rightmost = other.MutateOrCreate(other.Third, other.Fourth, lineage);
							return;
						case 3 << 3 | 1:
							leftmost = new Digit(First, Second, lineage);
							middle = MutateOrCreate(Third, other.First, lineage);
							rightmost = null;
							return;
						case 4 << 3 | 1:
							leftmost = new Digit(First, Second, lineage);
							middle = MutateOrCreate(Third, Fourth, other.First, lineage);
							rightmost = null;
							return;
						case 4 << 3 | 2:
						case 4 << 3 | 3:
							leftmost = new Digit(First, Second, lineage);
							middle = MutateOrCreate(Third, Fourth, lineage);
							rightmost = other;
							return;
						case 4 << 3 | 4:
							leftmost = new Digit(First, Second, Third, lineage);
							middle = new Digit(Fourth, other.First, other.Second, lineage);
							rightmost = other.MutateOrCreate(other.Third, other.Fourth, lineage);
							return;
						default:
							throw ImplErrors.Invalid_execution_path("Checked all possible size permutations already.");
					}
					//we should've handled all the cases in the Switch statement. Otherwise, produce this error.
					
				}

				/// <summary>
				///     This method will re-initialize the current instance with the specified parameters by mutation.
				/// </summary>
				public Digit _mutate(int measure, int size, TChild a, TChild b = null, TChild c = null, TChild d = null) {
#if ASSERTS
					var all = new[] { a, b, c, d };
					var notNull = all.Where(x => x != null).ToArray();
					if (notNull.Length != 2) {
						//a hack allows one of the inner elements to be fragments...
						notNull.All(x => !x.IsFragment).AssertTrue();
					}
					else {
						notNull[1].IsFragment.AssertFalse(); //this only applies to the 1st element!
					}
					
					notNull.Count().AssertEqual(size);
					notNull.Sum(x => x.Measure).AssertEqual(measure);

#endif
					First = a;
					Second = b;
					Third = c;
					Fourth = d;

					Measure = measure;
					Size = size;
					ChildCount = size;
					return this;
				}

				private Digit MutateOrCreate(TChild a, Lineage lineage) {
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure, 1, a) : new Digit(a, lineage);
				}

				private Digit MutateOrCreate(TChild a, TChild b, Lineage lineage) {
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure + b.Measure, 2, a, b) : new Digit(a, b, lineage);
				}

				private Digit MutateOrCreate(TChild a, TChild b, TChild c, Lineage lineage) {
					return Lineage.AllowMutation(lineage)
						? _mutate(a.Measure + b.Measure + c.Measure, 3, a, b, c) : new Digit(a, b, c, lineage);
				}

				private Digit MutateOrCreate(TChild a, TChild b, TChild c, TChild d, Lineage lineage) {
					return Lineage.AllowMutation(lineage)
						? _mutate(a.Measure + b.Measure + c.Measure + d.Measure, 4, a, b, c, d) : new Digit(a, b, c, d, lineage);
				}

				public Digit AddFirst(TChild item, Lineage lineage) {

					switch (Size) {
						case 1:
							return MutateOrCreate(item, First, lineage);
						case 2:
							return MutateOrCreate(item, First, Second, lineage);
						case 3:
							return MutateOrCreate(item, First, Second, Third, lineage);
						case 4:
							throw ImplErrors.Digit_too_large(Size);
					}
					throw ImplErrors.Bad_digit_size(Size);
				}

				public Digit AddLast(TChild item, Lineage lineage) {
					switch (Size) {
						case 1:
							return MutateOrCreate(First, item, lineage);
						case 2:
							return MutateOrCreate(First, Second, item, lineage);
						case 3:
							return MutateOrCreate(First, Second, Third, item, lineage);
						case 4:
							throw ImplErrors.Bad_digit_size(Size);
					}
					throw ImplErrors.Bad_digit_size(Size);
				}

				public override void Fuse(Digit other, out Digit first, out Digit last, Lineage lineage) {
					Digit skip;
					Fuse(other, out first, out last, out skip, lineage);
				}

				public override void Insert(int index, Leaf<TValue> leaf, out Digit leftmost, out Digit rightmost, Lineage lineage) {
#if ASSERTS
					leaf.AssertNotNull();
#endif

					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild myLeftmost;
					TChild myRightmost;
					leftmost = null;
					rightmost = null;
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_1:
							First.Insert(index, leaf, out myLeftmost, out myRightmost, lineage);
							if (Size == 4 && myRightmost != null) {

								leftmost = new Digit(myLeftmost, myRightmost, Second, lineage);
								rightmost = MutateOrCreate(Third, Fourth, lineage);
								return;
							}
							leftmost = myRightmost != null
								? CreateCheckNull(lineage, myLeftmost, myRightmost, Second, Third)
								: CreateCheckNull(lineage, myLeftmost, Second, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							Second.Insert(index - First.Measure, leaf, out myLeftmost, out myRightmost, lineage);
							if (Size == 4 && myRightmost != null) {
								leftmost = new Digit(First, myLeftmost, myRightmost, lineage);
								rightmost = MutateOrCreate(Third, Fourth, lineage);
								return;
							}
							leftmost = myRightmost != null
								? CreateCheckNull(lineage, First, myLeftmost, myRightmost, Third)
								: CreateCheckNull(lineage, First, myLeftmost, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							Third.Insert(index - First.Measure - Second.Measure, leaf, out myLeftmost, out myRightmost, lineage);
							if (Size == 4 && myRightmost != null) {
								leftmost = new Digit(First, Second, myLeftmost, lineage);
								rightmost = MutateOrCreate(myRightmost, Fourth, lineage);
								return;
							}
							leftmost =
								myRightmost != null
									? CreateCheckNull(lineage, First, Second, myLeftmost, myRightmost)
									: CreateCheckNull(lineage, First, Second, myLeftmost, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							Fourth.Insert(index - Measure + Fourth.Measure, leaf, out myLeftmost, out myRightmost, lineage);
							if (Size == 4 && myRightmost != null) {
								leftmost = new Digit(First, Second, Third, lineage);
								rightmost = MutateOrCreate(myLeftmost, myRightmost, lineage);
								return;
							}
							leftmost = MutateOrCreate(First, Second, Third, myLeftmost, lineage);
							rightmost = null;
							return;						
						default:
							throw ImplErrors.Invalid_execution_path("");
					}

					
				}

				public override void Iter(Action<Leaf<TValue>> action) {
#if ASSERTS
					action.AssertNotNull();
#endif
					switch (Size) {
						case 1:
							First.Iter(action);
							return;
						case 2:
							First.Iter(action);
							Second.Iter(action);
							return;
						case 3:
							First.Iter(action);
							Second.Iter(action);
							Third.Iter(action);
							return;
						case 4:
							First.Iter(action);
							Second.Iter(action);
							Third.Iter(action);
							Fourth.Iter(action);
							return;
						default:
							throw ImplErrors.Invalid_execution_path("Checked all sizes already");
					}
					
				}

				public override void IterBack(Action<Leaf<TValue>> action) {
#if ASSERTS
					action.AssertNotNull();
#endif
					switch (Size) {
						case 1:
							First.IterBack(action);
							return;
						case 2:
							Second.IterBack(action);
							First.IterBack(action);
							return;
						case 3:
							Third.IterBack(action);
							Second.IterBack(action);
							First.IterBack(action);
							return;
						case 4:
							Fourth.IterBack(action);
							Third.IterBack(action);
							Second.IterBack(action);
							First.IterBack(action);
							return;
						default:
							throw ImplErrors.Invalid_execution_path("Checked all sizes already");
					}
					
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> action) {
#if ASSERTS
					action.AssertNotNull();
#endif
					if (Fourth != null) if (!Fourth.IterBackWhile(action)) return false;
					if (Third != null) if (!Third.IterBackWhile(action)) return false;
					if (Second != null) if (!Second.IterBackWhile(action)) return false;
					return First.IterBackWhile(action);
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> action) {
#if ASSERTS
					action.AssertNotNull();
#endif

					if (!First.IterWhile(action)) return false;
					if (Second == null) return true;
					if (!Second.IterWhile(action)) return false;
					if (Third == null) return true;
					if (!Third.IterWhile(action)) return false;
					if (Fourth == null) return true;
					return Fourth.IterWhile(action);
				}

				public Digit RemoveFirst(Lineage lineage) {

					switch (Size) {
						case 1:
							throw ImplErrors.Digit_too_small(Size);
						case 2:
							return MutateOrCreate(Second, lineage);
						case 3:
							return MutateOrCreate(Second, Third, lineage);
						case 4:
							return MutateOrCreate(Second, Third, Fourth, lineage);
					}
					throw ImplErrors.Bad_digit_size(Size);
				}

				public Digit RemoveLast(Lineage lineage) {
					switch (Size) {
						case 1:
							throw ImplErrors.Bad_digit_size(Size);
						case 2:
							return MutateOrCreate(First, lineage);
						case 3:
							return MutateOrCreate(First, Second, lineage);
						case 4:
							return MutateOrCreate(First, Second, Third, lineage);
					}
					throw ImplErrors.Bad_digit_size(Size);
				}

				public override Digit Remove(int index, Lineage lineage) {
					var whereIsThisIndex = WhereIsThisIndex(index);
#if ASSERTS
					Size.AssertUnequal(1);
#endif
					TChild res;
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_1:
							res = First.Remove(index, lineage);
							if (res != null && res.IsFragment) {
								TChild left, right;
								res.Fuse(Second, out left, out right, lineage);
								return CreateCheckNull(lineage, left, right, Third, Fourth);
							}
							return CreateCheckNull(lineage, res, Second, Third, Fourth);
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							res = Second.Remove(index - First.Measure, lineage);
							if (res != null && res.IsFragment) {
								TChild left, right;
								First.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, left, right, Third, Fourth);
							}
							return CreateCheckNull(lineage, First, res, Third, Fourth);
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							res = Third.Remove(index - First.Measure - Second.Measure, lineage);
							if (res != null && res.IsFragment) {
								TChild left, right;
								Second.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, First, left, right, Fourth);
							}
							return CreateCheckNull(lineage, First, Second, res, Fourth);
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							res = Fourth.Remove(index - First.Measure - Second.Measure - Third.Measure, lineage);
							if (res != null && res.IsFragment) {
								TChild left, right;
								Third.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, First, Second, left, right);
							}
							return CreateCheckNull(lineage, First, Second, Third, res);
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all index locations.");
					}
				}

				public override Digit Reverse(Lineage lineage) {

					switch (Size) {
						case 1:
							return MutateOrCreate(First.Reverse(lineage), lineage);
						case 2:
							return MutateOrCreate(Second.Reverse(lineage), First.Reverse(lineage), lineage);
						case 3:
							return MutateOrCreate(Third.Reverse(lineage), Second.Reverse(lineage), First.Reverse(lineage), lineage);
						case 4:
							return MutateOrCreate(Fourth.Reverse(lineage), Third.Reverse(lineage), Second.Reverse(lineage),
								First.Reverse(lineage), lineage);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all sizes");
					}
					
				}

				public void Split(int index, out Digit left, out TChild center, out Digit right,Lineage lineage) {
					switch (WhereIsThisIndex(index)) {
						case IN_START:
						case IN_MIDDLE_OF_1:
							left = null;
							center = First;
							right = CreateCheckNull(lineage, Second, Third, Fourth);
							break;
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							left = new Digit(First, lineage);
							center = Second;
							right = CreateCheckNull(lineage, Third, Fourth);
							break;
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							left = new Digit(First, Second, lineage);
							center = Third;
							right = CreateCheckNull(lineage, Fourth);
							break;
						case IN_MIDDLE_OF_4:
						case IN_START_OF_4:
							left =new Digit(First, Second, Third, lineage);
							center = Fourth;
							right = null;
							break;
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all index locations.");

					}
				}

				public override Digit Update(int index, Leaf<TValue> leaf, Lineage lineage) {
					if (Lineage.AllowMutation(lineage)) return Update_MUTATES(index, leaf);
#if ASSERTS
					leaf.AssertNotNull();
#endif
					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild res;
					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_1:
							res = First.Update(index, leaf, lineage);
							return CreateCheckNull(lineage, res, Second, Third, Fourth);
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							res = Second.Update(index - First.Measure, leaf, lineage);
							return CreateCheckNull(lineage, First, res, Third, Fourth);
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							res = Third.Update(index - First.Measure - Second.Measure, leaf, lineage);
							return CreateCheckNull(lineage, First, Second, res, Fourth);
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							res = Fourth.Update(index - First.Measure - Second.Measure - Third.Measure, leaf, lineage);
							return CreateCheckNull(lineage, First, Second, Third, res);
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all index locations.");

					}
					
				}

				Digit Update_MUTATES(int index, Leaf<TValue> leaf) {
					var whereIsThisIndex = WhereIsThisIndex(index);

					switch (whereIsThisIndex) {
						case IN_START:
						case IN_MIDDLE_OF_1:
							First = First.Update(index, leaf, Lineage);
							return this;
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							Second = Second.Update(index - First.Measure, leaf, Lineage);
							return this;
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							Third = Third.Update(index - First.Measure - Second.Measure, leaf, Lineage);
							return this;
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							Fourth = Fourth.Update(index - First.Measure - Second.Measure - Third.Measure, leaf, Lineage);
							return this;
						case IN_END:
						case OUTSIDE:
							throw ImplErrors.Arg_out_of_range("index", index);
						default:
							throw ImplErrors.Invalid_execution_path("Checked all index locations already.");
					}
					
				}

				/// <summary>
				///     Returns a code telling where is the index located
				/// </summary>
				/// <param name="index"> </param>
				/// <returns> </returns>
				int WhereIsThisIndex(int index) {
#if ASSERTS
					index.AssertEqual(i => i < Measure);
#endif
					var measure1 = First.Measure;
					if (index == 0) return IN_START;
					if (index < measure1) return IN_MIDDLE_OF_1;
					if (index == measure1) return IN_START_OF_2;
					var measure2 = measure1 + Second.Measure;
					if (index < measure2) return IN_MIDDLE_OF_2;
					if (index == measure2) return IN_START_OF_3;
					var measure3 = measure2 + Third.Measure;
					if (index < measure3) return IN_MIDDLE_OF_3;
					if (index == measure3) return IN_START_OF_4;
					var measure4 = measure3 + Fourth.Measure;
					if (index < measure4) return IN_MIDDLE_OF_4;
					if (index == measure4) return IN_END;
					if (index > measure4) return OUTSIDE;
					throw ImplErrors.Invalid_execution_path("Checked all possible index locations");
				}
			}
		}
	}
}