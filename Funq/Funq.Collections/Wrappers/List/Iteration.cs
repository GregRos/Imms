using System;
using System.Collections;
using System.Collections.Generic;
using Funq.Implementation;

namespace Funq {
	//This file contains method for iterating, folding, and projecting over the collection.
	partial class FunqList<T> : IEnumerable<T> {
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public override bool ForEachBackWhile(Func<T, bool> function) {
			return Root.IterBackWhile(x => function(x));
		}

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