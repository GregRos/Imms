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
	public sealed partial class FunqMap<TKey, TValue> : AbstractMap<TKey, TValue, FunqMap<TKey, TValue>>
	{
		internal readonly HashedAvlTree<TKey, TValue>.Node Root;
		internal readonly IEqualityComparer<TKey> Equality; 

		public static FunqMap<TKey, TValue> Empty(IEqualityComparer<TKey> equality = null) {
			return new FunqMap<TKey, TValue>(HashedAvlTree<TKey, TValue>.Node.Empty, equality ?? FastEquality<TKey>.Default);
		}

		internal FunqMap(HashedAvlTree<TKey, TValue>.Node root, IEqualityComparer<TKey> equality)
		{
			Root = root;
			Equality = equality;
		}

		protected override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return Root.GetEnumerator();
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

		internal double CollisionMetric {
			get {
				return Root.CollisionMetric;
			}
		}

		internal FunqMap<TKey, TValue> Set(TKey k, TValue v, OverwriteBehavior behavior) {
			var r = Root.Root_Add(k, v, Lineage.Mutable(), Equality, behavior == OverwriteBehavior.Overwrite);
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
		public FunqMap<TKey, TValue> Remove(TKey k)
		{

			if (Root.IsEmpty) throw Errors.Is_empty;
			var removed = Root.Root_Remove(k, Lineage.Mutable());
			if (removed == null) return this;
			return removed.WrapMap(Equality);
		}

		protected override FunqMap<TKey, TValue> Merge(FunqMap<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> collision = null)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Union(other.Root, Lineage.Mutable(), collision).WrapMap(Equality);
		}

		protected override FunqMap<TKey, TValue> Join(FunqMap<TKey, TValue> other, System.Func<TKey, TValue, TValue, TValue> collision)
		{
			if (other == null) throw Errors.Is_null;
			return Root.Intersect(other.Root, collision, Lineage.Mutable()).WrapMap(Equality);
		}

		public override FunqMap<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) {
			var set = keys as FunqSet<TKey>;
			if (set != null && Equality.Equals(set.EqualityComparer)) {
				return Root.Except(set.Root, Lineage.Mutable()).WrapMap(Equality);
			}
			return base.RemoveRange(keys);
		}

		public override FunqMap<TKey, TValue> Except<TValue2>(IEnumerable<KeyValuePair<TKey, TValue2>> other, Func<TKey, TValue, TValue2, Option<TValue>> subtraction = null) {
			var map = other as FunqMap<TKey, TValue2>;
			if (map != null && Equality.Equals(map.Equality)) {
				return Root.Except(map.Root, Lineage.Mutable(), subtraction).WrapMap(Equality);
			}
			return base.Except(other, subtraction);
		}

		protected override FunqMap<TKey, TValue> Except(FunqMap<TKey, TValue> other, Func<TKey, TValue, TValue, Option<TValue>> subtraction = null ) {
			throw new Exception("Should never be called.");
		}

		protected override FunqMap<TKey, TValue> Difference(FunqMap<TKey, TValue> other)
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

		public FunqMap<TKey, TValue> Add(Tuple<TKey, TValue> pair)
		{
			return Add(pair.Item1, pair.Item2);
		}

		public override bool ForEachWhile(Func<KeyValuePair<TKey, TValue>, bool> iterator)
		{
			if (iterator == null) throw Errors.Is_null;
			return Root.ForEachWhile((eqKey, v) => iterator(Kvp.Of(eqKey, v)));
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
				return Root.IsEmpty;
			}
		}

	}
}
