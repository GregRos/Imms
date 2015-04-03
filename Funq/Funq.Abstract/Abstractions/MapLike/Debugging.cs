using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	/// <summary>
	/// The base class of all key-value maps.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <typeparam name="TMap"></typeparam>
	partial class AbstractMap<TKey, TValue, TMap>
	{
		/// <summary>
		/// The base DebugView for AbstractMaps.
		/// </summary>
		protected internal abstract class DebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly TMap _map;
			
			/// <param name="map"></param>
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

	}
}
