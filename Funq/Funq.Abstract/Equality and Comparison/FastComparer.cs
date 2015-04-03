using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal static class FastComparer<TKey>
	{
		private static IComparer<TKey> _defaultComparer;
		private static Option<bool> _isComparable;
		public static IComparer<TKey> Default {
			get {
				if (_isComparable.IsNone) {
					LoadComparer();
				}
				return _defaultComparer;
			}
		}

		private static void LoadComparer() {
			if (_isComparable.IsNone) {
				var tryComparer = TryGetComparer();
				_isComparable = tryComparer.IsSome;
				_defaultComparer = tryComparer.IsSome ? tryComparer.Value : null;
			}
		}

		private static Option<IComparer<TKey>> TryGetComparer() {
			var t = typeof(TKey);
			IComparer<TKey> comparer;
			if (t == typeof (string)) {
				comparer = (IComparer<TKey>) Comparers.CreateComparison<string>(String.CompareOrdinal);
			}
			else {
				comparer = Comparer<TKey>.Default;
			}
			return comparer.AsSome();
		} 


		static FastComparer()
		{
			
		}

	}
}
