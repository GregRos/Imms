using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Funq.Collections.Common;
using Funq.Collections;

namespace Funq.Collections
{
	//This file implements some interfaces for .NET compatibility.
	public partial class FunqVector<T> : IList<T>, IReadOnlyList<T>, IList {
		void IList.RemoveAt(int index) {
			throw Errors.Collection_readonly;
		}

		object IList.this[int index] {
			get { return this[index]; }
			set { throw Errors.Collection_readonly; }
		}

		bool IList.IsReadOnly {
			get { return true; }
		}

		bool IList.IsFixedSize {
			get { return true; }
		}

		T IList<T>.this[int index]
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

		void ICollection.CopyTo(Array array, int index) {
			this.CopyTo((T[]) array, index);
		}

		int ICollection.Count {
			get { return this.Length; }
		}

		object ICollection.SyncRoot {
			get { return root; }
		}

		bool ICollection.IsSynchronized {
			get { return true; }
		}

		int ICollection<T>.Count {get
		{
			return Length;
		}}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		int IList.Add(object value) {
			throw Errors.Collection_readonly;
		}

		bool IList.Contains(object value) {
			return base.Any(x => value.Equals(x));
		}

		void IList.Clear() {
			throw Errors.Collection_readonly;
		}

		int IList.IndexOf(object value) {
			return base.FindIndex(x => value.Equals(x));
		}

		void IList.Insert(int index, object value) {
			throw Errors.Collection_readonly;
		}

		void IList.Remove(object value) {
			throw Errors.Collection_readonly;
		}

		void ICollection<T>.Clear()
		{
			throw Funq.Errors.Collection_readonly;
		}

		int IList<T>.IndexOf(T item)
		{
			return base.FindIndex(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		bool ICollection<T>.Remove(T item)
		{
			throw Funq.Errors.Collection_readonly;
		}

		void IList<T>.RemoveAt(int index)
		{
			throw Funq.Errors.Collection_readonly;
		}

		protected override IEnumerator<T> GetEnumerator()
		{
			return root.GetEnumerator(true);
		}

		int IReadOnlyCollection<T>.Count {
			get {
				return this.Length;
			}
		}
	}
}