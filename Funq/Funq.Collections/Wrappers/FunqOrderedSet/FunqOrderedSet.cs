using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	public partial class FunqOrderedSet<T> : AbstractSet<T, FunqOrderedSet<T>> {
		internal readonly OrderedAvlTree<T, bool>.Node Root;
		internal readonly IComparer<T> Comparer; 
		public static FunqOrderedSet<T> Empty(IComparer<T> cm) {
			return new FunqOrderedSet<T>(OrderedAvlTree<T, bool>.Node.Empty, cm ?? FastComparer<T>.Default);
		}

		internal FunqOrderedSet(OrderedAvlTree<T, bool>.Node root, IComparer<T> comparer )
		{
			Root = root;
			Comparer = comparer;
		}

		public FunqOrderedSet<T> Add(T item) {
			var ret = Root.Root_Add(item, true, Comparer, false, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);

		}

		public FunqOrderedSet<T> Remove(T item) {
			var ret = Root.AvlRemove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(Comparer);
		}

		public override bool Contains(T item) {
			return Root.Contains(item);
		}


		public override int Length
		{
			get
			{
				return Root.Count;
			}
		}

		protected override FunqOrderedSet<T> Difference(FunqOrderedSet<T> other) {
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override FunqOrderedSet<T> Except(FunqOrderedSet<T> other)
		{
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override FunqOrderedSet<T> Union(FunqOrderedSet<T> other)
		{
			return Root.Union(other.Root, null, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override FunqOrderedSet<T> Intersect(FunqOrderedSet<T> other)
		{
			return Root.Intersect(other.Root, null, Lineage.Mutable()).Wrap(Comparer);
		}

		protected override bool IsDisjointWith(FunqOrderedSet<T> other)
		{
			return Root.IsDisjoint(other.Root);
		}

		protected override bool IsSupersetOf(FunqOrderedSet<T> other)
		{
			return Root.IsSupersetOf(other.Root);
		}

		public override bool ForEachWhile(Func<T, bool> iterator) {
			return Root.ForEachWhile((k,v) => iterator(k));
		}

		public T MinItem
		{
			get
			{
				return Root.Min.Key;
			}
		}

		public T MaxItem
		{
			get
			{
				return Root.Max.Key;
			}
		}

		public FunqOrderedSet<T> RemoveMax() {
			return Root.RemoveMax(Lineage.Mutable()).Wrap(Comparer);
		}

		public T ByOrder(int i) {
			return Root.ByOrder(i).Value.Key;
		}

		public FunqOrderedSet<T> RemoveMin() {
			return Root.RemoveMin(Lineage.Mutable()).Wrap(Comparer);
		}

		public override bool IsEmpty
		{
			get
			{
				return Root.IsEmpty;
			}
		}

		protected override IEnumerator<T> GetEnumerator() {
			foreach (var item in Root) {
				yield return item.Key;
			}
		}

		protected override SetRelation RelatesTo(FunqOrderedSet<T> other)
		{
			return Root.Relation(other.Root);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqOrderedSet<T> op_Add(T item)
		{
			return Add(item);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqOrderedSet<T> op_AddRange(IEnumerable<T> item)
		{
			return Union(item);
		}
	}
}
