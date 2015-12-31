using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imm.Collections.Common;
using Imm.Collections.Implementation;
using Imm.Abstract;
namespace Imm.Collections
{

	internal static class Exter {
		public static ImmMap2<TKey, TValue> WrapMap2<TKey, TValue>(this IEqualityComparer<TKey> eq,OrderedAvlTree<int, ImmMap2<TKey,TValue>.Bucket>.Node root, int length) {
			return new ImmMap2<TKey, TValue>(root, eq, length);
		}

		public static ImmMap2<TKey, TValue> WrapMap2<TKey, TValue>(this OrderedAvlTree<int, ImmMap2<TKey, TValue>.Bucket>.Node root, IEqualityComparer<TKey> eq, int length)
		{
			return new ImmMap2<TKey, TValue>(root, eq, length);
		}
	}

	public partial class ImmMap2<TKey, TValue> : Trait_MapLike<TKey, TValue, ImmMap2<TKey, TValue>>{

		internal readonly OrderedAvlTree<int, Bucket>.Node _root;
		internal readonly IEqualityComparer<TKey> _eq;
		private readonly int _length;
		public static ImmMap2<TKey, TValue> Empty(IEqualityComparer<TKey> eq) {
			return new ImmMap2<TKey, TValue>(OrderedAvlTree<int, Bucket>.Node.Null, eq, 0);
		}
		internal ImmMap2(OrderedAvlTree<int, Bucket>.Node root, IEqualityComparer<TKey> eq, int length) {
			_root = root;
			_eq = eq ?? EqualityComparer<TKey>.Default;
			_length = length;
		}

		public ImmMap2<TKey, TValue> Set(TKey key, TValue value) {
			var hash = _eq.GetHashCode(key);
			var bucket = _root.Find(hash);
			var lineage = Lineage.Mutable();
			var ret = this;
			var oldCount = bucket.IsNone ? 0 : bucket.Value.Count;
			Bucket newBucket = bucket.IsNone ? Bucket.FromKvp(key, value, _eq, lineage) : bucket.Value.Add(key, value, lineage);
			var newRoot = _root.AvlAdd(hash, newBucket, lineage, true);
			return _eq.WrapMap2(newRoot, _length + newBucket.Count - oldCount);
		}

		public ImmMap2<TKey, TValue> Drop(TKey key) {
			var hash = _eq.GetHashCode(key);
			var bucket = _root.Find(hash);
			if (bucket.IsNone) return this;
			var lineage = Lineage.Mutable();
			var newBucket = bucket.Value.Remove(key, lineage);
			if (newBucket == null) return this;
			var newRoot = newBucket.IsNull ? _root.AvlRemove(hash, lineage) : _root.AvlAdd(hash, newBucket, lineage, true);
			return _eq.WrapMap2(newRoot, _length - 1);
		}

		public ImmMap2<TKey, TValue> Add(TKey key, TValue value) {
			if (ContainsKey(key)) throw Errors.Key_exists;
			return Set(key, value);
		}

		public ImmMap2<TKey, TValue> AddMany(IEnumerable<Kvp<TKey, TValue>> kvps) {
			var lineage = Lineage.Mutable();
			int added = 0;
			var dictionary = new Dictionary<int, List<Kvp<TKey, TValue>>>();
			var newNode = _root;
			

			foreach (var kvp in kvps) {
				var hash = kvp.Key.GetHashCode();
				var bucket = _root.Find(hash);
				int oldCount;
				Bucket newBucket;
				if (bucket.IsNone) {
					oldCount = 0;
					newBucket = Bucket.FromKvp(kvp.Key, kvp.Value, _eq, lineage);
					newNode = newNode.AvlAdd(hash, newBucket, lineage, true);
				}
				else {
					oldCount = bucket.Value.Count;
					newBucket = bucket.Value.Add(kvp.Key, kvp.Value, lineage);
					newNode = newNode.AvlAdd(hash, newBucket, lineage, true);
				}
				added += newBucket.Count - oldCount;
			}
			return _eq.WrapMap2(newNode, Length + added);
		}

