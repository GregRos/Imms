using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for abstracting over collection builder factories when the concrete type is unknown. Should not be implemented in user code.
	/// </summary>
	/// <typeparam name="TElem">The type of element the builder is for.</typeparam>
	/// <typeparam name="TBuilder">The type of builder this factory produces.</typeparam>
	public interface IAnyBuilderFactory<TElem, out TBuilder> : IAnyIterable<TElem>
		where TBuilder : IterableBuilder<TElem>
	{
		/// <summary>
		/// Returns an empty builder.
		/// </summary>
		TBuilder EmptyBuilder
		{
			get;
		}

		/// <summary>
		/// Returns a builder initialized with the specified iterable collection. The collection must be of a type expected by this factory.
		/// </summary>
		/// <param name="iterable">The iterable collection.</param>
		/// <returns></returns>
		TBuilder BuilderFrom(IAnyIterable<TElem> iterable);

		/// <summary>
		/// Returns a concrete collection from the specified builder. The builder must be of the type expected by this factory.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
		IAnyBuilderFactory<TElem, TBuilder> IterableFrom(IterableBuilder<TElem> builder);
	}

}