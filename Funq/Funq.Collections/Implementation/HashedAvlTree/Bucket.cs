using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{

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
				private Bucket _inner;
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
				public Kvp<TKey, TValue>[] zContents
				{
					get;
					set;
				}
			}

			public EquatableKey<TKey> Key;
			public TValue Value;
			public Bucket Next = Null;
			public readonly bool IsNull = false;
			private  Lineage Lineage;
			public int Count;
			internal static readonly Bucket Null = new Bucket(true);

			private Bucket(bool isNull)
			{
				IsNull = isNull;
				Lineage = Lineage.Immutable;
				Count = 0;
			}

			public Bucket()
			{
				
			}

			public Kvp<TKey, TValue> AsKvp
			{
				get
				{
					return Kvp.Of(Key.Key, Value);
				}
			}

			public Bucket(EquatableKey<TKey> k, TValue v, Bucket next, Lineage lin)
			{
				this.Key = k;
				this.Value = v;
				this.Next = next;
				this.Lineage = lin;
				Count = next.Count + 1;
			}
			public Bucket WithNext(Bucket newNext, Lineage lineage)
			{
				if (Lineage.AllowMutation(lineage))
				{
					this.Next = newNext;
					Count = newNext.Count + 1;
					return this;
				}
				return new Bucket(this.Key, this.Value, newNext, lineage);
			}

			public Bucket WithKeyValue(EquatableKey<TKey> newKey, TValue newValue, Lineage lin)
			{
				if (Lineage.AllowMutation(lin))
				{
					this.Key = newKey;
					this.Value = newValue;
					return this;
				}
				return new Bucket(newKey, newValue, this.Next, lin);
			}


			public override string ToString()
			{
				return string.Join("; ", Buckets.Select(x => x.AsKvp.ToString()));
			}

			

			public Option<TValue> Find(EquatableKey<TKey> findKey)
			{
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					if (findKey == cur.Key)
					{
						return cur.Value;
					}
				}
				return Option.None;
			}

			public Bucket Remove(EquatableKey<TKey> findKey, Lineage lineage)
			{
				if (this.IsNull) return null;
				if (findKey == Key)
				{
					return this.Next;
				}
				var newNext = Next.Remove(findKey, lineage);

				return this.WithNext(newNext, lineage);
			}

			public Bucket Add(EquatableKey<TKey> key, TValue v, Lineage lin)
			{
				if (this.IsNull) return new Bucket(key, v, Null, lin);
				if (key == Key)
				{
					return this.WithKeyValue(key, v, lin);
				}
				var newNext = Next.Add(key, v, lin);
				return WithNext(newNext, lin);
			}

			public Bucket Difference(Bucket other, Lineage lineage)
			{
				var newBucket = Null;
				foreach (var item in this.Items)
				{
					if (other.Find(item.Key).IsNone)
					{
						newBucket = newBucket.Add(item.Key, item.Value, lineage);
					}
				}
				return newBucket;
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

			public Bucket Except(Bucket other)
			{
				var newBucket = Null;
				foreach (var item in this.Items)
				{
					var findOther = other.Find(item.Key);
					if (findOther.IsNone)
					{
						newBucket = newBucket.Add(item.Key, item.Value, Lineage.Immutable);
					}
				}
				return newBucket;
			}

			public Bucket Union(Bucket other, Func<TKey, TValue, TValue, TValue> collision)
			{
				var newBucket = this;
				foreach (var item in this.Items)
				{
					var findOther = other.Find(item.Key);
					var valueToAdd = findOther.IsNone || collision == null ? item.Value : collision(item.Key.Key, item.Value, findOther.Value);
					newBucket = newBucket.Add(item.Key, valueToAdd, Lineage.Immutable);
				}
				return newBucket;
			}

			public Bucket Intersect(Bucket other, Lineage lineage, Func<TKey, TValue, TValue, TValue> collision = null)
			{
				var newBucket = Null;
				foreach (var item in this.Items)
				{
					var otherItem = other.Find(item.Key);
					if (otherItem.IsNone) continue;
					var valueToAdd = collision == null ? item.Value : collision(item.Key.Key, item.Value, otherItem.Value);
					newBucket = newBucket.Add(item.Key, valueToAdd, lineage);
				}
				return newBucket;
			}

			public bool ForEachWhile(Func<EquatableKey<TKey>,  TValue, bool> iterate)
			{
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					var res = iterate(cur.Key, cur.Value);
					if (!res) return false;
				}
				return true;
			}

			public IEnumerable<Kvp<EquatableKey<TKey>,TValue>> Items
			{
				get
				{
					for (var cur = this; !cur.IsNull; cur = cur.Next)
					{
						yield return Kvp.Of(cur.Key, cur.Value);
					}
				}
			}


			public IEnumerable<Bucket> Buckets
			{
				get
				{
					for (var cur = this; !cur.IsNull; cur = cur.Next)
					{
						yield return cur;
					}
				}
			}


			public static Bucket FromKvp(EquatableKey<TKey> k, TValue v, Lineage lin)
			{
				return new Bucket(k, v, Null, lin);
			}

			public HashedAvlTree<TKey, TValue2>.Bucket Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				var newBucket = HashedAvlTree<TKey, TValue2>.Bucket.Null;
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					newBucket = newBucket.Add(cur.Key, selector(cur.Key.Key, cur.Value), lineage);
				}
				return newBucket;
			}
		}
	}
}
