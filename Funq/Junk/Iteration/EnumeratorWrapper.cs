using System;
using System.Collections;
using System.Collections.Generic;

namespace Solid.FingerTree.Iteration
{
	//internal class EnumeratorWrapper<T> : IEnumerator<T>, IEnumerable<T>
	//{
	//	private T current;
	//	private readonly IEnumerator<Measured> inner;

	//	public EnumeratorWrapper(IEnumerator<Measured> inner)
	//	{
	//		this.inner = inner;
	//	}

	//	public T Current
	//	{
	//		get
	//		{
	//			return current;
	//		}
	//	}

	//	object IEnumerator.Current
	//	{
	//		get
	//		{
	//			return Current;
	//		}
	//	}

	//	public void Dispose()
	//	{
	//	}

	//	public IEnumerator<T> GetEnumerator()
	//	{
	//		return new EnumeratorWrapper<T>(inner);
	//	}

	//	IEnumerator IEnumerable.GetEnumerator()
	//	{
	//		return GetEnumerator();
	//	}

	//	public bool MoveNext()
	//	{
	//		var ret = inner.MoveNext();
	//		if (ret)
	//		{
	//			var tmp = inner.Current as Value<T>;
	//			current = tmp.Content;
	//			return true;
	//		}
	//		return false;
	//	}

	//	public void Reset()
	//	{
	//		throw new NotSupportedException();
	//	}
	//}
}