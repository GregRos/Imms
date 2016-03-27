using System;
using System.Collections;
using System.Collections.Generic;

namespace Imms.Abstract {
	public abstract partial class AbstractSequential<TElem, TList> : IList<TElem>, IList, IReadOnlyList<TElem> {
	
		void IList.RemoveAt(int index) {
			throw Errors.Collection_readonly;
		}

		object IList.this[int index] {
			get { return this[index]; }
			set { throw Errors.Collection_readonly; }
		}

		bool IList.IsReadOnly {
			get { return true; }
		}

		bool IList.IsFixedSize {
			get { return true; }
		}

		int IList.Add(object value) {
			throw Errors.Collection_readonly;
		}

		bool IList.Contains(object value) {
			return Any(x => value.Equals(x));
		}

		void IList.Clear() {
			throw Errors.Collection_readonly;
		}

		int IList.IndexOf(object value) {
			return FindIndex(x => Equals(x, value)) | -1;
		}

		void IList.Insert(int index, object value) {
			throw Errors.Collection_readonly;
		}

		void IList.Remove(object value) {
			throw Errors.Collection_readonly;
		}

		TElem IList<TElem>.this[int index] {
			get { return this[index]; }
			set { throw Errors.Collection_readonly; }
		}

		int IList<TElem>.IndexOf(TElem item) {
			return FindIndex(item) | -1;
		}

		void IList<TElem>.Insert(int index, TElem item) {
			throw Errors.Collection_readonly;
		}

		void IList<TElem>.RemoveAt(int index) {
			throw Errors.Collection_readonly;
		}
	}
}