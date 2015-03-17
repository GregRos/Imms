using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using Funq.Abstract;

public interface __HandlerObject__<TKey>
{

}

public interface __Parent__
{
	
}

partial class __ListLikeClass__<T>
{
	protected override IterableBuilder<T> EmptyBuilder
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected override __ListLikeClass__<T> ProviderFrom(IterableBuilder<T> builder)
	{
		throw new NotImplementedException();
	}

	protected override IterableBuilder<T> BuilderFrom(__ListLikeClass__<T> provider)
	{
		throw new NotImplementedException();
	}

	protected override IEnumerator<T> GetEnumerator()
	{
		throw new NotImplementedException();
	}
}


partial class __OrderedSetLikeClass__<T>
{
	private __OrderedSetLikeClass__(__HandlerObject__<T> ph)
	{

	}

	public override bool Contains(T item)
	{
		throw new NotImplementedException();
	}
	private static __HandlerObject__<T> Default
	{
		get
		{
			return null;
		}
	}

	private static __OrderedSetLikeClass__<T> Empty(__HandlerObject__<T> ph)
	{
		return new __OrderedSetLikeClass__<T>(ph ?? Default);
	}


	protected override SetBuilder<T> EmptyBuilder
	{
		get
		{
			throw new NotImplementedException();
		}
	}


	protected override SetBuilder<T> BuilderFrom(__OrderedSetLikeClass__<T> provider)
	{
		throw new NotImplementedException();
	}

	protected override IEnumerator<T> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	protected override __OrderedSetLikeClass__<T> ProviderFrom(SetBuilder<T> builder)
	{
		throw new NotImplementedException();
	}
}


partial class __SetLikeClass__<T>
{
	private __SetLikeClass__(__HandlerObject__<T> ph)
	{

	}

	public override bool Contains(T item)
	{
		throw new NotImplementedException();
	}
	private static __HandlerObject__<T> Default
	{
		get
		{
			return null;
		}
	}

	private static __SetLikeClass__<T> Empty(__HandlerObject__<T> ph)
	{
		return new __SetLikeClass__<T>(ph ?? Default);
	}


	protected override SetBuilder<T> EmptyBuilder
	{
		get
		{
			throw new NotImplementedException();
		}
	}


	protected override SetBuilder<T> BuilderFrom(__SetLikeClass__<T> provider)
	{
		throw new NotImplementedException();
	}

	protected override IEnumerator<T> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	protected override __SetLikeClass__<T> ProviderFrom(SetBuilder<T> builder)
	{
		throw new NotImplementedException();
	}
}

partial class __MapLikeClass__<TKey, TValue>
{

	private __MapLikeClass__(__HandlerObject__<TKey> ph)
	{
	}


	private static __MapLikeClass__<TKey, TValue> Empty(__HandlerObject__<TKey> ph)
	{
		return new __MapLikeClass__<TKey, TValue>(ph ?? DefaultHandler);
	}

	private static __HandlerObject__<TKey> DefaultHandler
	{
		get
		{
			return null;
		}
	}

	private __HandlerObject__<TKey> __CurrentHandler__
	{
		get
		{
			return null;
		}
	}

	protected override MapBuilder<TKey, TValue> EmptyBuilder
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected override MapBuilder<TKey, TValue> BuilderFrom(__MapLikeClass__<TKey, TValue> provider)
	{
		throw new NotImplementedException();
	}

	protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	protected override __MapLikeClass__<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
	{
		throw new NotImplementedException();
	}

	public override Option<TValue> TryGet(TKey k)
	{
		throw new NotImplementedException();
	}
}


partial class __OrderedMapLikeClass__<TKey, TValue>
{

	private __OrderedMapLikeClass__(__HandlerObject__<TKey> ph)
	{
	}


	private static __OrderedMapLikeClass__<TKey, TValue> Empty(__HandlerObject__<TKey> ph)
	{
		return new __OrderedMapLikeClass__<TKey, TValue>(ph ?? DefaultHandler);
	}

	private static __HandlerObject__<TKey> DefaultHandler
	{
		get
		{
			return null;
		}
	}

	private __HandlerObject__<TKey> __CurrentHandler__
	{
		get
		{
			return null;
		}
	}

	protected override MapBuilder<TKey, TValue> EmptyBuilder
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected override MapBuilder<TKey, TValue> BuilderFrom(__OrderedMapLikeClass__<TKey, TValue> provider)
	{
		throw new NotImplementedException();
	}

	protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	protected override __OrderedMapLikeClass__<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
	{
		throw new NotImplementedException();
	}

	public override Option<TValue> TryGet(TKey k)
	{
		throw new NotImplementedException();
	}
}

