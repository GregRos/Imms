using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal static class FastComparer<TKey>
	{
		public static readonly IComparer<TKey> Default;

		static FastComparer()
		{
			var t = typeof(TKey);
			
			if (t == typeof(byte))
			{
				Default = (IComparer<TKey>)Comparer<byte>.Create((a, b) => a - b);
			}
			else if (t == typeof(short))
			{
				Default = (IComparer<TKey>)Comparer<short>.Create((a, b) => a - b);
			}
			else if (t == typeof(int))
			{
				Default = (IComparer<TKey>)Comparer<int>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(long))
			{
				Default = (IComparer<TKey>)Comparer<long>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(uint))
			{
				Default = (IComparer<TKey>)Comparer<uint>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(ushort))
			{
				Default = (IComparer<TKey>)Comparer<ushort>.Create((a, b) => a - b);
			}
			else if (t == typeof(sbyte))
			{
				Default = (IComparer<TKey>)Comparer<sbyte>.Create((a, b) => a - b);
			}
			else if (t == typeof(double))
			{
				Default = (IComparer<TKey>)Comparer<double>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(float))
			{
				Default = (IComparer<TKey>)Comparer<float>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(string))
			{
				Default = (IComparer<TKey>)Comparer<string>.Create(String.CompareOrdinal);
			}
			else if (t == typeof(decimal))
			{
				Default = (IComparer<TKey>)Comparer<decimal>.Create((a, b) => a > b ? 1 : a < b ? -1 : 0);
			}
			else if (t == typeof(char))
			{
				Default = (IComparer<TKey>)Comparer<char>.Create((a, b) => a - b);
			}
			else if (t == typeof(bool))
			{
				Default = (IComparer<TKey>)Comparer<bool>.Create((a, b) => a == b ? 0 : a ? 1 : -1);
			}
			else
			{
				Default = Comparer<TKey>.Default;
			}
		}

	}
}
