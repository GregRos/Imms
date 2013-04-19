using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid
{
	public sealed partial  class Vector<T> : IEnumerable<T>
	{
		/// <summary>
		///   Applies an accumulator over the collection.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value, or the default value for the type. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult Fold<TResult>( TResult initial,Func<TResult, T, TResult> accumulator)
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Applies the acummulator function over every element in the collection, from last to first.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		public TResult FoldBack<TResult>(TResult initial,Func<TResult, T, TResult> accumulator)
		{
			ForEachBack(v => initial = accumulator(initial, v));
			return initial;
		}
		 
		/// <summary>
		///   Iterates over every item in the vector, from first to last.
		/// </summary>
		/// <param name="action"> The function to apply on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEach(Action<T> action)
		{
			if (action == null) throw Errors.Argument_null("action");
			_root.Iter(action);
		}

		/// <summary>
		///   Iterates over every element in the vector, from last to first.
		/// </summary>
		/// <param name="conditional"> The function applied on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachBack(Action<T> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			_root.IterBack(conditional);
		}

		/// <summary>
		///   Iterates over every element in the vector, from last to first. Stops if the function returns false.
		/// </summary>
		/// <param name="conditional"> The function applied on each element. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			_root.IterBackWhile(conditional);
		}

		/// <summary>
		///   Iterates over every element in the collection, from first to last. Stops if the function returns false.
		/// </summary>
		/// <param name="conditional"> The function used for iterating over the collection. </param>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");
			_root.IterWhile(conditional);
		}

		/// <summary>
		///   Returns the enumerator that allows iterating over the collection.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _root.GetEnumerator(true);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Returns the index of the first item that fulfills the predicate, or null.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public int? IndexOf(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = 0;
			var result = _root.IterWhile(v =>
			                            {
				                            if (predicate(v)) return false;
				                            index++;
				                            return true;
			                            });
			return !result ? (int?) index : null;
		}

		/// <summary>
		///   Projects each element using the specified selector. O(n), fast.
		/// </summary>
		/// <typeparam name="TOut"> The projected type of each element. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		/// <exception cref="OutOfMemoryException">Thrown if the system runs out of memory.</exception>
		public Vector<TOut> Select<TOut>(Func<T, TOut> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			return new Vector<TOut>(_root.Apply(selector));
		}

		/// <summary>
		///   Returns a vector consisting of the first items that fulfill the predicate. O(m), fast.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function is null.</exception>
		public Vector<T> TakeWhile(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = IndexOf(predicate);
			if (!index.HasValue)
			{
				return this;
			}
			return Take((int) index + 1);
		}

		/// <summary>
		///   Filters the collection using the specified predicate. O(n), fast.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		/// <exception cref="OutOfMemoryException">Thrown if the system runs out of memory.</exception>
		public Vector<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			var index = 0;
			var array = new T[Count];
			var newVector = empty;
			ForEach(v =>
			        {
				        if (predicate(v))
				        {
					        array[index] = v;
					        index++;
				        }
			        });
			return new Vector<T>(TrieVector<T>.VectorNode.Empty.BulkLoad(array, 0, index));
		}
	}
}