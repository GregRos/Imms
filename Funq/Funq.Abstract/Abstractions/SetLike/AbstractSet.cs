using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Funq.Abstract {
	/// <summary>
	///     Parent of all unordered collections containing distinct values, which can efficiently determine membership.
	/// </summary>
	/// <typeparam name="TElem"> </typeparam>
	/// <typeparam name="TSet"> </typeparam>
	public abstract partial class AbstractSet<TElem, TSet>
		: AbstractIterable<TElem, TSet, SetBuilder<TElem>>, IAnySetLike<TElem>
		where TSet : AbstractSet<TElem, TSet> {


		/// <summary>
		/// Returns true if the item is contained in the set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool Contains(TElem item);

		/// <summary>
		/// Returns the set-theoretic relation between this set and another set. This member is optimized depending on the actual type of the input.
		/// </summary>
		/// <param name="other">The input sequence, treated as a set.</param>
		/// <returns></returns>
		public SetRelation RelatesTo(IEnumerable<TElem> other) {
			if (other is TSet && IsCompatibleWith((TSet)other)) return RelatesTo((TSet) other);
			if (other is IAnySetLike<TElem>) return RelatesTo_Unchecked((IAnySetLike<TElem>) other);
			if (this.RefEquals(other)) return IsEmpty ? SetRelation.Disjoint | SetRelation.Equal : SetRelation.Equal;
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

		/// <summary>
		/// Returns if this set is equal to another set. This member is optimized based on the actual type of the input.
		/// </summary>
		/// <param name="seq">The other set.</param>
		/// <returns></returns>
		public bool SetEquals(IEnumerable<TElem> seq) {
			var length = 0;
			var res = IsSupersetOf(seq, out length);
			return res && length == Length;
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Difference(TSet other) {
			var ex1 = this.Except(other);
			var ex2 = other.Except(this);
			return ex1.Union(ex2);
		}

		/// <summary>
		///     Applies a symmetric difference/XOR between a set, and a set-like collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TSet Difference(IEnumerable<TElem> other) {
			if (other is TSet) return Difference((TSet) other);
			if (other is IAnySetLike<TElem>) return Unchecked_Difference((IAnySetLike<TElem>) other);
			TSet otherProvider = ToIterable(this, other);
			return Difference(otherProvider);
		}

		public TSet ExceptInverse(IEnumerable<TElem> other) {
			if (this.RefEquals(other)) {
				return ProviderFrom(EmptyBuilder);
			}
			if (other is TSet && IsCompatibleWith((TSet)other)) {
				return ((TSet) other).Except(this);
			}
			using (var builder = EmptyBuilder) {
				other.ForEach(item => {
					if (!Contains(item)) {
						builder.Add(item);
					}
				});
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		/// Checks if this set is compatible with (e.g. same equality semantics) with another set.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected abstract bool IsCompatibleWith(TSet other);

		
		TSet Unchecked_Difference(IAnySetLike<TElem> other) {
			var union = Union(other);
			var intersect = Unchecked_Intersect(other);
			return union.Except(intersect);
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Except(TSet other) {
			return Unchecked_Except(other);
		}

		/// <summary>
		/// Performs the set-theoretic Except operation (non-symmetric difference) with the other collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TSet Except(IEnumerable<TElem> other) {
			if (ReferenceEquals(this, other)) return ProviderFrom(EmptyBuilder);
			if (other is TSet && IsCompatibleWith((TSet)other)) return Except((TSet) other);
			return Unchecked_Except(other);
		}

		TSet Unchecked_Except(IEnumerable<TElem> other)
		{
			using (var builder = BuilderFrom(this))
			{
				other.ForEach(x => { builder.Remove(x); });
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		/// Applies the set-theoretic Intersect operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TSet Intersect(IEnumerable<TElem> other) {
			if (ReferenceEquals(this, other)) return this;
			if (other is TSet && IsCompatibleWith((TSet)other)) return Intersect((TSet) other);
			if (other is IAnySetLike<TElem>) return Unchecked_Intersect((IAnySetLike<TElem>) other);
			using (var builder = EmptyBuilder) {
				other.ForEach(item => {
					if (Contains(item)) builder.Add(item);
				});
				return ProviderFrom(builder);
			}
		}

		TSet Unchecked_Intersect(IAnySetLike<TElem> other) {
			using (var builder = EmptyBuilder) {
				var shorterSet = Length > other.Length ? other : this;
				var longerSet = shorterSet == other ? this : other;
				shorterSet.ForEach(x => { if (longerSet.Contains(x)) builder.Add(x); });
				return ProviderFrom(builder);
			}
		}
		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Intersect(TSet other) {
			return Unchecked_Intersect(other);
		}

		SetRelation RelatesTo_Unchecked(IAnySetLike<TElem> other) {
			if (this.RefEquals(other)) return IsEmpty ? SetRelation.Disjoint | SetRelation.Equal : SetRelation.Equal;
			if (IsEmpty && other.IsEmpty()) return SetRelation.Equal | SetRelation.Disjoint;
			if (IsEmpty && !other.IsEmpty()) return SetRelation.ProperSubsetOf | SetRelation.Disjoint;
			if (!IsEmpty && other.IsEmpty()) return SetRelation.ProperSupersetOf | SetRelation.Disjoint;
			var driver = Length > other.Length ? other : this;
			var checker = driver == this ? other : this;
			var intersectCount = driver.Count(checker.Contains);
			if (intersectCount == 0) return SetRelation.Disjoint;
			var otherContainsThis = intersectCount == Length;
			var thisContainsOther = intersectCount == other.Length;
			if (otherContainsThis && thisContainsOther) return SetRelation.Equal;
			if (thisContainsOther) return SetRelation.ProperSupersetOf;
			if (otherContainsThis) return SetRelation.ProperSubsetOf;
			return SetRelation.None;
		}

		/// <summary>
		///     Returns true if this is a superset of the other, and maybe also the length of seq as an output parameter.
		///     IF THIS METHOD RETURNS 'false' LENGTH IS PROBABLY WRONG
		/// </summary>
		/// <param name="seq"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		bool IsSupersetOf(IEnumerable<TElem> seq, out int length) {
			if (this.RefEquals(seq)) {
				length = Length;
				return true;
			}
			var isSuper = true;
			if (seq is TSet && IsCompatibleWith((TSet)seq)) {
				var asSet = (TSet) seq;
				length = asSet.Length;
				return IsSupersetOf(asSet);
			}
			return IsSupersetOf_Unchecked(seq, out length);
		}

		public bool IsDisjointWith(IEnumerable<TElem> seq) {
			if (this.RefEquals(seq)) return IsEmpty;
			if (seq is TSet && IsCompatibleWith((TSet)seq)) return IsDisjointWith((TSet) seq);
			if (seq is IAnySetLike<TElem>) return IsDisjointWith_Unchecked((IAnySetLike<TElem>) seq);
			return !seq.ForEachWhile(Contains);
		}

		bool IsDisjointWith_Unchecked(IAnySetLike<TElem> other) {
			if (Length < other.Length) return !ForEachWhile(other.Contains);
			else return !other.ForEachWhile(Contains);
		}

		bool IsSupersetOf_Unchecked(IEnumerable<TElem> seq, out int length) {
			var len = 0;
			var ret = seq.ForEachWhile(x => {
				len++;
				if (!Contains(x)) return false;
				return true;
			});
			length = len;
			return ret;
		}

		public bool IsSupersetOf(IEnumerable<TElem> seq) {
			var length = 0;
			var res = IsSupersetOf(seq, out length);
			return res;
		}

		public bool IsProperSupersetOf(IEnumerable<TElem> seq) {
			var length = 0;
			var res = IsSupersetOf(seq, out length);
			return res && length > Length;
		}

		public bool IsProperSubsetOf(IEnumerable<TElem> seq) {
			var length = 0;
			var res = IsSubsetOf(seq, out length);
			return res && length > Length;
		}

		public bool IsSubsetOf(IEnumerable<TElem> seq) {
			var dummy = 0;
			return IsSubsetOf(seq, out dummy);
		}

		/// <summary>
		///     This method should not be called o
		///     <br />
		///     Returns if this set is a subset of seq, and the length of seq if it is true. <br />
		///     In this case the lenght is always correct, even if the result is false.
		/// </summary>
		/// <param name="seq"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		bool IsSubsetOf(IEnumerable<TElem> seq, out int length) {
			if (this.RefEquals(seq)) {
				length = Length;
				return true;
			}
			if (seq is TSet) {
				var asSet = seq as TSet;
				length = asSet.Length;
				return asSet.IsSupersetOf(this);
			}
			if (seq is IAnySetLike<TElem>) {
				var set = (IAnySetLike<TElem>) seq;
				length = set.Length;
				return IsSubsetOf_Unchecked(set);
			}
			var hs = seq.ToHashSet();
			length = hs.Count;
			var ret = this.ForEachWhile(item => {
				if (!hs.Contains(item)) return false;
				return true;
			});
			return ret;
		}

		bool IsSubsetOf_Unchecked(IAnySetLike<TElem> set) {
			if (Length < set.Length) return ForEachWhile(set.Contains);
			else return set.ForEachWhile(this.Contains);
		}
		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual bool IsSupersetOf(TSet other) {
			return IsSupersetOf((IEnumerable<TElem>) other);
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual bool IsDisjointWith(TSet other) {
			return IsDisjointWith_Unchecked(other);
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual SetRelation RelatesTo(TSet other) {
			return RelatesTo_Unchecked(other);
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Union(TSet other) {
			return Union_Unchecked(other);
		}

		TSet Union_Unchecked(IEnumerable<TElem> other) {
			if (this.RefEquals(other)) return this;
			using (var builder = BuilderFrom(this)) {
				other.ForEach(x => { builder.Add(x); });
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		/// Returns the set-theoretic union between this set and a set-like collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TSet Union(IEnumerable<TElem> other) {
			if (other.RefEquals(this)) return this;
			if (other is TSet && IsCompatibleWith((TSet)other)) return Union((TSet) other);
			return Union_Unchecked(other);
		}
	}
}