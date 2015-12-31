using System.Collections.Generic;

namespace Imm.Abstract {
	abstract class EquatableComparerBase<T, This> : IEquatableComparer<T> {
		public abstract int Compare(T x, T y);

		public bool Equals(IComparer<T> other) {
			return other is This && Equals((This) other);
		}

		public abstract bool Equals(This other);

		public override bool Equals(object obj) {
			return obj is This && Equals((This) obj);
		}

		public abstract override int GetHashCode();
	}
}