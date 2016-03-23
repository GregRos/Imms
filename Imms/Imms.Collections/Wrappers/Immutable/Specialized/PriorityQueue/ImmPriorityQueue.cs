using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imms.Abstract;

namespace Imms.Specialized
{
	internal struct ItemPriorityPair<T, TPriority> {
		public T Item;
		public TPriority Priority;
	}

	internal partial class ImmPriorityQueue<T, TPriority>{
		private readonly ImmSortedMap<TPriority, ImmList<T>> Inner;
		private readonly Func<T, TPriority> Appraise;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		internal ImmPriorityQueue(Func<T, TPriority> appraise, ImmSortedMap<TPriority, ImmList<T>> inner) {
			Appraise = appraise;
			Inner = inner;
		}

		private ImmPriorityQueue<T, TPriority> Wrap(ImmSortedMap<TPriority, ImmList<T>> items) {
			return new ImmPriorityQueue<T, TPriority>(Appraise, items);
		} 

		public ImmPriorityQueue<T, TPriority> Add(T item) {
			var priority = Appraise(item);
			var list = Inner.TryGet(priority);
			var newList = list.IsSome ? list.Value.AddLast(item) : ImmList.FromItems(item);

			return Wrap(Inner.Set(priority, newList));
		}

		public ImmPriorityQueue<T, TPriority> Remove(T item) {
			var priority = Appraise(item);
			var list = Inner.TryGet(priority);
			if (list.IsNone) return this;
			var itemIndex = list.Value.FindIndex(item);
			if (itemIndex.IsNone) return this;

			return Wrap(Inner.Set(priority, list.Value.RemoveAt(itemIndex.Value)));
		}

		

	}
}
