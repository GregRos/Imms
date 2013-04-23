using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid
{
	//This file implements some interfaces for .NET compatibility.
	public partial class Vector<T> : IList<T>
	{
		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw Errors.Collection_readonly;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<T>.Clear()
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<T>.Contains(T item)
		{
			return IndexOf(v => item.Equals(v)).HasValue;
		}

		int IList<T>.IndexOf(T item)
		{
			var found = IndexOf(v => item.Equals(v));
			if (found.HasValue)
			{
				return found.Value;
			}
			throw Errors.Arg_out_of_range("item", -1);
		}

		void IList<T>.Insert(int index, T item)
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<T>.Remove(T item)
		{
			throw Errors.Collection_readonly;
		}

		void IList<T>.RemoveAt(int index)
		{
			throw Errors.Collection_readonly;
		}
	}
}