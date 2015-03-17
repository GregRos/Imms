using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq.Collections.Common;

namespace Funq.Collections
{
	public partial class FunqList<T>
	{
		public static FunqList<T> operator +(T item, FunqList<T> list)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddFirst(item);
		}

		public static FunqList<T> operator +(FunqList<T> list, T item)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLast(item);
		}

		public static FunqList<T> operator +(FunqList<T> list, IEnumerable<T> items)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddLastRange(items);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static FunqList<T> operator +(IEnumerable<T> items, FunqList<T> list)
		{
			if (ReferenceEquals(list, null)) throw Funq.Errors.Argument_null("list");
			return list.AddFirstRange(items);
		}

		
		
	}
}