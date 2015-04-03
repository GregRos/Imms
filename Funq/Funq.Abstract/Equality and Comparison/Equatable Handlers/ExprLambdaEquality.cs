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
	internal class ExprLambdaEquality<T> : EquatableEqualityBase<T, ExprLambdaEquality<T>> {

		public ExprLambdaEquality(Expression<Func<T, T, bool>> equalityExpression, Expression<Func<T, int>> hashCodeExpression)
		{
			HashCodeExpression = hashCodeExpression;
			EqualityExpression = equalityExpression;
			EqualityFunction = Fun.MemoizeCompile(equalityExpression);
			HashCodeFunction = Fun.MemoizeCompile(HashCodeExpression);
		}

		private Expression<Func<T, T, bool>> EqualityExpression { get; set; }
		private Expression<Func<T, int>> HashCodeExpression { get; set; }

		private Func<T, T, bool> EqualityFunction { get; set; }

		Func<T, int> HashCodeFunction { get; set; }

		public override bool Equals(T x, T y)
		{
			var boiler = EqualityHelper.Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			return EqualityFunction(x, y);
		}

		public override int GetHashCode(T obj)
		{
			return HashCodeFunction(obj);
		}

		public override int GetHashCode() {
			return EqualityExpression.FunctionalHashCode() ^ HashCodeExpression.FunctionalHashCode();
		}

		public override bool Equals(ExprLambdaEquality<T> other) {
			return EqualityExpression.FunctionalEquals(other.EqualityExpression)
				&& HashCodeExpression.FunctionalEquals(other.HashCodeExpression);
		}
	}
}