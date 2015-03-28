using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	abstract partial class AbstractSequential<TElem,TList>
	{
		protected internal class SequentialDebugView
		{

			public SequentialDebugView(TList list)
			{
				zIterableView = new IterableDebugView(list);
			}

			public TElem First
			{
				get
				{
					return zIterableView.Object.First;
				}
			}

			public TElem Last
			{
				get
				{
					return zIterableView.Object.Last;
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
