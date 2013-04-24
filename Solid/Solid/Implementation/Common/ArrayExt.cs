using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Solid.Common
{
	internal static class Helpers
	{
		public new static bool ReferenceEquals(this object self, object other)
		{
			return Object.ReferenceEquals(self, other);
		}

		

		private static PropertyInfo GetLengthProperty(this Type type)
		{
			var bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.IgnoreCase;
			var members = type.FindMembers(MemberTypes.Property, bindings, (m, x) => m.Name == "Length" || m.Name == "Count", null);
			return members.FirstOrDefault() as PropertyInfo;
		}

		public static bool IsNull(this object o)
		{
			return Object.ReferenceEquals(o, null);
		}

		/// <summary>
		///   Converts a sequence to an array quickly. The array may be longer than the sequence. The final elements will be uninitialized.
		///   The length of the sequence is returned in the  output parameter.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <param name="length"> The length of the sequence. </param>
		/// <returns> </returns>
		internal static T[] ToArrayFast<T>(this IEnumerable<T> items, out int length)
		{
			if (items == null) throw Errors.Argument_null("items");
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

		private static TOut[] UsingLengthHint<TOut>(this IEnumerable<TOut> o, ref int length, double multiplier = 2)

		{
			var arr = new TOut[length];
			using (var iterator = o.GetEnumerator())
			{
				var i = 0;
				for (i = 0; iterator.MoveNext(); i++)
				{
					if (i >= arr.Length)
					{
						var newArr = new TOut[(int)(arr.Length * multiplier)];
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

	internal class EnumerableProxy<T> : IEnumerable<T>
	{
		private readonly Func<IEnumerator<T>> _getEnumerator;

		public EnumerableProxy(Func<IEnumerator<T>> getEnumerator)
		{
			_getEnumerator = getEnumerator;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _getEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal static class ArrayExt
	{
		public static T[] Add<T>(this T[] self, T value)
		{
			var myCopy = new T[self.Length + 1];
			self.CopyTo(myCopy, 0);
			myCopy[myCopy.Length - 1] = value;
			return myCopy;
		}

		public static T[] Insert<T>(this T[] self, int index, T value)
		{
			var myCopy = new T[self.Length + 1];
			Array.Copy(self, 0, myCopy, 0, index);
			myCopy[index] = value;
			Array.Copy(self, index, myCopy, index + 1, self.Length - index);
			return myCopy;
		}

		public static T[] Remove<T>(this T[] self)
		{
			var myCopy = new T[self.Length - 1];
			for (var i = 0; i < self.Length - 1; i++)
			{
				myCopy[i] = self[i];
			}
			return myCopy;
		}

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

		public static T[] Set<T>(this T[] self, int index, T value)
		{
			var myCopy = new T[self.Length];
			self.CopyTo(myCopy, 0);
			myCopy[index] = value;
			return myCopy;
		}

		public static T[] TakeFirst<T>(this T[] self, int count)
		{
			var myCopy = new T[count];
			Array.Copy(self, 0, myCopy, 0, count);
			return myCopy;
		}
	}
}