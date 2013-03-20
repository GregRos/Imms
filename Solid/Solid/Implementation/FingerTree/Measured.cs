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

		public abstract void IterBack(Action<Measured> action);
		public abstract IEnumerator<Measured> GetEnumerator();
		public abstract Measured this[int index] { get; }
		public abstract void Iter(Action<Measured> action);
	}

	internal abstract class Measured<TObject> : Measured
		where TObject : Measured
	{

		

		protected Measured(int measure) : base(measure)
		{
			
		}

		

		public abstract TObject Reverse();


		


		public abstract void Split(int index, out TObject leftmost, out TObject rightmost);


	

		public abstract TObject Set(int index, Measured value);


		public abstract void Insert(int index, object value, out TObject leftmost, out TObject rightmost);

	}
}