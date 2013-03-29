using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Solid.Common;

namespace Solid
{
	/// <summary>
	/// A class that implicitly builds and caches a collection of type FlexibleList.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelayedList<T>
		: IEnumerable<T>
	{
		private readonly int _ownerId;
		private readonly IEnumerable<T> _source;
		private FlexibleList<T> _cache;

		public DelayedList(IEnumerable<T> source)
		{
			_ownerId = Thread.CurrentThread.ManagedThreadId;
			_source = source;
		}

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

		protected IEnumerable<T> Source
		{
			get
			{
				return _cache ?? _source;
			}
		}

		public DelayedList<T> DefaultIfEmpty()
		{
			return new DelayedList<T>(Source.DefaultIfEmpty());
		}

		public DelayedList<T> DefaultIfEmpty(T value)
		{
			return new DelayedList<T>(Source.DefaultIfEmpty(value));
		}

		public DelayedList<T> Distinct<TKey>()
		{
			return new DelayedList<T>(Source.Distinct());
		}


		public DelayedList<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<IGrouping<TKey, T>>(Source.GroupBy(keySelector));
		}

		public DelayedList<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector,
		                                                                          Func<T, TElement> resultSelector)
		{
			return new DelayedList<IGrouping<TKey, TElement>>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedList<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector,
		                                                       Func<TKey, IEnumerable<T>, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedList<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector,
		                                                                 Func<T, TElement> elementSelector,
		                                                                 Func<TKey, IEnumerable<TElement>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupBy(keySelector, elementSelector, resultSelector));
		}

		public DelayedList<TResult> Join<TInner, TKey, TResult>
			(IEnumerable<TInner> inner, Func<T, TKey> outerSelector, Func<TInner, TKey> innerSelector,
			 Func<T, TInner, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.Join(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedList<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<T>(Source.OrderBy(keySelector));
		}

		public DelayedList<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedList<T>(Source.OrderByDescending(keySelector));
		}

		public DelayedList<TResult> OuterJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner,
		                                                                 Func<T, TKey> outerSelector,
		                                                                 Func<TInner, TKey> innerSelector,
		                                                                 Func<T, IEnumerable<TInner>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedList<TResult>(Source.GroupJoin(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedList<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new DelayedList<TResult>(Source.Select(selector));
		}

		public DelayedList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			return new DelayedList<TResult>(Source.SelectMany(selector));
		}

		public DelayedList<TResult> SelectMany<TCollect, TResult>(Func<T, IEnumerable<TCollect>> manySelector,
		                                                              Func<T, TCollect, TResult> resultSelector)
		{
			return new DelayedList<TResult>(Source.SelectMany(manySelector, resultSelector));
		}

		public DelayedList<T> Skip(int count)
		{
			return new DelayedList<T>(Source.Skip(count));
		}

		public DelayedList<T> SkipWhile(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.SkipWhile(predicate));
		}

		public DelayedList<T> Take(int count)
		{
			return new DelayedList<T>(Source.Take(count));
		}

		public DelayedList<T> TakeWhile(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.TakeWhile(predicate));
		}

		public DelayedList<T> Where(Func<T, bool> predicate)
		{
			return new DelayedList<T>(Source.Where(predicate));
		}

		public DelayedList<T> Where(Func<T, int, bool> predicate)
		{
			return new DelayedList<T>(Source.Where(predicate));
		}


		protected void Commit(FlexibleList<T> result)
		{
			if (_cache != null) return;
			_cache = result;
		}

		/// <summary>
		/// Forces the evaluation of the delayed sequence, if it hasn't been evaluated already.
		/// </summary>
		/// <param name="delayed"></param>
		/// <returns></returns>
		public static implicit operator FlexibleList<T>(DelayedList<T> delayed)
		{
			return delayed.Force;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
			return _cache != null ? (_cache as IEnumerable<T>).GetEnumerator() : new CachingEnumerator(this, _source);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class CachingEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<T> _inner;
			private readonly DelayedList<T> _parent;
			private FlexibleList<T> _cache;
			private bool _done;

			public CachingEnumerator(DelayedList<T> parent, IEnumerable<T> source)
			{
				_parent = parent;
				_cache = FlexibleList<T>.Empty;
				_inner = source.GetEnumerator();
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
		}
	}
}