using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Funq.Abstract
{
	internal class LambdaComparer<T> : IComparer<T> {
		private readonly Comparison<T> _comparison;

		public LambdaComparer(Comparison<T> comparison)
		{
			comparison.IsNotNull("comparison");
			_comparison = comparison;
		}

		public int Compare(T x, T y) {
			return _comparison(x, y);
		}
	}
}
