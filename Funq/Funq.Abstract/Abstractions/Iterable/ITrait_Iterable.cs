using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	public interface ITrait_Iterable<out Elem> : IEnumerable<Elem>
	{
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

		void ForEach(Action<Elem> act);

		/// <summary>
		///   Applies the specified delegate on every item in the collection, from last to first.
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <returns> </returns>
		bool ForEachWhile(Func<Elem, bool> iterator);
	}
}