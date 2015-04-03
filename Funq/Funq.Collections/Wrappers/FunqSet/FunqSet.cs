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
	
	public sealed partial class FunqSet<T> : AbstractSet<T, FunqSet<T>> {
		internal readonly HashedAvlTree<T, bool>.Node Root;
		internal readonly IEqualityComparer<T> EqualityComparer; 
		public static FunqSet<T> Empty(IEqualityComparer<T> eq = null) {
			return new FunqSet<T>(HashedAvlTree<T, bool>.Node.Empty, eq ?? FastEquality<T>.Default);
		}

		internal FunqSet(HashedAvlTree<T, bool>.Node inner, IEqualityComparer<T> eq) {
			EqualityComparer = eq;
			Root = inner;
		}

		public FunqSet<T> Add(T item) {
			var res = Root.Root_Add(item, true, Lineage.Mutable(), EqualityComparer, false);
			if (res == null) return this;
			return res.Wrap(EqualityComparer);
		}

		internal double CollisionMetric
		{
			get
			{
				return Root.CollisionMetric;
			}
		}

		public FunqSet<T> Remove(T item) {
			var ret = Root.Root_Remove(item, Lineage.Mutable());
			if (ret == null) return this;
			return ret.Wrap(EqualityComparer);
		}

		public override bool Contains(T item) {
			return Root.Root_Contains(item);
		}


		public override int Length
		{
			get
			{
				return Root.Count;
			}
		}

		protected override bool IsDisjointWith(FunqSet<T> other)
		{
			return Root.IsDisjoint(other.Root);
		}

		protected override SetRelation RelatesTo(FunqSet<T> other)
		{
			return Root.RelatesTo(other.Root);
		}

		protected override FunqSet<T> Difference(FunqSet<T> other)
		{
			return Root.SymDifference(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Except(FunqSet<T> other) {
			return Root.Except(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Union(FunqSet<T> other) {
			return Root.Union(other.Root, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override FunqSet<T> Intersect(FunqSet<T> other) {
			return Root.Intersect(other.Root, null, Lineage.Mutable()).Wrap(EqualityComparer);
		}

		protected override bool IsSupersetOf(FunqSet<T> other)
		{
			return Root.IsSupersetOf(other.Root);
		}

		public override bool ForEachWhile(Func<T, bool> iterator) {
			return Root.ForEachWhile((k, v) => iterator(k));
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqSet<T> op_Add(T item) {
			return Add(item);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public FunqSet<T> op_AddRange(IEnumerable<T> item)
		{
			return Union(item);
		}
	}
}
