using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	public partial class AbstractSet<TElem, TSet> : ISet<TElem> {
		void ICollection<TElem>.Clear()
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<TElem>.Add(TElem item)
		{
			throw Errors.Collection_readonly;
		}

		bool ISet<TElem>.Add(TElem item)
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<TElem>.Remove(TElem item)
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<TElem>.CopyTo(TElem[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex, Length);
		}

		int ICollection<TElem>.Count
		{
			get { return Length; }
		}

		bool ICollection<TElem>.IsReadOnly
		{
			get { return true; }
		}

		void ISet<TElem>.UnionWith(IEnumerable<TElem> other)
		{
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.IntersectWith(IEnumerable<TElem> other)
		{
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.ExceptWith(IEnumerable<TElem> other)
		{
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.SymmetricExceptWith(IEnumerable<TElem> other)
		{
			throw Errors.Collection_readonly;
		}

		bool ISet<TElem>.Overlaps(IEnumerable<TElem> other)
		{
			return !IsDisjointWith(other);
		}		
	}
}
