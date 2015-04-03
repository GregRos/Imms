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
				zIterableView = new IterableDebugView(set);
			}

			public T MaxItem
			{
				get
				{
					return zIterableView.Object.MaxItem;
				}
			}

			public T MinItem
			{
				get
				{
					return zIterableView.Object.MinItem;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView zIterableView
			{
				get; set;
			}
		}


		
	}
}
