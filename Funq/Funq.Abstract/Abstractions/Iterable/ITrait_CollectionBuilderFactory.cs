namespace Funq.Abstract
{
	public interface ITrait_CollectionBuilderFactory<Elem, out TBuilder> : ITrait_Iterable<Elem>
		where TBuilder : IterableBuilder<Elem>
	{
		TBuilder EmptyBuilder
		{
			get;
		}

		TBuilder BuilderFrom(ITrait_Iterable<Elem> provider);

		ITrait_CollectionBuilderFactory<Elem, TBuilder> ProviderFrom(IterableBuilder<Elem> builder);
	}
}