using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;

namespace Funq.Abstract
{

	/// <summary>
	///   Parent of all unordered collections containing distinct values, which can efficiently determine membership.
	/// </summary>
	/// <typeparam name="TElem"> </typeparam>
	/// <typeparam name="TSet"> </typeparam>
	public abstract class AbstractSet<TElem, TSet>
		: AbstractIterable<TElem, TSet, SetBuilder<TElem>>, IAnySetLike<TElem>
		where TSet : AbstractSet<TElem, TSet> {
		private static readonly bool IsDifferenceImplemented;
		private static readonly bool IsExceptImplemented;
		private static readonly bool IsIntersectImplemented;
		private static readonly bool IsUnionImplemented;
		private static readonly bool IsSetRelationImplemented;

		static AbstractSet() {
			IsDifferenceImplemented = ReflectExt.DoesImplementMethod<TSet>("Difference", typeof(TSet));
			IsExceptImplemented = ReflectExt.DoesImplementMethod<TSet>("Except", typeof(TSet));
			IsIntersectImplemented = ReflectExt.DoesImplementMethod<TSet>("Intersect", typeof(TSet));
			IsUnionImplemented = ReflectExt.DoesImplementMethod<TSet>("Union", typeof(TSet));
			IsSetRelationImplemented = ReflectExt.DoesImplementMethod<TSet>("SetRelation", typeof (TSet));
		} 
		
		public abstract bool Contains(TElem item);

		/// <summary>
		/// Applies a symmetric difference/XOR between two sets. In symbols, A△B.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TSet Difference(TSet other) {
			var ex1 = this.Except(other);
			var ex2 = other.Except(this);
			return ex1.Union(ex2);
		}

		public TSet Difference(IEnumerable<TElem> other) {
			if (other is IAnySetLike<TElem>) {
				return Difference((IAnySetLike<TElem>) other);
			}
			TSet otherProvider = ToIterable(this, other);
			return Difference(otherProvider);
		}

		/// <summary>
		/// Applies a symmetric difference/XOR between a set, and a set-like collection. 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		private TSet Difference(IAnySetLike<TElem> other)
		{
			if (other is TSet) {
				return Difference((TSet) other);
			}
			var union = Union(other);
			var intersect = Intersect(other);
			return union.Except(intersect);
		}

		public virtual TSet Except(TSet other)
		{
			return Except(other as IAnySetLike<TElem>);
		}

		public TSet Except(IEnumerable<TElem> other)
		{
			if (IsExceptImplemented && other is TSet) {
				return Except((TSet) other);
			}
			using (var builder = EmptyBuilder)
			{
				ForEach(x => { if (!other.Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		public TSet Intersect(IEnumerable<TElem> other) {
			if (other is IAnySetLike<TElem>) {
				return Intersect((IAnySetLike<TElem>) other);
			}
			using (var builder = EmptyBuilder) {
				foreach (var item in other) {
					if (Contains(item)) {
						builder.Add(item);
					}
				}
				return ProviderFrom(builder);
			}
		}

		private TSet Intersect(IAnySetLike<TElem> other)
		{
			if (object.ReferenceEquals(this, other)) {
				return this;
			}
			if (other is TSet && IsIntersectImplemented) {
				return Intersect((TSet) other);
			}
			using (var builder = EmptyBuilder)
			{
				var shorterSet = Length > other.Length ? other : this;
				var longerSet = shorterSet == other ? this : other;
				shorterSet.ForEach(x => { if (longerSet.Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		public virtual TSet Intersect(TSet other)
		{
			return Intersect(other as IAnySetLike<TElem>);
		}

		public SetRelation RelatesTo(IEnumerable<TElem> other) {
			if (other is IAnySetLike<TElem>) {
				return RelatesTo((IAnySetLike<TElem>) other);
			}
			var total = 0;
			
			var intersectSize = other.Count(x => {
				total += 1;
				return Contains(x);
			});
			if (IsEmpty && total == 0) return SetRelation.Equal | SetRelation.Disjoint;
			if (IsEmpty && total != 0) return SetRelation.ProperSubsetOf | SetRelation.Disjoint;
			if (!IsEmpty && total == 0) return SetRelation.ProperSupersetOf | SetRelation.Disjoint;
			if (intersectSize == 0) return SetRelation.Disjoint;
			var otherContainsThis = intersectSize == this.Length;
			var thisContainsOther = intersectSize == total;
			if (thisContainsOther && otherContainsThis) return SetRelation.Equal;
			if (thisContainsOther) return SetRelation.ProperSupersetOf;
			if (otherContainsThis) return SetRelation.ProperSubsetOf;
			return SetRelation.None;
		}

		private SetRelation RelatesTo(IAnySetLike<TElem> other) {
			if (other is TSet && IsSetRelationImplemented) {
				return RelatesTo((TSet) other);
			}
			if (IsEmpty && other.IsEmpty) return SetRelation.Equal | SetRelation.Disjoint;
			if (IsEmpty && !other.IsEmpty) return SetRelation.ProperSubsetOf | SetRelation.Disjoint ;
			if (!IsEmpty && other.IsEmpty) return SetRelation.ProperSupersetOf | SetRelation.Disjoint;
			var intersect = Intersect(other);
			if (intersect.IsEmpty) return SetRelation.Disjoint;
			var otherContainsThis = intersect.Length == Length;
			var thisContainsOther = intersect.Length == other.Length;
			if (otherContainsThis && thisContainsOther) return SetRelation.Equal;
			if (thisContainsOther) return SetRelation.ProperSupersetOf;
			if (otherContainsThis) return SetRelation.ProperSubsetOf;
			return SetRelation.None;
		}

		public bool SetEquals(IEnumerable<TElem> items) {
			return RelatesTo(items) == SetRelation.Equal;

		}

		public bool IsProperSubsetOf(TSet other) {
			return other.IsProperSupersetOf(this);
		}

		public virtual bool IsSupersetOf(TSet other) {
			return this.Length >= other.Length && ((RelatesTo(other) & (SetRelation.ProperSupersetOf | SetRelation.Equal)) != 0);
		}

		public bool IsSubsetOf(TSet other) {
			return other.IsSupersetOf(this);
		}

		public virtual bool IsDisjointWith(TSet other) {
			return RelatesTo(other) == SetRelation.Disjoint;
		}

		public bool IsProperSupersetOf(TSet other) {
			return other.Length < Length && this.IsSupersetOf(other);
		}

		public bool SetEquals(TSet other) {
			return other.Length == Length && this.IsSupersetOf(other);
		}

		public virtual SetRelation RelatesTo(TSet other)
		{
			return RelatesTo(other as IAnySetLike<TElem>);
		}

		public virtual TSet Union(TSet other)
		{
			return Union(other as IAnySetLike<TElem>);
		}

		public TSet Union(IEnumerable<TElem> other)
		{
			if (other is TSet && IsUnionImplemented) {
				return Union((TSet) other);
			}
			using (var builder = BuilderFrom(this))
			{
				other.ForEach(x => { builder.Add(x); });
				return ProviderFrom(builder);
			}
		}
	}
}