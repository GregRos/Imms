using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Funq.Abstract;

namespace Funq
{

	[Flags]
	internal enum KnownType {
		Unknown = 0
		,
		Array = 1
		,
		ICollection = 2
		,
		ICollectionLegacy = 4
		,
		IReadOnlyCollection = 8
		,
		IAnySequential = 16
		,
		IAnySetLike = 32
		,
		IAnyMapLike = 64
		,
		IAnyIterable = 128
		,
		GenericList = 256
	}

	internal static class ReflectExt
	{
		public static bool DoesImplementMethod<T>(string name, params Type[] paramTypes) {
			var method = typeof (T).GetMethod(name, paramTypes);
			return method != null && method.DeclaringType == typeof(T) && !method.IsAbstract;
		}

		public static string PrettyName(this Type type)
		{
			if (type.GetGenericArguments().Length == 0)
			{
				return type.Name;
			}
			var genericArguments = type.GetGenericArguments();
			var unmangledName = type.JustTypeName();
			return unmangledName + "<" + String.Join(",", genericArguments.Select(PrettyName)) + ">";
		}

		public static string JustTypeName(this Type type)
		{
			var typeDefeninition = type.Name;
			var indexOf = typeDefeninition.IndexOf("`", StringComparison.InvariantCulture);
			return indexOf < 0 ? typeDefeninition : typeDefeninition.Substring(0, indexOf);
		}

		internal static KnownType CheckType<T>(object items)
		{

			if (items is T[])
			{
				return KnownType.Array;
			}
			if (items is List<T>)
			{
				return KnownType.GenericList;
			}
			if (items is ICollection<T>)
			{
				return KnownType.ICollection;
			}
			if (items is ICollection)
			{
				return KnownType.ICollection;
			}
			if (items is IAnySequential<T>)
			{
				return KnownType.IAnySequential;
			}
			if (items is IAnySetLike<T>)
			{
				return KnownType.IAnySetLike;
			}
			if (items is IAnyIterable<T>)
			{
				return KnownType.IAnyIterable;
			}
			if (items is IReadOnlyCollection<T>)
			{
				return KnownType.IReadOnlyCollection;
			}
			return KnownType.Unknown;
		}

		internal static KnownType CheckType<T>(this IEnumerable<T> o)
		{
			return CheckType<T>((IEnumerable)o);
		}

	}
	internal static class Fun
	{
		private class LambdaEnumerable<T> : IEnumerable<T> {

			private readonly Func<IEnumerator<T>> getIterator;

			public LambdaEnumerable(Func<IEnumerator<T>> getIterator) {
				this.getIterator = getIterator;
			}

			public IEnumerator<T> GetEnumerator() {
				return getIterator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}


		public static Option<TOut> AsSome<TOut>(this TOut x)
		{
			return Option.Some(x);
		}

		public static IEnumerable<T> CreateEnumerable<T>(Func<IEnumerator<T>> getIterator) {
			return new LambdaEnumerable<T>(getIterator);
		} 

		public static Func<T, TOut> AttachIndex<T, TOut>(this Func<T, int, TOut> f)
		{
			var i = 0;
			return (x => f(x, i++));
		}

		public static TOut Cast<TOut>(this object x)
		{
			return (TOut) x;
		}

		internal static Option<int> TryGuessLength<T>(this IEnumerable<T> items) {
			if (items == null) throw Errors.Argument_null("items");
			int len;
			switch (items.CheckType()) {
				case KnownType.Array:
				case KnownType.ICollectionLegacy:
					return ((ICollection) items).Count;
				case KnownType.GenericList:
				case KnownType.ICollection:
					return ((ICollection<T>) items).Count;
				case KnownType.IReadOnlyCollection:
				case KnownType.IAnyIterable:
				case KnownType.IAnyMapLike:
				case KnownType.IAnySetLike:
				case KnownType.IAnySequential:
					return ((IReadOnlyCollection<T>) items).Count;
				default:
					return Option.None;
			}
		}

		/// <summary>
		///   Converts a sequence to an array efficiently. The array may be longer than the sequence. The final elements will be uninitialized.<br/>
		///   The length of the sequence is returned in the  output parameter. <br/>
		///   It is extremely unsafe to modify the array!!!!
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="items"> The sequence. </param>
		/// <param name="length"> The length of the sequence. </param>
		/// <returns> </returns>
		internal static T[] ToArrayFast<T>(this IEnumerable<T> items, out int length)
		{
			if (items == null) throw Funq.Errors.Argument_null("items");
			
			T[] arr;

			switch (items.CheckType())
			{
				case KnownType.Array:
					arr = items as T[];
					length = arr.Length;
					break;
				case KnownType.GenericList:
					//WARNING: Don't do this at home!
					var asList = items as List<T>;
					var itemsField = typeof (List<T>).GetField("_items",
						BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if (itemsField == null) goto case KnownType.ICollection; //this can happen due to a version change
					var value = itemsField.GetValue(asList) as T[];
					if (value == null) goto case KnownType.ICollection; //I don't know why this would happen
					arr = value;
					length = asList.Count;
					break;
				case KnownType.ICollection:
					var col = items as ICollection<T>;
					arr = new T[col.Count];
					col.CopyTo(arr, 0);
					length = arr.Length;
					break;
				case KnownType.ICollectionLegacy:
					var legacy = items as ICollection;
					arr = new T[legacy.Count];
					legacy.CopyTo(arr, 0);
					length = arr.Length;
					break;
				case KnownType.IReadOnlyCollection:
					var roc = items as IReadOnlyCollection<T>;
					arr = new T[roc.Count];
					int i = 0;
					roc.ForEach(x => {
						arr[i] = x;
						i++;
					});
					length = roc.Count;
					break;
				case KnownType.IAnySetLike:
				case KnownType.IAnyIterable:
				case KnownType.IAnySequential:
					var lst = items as IAnyIterable<T>;
					arr = new T[lst.Length];
					lst.CopyTo(arr, 0, arr.Length);
					length = lst.Length;
					break;
				case KnownType.Unknown:
					length = 4;
					arr = items.UsingLengthHint(ref length, 2);
					break;
				default:
					arr = items.ToArray();
					length = arr.Length;
					break;
			}
			return arr;
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
				Debug.Assert(arr.Take(o.Count()).SequenceEqual(o));
#endif
				length = i;
				return arr;
			}
		}
	

		internal static void ForEach<TElem>(this IEnumerable<TElem> seq, Action<TElem> act)
		{
			if (seq is IAnyIterable<TElem>)
			{
				((IAnyIterable<TElem>)seq).ForEach(act);
			}
			ForEachWhile(seq, x =>
			                  {
				                  act(x);
				                  return true;
			                  });
		}

		internal static bool HasEfficientForEach<T>(this IEnumerable<T> seq) {
			return seq is IAnyIterable<T>;
		}

		internal static bool ForEachWhile<TElem>(this IEnumerable<TElem> seq, Func<TElem, bool> act)
		{
			if (seq is IAnyIterable<TElem>) {
				return ((IAnyIterable<TElem>) seq).ForEachWhile(act);
			}
			return seq.All(act);
		}

		public static TOut Pipe<TIn, TOut>(this TIn o, Func<TIn, TOut> f)
		{
			return f(o);
		}

		public static Option<TOut> TryCast<TOut>(this object x)
		{
			return (TOut) x;
		}

	}
}