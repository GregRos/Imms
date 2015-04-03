using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(FunqSet<>.SetDebugView))]
	partial class FunqSet<T>
	{
		private class SetDebugView
		{
			public SetDebugView(FunqSet<T> set)
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
