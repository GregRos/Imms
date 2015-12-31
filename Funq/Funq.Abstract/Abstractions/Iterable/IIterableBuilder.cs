namespace Funq.Abstract {

	/// <summary>
	/// A builder for any iterable collection. A builder is a mutable collection used to produce immutable collections.
	/// </summary>
	/// <typeparam name="TElem">The element type stored by the collection.</typeparam>
	/// <typeparam name="TResult">The type of collection produced by the builder.</typeparam>
	public interface IIterableBuilder<in TElem, out TResult> : IAnyIterableBuilder<TElem> {
		/// <summary>
		/// Produces the result collection. While this method should be callable more than once, and should be efficient, it normally reduces the efficiency of future operations on this builder
		/// </summary>
		TResult Produce();
	}

}