using System.Collections.Generic;

// ReSharper disable AccessToDisposedClosure

namespace Funq.Abstract
{
	/// <summary>
	///   The parent class of collection builders that allow for iteration.
	/// </summary>
	/// <typeparam name="TElem"> The type of the element contained in the collection. </typeparam>
	public abstract class IterableBuilder<TElem> : IIterableBuilder<TElem>
	{
		/// <summary>
		/// Returns true if the builder has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns the result produced by the builder. May cause a collection to be constructed.
		/// </summary>
		public abstract object Result
		{
			get;
		}


		public void Add(TElem item)
		{
			if (IsDisposed) throw Errors.Is_disposed("SeqBuilder");
			add(item);
		}

		public virtual void AddMany(IEnumerable<TElem> items)
		{
			foreach (var item in items) Add(item);
		}

		public virtual void Dispose()
		{
			if (IsDisposed) throw Errors.Is_disposed("SeqBuilder");
			IsDisposed = true;
		}

		/// <summary>
		/// This method implements the actual addition operation.
		/// </summary>
		/// <param name="item">The item to add.</param>
		protected abstract void add(TElem item);
	}
}