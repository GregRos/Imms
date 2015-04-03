using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	static partial class FingerTree<TValue>
	{
		abstract partial class FTree<TChild>
		{
			internal sealed class Digit : Measured<Digit>
			{

				private const int
					IN_END = 8;

				private const int
					IN_MIDDLE_OF_1 = 1;

				private const int
					IN_MIDDLE_OF_2 = 3;

				private const int
					IN_MIDDLE_OF_3 = 5;

				private const int
					IN_MIDDLE_OF_4 = 7;

				private const int
					IN_START = 0;

				private const int
					IN_START_OF_2 = 2;

				private const int
					IN_START_OF_3 = 4;

				private const int
					IN_START_OF_4 = 6;

				public TChild First;
				public TChild Fourth;
				public TChild Second;
				public int Size;
				public TChild Third;

				public Digit(TChild one, Lineage lineage)
					: base(one.Measure, lineage, 1)
				{
#if ASSERTS

					one.IsNotNull();
#endif
					First = one;
					Size = 1;
				}

				public Digit(TChild one, TChild two, Lineage lineage)
					: base(one.Measure + two.Measure, lineage, 2)
				{
#if ASSERTS

					one.IsNotNull();
					two.IsNotNull();
#endif
					First = one;
					Second = two;
					Size = 2;
				}

				public Digit(TChild one, TChild two, TChild three, Lineage lineage)
					: base(one.Measure + two.Measure + three.Measure, lineage, 3)
				{
#if ASSERTS
					AssertEx.AreNotNull(one, two, three);

#endif

					First = one;
					Second = two;
					Third = three;
					Size = 3;
				}

				public Digit(TChild one, TChild two, TChild three, TChild four, Lineage lineage)
					: base(one.Measure + two.Measure + three.Measure + four.Measure, lineage, 4)
				{
#if ASSERTS
					AssertEx.AreNotNull(one, two, three, four);

#endif
					First = one;
					Second = two;
					Third = three;
					Fourth = four;
					Size = 4;
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
							throw Errors.Arg_out_of_range("index");
					}
				}

				private Digit()
					: base(0, Common.Lineage.Immutable, 0)
				{
				}

				//This method is used when we don't know the exact size of the digit we want to create.
				public Digit CreateCheckNull(Lineage lineage, TChild item1 = null, TChild item2 = null, TChild item3 = null, TChild item4 = null)
				{
					var items_present = item1 != null ? 1 : 0;
					items_present |= item2 != null ? 2 : 0;
					items_present |= item3 != null ? 4 : 0;
					items_present |= item4 != null ? 8 : 0;
					Digit res = null;
					switch (items_present)
					{
						case 0:
							res = null;
							break;
						case 1 << 0:
							res = MutateOrCreate(item1, lineage);
							break;
						case 1 << 1:
							res = MutateOrCreate(item2, lineage);
							break;
						case 1 << 2:
							res = MutateOrCreate(item3, lineage);
							break;
						case 1 << 3:
							res = MutateOrCreate(item4, lineage);
							break;
						case 1 << 0 | 1 << 1:
							res = MutateOrCreate(item1, item2, lineage);
							break;
						case 1 << 0 | 1 << 1 | 1 << 2:
							if (lineage != null) res = MutateOrCreate(item1, item2, item3, lineage);
							break;
						case 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item2, item3, item4, lineage);
							break;
						case 1 << 1 | 1 << 2:
							res = MutateOrCreate(item2, item3, lineage);
							break;
						case 1 << 1 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item2, item3, item4, lineage);
							break;
						case 1 << 2 | 1 << 3:
							res = MutateOrCreate(item3, item4, lineage);
							break;
						case 1 << 0 | 1 << 2:
							res = MutateOrCreate(item1, item3, lineage);
							break;
						case 1 << 0 | 1 << 2 | 1 << 3:
							res = MutateOrCreate(item1, item3, item4, lineage);
							break;
						case 1 << 1 | 1 << 3:
							res = MutateOrCreate(item2, item4, lineage);
							break;
						case 1 << 0 | 1 << 3: res = MutateOrCreate(item1, item4, lineage);
							break;
						case 1 << 0 | 1 << 1 | 1 << 3:
							res = MutateOrCreate(item1, item2, item4, lineage);
							break;
					}

					return res;
				}

				public static void ReformDigitsForConcat(Lineage lineage, Digit digit, Digit other, out Digit leftmost, out Digit middle, out Digit rightmost)
				{
#if ASSERTS
					digit.IsNotNull();
					other.IsNotNull();
#endif
					var match = digit.Size << 3 | other.Size;

					switch (match)
					{
						case 1 << 3 | 1:
							leftmost = digit.MutateOrCreate(digit.First, other.First, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 2:
							leftmost = digit.MutateOrCreate(digit.First, other.First, other.Second, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 3:
							leftmost = digit.MutateOrCreate(digit.First, other.First, other.Second, other.Third, lineage);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 4:
							leftmost = new Digit(digit.First, other.First, lineage);
							middle = digit.MutateOrCreate(other.Second, other.Third, other.Fourth, lineage);
							rightmost = null;
							return;
						case 2 << 3 | 1:
							leftmost = digit.MutateOrCreate(digit.First, digit.Second, other.First, lineage);
							middle = null;
							rightmost = null;
							return;
						case 2 << 3 | 2:
						case 2 << 3 | 3:
						case 3 << 3 | 2:
						case 3 << 3 | 3:
							leftmost = digit;
							middle = other;
							rightmost = null;
							return;
						case 2 << 3 | 4:
							leftmost = new Digit(digit.First, digit.Second, other.First, lineage);
							middle = digit.MutateOrCreate(other.Second, other.Third, other.Fourth, lineage);
							rightmost = null;
							return;
						case 3 << 3 | 4:
							leftmost = digit;
							middle = new Digit(other.First, other.Second, lineage);
							rightmost = other.MutateOrCreate(other.Third, other.Fourth, lineage);
							return;
						case 3 << 3 | 1:
							leftmost = new Digit(digit.First, digit.Second, lineage);
							middle = digit.MutateOrCreate(digit.Third, other.First, lineage);
							rightmost = null;
							return;
						case 4 << 3 | 1:
							leftmost = new Digit(digit.First, digit.Second, lineage);
							middle = digit.MutateOrCreate(digit.Third, digit.Fourth, other.First, lineage);
							rightmost = null;
							return;
						case 4 << 3 | 2:
						case 4 << 3 | 3:
							leftmost = new Digit(digit.First, digit.Second, lineage);
							middle = digit.MutateOrCreate(digit.Third, digit.Fourth, lineage);
							rightmost = other;
							return;
						case 4 << 3 | 4:
							leftmost = new Digit(digit.First, digit.Second, digit.Third, lineage);
							middle = new Digit(digit.Fourth, other.First, other.Second, lineage);
							rightmost = digit.MutateOrCreate(other.Third, other.Fourth, lineage);
							return;
					}
					//we should've handled all the cases in the Switch statement. Otherwise, produce this error.
					throw ImplErrors.Invalid_execution_path;
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
#if ASSERTS
						index.Is(i => i < Measure);
#endif
						var m_1 = First.Measure;
						if (index < m_1)
							return First[index];
						var m_2 = Second.Measure + m_1;
						if (index < m_2)
							return Second[index - m_1];
						var m_3 = m_2 + Third.Measure;
						if (index < m_3)
							return Third[index - m_2];
						if (index < Measure)
							return Fourth[index - m_3];
						throw ImplErrors.Invalid_digit_size;
					}
				}

				public override bool IsFragment
				{
					get
					{
						return Size == 1;
					}
				}

				public TChild Left
				{
					get
					{
						return First;
					}
				}

				public TChild Right
				{
					get
					{
						switch (Size)
						{
							case 1:
								return First;
							case 2:
								return Second;
							case 3:
								return Third;
							case 4:
								return Fourth;
						}
						throw ImplErrors.Invalid_digit_size;
					}
				
				}


				/// <summary>
				/// This method will re-initialize the current instance with the specified parameters by mutation.
				/// </summary>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Digit _mutate(int measure, int size, TChild a, TChild b = null, TChild c = null, TChild d = null)
				{
					First = a;
					Second = b;
					Third = c;
					Fourth = d;
					Measure = measure;
					Size = size;
					ChildCount = size;
					return this;
				}


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Digit MutateOrCreate(TChild a, Lineage lineage)
				{
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure, 1, a) : new Digit(a, lineage);
				}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Digit MutateOrCreate(TChild a, TChild b, Lineage lineage)
				{
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure + b.Measure, 2, a, b) : new Digit(a, b, lineage);
				}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Digit MutateOrCreate(TChild a, TChild b, TChild c, Lineage lineage)
				{
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure + b.Measure + c.Measure, 3, a, b, c) : new Digit(a, b, c, lineage);
				}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Digit MutateOrCreate(TChild a, TChild b, TChild c, TChild d, Lineage lineage)
				{
					return Lineage.AllowMutation(lineage) ? _mutate(a.Measure + b.Measure + c.Measure + d.Measure, 4, a, b, c, d) : new Digit(a, b, c, d, lineage);
				}

				public Digit AddFirst(TChild item, Lineage lineage)
				{
					
					switch (Size)
					{
						case 1:
							return MutateOrCreate(item, First, lineage);
						case 2:
							return MutateOrCreate(item, First, Second, lineage);
						case 3:
							return MutateOrCreate(item, First, Second, Third, lineage);
					}
					throw ImplErrors.Invalid_digit_size;
				}


				public Digit AddLast(TChild item, Lineage lineage)
				{
					switch (Size)
					{
						case 1:
							return MutateOrCreate(First, item, lineage);
						case 2:
							return MutateOrCreate(First, Second, item, lineage);
						case 3:
							return MutateOrCreate(First, Second, Third, item, lineage);
					}
					throw ImplErrors.Invalid_digit_size;
				}


				public override void Fuse(Digit other, out Digit first, out Digit last, Lineage lineage)
				{
					Digit skip;
					ReformDigitsForConcat(lineage, this, other, out first, out last, out skip);
				}

				public override void Insert(int index, Leaf<TValue> leaf, out Digit leftmost, out Digit rightmost, Lineage lineage)
				{
#if ASSERTS
					leaf.IsNotNull();
#endif

					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild my_leftmost;
					TChild my_rightmost;
					leftmost = null;
					rightmost = null;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_1:
							First.Insert(index, leaf, out my_leftmost, out my_rightmost, lineage);
							if (Size == 4 && my_rightmost != null) {
								var snd = Second;
								leftmost = new Digit(my_leftmost, my_rightmost, Second, lineage);
								rightmost = MutateOrCreate(Third, Fourth, lineage);
								return;
							}
							leftmost = my_rightmost != null
								? CreateCheckNull(lineage, my_leftmost, my_rightmost, Second, Third)
								: CreateCheckNull(lineage, my_leftmost, Second, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							Second.Insert(index - First.Measure, leaf, out my_leftmost, out my_rightmost, lineage);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, my_leftmost, my_rightmost, lineage);
								rightmost = MutateOrCreate(Third, Fourth, lineage);
								return;
							}
							leftmost = my_rightmost != null
								? CreateCheckNull(lineage, First, my_leftmost, my_rightmost, Third)
								: CreateCheckNull(lineage, First, my_leftmost, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							Third.Insert(index - First.Measure - Second.Measure, leaf, out my_leftmost, out my_rightmost, lineage);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, Second, my_leftmost, lineage);
								rightmost = MutateOrCreate(my_rightmost, Fourth, lineage);
								return;
							}
							leftmost =
								my_rightmost != null
									? CreateCheckNull(lineage, First, Second, my_leftmost, my_rightmost)
									: CreateCheckNull(lineage, First, Second, my_leftmost, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							Fourth.Insert(index - Measure + Fourth.Measure, leaf, out my_leftmost, out my_rightmost, lineage);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, Second, Third, lineage);
								rightmost = MutateOrCreate(my_leftmost, my_rightmost, lineage);
								return;
							}
							leftmost = MutateOrCreate(First, Second, Third, my_leftmost, lineage);
							rightmost = null;
							return;
					}

					throw ImplErrors.Invalid_execution_path;
				}

				public override void Iter(Action<Leaf<TValue>> action)
				{
#if ASSERTS
					action.IsNotNull();
#endif
					switch (Size)
					{
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
					}
					throw ImplErrors.Invalid_execution_path;
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
#if ASSERTS
					action.IsNotNull();
#endif
					switch (Size)
					{
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
					}
					throw ImplErrors.Invalid_execution_path;
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> action)
				{
#if ASSERTS
					action.IsNotNull();
#endif
					if (Fourth != null)
					{
						if (!Fourth.IterBackWhile(action)) return false;
					}
					if (Third != null)
					{
						if (!Third.IterBackWhile(action)) return false;
					}
					if (Second != null)
					{
						if (!Second.IterBackWhile(action)) return false;
					}
					return First.IterBackWhile(action);
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> action)
				{
#if ASSERTS
					action.IsNotNull();
#endif

					if (!First.IterWhile(action)) return false;
					if (Second == null) return true;
					if (!Second.IterWhile(action)) return false;
					if (Third == null) return true;
					if (!Third.IterWhile(action)) return false;
					if (Fourth == null) return true;
					return Fourth.IterWhile(action);
				}

				public Digit RemoveFirst(Lineage lineage)
				{
					var new_measure = Measure - First.Measure;
					switch (Size)
					{
						case 1:
							throw ImplErrors.Invalid_digit_size;
						case 2:
							return MutateOrCreate(Second, lineage);
						case 3:
							return MutateOrCreate(Second, Third, lineage);
						case 4:
							return MutateOrCreate(Second, Third, Fourth, lineage);
					}
					throw ImplErrors.Invalid_digit_size;
				}

				public Digit RemoveLast(Lineage lineage)
				{
					switch (Size)
					{
						case 1:
							throw ImplErrors.Invalid_digit_size;
						case 2:
							return MutateOrCreate(First, lineage);
						case 3:
							return MutateOrCreate(First, Second, lineage);
						case 4:
							return MutateOrCreate(First, Second, Third, lineage);
					}
					throw ImplErrors.Invalid_digit_size;
				}

				public override Digit Remove(int index, Lineage lineage)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
