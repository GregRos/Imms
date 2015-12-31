using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Imm.Abstract {
	/// <summary>
	///     Caches the result of comparisons/hashing without persisting objects in memory. Used when comparison or hashing is
	///     very expensive.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class CachingEqualityComparer<T> : EquatableEqualityBase<T, CachingEqualityComparer<T>> where T : class {
		static readonly ConditionalWeakTable<T, Box<int>> HashCodeCache = new ConditionalWeakTable<T, Box<int>>();

		static readonly ConditionalWeakTable<Tuple<T, T>, Box<bool>> EqCache =
			new ConditionalWeakTable<Tuple<T, T>, Box<bool>>();

		public CachingEqualityComparer(IEqualityComparer<T> inner) {
			Inner = inner;
		}

		IEqualityComparer<T> Inner { get; set; }

		public override bool Equals(T x, T y) {
			var boiler = EqualityHelper.BoilerEquality(x, y);
			if (boiler.IsSome) return boiler.Value;
			Box<bool> result;
			var tuple = Tuple.Create(x, y);
			var success = EqCache.TryGetValue(tuple, out result);
			if (success) return result.Value;
			var areEqual = Inner.Equals(x, y);
			EqCache.Add(tuple, new Box<bool>(areEqual));
			return areEqual;
		}

		public override int GetHashCode(T obj) {
			Box<int> value;
			var success = HashCodeCache.TryGetValue(obj, out value);
			if (success) return value.Value;
			var result = Inner.GetHashCode(obj);
			HashCodeCache.Add(obj, new Box<int>(result));
			return result;
		}

		public override bool Equals(CachingEqualityComparer<T> other) {
			return Inner.Equals(other.Inner);
		}

		public override int GetHashCode() {
			return Inner.GetHashCode();
		}
	}
}