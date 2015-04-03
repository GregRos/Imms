using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	public abstract partial class AbstractSequential<TElem, TList> : IList<TElem>, IList {
		void ICollection.CopyTo(Array array, int index)
		{
			if (array.GetType() != typeof(TElem[]))
			{
				throw Errors.Invalid_type_conversion;
			}
			this.CopyTo((TElem[])array, index);
		}

		int ICollection.Count
		{
			get { return this.Length; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		bool ICollection.IsSynchronized
		{
			get { return true; }
		}

		int ICollection<TElem>.Count
		{
			get
			{
				return this.Length;
			}
		}

		void IList.RemoveAt(int index)
		{
			throw Errors.Collection_readonly;
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { throw Errors.Collection_readonly; }
		}

		bool IList.IsReadOnly
		{
			get { return true; }
		}

		bool IList.IsFixedSize
		{
			get { return true; }
		}

		TElem IList<TElem>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw Funq.Errors.Collection_readonly;
			}
		}

		bool ICollection<TElem>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<TElem>.Add(TElem item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		int IList.Add(object value)
		{
			throw Errors.Collection_readonly;
		}

		bool IList.Contains(object value)
		{
			return base.Any(x => value.Equals(x));
		}

		void IList.Clear()
		{
			throw Errors.Collection_readonly;
		}

		int IList.IndexOf(object value)
		{
			return FindIndex(x => value.Equals(x));
		}

		void IList.Insert(int index, object value)
		{
			throw Errors.Collection_readonly;
		}

		void IList.Remove(object value)
		{
			throw Errors.Collection_readonly;
		}

		void ICollection<TElem>.Clear()
		{
			throw Funq.Errors.Collection_readonly;
		}

		int IList<TElem>.IndexOf(TElem item)
		{
			return FindIndex(item);
		}

		bool ICollection<TElem>.Contains(TElem item)
		{
			return false;
		}

		/// <summary>
		///   Copies the collection into the specified array..
		/// </summary>
		/// <param name="array"> The array. </param>
		/// <param name="arrayIndex"> The index at which to start copying. </param>
		public void CopyTo(TElem[] array, int arrayIndex)
		{
			CopyTo(array, 0, arrayIndex, Length);
		}


		void IList<TElem>.Insert(int index, TElem item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		bool ICollection<TElem>.Remove(TElem item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		void IList<TElem>.RemoveAt(int index)
		{
			throw Funq.Errors.Collection_readonly;
		}

		int IReadOnlyCollection<TElem>.Count
		{
			get
			{
				return Length;
			}
		}
	}
}
