using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmVector<>.VectorDebugView))]
	public partial class ImmVector<T> {

		/// <summary>
		/// Should not be visible.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is not meant to be used.", true)]
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