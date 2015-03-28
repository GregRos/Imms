using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqMap<TKey, TValue>
	{
		public static FunqMap<TKey, TValue> operator +(FunqMap<TKey, TValue> list, KeyValuePair<TKey,TValue> item)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.Add(item);
		}

		public static FunqMap<TKey, TValue> operator +(FunqMap<TKey, TValue> list, IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddRange(items);
		}

	}
}