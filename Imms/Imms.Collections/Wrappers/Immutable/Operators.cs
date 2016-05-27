using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imms
{
	partial class ImmList<T> {

		/// <summary>
		/// Adds an element to the end. Identical to <see cref="AddLast"/>.
		/// </summary>
		/// <param name="left">The list to which the item will be added.</param>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, T item) {
			return left.AddLast(item);
		}
		/// <summary>
		/// Adds an element to the start Identical to <see cref="AddFirst"/>.
		/// </summary>
		/// <param name="list">The list to which the item will be added.</param>
		/// <param name="item">The item to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(T item, ImmList<T> list) {
			return list.AddFirst(item);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddLastRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="left">The list to which the item swill be added.</param>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, IEnumerable<T> items) {
			return left.AddLastRange(items);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddFirstRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="list">The list to which the item swill be added.</param>
		/// <param name="items">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(IEnumerable<T> items, ImmList<T> list) {
			return list.AddFirstRange(items);
		}

		/// <summary>
		/// Adds a sequence of elements to the end. Identical to <see cref="AddLastRange(IEnumerable{T})"/>.
		/// </summary>
		/// <param name="left">The list to which the item swill be added.</param>
		/// <param name="right">The items to add.</param>
		/// <returns></returns>
		public static ImmList<T> operator +(ImmList<T> left, ImmList<T> right) {
			return left.AddLastRange(right);
		}
	}

	partial class ImmSet<T> {
		
	}
}
