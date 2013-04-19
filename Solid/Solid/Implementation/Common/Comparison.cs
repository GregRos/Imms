using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid.Common
{


	internal static class Comparisons
	{
		

		private const uint M = 0x5bd1e995;
		private const int R = 24;
		private const uint SEED = 0xc58f1a7b;

		public static bool? RefEquality(object a, object b)
		{
			var aIsNull = ReferenceEquals(a, null);
			var bIsNull = ReferenceEquals(b, null);
			if (aIsNull || bIsNull) return aIsNull && bIsNull;
			if (ReferenceEquals(a, b)) return true;
			return null;
		}

		public static uint SequenceHash<T>(Action<Action<T>> iterateWithFunc, int count, IEqualityComparer<T> equality)
		{
		
			var hash = (uint) (SEED ^ count);
			iterateWithFunc(v =>
			         {
				         unchecked
				         {
					         //This is MurMur Hash.
							 var k = (uint)equality.GetHashCode(v);
					         k = ((k * M) >> R) * M;
					         hash = (hash * M) ^ k;
				         }
			         });
			unchecked
			{
				hash ^= ((hash >> 13) * M) >> 15;
			}
			return hash;
		}

		public static int SequenceCompare<T>(Action<Func<T, bool>> iterateWithFunc, IEnumerable<T> second, IComparer<T> comparer)
		{
			int result = 0;
			using (var iterator = second.GetEnumerator())
			{
				iterateWithFunc(v =>
				                {
					                if (!iterator.MoveNext())
					                {
						                result = 1;
						                return false;
					                }
					                result = comparer.Compare(v, iterator.Current);
					                if (result != 0) return false;
					                return true;
				                });

				return iterator.MoveNext() && result == 0 ? -1 : result;
			}
		}

		public static bool SequenceEquate<T>(Action<Func<T, bool>> iterateWithFunc, IEnumerable<T> second, IEqualityComparer<T> equality)
		{
			var result = true;
			using (var iterator = second.GetEnumerator())
			{
				iterateWithFunc(v =>
				                {
					                if (!iterator.MoveNext())
					                {
						                result = false;
						                return false;
					                }
					                result = equality.Equals(v, iterator.Current);
					                if (!result) return false;
					                return true;
				                });
				return !iterator.MoveNext() && result;
			}
		}
	}
}
