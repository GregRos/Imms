using System.Collections.Generic;

namespace Imms.Abstract {

	/// <summary>
	/// Abstracts over map builders of any kind.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TMap">The type of the map built by this builder.</typeparam>
	public interface IMapBuilder<TKey, TValue, out TMap> : IIterableBuilder<KeyValuePair<TKey, TValue>, TMap>, IAnyMapBuilder<TKey, TValue>
	{

	}

}