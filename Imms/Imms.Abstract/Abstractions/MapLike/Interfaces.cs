using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Imms.Abstract {
	/// <summary>
	/// Abstracts over maps of any kind.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TMap">A self-reference to the type implementing this class.</typeparam>
	public abstract partial class AbstractMap<TKey, TValue, TMap> 
	: IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
	{
		bool IDictionary<TKey, TValue>.Remove(TKey key) {
			throw Errors.Collection_readonly;
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
			throw Errors.Collection_readonly;
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
			var tryGet = TryGet(key);
			if (tryGet.IsSome) {
				value = tryGet.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		/// <summary>
		/// Gets the value that is associated with the specified key.
		/// </summary>
		/// <returns>
		/// true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/> interface contains an element that has the specified key; otherwise, false.
		/// </returns>
		/// <param name="key">The key to locate.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
		bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
			return (this as IDictionary<TKey, TValue>).TryGetValue(key, out value);
		}

		TValue IDictionary<TKey, TValue>.this[TKey key] {
			get { return this[key]; }
			set { throw Errors.Collection_readonly; }
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IDictionary"/> object contains an element with the specified key.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary"/> contains an element with the key; otherwise, false.
		/// </returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/> object.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null. </exception>
		bool IDictionary.Contains(object key) {
			return ContainsKey((TKey) key);
		}

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"/> object.
		/// </summary>
		/// <param name="key">The <see cref="T:System.Object"/> to use as the key of the element to add. </param><param name="value">The <see cref="T:System.Object"/> to use as the value of the element to add. </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null. </exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"/> object. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> is read-only.-or- The <see cref="T:System.Collections.IDictionary"/> has a fixed size. </exception>
		void IDictionary.Add(object key, object value) {
			throw Errors.Collection_readonly;
		}

		

		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
		void IDictionary.Clear() {
			throw Errors.Collection_readonly;
		}

		/// <summary>
		/// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
		/// </returns>
		IDictionaryEnumerator IDictionary.GetEnumerator() {
			throw Errors.Collection_readonly;
		}

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"/> object.
		/// </summary>
		/// <param name="key">The key of the element to remove. </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only.-or- The <see cref="T:System.Collections.IDictionary"/> has a fixed size. </exception>
		void IDictionary.Remove(object key) {
			throw Errors.Collection_readonly;
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <returns>
		/// The element with the specified key.
		/// </returns>
		/// <param name="key">The key of the element to get or set. </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null. </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IDictionary"/> object is read-only.-or- The property is set, <paramref name="key"/> does not exist in the collection, and the <see cref="T:System.Collections.IDictionary"/> has a fixed size. </exception>
		object IDictionary.this[object key] {
			get {
				return this[(TKey)key];
			}
			set {
				throw Errors.Collection_readonly;
			}
		}

		ICollection IDictionary.Keys {
			get {
				return new FakeCollection<TKey>(Keys, Length);
			}
		}

		ICollection IDictionary.Values {
			get {
				return new FakeCollection<TValue>(Values, Length);
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </returns>
		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get { return new FakeCollection<TKey>(Keys, Length); }
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </returns>
		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get { return new FakeCollection<TValue>(Values, Length); }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary"/> object is read-only; otherwise, false.
		/// </returns>
		bool IDictionary.IsReadOnly {
			get {
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.
		/// </returns>
		bool IDictionary.IsFixedSize {
			get {
				return true;
			}
		}
	}
}