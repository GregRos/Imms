using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqVector<T>
	{
		public static FunqVector<T> operator +(FunqVector<T> list, T item)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLast(item);
		}

		public static FunqVector<T> operator +(FunqVector<T> list, IEnumerable<T> items)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLastRange(items);
		}

		public static FunqVector<T> operator +(IEnumerable<T> items, FunqVector<T> list)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddFirstRange(items);
		}
	}
}