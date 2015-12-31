using System;
using System.Linq.Expressions;

namespace Imm.Abstract {
	class ExprLambdaEquality<T> : EquatableEqualityBase<T, ExprLambdaEquality<T>> {

		public ExprLambdaEquality(Expression<Func<T, T, bool>> equalityExpression, Expression<Func<T, int>> hashCodeExpression) {
			HashCodeExpression = hashCodeExpression;
			EqualityExpression = equalityExpression;
			EqualityFunction = Fun.MemoizeCompile(equalityExpression);
			HashCodeFunction = Fun.MemoizeCompile(HashCodeExpression);
		}

		Expression<Func<T, T, bool>> EqualityExpression { get; set; }

		Expression<Func<T, int>> HashCodeExpression { get; set; }

		Func<T, T, bool> EqualityFunction { get; set; }

		Func<T, int> HashCodeFunction { get; set; }

		public override bool Equals(T x, T y) {
			var boiler = EqualityHelper.BoilerEquality(x, y);
			if (boiler.IsSome) return boiler.Value;
			return EqualityFunction(x, y);
		}

		public override int GetHashCode(T obj) {
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