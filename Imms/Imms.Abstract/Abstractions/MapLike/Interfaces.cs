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
	: IDictionary<TKey, TValue>
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

		TValue IDictionary<TKey, TValue>.this[TKey key] {
			get { return this[key]; }
			set { throw Errors.Collection_readonly; }
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get { return new FakeCollection<TKey>(Keys, Length); }
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get { return new FakeCollection<TValue>(Values, Length); }
		}
	}
}