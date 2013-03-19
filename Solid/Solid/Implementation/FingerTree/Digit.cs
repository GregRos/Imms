using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Solid.Common;
using Solid.FingerTree.Iteration;
using Errors = Solid.Common.Errors;
namespace Solid.FingerTree
{
	internal sealed class Digit<T> : Measured
		where T : Measured
	{
		public readonly T First;
		public readonly T Fourth;
		public readonly T Second;
		public readonly int Size;
		public readonly T Third;

		private Digit() : base(0)
		{
		}

		//In the following constructors, we take the measure as a parameter (instead of calculating it ourselves) for performance reasons.
		//Almost always the external code can find the measure using fewer operations (that is, if it's not available from the beginning).
		//The overhead associated with calculating and accessing the measure can bottleneck the data structure.
		public Digit(T one, int measure)
			: base(measure)
		{
			measure.Is(one.Measure);
			one.IsNotNull();
			First = one;
			Size = 1;
		}

		public Digit(T one, T two, int measure)
			: base(measure)
		{
			measure.Is(one.Measure + two.Measure);
			one.IsNotNull();
			two.IsNotNull();
			First = one;
			Second = two;
			Size = 2;
		}

		public Digit(T one, T two, T three, int measure)
			: base(measure)
		{

			AssertEx.AreNotNull(one, two, three);
			measure.Is(one.Measure + two.Measure + three.Measure);
			
			First = one;
			Second = two;
			Third = three;
			Size = 3;
		}
		
		public override U Reverse<U>()
		{
			return this.ReverseDigit() as U;
		}

		public Digit<T> ReverseDigit()
		{
			switch (Size)
			{
				case 1:
					return new Digit<T>(First.Reverse<T>(),Measure);
				case 2:
					return new Digit<T>(Second.Reverse<T>(), First.Reverse<T>(), Measure);
				case 3:
					return new Digit<T>(Third.Reverse<T>(), Second.Reverse<T>(), First.Reverse<T>(), Measure);
				case 4:
					return new Digit<T>(Fourth.Reverse<T>(), Third.Reverse<T>(), Second.Reverse<T>(), First.Reverse<T>(), Measure);
				default:
					throw Errors.Invalid_execution_path;
			}
		}

		public Digit(T one, T two, T three, T four, int measure)
			: base(measure)
		{
			AssertEx.AreNotNull(one, two, three, four);
			measure.Is(one.Measure + two.Measure + three.Measure + four.Measure);
			First = one;
			Second = two;
			Third = three;
			Fourth = four;
			Size = 4;
		}

		public T Left
		{
			get { return First; }
		}

		public T Right
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
					default:
						throw Errors.Invalid_digit_size;
				}
			}
		}

		public override Measured this[int index]
		{
			get
			{
				index.Is(i => i < Measure);
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

		public int SplitWhere(int index)
		{
			index.Is(i => i < Measure);
			int measure_1 = First.Measure;
			if (index == 0) return 0;
			if (index < measure_1) return 1;
			if (index == measure_1) return 2;
			int measure_2 = measure_1 + Second.Measure;
			if (index < measure_2) return 3;
			if (index == measure_2) return 4;
			int measure_3 = measure_2 + Third.Measure;
			if (index < measure_3) return 5;
			if (index == measure_3) return 6;
			int measure_4 = measure_3 + Fourth.Measure;
			if (index < measure_4) return 7;
			if (index == measure_4) return 8;
			throw Errors.Invalid_execution_path;
		}

		public T ChildbyIndex(int index)
		{
			if (index >= Size) throw Common.Errors.Index_out_of_range;
			switch (index)
			{
				case 0:
					return First;
				case 1:
					return Second;
				case 2:
					return Third;
				case 3:
					return Fourth;
				default:
					throw Errors.Invalid_execution_path;

			}
		}
	
		public static Digit<T> CreateCheckNull(int measure, T item1 = null, T item2 = null, T item3 = null, T item4 = null)
		{
			//+ Implementation
			//This method is essentially another constructor that decides the digit to create by performing checks.
			//It assumes that some of the digits can be null, but that these nulls must be in sequence. E.g.
			// NULL, NULL, DIGIT, DIGIT
			// NULL, DIGIT, NULL, NULL
			//But it does not expect the sequence
			// NULL DIGIT NULL DIGIT
			// For example. Note that this method could have been the only constructor, but I decided to only perform these checks
			// Where absolutely necessary. 
			int items_present = item1 != null ? 1 : 0;
			items_present |= item2 != null ? 2 : 0;
			items_present |= item3 != null ? 4 : 0;
			items_present |= item4 != null ? 8 : 0;

			switch (items_present)
			{
					
				case 0:
					return null;
				case 1 << 0:
					return new Digit<T>(item1, measure);
				case 1 << 1:
					return new Digit<T>(item2, measure);
				case 1 << 2:
					return new Digit<T>(item3, measure);
				case 1 << 3:
					return new Digit<T>(item4, measure);
				case 1 << 0 | 1 << 1:
					return new Digit<T>(item1, item2, measure);
				case 1 << 0 | 1 << 1 | 1 << 2:
					return new Digit<T>(item1, item2, item3, measure);
				case 1 << 0 | 1 << 1 |1 << 2 | 1 << 3:
					return new Digit<T>(item1, item2, item3, item4, measure);
				case 1 << 1 | 1 << 2:
					return new Digit<T>(item2, item3, measure);
				case 1 << 1 | 1 << 2 | 1 << 3:
					return new Digit<T>(item2, item3, item4, measure);
				case 1 << 2 | 1 << 3:
					return new Digit<T>(item3, item4, measure);
				default:
					throw Errors.Invalid_execution_path;
			}
		}

		public override void IterBack(Action<Measured> action)
		{
			action.IsNotNull();
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
				default:
					throw Errors.Invalid_execution_path;
			}
		}
		
		public override IEnumerator<Measured> GetEnumerator()
		{
			return new DigitEnumerator<T>(this);
		}

		
		public override void Split<TObject>(int index, out TObject leftmost, out TObject rightmost)
		{
			
			
			index.Is(i => i < Measure);
			int caseCode = SplitWhere(index);

			T split1, split2;
			switch (caseCode)
			{
				case 1:
					First.Split(index, out split1, out split2);
					leftmost = new Digit<T>(split1, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, split2, Second, Third, Fourth) as TObject;
					return;
				case 2:
					leftmost = new Digit<T>(First, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, Second, Third, Fourth) as TObject;
					return;
				case 3:
					Second.Split(index - First.Measure, out split1, out split2);
					leftmost = new Digit<T>(First, split1, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, split2, Third, Fourth) as TObject;
					return;
				case 4:
					leftmost = new Digit<T>(First, Second, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, Third, Fourth) as TObject;
					return;
				case 5:
					Third.Split(index - First.Measure - Second.Measure, out split1, out split2);
					leftmost = new Digit<T>(First, Second, split1, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, split2, Fourth) as TObject;
					return;
				case 6:
					leftmost = new Digit<T>(First, Second, Third, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, Fourth) as TObject;
					return;
				case 7:
					Fourth.Split(index - Measure + Fourth.Measure, out split1, out split2);
					leftmost = new Digit<T>(First, Second, Third, split1, index) as TObject;
					rightmost = CreateCheckNull(Measure - index, split2) as TObject;
					return;
				case 8:
					leftmost = this as TObject;
					rightmost = null;
					return;
			}
			throw Errors.Invalid_execution_path;
		}

		public override void Iter(Action<Measured> action)
		{
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
				default:
					throw Errors.Invalid_execution_path;
			}
		}

		public override TObject Set<TObject>(int index, object value)
		{
			var code = SplitWhere(index);
			T res;
			switch (code)
			{
				case 0:
				case 1:
					res = First.Set<T>(index, value);
					return CreateCheckNull(Measure, res, Second, Third, Fourth) as TObject;
				case 2:
				case 3:
					res = Second.Set<T>(index - First.Measure, value);
					return CreateCheckNull(Measure, First, res, Third, Fourth) as TObject;
				case 4:
				case 5:
					res = Third.Set<T>(index - First.Measure - Second.Measure, value);
					return CreateCheckNull(Measure, First, Second, res, Fourth) as TObject;
				case 6:
				case 7:
					res = Fourth.Set<T>(index - First.Measure - Second.Measure - Third.Measure, value);
					return CreateCheckNull(Measure, First, Second, Third, res) as TObject;
				default:
					throw Errors.Invalid_execution_path;
			}
		}

		/* Here is an ASCII diagram for what the next function does.
		 * X, X      => XX
		 * X, XX     => XXX
		 * X, XXX    => XX XX
		 * X, XXXX   => XXX XX
		 * XX X      => XXX
		 * XX XX       stays
		 * XX XXX      stays
		 * XX XXXX   => XXX XXX
		 * XXX X     => XX XX
		 * XXX XX       stays
		 * XXX XXX      stays
		 * XXX XXXX  => XX XX XXX
		 * XXXX X    => XX XXX
		 * XXXX XX   => XXX XXX
		 * XXXX XXX  => XXX XX XX
		 * XXXX XXXX => XXX XXX XX
		 * The function returns up to digits. Those that it doesn't return are null.
		 */
		public static void ReformDigitsForConcat(Digit<T> digit, Digit<T> other, out Digit<T> leftmost, out Digit<T> middle, out Digit<T> rightmost)
		{
			digit.IsNotNull();
			other.IsNotNull();
			int sizeCode = digit.Size << 3 | other.Size;
			int newMeasure = digit.Measure + other.Measure;
			

			switch (sizeCode)
			{
				case 1 << 3 | 1:
					leftmost = new Digit<T>(digit.First, other.First, newMeasure);
					rightmost = null;
					middle = null;
					break;
				case 1 << 3 | 2:
					leftmost = new Digit<T>(digit.First, other.First, other.Second, newMeasure);
					rightmost = null;
					middle = null;
					break;
				case 1 << 3 | 3:
					leftmost = new Digit<T>(digit.First, other.First, other.Second, other.Third, newMeasure);
					rightmost = null;
					middle = null;
					break;
				case 1 << 3 | 4:
					leftmost = new Digit<T>(digit.First, other.First, digit.First.Measure + other.First.Measure);
					middle = new Digit<T>(other.Second, other.Third, other.Fourth, other.Measure - other.First.Measure);
					rightmost = null;
					break;
				case 2 << 3 | 1:
					leftmost = new Digit<T>(digit.First, digit.Second, other.First, newMeasure);
					middle = null;
					rightmost = null;
					break;
				case 2 << 3 | 2:
				case 2 << 3 | 3:
				case 3 << 3 | 2:
				case 3 << 3 | 3:
					leftmost = digit;
					middle = other;
					rightmost = null;
					break;
				case 2 << 3 | 4:
					leftmost = new Digit<T>(digit.First, digit.Second, other.First, digit.Measure + other.First.Measure);
					middle = new Digit<T>(other.Second, other.Third, other.Fourth, other.Measure - other.First.Measure);
					rightmost = null;
					break;
				case 3 << 3 | 4:
					leftmost = digit;
					middle = new Digit<T>(other.First, other.Second, other.First.Measure + other.Second.Measure);
					rightmost = new Digit<T>(other.Third, other.Fourth, other.Third.Measure + other.Fourth.Measure);
					break;
				case 3 << 3 | 1:
					leftmost = new Digit<T>(digit.First, digit.Second, digit.Measure - digit.Third.Measure);
					middle = new Digit<T>(digit.Third, other.First, digit.Third.Measure + other.Measure);
					rightmost = null;
					break;
				case 4 << 3 | 1:
					leftmost = new Digit<T>(digit.First, digit.Second, digit.First.Measure + digit.Second.Measure);
					middle = new Digit<T>(digit.Third, digit.Fourth, other.First, other.Measure + digit.Third.Measure + digit.Fourth.Measure);
					rightmost = null;
					break;
				case 4 << 3 | 2:
				case 4 << 3 | 3:
					leftmost = new Digit<T>(digit.First, digit.Second, digit.First.Measure + digit.Second.Measure);
					middle = new Digit<T>(digit.Third, digit.Fourth, digit.Third.Measure + digit.Fourth.Measure);
					rightmost = other;
					break;
				case 4 << 3 | 4:
					leftmost = new Digit<T>(digit.First, digit.Second, digit.Third, digit.Measure - digit.Fourth.Measure);
					middle = new Digit<T>(digit.Fourth, other.First, other.Second, digit.Fourth.Measure + other.Second.Measure + other.First.Measure);
					rightmost = new Digit<T>(other.Third, other.Fourth, other.Third.Measure + other.Fourth.Measure);
					break;
				default:
					throw Errors.Invalid_execution_path;
			}
		}
	
		public Digit<T> AddRight(T item)
		{
			int new_measure = Measure + item.Measure;
			switch (Size)
			{
				case 1:
					return new Digit<T>(First, item, new_measure);
				case 2:
					return new Digit<T>(First, Second, item, new_measure);
				case 3:
					return new Digit<T>(First, Second, Third, item, new_measure);
				default:
					throw Errors.Invalid_digit_size;
			}
		}

		public Digit<T> AddLeft(T item)
		{
			int new_measure = Measure + item.Measure;
			switch (Size)
			{
				case 1:
					return new Digit<T>(item, First, new_measure);
				case 2:
					return new Digit<T>(item, First, Second, new_measure);
				case 3:
					return new Digit<T>(item, First, Second, Third, new_measure);
				default:
					throw Errors.Invalid_digit_size;
			}
		}
	
		public Digit<T> PopLeft()
		{
			int new_measure = Measure - First.Measure;
			switch (Size)
			{
				case 1:
					throw Errors.Invalid_digit_size;
				case 2:
					return new Digit<T>(Second, new_measure);
				case 3:
					return new Digit<T>(Second, Third, new_measure);
				case 4:
					return new Digit<T>(Second, Third, Fourth, new_measure);
				default:
					throw Errors.Invalid_digit_size;
			}
		}
	
		public Digit<T> PopRight()
		{
			switch (Size)
			{
				case 1:
					throw Errors.Invalid_digit_size;
				case 2:
					return new Digit<T>(First, First.Measure);
				case 3:
					return new Digit<T>(First, Second, Measure - Third.Measure);
				case 4:
					return new Digit<T>(First, Second, Third, Measure - Fourth.Measure);
				default:
					throw Errors.Invalid_digit_size;
			}
		}

		public void AddLeftSplit(T item, out Digit<T> leftmost, out Digit<T> rightmost)
		{
			leftmost = new Digit<T>(item, First, First.Measure + item.Measure);
			rightmost = new Digit<T>(Second, Third, Fourth, Measure - First.Measure);
		}
	
		public void AddRightSplit(T item, out Digit<T> leftmost, out Digit<T> rightmost)
		{
			leftmost = new Digit<T>(First, Second, Third, Measure - Fourth.Measure);
			rightmost = new Digit<T>(Fourth, item, Fourth.Measure + item.Measure);
		}
	}
}