using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq.Abstract;
namespace Funq.Collections
{
	internal static class FunqOrderedSet2Ext {
		public static FunqOrderedSet<T> Wrap<T>(this FunqOrderedMap<T, bool> inner) {
			return new FunqOrderedSet<T>(inner);
		}
	}
	public partial class FunqOrderedSet<T> : Trait_SetLike<T, FunqOrderedSet<T>> {
		private readonly FunqOrderedMap<T, bool> _inner;

		public static FunqOrderedSet<T> Empty(IComparer<T> cm) {
			return new FunqOrderedSet<T>(FunqOrderedMap<T, bool>.Empty(cm));
		}

		internal FunqOrderedSet(FunqOrderedMap<T, bool> inner) {
			_inner = inner;
		}

		public FunqOrderedSet<T> Add(T item) {
			var ret = _inner.Set(item, true, OverwriteBehavior.Ignore);
			if (ret == null) return this;
			return ret.Wrap();

		}

		public FunqOrderedSet<T> Drop(T item) {
			var ret = _inner.Drop(item);
			if (ret == null) return this;
			return ret.Wrap();
		}

		public FunqOrderedSet<T> AddRange(IEnumerable<T> items) {
			return _inner.AddRange(items.Select(x => Kvp.Of(x, true)), OverwriteBehavior.Ignore).Wrap();
		}

		public override bool Contains(T item) {
			return _inner.Root.Contains(item);
		}


		public override int Length
		{
			get
			{
				return _inner.Length;
			}
		}

		public override FunqOrderedSet<T> Difference(FunqOrderedSet<T> other) {
			return _inner.Difference(other._inner).Wrap();
		}

		public override FunqOrderedSet<T> Except(FunqOrderedSet<T> other) {
			return _inner.Except(other._inner).Wrap();
		}

		public override FunqOrderedSet<T> Union(FunqOrderedSet<T> other) {
			return _inner.Merge(other._inner, null).Wrap();
		}

		public override FunqOrderedSet<T> Intersect(FunqOrderedSet<T> other) {
			return _inner.Join(other._inner, null).Wrap();
		}

		public override bool IsDisjointWith(FunqOrderedSet<T> other)
		{
			return _inner.Root.IsDisjoint(other._inner.Root);
		}

		public override bool IsSupersetOf(FunqOrderedSet<T> other) {
			return _inner.Root.IsSupersetOf(other._inner.Root);
		}

		public override bool ForEachWhile(Func<T, bool> iterator) {
			return _inner.ForEachWhile(kvp => iterator(kvp.Key));
		}

		public FunqOrderedSet<T> DropRange(IEnumerable<T> items) {
			return _inner.DropRange(items).Wrap();
		} 

		public T MinItem
		{
			get
			{
				return _inner.MinItem.Key;
			}
		}

		public T MaxItem
		{
			get
			{
				return _inner.MaxItem.Key;
			}
		}

		public FunqOrderedSet<T> DropMax() {
			return _inner.DropMax().Wrap();
		}

		public T ByOrder(int i) {
			return _inner.ByOrder(i).Key;
		}

		public FunqOrderedSet<T> DropMin() {
			return _inner.DropMin().Wrap();
		}

		public override bool IsEmpty
		{
			get
			{
				return _inner.IsEmpty;
			}
		}

		protected override IEnumerator<T> GetEnumerator() {
			return _inner.Select(x => x.Key).GetEnumerator();
		}

		public override SetRelation RelatesTo(FunqOrderedSet<T> other) {
			return _inner.Root.Relation(other._inner.Root);
		}
	}
}
