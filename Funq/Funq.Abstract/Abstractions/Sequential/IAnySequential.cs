using System.Collections.Generic;

namespace Funq.Abstract
{
	/// <summary>
	/// Used for generalizing over list-like collections when the concrete collection type is unknown.
	/// </summary>
	/// <typeparam name="TElem"></typeparam>
	public interface IAnySequential<TElem> : IAnyBuilderFactory<TElem, IterableBuilder<TElem>>, IReadOnlyList<TElem> {
		/// <summary>
		/// Returns the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		TElem this[int index]
		{
			get;
		}

		/// <summary>
		/// Returns the first item in the collection.
		/// </summary>
		TElem First
		{
			get;
		}

		/// <summary>
		/// Returns the last item in the collection.
		/// </summary>
		TElem Last
		{
			get;
		}

		/// <summary>
		/// Returns the first item in the collection, or Option.None if the collection is empty.
		/// </summary>
		Option<TElem> TryFirst
		{
			get;
		}

		/// <summary>
		/// Returns the last item in the collection, or Option.None if the collection is empty.
		/// </summary>
		Option<TElem> TryLast
		{
			get;
		}

		/// <summary>
		/// Copies the collection to an array.
		/// </summary>
		/// <param name="arr">The array. Must be long enoguh to hold the data.</param>
		/// <param name="myStart">The index of this collection at which to start copying.</param>
		/// <param name="arrStart">The index of the array at which to start copying.</param>
		/// <param name="count">The number of elements to copy.</param>
		void CopyTo(TElem[] arr, int myStart, int arrStart, int count);
	}
}