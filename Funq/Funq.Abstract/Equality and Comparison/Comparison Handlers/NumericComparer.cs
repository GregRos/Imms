using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class NumericComparer<TElem> : IComparer<ITrait_Sequential<TElem>>
	{
		private readonly IComparer<TElem> _comparer;

		public NumericComparer(IComparer<TElem> comparer)
		{
			_comparer = comparer;
		}

		int IComparer<ITrait_Sequential<TElem>>.Compare(ITrait_Sequential<TElem> x, ITrait_Sequential<TElem> y)
		{
			return Equality.List_CompareNum(x, y, _comparer);
		}
	}
}