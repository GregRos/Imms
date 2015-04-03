using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	public static class Ext {
		public static bool Eq<TKey>(this IEqualityComparer<TKey> eq, TKey a, TKey b) {
			return eq == null ? a.Equals(b) : eq.Equals(a, b);
		}

		public static int Hash<TKey>(this IEqualityComparer<TKey> eq, TKey a) {
			return eq == null ? a.GetHashCode() : eq.GetHashCode(a);
		}
	}

	static partial class HashedAvlTree<TKey, TValue>
	{
		/// <summary>
		/// An unordered linked list of elements used to store key-value pairs that have identical hashes. 
		/// </summary>
		[DebuggerTypeProxy(typeof(HashedAvlTree<,>.Bucket.BucketDebugView))]
		internal sealed class Bucket
		{
			private class BucketDebugView
			{
				private readonly Bucket _inner;
				public BucketDebugView(Bucket bucket)
				{
					_inner = bucket;
					zContents = bucket.Buckets.Select(x => x.AsKvp).ToArray();
				}

				public int Count
				{
					get
					{
						return _inner.Count;
					}
				}

				[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
				public KeyValuePair<TKey, TValue>[] zContents
				{
					get;
					set;
				}
			}

			public IEqualityComparer<TKey> Eq; 
			public TKey Key;
			public TValue Value;
			public Bucket Next = Empty;
			public readonly bool IsEmpty = false;
			private readonly Lineage Lineage;
			public int Count;
			internal static readonly Bucket Empty = new Bucket(true);

			public static Bucket FromRange(IEnumerable<KeyValuePair<TKey, TValue>> kvps, IEqualityComparer<TKey> eq, Lineage lin) {
				Bucket bucket = Empty;
				foreach (var kvp in kvps) {
					bucket = bucket.IsEmpty ? new Bucket(kvp.Key, kvp.Value, Empty, eq, lin) : bucket.Add(kvp.Key, kvp.Value, lin, true);
				}
				return bucket;
			}

			private Bucket(bool isEmpty)
			{
				IsEmpty = isEmpty;
				Lineage = Lineage.Immutable;
				Count = 0;
			}

			public Bucket()
			{
				
			}

			public KeyValuePair<TKey, TValue> AsKvp
			{
				get
				{
					return Kvp.Of(Key, Value);
				}
			}

			public Bucket(TKey k, TValue v, Bucket next, IEqualityComparer<TKey> eq, Lineage lin) {
				Eq = eq;
				this.Key = k;
				this.Value = v;
				this.Next = next;
				this.Lineage = lin;
				Count = next.Count + 1;
			}

			public Bucket NewBucket(TKey k, TValue v, Bucket next, Lineage lin) {
				return new Bucket(k, v, next, Eq, lin);
			}

			public Bucket WithNext(Bucket newNext, Lineage lineage)
			{
				if (Lineage.AllowMutation(lineage))
				{
					this.Next = newNext;
					Count = newNext.Count + 1;
					return this;
				}
				return new Bucket(this.Key, this.Value, newNext, Eq, lineage);
			}

			public Bucket NewMutates(TKey newKey, TValue newValue, Bucket newNext, Lineage lineage) {
				if (Lineage.AllowMutation(lineage))
				{
					this.Next = newNext;
					Key = newKey;
					Value = newValue;
					Count = newNext.Count + 1;
					return this;
				}
				return new Bucket(newKey, newValue, newNext, Eq, lineage);
			}

			public bool IsDisjoint(Bucket other) {
				var iter = other.Buckets.GetEnumerator();
				var areDisjoint = true;
				ForEachWhile((k, v) => {
					if (!iter.MoveNext()) return false;
					var cur = iter.Current.Key;
					if (Eq.Equals(cur, k)) {
						areDisjoint = false;
						return false;
					}
					return true;
				});
				return areDisjoint;

			}

			

			public Bucket WithKeyValue(TKey newKey, TValue newValue, Lineage lin)
			{
				if (Lineage.AllowMutation(lin))
				{
					this.Key = newKey;
					this.Value = newValue;
					return this;
				}
				return new Bucket(newKey, newValue, this.Next, Eq, lin);
			}


			public override string ToString()
			{
				return string.Join("; ", Buckets.Select(x => x.AsKvp.ToString()));
			}

			public Option<TValue> Find(TKey findKey)
			{
				for (var cur = this; !cur.IsEmpty; cur = cur.Next)
				{
					if (Eq.Equals(cur.Key, findKey))
					{
						return cur.Value;
					}
				}
				return Option.None;
			}

			public Bucket Remove(TKey findKey, Lineage lineage)
			{
				if (this.IsEmpty) return null;
				if (Eq.Equals(findKey, Key))
				{
					return this.Next;
				}
				var newNext = Next.Remove(findKey, lineage);

				return this.WithNext(newNext, lineage);
			}

			public Bucket TrySet(TKey findKey, TValue v, Lineage lin)
			{
				if (this.IsEmpty) return null;
				if (Eq.Equals(findKey, Key))
				{
					return WithKeyValue(findKey, v, lin);
				}
				var next = Next.TrySet(findKey, v, lin);
				return next == null ? null : WithNext(next, lin);
			}

			public Bucket Add(TKey key, TValue v, Lineage lin, bool overwrite) {
				if (this.IsEmpty) throw ImplErrors.Invalid_execution_path;
				if (Eq.Equals(key, Key)) {
					return overwrite ? this.WithKeyValue(key, v, lin) : null;
				}
				var newNext = Next.IsEmpty ? new Bucket(key, v, Empty, Eq, lin) : Next.Add(key, v, lin, overwrite);
				if (newNext == null) return null;
				return WithNext(newNext, lin);
			}

			public int CountIntersection(Bucket other)
			{
				var count = 0;
				foreach (var item in this.Items)
				{
					if (other.Find(item.Key).IsSome) count++;
				}
				return count;
			}

			public Bucket Except<TValue2>(HashedAvlTree<TKey, TValue2>.Bucket other, Lineage lineage, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null)
			{
				var newBucket = Empty;
				foreach (var item in this.Items)
				{
					var findOther = other.Find(item.Key);
					if (findOther.IsNone)
					{
						newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);
					}
					else if (subtraction != null)
					{
						var newValue = subtraction(item.Key, item.Value, findOther.Value);
						if (newValue.IsSome)
						{
							newBucket = NewBucket(item.Key, newValue.Value, newBucket, lineage);
						}
					}
				}
				return newBucket;
			}

			public Bucket Union(Bucket other, Func<TKey, TValue, TValue, TValue> collision, Lineage lineage) {
				var newBucket = Empty;
				foreach (var item in this.Items) {
					var findOther = other.Find(item.Key);
					var valueToAdd = item.Value;
					if (findOther.IsSome) {
						other = other.Remove(item.Key, lineage);
						valueToAdd = collision == null ? valueToAdd : collision(item.Key, item.Value, findOther.Value);
					}
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}

				foreach (var item in other.Items) {
					newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);
				}

				return newBucket;
			}

			public Bucket Intersect(Bucket other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision = null) {
				
				var newBucket = Empty;
				foreach (var item in this.Items)
				{
					var otherItem = other.Find(item.Key);
					if (otherItem.IsNone) continue;
					var valueToAdd = collision == null ? item.Value : collision(item.Key, item.Value, otherItem.Value);
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}
				return newBucket;
			}

			public bool ForEachWhile(Func<TKey,  TValue, bool> iterate)
			{ 
				for (var cur = this; !cur.IsEmpty; cur = cur.Next)
				{
					var res = iterate(cur.Key, cur.Value);
					if (!res) return false;
				}
				return true;
			}

			public IEnumerable<KeyValuePair<TKey,TValue>> Items
			{
				get
				{
					for (var cur = this; !cur.IsEmpty; cur = cur.Next)
					{
						yield return Kvp.Of(cur.Key, cur.Value);
					}
				}
			}


			public IEnumerable<Bucket> Buckets
			{
				get
				{
					for (var cur = this; !cur.IsEmpty; cur = cur.Next)
					{
						yield return cur;
					}
				}
			}


			public static Bucket FromKvp(TKey k, TValue v, IEqualityComparer<TKey> eq, Lineage lin)
			{
				return new Bucket(k, v, Empty, eq, lin);
			}

			public HashedAvlTree<TKey, TValue2>.Bucket Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				var newBucket = HashedAvlTree<TKey, TValue2>.Bucket.Empty;
				for (var cur = this; !cur.IsEmpty; cur = cur.Next)
				{
					newBucket = new HashedAvlTree<TKey, TValue2>.Bucket(cur.Key, selector(cur.Key, cur.Value), newBucket, Eq, lineage);
				}
				return newBucket;
			}
		}
	}
}
