using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Solid.Common;
using Solid.TrieMap;

namespace Solid
{
	/// <summary>
	///   A key-value map that uses equality semantics.
	/// </summary>
	/// <typeparam name="TKey"> The type of the key. </typeparam>
	/// <typeparam name="TValue"> The type of the value. </typeparam>
	internal class HashMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private const int maxIter = 10;

		/// <summary>
		///   Gets an instance of the hash map with the default comparer for the type.
		/// </summary>
		internal static readonly HashMap<TKey, TValue> empty =
			new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(), EqualityComparer<TKey>.Default);

		/// <summary>
		///   Returns the equality comparer used by this instance.
		/// </summary>
		private readonly IEqualityComparer<TKey> _comparer;

		private readonly MapNode<TKey, TValue> root;

		internal HashMap(MapNode<TKey, TValue> root, IEqualityComparer<TKey> comparer)
		{
			this.root = root;
			_comparer = comparer;
		}

		/// <summary>
		///   Returns an empty hash map using the default equality comparer for the type.
		/// </summary>
		public static HashMap<TKey, TValue> Empty
		{
			get
			{
				return empty;
			}
		}

		private static InvalidOperationException Invalid_result_code
		{
			get
			{
				return new InvalidOperationException("The result code returned by the operation is invalid.");
			}
		}

		/// <summary>
		///   Returns an empty hash map utilizing the specified equality comparer.
		/// </summary>
		/// <param name="comparer"> </param>
		/// <returns> </returns>
		public static HashMap<TKey, TValue> WithComparer(IEqualityComparer<TKey> comparer)
		{
			return new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(), comparer);
		}

		/// <summary>
		///   Gets the value with the specified key.
		/// </summary>
		/// <param name="key"> The key to get. </param>
		/// <returns> </returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public TValue this[TKey key]
		{
			get
			{
				return TryGet(new HashedKey<TKey>(key, Comparer), 0);
			}
		}

		/// <summary>
		///   Returns the equality comparer used by this instance.
		/// </summary>
		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return _comparer;
			}
		}

		/// <summary>
		///   Gets the number of items in the collection.
		/// </summary>
		public int Count
		{
			get
			{
				return root.Count;
			}
		}

		/// <summary>
		///   Gets the keys contained by the hash map.
		/// </summary>
		public IEnumerable<TKey> Keys
		{
			get
			{
				foreach (var kvp in root)
				{
					yield return kvp.Key;
				}
			}
		}

		/// <summary>
		///   Gets the values stored in the hash map.
		/// </summary>
		public IEnumerable<TValue> Values
		{
			get
			{
				foreach (var kvp in root)
				{
					yield return kvp.Value;
				}
			}
		}

		/// <summary>
		///   Adds a new key-value pair to the hash map. If the key already exists throws an error.
		/// </summary>
		/// <param name="key"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentException">Thrown when the key already exists.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public HashMap<TKey, TValue> Add(TKey key, TValue value)
		{
			return TrySet(new HashedKey<TKey>(key, Comparer), value, WriteBehavior.OnlyCreate, 0);
		}

		/// <summary>
		///   Returns true if the specified key exists in the collection.
		/// </summary>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public bool ContainsKey(TKey key)
		{
			if (key == null) throw Errors.Argument_null("key");
			return TryContains(new HashedKey<TKey>(key, Comparer), 0);
		}

		/// <summary>
		///   Iterates over every key-value pair in the hash map.
		/// </summary>
		/// <param name="action"> The function used to iterate over the collection. </param>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public void ForEach(Action<TKey, TValue> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			root.Iter(action);
		}

		/// <summary>
		///   Returns an getEnumerator for iterating over this collection.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return root.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Removes the specified key from the hash map. Throws an exception if the key is not found.
		/// </summary>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public HashMap<TKey, TValue> Remove(TKey key)
		{
			if (key == null) throw Errors.Argument_null("key");
			return TryDrop(new HashedKey<TKey>(key, Comparer), 0, true);
		}

		/// <summary>
		///   Projects every value using the specified selector. Does not project the key.
		/// </summary>
		/// <typeparam name="TValue2"> The return type of the projection. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		public HashMap<TKey, TValue2> Select<TValue2>(Func<TKey, TValue, TValue2> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			return new HashMap<TKey, TValue2>(root.Apply(selector), Comparer);
		}

		/// <summary>
		///   Associates the specified key with a value. Can either overwrite or create a new key.
		/// </summary>
		/// <param name="key"> The key to update. </param>
		/// <param name="value"> The new value. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public HashMap<TKey, TValue> Set(TKey key, TValue value)
		{
			return TrySet(new HashedKey<TKey>(key, Comparer), value, WriteBehavior.Any, 0);
		}

		private bool TryContains(HashedKey<TKey> key, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			var outcome = Result.Unassigned;
			var output = root.TryContains(key, ref outcome);
			switch (outcome)
			{
				case Result.HashCollision:
					return TryContains(key.Rehash(), iter + 1);
				case Result.Success:
					return output;
				default:
					throw Invalid_result_code;
			}
		}

		private HashMap<TKey, TValue> TryDrop(HashedKey<TKey> key, int iter, bool canFail)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			var outcome = Result.Unassigned;
			;
			var output = root.TryDrop(key, ref outcome);
			switch (outcome)
			{
				case Result.KeyNotFound:
					if (canFail)
						throw Errors.Key_not_found;
					else
						return this;
				case Result.HashCollision:
					return TryDrop(key.Rehash(), iter + 1, canFail);
				case Result.TurnedEmpty:
					return empty;
				case Result.Success:
					return new HashMap<TKey, TValue>(output, Comparer);
				default:
					throw Invalid_result_code;
			}
		}

		private TValue TryGet(HashedKey<TKey> key, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			var outcome = Result.Unassigned;
			;
			var output = root.TryGet(key, ref outcome);
			switch (outcome)
			{
				case Result.KeyNotFound:
					throw Errors.Key_not_found;
				case Result.HashCollision:
					return TryGet(key.Rehash(), iter + 1);
				case Result.Success:
					return output;
				default:
					throw Invalid_result_code;
			}
		}

		/// <summary>
		///   Tries to get the value associated with the specified key.
		/// </summary>
		/// <param name="key"> The key. </param>
		/// <param name="v"> An output parameter that returns the value, if it was found. </param>
		/// <returns> Returns whether or not the key exists in the collection. </returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if there is an exceptional amount of hash collisions.</exception>
		public bool TryGetValue(TKey key, out TValue v)
		{
			var res = ContainsKey(key);
			v = res ? this[key] : default(TValue);
			return res;
		}

		private HashMap<TKey, TValue> TrySet(HashedKey<TKey> key, TValue value, WriteBehavior behave, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			var outcome = Result.Unassigned;
			;
			var output = root.TrySet(key, value, behave, ref outcome);
			switch (outcome)
			{
				case Result.HashCollision:
					return TrySet(key.Rehash(), value, behave, iter + 1);
				case Result.KeyExists:
					throw Errors.Key_exists;
				case Result.KeyNotFound:
					throw Errors.Key_not_found;
				case Result.Success:
					return new HashMap<TKey, TValue>(output, Comparer);
				default:
					throw Invalid_result_code;
			}
		}
	}
}