using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Imms {

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmList<>.ListDebugView))]
	public partial class ImmList<T> {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<T> AsSeq {
			get {
				return this;
			}
		}

		class ListDebugView {
			private readonly ImmList<T> _x;

			public ListDebugView(ImmList<T> x) {
				_x = x;

			}

			public SequentialDebugView DebugView {
				get {
					return new SequentialDebugView(_x);
				}
			}
		}
	}
}