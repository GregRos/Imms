using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Solid.Common;

namespace Solid
{
	static partial class FingerTree<TValue>
	{
		abstract partial class FTree<TChild>
		{
			internal sealed class Digit : Measured<Digit>
			{
				internal class DigitEnumerator : IReusableEnumerator<Digit>
				{
					private int _childNumber = -1;
					private Digit _digit;
					private readonly bool _forward;
					private readonly IReusableEnumerator<TChild> _inner;

					public DigitEnumerator(bool forward, Digit target)
					{
						_forward = forward;
						_digit = target;
						_inner = _digit.First.GetEnumerator(_forward);
					}

					public Leaf<TValue> Current
					{
						get
						{
#if DEBUG
							_inner.IsNotNull();
#endif
							return _inner.Current;
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
						if (_childNumber != -1 && _inner.MoveNext())
							return true;
						_childNumber++;
						if (_childNumber >= _digit.Size)
							return false;
						var fixedNum = _forward ? _childNumber : _digit.Size - 1 - _childNumber;
						switch (fixedNum)
						{
							case 0:
								_inner.Retarget(_digit.First);
								return MoveNext();
							case 1:
								_inner.Retarget(_digit.Second);
								return MoveNext();
							case 2:
								_inner.Retarget(_digit.Third);
								return MoveNext();
							case 3:
								_inner.Retarget(_digit.Fourth);
								return MoveNext();
						}
						throw Errors.Invalid_execution_path;
					}

					public void Reset()
					{
						_childNumber = -1;
					}

					public void Retarget(Digit next)
					{
						_digit = next;
						Reset();
					}
				}

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

				public readonly TChild First;
				public readonly TChild Fourth;
				public readonly TChild Second;
				public readonly int Size;
				public readonly TChild Third;

				//In the following constructors, we take the measure as a parameter (instead of calculating it ourselves) for performance reasons.
				//Almost always the external code can find the measure using fewer operations (that is, if it's not available from the beginning).
				//The overhead associated with calculating and accessing the measure can bottleneck the data structure.
				public Digit(TChild one)
					: base(one.Measure)
				{
#if DEBUG

					one.IsNotNull();
#endif
					First = one;
					Size = 1;
				}

				public Digit(TChild one, TChild two)
					: base(one.Measure + two.Measure)
				{
#if DEBUG

					one.IsNotNull();
					two.IsNotNull();
#endif
					First = one;
					Second = two;
					Size = 2;
				}

				public Digit(TChild one, TChild two, TChild three)
					: base(one.Measure + two.Measure + three.Measure)
				{
#if DEBUG
					AssertEx.AreNotNull(one, two, three);

#endif

					First = one;
					Second = two;
					Third = three;
					Size = 3;
				}

				public Digit(TChild one, TChild two, TChild three, TChild four)
					: base(one.Measure + two.Measure + three.Measure + four.Measure)
				{
#if DEBUG
					AssertEx.AreNotNull(one, two, three, four);

#endif
					First = one;
					Second = two;
					Third = three;
					Fourth = four;
					Size = 4;
				}

				private Digit()
					: base(0)
				{
				}

				//This method is used when we don't know the exact size of the digit we want to create.
				public static Digit CreateCheckNull(TChild item1 = null, TChild item2 = null, TChild item3 = null, TChild item4 = null)
				{
					var items_present = item1 != null ? 1 : 0;
					items_present |= item2 != null ? 2 : 0;
					items_present |= item3 != null ? 4 : 0;
					items_present |= item4 != null ? 8 : 0;

					switch (items_present)
					{
						case 0:
							return null;
						case 1 << 0:
							return new Digit(item1);
						case 1 << 1:
							return new Digit(item2);
						case 1 << 2:
							return new Digit(item3);
						case 1 << 3:
							return new Digit(item4);
						case 1 << 0 | 1 << 1:
							return new Digit(item1, item2);
						case 1 << 0 | 1 << 1 | 1 << 2:
							return new Digit(item1, item2, item3);
						case 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3:
							return new Digit(item1, item2, item3, item4);
						case 1 << 1 | 1 << 2:
							return new Digit(item2, item3);
						case 1 << 1 | 1 << 2 | 1 << 3:
							return new Digit(item2, item3, item4);
						case 1 << 2 | 1 << 3:
							return new Digit(item3, item4);
						case 1 << 0 | 1 << 2:
							return new Digit(item1, item3);
						case 1 << 0 | 1 << 2 | 1 << 3:
							return new Digit(item1, item3, item4);
						case 1 << 1 | 1 << 3:
							return new Digit(item2, item4);
						case 1 << 0 | 1 << 3:
							return new Digit(item1, item4);
						case 1 << 0 | 1 << 1 | 1 << 3:
							return new Digit(item1, item2, item4);
					}
					throw Errors.Invalid_execution_path;
				}

