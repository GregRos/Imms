using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class LambdaComparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> _comparer;

		internal LambdaComparer(Func<T, T, int> comparer)
		{
			_comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}
	}
}