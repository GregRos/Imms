using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Funq.Abstract
{
	internal class CachingEqualityComparer<T> : IEqualityComparer<T> where T : class
	{
		private static readonly ConditionalWeakTable<T, Box<int>> hashCodeCache = new ConditionalWeakTable<T, Box<int>>();
		private readonly IEqualityComparer<T> _inner;

		public CachingEqualityComparer(IEqualityComparer<T> inner)
		{
			_inner = inner;
		}

		public bool Equals(T x, T y)
		{
			var boiler = Equality.Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			var xHashCode = GetHashCode(x);
			var yHashCode = GetHashCode(y);
			if (xHashCode != yHashCode) return false;
			return _inner.Equals(x, y);
		}

		public int GetHashCode(T obj)
		{
			Box<int> value;
			var success = hashCodeCache.TryGetValue(obj, out value);
			if (success)
				return value.Value;
			var result = _inner.GetHashCode(obj);
			hashCodeCache.Add(obj, new Box<int>(result));
			return result;
		}
	}
}