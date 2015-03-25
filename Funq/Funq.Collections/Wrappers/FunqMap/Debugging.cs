using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections
{
	[DebuggerTypeProxy(typeof(FunqMap<,>.MapDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	partial class FunqMap<TKey, TValue>
	{
		private class MapDebugView
		{
			public MapDebugView(FunqMap<TKey, TValue> map)
			{
				IterableView = new IterableDebugView(map);
			}
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView
			{
				get; set;
			}
		}
	}
}
