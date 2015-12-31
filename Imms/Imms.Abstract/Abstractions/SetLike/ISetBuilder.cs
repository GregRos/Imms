namespace Imms.Abstract {

	/// <summary>
	/// A collection builder for set-like collections. This builder is used for abstracting over builders of all collections, no matter the type
	/// </summary>
	/// <typeparam name="TElem">The type of element stored in the collection.</typeparam>
	public interface IAnySetBuilder<in TElem> : IAnyIterableBuilder<TElem> {
		/// <summary>
		/// Removes the specified element from the set, if it exists. Returns true if an element was removed.
		/// </summary>
		/// <param name="value">The element to remove.</param>
		/// <returns></returns>
		bool Remove(TElem value);
		/// <summary>
		/// Returns true if the specified element is contained in the set.
		/// </summary>
		/// <param name="value">The element to check for.</param>
		/// <returns></returns>
		bool Contains(TElem value);
	}

	/// <summary>
	/// A collection builder for set-like collections. Builders are mutable objects that can produce immutable collections.
	/// </summary>
	/// <typeparam name="TElem">The type of element stored in the collection.</typeparam>
	/// <typeparam name="TSet">The set-like collection type produced by the builder.</typeparam>
	public interface ISetBuilder<in TElem, out TSet> : IIterableBuilder<TElem, TSet>, IAnySetBuilder<TElem>
	{
		
	}
}