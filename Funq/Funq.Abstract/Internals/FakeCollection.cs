using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	internal class FakeCollection<T> : ICollection<T> {
		readonly IEnumerable<T> _inner;
		readonly int _count;

		public FakeCollection(IEnumerable<T> inner, int count) {
			_inner = inner;
			_count = count;
		}

		public IEnumerator<T> GetEnumerator() {
			return _inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(T item) {
			throw Errors.Collection_readonly;
		}

		public void Clear() {
			throw Errors.Collection_readonly;
		}

		public bool Contains(T item) {
			return _inner.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			int i = arrayIndex;
			foreach (var item in _inner) {
				array[i] = item;
				i++;
			}
		}

		public bool Remove(T item) {
			throw Errors.Collection_readonly;
		}

		public int Count {
			get {
				return _count;
			}
		}

		public bool IsReadOnly {
			get {
				return true;
			}
		}
	}
}
