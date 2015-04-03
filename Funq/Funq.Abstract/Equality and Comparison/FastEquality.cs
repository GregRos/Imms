using System.Collections.Generic;

namespace Funq.Abstract
{

	static class FastEquality<T> {
		public static readonly IEqualityComparer<T> Default;

		static FastEquality() {
			Default = EqualityComparer<T>.Default;
		} 
	}
}
