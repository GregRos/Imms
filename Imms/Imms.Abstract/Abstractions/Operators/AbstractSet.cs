using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imms.Abstract
{
	partial class AbstractSet<TElem, TSet>
	{
		/// <summary>
		/// Adds an element to the set. Identical to <see cref="Add"/>.
		/// </summary>
		/// <param name="set">The instance to which to add.</param>
		/// <param name="item">The element to add.</param>
		/// <returns></returns>
		public static TSet operator +(AbstractSet<TElem, TSet> set, TElem item) {
			return set.Add(item);
		}


		/// <summary>
		/// Returns the union of a set and a sequence of elements. Identical to <see cref="Union(IEnumerable{TElem})"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Union with.</param>
		/// <returns></returns>
		public static TSet operator +(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Union(seq);
		}

		/// <summary>
		/// Removes an element from the set. Identical to <see cref="Remove"/>.
		/// </summary>
		/// <param name="set">The set from which to remove.</param>
		/// <param name="item">The element to remove.</param>
		/// <returns></returns>
		public static TSet operator -(AbstractSet<TElem, TSet> set, TElem item) {
			return set.Remove(item);
		}

		/// <summary>
		/// Returns the elements of the first set minus those of the second one. Identical to <see cref="Except(IEnumerable{TElem})"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Except with.</param>
		/// <returns></returns>
		public static TSet operator -(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Except(seq);
		}


		/// <summary>
		/// Returns the intersection of a set and a sequence of elements. Identical to <see cref="Intersect(IEnumerable{TElem})"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to intersect with.</param>
		/// <returns></returns>
		public static TSet operator &(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Intersect(seq);
		}

		/// <summary>
		/// Returns the symmetric difference. Identical to <see cref="Difference(IEnumerable{TElem})"/>.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="seq">The set or sequence to perform Union with.</param>
		/// <returns></returns>
		public static TSet operator ^(AbstractSet<TElem, TSet> set, IEnumerable<TElem> seq) {
			return set.Difference(seq);
		}
	}

	partial class AbstractMap<TKey, TValue, TMap> {

		/// <summary>
		/// Adds the key-value pair to the map, overwriting any existing pair. Identical to <see cref="Set(TKey,TValue)"/>.
		/// </summary>
		/// <param name="left">The map.</param>
		/// <param name="kvp">The key-value pair.</param>
		/// <returns></returns>
		public static TMap operator +(AbstractMap<TKey, TValue, TMap> left, KeyValuePair<TKey, TValue> kvp) {
			return left.Add(kvp);
		}

		/// <summary>
		/// Adds the key-value pairs to the map, overwriting any existing pairs. Identical to <see cref="SetRange"/>.
		/// </summary>
		/// <param name="left">The map.</param>
		/// <param name="kvps">The key-value pair.</param>
		/// <returns></returns>
		public static TMap operator +(AbstractMap<TKey, TValue, TMap> left, IEnumerable<KeyValuePair<TKey, TValue>> kvps) {
			return left.SetRange(kvps);
		}

		/// <summary>
		/// Removes the specified key from the map. Identical to <see cref="Remove"/>.
		/// </summary>
		/// <param name="left">The map.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static TMap operator -(AbstractMap<TKey, TValue, TMap> left, TKey key) {
			return left.Remove(key);
		}

		/// <summary>
		/// Removes a sequence of keys from the map. Identical to <see cref="RemoveRange"/>.
		/// </summary>
		/// <param name="left">The map.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static TMap operator -(AbstractMap<TKey, TValue, TMap> left, IEnumerable<TKey> key) {
			return left.RemoveRange(key);
		}

		/// <summary>
		/// Takes a sequence of key-value pairs and removes all keys present in the sequence from the map. Identical to <see cref="Subtract{T}"/>.
		/// </summary>
		/// <param name="left">The map.</param>
		/// <param name="kvps">The key-value pairs.</param>
		/// <returns></returns>
		public static TMap operator -(AbstractMap<TKey, TValue, TMap> left, IEnumerable<KeyValuePair<TKey, TValue>> kvps) {
			return left.Subtract(kvps);
		}

	}
}
