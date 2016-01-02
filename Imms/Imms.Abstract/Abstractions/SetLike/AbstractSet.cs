using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;

namespace Imms.Abstract {


	/// <summary>
	///     Parent of all unordered collections containing distinct values, which can efficiently determine membership.
	/// </summary>
	/// <typeparam name="TElem"> </typeparam>
	/// <typeparam name="TSet"> </typeparam>
	public abstract partial class AbstractSet<TElem, TSet>
		: AbstractIterable<TElem, TSet, ISetBuilder<TElem, TSet>>
		where TSet : AbstractSet<TElem, TSet> {

		/// <summary>
		///     Returns true if the item is contained in the set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Contains(TElem item) {
			return TryGet(item).IsSome;
		}

		/// <summary>
		/// Returns true if the item is contained in this set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool this[TElem item] {
			get {
				return Contains(item);
			}
		}

		/// <summary>
		/// Identical to <see cref="Union(TSet)"/>.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public TSet AddRange(IEnumerable<TElem> items) {
			return Union(items);
		}

		/// <summary>
		///     Returns the set-theoretic relation between this set and another set. This member is optimized depending on the
		///     actual type of the input.
		/// </summary>
		/// <param name="other">The input sequence, treated as a set.</param>
		/// <returns></returns>
		public SetRelation RelatesTo(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return RelatesTo(set);
			if (ReferenceEquals(this, other)) return IsEmpty ? SetRelation.Disjoint | SetRelation.Equal : SetRelation.Equal;
			var total = 0;
			var intersectSize = other.Count(x => {
				total += 1;
				return Contains(x);
			});
			if (IsEmpty) {
				if (total == 0) return SetRelation.Equal | SetRelation.Disjoint;
				if (total != 0) return SetRelation.ProperSubsetOf | SetRelation.Disjoint;
			}
			else if (total == 0) {
				return SetRelation.ProperSupersetOf | SetRelation.Disjoint;
			}
			if (intersectSize == 0) return SetRelation.Disjoint;
			var otherContainsThis = intersectSize == Length;
			var thisContainsOther = intersectSize == total;
			if (thisContainsOther && otherContainsThis) return SetRelation.Equal;
			if (thisContainsOther) return SetRelation.ProperSupersetOf;
			if (otherContainsThis) return SetRelation.ProperSubsetOf;
			return SetRelation.None;
		}

		/// <summary>
		/// Adds a new item to the set, or does nothing if the item already exists.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public abstract TSet Add(TElem item);

		/// <summary>
		/// Removes an item from the set, or does nothing if the item does not exist.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns></returns>
		public abstract TSet Remove(TElem item);

		/// <summary>
		/// Returns the instance of the specified element, as it appears in this set, or None if no such instance exists.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract Optional<TElem> TryGet(TElem item);

		/// <summary>
		///     Returns if this set is equal to another set. This member is optimized based on the actual type of the input.
		/// </summary>
		/// <param name="other">The other set.</param>
		/// <returns></returns>
		public virtual bool SetEquals(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			var set = other as TSet;
			if (set != null && IsCompatibleWith(set)) {
				return set.Length == Length && IsSupersetOf(set);
			}
			var guessLength = other.TryGuessLength();
			if (guessLength.IsSome && guessLength.Value < Length) {
				return false;
			}
			set = this.ToIterable(other);
			return set.Length == Length && IsSupersetOf(set);
		}

		/// <summary>
		/// Returns true if this set is a superset of the other set.
		/// </summary>
		/// <param name="other">The other set.</param>
		/// <returns></returns>
		public bool IsSupersetOf(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			var set = other as TSet;
			if (set != null && IsCompatibleWith(set)) {
				return set.Length <= Length && IsSupersetOf(set);
			}
			return other.ForEachWhile(Contains);
		}

		/// <summary>
		/// Returns true if this set is a proper superset of the other set.
		/// </summary>
		/// <param name="other">The other set.</param>
		/// <returns></returns>
		public bool IsProperSupersetOf(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) {
				return set.Length < Length && IsSupersetOf(set);
			}
			set = this.ToIterable(other);
			return set.Length < Length && IsSupersetOf(set);
		}

		/// <summary>
		/// Returns true if this set is a proper subset of the other set.
		/// </summary>
		/// <param name="other">The other set.</param>
		/// <returns></returns>
		public bool IsProperSubsetOf(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			var set = other as TSet;
			if (set != null && IsCompatibleWith(set)) {
				return set.IsProperSupersetOf(this);
			}
			var guessLength = other.TryGuessLength();
			if (guessLength.IsSome && guessLength.Value <= Length) {
				return false;
			}
			var tSet = this.ToIterable(other);
			return tSet.IsProperSupersetOf(this);
		}

		/// <summary>
		/// Returns true if this set is a subset of the other set.
		/// </summary>
		/// <param name="other">The other set.</param>
		/// <returns></returns>
		public bool IsSubsetOf(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) {
				return set.IsSupersetOf(this);
			}
			var guessLength = other.TryGuessLength();
			if (guessLength.IsSome && guessLength.Value < Length) {
				return false;
			}
			var tSet = this.ToIterable(other);
			return tSet.IsProperSupersetOf(this);
		}

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Difference(TSet other) {
			other.CheckNotNull("other");
			var ex1 = Except(other);
			var ex2 = other.Except(this);
			return ex1.Union(ex2);
		}

		/// <summary>
		///     Applies a symmetric difference/XOR between a set, and a set-like collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TSet Difference(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(this, other)) {
				return Empty;
			}
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return Difference(set);
			set = this.ToIterable(other);
			return Difference(set);
		}

		/// <summary>
		///     Applies an inverse except operation, essentially other - this.
		/// </summary>
		/// <param name="other">The other collection, taken to be a set.</param>
		/// <returns></returns>
		public TSet ExceptInverse(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(this, other)) return Empty;
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return set.Except(this);
			using (var builder = EmptyBuilder) {
				other.ForEach(item => {
					if (!Contains(item)) {
						builder.Add(item);
					}
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Checks if this set is compatible with (e.g. same equality semantics) with another set.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected abstract bool IsCompatibleWith(TSet other);

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Except(TSet other) {
			other.CheckNotNull("other");
			return Unchecked_Except(other);
		}

		/// <summary>
		///     Performs the set-theoretic Except operation (non-symmetric difference) with the other collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TSet Except(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(this, other)) return Empty;
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return Except(set);
			return Unchecked_Except(other);
		}

		TSet Unchecked_Except(IEnumerable<TElem> other) {
			using (var builder = BuilderFrom(this)) {
				var len = Length;
				other.ForEachWhile(x => {
					if (builder.Remove(x)) {
						len--;
					}
					return len > 0;
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Applies the set-theoretic Intersect operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TSet Intersect(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(this, other)) return this;
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return Intersect(set);
			int total = 0;
			int len = Length;
			using (var builder = EmptyBuilder) {
				other.ForEachWhile(item => {
					var myKey = TryGet(item);
					if (myKey.IsSome) {
						builder.Add(myKey.Value);
						total++;
					}
					return total < len;
				});
				return builder.Produce();
			}
		}


		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Intersect(TSet other) {
			other.CheckNotNull("other");
			using (var builder = EmptyBuilder) {
				var thisIsShorter = Length <= other.Length;
				var shorterSet = thisIsShorter ? this : (AbstractSet<TElem, TSet>) other;
				var longerSet = thisIsShorter ? (AbstractSet<TElem, TSet>) other : this;
				shorterSet.ForEach(x => {
					if (thisIsShorter) {
						if (longerSet.Contains(x)) builder.Add(x);
					} else {
						var myKey = shorterSet.TryGet(x);
						if (myKey.IsSome) {
							builder.Add(myKey.Value);
						}
					}

				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Returns true if this set is disjoint (shares no elements with) 'other'. The empty set is disjoint with all sets.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IsDisjointWith(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(this, other)) return IsEmpty;
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return IsDisjointWith(set);
			return !other.Any(Contains);
		}

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual bool IsSupersetOf(TSet other) {
			return other.Length <= Length && other.All(Contains);
		}

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual bool IsDisjointWith(TSet other) {
			if (Length < other.Length) return !Any(other.Contains);
			return !other.Any(Contains);
		}

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual SetRelation RelatesTo(TSet other) {
			if (ReferenceEquals(this, other)) return IsEmpty ? SetRelation.Disjoint | SetRelation.Equal : SetRelation.Equal;
			if (IsEmpty && other.IsEmpty) return SetRelation.Equal | SetRelation.Disjoint;
			if (IsEmpty && !other.IsEmpty) return SetRelation.ProperSubsetOf | SetRelation.Disjoint;
			if (!IsEmpty && other.IsEmpty) return SetRelation.ProperSupersetOf | SetRelation.Disjoint;
			var driver = Length > other.Length ? other : (TSet)this;
			var checker = driver == this ? other : (TSet)this;
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
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected virtual TSet Union(TSet other) {
			return Union_Unchecked(other);
		}

		TSet Union_Unchecked(IEnumerable<TElem> other) {
			if (ReferenceEquals(this, other)) return this;
			using (var builder = BuilderFrom(this)) {
				builder.AddRange(other);
				return builder.Produce();
			}
		}

		/// <summary>
		///     Returns the set-theoretic union between this set and a set-like collection.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual TSet Union(IEnumerable<TElem> other) {
			other.CheckNotNull("other");
			if (ReferenceEquals(other, this)) return this;
			TSet set = other as TSet;
			if (set != null && IsCompatibleWith(set)) return Union(set);
			return Union_Unchecked(other);
		}
	}

}