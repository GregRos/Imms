using System;
using System.Collections.Generic;
using System.Diagnostics;
using Funq.Abstract;
using Funq.Collections.Common;

namespace Funq.Collections
{
	/*
	 * This struct business called performance problems so it was scrapped.
	 */ 
	/*
	/// <summary>
	/// A struct that wraps a key together with logic determining equality and hash code.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	internal struct EquatableKey<TKey> : IEquatable<EquatableKey<TKey>>
	{
		/// <summary>
		/// The underlying key.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public readonly TKey Key;
		/// <summary>
		/// The cached hash code.
		/// </summary>
		public readonly int Hash;
		/// <summary>
		/// An equality function. This isn't provided as an IEqualityComparer due to performance issues.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public readonly Func<TKey, TKey, bool> EqualityFunction;

		public EquatableKey(TKey k, IEqualityComparer<TKey> equality)
		{
			Key = k;
			Hash = equality.GetHashCode(k);
			EqualityFunction = equality.Equals;
		}

		public override string ToString()
		{
			return Key.ToString();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				return string.Format("Key with Hash: {0}", Hash);
			}
		}

		/// <summary>
		/// Equates this key with another. Doesn't check that the equality functions are equal.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(EquatableKey<TKey> a, EquatableKey<TKey> b)
		{
			return a.EqualityFunction(a.Key, b.Key);
		}

		public static bool operator !=(EquatableKey<TKey> a, EquatableKey<TKey> b)
		{
			return !(a == b);
		}

		public bool Equals(EquatableKey<TKey> other)
		{
			return EqualityFunction(this.Key, other.Key);
		}

		public override bool Equals(object obj)
		{
			return obj is EquatableKey<TKey> && this.Equals((EquatableKey<TKey>)obj);
		}

		public override int GetHashCode()
		{
			return Hash;
		}

		/// <summary>
		/// Retrieves the underlying key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static implicit operator TKey (EquatableKey<TKey> key)
		{
			return key.Key;
		}
	}
	 * 
	/// <summary>
	/// A struct that wraps a key together with logic for comparing it to other keys.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	internal struct ComparableKey<TKey>
	{
		/// <summary>
		/// The underlying key.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public readonly TKey Key;
		/// <summary>
		/// The function used to compared two keys.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public readonly Func<TKey, TKey, Cmp> ComparisonHandler;


		public override string ToString()
		{
			return Key.ToString();
		}


		public ComparableKey(TKey key, Func<TKey, TKey, Cmp> comparisonHandler) : this()
		{
			Key = key;
			ComparisonHandler = comparisonHandler;
		}

		public ComparableKey(TKey key, IComparer<TKey> comparisonHandler)
			: this(key, (k1, k2) => comparisonHandler.Compare(k1, k2).ToCmp())
		{
			
		}
		/// <summary>
		/// Retrieves the underlying key.
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public static implicit operator TKey (ComparableKey<TKey> k)
		{
			return k.Key;
		}

		/// <summary>
		/// Compares this key to another key.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Cmp CompareTo(TKey other)
		{
			return ComparisonHandler(Key, other);
		}
	}
	 * */
}