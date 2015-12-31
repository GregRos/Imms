using System.Collections.Generic;

namespace Imms.Abstract {
	/// <summary>
	/// A collection builder for map-like collections.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public interface IAnyMapBuilder<TKey, TValue> : IAnyIterableBuilder<KeyValuePair<TKey, TValue>> {
		/// <summary>
		/// Returns the value associated with the specified key, or None if no such value exists.
		/// </summary>
		/// <param name="key">The key to get.</param>
		/// <returns></returns>
		Optional<TValue> TryGet(TKey key);

		Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key);

		/// <summary>
		/// Removes the specified key from the map, if it exists. Returns true if a key was removed, and false otherwise.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns></returns>
		bool Remove(TKey key);
	}
}