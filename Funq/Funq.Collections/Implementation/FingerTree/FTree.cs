using System;
using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{

	static partial class FingerTree<TValue>
	{
		internal abstract partial class FTree<TChild> : FingerTreeElement
			where TChild : Measured<TChild>
		{
			public static readonly FTree<TChild> Empty = EmptyTree.Instance;

			public int Kind;
			public readonly Lineage Lineage;
			public int Measure;

			private FTree(int measure, int kind, Lineage lineage, int groupings) : base(groupings)
			{
				Kind = kind;
				Measure = measure;
				Lineage = lineage;
			}

			public static FTree<TChild> Concat(FTree<TChild> first, FTree<TChild> last, Lineage lineage)
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
						/* If both are single... we just create a new Compound with their digits.*/
					case TreeType.Single << 3 | TreeType.Single:
						var single1 = first as Single;
						var single2 = last as Single;
						return new Compound(single1.CenterDigit, FTree<Digit>.Empty,
						                    single2.CenterDigit, lineage);
					case TreeType.Single << 3 | TreeType.Compound:
						var asSingle = first as Single;
						var asCompound = last as Compound;
						newDeep = asCompound.DeepTree.AddFirst(asCompound.LeftDigit, lineage);
						return new Compound(asSingle.CenterDigit, newDeep, asCompound.RightDigit, lineage);
						/* If one is single, we push the digit of the Compound into its Deep.*/
					case TreeType.Compound << 3 | TreeType.Single:
						var right_single = last as Single;
						var left_compound = first as Compound;

						newDeep = left_compound.DeepTree.AddLast(left_compound.RightDigit, lineage);
						return new Compound(left_compound.LeftDigit, newDeep, right_single.CenterDigit, lineage);

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
						var compound1 = first as Compound;
						var compound2 = last as Compound;
						var innerLeft = compound1.RightDigit;
						var innerRight = compound2.LeftDigit;
						Digit.ReformDigitsForConcat(lineage, innerLeft, innerRight, out leftmost, out middle, out rightmost);
						FTree<Digit> deep;
						if (compound1.Measure > compound2.Measure)
							//We want to push the small tree into the large one. 
						{
							deep = compound1.DeepTree;
							if (leftmost != null)
							{
								deep = deep.AddLast(leftmost, lineage);
							}
							if (middle != null)
							{
								deep = deep.AddLast(middle, lineage);
							}
							if (rightmost != null)
							{
								deep = deep.AddLast(rightmost, lineage);
							}
							deep = FTree<Digit>.Concat(deep, compound2.DeepTree, lineage);
						}
						else
						{
							deep = compound2.DeepTree;
							if (rightmost != null)
							{
								deep = deep.AddFirst(rightmost, lineage);
							}
							if (middle != null)
							{
								deep = deep.AddFirst(middle, lineage);
							}
							if (leftmost != null)
							{
								deep = deep.AddFirst(leftmost, lineage);
							}
							deep = FTree<Digit>.Concat(compound1.DeepTree, deep, lineage);
						}
						return new Compound(compound1.LeftDigit, deep, compound2.RightDigit, lineage);
				}
				throw ImplErrors.Invalid_execution_path;
			}

			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			//+ Implementation
			//  Extra performance may be squeezed out by transforming the Left and Right properties,
			//  Which get the left and right values of the FTree
			//  Into readonly fields, as outlined above.

			public abstract TChild Left { get; }
			public abstract TChild Right { get; }

			public FTree<TChild> AddFirst(FTree<TChild> list, Lineage lineage)
			{
				return Concat(list, this, lineage);
			}

			public abstract FTree<TChild> AddFirst(TChild item, Lineage lineage);


			public FTree<TChild> AddLastList(FTree<TChild> list, Lineage lineage)
			{
				return Concat(this, list, lineage);
			}

			public abstract FTree<TChild> AddLast(TChild item, Lineage lineage);

			public abstract FTree<TChild> RemoveFirst(Lineage lineage);

			public abstract FTree<TChild> RemoveLast(Lineage lineage);

			public abstract FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> func);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> func);

			public abstract FTree<TChild> Remove(int index, Lineage lineage);

			public abstract FTree<TChild> Reverse(Lineage lineage);

			public abstract void Split(int count, out FTree<TChild> leftmost, out FTree<TChild> rightmost, Lineage lineage);

			public abstract FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage);
		}
	}
}