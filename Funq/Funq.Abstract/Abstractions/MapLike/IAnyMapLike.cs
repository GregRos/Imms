using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for abstracting over map-like collection types when the concrete type is unknown. <br/>
	/// Should not be implemented from user code.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IAnyMapLike<TKey, TValue> : IAnyBuilderFactory<KeyValuePair<TKey, TValue>, MapBuilder<TKey,TValue>>, IReadOnlyDictionary<TKey, TValue> {
		Option<TValue> TryGet(TKey key);

	}
}