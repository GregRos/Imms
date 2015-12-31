using System.Collections.Generic;

namespace Funq.Abstract {

	public interface IMapBuilder<TKey, TValue, out TMap> : IIterableBuilder<KeyValuePair<TKey, TValue>, TMap>, IAnyMapBuilder<TKey, TValue>
	{

	}

}