				public static void ReformDigitsForConcat(Digit digit, Digit other, out Digit leftmost, out Digit middle,
				                                         out Digit rightmost)
				{
#if DEBUG
					digit.IsNotNull();
#endif
#if DEBUG
					other.IsNotNull();
#endif
					var sizePermutation = digit.Size << 3 | other.Size;

					switch (sizePermutation)
					{
						case 1 << 3 | 1:
							leftmost = new Digit(digit.First, other.First);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 2:
							leftmost = new Digit(digit.First, other.First, other.Second);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 3:
							leftmost = new Digit(digit.First, other.First, other.Second, other.Third);
							rightmost = null;
							middle = null;
							return;
						case 1 << 3 | 4:
							leftmost = new Digit(digit.First, other.First);
							middle = new Digit(other.Second, other.Third, other.Fourth);
							rightmost = null;
							return;
						case 2 << 3 | 1:
							leftmost = new Digit(digit.First, digit.Second, other.First);
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
							leftmost = new Digit(digit.First, digit.Second, other.First);
							middle = new Digit(other.Second, other.Third, other.Fourth);
							rightmost = null;
							return;
						case 3 << 3 | 4:
							leftmost = digit;
							middle = new Digit(other.First, other.Second);
							rightmost = new Digit(other.Third, other.Fourth);
							return;
						case 3 << 3 | 1:
							leftmost = new Digit(digit.First, digit.Second);
							middle = new Digit(digit.Third, other.First);
							rightmost = null;
							return;
						case 4 << 3 | 1:
							leftmost = new Digit(digit.First, digit.Second);
							middle = new Digit(digit.Third, digit.Fourth, other.First);
							rightmost = null;
							return;
						case 4 << 3 | 2:
						case 4 << 3 | 3:
							leftmost = new Digit(digit.First, digit.Second);
							middle = new Digit(digit.Third, digit.Fourth);
							rightmost = other;
							return;
						case 4 << 3 | 4:
							leftmost = new Digit(digit.First, digit.Second, digit.Third);
							middle = new Digit(digit.Fourth, other.First, other.Second);
							rightmost = new Digit(other.Third, other.Fourth);
							return;
					}
					throw Errors.Invalid_execution_path;
				}

				public override Leaf<TValue> this[int index]
				{
					get
					{
#if DEBUG
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
						throw Errors.Invalid_digit_size;
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
						throw Errors.Invalid_digit_size;
					}
				}

				public Digit AddLeft(TChild item)
				{
					switch (Size)
					{
						case 1:
							return new Digit(item, First);
						case 2:
							return new Digit(item, First, Second);
						case 3:
							return new Digit(item, First, Second, Third);
					}
					throw Errors.Invalid_digit_size;
				}

				public void AddLeftSplit(TChild item, out Digit leftmost, out Digit rightmost)
				{
					leftmost = new Digit(item, First);
					rightmost = new Digit(Second, Third, Fourth);
				}

				public Digit AddRight(TChild item)
				{
					switch (Size)
					{
						case 1:
							return new Digit(First, item);
						case 2:
							return new Digit(First, Second, item);
						case 3:
							return new Digit(First, Second, Third, item);
					}
					throw Errors.Invalid_digit_size;
				}

				public void AddRightSplit(TChild item, out Digit leftmost, out Digit rightmost)
				{
					leftmost = new Digit(First, Second, Third);
					rightmost = new Digit(Fourth, item);
				}

