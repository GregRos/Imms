using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FunqList<>.ListDebugView))]
	public partial class FunqList<T>
	{
		private class ListDebugView
		{
			public ListDebugView(FunqList<T> list) 
			{
				View = new SequentialDebugView(list);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SequentialDebugView View
			{
				get;
				set;
			}
		}

		public IEnumerable<T> AsSeq
		{
			get
			{
				return this;
			}
		}
	}
}