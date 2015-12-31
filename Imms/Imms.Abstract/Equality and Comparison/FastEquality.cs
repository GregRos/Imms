using System.Collections.Generic;

namespace Imms.Abstract {

	static class FastEquality<T> {
		public static readonly IEqualityComparer<T> Default = EqualityComparer<T>.Default;

	}
}