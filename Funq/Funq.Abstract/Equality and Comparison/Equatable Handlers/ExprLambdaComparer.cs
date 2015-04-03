using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract {
	internal class ExprLambdaComparer<T> : EquatableComparerBase<T, ExprLambdaComparer<T>> {
		public Expression<Func<T, T, int>> ComparisonExpr { get; set; }
		public Func<T, T, int> ComparisonFunc { get; set; } 

		public ExprLambdaComparer(Expression<Func<T, T, int>> comparisonExpr) {
			ComparisonExpr = comparisonExpr;
			ComparisonFunc = Fun.MemoizeCompile(comparisonExpr);
		}

		public override int Compare(T x, T y) {
			return ComparisonFunc(x, y);
		}

		public override bool Equals(ExprLambdaComparer<T> other) {
			return ExpressionEquality.CachingInstance.Equals(ComparisonExpr, other.ComparisonExpr);
		}

		public override bool Equals(object obj) {
			return obj is ExprLambdaComparer<T> && Equals((ExprLambdaComparer<T>) obj);
		}

		public override int GetHashCode() {
			return ExpressionEquality.CachingInstance.GetHashCode(ComparisonExpr);
		}
	}
}
