using System;
using System.Collections.Generic;

namespace Funq.Abstract
{
	public interface IIterableBuilder<in TElem> : IDisposable
	{

		void Add(TElem item);

		void AddMany(IEnumerable<TElem> items);

		void EnsureCapacity(int n);
	}

}