using System.Diagnostics;

namespace Imms {
	[DebuggerTypeProxy(typeof (ImmMap<,>.MapDebugView))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	partial class ImmMap<TKey, TValue> {
		class MapDebugView {
			public MapDebugView(ImmMap<TKey, TValue> map) {
				IterableView = new IterableDebugView(map);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView { get; set; }
		}
	}
}