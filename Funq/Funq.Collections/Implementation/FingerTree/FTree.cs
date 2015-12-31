using System;

namespace Funq.Implementation {

	static partial class FingerTree<TValue> {
		internal abstract partial class FTree<TChild> : FingerTreeElement where TChild : Measured<TChild>, new() {
			public static readonly FTree<TChild> Empty = EmptyTree.Instance;
			private readonly Lineage _lineage;
			private readonly int _kind;
			public int Measure;

			FTree(int measure, int kind, Lineage lineage, int groupings) : base(groupings) {
				_kind = kind;
				Measure = measure;
				_lineage = lineage;
			}

			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			//+ Implementation
			//  Extra performance may be squeezed out by transforming the Left and Right properties,
			//  Which get the left and right values of the FTree
			//  Into readonly fields, as outlined above.

			public abstract TChild Left { get; }

			public abstract TChild Right { get; }

			public bool IsEmpty
			{
				get { return Measure == 0; }
			}

			/// <summary>
			///     Constructs a finger tree from the specified array, consisting of the elements at [index..index+count)
			///     A lot faster than adding iteratively without lineages, and quite a bit faster than adding iteratively with
			///     lineages.
			/// </summary>
			/// <param name="arr">The arr.</param>
			/// <param name="index">The index.</param>
			/// <param name="count">The count.</param>
			/// <param name="lin">The lineage.</param>
			/// <returns></returns>
			public static FTree<TChild> Construct(TValue[] arr, ref int index, int count, Lineage lin) {
				var myChildSize = FastMath.PowN(3, Nesting - 1);
				var divRem = count % myChildSize;
				FTree<TChild> ret;
				var digit = ExampleDigit;
				int number;
				if (divRem == 0 && (number = count / myChildSize) <= 8) {
					switch (number) {
						case 0:
							return Empty;
						case 1:
						case 2:
						case 3:
						case 4:
							var center = digit.ConstructMult(arr, ref index, number, lin);
							ret = new Single(center, lin);
							break;
						case 5:
							var left1 = digit.Construct3(arr, ref index, lin);
							var right1 = digit.ConstructMult(arr, ref index, 2, lin);
							ret = new Compound(left1, FTree<Digit>.Empty, right1, lin);
							break;
						case 6:
							var left2 = digit.Construct3(arr, ref index, lin);
							var right2 = digit.Construct3(arr, ref index, lin);
							ret = new Compound(left2, FTree<Digit>.Empty, right2, lin);
							break;
						case 7:
							var left3 = digit.ConstructMult(arr, ref index, 4, lin);
							var right3 = digit.Construct3(arr, ref index, lin);
							ret = new Compound(left3, FTree<Digit>.Empty, right3, lin);
							break;
						case 8:
							var left4 = digit.ConstructMult(arr, ref index, 4, lin);
							var right4 = digit.ConstructMult(arr, ref index, 4, lin);
							ret = new Compound(left4, FTree<Digit>.Empty, right4, lin);
							break;
						default:
							throw ImplErrors.Invalid_execution_path("An if statement just checked if number <= 8.");
					}

				} else {
					var nextChildSize = myChildSize * 3;
					var nextDivRem = count % nextChildSize;
#if ASSERTS
					//this is guaranteed by previous recursive calls to Construct. count must be divisible by myChildSize
					//and since nextChildSize = myChildSize * 3, then nextDivRem % myChildSize must be within [0,2].
					//we maintain this invariant by the next if-else block, which makes sure that the 'count' for the next invocation
					//really is divisible by nextChildSize. At the topmost level, myChildSize is 1, and nextChildSize is 3. 
					var nextDivRemfixed = nextDivRem % myChildSize;
					nextDivRemfixed.AssertBetween(0, 2);
#endif
					if (nextDivRem == 0) {
						//If nextDivRem is already divisible by nextChildSize, we should preserve this by removing 2*nextChildSize from it.
						//Since 2*nextChildSize = 0 (mod nextChildSize), the divisibility is preserved.
						var left = digit.Construct3(arr, ref index, lin);
						var deep = FTree<Digit>.Construct(arr, ref index, count - (nextChildSize << 1), lin);
						var right = digit.Construct3(arr, ref index, lin);
						ret = new Compound(left, deep, right, lin);
					} else if (nextDivRem == myChildSize) {
						//In this case, nextDivRem % myChildSize == 1. 
						//In order to make sure 'count' is divisible by nextChildSize, we need to remove 1 myChildSize from it.
						//while also filling the current finger tree level. So we remove 7*myChildSize, which is 
						//7*myChildSize = myChildSize + 2*nextChildSize = myChildSize (mod nextChildSize)
						var left = digit.ConstructMult(arr, ref index, 4, lin);
						var deep = FTree<Digit>.Construct(arr, ref index, count - ((nextChildSize << 1) + myChildSize), lin);
						var right = digit.Construct3(arr, ref index, lin);
						ret = new Compound(left, deep, right, lin);
					} else {
						//like the other cases.
						var left = digit.ConstructMult(arr, ref index, 4, lin);
						var deep = FTree<Digit>.Construct(arr, ref index, count - ((nextChildSize << 1) + (myChildSize << 1)), lin);
						var right = digit.ConstructMult(arr, ref index, 4, lin);
						ret = new Compound(left, deep, right, lin);
					}
				}
#if ASSERTS
				ret.Measure.AssertEqual(count);
#endif
				return ret;
			}

