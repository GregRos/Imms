using System;
using System.Collections.Generic;
using Solid.Common;

namespace Solid
{
	//+ Implementation
	//  As the FTree<TChild> class is internal and all its children are sealed, there are three types of FTrees.
	//  The empty tree in empty.cs (self evident)
	//  The Single<TChild>, a tree with one digit.
	//  The CompoundTree<TChild>, a tree with two digits and a Deep node. 
	//  These have the same purpose as outlined in the referenced paper.
	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild>
			where TChild : Measured<TChild>
		{
			public static readonly FTree<TChild> Empty = EmptyTree.Instance;

			//+ Implementation
			//  These two are readonly fields due to performance reasons.
			//  While the CLR is normally supposed to inline property calls, in order to allow all types of FTrees to access
			//  These properties, they must be abstract or have a concrete implementation.
			//  In the first case, the properties cannot be inlined, and furthermore, using them involves extra indirection
			//  Due to virtual method table lookups. 
			//  The second option would require having the backing field in the FTree<TChild> class anyway, and performs noticeably worse.

			public readonly int Kind;
			public readonly int Measure;

			private FTree(int measure, int kind)
			{
				Kind = kind;
				Measure = measure;
			}

			public static FTree<TChild> Concat(FTree<TChild> first, FTree<TChild> last)
			{
				var status = first.Kind << 3 | last.Kind;
				var measure = first.Measure + last.Measure;
				FTree<Digit> newDeep;
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
						/* If both are single... we just create a new CompoundTree with their digits.*/
					case TreeType.Single << 3 | TreeType.Single:
						var single1 = first as Single;
						var single2 = last as Single;
						return new CompoundTree(single1.CenterDigit, FTree<Digit>.Empty,
						                        single2.CenterDigit);
					case TreeType.Single << 3 | TreeType.Compound:
						var asSingle = first as Single;
						var asCompound = last as CompoundTree;
						newDeep = asCompound.DeepTree.AddLeft(asCompound.LeftDigit);
						return new CompoundTree(asSingle.CenterDigit, newDeep, asCompound.RightDigit);
						/* If one is single, we push the digit of the CompoundTree into its Deep.*/
					case TreeType.Compound << 3 | TreeType.Single:
						var right_single = last as Single;
						var left_compound = first as CompoundTree;

						newDeep = left_compound.DeepTree.AddRight(left_compound.RightDigit);
						return new CompoundTree(left_compound.LeftDigit, newDeep, right_single.CenterDigit);

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
						Digit leftmost;
						Digit middle;
						Digit rightmost;
						var compound1 = first as CompoundTree;
						var compound2 = last as CompoundTree;
						var innerLeft = compound1.RightDigit;
						var innerRight = compound2.LeftDigit;
						Digit.ReformDigitsForConcat(innerLeft, innerRight, out leftmost, out middle, out rightmost);
						FTree<Digit> deep;
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
							deep = FTree<Digit>.Concat(deep, compound2.DeepTree);
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
							deep = FTree<Digit>.Concat(compound1.DeepTree, deep);
						}
						return new CompoundTree(compound1.LeftDigit, deep, compound2.RightDigit);
				}
				throw Errors.Invalid_execution_path;
			}

			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			//+ Implementation
			//  Extra performance may be squeezed out by transforming the Left and Right properties,
			//  Which get the left and right values of the FTree
			//  Into readonly fields, as outlined above.

			public abstract TChild Left { get; }
			public abstract TChild Right { get; }

			public FTree<TChild> AddLeft(FTree<TChild> list)
			{
				return Concat(list, this);
			}

			public abstract FTree<TChild> AddLeft(TChild item);

			public FTree<TChild> AddRight(FTree<TChild> list)
			{
				return Concat(this, list);
			}

			public abstract FTree<TChild> AddRight(TChild item);

			public abstract FTree<TChild> DropLeft();

			public abstract FTree<TChild> DropRight();

			public abstract IEnumerator<Leaf<TValue>> GetEnumerator(bool forward);

			public abstract FTree<TChild> Insert(int index, Leaf<TValue> leaf);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> func);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> func);

			public abstract FTree<TChild> Remove(int index);

			public abstract FTree<TChild> Reverse();

			public abstract FTree<TChild> Set(int index, Leaf<TValue> leaf);

			public abstract void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost);
		}
	}
}