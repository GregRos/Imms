using System.Diagnostics;
using Imms.Abstract;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmSetDebugView<>))]
	partial class ImmSet<T> {
	}

	class ImmSetDebugView<T> {
		public ImmSetDebugView(ImmSet<T> set) {
			IterableView = new IterableDebugView<T>(set);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IterableDebugView<T> IterableView { get; private set; }
	}
}