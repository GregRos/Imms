using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq;

namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(FunqArray<>.ArrayDebugView))]
	public partial class FunqArray<T>
	{
		public bool Contains(T item)
		{
			return !this.ForEachWhile(x => item.Equals(x));
		}

		private class ArrayDebugView
		{
			public ArrayDebugView(FunqArray<T> arr)
			{
				View = new SequentialDebugView(arr);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SequentialDebugView View
			{
				get; set;
			}
		}


		public void CopyTo(T[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, array.Length - arrayIndex);
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