using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Funq
{
	/// <summary>
	/// Indicates that an attempt was made to access the Value property of an Option with no value.
	/// </summary>
	public class NoValueException : InvalidOperationException
	{
		/// <summary>
		/// Creates a new instance of NoValueException for the option type 't'.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="message">An optional extra message.</param>
		public NoValueException(Type t, string message = "")
			: base(string.Format("The Optional<{0}> object had no value. {1}", t.PrettyName(), message))
		{
		}
	}
	
	internal static class Errors
	{

		public static void IsNotNull(this object o, string name) {
			if (o == null) throw Errors.Argument_null(name);
		}

		public static void IsInRange<T>(this T value, string name,T lower, T upper) where T : IComparable<T> {
			var inRange1 = value.CompareTo(lower) >= 0;
			var inRange2 = value.CompareTo(upper) <= 0;
			if (!inRange1 && !inRange2) {
				throw Errors.Arg_out_of_range(name);
			}
		}

		public static ArgumentException Eq_comparer_required(string argName)
		{
			return new ArgumentException("You must supply an equality comparer for this element to avoid inconsistencies.",
				argName);
		}



		public static ArgumentException Bad_argument(string argName, string message = "The argument value is invalid.") {
			return new ArgumentException(message, argName);
		}

		public static InvalidCastException Invalid_type_conversion {
			get {
				return new InvalidCastException("Invalid type.");
			}
		}

		public static InvalidOperationException Capacity_exceeded(string message = "The collection has exceeded its maximum capacity.")
		{
			return new InvalidOperationException(message);
		}

		public static InvalidOperationException Collection_readonly
		{
			get
			{
				return new InvalidOperationException("This instance is readonly.");
			}
		}

		public static ObjectDisposedException Disposed(string name = null) {
			return new ObjectDisposedException(name, "The object has been disposed.");
		}

		public static InvalidOperationException Maps_not_disjoint(object key)
		{
			return new InvalidOperationException(string.Format("The specified maps share some keys in common, such as: '{0}'.", key));
		}

		public static ArgumentException Key_exists
		{
			get
			{
				return new ArgumentException("The specified key already exists.");
			}
		}

		public static NotSupportedException Reset_not_supported
		{
			get
			{
				return new NotSupportedException("This instance does not support reset.");
			}
		}

		public static InvalidOperationException Single_instance
		{
			get
			{
				return new InvalidOperationException("A single instance of this object may exist at a time.");
			}
		}

		public static InvalidOperationException Too_many_hash_collisions
		{
			get
			{
				return new InvalidOperationException(
					"An abnormally large number of hash collisions has occurred. This may be a sign that equality members must be reimplemented.");
			}
		}

		public static InvalidOperationException Wrong_thread
		{
			get
			{
				return new InvalidOperationException("This instance can only be used from the thread that constructed it.");
			}
		}

		public static ArgumentNullException Argument_null(string name)
		{
			return new ArgumentNullException(name, "The argument cannot be null.");
		}

		public static ArgumentOutOfRangeException Not_found(string name)
		{
			return new ArgumentOutOfRangeException(name, "The specified item was not found in the collection.");
		}

		public static InvalidOperationException Equality_contradiction(Type instance, Type other)
		{
			return new InvalidOperationException("Error! The equality handlers used by the instances contradict one another.");
		}

		public static InvalidOperationException Is_empty
		{
			get
			{
				return new InvalidOperationException("The underlying collection is empty.");
			}
		}

		public static InvalidOperationException Is_null
		{
			get
			{
				return new InvalidOperationException("The underlying collection is null.");
			}
		}

		public static KeyNotFoundException Key_not_found
		{
			get
			{
				return new KeyNotFoundException();
			}
		}

		public static NoValueException NoValue<T>()
		{
			return new NoValueException(typeof(T));
		}

		public static InvalidOperationException Not_enough_elements
		{
			get
			{
				return new InvalidOperationException("The collection didn't have enough elements.");
			}
		}


        public static InvalidOperationException Too_many_elements(int? expected = null)
        {
            var expectedStr = expected == null ? "" : " Expected at most: " + expected.ToString();
            return new InvalidOperationException("The collection has too many elements." + expectedStr);
        }

        

	    public static ArgumentException Invalid_arg_value(string name, string expected = "")
	    {
	        expected = expected == "" ? "" : " Expected: " + expected;
	        return new ArgumentException(name, "The argument has an invalid value." + expected);
	    }

		public static ArgumentOutOfRangeException Arg_out_of_range(string name, int index)
		{
			return new ArgumentOutOfRangeException(name, "The index is out of range of this data structure. It was: " + index);
		}

		public static ArgumentOutOfRangeException Arg_out_of_range(string name)
		{
			return new ArgumentOutOfRangeException(name);
		}

		public static InvalidOperationException Comparison_contradiction(Type instance, Type other)
		{
			return new InvalidOperationException("The comparison handlers used by the instances contradict one another.");
		}

		public static ObjectDisposedException Is_disposed(string str)
		{
			return new ObjectDisposedException(str);
		}
	}
}