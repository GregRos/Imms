using System;
using System.Collections.Generic;

namespace Solid.FingerTree
{
	//+ Implementation
	//  As the FTree class is internal and all its children are sealed, there are three types of FTrees.
	//  The Empty tree in Empty.cs (self evident)
	//  The Single<T>, a tree with one digit.
	//  The Compound<T>, a tree with two digits and a Deep node. 
	//  These have the same purpose as outlined in the referenced paper.
	internal abstract class FTree<T>
		where T : Measured<T>
	{
		public static readonly FTree<T> Empty = Empty<T>.Instance;

		//+ Implementation
		//  These two are readonly fields due to performance reasons.
		//  While the CLR is normally supposed to inline property calls, in order to allow all types of FTrees to access
		//  These properties, they must be abstract or have a concrete implementation.
		//  In the first case, the properties cannot be inlined, and furthermore, using them involves extra indirection
		//  Due to virtual method table lookups. 
		//  The second option would require having the backing field in the FTree class anyway, and performs noticeably worse.

		public readonly int Kind;
		public readonly int Measure;

		protected FTree(int measure, int kind)
		{
			Kind = kind;
			Measure = measure;
		}

		//+ Implementation
		//  Extra performance may be squeezed out by transforming the Left and Right properties,
		//  Which get the left and right values of the FTree
		//  Into readonly fields, as outlined above.


		public abstract T Left
		{
			get;
		}

		public abstract T Right
		{
			get;
		}

		public abstract FTree<T> AddLeft(T item);

		public abstract FTree<T> AddRight(T item);

		public abstract FTree<T> DropLeft();

		public abstract FTree<T> DropRight();

		public abstract bool IterBackWhile(Func<Measured, bool> func);

		public abstract bool IterWhile(Func<Measured, bool> func);

		public abstract Measured Get(int index);

		public abstract bool IsFragment { get; }

		public abstract FTree<T> Remove(int index);

		public abstract IEnumerator<Measured> GetEnumerator();

		public abstract FTree<T> Insert(int index, Measured measured);

		public abstract void Iter(Action<Measured> action);

		public abstract void IterBack(Action<Measured> action);

		public abstract FTree<T> Reverse();

		public abstract FTree<T> Set(int index, Measured value);

		public abstract void Split(int count, out FTree<T> leftmost, out FTree<T> rightmost);

		public static FTree<T> Concat(FTree<T> first, FTree<T> last)
		{
			int status = first.Kind << 3 | last.Kind;
			int measure = first.Measure + last.Measure;
			FTree<Digit<T>> newDeep;
			switch (status)
			{
					//+ Implementation
					//This should be farily legible. It is a solution I like to call a 'case table'
					//Note that TreeType is *not* an enum but a static class with constants.
					//This is because enums do not support the bitwise << operator.


					/* If either of the trees is empty*/
				case TreeType.Empty << 3 | TreeType.Single:
				case TreeType.Empty << 3 | TreeType.Compound:
					return last;
				case TreeType.Single << 3 | TreeType.Empty:
				case TreeType.Compound << 3 | TreeType.Empty:
					return first;
				case TreeType.Empty << 3 | TreeType.Empty:
					return first;
					/* If both are single... we just create a new Compound with their digits.*/
				case TreeType.Single << 3 | TreeType.Single:
					var single1 = first as Single<T>;
					var single2 = last as Single<T>;
					return new Compound<T>(measure, single1.CenterDigit, Empty<Digit<T>>.Instance,
					                       single2.CenterDigit);
				case TreeType.Single << 3 | TreeType.Compound:
					var asSingle = first as Single<T>;
					var asCompound = last as Compound<T>;
					newDeep = asCompound.DeepTree.AddLeft(asCompound.LeftDigit);
					return new Compound<T>(measure,
					                       asSingle.CenterDigit, newDeep, asCompound.RightDigit);
					/* If one is single, we push the digit of the Compound into its Deep.*/
				case TreeType.Compound << 3 | TreeType.Single:
					var right_single = last as Single<T>;
					var left_compound = first as Compound<T>;

					newDeep = left_compound.DeepTree.AddRight(left_compound.RightDigit);
					return new Compound<T>(measure, left_compound.LeftDigit, newDeep, right_single.CenterDigit);

					/* This is the most complex case.
				 * First note that when we have two Compounds, we essentially have two inner digits and two outer digits:
				 *		A..B ++ C..D => A..D
				 *	The digits B C must somehow be pushed into the FTree, but the digits A D are going to be its left and right digits.
				 *	What we do with the digits B C is call the function ReformDigitsForConcat on the inner digits
				 *	Because the law is that only digits with 2 or 3 elements can be pushed to become items in the deeper trees
				 *	We need to reform the digits, whatever their current shape, into 2-3 digits.
				 *	Look up the function to see how it's done.
				 */
				case TreeType.Compound << 3 | TreeType.Compound:
					Digit<T> leftmost;
					Digit<T> middle;
					Digit<T> rightmost;
					var compound1 = first as Compound<T>;
					var compound2 = last as Compound<T>;
					Digit<T> innerLeft = compound1.RightDigit;
					Digit<T> innerRight = compound2.LeftDigit;
					Digit<T>.ReformDigitsForConcat(innerLeft, innerRight, out leftmost, out middle, out rightmost);
					FTree<Digit<T>> deep;
					if (compound1.Measure > compound2.Measure)
						//We want to push the small tree into the large one. 
					{
						deep = compound1.DeepTree;
						if (leftmost != null)
						{
							deep = deep.AddRight(leftmost);
						}
						if (middle != null)
						{
							deep = deep.AddRight(middle);
						}
						if (rightmost != null)
						{
							deep = deep.AddRight(rightmost);
						}
						deep = FTree<Digit<T>>.Concat(deep, compound2.DeepTree);
					}
					else
					{
						deep = compound2.DeepTree;
						if (rightmost != null)
						{
							deep = deep.AddLeft(rightmost);
						}
						if (middle != null)
						{
							deep = deep.AddLeft(middle);
						}
						if (leftmost != null)
						{
							deep = deep.AddLeft(leftmost);
						}
						deep = FTree<Digit<T>>.Concat(compound1.DeepTree, deep);
					}
					return new Compound<T>(measure, compound1.LeftDigit, deep, compound2.RightDigit);
			}
			return null;
		}
	}
}