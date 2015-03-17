using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections
{
	partial class FunqList<T> {
		private class EqualityComparer : IEqualityComparer<IEnumerable<T>> {
			public bool Equals(IEnumerable<T> x, IEnumerable<T> y) {
				throw new NotImplementedException();
			}

			public int GetHashCode(IEnumerable<T> obj) {
				throw new NotImplementedException();
			}
		}
	}
}
