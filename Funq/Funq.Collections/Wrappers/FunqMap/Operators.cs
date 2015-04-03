using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqMap<TKey, TValue>
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqMap<TKey, TValue> op_Add(Tuple<TKey, TValue> item)
		{
			return Add(item.Item1, item.Item2);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqMap<TKey, TValue> op_AddRange(IEnumerable<Tuple<TKey, TValue>> items)
		{
			return base.Merge(items.Select(Kvp.FromTuple), null);
		}

	}
}