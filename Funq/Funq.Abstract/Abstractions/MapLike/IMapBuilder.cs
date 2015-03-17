namespace Funq.Abstract
{
	public interface IMapBuilder<TKey, TValue> : IIterableBuilder<Kvp<TKey, TValue>>
	{
		TValue this[TKey key]
		{
			get;
			set;
		}
	}
}