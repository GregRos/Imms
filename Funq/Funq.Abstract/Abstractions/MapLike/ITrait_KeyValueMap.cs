namespace Funq.Abstract
{
	public interface ITrait_KeyValueMap<TKey, TValue> : ITrait_CollectionBuilderFactory<Kvp<TKey, TValue>, MapBuilder<TKey,TValue>>
	{
		Option<TValue> TryGet(TKey key);

		bool ContainsKey(TKey key);
	}
}