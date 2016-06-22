using System;
using System.Collections.Generic;

#pragma warning disable 219 //an unused variable that I want to keep...

namespace Imms.Implementation {
	static class Helpers {
		/// <summary>
		///     Returns the last element of the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T LastItem<T>(this List<T> list) {
			return list[list.Count - 1];
		}

		/// <summary>
		/// Removes the duplicates in sorted array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arr">The array.</param>
		/// <param name="equalityFunc">The equality function.</param>
		/// <param name="len">The length.</param>
		public static void RemoveDuplicatesInSortedArray<T>(this T[] arr, Func<T, T, bool> equalityFunc, ref int len) {
			len.CheckIsBetween("len", 0, arr.Length);
			if (len <= 1) return;

			var lastValue = arr[0];
			var erased = 0;
			var i = 1;
			var writeIndex = 1;
			while (i < len) {
				if (!equalityFunc(arr[i], lastValue)) {
					lastValue = arr[i];
					if (writeIndex != i) {
						arr[writeIndex] = arr[i];
					}
					writeIndex++;
				}
				i++;
			}
			len = writeIndex;
		}

		/// <summary>
		///     Removes the last element from the list and returns it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T PopLast<T>(this List<T> list) {
			var last = list.LastItem();
			list.RemoveAt(list.Count - 1);
			return last;
		}
	}

	/// <summary>
	///     Provides some helpful operations on arrays.
	/// </summary>
	static class ArrayExt {

		/// <summary>
		///     Returns a new array with a given size, and sets the first two elements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="size"></param>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <returns></returns>
		public static T[] OfItems<T>(int size, T item1, T item2) {
			var arr = new T[size];
			arr[0] = item1;
			arr[1] = item2;
			return arr;
		}

		/// <summary>
		///     Returns a new array consisting of the first elements from the current array. Any excess elements are uninitialized.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T[] Resize<T>(this T[] self, int count) {
			var newArr = new T[count];
			var len = self.Length < count ? self.Length : count;
			Array.Copy(self, 0, newArr, 0, len);
			return newArr;
		}

		/// <summary>
		///     Returns a new element with the specified element set to the specified value, and also truncates the result to the
		///     specified length.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <param name="truncate"></param>
		/// <returns></returns>
		public static T[] Update<T>(this T[] self, int index, T value, int truncate) {
#if ASSERTS
			truncate.AssertEqual(i => i > index);
#endif
			var myCopy = new T[truncate];
			var len = truncate > self.Length ? self.Length : truncate;
			Array.Copy(self, 0, myCopy, 0, len);
			myCopy[index] = value;
			return myCopy;
		}
	}
}