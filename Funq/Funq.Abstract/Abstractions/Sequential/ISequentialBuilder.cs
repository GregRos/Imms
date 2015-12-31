namespace Funq.Abstract {
	/// <summary>
	/// A collection builder for sequential, list-like collections.
	/// </summary>
	/// <typeparam name="TElem">The type of element stored in the collection.</typeparam>
	/// <typeparam name="TList">The type of collection produced by this builder.</typeparam>
	public interface ISequentialBuilder<in TElem, out TList> : IIterableBuilder<TElem, TList> {
			
	}
}