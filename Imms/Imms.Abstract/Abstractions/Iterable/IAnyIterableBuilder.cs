using System;
using System.Collections.Generic;

namespace Imms.Abstract {

	/// <summary>
	/// Represents a collection builder for an iterable collection. Collection builders are mutable collections that produce immutable ones. This interface is used for abstracting over all collection builders, when the specific collection type is not known.
	/// </summary>
	/// <typeparam name="TElem">The type of element stored in the collection.</typeparam>
	public interface IAnyIterableBuilder<in TElem> : IDisposable {
		/// <summary>
		/// Adds an element to the collection.
		/// </summary>
		/// <param name="elem">The element to add.</param>
		/// <returns></returns>
		bool Add(TElem elem);

		/// <summary>
		/// Adds a range of elements to the builder.
		/// </summary>
		/// <param name="items"></param>
		void AddRange(IEnumerable<TElem> items);

		int Length { get; }
	}
}