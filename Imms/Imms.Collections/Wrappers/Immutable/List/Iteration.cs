using System;
using System.Collections;
using System.Collections.Generic;
using Imms.Implementation;

namespace Imms {
	//This file contains method for iterating, folding, and projecting over the collection.
	partial class ImmList<T> : IEnumerable<T> {
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from last to first, until it returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function null.</exception>
		public override bool ForEachBackWhile(Func<T, bool> function) {
			return Root.IterBackWhile(x => function(x));
		}

		/// <summary>
		///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public override bool ForEachWhile(Func<T, bool> function) {
			return Root.IterWhile(x => function(x));
		}

		/// <summary>
		///     Gets a new enumerator that iterates over the list.
		/// </summary>
		/// <returns> </returns>
		public override IEnumerator<T> GetEnumerator() {
			return new FingerTreeIterator<T>(Root);
		}
	}
}