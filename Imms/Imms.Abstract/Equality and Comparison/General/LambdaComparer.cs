using System;
using System.Collections.Generic;

namespace Imms.Abstract {
	class LambdaComparer<T> : IComparer<T> {
		readonly Comparison<T> _comparison;

		public LambdaComparer(Comparison<T> comparison) {
			comparison.CheckNotNull("comparison");
			_comparison = comparison;
		}

		public int Compare(T x, T y) {
			return _comparison(x, y);
		}
	}
}