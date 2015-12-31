using System.Collections.Generic;

namespace Imm.Abstract {
	abstract class EquatableEqualityBase<T, This> : IEquatableEquality<T> {
		public abstract bool Equals(T x, T y);

		public abstract int GetHashCode(T obj);

		public bool Equals(IEqualityComparer<T> other) {
			return other is This && Equals((This) other);
		}

		public abstract bool Equals(This other);

		public override bool Equals(object obj) {
			return obj is IEqualityComparer<T> && Equals((IEqualityComparer<T>) obj);
		}

		public abstract override int GetHashCode();
	}
}