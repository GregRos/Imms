using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	public abstract partial class AbstractMap<TKey, TValue, TMap> : IDictionary<TKey, TValue> {
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			var found = this.TryGet(item.Key);
			return found.IsSome && found.Value.Equals(item.Value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, Length);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw Errors.Collection_readonly;
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count
		{
			get { return Length; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return true; }
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw Errors.Collection_readonly;
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair)
		{
			throw Errors.Collection_readonly;
		}


		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			var tryGet = TryGet(key);
			if (tryGet.IsSome)
			{
				value = tryGet.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return this[key]; }
			set { throw Errors.Collection_readonly; }
		}


		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get { return new FakeCollection<TKey>(Keys, Length); }
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get { return new FakeCollection<TValue>(Values, Length); }
		}

	}
}
