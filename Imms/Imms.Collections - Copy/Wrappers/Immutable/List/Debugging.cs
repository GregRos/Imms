using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Imms.Abstract;

namespace Imms {

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ListDebugView<>))]
	public partial class ImmList<T> {
		/// <summary>
		/// Not meant to be user-visible
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is not meant to be used.", true)]
		[CompilerGenerated]
		public IEnumerable<T> AsSeq {
			get {
				return this;
			}
		}

	}

	class ListDebugView<T> {
		private readonly ImmList<T> _x;

		public ListDebugView(ImmList<T> x) {
			_x = x;

		}
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public SequentialDebugView<T> DebugView {
			get {
				return new SequentialDebugView<T>(_x);
			}
		}
	}
}