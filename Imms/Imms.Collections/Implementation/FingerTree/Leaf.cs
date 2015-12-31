using System;

namespace Imms.Implementation {
	/// <summary>
	///     Used to wrap a single value. Needs to exist because Digit objects can only contain Measured objects.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	class Leaf<TValue> : FingerTree<TValue>.Measured<Leaf<TValue>>, IEquatable<Leaf<TValue>> {
		public readonly TValue Value;

		public Leaf() : base(0, Lineage.Immutable, 0, 0) {}

		public Leaf(TValue value)
			: base(1, Lineage.Immutable, 0, 0) {
			Value = value;
		}

		public override bool HasValue {
			get { return true; }
		}

		public override Leaf<TValue> this[int index] {
			get { return this; }
		}

		public override bool IsFragment {
			get { return false; }
		}

		public bool Equals(Leaf<TValue> other) {
			return Value.Equals(other.Value);
		}

		public override int GetHashCode() {
			return Value.GetHashCode();
		}

		public override FingerTreeElement GetChild(int index) {
			throw ImplErrors.Invalid_invocation("FingerTree Leaf");
		}

		public static implicit operator TValue(Leaf<TValue> leaf) {
			return leaf.Value;
		}

		public static implicit operator Leaf<TValue>(TValue v) {
			return new Leaf<TValue>(v);
		}

		public override bool Equals(object obj) {
			return obj is Leaf<TValue> && Equals((Leaf<TValue>) obj);
		}

		public override string Print() {
			return "1";
		}

		public override Leaf<TValue> Construct3(TValue[] arr, ref int i, Lineage lin) {
			return new Leaf<TValue>(arr[i++]);
		}

		public override void Fuse(Leaf<TValue> after, out Leaf<TValue> firstRes, out Leaf<TValue> lastRes, Lineage lineage) {
			throw ImplErrors.Invalid_invocation("FingerTree Leaf");
		}

		public override void Insert(int index, Leaf<TValue> leaf, out Leaf<TValue> leftmost, out Leaf<TValue> rightmost,
			Lineage lineage) {
			leftmost = leaf;
			rightmost = this;
		}

		public override void Iter(Action<Leaf<TValue>> action) {
			action(this);
		}

		public override void IterBack(Action<Leaf<TValue>> action) {
			action(this);
		}

		public override bool IterBackWhile(Func<Leaf<TValue>, bool> action) {
			return action(this);
		}

		public override bool IterWhile(Func<Leaf<TValue>, bool> action) {
			return action(this);
		}

		public override Leaf<TValue> Remove(int index, Lineage lineage) {
#if ASSERTS
			index.AssertEqual(0);
#endif
			return null;
		}

		public override Leaf<TValue> Reverse(Lineage lineage) {
			return this;
		}

		public override Leaf<TValue> Update(int index, Leaf<TValue> leaf, Lineage lineage) {
			return leaf;
		}

	}
}