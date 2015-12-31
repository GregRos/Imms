using System;
using System.Collections.Generic;

namespace Funq.Abstract {

	/// <summary>
	///     Used for abstracting over any Iterable collection type when the concrete type is unknown.
	///     Should not be implemented in user code. Mainly provided for optimization purposes.
	/// </summary>
	/// <typeparam name="TElem"></typeparam>
	internal interface IAnyIterable<out TElem> : IEnumerable<TElem> {

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		int Length { get; }

		/// <summary>
		///     Applies the specified delegate on every item in the collection, in the default ordering, until the function returns false
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		bool ForEachWhile(Func<TElem, bool> function);
	}
}