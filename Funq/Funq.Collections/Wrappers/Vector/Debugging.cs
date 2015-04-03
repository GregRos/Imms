using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq;
using Funq.Abstract;
namespace Funq.Collections
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(FunqVector<>.ArrayDebugView))]
	public partial class FunqVector<T>
	{

		private class ArrayDebugView
		{
			public ArrayDebugView(FunqVector<T> arr)
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
			this.CopyTo(array, 0, arrayIndex, array.Length - arrayIndex);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<T> AsSeq
		{
			get
			{
				return this;
			}
		} 
	}
}