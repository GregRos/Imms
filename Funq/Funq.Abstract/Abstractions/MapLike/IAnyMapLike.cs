using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Funq.Abstract
{
	

	/// <summary>
	/// Used for abstracting over map-like collection types when the concrete type is unknown. <br/>
	/// Should not be implemented from user code.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IAnyMapLike<TKey, TValue> : IAnyIterable<KeyValuePair<TKey, TValue>> {
		/// <summary>
		/// Returns the value matching the key, or None.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Option<TValue> TryGet(TKey key);
	}

	/// <summary>
	/// A map like collection that can also serve as a builder factory for that concrete type.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IAnyMapLikeWithBuilder<TKey, TValue> : IAnyMapLike<TKey, TValue>,
		IAnyBuilderFactory<KeyValuePair<TKey, TValue>, MapBuilder<TKey, TValue>> {
		
	}
}