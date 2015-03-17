using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class Errors
	{
		public class NoValueException : InvalidOperationException
		{
			public NoValueException()
				: base("The Optional object had no value.")
			{
			}
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

		public static NoValueException NoValue
		{
			get
			{
				return new NoValueException();
			}
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