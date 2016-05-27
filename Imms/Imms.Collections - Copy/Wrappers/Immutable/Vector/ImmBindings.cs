using System;
using System.Collections;
using System.Collections.Generic;
using Imms.Abstract;
using Imms.Implementation;

namespace Imms {
	public partial class ImmVector<T> : IList<T>, IList, IReadOnlyList<T> {
		protected override ISequentialBuilder<T, ImmVector<T>> EmptyBuilder {
			get { return new Builder(); }
		}

		/// <summary>
		///     Gets the value of the item with the specified index. O(logn); immediate.
		/// </summary>
		/// <param name="index"> The index of the item to get. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		protected override T GetItem(int index) {
			return Root[index];

		}

		protected override ISequentialBuilder<T, ImmVector<T>> BuilderFrom(ImmVector<T> collection) {
			return new Builder(collection);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override IEnumerator<T> GetEnumerator() {
			return Root.GetEnumerator(true);
		}

		class Builder : ISequentialBuilder<T, ImmVector<T>> {
			TrieVector<T>.Node _inner;
			Lineage _lineage;

			public Builder()
				: this(Empty) {}

			internal Builder(ImmVector<T> inner) {
				_inner = inner.Root;
				_lineage = Lineage.Mutable();
			}

			public ImmVector<T> Produce() {
				_lineage = Lineage.Mutable();
				return new ImmVector<T>(_inner);
			}

			public bool Add(T item) {
				_inner = _inner.Add(item, _lineage);
				return true;
			}

			public void AddRange(IEnumerable<T> items) {
				items.CheckNotNull("items");
				if (_inner.Length == 0) {
					var vector = items as ImmVector<T>;
					if (vector != null) {
						_inner = vector;
						return;
					}
				}
				int len;
				var arr = items.ToArrayFast(out len);
				var s = 0;
				_inner = _inner.AddRange(arr, _lineage, 6, ref s, ref len);
			}

			public int Length {
				get { return _inner.Length; }
			}

			public void Dispose() {
				_lineage = Lineage.Mutable();
			}
		}
	}
}