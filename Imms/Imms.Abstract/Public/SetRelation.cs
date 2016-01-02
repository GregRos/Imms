using System;

namespace Imms {
	/// <summary>
	///     Indicates the set-theoretic relation between two sets. 
	/// </summary>
	[Flags]
	public enum SetRelation {
		/// <summary>
		///     The two sets satisfy none of the relations covered by this enumeration.
		/// </summary>
		None = 0x0,
		/// <summary>
		///     The two sets are equal.
		/// </summary>
		Equal = 0x1,
		/// <summary>
		///     The first set is a proper subset of the second one.
		/// </summary>
		ProperSubsetOf = 0x2,
		/// <summary>
		///     The first set is a proper superset of the second one.
		/// </summary>
		ProperSupersetOf = 0x4,
		/// <summary>
		///     The two sets share no elements in common.
		/// </summary>
		Disjoint = 0x8
	}

	
}