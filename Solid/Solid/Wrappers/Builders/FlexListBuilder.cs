using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Solid.Common;

namespace Solid.Builders
{
	/// <summary>
	///   A class that implicitly builds and caches a collection of type FlexibleList.
	/// </summary>
	/// <typeparam name="T"> The type of value the flex list contains. </typeparam>
	public class FlexListBuilder<T>
		: IEnumerable<T>
	{
		private class CachingEnumerator : IEnumerator<T>
		{
			private FlexibleList<T> _cache;
			private bool _done;
			private readonly IEnumerator<T> _inner;
			private readonly FlexListBuilder<T> _parent;

			public CachingEnumerator(FlexListBuilder<T> parent, IEnumerable<T> source)
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
				_inner.Dispose();
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

		internal FlexListBuilder(IEnumerable<T> source)
		{
			_ownerId = Thread.CurrentThread.ManagedThreadId;
			_source = source;
		}

		/// <summary>
		///   Forces the evaluation of the flex sequence, if it hasn't been evaluated already, and returns the resulting flexible list.
		/// </summary>
		/// <param name="flex"> The flex. </param>
		/// <returns> </returns>
		public static implicit operator FlexibleList<T>(FlexListBuilder<T> flex)
		{
			return flex.Force();
		}

		/// <summary>
		///   Explicitly forces the evaluation of the flex list.
		/// </summary>
		public FlexibleList<T> Force()
		{

			if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
			if (_cache != null) return _cache;
			Commit(_source.ToFlexList());
			return _cache;
			
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
		///   Returns the flex list or the type parameter's default value in a singleton flex list.
		/// </summary>
		/// <returns> </returns>
		public FlexListBuilder<T> DefaultIfEmpty()
		{
			return new FlexListBuilder<T>(Source.DefaultIfEmpty());
		}

		/// <summary>
		///   Returns the flex list or the specified default value in a singleton flex list if empty.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> DefaultIfEmpty(T value)
		{
			return new FlexListBuilder<T>(Source.DefaultIfEmpty(value));
		}

		/// <summary>
		///   Returns a flex list consisting of distinct elements, as determined by the type's default equality comparer.
		/// </summary>
		/// <returns> </returns>
		public FlexListBuilder<T> Distinct()
		{
			return new FlexListBuilder<T>(Source.Distinct());
		}

		/// <summary>
		///   Returns a flex list consisting of distinct elements, as determined by the specified equality comparer..
		/// </summary>
		/// <param name="comparer"> The comparer. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> Distinct(IEqualityComparer<T> comparer)
		{
			return new FlexListBuilder<T>(Source.Distinct(comparer));
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
		///   Groups the elements of the flex list using the specified key selector function.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector)
		{
			return new FlexListBuilder<IGrouping<TKey, T>>(Source.GroupBy(keySelector));
		}

		/// <summary>
		///   Groups the elements of the flex list using the specified key selector function, and then applies a result selector on each element.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TElement"> The type of the element. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector,
		                                                                      Func<T, TElement> resultSelector)
		{
			return new FlexListBuilder<IGrouping<TKey, TElement>>(Source.GroupBy(keySelector, resultSelector));
		}

		/// <summary>
		///   Groups the elements of the flex list using the specified key selector function, and then applies a result selector on every group.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector,
		                                                   Func<TKey, IEnumerable<T>, TResult> resultSelector)
		{
			return new FlexListBuilder<TResult>(Source.GroupBy(keySelector, resultSelector));
		}

		/// <summary>
		///   Groups the elements of the flex list using the specified key selector, and then applies an additional selector on every element.
		///   Each group is then processed by a result selector.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TElement"> The type of the element. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <param name="elementSelector"> The element selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector,
		                                                             Func<T, TElement> elementSelector,
		                                                             Func<TKey, IEnumerable<TElement>, TResult>
			                                                             resultSelector)
		{
			return new FlexListBuilder<TResult>(Source.GroupBy(keySelector, elementSelector, resultSelector));
		}

		/// <summary>
		///   Correlates the elements of two flex lists based on matching keys. The keys are compared by the default equality comparer.
		/// </summary>
		/// <typeparam name="TInner"> The type of the inner. </typeparam>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="inner"> The inner. </param>
		/// <param name="outerSelector"> The outer selector. </param>
		/// <param name="innerSelector"> The inner selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> Join<TInner, TKey, TResult>
			(IEnumerable<TInner> inner, Func<T, TKey> outerSelector, Func<TInner, TKey> innerSelector,
			 Func<T, TInner, TResult> resultSelector)
		{
			return new FlexListBuilder<TResult>(Source.Join(inner, outerSelector, innerSelector, resultSelector));
		}

		/// <summary>
		///   Orders the elements of the flex list in ascending order, by key. The default comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			return new FlexListBuilder<T>(Source.OrderBy(keySelector));
		}

