using Funq.Collections.Implementation.Common;
using System;
namespace Funq.Collections.Implementation {
	static partial class FingerTree<TValue> {
		abstract partial class FTree<TChild> {
			internal sealed partial class Digit {
					public FingerTree<TValue2>.FTree<TInner>.Digit ApplyTo<TInner, TValue2>(int nesting, Func<TValue, TValue2> f,
						Lineage lin) where TInner : FingerTree<TValue2>.Measured<TInner>, new() {
					var nextNesting = nesting - 1;
					switch (Size) {
						case 1:
							return new FingerTree<TValue2>.FTree<TInner>.Digit(First.Apply<TInner, TValue2>(nextNesting, f, lin), lin);
						case 2:
							return new FingerTree<TValue2>.FTree<TInner>.Digit(First.Apply<TInner, TValue2>(nextNesting, f, lin),
								Second.Apply<TInner, TValue2>(nextNesting, f, lin), lin);
						case 3:
							return new FingerTree<TValue2>.FTree<TInner>.Digit(First.Apply<TInner, TValue2>(nextNesting, f, lin),
								Second.Apply<TInner, TValue2>(nextNesting, f, lin), Third.Apply<TInner, TValue2>(nextNesting, f, lin), lin);
						case 4:
							return new FingerTree<TValue2>.FTree<TInner>.Digit(
								First.Apply<TInner, TValue2>(nesting - 1, f, lin),
								Second.Apply<TInner, TValue2>(nesting - 1, f, lin),
								Third.Apply<TInner, TValue2>(nesting - 1, f, lin),
								Fourth.Apply<TInner, TValue2>(nesting - 1, f, lin),
								lin
								);
						default:
							throw ImplErrors.Bad_digit_size(Size);
					}
				}

				///<summary>
				///A kind of hack. Applies the 'selector' over each element in the digit, returning a new digit containing leaf values of the type 'TValue2'.<br/>
				///</summary>
				///<remarks>
				///To explain what this method really does and what it's for, you have to note that it's not possible to link the types of two digits that are structurally at the same level,
				///but have different leaf values.  So there is no way to meaningfuly encode the return type of this method, so casting will be required somewhere.
				///Now, I can determine the correct return type from the 'nesting' argument, since it keeps track of the depth of this digit, and I can also express that type at the point of call
				///So I just need to cast it correctly. I figured the most convenient way to cast was at this point.
				///</remarks>
				public override TExpected Apply<TExpected, TValue2>(int nesting, Func<TValue, TValue2> selector, Lineage lin) {
						switch (nesting) {
							case 1:
									return (TExpected)(object)ApplyTo<Leaf<TValue2>, TValue2>(nesting, selector, lin);
							case 2:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit, TValue2>(nesting, selector, lin);
							case 3:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 4:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 5:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 6:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 7:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 8:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 9:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 10:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 11:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 12:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 13:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 14:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 15:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 16:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 17:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 18:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 19:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 20:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 21:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 22:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 23:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 24:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 25:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 26:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 27:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 28:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 29:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 30:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
							case 31:
									return (TExpected)(object)ApplyTo<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<FingerTree<TValue2>.FTree<Leaf<TValue2>>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit>.Digit, TValue2>(nesting, selector, lin);
														default:
								throw ImplErrors.Invalid_execution_path("Finger trees shouldn't have digits more than 32 deep!");
						}
				}
			}
		}
	}
}
