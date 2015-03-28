using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for abstracting over any Iterable collection type when the concrete type is unknown.
	/// Should not be implemented in user code.
	/// </summary>
	/// <typeparam name="TElem"></typeparam>
	public interface IAnyIterable<TElem> : IReadOnlyCollection<TElem> {
		/// <summary>
		///   Returns true if the underlying colleciton is empty, and false otherwise.
		/// </summary>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		///   Returns the number of elements in the collection.
		/// </summary>
		int Length
		{
			get;
		}

		/// <summary>
		/// Copies the iterable collection to the specified array.
		/// </summary>
		/// <param name="arr">The array.</param>
		/// <param name="arrStart">The array index from which to start copying.</param>
		/// <param name="count">The number of elements to copy.</param>
		void CopyTo(TElem[] arr, int arrStart, int count);

		void ForEach(Action<TElem> act);

		/// <summary>
		///   Applies the specified delegate on every item in the collection, in the default ordering.
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <returns> </returns>
		bool ForEachWhile(Func<TElem, bool> iterator);
	}
}