			/// <summary>
			/// concatenates the two trees together. the supplied lineage cannot be shared by any of the trees, or the result will be corrupt!! <br/>
			/// However, you can reuse the lineage after calling this method.
			/// </summary>
			/// <param name="first">The first.</param>
			/// <param name="last">The last.</param>
			/// <param name="lineage">The lineage.</param>
			/// <returns></returns>
			public static FTree<TChild> Concat(FTree<TChild> first, FTree<TChild> last, Lineage lineage) {
				var status = first._kind << 3 | last._kind;
				FTree<Digit> newDeep;
				switch (status) {
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
						var single1 = (Single) first;
						var single2 = (Single) last;
						return new Compound(single1.CenterDigit, FTree<Digit>.Empty,
							single2.CenterDigit, lineage);
					case TreeType.Single << 3 | TreeType.Compound:
						var asSingle = (Single) first;
						var asCompound = (Compound) last;
						Digit left, mid, right;
						asSingle.CenterDigit.Fuse(asCompound.LeftDigit, out left, out mid, out right, lineage);
						newDeep = asCompound.DeepTree;
						if (right != null) {
							newDeep = newDeep.AddFirst(right, lineage);
						}
						if (mid != null) {
							newDeep = newDeep.AddFirst(mid, lineage);
						}
						return new Compound(left, newDeep, asCompound.RightDigit, lineage);
					/* If one is single, we push the digit of the Compound into its Deep.*/
					case TreeType.Compound << 3 | TreeType.Single:
						var rightSingle = (Single) last;
						var leftCompound = (Compound) first;
						Digit rLeft, rMid, rRight;
						leftCompound.RightDigit.Fuse(rightSingle.CenterDigit, out rLeft, out rMid, out rRight, lineage);
						Digit rDigit;
						newDeep = leftCompound.DeepTree;
						if (rMid != null) {
							newDeep = newDeep.AddLast(rLeft, lineage);
							if (rRight != null) {
								newDeep = newDeep.AddLast(rMid, lineage);
								rDigit = rRight;
							} else {
								rDigit = rMid;
							}
						} else {
							rDigit = rLeft;
						}
						return new Compound(leftCompound.LeftDigit, newDeep, rDigit, lineage);

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
						var compound1 = (Compound) first;
						var compound2 = (Compound) last;
						var innerLeft = compound1.RightDigit;
						var innerRight = compound2.LeftDigit;
						innerLeft.Fuse(innerRight, out leftmost, out middle, out rightmost, lineage);
						FTree<Digit> deep;
						if (compound1.Measure > compound2.Measure)
							//We want to push the small tree into the large one. 
						{
							deep = compound1.DeepTree;
							if (leftmost != null) deep = deep.AddLast(leftmost, lineage);
							if (middle != null) deep = deep.AddLast(middle, lineage);
							if (rightmost != null) deep = deep.AddLast(rightmost, lineage);
							deep = FTree<Digit>.Concat(deep, compound2.DeepTree, lineage);
						} else {
							deep = compound2.DeepTree;
							if (rightmost != null) deep = deep.AddFirst(rightmost, lineage);
							if (middle != null) deep = deep.AddFirst(middle, lineage);
							if (leftmost != null) deep = deep.AddFirst(leftmost, lineage);
							deep = FTree<Digit>.Concat(compound1.DeepTree, deep, lineage);
						}
						return new Compound(compound1.LeftDigit, deep, compound2.RightDigit, lineage);
				}
				throw ImplErrors.Invalid_execution_path("Checked all permutations of finger tree concatenation.");
			}

			public abstract string Print();

			public FTree<TChild> AddFirstList(FTree<TChild> list, Lineage lineage) {
				return Concat(list, this, lineage);
			}

			/// <summary>
			///     Adds an item to the start (the left) of the finger tree..
			/// </summary>
			/// <param name="item">The item.</param>
			/// <param name="lineage">The lineage.</param>
			/// <returns></returns>
			public abstract FTree<TChild> AddFirst(TChild item, Lineage lineage);

			/// <summary>
			///     Concats the specified ftree to the end of this one. If the lineage is shared by either of the trees, the result is corrupt!
			/// </summary>
			/// <param name="list">The list.</param>
			/// <param name="lineage">The lineage.</param>
			/// <returns></returns>
			public FTree<TChild> AddLastList(FTree<TChild> list, Lineage lineage) {
				return Concat(this, list, lineage);
			}

			public abstract FTree<TChild> AddLast(TChild item, Lineage lineage);

			public abstract FTree<TChild> RemoveFirst(Lineage lineage);

			public abstract FTree<TChild> RemoveLast(Lineage lineage);

			public abstract void Split(int index, out FTree<TChild> left, out TChild child, out FTree<TChild> right, Lineage lineage);

			public abstract FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> func);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> func);

			/// <summary>
			/// Splits the finger tree right before the specified index.
			/// </summary>
			/// <param name="index">The index.</param>
			/// <param name="left">The left.</param>
			/// <param name="right">The right.</param>
			/// <param name="lineage">The lineage.</param>
			public void Split(int index, out FTree<TChild> left, out FTree<TChild> right, Lineage lineage) {
				if (index == Measure) {
					left = this;
					right = Empty;
					return;
				}
#if ASSERTS
				var oldValue = this[index];
				var oldFirst = Left;
				var oldLast = Right;
#endif
				TChild center;
				Split(index, out left, out center, out right, lineage);
				right = right.AddFirst(center, lineage);
#if ASSERTS
				center[0].AssertEqual(oldValue);
				if (left.Measure != 0) {
					left.Left.AssertEqual(oldFirst);
				}
				if (right.Measure != 0) {
					right.Right.AssertEqual(oldLast);
				}
#endif
			}

			public abstract FTree<TChild> RemoveAt(int index, Lineage lineage);

			public abstract FTree<TChild> Reverse(Lineage lineage);

			public abstract FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage);

			static readonly Digit ExampleDigit = new Digit();
			public static int Nesting = ExampleDigit.Nesting;
		}
	}
}