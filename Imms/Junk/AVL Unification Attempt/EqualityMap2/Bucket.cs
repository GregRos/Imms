using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imm.Collections.Common;
using Imm.Collections.Implementation;

namespace Imm.Collections
{
	public partial class ImmMap2<TKey, TValue>
	{
		[DebuggerTypeProxy(typeof(ImmMap2<,>.Bucket.BucketDebugView))]
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

			public IEqualityComparer<TKey> Eq;
			public TKey Key;
			public TValue Value;
			public Bucket Next = Null;
			public readonly bool IsNull = false;
			private Lineage Lineage;
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
					return Kvp.Of(Key, Value);
				}
			}

			public Bucket(TKey k, TValue v, Bucket next, IEqualityComparer<TKey> eq, Lineage lin)
			{
				Eq = eq;
				this.Key = k;
				this.Value = v;
				this.Next = next;
				this.Lineage = lin;
				Count = next.Count + 1;
			}

			public Bucket NewBucket(TKey k, TValue v, Bucket next, Lineage lin)
			{
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
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					if (Eq.Eq(cur.Key, findKey))
					{
						return cur.Value;
					}
				}
				return Option.None;
			}

			public Bucket Remove(TKey findKey, Lineage lineage)
			{
				if (this.IsNull) return null;
				if (Eq.Eq(findKey, Key))
				{
					return this.Next;
				}
				var newNext = Next.Remove(findKey, lineage);
				return newNext == null ? null : this.WithNext(newNext, lineage);
			}

			public bool IsSupersetOf(Bucket other) {
				if (other.Count > Count) return false;
				foreach (var item in other.Buckets) {
					if (Find(item.Key).IsNone) {
						return false;
					}
				}
				return true;
			}

			public Bucket TrySet(TKey findKey, TValue v, Lineage lin) {
				if (this.IsNull) return null;
				if (Eq.Eq(findKey, Key)) {
					return WithKeyValue(findKey, v, lin);
				}
				var next = Next.TrySet(findKey, v, lin);
				return next == null ? null : WithNext(next, lin);
			}

			public Bucket Add(TKey key, TValue v, Lineage lin)
			{
				if (this.IsNull) throw ImplErrors.Invalid_execution_path;
				var trySet = TrySet(key, v, lin);
				if (trySet == null)
				{
					return new Bucket(key, v, this, Eq, lin);
				}
				else
				{
					return WithNext(trySet, lin);
				}
			}
			/*
			public Bucket Add(TKey key, TValue v, Lineage lin)
			{
				if (this.IsNull) throw ImplErrors.Invalid_execution_path;
				if (Eq.Eq(key, Key))
				{
					return this.WithKeyValue(key, v, lin);
				}
				var newNext = Next.IsNull ? new Bucket(key, v, Null, Eq, lin) : Next.Add(key, v, lin);
				return WithNext(newNext, lin);
			}
			*/
			
			
			public Bucket Difference(Bucket other, Lineage lineage)
			{
				var newBucket = Null;
				foreach (var item in this.Items)
				{
					if (other.Find(item.Key).IsNone)
					{
						newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);
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
						newBucket = NewBucket(item.Key, item.Value, newBucket, Lineage.Immutable);
					}
				}
				return newBucket;
			}

			public Bucket Union(Bucket other, Func<TKey, TValue, TValue, TValue> collision)
			{
				var newBucket = Null;
				var lineage = Lineage.Mutable();
				foreach (var item in this.Items)
				{
					var findOther = other.Find(item.Key);
					var valueToAdd = item.Value;
					if (findOther.IsSome)
					{
						other = other.Remove(item.Key, lineage);
						valueToAdd = collision == null ? valueToAdd : collision(item.Key, item.Value, findOther.Value);
					}
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}

				foreach (var item in other.Items)
				{
					newBucket = NewBucket(item.Key, item.Value, newBucket, lineage);
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
					var valueToAdd = collision == null ? item.Value : collision(item.Key, item.Value, otherItem.Value);
					newBucket = NewBucket(item.Key, valueToAdd, newBucket, lineage);
				}
				return newBucket;
			}

			public bool ForEachWhile(Func<TKey, TValue, bool> iterate)
			{
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					var res = iterate(cur.Key, cur.Value);
					if (!res) return false;
				}
				return true;
			}

			public IEnumerable<Kvp<TKey, TValue>> Items
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


			public static Bucket FromKvp(TKey k, TValue v, IEqualityComparer<TKey> eq, Lineage lin)
			{
				return new Bucket(k, v, Null, eq, lin);
			}

			public ImmMap2<TKey, TValue2>.Bucket Apply<TValue2>(Func<TKey, TValue, TValue2> selector, Lineage lineage)
			{
				var newBucket = ImmMap2<TKey, TValue2>.Bucket.Null;
				for (var cur = this; !cur.IsNull; cur = cur.Next)
				{
					newBucket = new ImmMap2<TKey, TValue2>.Bucket(cur.Key, selector(cur.Key, cur.Value), newBucket, Eq, lineage);
				}
				return newBucket;
			}
		}
	}
}
