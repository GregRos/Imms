
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Funq.Collections.Implementation;

namespace Funq.Collections.Common
{
	/// <summary>
	/// A special lock-type object used to control whether mutation is possible or not. <br/>
	/// Used to guarantee persistence, while safely allowing some safe mutation to greatly reduce object construction overhead.
	/// </summary>
	internal sealed class Lineage
	{
		/// <summary>
		/// An instance of the Lineage which always denies mutation.
		/// </summary>
		public static readonly Lineage Immutable = new Lineage(true);
		public readonly bool neverMutate;
		private Lineage()
		{
		
		}
		
		private Lineage(bool never)
		{
			neverMutate = never;
		}

		/// <summary>
		/// Creates a new Lineage that allows controlled mutation for an operation with the right key. 
		/// </summary>
		/// <returns></returns>
		public static Lineage Mutable()
		{
#if NO_MUTATION
			return Lineage.Immutable;
#endif
			return new Lineage();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AllowMutation(Lineage other)
		{
#if NO_MUTATION
			return false;
#endif
			return !neverMutate && this == other;
		}
	}
}