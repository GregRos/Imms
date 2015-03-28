using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq.Abstract;
namespace Funq.Collections
{
	internal static class FunqSet2Ext {
		public static FunqSet<T> Wrap<T>(this FunqMap<T, bool> inner) {
			return new FunqSet<T>(inner);
		}
	}



	public partial class FunqSet<T> : AbstractSet<T, FunqSet<T>> {
		private readonly FunqMap<T, bool> _inner;

		public static FunqSet<T> Empty(IEqualityComparer<T> eq) {
			return new FunqSet<T>(FunqMap<T, bool>.Empty(eq));
		}

		internal FunqSet(FunqMap<T, bool> inner) {
			_inner = inner;
		}

		public FunqSet<T> Add(T item) {
			var res = _inner.Set(item, true, OverwriteBehavior.Ignore);
			if (res == null) return this;
			return res.Wrap();
		}

		public FunqSet<T> Drop(T item) {
			return _inner.Drop(item).Wrap();
		}

		public FunqSet<T> AddRange(IEnumerable<T> items) {
			return _inner.AddSetRange(items).Wrap();
		}

		public override bool Contains(T item) {
			return _inner.ContainsKey(item);
		}


		public override int Length
		{
			get
			{
				return _inner.Length;
			}
		}

		public FunqSet<T> DropRange(IEnumerable<T> items) {
			return _inner.DropRange(items).Wrap();
		}

		public override bool IsDisjointWith(FunqSet<T> other) {
			return _inner.Root.IsDisjoint(other._inner.Root);
		}

		public override SetRelation RelatesTo(FunqSet<T> other) {
			return _inner.Root.RelatesTo(other._inner.Root);
		}

		public override FunqSet<T> Difference(FunqSet<T> other) {
			return _inner.Difference(other._inner).Wrap();
		}

		public override FunqSet<T> Except(FunqSet<T> other) {
			return _inner.Except(other._inner).Wrap();
		}

		public override FunqSet<T> Union(FunqSet<T> other) {
			return _inner.Merge(other._inner, null).Wrap();
		}

		public override FunqSet<T> Intersect(FunqSet<T> other) {
			return _inner.Join(other._inner, null).Wrap();
		}

		public override bool IsSupersetOf(FunqSet<T> other) {
			return _inner.Root.IsSupersetOf(other._inner.Root);
		}

		public override bool ForEachWhile(Func<T, bool> iterator) {
			return _inner.ForEachWhile(kvp => iterator(kvp.Key));
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
	}
}
