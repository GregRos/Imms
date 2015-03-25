using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal static class FastComparer<TKey>
	{
		public static readonly IComparer<TKey> Value;

		static FastComparer()
		{
			var t = typeof(TKey);
			
			if (t == typeof(byte))
			{
				Value = (IComparer<TKey>)Comparer<byte>.Create((a, b) => a - b);
			}
			else if (t == typeof(short))
			{
				Value = (IComparer<TKey>)Comparer<short>.Create((a, b) => a - b);
			}
			else if (t == typeof(int))
			{
				Value = (IComparer<TKey>)Comparer<int>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(long))
			{
				Value = (IComparer<TKey>)Comparer<long>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(uint))
			{
				Value = (IComparer<TKey>)Comparer<uint>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(ushort))
			{
				Value = (IComparer<TKey>)Comparer<ushort>.Create((a, b) => a - b);
			}
			else if (t == typeof(sbyte))
			{
				Value = (IComparer<TKey>)Comparer<sbyte>.Create((a, b) => a - b);
			}
			else if (t == typeof(double))
			{
				Value = (IComparer<TKey>)Comparer<double>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(float))
			{
				Value = (IComparer<TKey>)Comparer<float>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(string))
			{
				Value = (IComparer<TKey>)Comparer<string>.Create(String.CompareOrdinal);
			}
			else if (t == typeof(decimal))
			{
				Value = (IComparer<TKey>)Comparer<decimal>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(char))
			{
				Value = (IComparer<TKey>)Comparer<char>.Create((a, b) => a - b);
			}
			else if (t == typeof(bool))
			{
				Value = (IComparer<TKey>)Comparer<bool>.Create((a, b) => a == b ? 0 : a ? 1 : -1);
			}
			else
			{
				Value = Comparer<TKey>.Default;
			}
		}

	}
}
