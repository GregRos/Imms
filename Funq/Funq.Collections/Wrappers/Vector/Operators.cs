using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqVector<T>
	{

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqVector<T> op_AddLast(T b)
		{
			return AddLast(b);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqVector<T> op_AddLastRange(IEnumerable<T> b)
		{
			return AddLastRange(b);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqVector<T> op_AddFirstRange(IEnumerable<T> a) {
			return AddFirstRange(a);
		}
	}
}