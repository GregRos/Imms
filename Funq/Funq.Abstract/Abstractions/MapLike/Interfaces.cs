using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Funq.Abstract {
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_Remove(TKey item)
		{
			return Remove(item);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_RemoveRange(IEnumerable<TKey> items) {
			return RemoveRange(items);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_Add(KeyValuePair<TKey, TValue> item) {
			return Add(item.Key, item.Value);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
			return SetRange(items);
		}
	
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_Add(Tuple<TKey, TValue> item)
		{
			return Add(item.Item1, item.Item2);
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TMap op_AddRange(IEnumerable<Tuple<TKey, TValue>> items)
		{
			return SetRange(items.Select(x => Kvp.Of(x)));
		}
	}
}