		/// <summary>
		///   Orders the flex list in descending order, by key. The default equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="keySelector"> The key selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			return new FlexListBuilder<T>(Source.OrderByDescending(keySelector));
		}

		/// <summary>
		///   Correlates the elements of this flex list with another, and groups the results. The default equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TInner"> The type of the elements of the inner sequence. </typeparam>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="inner"> The inner sequence to join with the flex list. </param>
		/// <param name="outerSelector"> The key selector for the flex list. </param>
		/// <param name="innerSelector"> The key selector for the sequence. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> OuterJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner,
		                                                             Func<T, TKey> outerSelector,
		                                                             Func<TInner, TKey> innerSelector,
		                                                             Func<T, IEnumerable<TInner>, TResult>
			                                                             resultSelector)
		{
			return new FlexListBuilder<TResult>(Source.GroupJoin(inner, outerSelector, innerSelector, resultSelector));
		}

		/// <summary>
		///   Projects each element of the flex list using the specified selector..
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new FlexListBuilder<TResult>(Source.Select(selector));
		}

		/// <summary>
		///   Projects each element of the flex list into a sequence, and then flattens the results.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			return new FlexListBuilder<TResult>(Source.SelectMany(selector));
		}

		/// <summary>
		///   Projects each element of the flex list into a sequence, and then applies a result selector on every sequence.
		/// </summary>
		/// <typeparam name="TCollect"> The type of the sequence. </typeparam>
		/// <typeparam name="TResult"> The type of the final result. </typeparam>
		/// <param name="manySelector"> The sequence selector. </param>
		/// <param name="resultSelector"> The result selector. </param>
		/// <returns> </returns>
		public FlexListBuilder<TResult> SelectMany<TCollect, TResult>(Func<T, IEnumerable<TCollect>> manySelector,
		                                                          Func<T, TCollect, TResult> resultSelector)
		{
			return new FlexListBuilder<TResult>(Source.SelectMany(manySelector, resultSelector));
		}

		/// <summary>
		///   Skips the first several elements of the flex list.
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> Skip(int count)
		{
			return new FlexListBuilder<T>(Source.Skip(count));
		}

		/// <summary>
		///   Bypasses the elements of the flex list as log as the condition is true.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> SkipWhile(Func<T, bool> predicate)
		{
			return new FlexListBuilder<T>(Source.SkipWhile(predicate));
		}

		/// <summary>
		///   Takes the first several elements of the flex list.
		/// </summary>
		/// <param name="count"> The count. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> Take(int count)
		{
			return new FlexListBuilder<T>(Source.Take(count));
		}

		/// <summary>
		///   Returns elements from the flex list as long as the predicate is true.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> TakeWhile(Func<T, bool> predicate)
		{
			return new FlexListBuilder<T>(Source.TakeWhile(predicate));
		}

		/// <summary>
		///   Filters the elements of the flex list using the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> Where(Func<T, bool> predicate)
		{
			return new FlexListBuilder<T>(Source.Where(predicate));
		}

		/// <summary>
		///   Filters the elements of the flex list using the specified predicate. The predicate takes the index as a parameter.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public FlexListBuilder<T> Where(Func<T, int, bool> predicate)
		{
			return new FlexListBuilder<T>(Source.Where(predicate));
		}
	}
}