using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(FunqOrderedMap<,>.MapDebugView))]
	partial class FunqOrderedMap<TKey, TValue>
	{
		private class MapDebugView 
		{
			public MapDebugView(FunqOrderedMap<TKey, TValue> map)
			{
				zIterableView = new IterableDebugView(map);
			}

			public Kvp<TKey, TValue> MaxItem
			{
				get
				{
					return zIterableView.Object.MaxItem;
				}
			}

			public Kvp<TKey, TValue> MinItem
			{
				get
				{
					return zIterableView.Object.MinItem;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView zIterableView
			{
				get; set;
			}
		}
	}
}
