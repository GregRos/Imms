using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Imms.Abstract;

namespace Imms {
	/// <summary>
	///     Extensions for IEnumerable and collection classes.
	/// </summary>
	static class CollectionExt {

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> seq, IEqualityComparer<T> eq ) {
			eq = eq ?? FastEquality<T>.Default;
			return new HashSet<T>(seq, eq);
		}

		public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> seq, IComparer<T> comparer) {
			comparer = comparer ?? FastComparer<T>.Default;
			return new SortedSet<T>(seq, comparer);
		} 

		/// <summary>
		///     Tries the guess the length of the sequence by checking if it's a known collection type, WITHOUT iterating over it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items">The items.</param>
		/// <returns>The length of the sequence, or None if the guess didn't work.</returns>
		internal static Optional<int> TryGuessLength<T>(this IEnumerable<T> items) {
			if (items == null) throw Errors.Argument_null("items");

			var iCollection = items as ICollection<T>;
			if (iCollection != null) {
				return iCollection.Count;
			}
			var icollectionLegacy = items as ICollection;
			if (icollectionLegacy != null) {
				return icollectionLegacy.Count;
			}
			var ianyIterable = items as IAnyIterable<T>;
			if (ianyIterable != null) {
				return ianyIterable.Length;
			}
			return Optional.None;
		}

		/// <summary>
		///     Converts a sequence to an array efficiently. The array may be longer than the sequence. The final elements will be
		///     uninitialized.<br />
		///     The length of the sequence is returned in the  output parameter. <br />
		///     It is extremely unsafe to modify the array!!!!
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <param name="length"> The length of the sequence, computed while converting it to an array. </param>
		/// <returns> </returns>
		internal static T[] ToArrayFast<T>(this IEnumerable<T> items, out int length) {
			//There are several main reasons why this method is faster than other ways of converting sequences to arrays.
			//1. It intelligently calls the right method if the sequence is really a concrete collection.
			//2. It cheats using type checking and reflection.
			//3. If all else fails and the input isn't a known collection, it uses the classical conversion algorithm,
			//		but doesn't need to resize the array to the size of the sequence. This saves a lot of time, because
			//		this last resize operation is the longest of all them all.
			// That said, it can be slow for small collections.
			if (items == null) throw Errors.Argument_null("items");


			T[] arr;
			var array = items as T[];
			if (array != null) {
				arr = array;
				length = array.Length;
				goto exit;
			}
			
			var iCollection = items as ICollection<T>;
			if (iCollection != null) {
				
				arr = new T[iCollection.Count];
				iCollection.CopyTo(arr, 0);
				length = arr.Length;
				goto exit;
			}
			var iCollectionLegacy = items as ICollection;
			if (iCollectionLegacy != null) {
				arr = new T[iCollectionLegacy.Count];
				iCollectionLegacy.CopyTo(arr, 0);
				length = arr.Length;
				goto exit;
			}
			length = 4;
			arr = items.ToArrayWithLengthHint(ref length);
			exit:

			return arr;
		}

		/// <summary>
		///     Constructs an array from a sequence using a length hint.. <br />
		///     The length hint parameter also returns the number of items in the sequence. The array is returned not truncated
		///     (e.g. it can have uninitialized elements in the end).
		/// </summary>
		/// <param name="o"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private static T[] ToArrayWithLengthHint<T>(this IEnumerable<T> o, ref int length) {
			var arr = new T[length];
			using (var iterator = o.GetEnumerator()) {
				int i;
				for (i = 0; iterator.MoveNext(); i++) {
					if (i >= arr.Length) {
						var newArr = new T[arr.Length << 1];
						arr.CopyTo(newArr, 0);
						arr = newArr;
					}
					arr[i] = iterator.Current;
				}

				length = i;
				return arr;
			}
		}

		internal static void ForEach<TElem>(this IEnumerable<TElem> seq, Action<TElem> act) {
			IAnyIterable<TElem> elems = seq as IAnyIterable<TElem>;
			if (elems != null) {
				elems.ForEachWhile(item => {
					act(item);
					return true;
				});
			} else if (seq is TElem[]) {
				var arr = (TElem[]) seq;
				for (var i = 0; i < arr.Length; i++) act(arr[i]);
			} else {
				var list = seq as List<TElem>;
				if (list != null) list.ForEach(act);
				else {
					ForEachWhile(seq, x => {
						act(x);
						return true;
					});
				}
			}
		}

		internal static IEnumerable<TOut> Cross<T, TOther, TOut>(this IEnumerable<T> self, IEnumerable<TOther> other,
			Func<T, TOther, TOut> selector) {
			var arrOther = other.ToArray();
			return from x in self from y in arrOther select selector(x, y);
		}

		internal static bool HasEfficientForEach<T>(this IEnumerable<T> seq) {
			return seq is IAnyIterable<T> || seq is T[] || seq is List<T>;
		}

		internal static bool ForEachWhile<TElem>(this IEnumerable<TElem> seq, Func<TElem, bool> act) {
			var elems = seq as IAnyIterable<TElem>;
			if (elems != null) {
				return elems.ForEachWhile(act);
			}
			var arr = seq as TElem[];
			if (arr != null) {
				for (var i = 0; i < arr.Length; i++) {
					if (!act(arr[i])) {
						return false;
					}
				}
			}
			var list = seq as List<TElem>;
			if (list != null) {
				for (var i = 0; i < list.Count; i++) {
					if (!act(list[i])) {
						return false;
					}
				}
			}
			return seq.All(act);
		}

		class LambdaEnumerable<T> : IEnumerable<T> {

			readonly Func<IEnumerator<T>> _getIterator;

			public LambdaEnumerable(Func<IEnumerator<T>> getIterator) {
				_getIterator = getIterator;
			}

			public IEnumerator<T> GetEnumerator() {
				return _getIterator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
	}
}