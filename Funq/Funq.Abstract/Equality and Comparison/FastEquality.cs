using System.Collections.Generic;

namespace Funq.Abstract
{

	static class FastEquality<T> {
		public static readonly IEqualityComparer<T> Value;

		static FastEquality() {
			var t = typeof (T);
			Value = EqualityComparer<T>.Default;
		} 
	}
}
