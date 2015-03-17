using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class SetEquality<TElem> : IEqualityComparer<ITrait_SetLike<TElem>>
	{
		private IEqualityComparer<TElem> _equality;

		public SetEquality(IEqualityComparer<TElem> equality)
		{
			_equality = equality;
		}

		public bool Equals(ITrait_SetLike<TElem> x, ITrait_SetLike<TElem> y)
		{
			return Equality.Set_Equate(x, y);
		}

		public int GetHashCode(ITrait_SetLike<TElem> obj)
		{
			return Equality.Set_HashCode(obj);
		}
	}
}