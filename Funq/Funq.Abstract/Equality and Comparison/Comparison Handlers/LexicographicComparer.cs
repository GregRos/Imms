using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class LexicographicComparer<TElem> : IComparer<ITrait_Sequential<TElem>>
	{
		private readonly IComparer<TElem> _comparer;
		public LexicographicComparer(IComparer<TElem> comparer)
		{
			_comparer = comparer;
		}

		int IComparer<ITrait_Sequential<TElem>>.Compare(ITrait_Sequential<TElem> a, ITrait_Sequential<TElem> b)
		{
			return this.Compare(a, b).ToInt();
		}

		Cmp Compare(ITrait_Sequential<TElem> a, ITrait_Sequential<TElem> b)
		{
			return Equality.List_CompareLex(a, b, _comparer);
		}
			
	}
}