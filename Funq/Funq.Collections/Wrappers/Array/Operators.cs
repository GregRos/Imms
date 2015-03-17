using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqArray<T>
	{
		public static FunqArray<T> operator +(FunqArray<T> list, T item)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLast(item);
		}

		public static FunqArray<T> operator +(FunqArray<T> list, IEnumerable<T> items)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLastRange(items);
		}

		public static FunqArray<T> operator +(IEnumerable<T> items, FunqArray<T> list)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddFirstRange(items);
		}
	}
}