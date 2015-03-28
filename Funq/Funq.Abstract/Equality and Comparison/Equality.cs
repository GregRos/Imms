using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Funq.Abstract
{
	internal static class Equality
	{
		public static int Map_HashCode<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> map, IEqualityComparer<TKey> kEquality = null, IEqualityComparer<TValue> vEquality = null)
		{
			var hash = 0;
			vEquality = vEquality ?? FastEquality<TValue>.Default;
			kEquality = kEquality ?? FastEquality<TKey>.Default;
			unchecked {
				map.ForEach(pair => { hash ^= (kEquality.GetHashCode(pair.Key)*31) ^ vEquality.GetHashCode(pair.Value); });
			}
			return hash;
		}

		private static Func<TKey, Option<TValue>> GetValueSelectorFor<TKey, TValue>(
			IEnumerable<KeyValuePair<TKey, TValue>> map) {
			if (map is IDictionary<TKey, TValue>) {
				var asDict = map as IDictionary<TKey, TValue>;
				return k => asDict.ContainsKey(k) ? asDict[k].AsSome() : Option.None;
			}
			if (map is IReadOnlyDictionary<TKey, TValue>) {
				var asDict = map as IReadOnlyDictionary<TKey, TValue>;
				return k => asDict.ContainsKey(k) ? asDict[k].AsSome() : Option.None;
			}
			return null;
		} 

		public static bool Map_Equate<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> map1,
			IEnumerable<KeyValuePair<TKey, TValue>> map2, IEqualityComparer<TValue> eq ) {
			var boiler = Boilerplate(map1, map2);
			if (boiler.IsSome) return boiler;
			eq = eq ?? FastEquality<TValue>.Default;
			var len1 = map1.TryGuessLength();
			var len2 = map2.TryGuessLength();
			if (len1.IsSome && len2.IsSome && len1.Value != len2.Value) {
				return false;
			} 
			Func<TKey, Option<TValue>> getValue2 = GetValueSelectorFor(map2);
			if (getValue2 == null) {
				var dict = map2.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				getValue2 = GetValueSelectorFor(dict);
			}
			return map1.ForEachWhile(kvp => {
				var v1 = kvp.Value;
				var v2 = getValue2(kvp.Key);
				if (v2.IsNone) {
					return false;
				}
				var x = eq.Equals(v1, v2);
				if (!x) {
					Debugger.Break();
				}
				return x;
			});
		}

		public static int Seq_HashCode<TElem>(IEnumerable<TElem> obj, IEqualityComparer<TElem> equality = null)
		{
			equality = equality ?? FastEquality<TElem>.Default;
			uint hash = 2166136261;
			const uint prime = 16777619;
			obj.ForEach(item =>
			            {
				            unchecked
				            {
					            var k = (uint) equality.GetHashCode(item);
					            hash ^=  k;
					            hash *= prime;
				            }
			            });
			return (int) hash;
		}

		public static bool Set_Equate<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IEqualityComparer<TElem> eq = null) {
			var boiler = Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			eq = eq ?? FastEquality<TElem>.Default;
			if (x is ISet<TElem>) {
				var xAsSet = x as ISet<TElem>;
				return xAsSet.SetEquals(y);
			}
			if (x is IAnySetLike<TElem>) {
				var xAsAny = x as IAnySetLike<TElem>;
				return xAsAny.SetEquals(y);
			}
			var hs = new HashSet<TElem>(x,eq);
			return y.ForEachWhile(hs.Contains);
		}

		public static int Set_HashCode<TElem>(IEnumerable<TElem> obj, IEqualityComparer<TElem> equality = null)
		{
			equality = equality ?? FastEquality<TElem>.Default;
			var hash = 0;
			obj.ForEach(x => { hash ^= equality.GetHashCode(x); });
			return hash;
		}

		public static bool List_Equate<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IEqualityComparer<TElem> equality = null)
		{
			var boiler = Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			equality = equality ?? FastEquality<TElem>.Default;
			

			var xLen = x.TryGuessLength();
			var yLen = y.TryGuessLength();
			if (xLen.IsSome && yLen.IsSome && xLen.Value != yLen.Value) {
				return false;
			}
			using (var yIter = y.GetEnumerator()) {
				return x.ForEachWhile(v => {
					if (!yIter.MoveNext()) return false;
					return equality.Equals(v, yIter.Current);
				});
			}
		}

		internal static Option<bool> Boilerplate<T>(T left, T right)
		{
			if (left is ValueType) return Option.None;
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(null, right)) return false;
			return Option.None;
		}

		/// <summary>
		///   Returns an equality handler that determines whether two items are equal by using the specified key.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
		/// <param name="keyComparer"> </param>
		/// <returns> </returns>
		public static IEqualityComparer<T> ByKey<T, TKey>(Func<T, TKey> selector, IEqualityComparer<TKey> keyComparer = null)
		{
			keyComparer = keyComparer ?? FastEquality<TKey>.Default;
			return new LambdaEquality<T>((x, y) => keyComparer.Equals(selector(x), selector(y)), x => keyComparer.GetHashCode(selector(x)));
		}

		/// <summary>
		///   <para> Returns an equality handler that wraps another handler, intelligently caching its hash code result. </para>
		///   <para> Note that unexpected behavior can result when using this method to cache the hash code of a potentially mutable object. </para>
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="eq"> </param>
		/// <returns> </returns>
		public static IEqualityComparer<T> Caching<T>(IEqualityComparer<T> eq) where T : class
		{
			return new CachingEqualityComparer<T>(eq);
		}
		
		public static int Seq_CompareLex<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IComparer<TElem> comparer = null)
		{
			var boiler = Boilerplate(x, y);
			if (boiler.Equals(true)) return 0;
			if (y.HasEfficientForEach() && !x.HasEfficientForEach()) {
				return -1*Seq_CompareLex(y, x, comparer);
			}
			comparer = comparer ?? FastComparer<TElem>.Default;
			using (var yIter = y.GetEnumerator())
			{
				int finalResult = 0;
				x.ForEachWhile(v =>
				               {
					               if (!yIter.MoveNext()) return false;
					               var compResult = comparer.Compare(v, yIter.Current);
					               if (compResult == 0) return true;
					               finalResult = compResult;
					               return false;
				               });
				return finalResult;
			}
		}

		public static int Seq_CompareNum<TElem>(IEnumerable<TElem> x, IEnumerable<TElem> y, IComparer<TElem> comparer = null) {
			var boiler = Boilerplate(x, y);
			if (boiler.Equals(true)) return 0;
			comparer = comparer ?? FastComparer<TElem>.Default;
			if (y.HasEfficientForEach() && !x.HasEfficientForEach())
			{
				return -1 * Seq_CompareNum(y, x, comparer);
			}
			comparer = comparer ?? FastComparer<TElem>.Default;
			var xLen = x.TryGuessLength();
			var yLen = y.TryGuessLength();
			if (xLen.IsSome && yLen.IsSome) {
				if (xLen.Value < yLen.Value) {
					return -1;
				}
				if (xLen.Value > yLen.Value) {
					return 1;
				}
			}

			using (var yIter = y.GetEnumerator()) {
				int nResult = 0;
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

		/// <summary>
		///   Inverts the specified comparison handler; changes the order from ascending to descending and vice versa.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="comparer"> </param>
		/// <returns> </returns>
		public static IComparer<T> Invert<T>(this IComparer<T> comparer)
		{
			return Comparer<T>.Create((a, b) =>
			                             {
				                             var result = comparer.Compare(a, b);
				                             if (result < 0) return 1;
				                             if (result > 0) return -1;
				                             return 0;
			                             });
		}
	}
}