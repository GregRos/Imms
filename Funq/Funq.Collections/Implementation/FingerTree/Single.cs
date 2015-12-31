using System;

namespace Funq.Implementation {
	static partial class FingerTree<TValue> {
		internal abstract partial class FTree<TChild> where TChild : Measured<TChild>, new() {
			private sealed class Single : FTree<TChild> {
				public Digit CenterDigit;

				public Single(Digit centerDigit, Lineage lineage)
					: base(centerDigit.Measure, TreeType.Single, lineage, 1) {
					CenterDigit = centerDigit;
				}

				public override Leaf<TValue> this[int index] {
					get {
						var r = CenterDigit[index];
						return r;
					}
				}

				public override bool IsFragment {
					get { return CenterDigit.IsFragment; }
				}

				public override TChild Left {
					get { return CenterDigit.Left; }
				}

				public override TChild Right {
					get { return CenterDigit.Right; }
				}

				public override string Print() {
					return string.Format("[[{0}]]", CenterDigit.Print());
				}

				FTree<TChild> _mutate(Digit digit) {
					CenterDigit = digit;
					Measure = digit.Measure;
					return this;
				}

				private FTree<TChild> MutateOrCreate(Digit digit, Lineage lineage) {
					return _lineage.AllowMutation(lineage) ? _mutate(digit) : new Single(digit, lineage);
				}

				public override FTree<TChild> AddFirst(TChild item, Lineage lineage) {
					FTree<TChild> ret;
#if ASSERTS
					var expected = Measure + item.Measure;
#endif
					if (CenterDigit.Size < 4) ret = MutateOrCreate(CenterDigit.AddFirst(item, lineage), lineage);
					else {
						var leftmost = new Digit(item, CenterDigit.First, lineage);
						var rightmost = CenterDigit.RemoveFirst(lineage);
						ret = new Compound(leftmost, FTree<Digit>.Empty, rightmost, lineage);
					}
#if ASSERTS
					ret.Measure.AssertEqual(expected);
					ret.Left.AssertEqual(item);
#endif
					return ret;

				}

				public override FTree<TChild> AddLast(TChild item, Lineage lineage) {
#if ASSERTS
					var expected = Measure + item.Measure;
#endif
					FTree<TChild> ret;
					if (CenterDigit.Size < 4) ret = new Single(CenterDigit.AddLast(item, lineage), lineage);
					else {
						var rightmost = new Digit(CenterDigit.Fourth, item, lineage);
						var leftmost = CenterDigit.RemoveLast(lineage);

						ret = new Compound(leftmost, FTree<Digit>.Empty, rightmost, lineage);
					}
#if ASSERTS
					ret.Measure.AssertEqual(expected);
					ret.Right.AssertEqual(item);
#endif
					return ret;

				}

				public override FTree<TChild> RemoveFirst(Lineage lineage) {
					FTree<TChild> ret;
#if ASSERTS
					var expected = Measure - Left.Measure;
					var expectedFirst = Measure > 1 ? CenterDigit.Second : null;
#endif
					if (CenterDigit.Size > 1) {
						var newDigit = CenterDigit.RemoveFirst(lineage);
						ret = MutateOrCreate(newDigit, lineage);
					} else ret = Empty;
#if ASSERTS
					ret.Measure.AssertEqual(expected);
					if (expectedFirst != null) ret.Left.AssertEqual(expectedFirst);
#endif
					return ret;
				}

				public override FTree<TChild> RemoveLast(Lineage lineage) {
					if (CenterDigit.Size > 1) {
						var newDigit = CenterDigit.RemoveLast(lineage);
						return MutateOrCreate(newDigit, lineage);
					}
					return Empty;
				}

				public override void Split(int index, out FTree<TChild> left, out TChild center, out FTree<TChild> right, Lineage lineage) {
					Digit leftDigit, rightDigit;
					CenterDigit.Split(index, out leftDigit, out center, out rightDigit, lineage);
					left = leftDigit == null ? Empty : new Single(leftDigit, lineage);
					right = rightDigit == null ? Empty : new Single(rightDigit, lineage);
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage) {
					Digit leftmost, rightmost;
					CenterDigit.Insert(index, leaf, out leftmost, out rightmost, lineage);
					if (rightmost == null) return MutateOrCreate(leftmost, lineage);
					return new Compound(leftmost, FTree<Digit>.Empty, rightmost, lineage);
				}

				public override void Iter(Action<Leaf<TValue>> action) {
					CenterDigit.Iter(action);
				}

				public override void IterBack(Action<Leaf<TValue>> action) {
					CenterDigit.IterBack(action);
				}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> func) {
					return CenterDigit.IterBackWhile(func);
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> func) {
					return CenterDigit.IterWhile(func);
				}

				public override FTree<TChild> RemoveAt(int index, Lineage lineage) {
					if (CenterDigit.Measure == 1 && index == 0) return EmptyTree.Instance;
#if ASSERTS
					CenterDigit.IsFragment.AssertEqual(false);
#endif

					var res = CenterDigit.Remove(index, lineage);
					if (res == null) return Empty;
					return MutateOrCreate(res, lineage);
				}

				public override FTree<TChild> Reverse(Lineage lineage) {
					return new Single(CenterDigit.Reverse(lineage), lineage);
				}

				public override FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage) {
					return new Single(CenterDigit.Update(index, leaf, lineage), lineage);
				}

				public override FingerTreeElement GetChild(int index) {
					if (index != 0) throw ImplErrors.Arg_out_of_range("index", index);
					return CenterDigit;
				}
			}
		}
	}
}