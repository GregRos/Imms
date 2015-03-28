using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	partial class AbstractMap<TKey, TValue, TMap>
	{
		protected internal abstract class DebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly TMap _map;
			protected DebugView(TMap map)
			{
				_map = map;
			}

			public KeyValuePair<TKey,TValue>[] Items
			{
				get
				{
					return _map.ToArray();
				}
			}
		}


		bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
			var r = TryGet(key);
			if (r.IsNone) {
				value = default(TValue);
				return false;
			}
			value = r.Value;
			return true;
		}
	}
}
