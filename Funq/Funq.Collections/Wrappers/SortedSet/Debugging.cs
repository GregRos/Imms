using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(FunqOrderedSet<>.SetDebugView))]
	partial class FunqOrderedSet<T>
	{
		private class SetDebugView
		{
			public SetDebugView(FunqOrderedSet<T> set)
			{
				IterableView = new IterableDebugView(set);
			}

			public T MaxItem
			{
				get
				{
					return IterableView.Object.MaxItem;
				}
			}

			public T MinItem
			{
				get
				{
					return IterableView.Object.MinItem;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView IterableView
			{
				get; set;
			}
		}
	}
}
