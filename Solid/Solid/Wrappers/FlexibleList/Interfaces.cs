using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid
{
	public partial class FlexibleList<T> : IList<T>
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

		void ICollection<T>.Clear()
		{
			throw Errors.Collection_readonly;
		}

		bool ICollection<T>.Contains(T item)
		{
			return IndexOf(x => item.Equals(x)).HasValue;
		}

		/// <summary>
		///   Copies the collection into the specified array..
		/// </summary>
		/// <param name="array"> The array. </param>
		/// <param name="arrayIndex"> The index at which to start copying. </param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			ForEach(v =>
			        {
				        array[arrayIndex] = v;
				        arrayIndex++;
			        });
		}

		int IList<T>.IndexOf(T item)
		{
			var res = IndexOf(v => item.Equals(v));
			if (res.HasValue) return res.Value;
			throw Errors.Arg_out_of_range("item");
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

		void ICollection<T>.Add(T item)
		{
			throw Errors.Collection_readonly;
		}
	}
}