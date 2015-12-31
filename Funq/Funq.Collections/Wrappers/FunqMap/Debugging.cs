using System.Diagnostics;

namespace Funq {
	[DebuggerTypeProxy(typeof (FunqMap<,>.MapDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	partial class FunqMap<TKey, TValue> {
		class MapDebugView {
			public MapDebugView(FunqMap<TKey, TValue> map) {
				IterableView = new IterableDebugView(map);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView { get; set; }
		}
	}
}