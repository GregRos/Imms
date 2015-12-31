//Implementation Note: I do not use conditional compilation by itself due to performance reasons.
//Even though conditional compilation eliminates the actual call to a method, it does not eliminate
//the operations you perform to get the parameters of the method. These could potentially be expensive.
#if ASSERTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Imms.Abstract;
namespace Imms
{
	/// <summary>
	/// Indicates that an assertion has failed.
	/// </summary>
	internal class AssertionFailedException : Exception {
		static string FormatMessage(string assertionType, string message) {
			return string.Format("Failed assertion of type '{0}'. Assertion message: \r\n {1}",
				assertionType, message);
		}
		public AssertionFailedException(string name, string message = "") : base(FormatMessage(name, message)) {
			
		}
	}

	/// <summary>
	/// Contains assertion extension methods
	/// </summary>
	internal static class AssertEx
	{
		[Conditional("ASSERTS")]
		private static void Assert(bool condition, string kind, string message = "") {
			if (!condition) {
				Debugger.Break();
				throw new AssertionFailedException(kind, message);
			}
		}

		[Conditional("ASSERTS")]
		public static void AssertAreNotNull<T>(params T[] items)
		{
			items.AssertAreNotNull();
		}

		[Conditional("ASSERTS")]
		public static void AssertAreNotNull<T>(this IEnumerable<T> me)
		{
			me.AssertNotNull();

			foreach (var item in me)
			{
				item.AssertNotNull();
			}
		}

		[Conditional("ASSERTS")]
		public static void AssertBetween<T>(this T actual, T lower, T upper, string message = "")
		where T : IComparable<T> {
			Assert(actual.CompareTo(lower) >= 0, "actual >= lower", message);
			Assert(actual.CompareTo(upper) <= 0, "actual <= higher", message);
		}

		[Conditional("ASSERTS")]
		public static void AssertEqual(this IEnumerable actual, IEnumerable expected, string message = "") {
			Assert(actual.Cast<object>().SequenceEqual(expected.Cast<object>()), "actual.SequenceEqual(expected)", message);
		}

		/// <summary>
		///   Assert.AreEqual, if T is IEnumerable then CollectionAssert.AreEqual
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertEqual<T>(this T actual, T expected, string message = "")
		{
			if (typeof (T).IsAssignableFrom(typeof (IEnumerable))) {
				var seq1 = (IEnumerable) actual;
				var seq2 = (IEnumerable) expected;
				seq1.AssertEqual(seq2, message);
			}
			Assert(actual.Equals(expected), "actual.Equals(expected)", message);
		}

		/// <summary>
		///   Assert.IsTrue(predicate(value))
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertEqual<T>(this T value, Func<T, bool> predicate, string message = "")
		{
			Assert(predicate(value), "predicate(value)", message);
		}

		/// <summary>
		///   Is(false)
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertFalse(this bool value, string message = "")
		{
			Assert(!value, "!value", message);
		}

		/// <summary>
		///   Assert.AreNotEqual, if T is IEnumerable then CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertUnequal<T>(this T actual, T notExpected, string message = "")
		{
			if (typeof (T) != typeof (string) && typeof (IEnumerable).IsAssignableFrom(typeof (T))) {
				((IEnumerable) actual).AssertUnequal((IEnumerable) notExpected, message);
			}
			Assert(!actual.Equals(notExpected), "actual != notExpected", message);
		}

		/// <summary>
		///   CollectionAssert.AreNotEqual
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertUnequal(this IEnumerable actual, IEnumerable notExpected, string message = "")
		{
			Assert(!actual.Cast<object>().SequenceEqual(notExpected.Cast<object>()), "!actual.SequenceEqual(expected)", message);
		}

		/// <summary>
		///   Assert.IsNotNull
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertNotNull<T>(this T value, string message = "")
		{
			Assert(!ReferenceEquals(value, null), "value != null", message);
		}

		/// <summary>
		///   Is(true)
		/// </summary>
		[Conditional("ASSERTS")]
		public static void AssertTrue(this bool value, string message = "")
		{
			value.AssertEqual(true, message);
		}
	}
}
#endif