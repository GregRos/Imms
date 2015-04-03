using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Funq.Abstract
{
	/// <summary>
	/// Caches the result of comparisons/hashing without persisting objects in memory. Used when comparison or hashing is very expensive.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class CachingEqualityComparer<T> : EquatableEqualityBase<T, CachingEqualityComparer<T>> where T : class
	{
		private static readonly ConditionalWeakTable<T, Box<int>> hashCodeCache = new ConditionalWeakTable<T, Box<int>>();
		private static readonly ConditionalWeakTable<Tuple<T, T>, Box<bool>> eqCache = new ConditionalWeakTable<Tuple<T, T>, Box<bool>>();

		public CachingEqualityComparer(IEqualityComparer<T> inner)
		{
			Inner = inner;
		}

		IEqualityComparer<T> Inner { get; set; }

		public override bool Equals(T x, T y)
		{
			var boiler = EqualityHelper.Boilerplate(x, y);
			if (boiler.IsSome) return boiler.Value;
			Box<bool> result;
			var tuple = Tuple.Create(x, y);
			var success = eqCache.TryGetValue(tuple, out result);
			if (success) return result.Value;
			var areEqual = Inner.Equals(x, y);
			eqCache.Add(tuple, new Box<bool>(areEqual));
			return areEqual;
		}

		public override int GetHashCode(T obj)
		{
			Box<int> value;
			var success = hashCodeCache.TryGetValue(obj, out value);
			if (success)
				return value.Value;
			var result = Inner.GetHashCode(obj);
			hashCodeCache.Add(obj, new Box<int>(result));
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