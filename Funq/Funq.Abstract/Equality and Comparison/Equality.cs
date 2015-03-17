using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	internal static class Equality
	{
		public static int Map_HashCode<TKey, TValue>(ITrait_MapLike<TKey, TValue> map, IEqualityComparer<TKey> kEquality = null, IEqualityComparer<TValue> vEquality = null)
		{
			var hash = 0;
			unchecked
			{
				map.ForEach(pair => { hash ^= (kEquality.GetHashCode(pair.Key) * 31) ^ vEquality.GetHashCode(pair.Value); });
			}
			return hash;
		}

		public static bool Map_Equate<TKey, TValue>(ITrait_MapLike<TKey, TValue> x, ITrait_MapLike<TKey, TValue> y, IEqualityComparer<TValue> vEquality = null)
		{
			vEquality = vEquality ?? EqualityComparer<TValue>.Default;
			var boiler = Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			if (x.Length != y.Length) return false;
			return x.ForEachWhile(xKvp =>
			                      {
				                      var yVal = y.TryGet(xKvp.Key);
				                      if (!yVal.EqualsWith(xKvp.Value, vEquality)) return false;
				                      return true;
			                      });
		}

		public static int List_HashCode<TElem>(ITrait_Iterable<TElem> obj, IEqualityComparer<TElem> equality = null)
		{
			const uint M = 0x5bd1e995;
			const int R = 24;
			const uint SEED = 0xc58f1a7b;
			equality = equality ?? EqualityComparer<TElem>.Default;
			var hash = (uint) (SEED ^ obj.Length);
			obj.ForEach(item =>
			            {
				            unchecked
				            {
					            var k = (uint) equality.GetHashCode(item);
					            k = ((k * M) >> R) * M;
					            hash = (hash * M) ^ k;
				            }
			            });
			unchecked
			{
				hash ^= ((hash >> 13) * M) >> 15;
				return (int) hash;
			}
		}

		public static bool Set_Equate<TElem>(ITrait_SetLike<TElem> x, ITrait_SetLike<TElem> y)
		{
			return x.RelatesTo(y) == SetRelation.Equal;
		}

		public static int Set_HashCode<TElem>(ITrait_SetLike<TElem> obj, IEqualityComparer<TElem> equality = null)
		{
			equality = equality ?? EqualityComparer<TElem>.Default;
			var hash = 0;
			obj.ForEach(x => { hash ^= equality.GetHashCode(x); });
			return hash;
		}

		public static bool List_Equate<TElem>(ITrait_Iterable<TElem> x, ITrait_Iterable<TElem> y, IEqualityComparer<TElem> equality = null)
		{
			equality = equality ?? EqualityComparer<TElem>.Default;
			var boiler = Boilerplate(x, y);
			if (boiler.IsSome) return boiler;
			if (x.Length != y.Length) return false;
			using (var yIter = y.GetEnumerator())
			{
				return x.ForEachWhile(v =>
				                      {
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
			keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
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

		internal static int ToInt(this Cmp r)
		{
			return r == Cmp.Equal ? 0 : r == Cmp.Greater ? 1 : -1;
		}

		internal static Cmp ToCmp(this int r)
		{
			return r == 0 ? Cmp.Equal : r > 0 ? Cmp.Greater : Cmp.Lesser;
		}


		public static Cmp List_CompareLex<TElem>(ITrait_Iterable<TElem> x, ITrait_Iterable<TElem> y, IComparer<TElem> comparer = null)
		{
			comparer = comparer ?? Comparer<TElem>.Default;
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
				return finalResult.ToCmp();
			}
		}

		public static Cmp List_CompareNum<TElem>(ITrait_Sequential<TElem> x, ITrait_Sequential<TElem> y, IComparer<TElem> comparer = null)
		{
			var xLen = x.Length;
			var yLen = y.Length;
			if (xLen > yLen) return Cmp.Greater;
			if (xLen < yLen) return Cmp.Lesser;
			return List_CompareLex(x, y, comparer);
		}



		/// <summary>
		///   Inverts the specified comparison handler; changes the order from ascending to descending and vice versa.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="comparer"> </param>
		/// <returns> </returns>
		public static IComparer<T> Invert<T>(this IComparer<T> comparer)
		{
			return new LambdaComparer<T>((a, b) =>
			                             {
				                             var result = comparer.Compare(a, b);
				                             if (result < 0) return 1;
				                             if (result > 0) return -1;
				                             return 0;
			                             });
		}
	}
}