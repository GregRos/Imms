using System;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	/// <summary>
	/// Immutable and persistent ordered set. Uses comparison semantics, and allows looking up items by sort order.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed partial class ImmSortedSet<T> : AbstractSet<T, ImmSortedSet<T>> {
		internal readonly IComparer<T> Comparer;
		internal readonly OrderedAvlTree<T, bool>.Node Root;

		internal ImmSortedSet(OrderedAvlTree<T, bool>.Node root, IComparer<T> comparer) {
			Root = root;
			Comparer = comparer;
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
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

		/// <summary>
		/// Returns the index of the specified key, by sort order, or None if the key was not found.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Optional<int> IndexOf(T key) {
			return Root.IndexOf(key);
		}

		/// <summary>
		///     Returns true if the collection is empty.
		/// </summary>
		public override bool IsEmpty {
			get { return Root.IsEmpty; }
		}

		protected override ImmSortedSet<T> UnderlyingCollection
		{
			get { return this; }
		}

		/// <summary>
		/// Returns an empty <see cref="ImmSortedSet{T}"/> using the specified comparer.
		/// </summary>
		/// <param name="cm"></param>
		/// <returns></returns>
		public new static ImmSortedSet<T> Empty(IComparer<T> cm) {
			return new ImmSortedSet<T>(OrderedAvlTree<T, bool>.Node.Empty, cm ?? FastComparer<T>.Default);
		}

		/// <summary>
		///     Returns the set-theoretic union between this set and a set-like collection.
		/// </summary>
		/// <param name="other">A sequence of values. This operation is much faster if it's a set compatible with this one.</param>
		/// <returns></returns>
		public override ImmSortedSet<T> Union(IEnumerable<T> other) {
			other.CheckNotNull("other");
			var set = other as ImmSortedSet<T>;
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

		/// <summary>
		/// Adds a new item to the set, or does nothing if the item already exists.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public override ImmSortedSet<T> Add(T item) {
			var ret = Root.Root_Add(item, true, Comparer, false, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);
		}

		/// <summary>
		/// Removes an item from the set, or does nothing if the item does not exist.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns></returns>
		public override ImmSortedSet<T> Remove(T item) {
			var ret = Root.AvlRemove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);
		}

		protected override Optional<T> TryGet(T item) {
			var ret = Root.FindKvp(item);
			return ret.IsSome ? ret.Value.Key.AsSome() : Optional.None;
		}

		/// <summary>
		///     Returns true if the item is contained in the set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Contains(T item) {
			return Root.Contains(item);
		}

		protected override ImmSortedSet<T> Difference(ImmSortedSet<T> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmSortedSet<T> Except(ImmSortedSet<T> other) {
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmSortedSet<T> Union(ImmSortedSet<T> other) {
			return Root.Union(other.Root, null, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override ImmSortedSet<T> Intersect(ImmSortedSet<T> other) {
			return Root.Intersect(other.Root, Lineage.Mutable(), null).Wrap(Comparer);
		}

		protected override bool IsDisjointWith(ImmSortedSet<T> other) {
			return Root.IsDisjoint(other.Root);
		}

		protected override bool IsSupersetOf(ImmSortedSet<T> other) {
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
		/// Removes the maximal element from the set.
		/// </summary>
		/// <returns></returns>
		public ImmSortedSet<T> RemoveMax() {
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

		/// <summary>
		/// Removes the minimal element from this set.
		/// </summary>
		/// <returns></returns>
		public ImmSortedSet<T> RemoveMin() {
			return Root.RemoveMin(Lineage.Mutable()).Wrap(Comparer);
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

		protected override SetRelation RelatesTo(ImmSortedSet<T> other) {
			return Root.Relation(other.Root);
		}

		/// <summary>
		/// Returns a submap containing keys lower than <paramref name="maxItem"/> keys.
		/// </summary>
		/// <param name="maxItem"></param>
		/// <returns></returns>
		public ImmSortedSet<T> TakeLess(T maxItem) {
			OrderedAvlTree<T, bool>.Node left, central, right;
			Root.Split(maxItem, out left, out central, out right, Lineage.Immutable);
			return left.Wrap(Comparer);
		}

		/// <summary>
		/// Returns a submap consisting of keys larger than <paramref name="minItem"/>.
		/// </summary>
		/// <param name="minItem"></param>
		/// <returns></returns>
		public ImmSortedSet<T> TakeMore(T minItem) {
			OrderedAvlTree<T, bool>.Node left, central, right;
			Root.Split(minItem, out left, out central, out right, Lineage.Immutable);
			return right.Wrap(Comparer);
		}
	}
}