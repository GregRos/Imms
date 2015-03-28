/*--------------------------------------------------------------------------
 * Chaining Assertion
 * ver 1.7.0.1 (Nov. 28th, 2012)
 *
 * created and maintained by neuecc <ils@neue.cc - @neuecc on Twitter>
 * licensed under Microsoft Public License(Ms-PL)
 * http://chainingassertion.codeplex.com/
 * 
 * (Some methods were added by the user)
 *--------------------------------------------------------------------------*/

/* -- Tutorial --
 * | at first, include this file on NUnit Project.
 * 
 * | three example, "Is" overloads.
 * 
 * // This same as Assert.AreEqual(25, Math.Pow(5, 2))
 * Math.Pow(5, 2).Is(25);
 * 
 * // This same as Assert.IsTrue("foobar".StartsWith("foo") && "foobar".EndWith("bar"))
 * "foobar".Is(s => s.StartsWith("foo") && s.EndsWith("bar"));
 * 
 * // This same as CollectionAssert.AreEqual(Enumerable.Range(1,5), new[]{1, 2, 3, 4, 5})
 * Enumerable.Range(1, 5).Is(1, 2, 3, 4, 5);
 * 
 * | CollectionAssert
 * | if you want to use CollectionAssert Methods then use Linq to Objects and Is
 * 
 * var array = new[] { 1, 3, 7, 8 };
 * array.Length().Is(4);
 * array.Contains(8).IsTrue(); // IsTrue() == Is(true)
 * array.All(i => i < 5).IsFalse(); // IsFalse() == Is(false)
 * array.Any().Is(true);
 * new int[] { }.Any().Is(false);   // IsEmpty
 * array.OrderBy(x => x).Is(array); // IsOrdered
 *
 * | Other Assertions
 * 
 * // Null Assertions
 * Object obj = null;
 * obj.IsNull();             // Assert.IsNull(obj)
 * new Object().IsNotNull(); // Assert.IsNotNull(obj)
 *
 * // Not Assertion
 * "foobar".IsNot("fooooooo"); // Assert.AreNotEqual
 * new[] { "a", "z", "x" }.IsNot("a", "x", "z"); /// CollectionAssert.AreNotEqual
 *
 * // ReferenceEqual Assertion
 * var tuple = Tuple.MutateOrCreate("foo");
 * tuple.IsSameReferenceAs(tuple); // Assert.AreSame
 * tuple.IsNotSameReferenceAs(Tuple.MutateOrCreate("foo")); // Assert.AreNotSame
 *
 * // Type Assertion
 * "foobar".IsInstanceOf<string>(); // Assert.IsInstanceOf
 * (999).IsNotInstanceOf<double>(); // Assert.IsNotInstanceOf
 * 
 * | Advanced Collection Assertion
 * 
 * var lower = new[] { "a", "b", "c" };
 * var upper = new[] { "A", "B", "C" };
 *
 * // EqualityComparer CollectionAssert, use IEqualityComparer<T> or Func<T,T,bool> delegate
 * #if ASSERTS 
 lower.Is(upper, StringComparer.InvariantCultureIgnoreCase); 
 #endif
 * #if ASSERTS 
 lower.Is(upper, (x, y) => x.ToUpper() == y.ToUpper()); 
 #endif
 *
 * // or you can use Linq to Objects - SequenceEqual
 * lower.SequenceEqual(upper, StringComparer.InvariantCultureIgnoreCase).Is(true);
 * 
 * | StructuralEqual
 * 
 * class MyClass
 * {
 *     public int IntProp { get; set; }
 *     public string StrField;
 * }
 * 
 * var mc1 = new MyClass() { IntProp = 10, StrField = "foo" };
 * var mc2 = new MyClass() { IntProp = 10, StrField = "foo" };
 * 
 * mc1.IsStructuralEqual(mc2); // deep recursive value equality compare
 * 
 * mc1.IntProp = 20;
 * mc1.IsNotStructuralEqual(mc2);
 * 
 * | DynamicAccessor
 * 
 * // AsDynamic convert to "dynamic" that can call private method/property/field/indexer.
 * 
 * // a class and private field/property/method.
 * public class PrivateMock
 * {
 *     private string privateField = "homu";
 * 
 *     private string PrivateProperty
 *     {
 *         get { return privateField + privateField; }
 *         set { privateField = value; }
 *     }
 * 
 *     private string PrivateMethod(int count)
 *     {
 *         return string.Join("", Enumerable.Repeat(privateField, count));
 *     }
 * }
 * 
 * // call private property.
 * var actual = new PrivateMock().AsDynamic().PrivateProperty;
 * Assert.AreEqual("homuhomu", actual);
 * 
 * // dynamic can't invoke extension methods.
 * // if you want to invoke "Is" then cast type.
 * (new PrivateMock().AsDynamic().PrivateMethod(3) as string).Is("homuhomuhomu");
 * 
 * // set value
 * var mock = new PrivateMock().AsDynamic();
 * mock.PrivateProperty = "mogumogu";
 * (mock.privateField as string).Is("mogumogu");
 * 
 * -- more details see project home --*/

