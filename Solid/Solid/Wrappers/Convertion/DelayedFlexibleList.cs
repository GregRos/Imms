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
	public class DelayedFlexibleList<T>
		: IEnumerable<T>
	{
		private readonly int _ownerId;
		private readonly IEnumerable<T> _source;
		private FlexibleList<T> _cache;

		public DelayedFlexibleList(IEnumerable<T> source)
		{
			List<int> x;
			
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

		public DelayedFlexibleList<T> DefaultIfEmpty()
		{
			return new DelayedFlexibleList<T>(Source.DefaultIfEmpty());
		}

		public DelayedFlexibleList<T> DefaultIfEmpty(T value)
		{
			return new DelayedFlexibleList<T>(Source.DefaultIfEmpty(value));
		}

		public DelayedFlexibleList<T> Distinct<TKey>()
		{
			return new DelayedFlexibleList<T>(Source.Distinct());
		}


		public DelayedFlexibleList<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedFlexibleList<IGrouping<TKey, T>>(Source.GroupBy(keySelector));
		}

		public DelayedFlexibleList<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector,
		                                                                          Func<T, TElement> resultSelector)
		{
			return new DelayedFlexibleList<IGrouping<TKey, TElement>>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedFlexibleList<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector,
		                                                       Func<TKey, IEnumerable<T>, TResult> resultSelector)
		{
			return new DelayedFlexibleList<TResult>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedFlexibleList<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector,
		                                                                 Func<T, TElement> elementSelector,
		                                                                 Func<TKey, IEnumerable<TElement>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedFlexibleList<TResult>(Source.GroupBy(keySelector, elementSelector, resultSelector));
		}

		public DelayedFlexibleList<TResult> Join<TInner, TKey, TResult>
			(IEnumerable<TInner> inner, Func<T, TKey> outerSelector, Func<TInner, TKey> innerSelector,
			 Func<T, TInner, TResult> resultSelector)
		{
			return new DelayedFlexibleList<TResult>(Source.Join(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedFlexibleList<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedFlexibleList<T>(Source.OrderBy(keySelector));
		}

		public DelayedFlexibleList<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedFlexibleList<T>(Source.OrderByDescending(keySelector));
		}

		public DelayedFlexibleList<TResult> OuterJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner,
		                                                                 Func<T, TKey> outerSelector,
		                                                                 Func<TInner, TKey> innerSelector,
		                                                                 Func<T, IEnumerable<TInner>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedFlexibleList<TResult>(Source.GroupJoin(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedFlexibleList<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new DelayedFlexibleList<TResult>(Source.Select(selector));
		}

		public DelayedFlexibleList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			return new DelayedFlexibleList<TResult>(Source.SelectMany(selector));
		}

		public DelayedFlexibleList<TResult> SelectMany<TCollect, TResult>(Func<T, IEnumerable<TCollect>> manySelector,
		                                                              Func<T, TCollect, TResult> resultSelector)
		{
			return new DelayedFlexibleList<TResult>(Source.SelectMany(manySelector, resultSelector));
		}

		public DelayedFlexibleList<T> Skip(int count)
		{
			return new DelayedFlexibleList<T>(Source.Skip(count));
		}

		public DelayedFlexibleList<T> SkipWhile(Func<T, bool> predicate)
		{
			return new DelayedFlexibleList<T>(Source.SkipWhile(predicate));
		}

		public DelayedFlexibleList<T> Take(int count)
		{
			return new DelayedFlexibleList<T>(Source.Take(count));
		}

		public DelayedFlexibleList<T> TakeWhile(Func<T, bool> predicate)
		{
			return new DelayedFlexibleList<T>(Source.TakeWhile(predicate));
		}

		public DelayedFlexibleList<T> Where(Func<T, bool> predicate)
		{
			return new DelayedFlexibleList<T>(Source.Where(predicate));
		}

		public DelayedFlexibleList<T> Where(Func<T, int, bool> predicate)
		{
			return new DelayedFlexibleList<T>(Source.Where(predicate));
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
		public static implicit operator FlexibleList<T>(DelayedFlexibleList<T> delayed)
		{
			return delayed.Force;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
			return _cache != null ? _cache.GetEnumerator() : new CachingEnumerator(this, _source);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class CachingEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<T> _inner;
			private readonly DelayedFlexibleList<T> _parent;
			private FlexibleList<T> _cache;
			private bool _done;

			public CachingEnumerator(DelayedFlexibleList<T> parent, IEnumerable<T> source)
			{
				_parent = parent;
				_cache = FlexibleList.Empty<T>();
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