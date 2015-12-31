using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Funq {

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FunqList<>.ListDebugView))]
	public partial class FunqList<T> {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<T> AsSeq {
			get {
				return this;
			}
		}

		class ListDebugView {
			private readonly FunqList<T> _x;

			public ListDebugView(FunqList<T> x) {
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