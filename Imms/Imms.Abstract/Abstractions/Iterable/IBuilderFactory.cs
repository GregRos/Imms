namespace Imms.Abstract {

	/// <summary>
	///     Represents a builder factory, which produces builders for some collection type. Used for abstracting over all builders, for any collection and element type.
	/// </summary>
	/// <typeparam name="TBuilder">The type of builder this factory produces.</typeparam>
	public interface IBuilderFactory<out TBuilder>{
		/// <summary>
		///     Returns an empty builder.
		/// </summary>
		TBuilder EmptyBuilder { get; }

	}

}