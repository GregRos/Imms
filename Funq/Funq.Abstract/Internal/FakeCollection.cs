using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Funq.Abstract {
	/// <summary>
	///     Fake collection encapsulating an IEnumerable[T]. Readonly and not very functional, but gets length in O(1). <br />
	///     Only reason for existence is because IDictionary[TKey, TValue] requires it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class FakeCollection<T> : ICollection<T> {
		readonly int _count;
		readonly IEnumerable<T> _inner;

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
			var i = arrayIndex;
			foreach (var item in _inner) {
				array[i] = item;
				i++;
			}
		}

		public bool Remove(T item) {
			throw Errors.Collection_readonly;
		}

		public int Count {
			get { return _count; }
		}

		public bool IsReadOnly {
			get { return true; }
		}
	}
}