using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {

	/// <summary>
	/// Immutable and persistent set that uses hashing and equality for membership.
	/// </summary>
	/// <typeparam name="T">The type of element contained in the set.</typeparam>
	public sealed partial class ImmSet<T> : AbstractSet<T, ImmSet<T>> {
		internal readonly IEqualityComparer<T> EqualityComparer;
		internal readonly HashedAvlTree<T, bool>.Node Root;

		internal ImmSet(HashedAvlTree<T, bool>.Node inner, IEqualityComparer<T> eq) {
			EqualityComparer = eq;
			Root = inner;
		}

		protected override ImmSet<T> UnderlyingCollection
		{
			get { return this; }
		}

		internal double CollisionMetric {
			get { return Root.CollisionMetric; }
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public override int Length {
			get { return Root.Count; }
		}

		/// <summary>
		///     Returns true if the collection is empty.
		/// </summary>
		public override bool IsEmpty {
			get { return Root.IsEmpty; }
		}

		/// <summary>
		/// Returns an empty <see cref="ImmSet{T}"/> using the specified eq comparer, or the default.
		/// </summary>
		/// <param name="eq"></param>
		/// <returns></returns>
		public new static ImmSet<T> Empty(IEqualityComparer<T> eq = null) {
			return new ImmSet<T>(HashedAvlTree<T, bool>.Node.Empty, eq ?? FastEquality<T>.Default);
		}

		/// <summary>
		/// Adds a new item to the set, or does nothing if the item already exists.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public override ImmSet<T> Add(T item) {
			var res = Root.Root_Add(item, true, Lineage.Mutable(), EqualityComparer, false);
			if (res == null) return this;
			return res.Wrap(EqualityComparer);
		}

		/// <summary>
		/// Removes an item from the set, or does nothing if the item does not exist.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns></returns>
		public override ImmSet<T> Remove(T item) {
			var ret = Root.Root_Remove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(EqualityComparer);
		}

		protected override Optional<T> TryGet(T item) {
			var kvp = Root.Root_FindKvp(item);
			return kvp.IsSome ? kvp.Value.Key.AsOptional() : Optional.None;
		}

		/// <summary>
		///     Returns true if the item is contained in the set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Contains(T item) {
			return Root.Root_Contains(item);
		}

		protected override bool IsDisjointWith(ImmSet<T> other) {
			return Root.IsDisjoint(other.Root);
		}

		protected override SetRelation RelatesTo(ImmSet<T> other) {
			return Root.RelatesTo(other.Root);
		}

		protected override ImmSet<T> Difference(ImmSet<T> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override ImmSet<T> Except(ImmSet<T> other) {
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override ImmSet<T> Union(ImmSet<T> other) {
			return Root.Union(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override ImmSet<T> Intersect(ImmSet<T> other) {
			return Root.Intersect(other.Root, Lineage.Mutable(), null).Wrap(EqualityComparer);
		}

		internal T ByArbitraryOrder(int index) {
			index.CheckIsBetween("index", -Length, Length - 1);
			index = index < 0 ? index + Length : index;
			return Root.ByArbitraryOrder(index).Key;
		}

		protected override bool IsSupersetOf(ImmSet<T> other) {
			return Root.IsSupersetOf(other.Root);
		}

		/// <summary>
		///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public override bool ForEachWhile(Func<T, bool> function) {
			function.CheckNotNull("function");
			return Root.ForEachWhile((k, v) => function(k));
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override IEnumerator<T> GetEnumerator() {
			foreach (var item in Root) yield return item.Key;
		}
	}
}