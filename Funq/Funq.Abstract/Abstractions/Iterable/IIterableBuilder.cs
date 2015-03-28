using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for abstracting over builders for iterable collections when the underlying type is unknown.
	/// </summary>
	/// <typeparam name="TElem"></typeparam>
	public interface IIterableBuilder<in TElem> : IDisposable
	{
		/// <summary>
		/// Adds an element to the builder.
		/// </summary>
		/// <param name="item"></param>
		void Add(TElem item);

		/// <summary>
		/// Adds multiple elements to the builder.
		/// </summary>
		/// <param name="items"></param>
		void AddMany(IEnumerable<TElem> items);

	}

}