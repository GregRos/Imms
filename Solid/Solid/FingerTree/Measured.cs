using System;
using System.Collections.Generic;

namespace Solid.FingerTree
{
	internal abstract class Measured
	{

		public readonly int Measure;

		protected Measured(int measure)
		{
			Measure = measure;
		}

		public abstract Measured this[int index] { get; }

		public abstract TObject Reverse<TObject>()
			where TObject : Measured;

		public abstract void IterBack(Action<Measured> action);

		public abstract IEnumerator<Measured> GetEnumerator();


		public abstract void Split<TObject>(int index, out TObject leftmost, out TObject rightmost)
			where TObject : Measured;


		public abstract void Iter(Action<Measured> action);
	}
}