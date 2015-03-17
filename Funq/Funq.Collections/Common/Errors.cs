using System;
using System.Collections.Generic;

namespace Funq.Collections.Common
{
	/// <summary>
	/// Errors used by the implementation of some data structures. These shouldn't be user-visible and if they are, this constitutes a bug.
	/// </summary>
	internal static class ImplErrors
	{
		/// <summary>
		/// Thrown when a digit gets an invalid size.
		/// </summary>
		internal static InvalidOperationException Invalid_digit_size
		{
			get
			{
				return new InvalidOperationException("This operation cannot be performed on a digit with this size.");
			}
		}

		/// <summary>
		/// Thrown when a switch statement that was supposed to handle all cases somehow didn't.
		/// </summary>
		internal static InvalidOperationException Invalid_execution_path
		{
			get
			{
				return new InvalidOperationException("A switch statement took an invalid execution path.");
			}
		}
		/// <summary>
		/// Thrown when a method was executed on a node marked as Null, which is illegal for executions.
		/// </summary>
		internal static InvalidOperationException Invalid_null_invocation
		{
			get
			{
				return new InvalidOperationException("This operation cannot be executed on a Null node.");
			}
		}
	}
}