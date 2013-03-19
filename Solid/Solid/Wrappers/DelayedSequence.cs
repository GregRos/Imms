using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid
{
	internal struct DelayedSequence<T>
	{
		public readonly IEnumerable<T> Inner;

		public DelayedSequence(IEnumerable<T> inner)
		{
			Inner = inner;
		}

		private void ForEach(Action<T> action)
		{
			foreach (var item in Inner)
			{
				action(item);
			}
		}

		public DelayedSequence<T> Where(Func<T, bool> predicate)
		{
			return new DelayedSequence<T>(Inner.Where(predicate));
		}

		public DelayedSequence<TOut> Select<TOut>(Func<T, TOut> transform)
		{
			return new DelayedSequence<TOut>(Inner.Select(transform));
		}

		public DelayedSequence<TOut> SelectMany<TOut>(Func<T, IEnumerable<TOut>> unfold)
		{
			return new DelayedSequence<TOut>(Inner.SelectMany(unfold));
		}

		public DelayedSequence<TOut> SelectMany<TCol, TOut>(Func<T, IEnumerable<TCol>> project, Func<T, TCol, TOut> selector)
		{
			return new DelayedSequence<TOut>(Inner.SelectMany(project, selector));
		}

		public static implicit operator Sequence<T>(DelayedSequence<T> delayed)
		{
			var output = Sequence<T>.Empty;
			delayed.ForEach(v => output = output.AddLast(v));
			return output;
		}
	}
}