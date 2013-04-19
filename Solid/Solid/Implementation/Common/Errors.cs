using System;
using System.Collections.Generic;

namespace Solid.Common
{
	internal static class Errors
	{
		

		internal static InvalidOperationException Capacity_exceeded
		{
			get
			{
				return new InvalidOperationException("The collection has exceeded its maximum capacity.");
			}
		}

		internal static InvalidOperationException Collection_readonly
		{
			get
			{
				return new InvalidOperationException("This instance is readonly.");
			}
		}

		internal static InvalidOperationException Invalid_digit_size
		{
			get
			{
				return new InvalidOperationException("This operation cannot be performed on a digit with this size.");
			}
		}

		internal static InvalidOperationException Invalid_execution_path
		{
			get
			{
				return new InvalidOperationException("A switch statement took an invalid execution path.");
			}
		}

		internal static InvalidOperationException Invalid_leaf_invocation
		{
			get
			{
				return new InvalidOperationException("This operation cannot be performed on a leaf.");
			}
		}

		internal static InvalidOperationException Is_empty
		{
			get
			{
				return new InvalidOperationException("The operation is invalid because the data structure is empty.");
			}
		}

		internal static ArgumentException Key_exists
		{
			get
			{
				return new ArgumentException("The specified key already exists.");
			}
		}

		internal static KeyNotFoundException Key_not_found
		{
			get
			{
				return new KeyNotFoundException("The specified key does not exist in this data structure.");
			}
		}

		internal static InvalidOperationException Too_many_hash_collisions
		{
			get
			{
				return new InvalidOperationException(
					"An abnormally large number of hash collisions has occurred. This may be a sign that equality members must be reimplemented.");
			}
		}

		internal static InvalidOperationException Wrong_thread
		{
			get
			{
				return new InvalidOperationException("This instance can only be used from the thread that constructed it.");
			}
		}

		internal static ArgumentOutOfRangeException Arg_out_of_range(string name)
		{
			return new ArgumentOutOfRangeException(name, "The index is out of range of this data structure");
		}

		internal static ArgumentNullException Argument_null(string name)
		{
			return new ArgumentNullException(name, "The argument cannot be null.");
		}

		internal static ArgumentOutOfRangeException Not_found(string name)
		{
			return new ArgumentOutOfRangeException(name, "The specified item was not found in the collection.");
		}
	}
}