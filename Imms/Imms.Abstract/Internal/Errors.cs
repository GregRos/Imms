using System;
using System.Collections.Generic;
using System.Diagnostics;
using Imms.Abstract;

namespace Imms {

	/// <summary>
	///     Indicates that an attempt was made to access the Value property of an Option with no value.
	/// </summary>
	public class NoValueException : InvalidOperationException {

		private static string GetMessage(Type t, string message) {
			string typeName = t == null ? "an unknown type" : "type " + t.PrettyName();
			return string.Format("Tried to get the underlying value of an optional value of {0}, but no value exists. {1}",
				typeName, message ?? "");
		}

		/// <summary>
		///     Creates a new instance of NoValueException for the option type 't'.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="message">An optional extra message.</param>
		internal NoValueException(Type t = null, string message = "")
			: base(GetMessage(t, message)) {}
	}

	static class Errors {

		public static InvalidOperationException Maps_not_disjoint(object key)
		{
			return new InvalidOperationException(string.Format("The specified maps share some keys in common, such as: '{0}'.", key));
		}
		public static InvalidOperationException Collection_readonly {
			get {
				var invalidOperationException = new InvalidOperationException("This instance is readonly.");
				return invalidOperationException;
			}
		}
		/// <summary>
		/// The key already exists in the dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static ArgumentException Key_exists(object key = null) {
			var expected = key == null ? "" : "It was: " + key;
			var keyExists = new ArgumentException("The specified key already exists in the dictionary. " + expected);
			return keyExists;
		}

		public static NotSupportedException Reset_not_supported {
			get { return new NotSupportedException("This instance does not support reset."); }
		}

		public static InvalidOperationException Is_empty {
			get { return new InvalidOperationException("The underlying collection is empty."); }
		}

		/// <summary>
		/// A key wasn't found in a dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static KeyNotFoundException Key_not_found(object key = null) {
			var message = key == null
				? "The key was not found in this dictionary."
				: string.Format("The key '{0}' was not found in this dictionary.", key);
			var keyNotFound = new KeyNotFoundException(message);
			return keyNotFound;
		}


		public static void CheckNotNull(this object o, string name) {
			if (o == null) throw Argument_null(name);
		}

		public static void CheckIsBetween(this int value, string name, int? lower = null, int? upper = null,
			string message = null) {
				if (lower.HasValue && value < lower.Value)
				{
					message = string.Format("Expected in range [{0}, {1}]. More info: {2}", lower.AsOptional().AsString().Or("*"),
						upper.AsOptional().AsString().Or("*"), message);
					throw Arg_out_of_range(name, value, message);
				}
				if (upper.HasValue && value > upper.Value)
				{
					message = string.Format("Expected in range [{0}, {1}]. More info: {2}", lower.AsOptional().AsString().Or("*"),
						upper.AsOptional().AsString().Or("*"), message);
					throw Arg_out_of_range(name, value, message);
				}
		}

		public static void CheckIsBetweenT<T>(this T value, string name, Optional<T> lower = default(Optional<T>),
			Optional<T> upper = default(Optional<T>), string message = null) where T : IComparable<T> {
			if (lower.IsSome && value.CompareTo(lower.Value) < 0) {
				message = string.Format("Expected in range [{0}, {1}]. More info: {2}", lower.AsString().Or("*"),
					upper.AsString().Or("*"), message);
				throw Arg_out_of_range(name, value, message);
			}
			if (upper.IsSome && value.CompareTo(upper.Value) > 0) {
				message = string.Format("Expected in range [{0}, {1}]. More info: {2}", lower.AsString().Or("*"),
					upper.AsString().Or("*"), message);
				throw Arg_out_of_range(name, value, message);
			}
		}

		public static ArgumentException Eq_comparer_required(string argName) {
			var eqComparerRequired = new ArgumentException("You must supply an equality comparer for this element to avoid inconsistencies.",
				argName);
			return eqComparerRequired;
		}

		public static InvalidOperationException Capacity_exceeded(
			string message = "The collection has exceeded its maximum capacity.") {
			return new InvalidOperationException(message);
		}

		public static ArgumentNullException Argument_null(string name) {
			return new ArgumentNullException(name, "The argument cannot be null.");
		}

		public static NoValueException NoValue<T>() {
			return new NoValueException(typeof (T));
		}

		public static NoValueException NoValue() {
			return new NoValueException();
		}

		public static InvalidOperationException Too_many_elements(int? expected = null) {
			var expectedStr = expected == null ? "" : " Expected at most: " + expected;
			return new InvalidOperationException("The collection has too many elements for this operation." + expectedStr);
		}

		public static ArgumentOutOfRangeException Arg_out_of_range(string name, object value, string message) {
			return new ArgumentOutOfRangeException(name, value, message);
		}

		public static ObjectDisposedException Is_disposed(string str) {
			return new ObjectDisposedException(str);
		}
	}
}