using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Solid.Common;

namespace Solid
{
	/// <summary>
	/// A class that implicitly builds and caches a collection of type Sequence.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelayedSequence<T>
		: IEnumerable<T>
	{
		private readonly int _ownerId;
		private readonly IEnumerable<T> _source;
		private Sequence<T> _cache;

		public DelayedSequence(IEnumerable<T> source)
		{
			_ownerId = Thread.CurrentThread.ManagedThreadId;
			_source = source;
		}

		public Sequence<T> Force
		{
			get
			{
				if (_ownerId != Thread.CurrentThread.ManagedThreadId) throw Errors.Wrong_thread;
				if (_cache != null) return _cache;
				Commit(_source.ToSequence());
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

		public DelayedSequence<T> DefaultIfEmpty()
		{
			return new DelayedSequence<T>(Source.DefaultIfEmpty());
		}

		public DelayedSequence<T> DefaultIfEmpty(T value)
		{
			return new DelayedSequence<T>(Source.DefaultIfEmpty(value));
		}

		public DelayedSequence<T> Distinct<TKey>()
		{
			return new DelayedSequence<T>(Source.Distinct());
		}


		public DelayedSequence<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedSequence<IGrouping<TKey, T>>(Source.GroupBy(keySelector));
		}

		public DelayedSequence<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector,
		                                                                          Func<T, TElement> resultSelector)
		{
			return new DelayedSequence<IGrouping<TKey, TElement>>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedSequence<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector,
		                                                       Func<TKey, IEnumerable<T>, TResult> resultSelector)
		{
			return new DelayedSequence<TResult>(Source.GroupBy(keySelector, resultSelector));
		}

		public DelayedSequence<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector,
		                                                                 Func<T, TElement> elementSelector,
		                                                                 Func<TKey, IEnumerable<TElement>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedSequence<TResult>(Source.GroupBy(keySelector, elementSelector, resultSelector));
		}

		public DelayedSequence<TResult> Join<TInner, TKey, TResult>
			(IEnumerable<TInner> inner, Func<T, TKey> outerSelector, Func<TInner, TKey> innerSelector,
			 Func<T, TInner, TResult> resultSelector)
		{
			return new DelayedSequence<TResult>(Source.Join(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedSequence<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedSequence<T>(Source.OrderBy(keySelector));
		}

		public DelayedSequence<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			return new DelayedSequence<T>(Source.OrderByDescending(keySelector));
		}

		public DelayedSequence<TResult> OuterJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner,
		                                                                 Func<T, TKey> outerSelector,
		                                                                 Func<TInner, TKey> innerSelector,
		                                                                 Func<T, IEnumerable<TInner>, TResult>
			                                                                 resultSelector)
		{
			return new DelayedSequence<TResult>(Source.GroupJoin(inner, outerSelector, innerSelector, resultSelector));
		}

		public DelayedSequence<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new DelayedSequence<TResult>(Source.Select(selector));
		}

		public DelayedSequence<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
		{
			return new DelayedSequence<TResult>(Source.SelectMany(selector));
		}

		public DelayedSequence<TResult> SelectMany<TCollect, TResult>(Func<T, IEnumerable<TCollect>> manySelector,
		                                                              Func<T, TCollect, TResult> resultSelector)
		{
			return new DelayedSequence<TResult>(Source.SelectMany(manySelector, resultSelector));
		}

		public DelayedSequence<T> Skip(int count)
		{
			return new DelayedSequence<T>(Source.Skip(count));
		}

		public DelayedSequence<T> SkipWhile(Func<T, bool> predicate)
		{
			return new DelayedSequence<T>(Source.SkipWhile(predicate));
		}

		public DelayedSequence<T> Take(int count)
		{
			return new DelayedSequence<T>(Source.Take(count));
		}

		public DelayedSequence<T> TakeWhile(Func<T, bool> predicate)
		{
			return new DelayedSequence<T>(Source.TakeWhile(predicate));
		}

		public DelayedSequence<T> Where(Func<T, bool> predicate)
		{
			return new DelayedSequence<T>(Source.Where(predicate));
		}

		public DelayedSequence<T> Where(Func<T, int, bool> predicate)
		{
			return new DelayedSequence<T>(Source.Where(predicate));
		}


		protected void Commit(Sequence<T> result)
		{
			if (_cache != null) return;
			_cache = result;
		}

		/// <summary>
		/// Forces the evaluation of the delayed sequence, if it hasn't been evaluated already.
		/// </summary>
		/// <param name="delayed"></param>
		/// <returns></returns>
		public static implicit operator Sequence<T>(DelayedSequence<T> delayed)
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
			private readonly DelayedSequence<T> _parent;
			private Sequence<T> _cache;
			private bool _done;

			public CachingEnumerator(DelayedSequence<T> parent, IEnumerable<T> source)
			{
				_parent = parent;
				_cache = Sequence.Empty<T>();
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