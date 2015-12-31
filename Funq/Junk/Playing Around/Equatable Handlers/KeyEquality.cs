using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Funq.Abstract {
	class KeyEquality<TSource, TKey> : EquatableEqualityBase<TSource, KeyEquality<TSource, TKey>> {

		public KeyEquality(Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> eq) {
			InnerEquality = eq;
			KeySelectorExpr = keySelector;
			KeySelectorFunc = Fun.MemoizeCompile(keySelector);

		}

		IEqualityComparer<TKey> InnerEquality { get; set; }

		Expression<Func<TSource, TKey>> KeySelectorExpr { get; set; }

		Func<TSource, TKey> KeySelectorFunc { get; set; }

		public override bool Equals(TSource x, TSource y) {
			return InnerEquality.Equals(KeySelectorFunc(x), KeySelectorFunc(y));
		}

		public override int GetHashCode(TSource obj) {
			return InnerEquality.GetHashCode(KeySelectorFunc(obj));
		}

		public override bool Equals(KeyEquality<TSource, TKey> other) {
			return InnerEquality.Equals(other.InnerEquality) && KeySelectorExpr.FunctionalEquals(other.KeySelectorExpr);
		}

		public override int GetHashCode() {
			return InnerEquality.GetHashCode() ^ KeySelectorExpr.GetHashCode();
		}
	}
}