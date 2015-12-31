using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Imms.Abstract {

	static class EqualityHelper {

		public static int Map_HashCode<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> map,
			IEqualityComparer<TKey> kEquality = null, IEqualityComparer<TValue> vEquality = null) {
			var hash = 0;
			vEquality = vEquality ?? FastEquality<TValue>.Default;
			kEquality = kEquality ?? FastEquality<TKey>.Default;
			unchecked {
				var dict = new Dictionary<TKey, TValue>(kEquality);
				map.ForEach(kvp => dict[kvp.Key] = kvp.Value);

			}

			unchecked {
				map.ForEach(pair => { hash ^= (kEquality.GetHashCode(pair.Key) * 31) ^ vEquality.GetHashCode(pair.Value); });
			}
			return hash;
		}

		static Func<TKey, Optional<TValue>> GetValueSelectorFor<TKey, TValue>(
			IEnumerable<KeyValuePair<TKey, TValue>> map) {
			if (map is IDictionary<TKey, TValue>) {
				var asDict = map as IDictionary<TKey, TValue>;
				return k => asDict.ContainsKey(k) ? asDict[k].AsOptional() : Optional.None;
			}
			return null;
		}

		public static bool MapEquals<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> map1,
			IEnumerable<KeyValuePair<TKey, TValue>> map2, IEqualityComparer<TValue> eq = null) {
			var boiler = BoilerEquality(map1, map2);
			if (boiler.IsSome) return boiler.Value;
			eq = eq ?? FastEquality<TValue>.Default;
			var len1 = map1.TryGuessLength();
			var len2 = map2.TryGuessLength();
			if (len1.IsSome && len2.IsSome && len1.Value != len2.Value) return false;
			var getValue1 = GetValueSelectorFor(map1);
			if (getValue1 == null) {
				var dict = map1.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				getValue1 = GetValueSelectorFor(dict);
			}
			return map2.ForEachWhile(kvp => {
				var v1 = kvp.Value;
				var v2 = getValue1(kvp.Key);
				if (v2.IsNone) return false;
				var x = eq.Equals(v1, v2.Value);
				return x;
			});
		}

		public static int SeqHashCode<TElem>(IEnumerable<TElem> obj, IEqualityComparer<TElem> equality = null) {
			equality = equality ?? FastEquality<TElem>.Default;
			var hash = 2166136261;
			const uint prime = 16777619;
			obj.ForEach(item => {
				unchecked {
					var k = (uint) equality.GetHashCode(item);
					hash ^= k;
					hash *= prime;
				}
			});
			return (int) hash;
		}

		public static bool SetEquals<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IEqualityComparer<TElem> eq = null) {
			var boiler = BoilerEquality(x, y);
			if (boiler.IsSome) return boiler.Value;
			if (eq == null) {
				var iset = x as ISet<TElem>;
				if (iset != null) {
					return iset.SetEquals(y);
				}
				eq = FastEquality<TElem>.Default;
			}
			var hs = new HashSet<TElem>(x, eq);
			return y.ForEachWhile(hs.Contains);
		}

		public static int SetHashCode<TElem>(IEnumerable<TElem> obj, IEqualityComparer<TElem> equality = null) {
			equality = equality ?? FastEquality<TElem>.Default;
			var hash = 0;
			obj.ForEach(x => { hash ^= equality.GetHashCode(x); });
			return hash;
		}

		public static bool SeqEquals<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y,
			IEqualityComparer<TElem> equality = null) {
			var boiler = BoilerEquality(x, y);
			if (boiler.IsSome) return boiler.Value;
			equality = equality ?? FastEquality<TElem>.Default;

			var xLen = x.TryGuessLength();
			var yLen = y.TryGuessLength();
			if (xLen.IsSome && yLen.IsSome && xLen.Value != yLen.Value) return false;
			using (var yIter = y.GetEnumerator()) {
				return x.ForEachWhile(v => {
					if (!yIter.MoveNext()) return false;
					return equality.Equals(v, yIter.Current);
				});
			}
		}

		[Pure]
		internal static Optional<bool> BoilerEquality<T>(T left, T right) {
			if (left is ValueType) return Optional.None;
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(null, right)) return false;
			return Optional.None;
		}

		public static int SeqCompareLex<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IComparer<TElem> comparer = null) {
			var boiler = BoilerEquality(x, y);
			if (boiler.Equals(true)) return 0;
			if (y.HasEfficientForEach() && !x.HasEfficientForEach()) return -1 * SeqCompareLex(y, x, comparer);
			comparer = comparer ?? FastComparer<TElem>.Default;
			using (var yIter = y.GetEnumerator()) {
				var finalResult = 0;
				x.ForEachWhile(v => {
					if (!yIter.MoveNext()) return false;
					var compResult = comparer.Compare(v, yIter.Current);
					if (compResult == 0) return true;
					finalResult = compResult;
					return false;
				});
				return finalResult;
			}
		}

		public static int SeqCompareNum<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IComparer<TElem> comparer = null) {
			var boiler = BoilerEquality(x, y);
			if (boiler.Equals(true)) return 0;
			comparer = comparer ?? FastComparer<TElem>.Default;
			var xLen = x.TryGuessLength();
			var yLen = y.TryGuessLength();
			if (xLen.IsSome && yLen.IsSome)
			{
				if (xLen.Value < yLen.Value) return -1;
				if (xLen.Value > yLen.Value) return 1;
			}
			if (y.HasEfficientForEach() && !x.HasEfficientForEach()) return -1 * SeqCompareNum(y, x, comparer);
			comparer = comparer ?? FastComparer<TElem>.Default;


			using (var yIter = y.GetEnumerator()) {
				var nResult = 0;
				x.ForEachWhile(xItem => {
					if (!yIter.MoveNext()) {
						nResult = 1;
						return false;
					}
					var yItem = yIter.Current;
					nResult = nResult == 0 ? comparer.Compare(xItem, yItem) : nResult;
					return true;
				});
				return yIter.MoveNext() ? -1 : nResult;
			}
		}

	}
}