				public override void Fuse(Digit other, out Digit first, out Digit last)
				{
					Digit skip;
					ReformDigitsForConcat(this, other, out first, out last, out skip);
				}

				public override IReusableEnumerator<Digit> GetEnumerator(bool x)
				{
					return new DigitEnumerator(x, this);
				}

				public override void Insert(int index, Leaf<TValue> leaf, out Digit leftmost, out Digit rightmost)
				{
#if DEBUG
					leaf.IsNotNull();
#endif

					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild my_leftmost;
					TChild my_rightmost;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_1:
							First.Insert(index, leaf, out my_leftmost, out my_rightmost);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(my_leftmost, my_rightmost, Second);
								rightmost = new Digit(Third, Fourth);
								return;
							}
							leftmost = my_rightmost != null
								? CreateCheckNull(my_leftmost, my_rightmost, Second, Third)
								: CreateCheckNull(my_leftmost, Second, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							Second.Insert(index - First.Measure, leaf, out my_leftmost, out my_rightmost);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, my_leftmost, my_rightmost);
								rightmost = new Digit(Third, Fourth);
								return;
							}
							leftmost = my_rightmost != null
								? CreateCheckNull(First, my_leftmost, my_rightmost, Third)
								: CreateCheckNull(First, my_leftmost, Third, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							Third.Insert(index - First.Measure - Second.Measure, leaf, out my_leftmost, out my_rightmost);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, Second, my_leftmost);
								rightmost = new Digit(my_rightmost, Fourth);
								return;
							}
							leftmost =
								my_rightmost != null
									? CreateCheckNull(First, Second, my_leftmost, my_rightmost)
									: CreateCheckNull(First, Second, my_leftmost, Fourth);
							rightmost = null;
							return;
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							Fourth.Insert(index - Measure + Fourth.Measure, leaf, out my_leftmost, out my_rightmost);
							if (Size == 4 && my_rightmost != null)
							{
								leftmost = new Digit(First, Second, Third);
								rightmost = new Digit(my_leftmost, my_rightmost);
								return;
							}
							leftmost = new Digit(First, Second, Third, my_leftmost);
							rightmost = null;
							return;
					}
					throw Errors.Invalid_execution_path;
				}

				public override void Iter(Action<Leaf<TValue>> action)
				{
#if DEBUG
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
					throw Errors.Invalid_execution_path;
				}

