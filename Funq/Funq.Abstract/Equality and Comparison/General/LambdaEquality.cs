using System;
using System.Collections.Generic;

namespace Funq.Abstract {
	class LambdaEquality<T> : IEqualityComparer<T> {

		public LambdaEquality(Func<T, T, bool> equalsFunction, Func<T, int> hashFunction) {
			EqualityFunction = equalsFunction;
			HashCodeFunction = hashFunction;
		}

		Func<T, T, bool> EqualityFunction { get; set; }

		Func<T, int> HashCodeFunction { get; set; }

		public bool Equals(T x, T y) {
			var boiler = EqualityHelper.BoilerEquality(x, y);
			if (boiler.IsSome) return boiler.Value;
			return EqualityFunction(x, y);
		}

		public int GetHashCode(T obj) {
			return HashCodeFunction(obj);
		}
	}
}