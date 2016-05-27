using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imms.Abstract
{
	partial class AbstractIterable<TElem, TIterable, TBuilder>
	{

		/// <summary>
		/// (Implementation) Cartesian product between two iterable collections. You can supply a selector to apply on the results. The current instance is the left operand of the product.
		/// </summary>
		/// <typeparam name="TRight">The type of element in the second collection.</typeparam>
		/// <typeparam name="TOut">The type of the output element.</typeparam>
		/// <typeparam name="TOIter">The type of the output collection.</typeparam>
		/// <param name="bFactory">A prototype instance of the resulting collection provider, used as a builder factory.</param>
		/// <param name="right">The right operand of the cartesian product. Must be an iterable collection. </param>
		/// <param name="selector">A selector.</param>
		/// <remarks>
		/// This method basically applies a function on every combination of elements from the left operand and the right operand.
		/// </remarks>
		/// <returns></returns>
		protected virtual TOIter _Cartesian<TRight, TOut, TOIter>(TOIter bFactory, IEnumerable<TRight> right,
																   Func<TElem, TRight, TOut> selector)
			where TOIter : IBuilderFactory<IIterableBuilder<TOut, TOIter>>
		{
			bFactory.CheckNotNull("bFactory");
			right.CheckNotNull("right");
			selector.CheckNotNull("selector");
			if (right is IAnyIterable<TElem>)
			{
				return _Cartesian(bFactory, (IAnyIterable<TRight>)right, selector);
			}
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(x =>
						{
							foreach (var item in right)
							{
								builder.Add(selector(x, item));
							}
						});
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) Casts every element of the collection to a different type.
		/// </summary>
		/// <typeparam name="TOut">The type of the output element.</typeparam>
		/// <typeparam name="TOIter">The type of the collection returned by the method.</typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection used as a builder factory. </param>
		/// <returns> </returns>
		protected virtual TOIter _Cast<TOut, TOIter>(TOIter bFactory)
			where TOIter : IBuilderFactory<IIterableBuilder<TOut, TOIter>>
		{
			return _Select(bFactory, (v) => (TOut)(object)v);
		}

		/// <summary>
		///  (Implementation) Applies a selector on every item in the collection, discarding all items for which the selector returns None.
		/// </summary>
		/// <typeparam name="TOut"> The value type of the resulting collection. </typeparam>
		/// <typeparam name="TOutIter"> The type of collection returned by the method. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection, used as a builder factory. </param>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		protected virtual TOutIter _Choose<TOut, TOutIter>(TOutIter bFactory, Func<TElem, Optional<TOut>> selector)
			where TOutIter : IBuilderFactory<IIterableBuilder<TOut, TOutIter>>
		{
			bFactory.CheckNotNull("bFactory");
			selector.CheckNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach((v) =>
						{
							var res = selector(v);
							if (res.IsSome)
								builder.Add(res.Value);
						});
				return builder.Produce();
			}
		}


		/// <summary>
		/// (Implementation) The GroupBy operator. Results are returned as a concrete map type, and group elements are stored as an iterator.
		/// </summary>
		/// <typeparam name="TOutMap">The type of the output map.</typeparam>
		/// <typeparam name="TElem2">The type of intermediate value returned by the value selector.</typeparam>
		/// <typeparam name="TOut">The type of the output element.</typeparam>
		/// <typeparam name="TKey">The type of the key used to perform the grouping.</typeparam>
		/// <param name="bFactory">A prototype instance of the resulting collection, used as a builder factory.</param>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <param name="resultSelector">The result selector.</param>
		/// <param name="eq">The eq.</param>
		/// <returns></returns>
		protected virtual TOutMap _GroupBy<TOutMap, TElem2, TOut, TKey>(
			TOutMap bFactory, Func<TElem, TKey> keySelector,
			Func<TElem, TElem2> valueSelector,
			Func<TKey, IEnumerable<TElem2>, TOut> resultSelector,
			IEqualityComparer<TKey> eq = null)
			where TOutMap : IBuilderFactory<IIterableBuilder<TOut, TOutMap>>
		{
			keySelector.CheckNotNull("keySelector");
			valueSelector.CheckNotNull("valueSelector");
			resultSelector.CheckNotNull("resultSelector");
			eq = eq ?? FastEquality<TKey>.Default;
			using (var builder = bFactory.EmptyBuilder)
			{
				var groups = new Dictionary<TKey, List<TElem2>>(eq);

				ForEach(x =>
				{
					var key = keySelector(x);
					if (!groups.ContainsKey(key))
						groups[key] = new List<TElem2>();
					groups[key].Add(valueSelector(x));
				});
				foreach (var kvp in groups)
				{
					var myGrouping = resultSelector(kvp.Key, kvp.Value);
					builder.Add(myGrouping);
				}
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) The GroupBy operator. Results are returned as a concrete key-value map, and group elements are stored as a concrete collection type.
		/// </summary>
		/// <typeparam name="TROuterMap">The type of the return map.</typeparam>
		/// <typeparam name="TRInnerSeq">The type of the iterable collection used to store group elements.</typeparam>
		/// <typeparam name="TElem2">The return type of the value selector.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="mapFactory">A prototype instance of the returned key-value map, used as a builder factory.</param>
		/// <param name="seqFactory">A prototype instance of the grouping iterable collection, used as a builder factory.</param>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <param name="eq">The eq.</param>
		/// <returns></returns>
		protected virtual TROuterMap _GroupBy<TROuterMap, TRInnerSeq, TElem2, TKey>(
			TROuterMap mapFactory, TRInnerSeq seqFactory, Func<TElem, TKey> keySelector, Func<TElem, TElem2> valueSelector, IEqualityComparer<TKey> eq)
			where TROuterMap : IBuilderFactory<IMapBuilder<TKey, TRInnerSeq, TROuterMap>>
			where TRInnerSeq : IBuilderFactory<IIterableBuilder<TElem2, TRInnerSeq>>
		{
			return _GroupBy(mapFactory, keySelector, valueSelector, (k, vs) =>
																   {
																	   var theSeq = seqFactory.ToIterable(vs);
																	   return Kvp.Of(k, theSeq);
																   }, eq);
		}



		/// <summary>
		/// (Implemnetation) The GroupJoin operator. Results are returned as a key-value map, and group elements are stored as an iterator.
		/// </summary>
		/// <typeparam name="TOutIter"> The type of the return collection. </typeparam>
		/// <typeparam name="TOut"> The type of element in the return collection. </typeparam>
		/// <typeparam name="TInner"> The type of element in the inner sequence. </typeparam>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="inner"> The inner sequence. </param>
		/// <param name="oKeySelector"> The outer key selector. </param>
		/// <param name="iKeySelector"> The inner key selector. </param>
		/// <param name="rSelector"> The return element selector. </param>
		/// <param name="eq"> The equality comparer. </param>
		/// <returns> </returns>
		protected virtual TOutIter _GroupJoin<TOut, TOutIter, TInner, TKey>(TOutIter bFactory, IEnumerable<TInner> inner,
																							Func<TElem, TKey> oKeySelector, Func<TInner, TKey> iKeySelector,
																							Func<TElem, IEnumerable<TInner>, TOut> rSelector,
																							IEqualityComparer<TKey> eq = null)
			where TOutIter : IBuilderFactory<IIterableBuilder<TOut, TOutIter>>
		{
			bFactory.CheckNotNull("bFactory");
			inner.CheckNotNull("inner");
			oKeySelector.CheckNotNull("oKeySelector");
			iKeySelector.CheckNotNull("iKeySelector");
			rSelector.CheckNotNull("rSelector");
			eq = eq ?? FastEquality<TKey>.Default;
			using (var builder = bFactory.EmptyBuilder)
			{
				var dict = new Dictionary<TKey, List<TInner>>(eq);
				foreach (var item in inner)
				{
					var k = iKeySelector(item);
					dict[k] = dict[k] ?? new List<TInner>();
					dict[k].Add(item);
				}
				ForEach(v =>
				{
					var k = oKeySelector(v);
					var ins = dict[k];
					builder.Add(rSelector(v, ins));
				});
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) implementation of the Join operator that returns a concrete collection type.
		/// </summary>
		/// <typeparam name="TOut"> The type of element in the return sequential collection. </typeparam>
		/// <typeparam name="TOutIter"> The type of provider returned </typeparam>
		/// <typeparam name="TInner"> The type of element of the inner sequence. </typeparam>
		/// <typeparam name="TKey"> The type of the key used to perform the join. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="inner"> The inner sequence. </param>
		/// <param name="oKeySelector"> The outer key selector. </param>
		/// <param name="iKeySelector"> The inner key selector. </param>
		/// <param name="rSelector"> The return element selector. </param>
		/// <param name="eq"> The equality comparer. </param>
		/// <returns> </returns>
		protected virtual TOutIter _Join<TOut, TOutIter, TInner, TKey>(TOutIter bFactory, IEnumerable<TInner> inner, Func<TElem, TKey> oKeySelector,
																					 Func<TInner, TKey> iKeySelector, Func<TElem, TInner, TOut> rSelector,
																					 IEqualityComparer<TKey> eq)
			where TOutIter : IBuilderFactory<IIterableBuilder<TOut, TOutIter>>
		{
			bFactory.CheckNotNull("bFactory");
			inner.CheckNotNull("inner");
			oKeySelector.CheckNotNull("oKeySelector");
			iKeySelector.CheckNotNull("iKeySelector");
			rSelector.CheckNotNull("rSelector");
			using (var builder = bFactory.EmptyBuilder)
			{
				var dict = new Dictionary<TKey, List<TInner>>(eq);
				foreach (var item in inner)
				{
					var k = iKeySelector(item);
					if (dict.ContainsKey(k))
						dict[k].Add(item);
					else
						dict[k] = new List<TInner> { item };
				}
				ForEach(v =>
				{
					var k = oKeySelector(v);
					if (!dict.ContainsKey(k))
						return;
					var ins = dict[k];
					ins.ForEach(u => builder.Add(rSelector(v, u)));
				});
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) Constructs a sequential collection of the specified type by sorting the elements of the current collection using the specified comparer.
		/// </summary>
		/// <typeparam name="TRList"></typeparam>
		/// <param name="bFactory"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		protected virtual TRList _OrderBy<TRList>(TRList bFactory, IComparer<TElem> comparer)
			where TRList : IBuilderFactory<ISequentialBuilder<TElem, TRList>>
		{
			bFactory.CheckNotNull("bFactory");
			comparer.CheckNotNull("comparer");
			var arr = ToArray();
			Array.Sort(arr, comparer);
			using (var builder = bFactory.EmptyBuilder)
			{
				Array.ForEach(arr, x => builder.Add(x));
				return builder.Produce();
			}
		}

		/// <summary>
		///  (Implementation) Incrementally applies an accumulator over the collection, and returns a sequence of partial results.
		/// </summary>
		/// <typeparam name="TElem2"> The type of the element in the return collection. </typeparam>
		/// <typeparam name="TRSeq"> The type of the return provider. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="initial"> The initial value for the accumulator. </param>
		/// <param name="accumulator"> The accumulator. </param>
		/// <returns> </returns>
		protected virtual TRSeq _Scan<TElem2, TRSeq>(TRSeq bFactory,
													TElem2 initial,
													Func<TElem2, TElem, TElem2> accumulator)
			where TRSeq : IBuilderFactory<IIterableBuilder<TElem2, TRSeq>>
		{
			bFactory.CheckNotNull("bFactory");
			accumulator.CheckNotNull("accumulator");
			using (var builder = bFactory.EmptyBuilder)
			{
				Aggregate(initial, (r, v) =>
								   {
									   r = accumulator(r, v);
									   builder.Add(r);
									   return r;
								   });
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) The SelectMany operation. Returns a concrete collection type.
		/// </summary>
		/// <typeparam> The type of the return provider. <name>TRProvider</name> </typeparam>
		/// <typeparam name="TOutIter"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="selector"> The selector. </param>
		/// <param name="bFactory"> </param>
		/// <returns> </returns>
		protected virtual TOutIter _SelectMany<TOut, TOutIter>(
	TOutIter bFactory, Func<TElem, IEnumerable<TOut>> selector)
			where TOutIter : IBuilderFactory<IIterableBuilder<TOut, TOutIter>>
		{
			bFactory.CheckNotNull("bFactory");
			selector.CheckNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v =>
						{
							var r = selector(v);
							builder.AddRange(r);
						});
				return builder.Produce();
			}
		}

		/// <summary>
		/// (Implementation) The SelectMany operation. Returns a concrete collection of the desired type.
		/// </summary>
		/// <typeparam name="TRElem"> The type of the return element. </typeparam>
		/// <typeparam name="TRSeq"> The type of the return provider. </typeparam>
		/// <typeparam name="TElem2"> The type of sequence returned by the selector. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="selector"> The selector. </param>
		/// <param name="rSelector"> </param>
		/// <returns> </returns>
		protected virtual TRSeq _SelectMany<TRElem, TRSeq, TElem2>(TRSeq bFactory, Func<TElem, IEnumerable<TElem2>> selector,
																  Func<TElem, TElem2, TRElem> rSelector)
			where TRSeq : IBuilderFactory<IIterableBuilder<TRElem, TRSeq>>
		{
			bFactory.CheckNotNull("bFactory");
			selector.CheckNotNull("selector");
			rSelector.CheckNotNull("rSelector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v =>
						{
							var vs = selector(v);
							vs.ForEach(item => {
								builder.Add(rSelector(v, item));
							});
						});
				return builder.Produce();
			}
		}

	}
}
