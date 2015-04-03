using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	public static class InterfaceExtensions 
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

		public static Option<TValue> TryGet<TKey, TValue>(this IAnyMapLike<TKey, TValue> self, TKey key) {
			if (self.ContainsKey(key)) {
				return self[key].AsSome();
			}
			return Option.None;
		}

	}
}
