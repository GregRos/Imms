using System;

namespace Funq {
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

	/// <summary>
	/// Static class containing utility and extension methods for working with the SetRelation enumeration.
	/// </summary>
	public static class SetRelationExtensions {
		/// <summary>
		/// Determines whether the set relation indicates that the first set is a subset (proper or not) of the second set.
		/// </summary>
		/// <param name="relation">The set relation.</param>
		/// <returns></returns>
		public static bool IsSubsetOf(this SetRelation relation) {
			return relation.HasFlag(SetRelation.ProperSubsetOf) || relation.HasFlag(SetRelation.Equal);
		}

		/// <summary>
		/// Determines whether the set relation indicates that the first set is a superset (proper or not) of the second set.
		/// </summary>
		/// <param name="relation">The set relation.</param>
		/// <returns></returns>
		public static bool IsSupersetOf(this SetRelation relation) {
			return relation.HasFlag(SetRelation.ProperSupersetOf) || relation.HasFlag(SetRelation.Equal);
		}

		/// <summary>
		/// Determines whether the set relation indicates that the first set is a proper superset of the second set.
		/// </summary>
		/// <param name="relation">The relation.</param>
		/// <returns></returns>
		public static bool IsProperSupersetOf(this SetRelation relation) {
			return relation.HasFlag(SetRelation.ProperSupersetOf);
		}

		/// <summary>
		/// Determines whether the first set indicates that the second set is a proper subset of the second set.
		/// </summary>
		/// <param name="relation">The relation.</param>
		/// <returns></returns>
		public static bool IsProperSubsetOf(this SetRelation relation) {
			return relation.HasFlag(SetRelation.ProperSubsetOf);
		}

		/// <summary>
		/// Determines whether the set relation indicates that the two sets are disjoint.
		/// </summary>
		/// <param name="relation">The relation.</param>
		/// <returns></returns>
		public static bool IsDisjointWith(this SetRelation relation) {
			return relation.HasFlag(SetRelation.Disjoint);
		}

		/// <summary>
		/// Determines whether the set relation indicates that the two sets are equal.
		/// </summary>
		/// <param name="relation">The relation.</param>
		/// <returns></returns>
		public static bool IsEqual(this SetRelation relation) {
			return relation.HasFlag(SetRelation.Equal);
		}

		/// <summary>
		/// Determines whether the set relation indicates that the two sets satisfy none of the relations covered by the enumeration.
		/// </summary>
		/// <param name="relation">The relation.</param>
		/// <returns></returns>
		public static bool IsNone(this SetRelation relation) {
			return relation == SetRelation.None;
		}
	}
}