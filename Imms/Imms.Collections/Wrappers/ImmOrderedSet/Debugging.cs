using System.Diagnostics;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmOrderedSet<>.SetDebugView))]
	partial class ImmOrderedSet<T> {
		class SetDebugView {
			public SetDebugView(ImmOrderedSet<T> set) {
				zIterableView = new IterableDebugView(set);
			}

			public T MaxItem {
				get { return zIterableView.Object.MaxItem; }
			}

			public T MinItem {
				get { return zIterableView.Object.MinItem; }
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView zIterableView { get; set; }
		}


	}
}