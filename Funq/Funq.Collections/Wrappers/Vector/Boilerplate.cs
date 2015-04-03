

namespace Funq.Collections
{
		using System.Globalization;
	using Funq;
	using System.Collections.Generic;
	using System;
	using Funq.Abstract;
	using Linq = System.Linq;
	using Enumerable = System.Linq.Enumerable;
	
	public partial class FunqVector<T> : AbstractSequential<T, FunqVector<T>> {
		private static readonly FunqVector<T> _instance = new FunqVector<T>();
	
		private FunqVector()
		{
		}
	
		private FunqVector<TElem2> GetPrototype<TElem2>()
		{
			return FunqVector<TElem2>._instance;
		}
	
	
		/// <summary>
		/// Applies a selector on every element of the collection.
		/// </summary>
		/// <typeparam name="TRElem">The type of the result element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqVector<TRElem> Select<TRElem>(Func<T, TRElem> selector)
		{
			if (selector == null) throw Errors.Is_null;
			return base.Select(GetPrototype<TRElem>(), selector);
		}
	
	
	
		/// <summary>
		/// Applies a selector on every element of the collection, also filtering elements
		/// based on whether the selector returns Option.Some.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqVector<TRElem> Select<TRElem>(Func<T, Option<TRElem>> selector)
		{
			if (selector == null) throw Errors.Is_null;
			return base.Choose(this.GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies a selector on every element of the collection, also filtering elements
		/// based on whether the selector returns Option.Some.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection element.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqVector<TRElem> Choose<TRElem>(Func<T, Option<TRElem>> selector)
		{
			if (selector == null) throw Errors.Is_null;
			return base.Choose(this.GetPrototype<TRElem>(), selector);
		}
	
		/// <summary>
		/// Applies a selector on every element of the collection, yielding a sequence.
		/// Adds all the elements to a single result collection.
		/// </summary>
		/// <typeparam name="TRElem">The type of the ouput collection elem.</typeparam>
		/// <param name="selector">The selector.</param>
		/// <returns></returns>
		public FunqVector<TRElem> SelectMany<TRElem>(Func<T, IEnumerable<TRElem>> selector)
		{
			if (selector == null) throw Errors.Is_null;
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
		public FunqVector<TRElem> SelectMany<TElem2, TRElem>(Func<T, IEnumerable<TElem2>> selector,
			                                                         Func<T, IEnumerable<TElem2>, TRElem> rSelector)
		{
			if (selector == null) throw Errors.Is_null;
			if (rSelector == null) throw Errors.Is_null;
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
		/// <returns></returns>
		public FunqVector<TRElem> Join<TRElem, TInner, TKey>(IEnumerable<TInner> inner, Func<T, TKey> oKeySelector,
			                                                         Func<TInner, TKey> iKeySelector, Func<T, TInner, TRElem> rSelector,
			                                                         IEqualityComparer<TKey> eq = null)
		{
			if (inner == null) throw Errors.Is_null;
			if (oKeySelector == null) throw Errors.Is_null;
			if (iKeySelector == null) throw Errors.Is_null;
			if (rSelector == null) throw Errors.Is_null;
	
			return base.Join(GetPrototype<TRElem>(), inner, oKeySelector, iKeySelector, rSelector, eq ?? EqualityComparer<TKey>.Default);
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
		/// <returns></returns>
		public FunqVector<Tuple<T,TInner>> Join<TInner, TKey>(IEnumerable<TInner> inner, Func<T, TKey> oKeySelector,
																						Func<TInner, TKey> iKeySelector, IEqualityComparer<TKey> eq = null)
		{
			if (inner == null) throw Errors.Is_null;
			if (oKeySelector == null) throw Errors.Is_null;
			if (iKeySelector == null) throw Errors.Is_null;
			Func<T,TInner, Tuple<T, TInner>> tuple = Tuple.Create;
	
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
		public FunqVector<TRElem> GroupBy<TRElem, TElem2, TKey>(Func<T, TKey> keySelector, Func<T, TElem2> valueSelector,
			                                                            Func<TKey, IEnumerable<TElem2>, TRElem> rSelector, IEqualityComparer<TKey> eq = null)
		{
			if (keySelector == null) throw Errors.Is_null;
			if (valueSelector == null) throw Errors.Is_null;
			if (rSelector == null) throw Errors.Is_null;
			return base.GroupBy(GetPrototype<TRElem>(), keySelector, valueSelector, rSelector, eq ?? EqualityComparer<TKey>.Default);
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
		public FunqVector<Tuple<TKey, IEnumerable<T>>> GroupBy<TKey>(Func<T, TKey> keySelector,  IEqualityComparer<TKey> eq = null)
		{
			if (keySelector == null) throw Errors.Is_null;
			return this.GroupBy(keySelector, x => x, Tuple.Create, eq ?? EqualityComparer<TKey>.Default);
		}
	
		public FunqVector<TRElem> Cast<TRElem>()
		{
			return base.Cast<TRElem, FunqVector<TRElem>>(GetPrototype<TRElem>());
		}
	
		public FunqVector<TRElem> OfType<TRElem>()
		{
			
			return base.OfType<TRElem, FunqVector<TRElem>>(GetPrototype<TRElem>());
		}
	
		/// <summary>
		/// Applies the specified accumulator over each element of a collection, constructing a collection from its partial results.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <param name="initial">The initial value for the accumulator.</param>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns></returns>
		public FunqVector<TRElem> Scan<TRElem>(TRElem initial, Func<TRElem, T, TRElem> accumulator)
		{
			if (accumulator == null) throw Errors.Is_null;
			return base.Scan(GetPrototype<TRElem>(), initial, accumulator);
		}
	
		/// <summary>
		/// Applies the specified accumulator over each element of a collection, constructing a collection from its partial results. The accumulator is applied from the last element to the first element.
		/// </summary>
		/// <typeparam name="TRElem">The type of the output collection elem.</typeparam>
		/// <param name="initial">The initial value for the accumulator.</param>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns></returns>
		public FunqVector<TRElem> ScanBack<TRElem>(TRElem initial, Func<TRElem, T, TRElem> accumulator)
		{
			if (accumulator == null) throw Errors.Is_null;
			return base.ScanBack(GetPrototype<TRElem>(), initial, accumulator);
		}
	
		public FunqVector<TRElem> Zip<TElem2, TRElem>(IEnumerable<TElem2> other, Func<T, TElem2, TRElem> selector)
		{
			return base.Zip(GetPrototype<TRElem>(), other, selector);
		}
	
		public FunqVector<Tuple<T, TElem2>> Zip<TElem2>(IEnumerable<TElem2> other)
		{
			return this.Zip(other, Tuple.Create);
		}
	}
	
		}