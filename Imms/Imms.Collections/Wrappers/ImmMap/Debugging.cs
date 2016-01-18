using System.Collections.Generic;
using System.Diagnostics;
using Imms.Abstract;

namespace Imms {

	[DebuggerTypeProxy(typeof (ImmMapDebugView<,>))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	partial class ImmMap<TKey, TValue> {
	}

	internal class ImmMapDebugView<TKey, TValue> {
		public ImmMapDebugView(ImmMap<TKey, TValue> map) {
			IterableView = new IterableDebugView<KeyValuePair<TKey, TValue>>(map);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IterableDebugView<KeyValuePair<TKey, TValue>> IterableView { get; set; }
	}
}