using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Solid
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (FlexibleList<>.FlexibleListDebugView))]
	public partial class FlexibleList<T>
	{
		private class FlexibleListDebugView
		{
			private readonly FlexibleList<T> _inner;

			public FlexibleListDebugView(FlexibleList<T> inner)
			{
				_inner = inner;
			}

			public int Count
			{
				get
				{
					return _inner.Count;
				}
			}

			public T First
			{
				get
				{
					return _inner.First;
				}
			}

			public T Last
			{
				get
				{
					return _inner.Last;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Zitems
			{
				get
				{
					return _inner.ToArray();
				}
			}
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("FlexibleList, Count = {0}", Count);
			}
		}
	}
}