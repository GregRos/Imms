using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	internal class KeyEquality<TSource, TKey> : EquatableEqualityBase<TSource, KeyEquality<TSource, TKey>> {

		private IEqualityComparer<TKey> InnerEquality { get; set; }
		private Expression<Func<TSource, TKey>> KeySelectorExpr { get; set; }

		private Func<TSource, TKey> KeySelectorFunc { get; set; }

		public KeyEquality(Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> eq) {
			InnerEquality = eq;
			KeySelectorExpr = keySelector;
			KeySelectorFunc = Fun.MemoizeCompile(keySelector);

		}

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
