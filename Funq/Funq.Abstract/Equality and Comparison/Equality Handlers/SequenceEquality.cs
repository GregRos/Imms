using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class SequenceEquality<TElem> : IEqualityComparer<ITrait_Sequential<TElem>>
	{
		private const uint M = 0x5bd1e995;
		private const int R = 24;
		private const uint SEED = 0xc58f1a7b;
		private readonly IEqualityComparer<TElem> _equality;

		public SequenceEquality(IEqualityComparer<TElem> equality)
		{
			_equality = equality;
		}

		public bool Equals(ITrait_Sequential<TElem> x, ITrait_Sequential<TElem> y)
		{
			return Equality.List_Equate(x, y, _equality);
		}

		public int GetHashCode(ITrait_Sequential<TElem> obj)
		{
			return Equality.List_HashCode(obj, _equality);
		}
	}
}