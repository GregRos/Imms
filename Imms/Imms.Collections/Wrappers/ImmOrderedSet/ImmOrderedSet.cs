using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	public sealed partial class ImmOrderedSet<T> : AbstractSet<T, ImmOrderedSet<T>> {
		internal readonly IComparer<T> Comparer;
		internal readonly OrderedAvlTree<T, bool>.Node Root;

		internal ImmOrderedSet(OrderedAvlTree<T, bool>.Node root, IComparer<T> comparer) {
			Root = root;
			Comparer = comparer;
		}

		public override int Length {
			get { return Root.Count; }
		}

		/// <summary>
		/// Returns the minimum element in this ordered set.
		/// </summary>
		public T MinItem {
			get
			{
				if (Root.IsEmpty) throw Errors.Is_empty;
				return Root.Min.Key;
			}
		}

		/// <summary>
		/// Returns the maximum element in this ordered set.
		/// </summary>
		public T MaxItem {
			get { 
				if (Root.IsEmpty) throw Errors.Is_empty; 
				return Root.Max.Key; 
			}
		}

		public override bool IsEmpty {
			get { return Root.IsEmpty; }
		}

		protected override ImmOrderedSet<T> UnderlyingCollection
		{
			get { return this; }
		}

		public new static ImmOrderedSet<T> Empty(IComparer<T> cm) {
			return new ImmOrderedSet<T>(OrderedAvlTree<T, bool>.Node.Empty, cm ?? FastComparer<T>.Default);
		}

		public override ImmOrderedSet<T> Union(IEnumerable<T> other) {
			other.CheckNotNull("other");
			var set = other as ImmOrderedSet<T>;
			if (set != null && IsCompatibleWith(set)) return Union(set);
			//this trick can't really be repeated with a non-ordered set...
			//or least I haven't figured it out yet. Basically, converts the sequence into an array, sorts it on its own
			//And then builds a tree out of it. Then it unions it with the main tree. This improves performance by a fair bit
			//Even if the data structure isn't an array already.
			int len;
			var arr = other.ToArrayFast(out len);
			Array.Sort(arr, 0, len, Comparer);
			arr.RemoveDuplicatesInSortedArray((a, b) => Comparer.Compare(a, b) == 0, ref len);
			var lineage = Lineage.Mutable();
			var node = OrderedAvlTree<T, bool>.Node.FromSortedArraySet(arr, 0, len - 1, Comparer, lineage);
			var newRoot = node.Union(Root, null, lineage);
			return newRoot.Wrap(Comparer);
		}

		public override ImmOrderedSet<T> Add(T item) {
			var ret = Root.Root_Add(item, true, Comparer, false, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);
		}

		public override ImmOrderedSet<T> Remove(T item) {
			var ret = Root.AvlRemove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);
		}

		protected override Optional<T> TryGet(T item) {
			var ret = Root.FindKvp(item);
			return ret.IsSome ? ret.Value.Key.AsOptional() : Optional.None;
		}

		public override bool Contains(T item) {
			return Root.Contains(item);
		}

		protected override ImmOrderedSet<T> Difference(ImmOrderedSet<T> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmOrderedSet<T> Except(ImmOrderedSet<T> other) {
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmOrderedSet<T> Union(ImmOrderedSet<T> other) {
			return Root.Union(other.Root, null, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmOrderedSet<T> Intersect(ImmOrderedSet<T> other) {
			return Root.Intersect(other.Root, Lineage.Mutable(), null).Wrap(Comparer);
		}

		protected override bool IsDisjointWith(ImmOrderedSet<T> other) {
			return Root.IsDisjoint(other.Root);
		}

		protected override bool IsSupersetOf(ImmOrderedSet<T> other) {
			return Root.IsSupersetOf(other.Root);
		}

		public override bool ForEachWhile(Func<T, bool> function) {
			function.CheckNotNull("function");
			return Root.ForEachWhile((k, v) => function(k));
		}

		public ImmOrderedSet<T> RemoveMax() {
			if (Root.IsEmpty) throw Errors.Is_empty;
			return Root.RemoveMax(Lineage.Mutable()).Wrap(Comparer);
		}

		/// <summary>
		/// Returns the element at the specified position in the sort order.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T ByOrder(int index) {
			index.CheckIsBetween("index", -Length, Length - 1);
			index = index < 0 ? index + Length : index;
			return Root.ByOrder(index).Key;
		}

		public ImmOrderedSet<T> RemoveMin() {
			return Root.RemoveMin(Lineage.Mutable()).Wrap(Comparer);
		}

		public override IEnumerator<T> GetEnumerator() {
			foreach (var item in Root) yield return item.Key;
		}

		protected override SetRelation RelatesTo(ImmOrderedSet<T> other) {
			return Root.Relation(other.Root);
		}
	}
}