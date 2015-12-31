

namespace Funq
{
		using System.Globalization;
	using Funq;
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using Funq.Abstract;
	using Linq = System.Linq;
	using Enumerable = System.Linq.Enumerable;
	
	public partial class FunqList<T> : AbstractSequential<T, FunqList<T>> {
		private static readonly FunqList<T> Instance = new FunqList<T>();
	
		private FunqList()
		{
		}
	
		private FunqList<TElem2> GetPrototype<TElem2>()
		{
			return FunqList<TElem2>.Instance;
		}
	
	
		/// <summary>
		/// Applies a selector on every element of the collection.
		/// </summary>
		/// <typeparam name="TRElem">The type of the result element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqList<TRElem> Select<TRElem>(Func<T, TRElem> selector)
		{
			return base.Select(GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies a selector on every element of the collection, also filtering elements
		/// based on whether the selector returns Option.Some.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqList<TRElem> Select<TRElem>(Func<T, Optional<TRElem>> selector)
		{
			return base.Choose(this.GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies a selector on every element of the collection, also filtering elements
		/// based on whether the selector returns Option.Some.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqList<TRElem> Choose<TRElem>(Func<T, Optional<TRElem>> selector)
		{
			return base.Choose(this.GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies a selector on every element of the collection, yielding a sequence.
		/// Adds all the elements to a single result collection.
		/// </summary>
		/// <typeparam name="TRElem">The type of the ouput collection elem.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqList<TRElem> SelectMany<TRElem>(Func<T, IEnumerable<TRElem>> selector)
		{
			return base.SelectMany(GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies an element selector on every element of the collection, yielding a sequence.
		/// Applies a result selector on every source element and generated sequence, yielding a result.
		/// Adds all the results to a collection.
		/// </summary>
		/// <typeparam name="TElem2">The type of the sequence element.</typeparam>
		/// <typeparam name="TRElem">The type of the result.</typeparam>
		/// <param name="selector">The element selector.</param>
		/// <param name="rSelector">The result selector.</param>
		/// <returns></returns>
		public FunqList<TRElem> SelectMany<TElem2, TRElem>(Func<T, IEnumerable<TElem2>> selector,
			                                                         Func<T, IEnumerable<TElem2>, TRElem> rSelector)
		{
			return base.SelectMany(GetPrototype<TRElem>(), selector, rSelector);
		}
	
		/// <summary>
		/// Correlates the elements of the collection with those of the specified sequence. Uses the specified equality comparer.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TInner">The type of the inner sequence.</typeparam>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <param name="inner">The inner sequence.</param>
		/// <param name="oKeySelector">The outer key selector (for elements of the current collection).</param>
		/// <param name="iKeySelector">The inner key selector (for the specified sequence).</param>
		/// <param name="rSelector">The result selector.</param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public FunqList<TRElem> Join<TRElem, TInner, TKey>(IEnumerable<TInner> inner, Func<T, TKey> oKeySelector,
			                                                         Func<TInner, TKey> iKeySelector, Func<T, TInner, TRElem> rSelector,
			                                                         IEqualityComparer<TKey> eq = null)
		{
			return base.Join(GetPrototype<TRElem>(), inner, oKeySelector, iKeySelector, rSelector, eq ?? EqualityComparer<TKey>.Default);
		}
	
		/// <summary>
		/// Correlates the elements of the collection with those of the specified sequence. Uses the specified equality comparer.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TInner">The type of the inner sequence.</typeparam>
		/// <param name="inner">The inner sequence.</param>
		/// <param name="oKeySelector">The outer key selector (for elements of the current collection).</param>
		/// <param name="iKeySelector">The inner key selector (for the specified sequence).</param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public FunqList<Tuple<T, TInner>> Join<TInner, TKey>(IEnumerable<TInner> inner, Func<T, TKey> oKeySelector,
																						Func<TInner, TKey> iKeySelector, IEqualityComparer<TKey> eq = null)
		{
			Func<T, TInner, Tuple<T, TInner>> tuple = Tuple.Create;
	
			return this.Join(inner, oKeySelector, iKeySelector, tuple, eq);
		}
	
		/// <summary>
		/// Groups the elements of the collection by key, applies a selector on each value, and then applies a result selector on each key-group pair. Uses the specified equality comparer.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <typeparam name="TElem2">The type of the elem2.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <param name="rSelector">The result selector.</param>
		/// <param name="eq">The equality comparer.</param>
		/// <returns></returns>
		public FunqList<TRElem> GroupBy<TRElem, TElem2, TKey>(Func<T, TKey> keySelector, Func<T, TElem2> valueSelector,
			                                                            Func<TKey, IEnumerable<TElem2>, TRElem> rSelector, IEqualityComparer<TKey> eq = null)
		{
			return base.GroupBy(GetPrototype<TRElem>(), keySelector, valueSelector, rSelector, eq ?? EqualityComparer<TKey>.Default);
		}
	
		/// <summary>
		/// Groups the elements of the collection by key, applies a selector on each value, and then applies a result selector on each key-group pair. Uses the specified equality comparer.
		/// </summary>
		/// <typeparam>The type of the key.
		///     <name>TKey</name>
		/// </typeparam>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="eq">The equality comparer.</param>
		/// <returns></returns>
		public FunqList<Tuple<TKey, IEnumerable<T>>> GroupBy<TKey>(Func<T, TKey> keySelector,  IEqualityComparer<TKey> eq = null)
		{
			return this.GroupBy(keySelector, x => x, Tuple.Create, eq ?? EqualityComparer<TKey>.Default);
		}
	
		public FunqList<TRElem> Cast<TRElem>()
		{
			return base.Cast<TRElem, FunqList<TRElem>>(GetPrototype<TRElem>());
		}
	
		/// <summary>
		/// Applies the specified accumulator over each element of a collection, constructing a collection from its partial results.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <param name="initial">The initial value for the accumulator.</param>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns></returns>
		public FunqList<TRElem> Scan<TRElem>(TRElem initial, Func<TRElem, T, TRElem> accumulator)
		{
			return base.Scan(GetPrototype<TRElem>(), initial, accumulator);
		}
	
		/// <summary>
		/// Applies the specified accumulator over each element of a collection, constructing a collection from its partial results. The accumulator is applied from the last element to the first element.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <param name="initial">The initial value for the accumulator.</param>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns></returns>
		public FunqList<TRElem> ScanBack<TRElem>(TRElem initial, Func<TRElem, T, TRElem> accumulator)
		{
			return base.ScanBack(GetPrototype<TRElem>(), initial, accumulator);
		}
	
		public FunqList<TRElem> Zip<TElem2, TRElem>(IEnumerable<TElem2> other, Func<T, TElem2, TRElem> selector)
		{
			return base.Zip(GetPrototype<TRElem>(), other, selector);
		}
	
		public FunqList<Tuple<T, TElem2>> Zip<TElem2>(IEnumerable<TElem2> other)
		{
			return this.Zip(other, Tuple.Create);
		}
	
	}
	
		}