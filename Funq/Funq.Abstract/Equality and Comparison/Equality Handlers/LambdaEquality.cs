using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class LambdaEquality<T> : IEqualityComparer<T>
	{
		public LambdaEquality(Func<T, T, bool> equalityFunction, Func<T, int> hashCodeFunction)
		{
			HashCodeFunction = hashCodeFunction;
			EqualityFunction = equalityFunction;
		}

		public Func<T, T, bool> EqualityFunction
		{
			get;
			private set;
		}

		public Func<T, int> HashCodeFunction
		{
			get;
			private set;
		}

		public virtual bool Equals(T x, T y)
		{
			var boiler = Equality.Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			return EqualityFunction(x, y);
		}

		public virtual int GetHashCode(T obj)
		{
			return HashCodeFunction(obj);
		}
	}
}