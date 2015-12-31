using System.Diagnostics;

namespace Funq {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FunqSet<>.SetDebugView))]
	partial class FunqSet<T> {
		class SetDebugView {
			public SetDebugView(FunqSet<T> set) {
				IterableView = new IterableDebugView(set);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView { get; private set; }
		}
	}
}