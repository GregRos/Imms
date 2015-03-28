using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for abstracting over map builders without knowing the concrete type.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public interface IMapBuilder<TKey, TValue> : IIterableBuilder<KeyValuePair<TKey, TValue>>
	{
		/// <summary>
		/// Gets or sets the value corresponding to the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		TValue this[TKey key]
		{
			get;
			set;
		}

		/// <summary>
		/// Returns true if the key is contained in this builder.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(TKey key);
	}
}