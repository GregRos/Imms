using System.Collections;
using System.Collections.Generic;

namespace Solid.FingerTree.Iteration
{
	internal sealed class ValueEnumerator<T> : IEnumerator<Measured>
	{
		private readonly Value<T> value;
		private bool started = false;

		public ValueEnumerator(Value<T> value)
		{
			this.value = value;
		}

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			if (!started)
			{
				started = true;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			started = false;
		}

		public Measured Current
		{
			get
			{
				return value;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}