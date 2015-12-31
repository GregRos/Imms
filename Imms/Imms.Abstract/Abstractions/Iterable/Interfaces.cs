using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imms.Abstract
{
	public abstract partial class AbstractIterable<TElem, TIterable, TBuilder> : ICollection<TElem>, ICollection {
		void ICollection<TElem>.Add(TElem item) {
			throw Errors.Collection_readonly;
		}

		void ICollection<TElem>.Clear() {
			throw Errors.Collection_readonly;
		}

		bool ICollection<TElem>.Contains(TElem item) {
			return Find(x => Equals(x, item)).IsSome;
		}

		bool ICollection<TElem>.Remove(TElem item) {
			throw Errors.Collection_readonly;
		}

		void ICollection.CopyTo(Array array, int index) {
			CopyTo((TElem[])array, index, Length);
		}

		int ICollection.Count
		{
			get { return Length; }
		}

		object ICollection.SyncRoot {
			get;
		} = new object();

		bool ICollection.IsSynchronized
		{
			get { return true; }
		}

		int ICollection<TElem>.Count
		{
			get { return Length; }
		}

		bool ICollection<TElem>.IsReadOnly
		{
			get { return true; }
		}
	}
}
