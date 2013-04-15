using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Solid.Common;

namespace Solid
{
	/// <summary>
	///   A class that implicitly builds and caches a collection of type FlexibleList.
	/// </summary>
	/// <typeparam name="T"> The type of value the delayed list contains. </typeparam>
	public class DelayedList<T>
		: IEnumerable<T>
	{
		private class CachingEnumerator : IEnumerator<T>
		{
			private FlexibleList<T> _cache;
			private bool _done;
			private readonly IEnumerator<T> _inner;
			private readonly DelayedList<T> _parent;

			public CachingEnumerator(DelayedList<T> parent, IEnumerable<T> source)
			{
				_parent = parent;
				_cache = FlexibleList<T>.Empty;
				_inner = source.GetEnumerator();
			}

			public T Current
			{
				get
				{
					return _inner.Current;
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
				if (_done)
				{
					_parent.Commit(_cache);
				}
			}

			public bool MoveNext()
			{
				if (_inner.MoveNext())
				{
					_cache = _cache.AddLast(_inner.Current);
					return true;
				}
				_done = true;
				return false;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private FlexibleList<T> _cache;

		private readonly int _ownerId;
		private readonly IEnumerable<T> _source;

		internal DelayedList(IEnumerable<T> source)
		{
			_ownerId = Thread.CurrentThread.ManagedThreadId;
			_source = source;
		}

		/// <summary>
		/// Forces the evaluation of the delayed sequence, if it hasn't been evaluated already, and returns the resulting flexible list.
		/// </summary>
		/// <param name="delayed">The delayed.</param>
		/// <returns></returns>
		public static implicit operator FlexibleList<T>(DelayedList<T> delayed)
		{
			return delayed.Force;
		}

		/// <summary>
		///   Explicitly forces the evaluation of the delayed list.
		/// </summary>
		public FlexibleList<T> Force
		{
			get
			{
				if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
				if (_cache != null) return _cache;
				Commit(_source.ToFlexibleList());
				return _cache;
			}
		}

		internal IEnumerable<T> Source
		{
			get
			{
				return _cache ?? _source;
			}
		}

		private void Commit(FlexibleList<T> result)
		{
			if (_cache != null) return;
			_cache = result;
		}

		/// <summary>
		///   Returns the delayed list or the type parameter's default value in a singleton delayed list.
		/// </summary>
		/// <returns> </returns>
		public DelayedList<T> DefaultIfEmpty()
		{
			return new DelayedList<T>(Source.DefaultIfEmpty());
		}

		/// <summary>
		/// Returns the delayed list or the specified default value in a singleton delayed list if empty.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public DelayedList<T> DefaultIfEmpty(T value)
		{
			return new DelayedList<T>(Source.DefaultIfEmpty(value));
		}

		/// <summary>
		///   Returns a delayed list consisting of distinct elements, as determined by the type's default equality comparer.
		/// </summary>
		/// <returns> </returns>
		public DelayedList<T> Distinct()
		{
			return new DelayedList<T>(Source.Distinct());
		}

		/// <summary>
		///   Returns a delayed list consisting of distinct elements, as determined by the specified equality comparer..
		/// </summary>
		/// <param name="comparer"> The comparer. </param>
		/// <returns> </returns>
		public DelayedList<T> Distinct(IEqualityComparer<T> comparer)
		{
			return new DelayedList<T>(Source.Distinct(comparer));
		}

		/// <summary>
		///   Returns the getEnumerator that allows iterating over the collection.
		/// </summary>
		/// <returns> </returns>
		public IEnumerator<T> GetEnumerator()
		{
			if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
			return _cache != null ? (_cache as IEnumerable<T>).GetEnumerator() : new CachingEnumerator(this, _source);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<T>).GetEnumerator();
		}

		/// <summary>
		///   Groups the elements of the delayed list using the specified key selector function.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public DelayedList<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<IGrouping<TKey, T>>(Source.GroupBy(keySelector));
		}

		/// <summary>
		///   Groups the elements of the delayed list using the specified key selector function, and then applies a result selector on each element.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TElement"> The type of the element. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public DelayedList<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector,
		                                                                      Func<T, TElement> resultSelector)
		{
			return new DelayedList<IGrouping<TKey, TElement>>(Source.GroupBy(keySelector, resultSelector));
		}

		/// <summary>
		///   Groups the elements of the delayed list using the specified key selector function, and then applies a result selector on every group.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public DelayedList<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector,
		                                                   Func<TKey, IEnumerable<T>, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupBy(keySelector, resultSelector));
		}

		/// <summary>
		///   Groups the elements of the delayed list using the specified key selector, and then applies an additional selector on every element.
		///   Each group is then processed by a result selector.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TElement"> The type of the element. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="elementSelector"> The element selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public DelayedList<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector,
		                                                             Func<T, TElement> elementSelector,
		                                                             Func<TKey, IEnumerable<TElement>, TResult>
			                                                             resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupBy(keySelector, elementSelector, resultSelector));
		}

		/// <summary>
		/// Correlates the elements of two delayed lists based on matching keys. The keys are compared by the default equality comparer.
		/// </summary>
		/// <typeparam name="TInner">The type of the inner.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="inner">The inner.</param>
		/// <param name="outerSelector">The outer selector.</param>
		/// <param name="innerSelector">The inner selector.</param>
		/// <param name="resultSelector">The result selector.</param>
		/// <returns></returns>
		public DelayedList<TResult> Join<TInner, TKey, TResult>
			(IEnumerable<TInner> inner, Func<T, TKey> outerSelector, Func<TInner, TKey> innerSelector,
			 Func<T, TInner, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.Join(inner, outerSelector, innerSelector, resultSelector));
		}

		/// <summary>
		///   Orders the elements of the delayed list in ascending order, by key. The default comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public DelayedList<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<T>(Source.OrderBy(keySelector));
		}

		/// <summary>
		///   Orders the delayed list in descending order, by key. The default equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public DelayedList<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<T>(Source.OrderByDescending(keySelector));
		}

		/// <summary>
		///   Correlates the elements of this delayed list with another, and groups the results. The default equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TInner"> The type of the elements of the inner sequence. </typeparam>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="inner"> The inner sequence to join with the delayed list. </param>
		/// <param name="outerSelector"> The key selector for the delayed list. </param>
		/// <param name="innerSelector"> The key selector for the sequence. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public DelayedList<TResult> OuterJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner,
		                                                             Func<T, TKey> outerSelector,
		                                                             Func<TInner, TKey> innerSelector,
		                                                             Func<T, IEnumerable<TInner>, TResult>
			                                                             resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupJoin(inner, outerSelector, innerSelector, resultSelector));
		}

		/// <summary>
		///   Projects each element of the delayed list using the specified selector..
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		public DelayedList<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new DelayedList<TResult>(Source.Select(selector));
		}

		/// <summary>
		/// Projects each element of the delayed list into a sequence, and then flattens the results.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public DelayedList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			return new DelayedList<TResult>(Source.SelectMany(selector));
		}

		/// <summary>
		///   Projects each element of the delayed list into a sequence, and then applies a result selector on every sequence.
		/// </summary>
		/// <typeparam name="TCollect"> The type of the sequence. </typeparam>
		/// <typeparam name="TResult"> The type of the final result. </typeparam>
		/// <param name="manySelector"> The sequence selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public DelayedList<TResult> SelectMany<TCollect, TResult>(Func<T, IEnumerable<TCollect>> manySelector,
		                                                          Func<T, TCollect, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.SelectMany(manySelector, resultSelector));
		}

		/// <summary>
		///   Skips the first several elements of the delayed list.
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
		/// <returns> </returns>
		public DelayedList<T> Skip(int count)
		{
			return new DelayedList<T>(Source.Skip(count));
		}

		/// <summary>
		///   Bypasses the elements of the delayed list as log as the condition is true.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public DelayedList<T> SkipWhile(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.SkipWhile(predicate));
		}

		/// <summary>
		///   Takes the first several elements of the delayed list.
		/// </summary>
		/// <param name="count"> The count. </param>
		/// <returns> </returns>
		public DelayedList<T> Take(int count)
		{
			return new DelayedList<T>(Source.Take(count));
		}

		/// <summary>
		///   Returns elements from the delayed list as long as the predicate is true.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public DelayedList<T> TakeWhile(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.TakeWhile(predicate));
		}

		/// <summary>
		///   Filters the elements of the delayed list using the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public DelayedList<T> Where(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.Where(predicate));
		}

		/// <summary>
		///   Filters the elements of the delayed list using the specified predicate. The predicate takes the index as a parameter.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public DelayedList<T> Where(Func<T, int, bool> predicate)
		{
			return new DelayedList<T>(Source.Where(predicate));
		}
	}
}