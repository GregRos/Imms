using System.Collections.Generic;
using System.Diagnostics;
using Imms.Abstract;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmOrderedMap<,>.MapDebugView))]
	partial class ImmOrderedMap<TKey, TValue> {
		class MapDebugView {
			public MapDebugView(ImmOrderedMap<TKey, TValue> map) {
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

		protected override IMapBuilder<TKey, TValue, ImmOrderedMap<TKey, TValue>> BuilderFrom(ImmOrderedMap<TKey, TValue> collection) {
			return new Builder(collection);
		}
	}
}