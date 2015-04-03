using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	internal class KeyComparer<TSource, TKey> : EquatableComparerBase<TSource, KeyComparer<TSource, TKey>> {

		private IComparer<TKey> InnerComparer { get; set; }
		private Expression<Func<TSource, TKey>> KeySelectorExpr { get; set; }

		private Func<TSource, TKey> KeySelectorFunc { get; set; }

		public KeyComparer(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
		{
			InnerComparer = comparer;
			KeySelectorExpr = keySelector;
			KeySelectorFunc = Fun.MemoizeCompile(keySelector);
		}

		public override int Compare(TSource x, TSource y)
		{
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
