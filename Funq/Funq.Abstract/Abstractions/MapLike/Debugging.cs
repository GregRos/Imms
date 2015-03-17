using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	partial class Trait_KeyValueMap<TKey, TValue, TMap>
	{
		protected internal abstract class DebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly TMap _map;
			protected DebugView(TMap map)
			{
				_map = map;
			}

			public Kvp<TKey,TValue>[] Items
			{
				get
				{
					return _map.ToArray();
				}
			}
		}

		
	}
}
