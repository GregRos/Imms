using System;
using System.Collections.Generic;

namespace Solid
{
	static partial class FingerTree<TValue>
	{
		internal interface IReusableEnumerator<in TOver> : IEnumerator<Leaf<TValue>>
			where TOver : Measured<TOver>
		{
			void Retarget(TOver next);
		}

		internal abstract class Measured<TObject>
			where TObject : Measured<TObject>
		{
			public readonly int Measure;

			protected Measured(int measure)
			{
				Measure = measure;
			}

			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			public abstract void Fuse(TObject after, out TObject firstRes, out TObject lastRes);

			public abstract IReusableEnumerator<TObject> GetEnumerator(bool forward);

			public abstract void Insert(int index, Leaf<TValue> leaf, out TObject value, out TObject rightmost1);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> action);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> action);

			public abstract TObject Remove(int index);

			public abstract TObject Reverse();

			public abstract TObject Set(int index, Leaf<TValue> leaf);

			public abstract void Split(int index, out TObject leftmost, out TObject rightmost);
		}
	}
}