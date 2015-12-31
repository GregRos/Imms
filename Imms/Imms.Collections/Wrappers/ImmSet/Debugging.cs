using System.Diagnostics;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmSet<>.SetDebugView))]
	partial class ImmSet<T> {
		class SetDebugView {
			public SetDebugView(ImmSet<T> set) {
				IterableView = new IterableDebugView(set);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView { get; private set; }
		}
	}
}