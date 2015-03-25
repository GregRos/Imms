using System;
using System.Collections.Generic;
using System.Net.Configuration;

namespace Funq.Abstract
{

	/// <summary>
	///   Parent of all unordered collections containing distinct values, which can efficiently determine membership.
	/// </summary>
	/// <typeparam name="TSet"> </typeparam>
	/// <typeparam name="TElem"> </typeparam>
	/// <typeparam name="TProvider"> </typeparam>
	public abstract class Trait_SetLike<TElem, TProvider>
		: Trait_Iterable<TElem, TProvider, SetBuilder<TElem>>, ITrait_SetLike<TElem>
		where TProvider : Trait_SetLike<TElem, TProvider>
	{

		public abstract bool Contains(TElem item);

		/// <summary>
		/// Applies a symmetric difference/XOR between two sets. In symbols, A△B.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TProvider Difference(TProvider other)
		{
			return Difference(other as ITrait_SetLike<TElem>);
		}

		/// <summary>
		/// Applies a symmetric difference/XOR between a set, and a set-like collection. 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TProvider Difference(ITrait_SetLike<TElem> other)
		{
			var union = Union(other);
			var intersect = Intersect(other);
			return union.Except(intersect);
		}

		public virtual TProvider Except(TProvider other)
		{
			return Except(other as ITrait_SetLike<TElem>);
		}

		public TProvider Except(ITrait_SetLike<TElem> other)
		{
			using (var builder = EmptyBuilder)
			{
				ForEach(x => { if (!other.Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		public TProvider ExceptInverse(ITrait_SetLike<TElem> other)
		{
			using (var builder = EmptyBuilder)
			{
				var y = new HashSet<int>();
				other.ForEach(x => { if (!Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		public virtual TProvider ExceptInverse(TProvider other)
		{
			return ExceptInverse(other as ITrait_SetLike<TElem>);
		}

		public TProvider Intersect(ITrait_SetLike<TElem> other)
		{
			using (var builder = EmptyBuilder)
			{
				var shorterSet = Length > other.Length ? other : this;
				var longerSet = shorterSet == other ? this : other;
				shorterSet.ForEach(x => { if (longerSet.Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		public virtual TProvider Intersect(TProvider other)
		{
			return Intersect(other as ITrait_SetLike<TElem>);
		}

		public SetRelation RelatesTo(ITrait_SetLike<TElem> other) {
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

		public bool IsProperSubsetOf(TProvider other) {
			return other.IsProperSupersetOf(this);
		}

		public virtual bool IsSupersetOf(TProvider other) {
			return this.Length >= other.Length && ((RelatesTo(other) & (SetRelation.ProperSupersetOf | SetRelation.Equal)) != 0);
		}

		public bool IsSubsetOf(TProvider other) {
			return other.IsSupersetOf(this);
		}

		public virtual bool IsDisjointWith(TProvider other) {
			return RelatesTo(other) == SetRelation.Disjoint;
		}

		public bool IsProperSupersetOf(TProvider other) {
			return other.Length < Length && this.IsSupersetOf(other);
		}

		public bool SetEquals(TProvider other) {
			return other.Length == Length && this.IsSupersetOf(other);
		}

		public virtual SetRelation RelatesTo(TProvider other)
		{
			return RelatesTo(other as ITrait_SetLike<TElem>);
		}

		public virtual TProvider Union(TProvider other)
		{
			return Union(other as ITrait_SetLike<TElem>);
		}

		public TProvider Union(ITrait_SetLike<TElem> other)
		{
			using (var builder = BuilderFrom(this))
			{
				other.ForEach(x => { builder.Add(x); });
				return ProviderFrom(builder);
			}
		}
	}
}