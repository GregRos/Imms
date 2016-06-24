using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Imms.Abstract;
#pragma warning disable 618
namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (VectorDebugView<>))]
	public partial class ImmVector<T> {

		/// <summary>
		/// Should not be visible.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is not meant to be used.", true)]
		[CompilerGenerated]
		public IEnumerable<T> AsSeq {
			get { return this; }
		}
	}

	class VectorDebugView<T> {
		public VectorDebugView(ImmVector<T> arr) {
			View = new SequentialDebugView<T>(arr);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public SequentialDebugView<T> View { get; private set; }
	}
}