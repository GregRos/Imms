using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

#pragma warning disable 219 //an unused variable that I want to keep...
namespace Funq.Collections.Common
{
	internal static class Helpers
	{
		/// <summary>
		/// Returns the last element of the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T LastItem<T>(this List<T> list)
		{
			return list[list.Count - 1];
		}

		/// <summary>
		/// Removes the last element from the list and returns it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T PopLast<T>(this List<T> list)
		{
			var last = list.LastItem();
			list.RemoveAt(list.Count - 1);
			return last;
		}
		/// <summary>
		/// Tries to get a length or count properly by guesswork.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static PropertyInfo GetLengthProperty(this Type type)
		{
			var bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.IgnoreCase;
			var members = type.FindMembers(MemberTypes.Property, bindings, (m, x) => m.Name == "Length" || m.Name == "Length", null);
			return members.FirstOrDefault() as PropertyInfo;
		}


		public static bool IsNull(this object o)
		{
			return Object.ReferenceEquals(o, null);
		}

		/// <summary>
		///   Converts a sequence to an array efficiently. The array may be longer than the sequence. The final elements will be uninitialized.<br/>
		///   The length of the sequence is returned in the  output parameter.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <param name="length"> The length of the sequence. </param>
		/// <returns> </returns>
		internal static T[] ToArrayFast<T>(this IEnumerable<T> items, out int length)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
			const int
				IS_ARRAY = 1,
				IS_ICOLLECTION = 3,
				IS_ICOLLECTION_LEGACY = 4,
				TRY_DYNAMIC = 5,
				USE_DEFAULT = 6,
				OTHER = 7;

			var match =
				items is T[] ? IS_ARRAY
					: items is ICollection<T> ? IS_ICOLLECTION
						: items is ICollection ? IS_ICOLLECTION_LEGACY
							: TRY_DYNAMIC;

			T[] arr;

			switch (match)
			{
				case IS_ARRAY:
					arr = items as T[];
					length = arr.Length;
					return arr;
				case IS_ICOLLECTION:
					var col = items as ICollection<T>;
					arr = new T[col.Count];
					col.CopyTo(arr, 0);
					length = arr.Length;
					return arr;
				case IS_ICOLLECTION_LEGACY:
					var legacy = items as ICollection;
					arr = new T[legacy.Count];
					legacy.CopyTo(arr, 0);
					length = arr.Length;
					return arr;
				case TRY_DYNAMIC:
					var type = items.GetType();
					var hintProperty = type.GetLengthProperty();
					if (hintProperty == null) goto case USE_DEFAULT;
					length = hintProperty.GetValue(items, null) as int? ?? 4;
					arr = items.UsingLengthHint(ref length, 2);
					return arr;
				case USE_DEFAULT:
					length = 4;
					arr = items.UsingLengthHint(ref length, 2);
					return arr;
				default:
					arr = items.ToArray();
					length = arr.Length;
					return arr;
			}
		}

		internal static T[] ToArrayFastExact<T>(this IEnumerable<T> items)
		{
			var len = 0;
			var fast = items.ToArrayFast(out len);
			Array.Resize(ref fast, len);
			return fast;
		}
		/// <summary>
		/// Constructs an array from a sequence using a length hint, and a scaling multiplier for iteratively increasing array size. <br/>
		/// The length hint parameter also returns the number of items in the sequence. The array is returned not truncated (e.g. it can have uninitialized elements in the end).
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="o"></param>
		/// <param name="length"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static TOut[] UsingLengthHint<TOut>(this IEnumerable<TOut> o, ref int length, double multiplier = 2)

		{
			var arr = new TOut[length];
			using (var iterator = o.GetEnumerator())
			{
				var i = 0;
				for (i = 0; iterator.MoveNext(); i++)
				{
					if (i >= arr.Length)
					{
						var newArr = new TOut[(int) (arr.Length * multiplier)];
						arr.CopyTo(newArr, 0);
						arr = newArr;
					}
					arr[i] = iterator.Current;
				}
#if DEBUG
			arr.Take(o.Count()).SequenceEqual(o).Is(true);
#endif
				length = i;
				return arr;
			}
		}
	}

	/// <summary>
	/// Provides some helpful operations on arrays.
	/// </summary>
	internal static class ArrayExt
	{
		/// <summary>
		/// Returns a new copy of the array with the specified element added to the end.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T[] Add<T>(this T[] self, T value)
		{
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
		public static T[] Create<T>(int count)
		{
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
		public static T[] Insert<T>(this T[] self, int index, T value)
		{
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
		public static T[] OfItem<T>(T item, int size = 1)
		{
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
		public static T[] OfItems<T>(int size, T item1)
		{
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
		public static T[] OfItems<T>(int size, T item1, T item2)
		{
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
		public static T[] OfItems<T>(int size, T item1, T item2, T item3)
		{
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
		public static T[] OfItems<T>(params T[] arr)
		{
			return arr;
		}
		/// <summary>
		/// Returns a new, shorted array without the last element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		public static T[] Drop<T>(this T[] self)
		{
			var myCopy = new T[self.Length - 1];
			for (var i = 0; i < self.Length - 1; i++)
			{
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
		public static T[] RemoveAt<T>(this T[] self, int index)
		{
			var myCopy = new T[self.Length];
			var i = 0;
			if (index >= self.Length) throw new Exception();
			for (; i < index; i++)
			{
				myCopy[i] = self[i];
			}
			i++;
			for (; i < self.Length; i++)
			{
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
		public static T[] Take<T>(this T[] self, int count)
		{
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
		public static T[] Update<T>(this T[] self, int index, T value, int truncate)
		{
#if DEBUG
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
		public static T Last<T>(this T[] self)
		{
			return self[self.Length - 1];
		}
	}
}