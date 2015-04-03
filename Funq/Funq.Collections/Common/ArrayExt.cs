using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

#pragma warning disable 219 //an unused variable that I want to keep...

namespace Funq.Collections.Common {
	internal static class Helpers {
		/// <summary>
		/// Returns the last element of the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T LastItem<T>(this List<T> list) {
			return list[list.Count - 1];
		}

		/// <summary>
		/// Removes the last element from the list and returns it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T PopLast<T>(this List<T> list) {
			var last = list.LastItem();
			list.RemoveAt(list.Count - 1);
			return last;
		}

		/// <summary>
		/// Tries to get a length or count properly by guesswork.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static PropertyInfo GetLengthProperty(this Type type) {
			var bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.IgnoreCase;
			var members = type.FindMembers(MemberTypes.Property, bindings, (m, x) => m.Name == "Length" || m.Name == "Length",
				null);
			return members.FirstOrDefault() as PropertyInfo;
		}


		internal static T[] ToArrayFastExact<T>(this IEnumerable<T> items) {
			var len = 0;
			var fast = items.ToArrayFast(out len);
			Array.Resize(ref fast, len);
			return fast;
		}
	}

	/// <summary>
		/// Provides some helpful operations on arrays.
		/// </summary>
		internal static class ArrayExt {
			/// <summary>
			/// Returns a new copy of the array with the specified element added to the end.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static T[] Add<T>(this T[] self, T value) {
				var myCopy = new T[self.Length + 1];
				self.CopyTo(myCopy, 0);
				myCopy[myCopy.Length - 1] = value;
				return myCopy;
			}

			/// <summary>
			/// Creates a new array with the specified number of elements.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="count"></param>
			/// <returns></returns>
			public static T[] Create<T>(int count) {
				return new T[count];
			}

			/// <summary>
			/// Returns a copy of the array with an element inserted at an index.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static T[] Insert<T>(this T[] self, int index, T value) {
				var myCopy = new T[self.Length + 1];
				Array.Copy(self, 0, myCopy, 0, index);
				myCopy[index] = value;
				Array.Copy(self, index, myCopy, index + 1, self.Length - index);
				return myCopy;
			}

			/// <summary>
			/// Returns a new array consisting of one element.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="item"></param>
			/// <param name="size"></param>
			/// <returns></returns>
			public static T[] OfItem<T>(T item, int size = 1) {
				var arr = new T[size];
				arr[0] = item;
				return arr;
			}

			/// <summary>
			/// Returns a new array with a given size, and sets the first element to be the specified object.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="size"></param>
			/// <param name="item1"></param>
			/// <returns></returns>
			public static T[] OfItems<T>(int size, T item1) {
				var arr = new T[size];
				arr[0] = item1;
				return arr;
			}

			/// <summary>
			/// Returns a new array with a given size, and sets the first two elements.
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
			/// Returns a new array of a given size, and sets the first three elements.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="size"></param>
			/// <param name="item1"></param>
			/// <param name="item2"></param>
			/// <param name="item3"></param>
			/// <returns></returns>
			public static T[] OfItems<T>(int size, T item1, T item2, T item3) {
				var arr = new T[size];
				arr[0] = item1;
				arr[1] = item2;
				arr[2] = item3;
				return arr;
			}

			/// <summary>
			/// Returns a new array consisting of the specified items.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="arr"></param>
			/// <returns></returns>
			public static T[] OfItems<T>(params T[] arr) {
				return arr;
			}

			/// <summary>
			/// Returns a new, shorted array without the last element.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <returns></returns>
			public static T[] Remove<T>(this T[] self) {
				var myCopy = new T[self.Length - 1];
				for (var i = 0; i < self.Length - 1; i++) {
					myCopy[i] = self[i];
				}
				return myCopy;
			}

			/// <summary>
			/// Returns a new array with the specified element removed.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <param name="index"></param>
			/// <returns></returns>
			public static T[] RemoveAt<T>(this T[] self, int index) {
				var myCopy = new T[self.Length];
				var i = 0;
				if (index >= self.Length) throw new Exception();
				for (; i < index; i++) {
					myCopy[i] = self[i];
				}
				i++;
				for (; i < self.Length; i++) {
					myCopy[i] = self[i];
				}
				return myCopy;
			}

			/// <summary>
			/// Returns a new array consisting of the first elements from the current array. Any excess elements are uninitialized.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <param name="count"></param>
			/// <returns></returns>
			public static T[] Take<T>(this T[] self, int count) {
				var newArr = new T[count];
				var len = self.Length < count ? self.Length : count;
				Array.Copy(self, 0, newArr, 0, len);
				return newArr;
			}

			/// <summary>
			/// Returns a new element with the specified element set to the specified value, and also truncates the result to the specified length.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <param name="truncate"></param>
			/// <returns></returns>
			public static T[] Update<T>(this T[] self, int index, T value, int truncate) {
#if ASSERTS
			truncate.Is(i => i > index);
#endif
				var myCopy = new T[truncate];
				var len = truncate > self.Length ? self.Length : truncate;
				Array.Copy(self, 0, myCopy, 0, len);
				myCopy[index] = value;
				return myCopy;
			}

			/// <summary>
			/// Returns the last element of the array.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="self"></param>
			/// <returns></returns>
			public static T Last<T>(this T[] self) {
				return self[self.Length - 1];
			}
		}
	}
