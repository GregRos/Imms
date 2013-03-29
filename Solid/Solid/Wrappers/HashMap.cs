using System;
using System.Collections.Generic;
using Solid.Common;
using Solid.TrieMap;

namespace Solid
{
	/// <summary>
	/// A key-value map that uses equality semantics
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public sealed class HashMap<TKey, TValue>
	{
		private const int maxIter = 20;

		public readonly IEqualityComparer<TKey> Comparer;

		/// <summary>
		///   Gets an instance of the hash map with the default comparer for the type.
		/// </summary>
		public static readonly HashMap<TKey, TValue> Empty =
			new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(), EqualityComparer<TKey>.Default);

		private readonly MapNode<TKey, TValue> root;

		internal HashMap(MapNode<TKey, TValue> root, IEqualityComparer<TKey> comparer)
		{
			this.root = root;
			Comparer = comparer;
		}

		/// <summary>
		///   Gets the value with the specified key.
		/// </summary>
		/// <param name="key">The key to get.</param>
		/// <returns> </returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		public TValue this[TKey key]
		{
			get
			{
				return TryGet(new HashedKey<TKey>(key, Comparer), 0);
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
		///   Creates a new key-value pair in the hash map. If the key already exists throws an error.
		/// </summary>
		/// <param name="key"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentException">Thrown when the key already exists.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public HashMap<TKey, TValue> Add(TKey key, TValue value)
		{
			return TrySet(new HashedKey<TKey>(key, Comparer), value, WriteBehavior.OnlyCreate, 0);
		}


		/// <summary>
		/// Applies a transformation on every value in the hash map.
		/// </summary>
		/// <typeparam name="TValue2">The type of values stored in the new hashmap.</typeparam>
		/// <param name="transform">The transformation applied to every value.</param>
		/// <returns></returns>
		public HashMap<TKey, TValue2> Select<TValue2>(Func<TKey, TValue, TValue2> transform)
		{
			if (transform == null) throw Errors.Argument_null("transform");
			return new HashMap<TKey, TValue2>(root.Apply(transform), Comparer);
		}

		/// <summary>
		/// Returns true if the specified key exists in the collection.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public bool Contains(TKey key)
		{
			if (key == null) throw Errors.Argument_null("key");
			return TryContains(new HashedKey<TKey>(key, Comparer), 0);
		}

		/// <summary>
		/// Iterates over every key-value pair in the hash map.
		/// </summary>
		/// <param name="action">The function used to iterate over the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public void ForEach(Action<TKey, TValue> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			root.Iter(action);
		}

		/// <summary>
		///   Removes the specified key from the hash map. Throws an exception if the key is not found.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns> </returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		public HashMap<TKey, TValue> Remove(TKey key)
		{
			if (key == null) throw Errors.Argument_null("key");
			return TryDrop(new HashedKey<TKey>(key, Comparer), 0, true);
		}

		/// <summary>
		///   Associates the specified key with a value. Can either overwrite or create a new key.
		/// </summary>
		/// <param name="key">The key to update.</param>
		/// <param name="value">The new value.</param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
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
			Result outcome;
			bool output = root.TryContains(key, out outcome);
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
			Result outcome;
			MapNode<TKey, TValue> output = root.TryDrop(key, out outcome);
			switch (outcome)
			{
				case Result.KeyNotFound:
					if (canFail)
						throw Errors.Key_not_found;
					else
						return this;
				case Result.TurnedEmpty:
					return Empty;
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
			Result outcome;
			TValue output = root.TryGet(key, out outcome);
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

		private HashMap<TKey, TValue> TrySet(HashedKey<TKey> key, TValue value, WriteBehavior behave, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			Result outcome;
			MapNode<TKey, TValue> output = root.TrySet(key, value, behave, out outcome);
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

		private static InvalidOperationException Invalid_result_code
		{
			get
			{
				return new InvalidOperationException("The result code returned by the operation is invalid.");
			}
		}

		public static HashMap<TKey, TValue> WithComparer(IEqualityComparer<TKey> comparer)
		{
			return new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(), comparer);
		}
	}
}