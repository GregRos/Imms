using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq;
using Funq.Collections.Common;
using Funq.Collections.Implementation;

namespace Funq.Collections
{
	//This file contains method for iterating, folding, and projecting over the collection.
	partial class FunqList<T> : IEnumerable<T>
	{
		public override bool ForEachBackWhile(Func<T, bool> iterator)
		{
			return _root.IterBackWhile(x => iterator(x));
		}

		public override bool ForEachWhile(Func<T, bool> iterator)
		{
			return _root.IterWhile(x => iterator(x));
		}


		/// <summary>
		///   Gets a new enumerator that iterates over the list.
		/// </summary>
		/// <returns> </returns>
		protected override IEnumerator<T> GetEnumerator() {
			return new FingerTreeIterator<T>(_root);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<T>).GetEnumerator();
		}
	}
}