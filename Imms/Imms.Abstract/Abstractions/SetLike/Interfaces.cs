using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Imms.Abstract {


	public partial class AbstractSet<TElem, TSet> : ISet<TElem> {



		bool ISet<TElem>.Add(TElem item) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.UnionWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.IntersectWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.ExceptWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.SymmetricExceptWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		bool ISet<TElem>.Overlaps(IEnumerable<TElem> other) {
			return !IsDisjointWith(other);
		}
	}
}