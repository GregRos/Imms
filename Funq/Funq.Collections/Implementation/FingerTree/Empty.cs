using System;

namespace Funq.Implementation {
	static partial class FingerTree<TValue> {

		internal abstract partial class FTree<TChild> where TChild : Measured<TChild>, new() {
			internal sealed class EmptyTree : FTree<TChild> {
				public static readonly EmptyTree Instance = new EmptyTree();

				EmptyTree()
					: base(0, TreeType.Empty, Lineage.Immutable, 0) {}

				public override Leaf<TValue> this[int index] {
					get { throw ImplErrors.Invalid_invocation("Empty FingerTree"); }
				}

				public override bool IsFragment {
					get { throw ImplErrors.Invalid_invocation("Empty FingerTree"); }
				}

				public override TChild Left {
					get { throw ImplErrors.Invalid_invocation("Empty FingerTree");}
				}

				public override TChild Right {
					get { throw ImplErrors.Invalid_invocation("Empty FingerTree"); }
				}

				public override string Print() {
					return "[[ - ]]";
				}

				public override FTree<TChild> AddFirst(TChild item, Lineage lineage) {
					return new Single(new Digit(item, lineage), lineage);
				}

				public override FTree<TChild> AddLast(TChild item, Lineage lineage) {
					return new Single(new Digit(item, lineage), lineage);
				}

				public override FTree<TChild> RemoveFirst(Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override FTree<TChild> RemoveLast(Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override void Split(int index, out FTree<TChild> left, out TChild child, out FTree<TChild> right, Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override FTree<TChild> Insert(int index, Leaf<TValue> leaf, Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override void Iter(Action<Leaf<TValue>> action1) {}

				public override void IterBack(Action<Leaf<TValue>> action) {}

				public override bool IterBackWhile(Func<Leaf<TValue>, bool> func) {
					return true;
				}

				public override bool IterWhile(Func<Leaf<TValue>, bool> func) {
					return true;
				}

				public override FTree<TChild> RemoveAt(int index, Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override FTree<TChild> Reverse(Lineage lineage) {
					return this;
				}

	
				public override FTree<TChild> Update(int index, Leaf<TValue> leaf, Lineage lineage) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}

				public override FingerTreeElement GetChild(int index) {
					throw ImplErrors.Invalid_invocation("Empty FingerTree");
				}
			}
		}
	}
}