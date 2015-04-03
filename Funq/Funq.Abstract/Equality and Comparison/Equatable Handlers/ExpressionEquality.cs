using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Funq.Abstract {
	internal class ExpressionEquality : IEqualityComparer<LambdaExpression> {
		public static bool Eq<TSource, TValue>(
			Expression<Func<TSource, TValue>> x,
			Expression<Func<TSource, TValue>> y) {
			return EquateExpr(x, y, null, null);
		}

		private ExpressionEquality() {
			
		}

		private static int HashExpr(int positionMult, Expression x) {
			int hash;
			var nodeHash = x.NodeType.GetHashCode();
			var typeHash = x.Type.GetHashCode();
			hash = positionMult*(nodeHash ^ typeHash);

			int nextMult = (positionMult + 1);
			if (x is ConstantExpression)
			{
				var xConst = (ConstantExpression)x;
				hash ^= nextMult*RuntimeHelpers.GetHashCode(xConst.Value);
			}
			else if (x is LambdaExpression)
			{
				var lx = (LambdaExpression)x;
				foreach (var pr in lx.Parameters) {
					hash ^= HashExpr(nextMult, pr);
				}
				hash ^= HashExpr(nextMult, lx.Body);
			}
			else if (x is MemberExpression)
			{
				var mex = (MemberExpression)x;
				hash ^= nextMult*mex.Member.GetHashCode();
				hash ^= HashExpr(nextMult, mex.Expression);
			}
			else if (x is BinaryExpression)
			{
				var bx = (BinaryExpression)x;
				hash ^= nextMult * bx.Method.GetHashCode();
				hash ^= HashExpr(nextMult, bx.Left);
				hash ^= HashExpr(nextMult, bx.Right);
			}
			else if (x is ParameterExpression)
			{
				//nothing
			}
			else if (x is MethodCallExpression) {
				var mx = (MethodCallExpression) x;
				hash ^= nextMult*mx.Method.GetHashCode();
				hash ^= HashExpr(nextMult, mx.Object);
				mx.Arguments.ForEach(expr => {
					hash ^= HashExpr(nextMult, expr);
				});
			}
			else if (x is ConditionalExpression)
			{
				var xCond = (ConditionalExpression)x;
				hash ^= HashExpr(nextMult, xCond.Test);
				hash ^= HashExpr(nextMult, xCond.IfTrue);
				hash ^= HashExpr(nextMult, xCond.IfFalse);
			}
			else if (x is IndexExpression)
			{
				var xIndex = (IndexExpression)x;
				hash ^= HashExpr(nextMult, xIndex.Object);
				xIndex.Arguments.ForEach(expr => hash ^= HashExpr(nextMult, expr));
				hash ^= nextMult*xIndex.Indexer.GetHashCode();

			}
			else if (x is NewExpression)
			{
				var xNew = (NewExpression)x;
				hash ^= nextMult * xNew.Constructor.GetHashCode();
				xNew.Arguments.ForEach(expr => hash ^= HashExpr(nextMult, expr));
			}
			else if (x is ListInitExpression)
			{
				var xInit = (ListInitExpression)x;
				hash ^= HashExpr(nextMult, xInit.NewExpression);
				xInit.Initializers.ForEach(init => {
					hash ^= init.AddMethod.GetHashCode();
					init.Arguments.ForEach(arg => hash ^= HashExpr(nextMult, arg));
				});
			}
			else if (x is UnaryExpression) {
				var xUnary = (UnaryExpression) x;
				hash ^= HashExpr(nextMult, xUnary.Operand);
				hash ^= nextMult*xUnary.Method.GetHashCode();
			}
			else {
				
			}
			return hash;
		}

		private static bool EquateExpr(Expression x, Expression y, LambdaExpression rootX, LambdaExpression rootY)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;

			if (x.NodeType != y.NodeType
				|| x.Type != y.Type) return false;

			if (x is ConstantExpression) {
				var xConst = (ConstantExpression) x;
				var yConst = (ConstantExpression) y;
				return ValuesEqual(xConst.Value, yConst.Value);
			}
			if (x is LambdaExpression)
			{
				var lx = (LambdaExpression)x;
				var ly = (LambdaExpression)y;
				var paramsX = lx.Parameters;
				var paramsY = ly.Parameters;
				return CollectionsEqual(paramsX, paramsY, lx, ly) && EquateExpr(lx.Body, ly.Body, lx, ly);
			}
			if (x is MemberExpression)
			{
				var mex = (MemberExpression)x;
				var mey = (MemberExpression)y;
				return Equals(mex.Member, mey.Member) && EquateExpr(mex.Expression, mey.Expression, rootX, rootY);
			}
			if (x is BinaryExpression)
			{
				var bx = (BinaryExpression)x;
				var by = (BinaryExpression)y;
				return bx.Method == @by.Method && EquateExpr(bx.Left, @by.Left, rootX, rootY) &&
					EquateExpr(bx.Right, @by.Right, rootX, rootY);
			}
			if (x is ParameterExpression)
			{
				var px = (ParameterExpression)x;
				var py = (ParameterExpression)y;
				return rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py);
			}
			if (x is MethodCallExpression)
			{
				var cx = (MethodCallExpression)x;
				var cy = (MethodCallExpression)y;
				return cx.Method == cy.Method
					&& EquateExpr(cx.Object, cy.Object, rootX, rootY)
					&& CollectionsEqual(cx.Arguments, cy.Arguments, rootX, rootY);
			}
			if (x is ConditionalExpression) {
				var xCond = (ConditionalExpression) x;
				var yCond = (ConditionalExpression) y;
				return EquateExpr(xCond.Test, yCond.Test, rootX, rootY) && EquateExpr(xCond.IfTrue, yCond.IfTrue, rootX, rootY) &&
					EquateExpr(xCond.IfFalse, yCond.IfFalse, rootX, rootY);
			}
			if (x is IndexExpression) {
				var xIndex = (IndexExpression) x;
				var yIndex = (IndexExpression) y;
				return
					EquateExpr(xIndex.Object, yIndex.Object, rootX, rootY)
						&& CollectionsEqual(xIndex.Arguments, yIndex.Arguments, rootX, rootY)
						&& xIndex.Indexer.Equals(yIndex.Indexer);
			}
			if (x is NewExpression) {
				var xNew = (NewExpression) x;
				var yNew = (NewExpression) y;
				return
					xNew.Constructor.Equals(yNew.Constructor)
						&& CollectionsEqual(xNew.Arguments, yNew.Arguments, rootX, rootY);
			}
			if (x is ListInitExpression) {
				var xInit = (ListInitExpression) x;
				var yInit = (ListInitExpression)y;
				return
					EquateExpr(xInit.NewExpression, yInit.NewExpression, rootX, rootY) &&
						xInit.Initializers.SequenceEquals(yInit.Initializers,
							(xElem, yElem) =>
								xElem.AddMethod.Equals(yElem.AddMethod) && CollectionsEqual(xElem.Arguments, yElem.Arguments, rootX, rootY));
			}
			if (x is UnaryExpression) {
				var xUnary = (UnaryExpression) x;
				var yUnary = (UnaryExpression) y;
				return EquateExpr(xUnary.Operand, yUnary.Operand, rootX, rootY)
					&& xUnary.Method.Equals(yUnary.Method);
			}
			throw new NotImplementedException(x.ToString());
		}

		private static bool ValuesEqual(object xVal, object yVal) {
			return RuntimeHelpers.Equals(xVal, yVal);
		}

		private static bool CollectionsEqual(IEnumerable<Expression> x, IEnumerable<Expression> y, LambdaExpression rootX, LambdaExpression rootY)
		{
			return x.Count() == y.Count()
				&& x.Select((e, i) => new { Expr = e, Index = i })
					.Join(y.Select((e, i) => new { Expr = e, Index = i }),
						o => o.Index, o => o.Index, (xe, ye) => new { X = xe.Expr, Y = ye.Expr })
					.All(o => EquateExpr(o.X, o.Y, rootX, rootY));
		}

		public bool Equals(LambdaExpression x, LambdaExpression y) {
			return EquateExpr(x, y, null, null);
		}

		public int GetHashCode(LambdaExpression obj) {
			return HashExpr(1, obj);
		}

		static ExpressionEquality() {
			Instance = new ExpressionEquality();
			CachingInstance = new CachingEqualityComparer<LambdaExpression>(Instance); 
		}

		public static readonly ExpressionEquality Instance;

		public static readonly IEqualityComparer<LambdaExpression> CachingInstance;
	}
}