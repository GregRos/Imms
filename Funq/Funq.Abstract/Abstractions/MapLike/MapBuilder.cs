using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	///   A parent class for map builders
	/// </summary>
	/// <typeparam name="TKey"> The type of the key. </typeparam>
	/// <typeparam name="TValue"> The type of the value. </typeparam>
	public abstract class MapBuilder<TKey, TValue> :
		IterableBuilder<Kvp<TKey, TValue>>, IMapBuilder<TKey, TValue>
	{
		public TValue this[TKey k]
		{
			get
			{
				if (IsDisposed) throw Errors.Is_disposed("MapBuilder");
				var v = Lookup(k);
				return v.ValueOrError(Errors.Key_not_found);
			}
			set
			{
				Add(k, value);
			}
		}

		public void Add(TKey k, TValue v)
		{
			if (IsDisposed) throw Errors.Is_disposed("MapBuilder");
			Add(Kvp.Of(k, v));
		}

		public abstract Option<TValue> Lookup(TKey k);
	}
}