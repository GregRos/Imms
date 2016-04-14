using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Imms.Abstract {


	public partial class AbstractSet<TElem, TSet> : ISet<TElem> {

		/// <summary>
		/// Adds an element to the set. Identical to <see cref="Add"/>.
		/// </summary>
		/// <param name="set">The instance to which to add.</param>
		/// <param name="item">The element to add.</param>
		/// <returns></returns>
		public static TSet operator +(AbstractSet<TElem, TSet> set, TElem item) {
			return set.Add(item);
		}

		/// <summary>
		/// Removes an element from the set. Identical to <see cref="Remove"/>.
		/// </summary>
		/// <param name="set">The set from which to remove.</param>
		/// <param name="item">The element to remove.</param>
		/// <returns></returns>
		public static TSet operator -(AbstractSet<TElem, TSet> set, TElem item) {
			return set.Remove(item);
		}

		/// <summary>
		/// Returns the intersection of a set and a sequence of elements. Identical to <see cref="Intersect"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to intersect with.</param>
		/// <returns></returns>
		public static TSet operator &(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Intersect(seq);
		}

		/// <summary>
		/// Returns the union of a set and a sequence of elements. Identical to <see cref="Union"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Union with.</param>
		/// <returns></returns>
		public static TSet operator +(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Union(seq);
		}

		/// <summary>
		/// Returns the elements of the first set minus those of the second one. Identical to <see cref="Except"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Except with.</param>
		/// <returns></returns>
		public static TSet operator -(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Except(seq);
		}

		/// <summary>
		/// Returns the symmetric difference. Identical to <see cref="Difference"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Union with.</param>
		/// <returns></returns>
		public static TSet operator ^(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Difference(seq);
		}

		/// <summary>
		/// Returns true if self is a proper superset of other.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator >(AbstractSet<TElem, TSet> self, IEnumerable<TElem> other) {
			return self.IsProperSupersetOf(other);
		}

		/// <summary>
		/// Returns true if self is a proper subset of other.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator <(AbstractSet<TElem, TSet> self, IEnumerable<TElem> other) {
			return self.IsProperSubsetOf(other);
		}

		/// <summary>
		/// Returns true if self is a superset of other.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator >=(AbstractSet<TElem, TSet> self, IEnumerable<TElem> other) {
			return self.IsSupersetOf(other);
		}

		/// <summary>
		/// Returns true if self is a subset of other.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator <=(AbstractSet<TElem, TSet> self, IEnumerable<TElem> other) {
			return self.IsSubsetOf(other);
		}

		bool ISet<TElem>.Add(TElem item) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.UnionWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.IntersectWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.ExceptWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		void ISet<TElem>.SymmetricExceptWith(IEnumerable<TElem> other) {
			throw Errors.Collection_readonly;
		}

		bool ISet<TElem>.Overlaps(IEnumerable<TElem> other) {
			return !IsDisjointWith(other);
		}
	}
}