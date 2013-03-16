using System;
using System.Collections.Generic;
using Solid.Common;
using Solid.TrieMap;

namespace Solid
{
	public static class HashMap
	{
		/// <summary>
		/// Gets an empty HashMap.
		/// </summary>
		/// <param name="comparer">
		/// The equality comparer used by the instance to determine equality. 
		/// If not supplied or null the instance will use the key's default equality semantics.
		/// </param>
		/// <typeparam name="TKey">The type of the key./</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <returns></returns>
		/// <remarks>
		/// Empty hashmaps may or may not be reference equal.
		/// </remarks>
		public static HashMap<TKey, TValue> Empty<TKey, TValue>(IEqualityComparer<TKey> comparer = null)
		{
			return comparer == null ? HashMap<TKey, TValue>.Empty : HashMap<TKey, TValue>.WithComparer(comparer);
		}

		/// <summary>
		/// Creates a hash map from a single key-value pair.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="key">The key object.</param>
		/// <param name="value">The value. May be null. </param>
		/// <param name="comparer">The equality comparer used to determine equality by the instance. 
		/// If this parameter is not supplied or is null, the hash map will use the type's default equality semantics.</param>
		/// <returns></returns>
		public static HashMap<TKey, TValue> FromItem<TKey, TValue>(TKey key, TValue value, IEqualityComparer<TKey> comparer = null)
		{
			return HashMap<TKey, TValue>.Empty.Add(key, value);
		}
	}
	public sealed class HashMap<TKey,TValue>
	{
		private static InvalidOperationException Invalid_result_code
		{
			get { return new InvalidOperationException("The result code returned by the operation is invalid."); }
		}

		public readonly IEqualityComparer<TKey> Comparer;
		private readonly MapNode<TKey, TValue> root;
		internal HashMap(MapNode<TKey, TValue> root, IEqualityComparer<TKey> comparer)
		{
			this.root = root;
			Comparer = comparer;
		}

		private const int maxIter = 20;
		private HashMap<TKey, TValue> TrySet(HashedKey<TKey> key, TValue value, WriteBehavior behave, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			Result outcome;
			var output = root.TrySet(key, value, behave, out outcome);
			switch (outcome)
			{
				case Result.HashCollision:
					return TrySet(key.Rehash(), value, behave, iter + 1);
				case Result.KeyExists:
					throw Errors.Key_exists;
				case Result.KeyNotFound:
					throw Errors.Key_not_found;
				case Result.Success:
					return new HashMap<TKey, TValue>(output,Comparer);
				default:
					throw Invalid_result_code;
			}
		}

		public static HashMap<TKey,TValue> WithComparer(IEqualityComparer<TKey> comparer)
		{
			return new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(),comparer);
		}

		/// <summary>
		/// Gets an instance of the hash map with the default comparer for the type.
		/// </summary>
		public static readonly HashMap<TKey, TValue> Empty =
			new HashMap<TKey, TValue>(new MapEmpty<TKey, TValue>(),EqualityComparer<TKey>.Default);
		

		private TValue TryGet(HashedKey<TKey> key,int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			Result outcome;
			var output = root.TryGet(key, out outcome);
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

		private HashMap<TKey, TValue> TryDrop(HashedKey<TKey> key, int iter, bool canFail)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			Result outcome;
			var output = root.TryDrop(key, out outcome);
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
					return new HashMap<TKey, TValue>(output,Comparer);
				default:
					throw Invalid_result_code;
			}
		}

		private bool TryContains(HashedKey<TKey> key, int iter)
		{
			if (iter > maxIter)
			{
				throw Errors.Too_many_hash_collisions;
			}
			Result outcome;
			var output = root.TryContains(key, out outcome);
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

		/// <summary>
		/// Creates a new key-value pair in the hash map. If the key already exists throws an error.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when the key already exists.</exception>
		public HashMap<TKey,TValue> Add(TKey key,TValue value)
		{
			return TrySet(new HashedKey<TKey>(key,Comparer), value, WriteBehavior.OnlyCreate, 0);
		}

		/// <summary>
		/// Associates the specified key with a value. Can either overwrite or create a new key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public HashMap<TKey,TValue> Set(TKey key,TValue value)
		{
			return TrySet(new HashedKey<TKey>(key,Comparer), value, WriteBehavior.Any, 0);
		}

		/// <summary>
		/// Removes the specified key from the hash map. Throws an exception if the key is not found.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		public HashMap<TKey,TValue> Remove(TKey key)
		{
			return TryDrop(new HashedKey<TKey>(key,Comparer), 0, true);
		}

		/// <summary>
		/// Gets the value with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the supplied key is null.</exception>
		public TValue this[TKey key]
		{
			get { return TryGet(new HashedKey<TKey>(key,Comparer), 0); }
		}

		/// <summary>
		/// Gets if the item exists in the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
		public bool Contains(TKey key)
		{
			return TryContains(new HashedKey<TKey>(key,Comparer), 0);
		}


		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count
		{
			get
			{
				return root.Count;
			}
		}


		/// <summary>
		/// Applies a transformation on every value in the hash map.
		/// </summary>
		/// <param name="transform"></param>
		public HashMap<TKey,TValue2> Apply<TValue2>(Func<TKey,TValue,TValue2> transform)
		{
			return new HashMap<TKey, TValue2>(root.Apply(transform),Comparer);
		}

		/// <summary>
		/// Applies a function on each key-value pair in the hash map.
		/// </summary>
		/// <param name="action"></param>
		public void ForEach(Action<TKey, TValue> action)
		{
			root.Iter(action);
		}


	}
}
