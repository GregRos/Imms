using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Solid
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(Vector<>.VectorDebugView))]
	public partial class Vector<T>
	{
		private class VectorDebugView
		{
			private readonly Vector<T> _inner;

			public VectorDebugView(Vector<T> inner)
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

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] zContents
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
				return string.Format("Vector, Count = {0}", Count);
			}
		}
	}
}