		public ImmMap2<TKey, TValue> AddMany(IEnumerable<KeyValuePair<TKey, TValue>> kvps)
		{
			var lineage = Lineage.Mutable();
			int added = 0;
			var dictionary = new Dictionary<int, List<Kvp<TKey, TValue>>>();
			var newNode = _root;


			foreach (var kvp in kvps)
			{
				var hash = kvp.Key.GetHashCode();
				var bucket = _root.Find(hash);
				int oldCount;
				Bucket newBucket;
				if (bucket.IsNone)
				{
					oldCount = 0;
					newBucket = Bucket.FromKvp(kvp.Key, kvp.Value, _eq, lineage);
					newNode = newNode.AvlAdd(hash, newBucket, lineage, true);
				}
				else
				{
					oldCount = bucket.Value.Count;
					newBucket = bucket.Value.Add(kvp.Key, kvp.Value, lineage);
					newNode = newNode.AvlAdd(hash, newBucket, lineage, true);
				}
				added += newBucket.Count - oldCount;
			}
			return _eq.WrapMap2(newNode, Length + added);
		}

		public ImmMap2<TKey, TValue> DropMany(IEnumerable<TKey> keys) {
			var lineage = Lineage.Mutable();
			int removed = 0;
			var newNode = _root;
			foreach (var key in keys)
			{
				var hash = key.GetHashCode();
				var bucket = _root.Find(hash);
				int oldCount;
				if (!bucket.IsSome) continue;
				Bucket newBucket = bucket.Value.Remove(key, lineage);
				if (newBucket != null) {
					newNode = newBucket.IsNull ? newNode.AvlRemove(hash, lineage) : newNode.AvlAdd(hash, newBucket, lineage, true);
					removed += 1;
				}
			}
			return _eq.WrapMap2(newNode, Length - removed);
		}

		public override Option<TValue> TryGet(TKey k) {
			var bucket = _root.Find(_eq.GetHashCode(k));
			if (bucket.IsNone) {
				return Option.None;
			}
			var ret = bucket.Value.Find(k);
			return ret;
		}

		public override int Length
		{
			get
			{
				return _root.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return _root.IsNull;
			}
		}

		public override ImmMap2<TKey, TValue> Except(ImmMap2<TKey, TValue> other) {
			var count = Length;
			return _root.Except(other._root, (hash, my, yours) => {
				count -= my.Count;
				var result = my.Except(yours);
				count += result.Count;
				return result;
			}).WrapMap2(_eq, count);
		}

		public override ImmMap2<TKey, TValue> Join(ImmMap2<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> collision) {
			var lineage = Lineage.Mutable();
			var total = 0;
			var ret = _root.Intersect(other._root, (hash, my, yours) => {
				var result = my.Intersect(yours, lineage, collision);
				total += result.Count;
				return result;
			}, Lineage.Mutable());
			return ret.WrapMap2(_eq, total);
		}

		public override ImmMap2<TKey, TValue> Merge(ImmMap2<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> collision = null) {
			var total = other._length + _length;
			var lineage = Lineage.Mutable();
			var ret = _root.Union(other._root, (hash, my, yours) => {
				total -= my.Count + yours.Count;
				var result = my.Union(yours, collision);
				total += result.Count;
				return result;
			});
			return ret.WrapMap2(_eq, total);
		}

		internal bool IsSupersetOf(ImmMap2<TKey, TValue> other) {
			return _root.IsSupersetOf(other._root, (hash, my, yours) => my.IsSupersetOf(yours));
		}

		public override bool ForEachWhile(Func<Kvp<TKey, TValue>, bool> iterator) {
			return _root.ForEachWhile((i, bucket) => bucket.ForEachWhile((k, v) => iterator(Kvp.Of(k, v))));
		}

		public override ImmMap2<TKey, TValue> Difference(ImmMap2<TKey, TValue> other) {
			var meExceptYou = this.Except(other);
			var youExceptMe = other.Except(this);
			return meExceptYou.Merge(youExceptMe);
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator() {
			return _root.Items.SelectMany(x => x.Value.Buckets).Select(x => Kvp.Of(x.Key, x.Value)).GetEnumerator();
		}
	}
}
