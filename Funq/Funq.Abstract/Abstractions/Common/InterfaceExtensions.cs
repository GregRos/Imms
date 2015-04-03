using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	internal static class InterfaceExtensions 
	{
		public static void ForEach<T>(this IAnyIterable<T> iterable, Action<T> act)
		{
			iterable.ForEachWhile(x =>
			{
				act(x);
				return true;
			});
		}

		public static bool IsEmpty<T>(this IAnyIterable<T> iterable)
		{
			return iterable.ForEachWhile(x => false);
		}

	}
}
