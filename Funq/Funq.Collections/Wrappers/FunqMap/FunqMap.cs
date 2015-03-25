using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Funq.Abstract;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	internal enum OverwriteBehavior {
		Overwrite,
		Ignore,
		Throw
	}
	public sealed partial class FunqMap<TKey, TValue> : Trait_MapLike<TKey, TValue, FunqMap<TKey, TValue>>
	{

		internal readonly HashedAvlTree<TKey, TValue>.Node Root;
		internal readonly IEqualityComparer<TKey> Equality; 

		public static FunqMap<TKey, TValue> Empty(IEqualityComparer<TKey> equality = null) {
			return new FunqMap<TKey, TValue>(HashedAvlTree<TKey, TValue>.Node.Null, equality ?? FastEquality<TKey>.Value);
		}

		internal FunqMap(HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality)
		{
			Root = root;
			Equality = equality;
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator() {
			var iterator = new HashedAvlTree<TKey, TValue>.TreeIterator(Root);
			while (iterator.MoveNext()) {
				var bucket = iterator.Current.Bucket;
				while (!bucket.IsNull) {
					yield return Kvp.Of(bucket.Key, bucket.Value);
					bucket = bucket.Next;
				}
			}
		}

		public Option<TValue> GetDirect(TKey k) {
			return Root.Root_Find(k);
		}

	
		public override Option<TValue> TryGet(TKey k)
		{
			return Root.Root_Find(k);
		}
		/// <summary>
		/// Adds a new key-value pair to the map.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if the specified key already exists in the map.</exception>
		/// <returns></returns>
		public FunqMap<TKey, TValue> Add(TKey key, TValue value)
		{
			return Set(key, value, OverwriteBehavior.Throw);
		}

		/// <summary>
		/// Adds the specified key-value pair to the map.
		/// </summary>
		/// <param name="pair">The key-value pair.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if the specified key already exists in the map.</exception>
		public FunqMap<TKey, TValue> Add(Kvp<TKey, TValue> pair)
		{
			return Add(pair.Key, pair.Value);
		}

		public FunqMap<TKey, TValue> Add(Tuple<TKey, TValue> pair)
		{
			return Add(pair.Item1, pair.Item2);
		}


		public FunqMap<TKey, TValue> AddRange(IEnumerable<Tuple<TKey, TValue>> tuples)
		{
			return this.AddRange(tuples.Select(x => (Kvp<TKey, TValue>)x));
		}

		internal FunqMap<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = Root;

			foreach (var item in items)
			{
				var newerRoot = newRoot.IsNull ? newRoot.InitializeFromNull(item.Key, item.Value, Equality, lineage) : newRoot.Root_Add(item.Key, item.Value, lineage, true);
				newRoot = newerRoot;
			}
			return newRoot.WrapMap(Equality);
		}

		internal FunqMap<TKey, TValue> AddRange(IEnumerable<TKey> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = Root;

			foreach (var item in items)
			{
				var newerRoot = newRoot.IsNull ? newRoot.InitializeFromNull(item, default(TValue), Equality, lineage) : newRoot.Root_Add(item, default(TValue), lineage, false);
				if (newerRoot != null) newRoot = newerRoot;
			}
			return newRoot.WrapMap(Equality);
		}

		internal FunqMap<TKey, TValue> Set(TKey k, TValue v, OverwriteBehavior behavior) {
			var r = Root.IsNull
				? Root.InitializeFromNull(k, v, Equality, Lineage.Mutable())
				: Root.Root_Add(k, v, Lineage.Mutable(), behavior == OverwriteBehavior.Overwrite);
			if (r == null && behavior == OverwriteBehavior.Throw) throw Errors.Key_exists;
			if (r == null) return this;
			return r.WrapMap(Equality);
		}

		/// <summary>
		/// Adds a new key-value pair, or sets the value of an existing key.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <param name="v">The value.</param>
		/// <returns></returns>
		public FunqMap<TKey, TValue> Set(TKey k, TValue v)
		{
			return Set(k, v, OverwriteBehavior.Overwrite);
		}

		/// <summary>
		/// Removes the specified key from the map.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <exception cref="KeyNotFoundException">Thrown if the specified key doesn't exist in the map.</exception>
		/// <returns></returns>
		public FunqMap<TKey, TValue> Drop(TKey k)
		{
			if (Root.IsNull) throw Errors.Is_empty;
			var removed = Root.Root_Remove(k, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(Equality);
		}


		public override FunqMap<TKey, TValue> Merge(FunqMap<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Union(other.Root, Lineage.Mutable(), collision).WrapMap(Equality);
		}
		public override FunqMap<TKey, TValue> Join(FunqMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Intersect(other.Root, collision, Lineage.Mutable()).WrapMap(Equality);
		}

		public override FunqMap<TKey, TValue> Except(FunqMap<TKey, TValue> other, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null )
		{
			if (other == null) throw Errors.Is_null;
			return Root.Except(other.Root, Lineage.Mutable(), subtraction).WrapMap(Equality);
		}

		public override FunqMap<TKey, TValue> Difference(FunqMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return Root.SymDifference(other.Root, Lineage.Mutable()).WrapMap(Equality);
		}


		/// <summary>
		/// Applies a selector on each value of the map. The result map uses the same key and membership semantics.
		/// </summary>
		/// <typeparam name="TRValue">The type of the return value.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <returns></returns>
		public FunqMap<TKey, TRValue> SelectValues<TRValue>(Func<TKey, TValue, TRValue> selector)
		{
			return Root.Apply(selector, Lineage.Mutable()).WrapMap(Equality);
		}

		internal FunqMap<TKey, TValue> AddRange(IEnumerable<Kvp<TKey, TValue>> items, OverwriteBehavior behavior)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = Root;

			foreach (var item in items)
			{
				var newerRoot = newRoot.IsNull ? newRoot.InitializeFromNull(item.Key, item.Value, Equality, lineage) : newRoot.Root_Add(item.Key, item.Value, lineage, behavior == OverwriteBehavior.Overwrite);

				if (newerRoot == null)
				{
					if (behavior == OverwriteBehavior.Throw) {
						throw Errors.Key_exists;
					}
				}
				else {
					newRoot = newerRoot;
				}
			}
			return newRoot.WrapMap(Equality);
		}

		/// <summary>
		/// Adds multiple key-value pairs to the map. May overwrite existing keys.
		/// </summary>
		/// <param name="items">The key-value pairs to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqMap<TKey, TValue> AddRange(IEnumerable<Kvp<TKey, TValue>> items) {
			return AddRange(items, OverwriteBehavior.Overwrite);
		}


		private FunqMap<TKey, TValue> AddRangeForHashCollisions(IEnumerable<Kvp<TKey, TValue>> items) {
			//This is an implementaiton I wrote, expecting heavy hash collisions. Turned out there were very few. It just reduces performance.
			var dictionary = new Dictionary<int, LinkedList<Kvp<TKey, TValue>>>();
			foreach (var item in items)
			{
				var hash = Equality.GetHashCode(item.Key);
				if (dictionary.ContainsKey(hash))
				{
					dictionary[hash].AddLast(item);
				}
				else
				{
					var list = new LinkedList<Kvp<TKey, TValue>>();
					list.AddLast(item);
					dictionary[hash] = list;
				}
			}
			var newRoot = Root;
			var lineage = Lineage.Mutable();
			foreach (var item in dictionary)
			{
				var hash = item.Key;
				var list = item.Value;
				newRoot = newRoot.IsNull ? newRoot.InitializeFromNullRange(hash, list, Equality, lineage) : newRoot.AddRange(hash, list, lineage);
			}
			return newRoot.WrapMap(Equality);
		}

		public FunqMap<TKey, TValue> DropRange(IEnumerable<TKey> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var root = Root;
			var removed = false;
			foreach (var item in items) {
				var newRoot = root.Root_Remove(item, lineage);
				if (newRoot != null) {
					if (newRoot.IsNull) return Empty(Equality);
					removed = true;
					root = newRoot;
				}
			}
			return removed ? root.WrapMap(Equality) : this;
		}

		


		/// <summary>
		/// Removes several keys from the map.
		/// </summary>
		/// <param name="keys">A collection of keys to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if a key doesn't exist in the map.</exception>
		private FunqMap<TKey, TValue> DropRangeForHashCollisions(IEnumerable<TKey> keys)
		{
			if (keys == null) throw Errors.Is_null;
			if (IsEmpty) return this;
			var lineage = Lineage.Mutable();
			

			var dictionary = new Dictionary<int, LinkedList<TKey>>();
			foreach (var item in keys) {
				var hash = Equality.GetHashCode(item);
				if (dictionary.ContainsKey(hash)) {
					dictionary[hash].AddLast(item);
				}
				else {
					var list = new LinkedList<TKey>();
					list.AddLast(item);
					dictionary[hash] = list;
				}
			}
			var newRoot = Root;
			bool removedAny = false;
			foreach (var item in dictionary) {
				var hash = item.Key;
				var list = item.Value;
				var tmpRoot = Root.DropRange(hash, list, lineage);
				if (tmpRoot != null) {
					newRoot = tmpRoot;
					removedAny = true;
				}
			}
			return removedAny ? newRoot.WrapMap(Equality) : this;
		}

		public override bool ForEachWhile(Func<Kvp<TKey, TValue>, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return Root.ForEachWhile((eqKey, v) => Kvp.Of(eqKey, v).Pipe(iterator));
		}

		public override int Length
		{
			get
			{
				return Root.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return Root.IsNull;
			}
		}

	}
}