				public override void IterBack(Action<Leaf<TValue>> action)
				{
#if DEBUG
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
					throw Errors.Invalid_execution_path;
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> action)
				{
#if DEBUG
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
#if DEBUG
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

				public Digit PopLeft()
				{
					var new_measure = Measure - First.Measure;
					switch (Size)
					{
						case 1:
							throw Errors.Invalid_digit_size;
						case 2:
							return new Digit(Second);
						case 3:
							return new Digit(Second, Third);
						case 4:
							return new Digit(Second, Third, Fourth);
					}
					throw Errors.Invalid_digit_size;
				}

				public Digit PopRight()
				{
					switch (Size)
					{
						case 1:
							throw Errors.Invalid_digit_size;
						case 2:
							return new Digit(First);
						case 3:
							return new Digit(First, Second);
						case 4:
							return new Digit(First, Second, Third);
					}
					throw Errors.Invalid_digit_size;
				}

				public override Digit Remove(int index)
				{
					var whereIsThisIndex = WhereIsThisIndex(index);
#if DEBUG
					Size.IsNot(1);
#endif
					TChild res;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_1:
							res = First.Remove(index);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								res.Fuse(Second, out left, out right);
								return CreateCheckNull(left, right, Third, Fourth);
							}
							return CreateCheckNull(res, Second, Third, Fourth);
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							res = Second.Remove(index - First.Measure);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								First.Fuse(res, out left, out right);
								return CreateCheckNull(left, right, Third, Fourth);
							}
							return CreateCheckNull(First, res, Third, Fourth);
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							res = Third.Remove(index - First.Measure - Second.Measure);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								Second.Fuse(res, out left, out right);
								return CreateCheckNull(First, left, right, Fourth);
							}
							return CreateCheckNull(First, Second, res, Fourth);
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							res = Fourth.Remove(index - First.Measure - Second.Measure - Third.Measure);
							if (res != null && res.IsFragment)
							{
								TChild left, right;
								Third.Fuse(res, out left, out right);
								return CreateCheckNull(First, Second, left, right);
							}
							return CreateCheckNull(First, Second, Third, res);
					}
					throw Errors.Invalid_execution_path;
				}

				public override Digit Reverse()
				{
					switch (Size)
					{
						case 1:
							return new Digit(First.Reverse());
						case 2:
							return new Digit(Second.Reverse(), First.Reverse());
						case 3:
							return new Digit(Third.Reverse(), Second.Reverse(), First.Reverse());
						case 4:
							return new Digit(Fourth.Reverse(), Third.Reverse(), Second.Reverse(), First.Reverse());
					}
					throw Errors.Invalid_execution_path;
				}

				public override Digit Set(int index, Leaf<TValue> leaf)
				{
#if DEBUG
					leaf.IsNotNull();
#endif
					var whereIsThisIndex = WhereIsThisIndex(index);
					TChild res;
					switch (whereIsThisIndex)
					{
						case IN_START:
						case IN_MIDDLE_OF_1:
							res = First.Set(index, leaf);
							return CreateCheckNull(res, Second, Third, Fourth);
						case IN_START_OF_2:
						case IN_MIDDLE_OF_2:
							res = Second.Set(index - First.Measure, leaf);
							return CreateCheckNull(First, res, Third, Fourth);
						case IN_START_OF_3:
						case IN_MIDDLE_OF_3:
							res = Third.Set(index - First.Measure - Second.Measure, leaf);
							return CreateCheckNull(First, Second, res, Fourth);
						case IN_START_OF_4:
						case IN_MIDDLE_OF_4:
							res = Fourth.Set(index - First.Measure - Second.Measure - Third.Measure, leaf);
							return CreateCheckNull(First, Second, Third, res);
					}
					throw Errors.Invalid_execution_path;
				}

				public override void Split(int index, out Digit leftmost, out Digit rightmost)
				{
#if DEBUG
					index.Is(i => i < Measure);
#endif
					var whereIsThisIndex = WhereIsThisIndex(index);

					TChild split1, split2;
					switch (whereIsThisIndex)
					{
						case IN_START:
							throw Errors.Invalid_execution_path; //The finger tree is supposed to take care of this.
						case IN_MIDDLE_OF_1:
							First.Split(index, out split1, out split2);
							leftmost = new Digit(split1);
							rightmost = CreateCheckNull(split2, Second, Third, Fourth);
							return;
						case IN_START_OF_2:
							leftmost = new Digit(First);
							rightmost = CreateCheckNull(Second, Third, Fourth);
							return;
						case IN_MIDDLE_OF_2:
							Second.Split(index - First.Measure, out split1, out split2);
							leftmost = new Digit(First, split1);
							rightmost = CreateCheckNull(split2, Third, Fourth);
							return;
						case IN_START_OF_3:
							leftmost = new Digit(First, Second);
							rightmost = CreateCheckNull(Third, Fourth);
							return;
						case IN_MIDDLE_OF_3:
							Third.Split(index - First.Measure - Second.Measure, out split1, out split2);
							leftmost = new Digit(First, Second, split1);
							rightmost = CreateCheckNull(split2, Fourth);
							return;
						case IN_START_OF_4:
							leftmost = new Digit(First, Second, Third);
							rightmost = CreateCheckNull(Fourth);
							return;
						case IN_MIDDLE_OF_4:
							Fourth.Split(index - Measure + Fourth.Measure, out split1, out split2);
							leftmost = new Digit(First, Second, Third, split1);
							rightmost = CreateCheckNull(split2);
							return;
						case IN_END:
							leftmost = this;
							rightmost = null;
							return;
					}
					throw Errors.Invalid_execution_path;
				}

				/// <summary>
				///   Returns a code telling where is the index located
				/// </summary>
				/// <param name="index"> </param>
				/// <returns> </returns>
				private int WhereIsThisIndex(int index)
				{
#if DEBUG
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
					throw Errors.Invalid_execution_path;
				}
			}
		}
	}
}