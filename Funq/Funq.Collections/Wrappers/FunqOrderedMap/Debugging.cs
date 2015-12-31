using System.Collections.Generic;
using System.Diagnostics;
using Funq.Abstract;

namespace Funq {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FunqOrderedMap<,>.MapDebugView))]
	partial class FunqOrderedMap<TKey, TValue> {
		class MapDebugView {
			public MapDebugView(FunqOrderedMap<TKey, TValue> map) {
				zIterableView = new IterableDebugView(map);
			}

			public KeyValuePair<TKey, TValue> MaxItem {
				get { return zIterableView.Object.MaxItem; }
			}

			public KeyValuePair<TKey, TValue> MinItem {
				get { return zIterableView.Object.MinItem; }
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView zIterableView { get; set; }
		}

		protected override IMapBuilder<TKey, TValue, FunqOrderedMap<TKey, TValue>> BuilderFrom(FunqOrderedMap<TKey, TValue> collection) {
			return new Builder(collection);
		}
	}
}