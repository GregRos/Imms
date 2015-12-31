using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Funq.Abstract {
	class KeyComparer<TSource, TKey> : EquatableComparerBase<TSource, KeyComparer<TSource, TKey>> {

		public KeyComparer(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer) {
			InnerComparer = comparer;
			KeySelectorExpr = keySelector;
			KeySelectorFunc = Fun.MemoizeCompile(keySelector);
		}

		IComparer<TKey> InnerComparer { get; set; }

		Expression<Func<TSource, TKey>> KeySelectorExpr { get; set; }

		Func<TSource, TKey> KeySelectorFunc { get; set; }

		public override int Compare(TSource x, TSource y) {
			return InnerComparer.Compare(KeySelectorFunc(x), KeySelectorFunc(y));
		}

		public override bool Equals(KeyComparer<TSource, TKey> other) {
			return InnerComparer.Equals(other.InnerComparer) && KeySelectorExpr.FunctionalEquals(other.KeySelectorExpr);
		}

		public override int GetHashCode() {
			return InnerComparer.GetHashCode() & KeySelectorExpr.GetHashCode();
		}
	}
}