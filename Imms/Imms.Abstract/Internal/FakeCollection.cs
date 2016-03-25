using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imms.Abstract {
	/// <summary>
	///     Fake collection encapsulating an IEnumerable[T]. Readonly and not very functional, but gets length in O(1). <br />
	///     Only reason for existence is because IDictionary[TKey, TValue] requires it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class FakeCollection<T> : ICollection<T>, ICollection {
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

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.-or-The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(Array array, int index) {
			CopyTo((T[])array, index);
		}

		public int Count {
			get { return _count; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <returns>
		/// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </returns>
		public object SyncRoot { get; } = new object();

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
		/// </summary>
		/// <returns>
		/// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
		/// </returns>
		public bool IsSynchronized { get; } = true;

		public bool IsReadOnly {
			get { return true; }
		}
	}
}