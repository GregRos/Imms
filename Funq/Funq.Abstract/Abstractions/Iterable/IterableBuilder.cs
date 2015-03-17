using System.Collections.Generic;

// ReSharper disable AccessToDisposedClosure

namespace Funq.Abstract
{
	/// <summary>
	///   The parent class of collection builders.
	/// </summary>
	/// <typeparam name="Seq"> The type of the sequential collection constructed by this instance. </typeparam>
	/// <typeparam name="Elem"> The type of the element contained in the collection. </typeparam>
	public abstract class IterableBuilder<Elem> : IIterableBuilder<Elem>
	{
		public bool IsDisposed
		{
			get;
			private set;
		}

		public abstract object Result
		{
			get;
		}

		public void Add(Elem item)
		{
			if (IsDisposed) throw Errors.Is_disposed("SeqBuilder");
			add(item);
		}

		public virtual void AddMany(IEnumerable<Elem> items)
		{
			foreach (var item in items) Add(item);
		}

		public virtual void Dispose()
		{
			if (IsDisposed) throw Errors.Is_disposed("SeqBuilder");
			IsDisposed = true;
		}

		public abstract void EnsureCapacity(int n);

		protected abstract void add(Elem item);
	}
}