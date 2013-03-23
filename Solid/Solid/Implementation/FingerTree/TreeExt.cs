using System;

namespace Solid.FingerTree
{
	internal static class TreeExt
	{
		internal static T Item<T>(this FTree<Value<T>> ftree, int index)
		{
			Measured leaf = ftree.Get(index);
			var leaf2 = leaf as Value<T>;
			return leaf2.Content;
		}

		internal static void Iter<T>(this FTree<Value<T>> ftree, Action<T> action)
		{
			ftree.Iter(x => action((x as Value<T>).Content));
		}

		internal static void IterBack<T>(this FTree<Value<T>> ftree, Action<T> action)
		{
			ftree.IterBack(x => action((x as Value<T>).Content));
		}
	}
}