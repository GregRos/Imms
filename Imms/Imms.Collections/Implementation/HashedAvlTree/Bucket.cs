using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Imms.Implementation {

	static partial class HashedAvlTree<TKey, TValue> {
		/// <summary>
		///     An unordered linked list of elements used to store key-value pairs that have identical hashes.
		/// </summary>
		[DebuggerTypeProxy(typeof (HashedAvlTree<,>.Bucket.BucketDebugView))]
		internal sealed class Bucket {
			private static readonly Bucket Empty = new Bucket(true);
			readonly Lineage _lineage;
			public readonly bool IsEmpty;
			public int Count;
			public readonly IEqualityComparer<TKey> Eq;
			public TKey Key;
			public Bucket Next = Empty;
			public TValue Value;

			Bucket(bool isEmpty) {
				IsEmpty = isEmpty;
				_lineage = Lineage.Immutable;
				Count = 0;
			}

			Bucket(TKey k, TValue v, Bucket next, IEqualityComparer<TKey> eq, Lineage lin) {
				Eq = eq;
				Key = k;
				Value = v;
				Next = next;
				_lineage = lin;
				Count = next.Count + 1;
			}

			public Bucket ByIndex(int index) {
				Bucket bucket = this;
				for (; index > 0; bucket = bucket.Next, index--) {

				}
				return bucket;
			}


			public KeyValuePair<TKey, TValue> AsKvp {
				get { return Kvp.Of(Key, Value); }
			}

			private IEnumerable<KeyValuePair<TKey, TValue>> Items {
				get {
					for (var cur = this; !cur.IsEmpty; cur = cur.Next) yield return Kvp.Of(cur.Key, cur.Value);
				}
			}

			public IEnumerable<Bucket> Buckets {
				get {
					for (var cur = this; !cur.IsEmpty; cur = cur.Next) yield return cur;
				}
			}

			private Bucket NewBucket(TKey k, TValue v, Bucket next, Lineage lin) {
				return new Bucket(k, v, next, Eq, lin);
			}

			private Bucket WithNext(Bucket newNext, Lineage lineage) {
				if (_lineage.AllowMutation(lineage)) {
					Next = newNext;
					Count = newNext.Count + 1;
					return this;
				}
				return new Bucket(Key, Value, newNext, Eq, lineage);
			}

			private Bucket WithKeyValue(TKey newKey, TValue newValue, Lineage lin) {
				if (_lineage.AllowMutation(lin)) {
					Key = newKey;
					Value = newValue;
					return this;
				}
				return new Bucket(newKey, newValue, Next, Eq, lin);
			}

			public override string ToString() {
				return string.Join("; ", Buckets.Select(x => x.AsKvp.ToString()));
			}

			public Optional<TValue> Find(TKey findKey)
			{
				for (var cur = this; !cur.IsEmpty; cur = cur.Next) if (Eq.Equals(cur.Key, findKey)) return cur.Value;
				return Optional.None;
			}

			public Optional<KeyValuePair<TKey, TValue>> FindKvp(TKey findKey) {
				for (var cur = this; !cur.IsEmpty; cur = cur.Next) if (Eq.Equals(cur.Key, findKey)) return cur.AsKvp;
				return Optional.None;
			}

			public Bucket Remove(TKey findKey, Lineage lineage) {
				if (IsEmpty) return null;
				if (Eq.Equals(findKey, Key)) return Next;
				var newNext = Next.Remove(findKey, lineage);

				return WithNext(newNext, lineage);
			}

			public Bucket Add(TKey key, TValue v, Lineage lin, bool overwrite) {
				if (IsEmpty) throw ImplErrors.Invalid_invocation("Empty Bucket");
				if (Eq.Equals(key, Key)) return overwrite ? WithKeyValue(key, v, lin) : null;
				var newNext = Next.IsEmpty ? new Bucket(key, v, Empty, Eq, lin) : Next.Add(key, v, lin, overwrite);
				if (newNext == null) return null;
				return WithNext(newNext, lin);
			}

			public int CountIntersection(Bucket other) {
				var count = 0;
				foreach (var item in Items) if (other.Find(item.Key).IsSome) count++;
				return count;
			}

			public Bucket Except<TValue2>(HashedAvlTree<TKey, TValue2>.Bucket other, Lineage lineage,
				Func<TKey, TValue, TValue2, Optional<TValue>> subtraction = null) {
				var newBucket = Empty;
				foreach (var item in Items) {
					var findOther = other.Find(item.Key);
					if (findOther.IsNone) newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);
					else if (subtraction != null) {
						var newValue = subtraction(item.Key, item.Value, findOther.Value);
						if (newValue.IsSome) newBucket = NewBucket(item.Key, newValue.Value, newBucket, lineage);
					}
				}
				return newBucket;
			}

			public Bucket Union(Bucket other, Func<TKey, TValue, TValue, TValue> collision, Lineage lineage) {
				var newBucket = Empty;
				foreach (var item in Items) {
					var findOther = other.Find(item.Key);
					var valueToAdd = item.Value;
					if (findOther.IsSome) {
						other = other.Remove(item.Key, lineage);
						valueToAdd = collision == null ? findOther.Value : collision(item.Key, item.Value, findOther.Value);
					}
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}

				foreach (var item in other.Items) newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);

				return newBucket;
			}

			public Bucket Intersect(Bucket other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision = null) {

				var newBucket = Empty;
				foreach (var item in Items) {
					var otherItem = other.Find(item.Key);
					if (otherItem.IsNone) continue;
					var valueToAdd = collision == null ? item.Value : collision(item.Key, item.Value, otherItem.Value);
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}
				return newBucket;
			}

			public bool ForEachWhile(Func<TKey, TValue, bool> iterate) {
				for (var cur = this; !cur.IsEmpty; cur = cur.Next) {
					var res = iterate(cur.Key, cur.Value);
					if (!res) return false;
				}
				return true;
			}

			public static Bucket FromKvp(TKey k, TValue v, IEqualityComparer<TKey> eq, Lineage lin) {
				return new Bucket(k, v, Empty, eq, lin);
			}

			public HashedAvlTree<TKey, TValue2>.Bucket Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage) {
				var newBucket = HashedAvlTree<TKey, TValue2>.Bucket.Empty;
				for (var cur = this; !cur.IsEmpty; cur = cur.Next) newBucket = new HashedAvlTree<TKey, TValue2>.Bucket(cur.Key, selector(cur.Key, cur.Value), newBucket, Eq, lineage);
				return newBucket;
			}

			class BucketDebugView {
				readonly Bucket _inner;

				public BucketDebugView(Bucket bucket) {
					_inner = bucket;
					zContents = bucket.Buckets.Select(x => x.AsKvp).ToArray();
				}

				public int Count {
					get { return _inner.Count; }
				}

				[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
				public KeyValuePair<TKey, TValue>[] zContents { get; private set; }
			}
		}
	}
}