#if ASSERTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NUnit.Framework;



namespace Funq.Collections
{

	#region Extensions

	internal static class AssertEx
	{
		/// <summary>
		///   EqualityComparison to IComparer Converter for CollectionAssert
		/// </summary>
		private class ComparisonComparer<T> : IComparer
		{
			private readonly Func<T, T, bool> comparison;

			public ComparisonComparer(Func<T, T, bool> comparison)
			{
				this.comparison = comparison;
			}

			public int Compare(object x, object y)
			{
				return (comparison != null)
					? comparison((T) x, (T) y) ? 0 : -1
					: Equals(x, y) ? 0 : -1;
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotNull<T>(params T[] items)
		{
			items.AreNotNull();
		}

		[Conditional("DEBUG")]
		public static void AreNotNull<T>(this IEnumerable<T> me)
		{
			me.IsNotNull();

			foreach (var item in me)
			{
				item.IsNotNull();
			}
		}


		/// <summary>
		///   Assert.AreEqual, if T is IEnumerable then CollectionAssert.AreEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void Is<T>(this T actual, T expected, string message = "")
		{
			Assert.AreEqual(expected, actual, message);
		}

		/// <summary>
		///   Assert.IsTrue(predicate(value))
		/// </summary>
		[Conditional("DEBUG")]
		public static void Is<T>(this T value, Func<T, bool> predicate, string message = "")
		{
			Assert.IsTrue(predicate(value));
		}

		/// <summary>
		///   CollectionAssert.AreEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void Is<T>(this IEnumerable<T> actual, params T[] expected)
		{
			Is(actual, expected.AsEnumerable());
		}

		/// <summary>
		///   CollectionAssert.AreEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = "")
		{
			CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), message);
		}

		/// <summary>
		///   CollectionAssert.AreEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer, string message = "")
		{
			Is(actual, expected, comparer.Equals, message);
		}

		/// <summary>
		///   CollectionAssert.AreEqual
		/// </summary>
		public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<T, T, bool> equalityComparison, string message = "")
		{
			CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
		}

		/// <summary>
		///   Is(false)
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsFalse(this bool value, string message = "")
		{
#if ASSERTS
			value.Is(false, message);
#endif
		}

		/// <summary>
		///   Assert.IsInstanceOf
		/// </summary>
		public static TExpected IsInstanceOf<TExpected>(this object value, string message = "")
		{
			Assert.IsInstanceOf<TExpected>(value, message);
			return (TExpected) value;
		}

		/// <summary>
		///   Assert.AreNotEqual, if T is IEnumerable then CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNot<T>(this T actual, T notExpected, string message = "")
		{
			if (typeof (T) != typeof (string) && typeof (IEnumerable).IsAssignableFrom(typeof (T)))
			{
				Fun.Cast<object>(((IEnumerable) actual)).IsNot(Fun.Cast<object>(((IEnumerable) notExpected)), message);
				return;
			}

			Assert.AreNotEqual(notExpected, actual, message);
		}

		/// <summary>
		///   CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNot<T>(this IEnumerable<T> actual, params T[] notExpected)
		{
			IsNot(actual, notExpected.AsEnumerable());
		}

		/// <summary>
		///   CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, string message = "")
		{
			CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), message);
		}

		/// <summary>
		///   CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, IEqualityComparer<T> comparer, string message = "")
		{
			IsNot(actual, notExpected, comparer.Equals, message);
		}

		/// <summary>
		///   CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, Func<T, T, bool> equalityComparison, string message = "")
		{
			CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
		}

		/// <summary>
		///   Assert.IsNotInstanceOf
		/// </summary>
		public static void IsNotInstanceOf<TWrong>(this object value, string message = "")
		{
			Assert.IsNotInstanceOf<TWrong>(value, message);
		}

		/// <summary>
		///   Assert.IsNotNull
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNotNull<T>(this T value, string message = "")
		{
			Assert.IsNotNull(value, message);
		}

		/// <summary>
		///   Assert.AreNotSame
		/// </summary>
		public static void IsNotSameReferenceAs<T>(this T actual, T notExpected, string message = "")
		{
			Assert.AreNotSame(notExpected, actual, message);
		}

		/// <summary>
		///   Assert.IsNull
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsNull<T>(this T value, string message = "")
		{
			Assert.IsNull(value, message);
		}

		/// <summary>
		///   Assert.AreSame
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsSameReferenceAs<T>(this T actual, T expected, string message = "")
		{
			Assert.AreSame(expected, actual, message);
		}

		/// <summary>
		///   Is(true)
		/// </summary>
		[Conditional("DEBUG")]
		public static void IsTrue(this bool value, string message = "")
		{
#if ASSERTS
			value.Is(true, message);
#endif
		}
	}

	#endregion
}
#endif