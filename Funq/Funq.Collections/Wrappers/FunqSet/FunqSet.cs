using System;
using System.Collections.Generic;
using Funq.Abstract;
using Funq.Implementation;

namespace Funq {

	public sealed partial class FunqSet<T> : AbstractSet<T, FunqSet<T>> {
		internal readonly IEqualityComparer<T> EqualityComparer;
		internal readonly HashedAvlTree<T, bool>.Node Root;

		internal FunqSet(HashedAvlTree<T, bool>.Node inner, IEqualityComparer<T> eq) {
			EqualityComparer = eq;
			Root = inner;
		}

		protected override FunqSet<T> UnderlyingCollection
		{
			get { return this; }
		}

		internal double CollisionMetric {
			get { return Root.CollisionMetric; }
		}

		public override int Length {
			get { return Root.Count; }
		}

		public override bool IsEmpty {
			get { return Root.IsEmpty; }
		}

		public new static FunqSet<T> Empty(IEqualityComparer<T> eq = null) {
			return new FunqSet<T>(HashedAvlTree<T, bool>.Node.Empty, eq ?? FastEquality<T>.Default);
		}

		public override FunqSet<T> Add(T item) {
			var res = Root.Root_Add(item, true, Lineage.Mutable(), EqualityComparer, false);
			if (res == null) return this;
			return res.Wrap(EqualityComparer);
		}

		public override FunqSet<T> Remove(T item) {
			var ret = Root.Root_Remove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(EqualityComparer);
		}

		protected override Optional<T> TryGet(T item) {
			var kvp = Root.Root_FindKvp(item);
			return kvp.IsSome ? kvp.Value.Key.AsOptional() : Optional.None;
		}

		public override bool Contains(T item) {
			return Root.Root_Contains(item);
		}

		protected override bool IsDisjointWith(FunqSet<T> other) {
			return Root.IsDisjoint(other.Root);
		}

		protected override SetRelation RelatesTo(FunqSet<T> other) {
			return Root.RelatesTo(other.Root);
		}

		protected override FunqSet<T> Difference(FunqSet<T> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Except(FunqSet<T> other) {
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Union(FunqSet<T> other) {
			return Root.Union(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Intersect(FunqSet<T> other) {
			return Root.Intersect(other.Root, Lineage.Mutable(), null).Wrap(EqualityComparer);
		}

		internal T ByArbitraryOrder(int index) {
			index.CheckIsBetween("index", -Length, Length - 1);
			index = index < 0 ? index + Length : index;
			return Root.ByArbitraryOrder(index).Key;
		}

		protected override bool IsSupersetOf(FunqSet<T> other) {
			return Root.IsSupersetOf(other.Root);
		}

		public override bool ForEachWhile(Func<T, bool> function) {
			function.CheckNotNull("function");
			return Root.ForEachWhile((k, v) => function(k));
		}

		public override IEnumerator<T> GetEnumerator() {
			foreach (var item in Root) yield return item.Key;
		}
	}
}