#if ASSERTS
					Size.IsNot(1);
#endif
					TChild res;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_1:
							res = First.Remove(index, lineage);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								res.Fuse(Second, out left, out right, lineage);
								return CreateCheckNull(lineage, left, right, Third, Fourth);
							}
							return CreateCheckNull(lineage, res, Second, Third, Fourth);
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							res = Second.Remove(index - First.Measure, lineage);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								First.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, left, right, Third, Fourth);
							}
							return CreateCheckNull(lineage, First, res, Third, Fourth);
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							res = Third.Remove(index - First.Measure - Second.Measure, lineage);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								Second.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, First, left, right, Fourth);
							}
							return CreateCheckNull(lineage, First, Second, res, Fourth);
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							res = Fourth.Remove(index - First.Measure - Second.Measure - Third.Measure, lineage);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								Third.Fuse(res, out left, out right, lineage);
								return CreateCheckNull(lineage, First, Second, left, right);
							}
							return CreateCheckNull(lineage, First, Second, Third, res);
					}
					throw ImplErrors.Invalid_execution_path;
				}

				public override Digit Reverse(Lineage lineage)
				{
					
					switch (Size)
					{
						case 1:
							return MutateOrCreate(First.Reverse(lineage), lineage);
						case 2:
							return MutateOrCreate(Second.Reverse(lineage), First.Reverse(lineage), lineage);
						case 3:
							return MutateOrCreate(Third.Reverse(lineage), Second.Reverse(lineage), First.Reverse(lineage), lineage);
						case 4:
							return MutateOrCreate(Fourth.Reverse(lineage), Third.Reverse(lineage), Second.Reverse(lineage), First.Reverse(lineage), lineage);
					}
					throw ImplErrors.Invalid_execution_path;
				}


				public override void Split(int count, out Digit leftmost, out Digit rightmost, Lineage lineage)
				{
#if ASSERTS
					count.Is(i => i <= Measure);
#endif
					var whereIsThisIndex = WhereIsThisIndex(count);

					TChild split1, split2;
					switch (whereIsThisIndex)
					{
						case IN_START:
							throw ImplErrors.Invalid_execution_path; //The finger tree is supposed to take care of this.
						case IN_MIDDLE_OF_1:
							First.Split(count, out split1, out split2, lineage);
							leftmost = new Digit(split1, lineage);
							rightmost = CreateCheckNull(lineage, split2, Second, Third, Fourth);
							return;
						case IN_START_OF_2:
							leftmost = new Digit(First, lineage);
							rightmost = CreateCheckNull(lineage, Second, Third, Fourth);
							return;
						case IN_MIDDLE_OF_2:
							Second.Split(count - First.Measure, out split1, out split2, lineage);
							leftmost = new Digit(First, split1, lineage);
							rightmost = CreateCheckNull(lineage, split2, Third, Fourth);
							return;
						case IN_START_OF_3:
							leftmost = new Digit(First, Second, lineage);
							rightmost = CreateCheckNull(lineage, Third, Fourth);
							return;
						case IN_MIDDLE_OF_3:
							Third.Split(count - First.Measure - Second.Measure, out split1, out split2, lineage);
							leftmost = new Digit(First, Second, split1, lineage);
							rightmost = CreateCheckNull(lineage, split2, Fourth);
							return;
						case IN_START_OF_4:
							leftmost = new Digit(First, Second, Third, lineage);
							rightmost = CreateCheckNull(lineage, Fourth);
							return;
						case IN_MIDDLE_OF_4:
							Fourth.Split(count - Measure + Fourth.Measure, out split1, out split2, lineage);
							leftmost = new Digit(First, Second, Third, split1, lineage);
							rightmost = CreateCheckNull(lineage, split2);
							return;
						case IN_END:
							leftmost = this;
							rightmost = null;
							return;
					}
					throw ImplErrors.Invalid_execution_path;
				}

				public override Digit Update(int index, Leaf<TValue> leaf, Lineage lineage)
				{
					if (Lineage.AllowMutation(lineage)) return Update_MUTATES(index, leaf);
#if ASSERTS
					leaf.IsNotNull();
#endif
					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild res;
					switch (whereIsThisIndex)
					{
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
					}
					throw ImplErrors.Invalid_execution_path;
				}

				private Digit Update_MUTATES(int index, Leaf<TValue> leaf)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
			
					switch (whereIsThisIndex)
					{
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
					}
					throw ImplErrors.Invalid_execution_path;
				}

				/// <summary>
				///   Returns a code telling where is the index located
				/// </summary>
				/// <param name="index"> </param>
				/// <returns> </returns>
				private int WhereIsThisIndex(int index)
				{
#if ASSERTS
					index.Is(i => i < Measure);
#endif
					var measure_1 = First.Measure;
					if (index == 0) return 0;
					if (index < measure_1) return 1;
					if (index == measure_1) return 2;
					var measure_2 = measure_1 + Second.Measure;
					if (index < measure_2) return 3;
					if (index == measure_2) return 4;
					var measure_3 = measure_2 + Third.Measure;
					if (index < measure_3) return 5;
					if (index == measure_3) return 6;
					var measure_4 = measure_3 + Fourth.Measure;
					if (index < measure_4) return 7;
					if (index == measure_4) return 8;
					throw ImplErrors.Invalid_execution_path;
				}
			}
		}
	}
}