using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Funq {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FunqVector<>.VectorDebugView))]
	public partial class FunqVector<T> {

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<T> AsSeq {
			get { return this; }
		}

		class VectorDebugView {
			public VectorDebugView(FunqVector<T> arr) {
				View = new SequentialDebugView(arr);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SequentialDebugView View { get; private set; }
		}
	}
}