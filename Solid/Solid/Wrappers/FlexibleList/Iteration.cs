using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid
{
	//This file contains method for iterating, folding, and projecting over the collection.

	partial class FlexibleList<T> : IEnumerable<T>
	{
		private class FinalEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<Leaf<T>> inner;

			public FinalEnumerator(IEnumerator<Leaf<T>> inner)
			{
				this.inner = inner;
			}

			public T Current
			{
				get
				{
					return inner.Current.Value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return inner.MoveNext();
			}

			public void Reset()
			{
			}
		}

		/// <summary>
		///   Returns an enumerator for iterating over the collection from last to first.
		/// </summary>
		public IEnumerable<T> Backward
		{
			get
			{
				return new EnumerableProxy<T>(() => new FinalEnumerator(_root.GetEnumerator(false)));
			}
		}

		/// <summary>
		///   Returns an enumerator for iterating over the collection first element to last.
		/// </summary>
		public IEnumerable<T> Forward
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		///   Applies the accumulator function on the list, from first to last. O(n)
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult Fold<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Applies an accumulator over the list, from last to first.  O(n)
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="accumulator"> The accumulator. </param>
		/// <param name="initial"> The initial value. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		public TResult FoldBack<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
		{
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			ForEachBack(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Iterates over the list, from first to last. O(n)
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEach(Action<T> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			_root.Iter(x => iterator(x.Value));
		}

		/// <summary>
		///   Iterates over the list, from last to first. O(n)
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachBack(Action<T> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			_root.IterBack(x => iterator(x.Value));
		}

		/// <summary>
		///   Iterates over the list, from last to first, and stops if the conditional returns false. O(m)
		/// </summary>
		/// <param name="conditional"> The conditional. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachBackWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			_root.IterBackWhile(m => conditional(m.Value));
		}

		///<summary>
		///  Iterates over the list, from first to last, and stops if the conditional returns false.
		///</summary>
		///<param name="conditional"> The conditional iterator that specifies whether to proceed with iteration. </param>
		///<exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public void ForEachWhile(Func<T, bool> conditional)
		{
			if (conditional == null) throw Errors.Argument_null("conditional");

			_root.IterWhile(m => conditional(m.Value));
		}

		/// <summary>
		///   Gets a new enumerator that iterates over the list.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<T> GetEnumerator()
		{
			var enumerator = _root.GetEnumerator(true);
			for (; enumerator.MoveNext();)
			{
				yield return enumerator.Current.Value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<T>).GetEnumerator();
		}

		/// <summary>
		///   Returns the index of the first item that fulfills the specified conditional, or null.
		/// </summary>
		/// <param name="predicate"> The conditional. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		internal int? IndexOf(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var index = 0;
			ForEachWhile(v =>
			             {
				             if (predicate(v)) return false;
				             index++;
				             return true;
			             });
			return index > Count ? null : (int?) index;
		}

		/// <summary>
		///   Transforms the list by applying the specified selector on every item in the list. O(n).
		/// </summary>
		/// <typeparam name="TOut"> The output type of the transformation. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<TOut> Select<TOut>(Func<T, TOut> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			var newRoot = FlexibleList<TOut>.emptyFTree;
			ForEach(v => newRoot.AddRight(new Leaf<TOut>(selector(v))));
			return new FlexibleList<TOut>(newRoot);
		}

		///<summary>
		///  Applies the specified selector on every item in the list, flattens, and constructs a new list. O(n·m)
		///</summary>
		///<typeparam name="TResult"> The type of the result. </typeparam>
		///<param name="selector"> The selector. </param>
		///<returns> </returns>
		///<exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public FlexibleList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			if (selector == null) throw Errors.Argument_null("selector");
			var result = FlexibleList<TResult>.Empty;
			ForEach(v => result = result.AddLastRange(selector(v)));
			return result;
		}

		/// <summary>
		///   Returns a list without the first elements for which the conditional returns true. O(logn+m)
		/// </summary>
		/// <param name="predicate"> The function </param>
		/// <returns> </returns>
		/// <exception cref="NullReferenceException">Thrown if the conditional is null.</exception>
		public FlexibleList<T> SkipWhile(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var lastIndex = IndexOf(v => !predicate(v));
			return lastIndex.HasValue ? Slice((int) lastIndex) : empty;
		}

		/// <summary>
		///   Returns a list consisting of all the elements for which the conditional returns true.
		/// </summary>
		/// <param name="predicate"> The conditional used to filter the list. </param>
		/// <returns> </returns>
		public FlexibleList<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("conditional");
			var newList = emptyFTree;
			_root.Iter(leaf =>
			          {
				          if (predicate(leaf.Value))
				          {
					          newList = newList.AddRight(leaf);
				          }
			          });
			return new FlexibleList<T>(newList);
		}
	}
}