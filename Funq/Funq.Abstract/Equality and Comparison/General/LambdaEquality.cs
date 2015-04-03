using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

namespace Funq.Abstract
{
	internal class LambdaEquality<T> : IEqualityComparer<T> {

		private Func<T, T, bool> EqualityFunction { get; set; }

		Func<T, int> HashCodeFunction { get; set; }
		public LambdaEquality(Func<T, T, bool> equalsFunction, Func<T, int> hashFunction)
		{
			EqualityFunction = equalsFunction;
			HashCodeFunction = hashFunction;
		}

		public bool Equals(T x, T y)
		{
			var boiler = EqualityHelper.Boilerplate(x, y);
			if (boiler.IsSome) return boiler.Value;
			return EqualityFunction(x, y);
		}

		public int GetHashCode(T obj)
		{
			return HashCodeFunction(obj);
		}
	}
}