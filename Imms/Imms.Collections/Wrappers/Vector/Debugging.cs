using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmVector<>.VectorDebugView))]
	public partial class ImmVector<T> {

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<T> AsSeq {
			get { return this; }
		}

		class VectorDebugView {
			public VectorDebugView(ImmVector<T> arr) {
				View = new SequentialDebugView(arr);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SequentialDebugView View { get; private set; }
		}
	}
}