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
	public partial class FunqMap<TKey, TValue> : Trait_KeyValueMap<TKey, TValue, FunqMap<TKey, TValue>>
	{
		private readonly HashedAvlTree<TKey, TValue>.Node _root;
		private readonly IEqualityComparer<TKey> _equality; 

		public static FunqMap<TKey, TValue> Empty(IEqualityComparer<TKey> equality)
		{
			return new FunqMap<TKey, TValue>(HashedAvlTree<TKey, TValue>.Node.Null, equality ?? EqualityComparer<TKey>.Default);
		}

		internal FunqMap(HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality)
		{
			_root = root;
			_equality = equality;
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
		{
			foreach (var bucket in _root.Buckets)
				foreach (var item in bucket.Items)
				{
					yield return Kvp.Of(item.Key.Key, item.Value);
				}
		}


		public override Option<TValue> TryGet(TKey k)
		{
			return _root.Find(_equality.WrapKey(k));
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
			if (this.ContainsKey(key)) throw Funq.Errors.Key_exists;
			return _root.AvlAdd(_equality.WrapKey(key), value, Lineage.Mutable()).WrapMap(_equality);
		}

		/// <summary>
		/// Adds the specified key-value pair to the map.
		/// </summary>
		/// <param name="pair">The key-value pair.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if the specified key already exists in the map.</exception>
		public FunqMap<TKey, TValue> Add(Kvp<TKey, TValue> pair)
		{
			if (this.ContainsKey(pair.Key)) throw Funq.Errors.Key_exists;
			return _root.AvlAdd(_equality.WrapKey(pair.Key), pair.Value, Lineage.Mutable()).WrapMap(_equality);
		}

		/// <summary>
		/// Adds a new key-value pair, or sets the value of an existing key.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <param name="v">The value.</param>
		/// <returns></returns>
		public FunqMap<TKey, TValue> Set(TKey k, TValue v)
		{
			return _root.AvlAdd(_equality.WrapKey(k), v, Lineage.Mutable()).WrapMap(_equality);
		}

		/// <summary>
		/// Removes the specified key from the map.
		/// </summary>
		/// <param name="k">The key.</param>
		/// <exception cref="KeyNotFoundException">Thrown if the specified key doesn't exist in the map.</exception>
		/// <returns></returns>
		public FunqMap<TKey, TValue> Drop(TKey k)
		{
			if (_root.IsNull) throw Errors.Is_empty;
			if (!this.ContainsKey(k)) throw Errors.Key_not_found;
			var removed = _root.AvlRemove(_equality.WrapKey(k), Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(_equality);
		}


		public override FunqMap<TKey, TValue> Merge(FunqMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null || collision == null) throw Errors.Is_null;
				
			return _root.Union(other._root, collision).WrapMap(_equality);
		}

		public FunqMap<TKey, TValue> Add(Tuple<TKey, TValue> tpl) {
			return this.Add(tpl.Item1, tpl.Item2);
		}

		public FunqMap<TKey, TValue> AddMany(IEnumerable<Tuple<TKey, TValue>> tpls) {
			return this.AddMany(tpls.Select(x => Kvp.Of(x.Item1, x.Item2)));
		}

		public override FunqMap<TKey, TValue> Join(FunqMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null || collision == null) throw Errors.Is_null;
			return _root.Intersect(other._root, collision, Lineage.Immutable).WrapMap(_equality);
		}

		public override FunqMap<TKey, TValue> Except(FunqMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.Except(other._root).WrapMap(_equality);
		}

		public override FunqMap<TKey, TValue> Difference(FunqMap<TKey, TValue> other)
		{
			if (other == null) throw Errors.Is_null;
			return _root.SymDifference(other._root).WrapMap(_equality);
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
			return _root.Apply(selector, Lineage.Immutable).WrapMap(_equality);
		}


		/// <summary>
		/// Adds multiple key-value pairs to the map. May overwrite existing keys.
		/// </summary>
		/// <param name="items">The key-value pairs to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FunqMap<TKey, TValue> AddMany(IEnumerable<Kvp<TKey, TValue>> items)
		{
			if (items == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in items)
			{
				newRoot = newRoot.AvlAdd(_equality.WrapKey(item.Key), item.Value, lineage);
			}
			return newRoot.WrapMap(_equality);
		}

		/// <summary>
		/// Removes several keys from the map.
		/// </summary>
		/// <param name="keys">A collection of keys to remove.</param>
		/// <exception cref="KeyNotFoundException">Thrown if a key doesn't exist in the map.</exception>
		public FunqMap<TKey, TValue> DropMany(IEnumerable<TKey> keys)
		{
			if (keys == null) throw Errors.Is_null;
			var lineage = Lineage.Mutable();
			var newRoot = _root;
			foreach (var item in keys)
			{
				newRoot = newRoot.AvlRemove(_equality.WrapKey(item), lineage) ?? newRoot;
			}
			return newRoot.WrapMap(_equality);
		}

		public override bool ForEachWhile(Func<Kvp<TKey, TValue>, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return _root.ForEachWhile((eqKey, v) => Kvp.Of(eqKey.Key, v).Pipe(iterator));
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

	}
}
