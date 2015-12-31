using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imm.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(ImmSet<>.SetDebugView))]
	partial class ImmSet<T>
	{
		private class SetDebugView
		{
			public SetDebugView(ImmSet<T> set)
			{
				IterableView = new IterableDebugView(set);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView
			{
				get; set;
			}
		}
	}
}
