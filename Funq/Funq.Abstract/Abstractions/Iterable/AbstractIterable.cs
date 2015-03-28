using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Enumerable = System.Linq.Enumerable;
#pragma warning disable 279
namespace Funq.Abstract
{
	
	/// <summary>
	///   A parent class for collections of all sorts (ordered, unordered, set-like, dictionary-like) that support a form of iteration.
	/// </summary>
	/// <typeparam name="TElem"> he type of element stored in the collection. </typeparam>
	/// <typeparam name="TIterable">This is a self-reference to the class that implements this trait.</typeparam>
	/// <typeparam name="TBuilder"> The type of collection builder used by the collection that implements this trait. </typeparam>
	public abstract partial class AbstractIterable<TElem, TIterable, TBuilder> 
		: IAnyBuilderFactory<TElem, TBuilder>, IReadOnlyCollection<TElem> where TBuilder : IterableBuilder<TElem>
		where TIterable : AbstractIterable<TElem, TIterable, TBuilder>
	{
		/// <summary>
		///   Implicit conversion into the generic provider type.
		/// </summary>
		/// <param name="o"> The o. </param>
		/// <returns> </returns>
		public static implicit operator TIterable(AbstractIterable<TElem, TIterable, TBuilder> o)
		{
			return (TIterable) (object) o;
		}


		/// <summary>
		/// (Implementation) Constructs an iterable sequence of the specified type from an iterator.
		/// </summary>
		/// <typeparam name="TRSeq"></typeparam>
		/// <typeparam name="TRElem"></typeparam>
		/// <param name="bFactory"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		protected static TRSeq ToIterable<TRSeq, TRElem>(TRSeq bFactory, IEnumerable<TRElem> items)
			where TRSeq : IAnyBuilderFactory<TRElem, IterableBuilder<TRElem>> {
			bFactory.IsNotNull("bFactory");
			items.IsNotNull("items");
			using (var builder = bFactory.EmptyBuilder)
			{
				foreach (var item in items) builder.Add(item);
				return (TRSeq) bFactory.IterableFrom(builder);
			}
		}

		protected internal abstract TBuilder EmptyBuilder
		{
			get;
		}

		public virtual void CopyTo(TElem[] arr, int arrStart, int count) {
			int i = 0;
			this.ForEachWhile(x => {
				arr[arrStart + i] = x;
				i++;
				return i < count;
			});
		}

		TBuilder IAnyBuilderFactory<TElem, TBuilder>.EmptyBuilder
		{
			get
			{
				return EmptyBuilder;
			}
		}

		


		/// <summary>
		///   Returns true if the underlying colleciton is empty, and false otherwise.
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				return !ForEachWhile(v => false);
			}
		}

		/// <summary>
		///   Returns the number of elements in the collection.
		/// </summary>
		public virtual int Length
		{
			get
			{
				return Count(x => true);
			}
		}

		/// <summary>
		///   Applies an accumulator on every item in the collection, with the specified initial value
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="initial"> The initial value. </param>
		/// <param name="accumulator"> The accumulator. </param>
		/// <returns> </returns>
		public TResult Aggregate<TResult>(TResult initial, Func<TResult, TElem, TResult> accumulator) {
			accumulator.IsNotNull("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///   Determines if all the items in the collection fulfill the specified predicate.
		/// </summary>
		/// <param name="pred"> The pred. </param>
		/// <returns> </returns>
		public bool All(Func<TElem, bool> pred)
		{
			pred.IsNotNull("pred");
			return Find(x => !pred(x)).IsNone;
		}

		/// <summary>
		///   Returns true if any item in the collection fulfills the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public bool Any(Func<TElem, bool> predicate)
		{
			predicate.IsNotNull("predicate");
			return !ForEachWhile(v => !predicate(v));
		}
		
		protected internal abstract TBuilder BuilderFrom(TIterable provider);

		TBuilder IAnyBuilderFactory<TElem, TBuilder>.BuilderFrom(IAnyIterable<TElem> provider)
		{
			provider.IsNotNull("provider");
			return BuilderFrom(provider as TIterable);
		}

		/// <summary>
		/// Returns a string representation of the elements of this collection.
		/// </summary>
		/// <param name="sep">A seperator character.</param>
		/// <param name="printFunc">Optionally, a function that converts every element to its string representation. </param>
		/// <returns></returns>
		public virtual string Print(string sep = "; ",Func<TElem, string> printFunc = null)
		{
			printFunc = printFunc ?? (x => x.ToString());
			var joined = string.Join(sep, Enumerable.Select(this, printFunc));
			return string.Format("{{{0}}}", joined);
		}

		/// <summary>
		/// (Implementation) Cartesian product between two iterable collections. You can supply a selector to apply on the result. The current instance is the left operand of the product.
		/// </summary>
		/// <typeparam name="TRight">The type of element in the second collection.</typeparam>
		/// <typeparam name="TOut">The type of the output element.</typeparam>
		/// <typeparam name="TOutIter">The type of the output collection.</typeparam>
		/// <param name="bFactory">A prototype instance of the resulting collection provider, used as a builder factory.</param>
		/// <param name="right">The right operand of the cartesian product. Must be an iterable collection. </param>
		/// <param name="selector">A selector.</param>
		/// <remarks>
		/// This method basically applies a function on every combination of elements from the left operand and the right operand.
		/// </remarks>
		/// <returns></returns>
		protected virtual TOutIter Cartesian<TRight, TOut, TOutIter>(TOutIter bFactory, IAnyIterable<TRight> right,
		                                                           Func<TElem, TRight, TOut> selector)
			where TOutIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>>
		{
			bFactory.IsNotNull("bFactory");
			right.IsNotNull("right");
			selector.IsNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(x => { right.ForEach(y => { builder.Add(selector(x, y)); }); });
				return (TOutIter) bFactory.IterableFrom(builder);
			}
		}

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
		protected virtual TOIter Cartesian<TRight, TOut, TOIter>(TOIter bFactory, IEnumerable<TRight> right,
		                                                           Func<TElem, TRight, TOut> selector)
			where TOIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>> {
			bFactory.IsNotNull("bFactory");
			right.IsNotNull("right");
			selector.IsNotNull("selector");
			if (right is IAnyIterable<TElem>) {
				return Cartesian(bFactory, (IAnyIterable<TRight>) right, selector);
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
				return (TOIter) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		/// (Implementation) Casts every element of the collection to a different type.
		/// </summary>
		/// <typeparam name="TOut">The type of the output element.</typeparam>
		/// <typeparam name="TOIter">The type of the collection returned by the method.</typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection used as a builder factory. </param>
		/// <returns> </returns>
		protected virtual TOIter Cast<TOut, TOIter>(TOIter bFactory)
			where TOIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>>
		{
			return Select(bFactory, (v) => (TOut) (object) v);
		}

		/// <summary>
		///  (Implementation) Applies a selector on every item in the collection, discarding all items for which the selector returns None.
		/// </summary>
		/// <typeparam name="TOut"> The value type of the resulting collection. </typeparam>
		/// <typeparam name="TOutIter"> The type of collection returned by the method. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection, used as a builder factory. </param>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		protected virtual TOutIter Choose<TOut, TOutIter>(TOutIter bFactory, Func<TElem, Option<TOut>> selector)
			where TOutIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach((v) =>
				        {
					        var res = selector(v);
					        if (res.IsSome) builder.Add(res.Value);
				        });
				return (TOutIter) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		///   Returns the number of items that satisfy a given predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public int Count(Func<TElem, bool> predicate) {
			predicate.IsNotNull("predicate");
			return Aggregate(0, (r, v) => predicate(v) ? r + 1 : r);
		}

		/// <summary>
		///   Returns a collection consisting of a single specified default value if the current collection is empty.
		/// </summary>
		/// <param name="default"> The default. </param>
		/// <returns> </returns>
		public virtual TIterable DefaultIfEmpty(TElem @default)
		{
			if (IsEmpty)
				using (var builder = EmptyBuilder)
				{
					builder.Add(@default);
					return ProviderFrom(builder);
				}
			else return this;
		}

		/// <summary>
		///   Finds the first item that matches the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public Option<TElem> Find(Func<TElem, bool> predicate)
		{
		    if (predicate == null) throw Errors.Argument_null("predicate");
			var item = Option.NoneOf<TElem>();
			ForEachWhile((v) =>
			             {
				             if (predicate(v))
				             {
					             item = v;
					             return false;
				             }
				             return true;
			             });
			return item;
		}

		/// <summary>
		///   Applies the specified delegate on every item in the collection, from first to last.
		/// </summary>
		/// <param name="action"> The action. </param>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public virtual void ForEach(Action<TElem> action)
		{
		    if (action == null) throw Errors.Argument_null("action");
			ForEachWhile(x =>
			             {
				             action(x);
				             return true;
			             });
		}

		/// <summary>
		///   Applies the specified delegate on every item in the collection, from last to first.
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public virtual bool ForEachWhile(Func<TElem, bool> iterator)
		{
		    if (iterator == null) throw Errors.Argument_null("iterator");
			foreach (var item in this)
			{
				if (!iterator(item)) return false;
			}
			return true;
		}

		protected abstract IEnumerator<TElem> GetEnumerator();

		IEnumerator<TElem> IEnumerable<TElem>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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
		protected virtual TOutMap GroupBy<TOutMap, TElem2, TOut, TKey>(
			TOutMap bFactory, Func<TElem, TKey> keySelector,
			Func<TElem, TElem2> valueSelector,
			Func<TKey, IEnumerable<TElem2>, TOut> resultSelector,
			IEqualityComparer<TKey> eq = null)
			where TOutMap : IAnyBuilderFactory<TOut, IterableBuilder<TOut>>
		{
			keySelector.IsNotNull("keySelector");
			valueSelector.IsNotNull("valueSelector");
			resultSelector.IsNotNull("resultSelector");
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
				return (TOutMap)bFactory.IterableFrom(builder);
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
		protected virtual TROuterMap GroupBy<TROuterMap, TRInnerSeq, TElem2, TKey>(
			TROuterMap mapFactory, TRInnerSeq seqFactory, Func<TElem, TKey> keySelector, Func<TElem, TElem2> valueSelector, IEqualityComparer<TKey> eq)
			where TROuterMap : IAnyMapLike<TKey, TRInnerSeq>
			where TRInnerSeq : IAnyBuilderFactory<TElem2, IterableBuilder<TElem2>>
		{
			return GroupBy(mapFactory, keySelector, valueSelector, (k, vs) =>
			                                                       {
				                                                       var theSeq = ToIterable(seqFactory, vs);
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
		protected virtual TOutIter GroupJoin<TOut, TOutIter, TInner, TKey>(TOutIter bFactory, IEnumerable<TInner> inner,
																							Func<TElem, TKey> oKeySelector, Func<TInner, TKey> iKeySelector,
																							Func<TElem, IEnumerable<TInner>, TOut> rSelector,
																							IEqualityComparer<TKey> eq = null)
			where TOutIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>> {
			bFactory.IsNotNull("bFactory");
			inner.IsNotNull("inner");
			oKeySelector.IsNotNull("oKeySelector");
			iKeySelector.IsNotNull("iKeySelector");
			rSelector.IsNotNull("rSelector");
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
				return (TOutIter)bFactory.IterableFrom(builder);
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
		protected virtual TOutIter Join<TOut, TOutIter, TInner, TKey>(TOutIter bFactory, IEnumerable<TInner> inner, Func<TElem, TKey> oKeySelector,
																					 Func<TInner, TKey> iKeySelector, Func<TElem, TInner, TOut> rSelector,
																					 IEqualityComparer<TKey> eq)
			where TOutIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>>
		{
			bFactory.IsNotNull("bFactory");
			inner.IsNotNull("inner");
			oKeySelector.IsNotNull("oKeySelector");
			iKeySelector.IsNotNull("iKeySelector");
			rSelector.IsNotNull("rSelector");
			using (var builder = bFactory.EmptyBuilder)
			{
				var dict = new Dictionary<TKey, List<TInner>>(eq);
				foreach (var item in inner)
				{
					var k = iKeySelector(item);
					if (dict.ContainsKey(k)) dict[k].Add(item);
					else dict[k] = new List<TInner> { item };
				}
				ForEach(v =>
				{
					var k = oKeySelector(v);
					if (!dict.ContainsKey(k)) return;
					var ins = dict[k];
					ins.ForEach(u => builder.Add(rSelector(v, u)));
				});
				return (TOutIter)bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		///   Returns the first occurrence of the maximum value in the sequence, using the specified comparer.
		/// </summary>
		/// <param name="comp"> The comparer. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public virtual TElem Max(IComparer<TElem> comp) {
			comp.IsNotNull("comp");
			return Reduce((r, v) => comp.Compare(v, r) < 0 ? v : r);
		}

		/// <summary>
		///   Returns the first occurrence of the maximum value in the sequence, using the specified key selector.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		public TElem Max<TKey>(Func<TElem, TKey> selector)
			where TKey : IComparable<TKey> {
			selector.IsNotNull("selector");
			return Max(Comparers.KeyComparer(selector));
		}

		/// <summary>
		///   Returns the first occurrence of the minimum value in the sequence, using the specified key selector.
		/// </summary>
		/// <typeparam name="TKey"> </typeparam>
		/// <param name="selector"> </param>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		public TElem Min<TKey>(Func<TElem, TKey> selector)
			where TKey : IComparable<TKey>
		{
		    if (selector == null) throw Errors.Argument_null("selector");
			return Min(Comparers.KeyComparer(selector));
		}

		/// <summary>
		///   Returns the first occurrence of the minimum value in the sequence, using the specified comparer.
		/// </summary>
		/// <param name="comp"> </param>
		/// <returns> </returns>
		public virtual TElem Min(IComparer<TElem> comp) {
			comp.IsNotNull("comp");
			return Reduce((r, v) => comp.Compare(v, r) > 0 ? v : r);
		}

		/// <summary>
		/// (Implemnetation) Filters the elements of the current collection by type.
		/// </summary>
		/// <typeparam name="TElem2"> The value type of the resulting collection.. </typeparam>
		/// <typeparam name="TRSeq"> The provider returned by the method. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection used as a builder factory. </param>
		/// <returns> </returns>
		protected virtual TRSeq OfType<TElem2, TRSeq>(TRSeq bFactory)
			where TRSeq : IAnyBuilderFactory<TElem2, IterableBuilder<TElem2>> {
			bFactory.IsNotNull("bFactory");
			return Choose(bFactory, v => v.TryCast<TElem2>());
		}

		/// <summary>
		/// (Implementation) Constructs a sequential collection of the specified type by sorting the elements of the current collection using the specified comparer.
		/// </summary>
		/// <typeparam name="TRList"></typeparam>
		/// <param name="bFactory"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		protected internal virtual TRList OrderBy<TRList>(TRList bFactory, IComparer<TElem> comparer)
			where TRList : IAnySequential<TElem> {
			bFactory.IsNotNull("bFactory");
			comparer.IsNotNull("comparer");
			var arr = ToArray();
			Array.Sort(arr, comparer);
			using (var builder = bFactory.EmptyBuilder)
			{
				Array.ForEach(arr, x => builder.Add(x));
				return (TRList) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		/// (Implementation) Constructs a sequential collection by sorting the elements of the current collection by descending order, using the specified comparer.
		/// </summary>
		/// <typeparam name="TRList"></typeparam>
		/// <param name="bFactory"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>

		protected internal virtual TRList OrderByDescending<TRList>(TRList bFactory, IComparer<TElem> comparer)
			where TRList : IAnySequential<TElem> {
			bFactory.IsNotNull("bFactory");
			comparer.IsNotNull("comparer");
			return OrderBy(bFactory, comparer.Invert());
		}

		/// <summary>
		///   Finds the first item for which the specified selector returns Some.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public Option<TResult> Pick<TResult>(Func<TElem, Option<TResult>> selector)
		{
		    if (selector == null) throw Errors.Argument_null("selector");
			var item = Option.NoneOf<TResult>();
			ForEachWhile(v =>
			             {
				             var res = selector(v);
				             if (res.IsSome)
				             {
					             item = res;
					             return false;
				             }
				             return true;
			             });
			return item;
		}

		/// <summary>
		///   Finds the first item for which the specified selector returns Some.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		internal Option<TResult> Pick<TResult>(Func<TElem, int, Option<TResult>> selector)
		{
		    if (selector == null) throw Errors.Argument_null("selector");
			return Pick(selector.AttachIndex());
		}

		/// <summary>
		/// Extracts a concrete collection of the current type from a builder of the current type.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		protected internal abstract TIterable ProviderFrom(TBuilder builder);

		IAnyBuilderFactory<TElem, TBuilder> IAnyBuilderFactory<TElem, TBuilder>.IterableFrom(IterableBuilder<TElem> builder)
		{
			builder.IsNotNull("builder");
			return ProviderFrom((TBuilder) builder);
		}

        /// <summary>
        ///   Applies an accumulator on each element in the sequence. Begins by applying the accumulator on the first two elements.
        /// </summary>
        /// <param name="fold"> The accumulator </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public TElem Reduce(Func<TElem, TElem, TElem> fold)
		{
            if (fold == null) throw Errors.Argument_null("fold");
			return Reduce(x => x, fold);
		}

		/// <summary>
		///   Applies an accumulator on each element in the sequence. Begins by applying the accumulator on the first two elements.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="first"> A selector used to convert the first element to the appropriate type </param>
		/// <param name="fold"> The accumulator </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if an argument is null.</exception>
		public TResult Reduce<TResult>(Func<TElem, TResult> first, Func<TResult, TElem, TResult> fold)
		{
		    if (first == null) throw Errors.Argument_null("first");
		    if (fold == null) throw Errors.Argument_null("fold");
			return Aggregate(Option.NoneOf<TResult>(), (r, v) => r.IsSome ? fold(r, v) : first(v)).ValueOrError(Errors.Not_enough_elements);
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
		protected virtual TRSeq Scan<TElem2, TRSeq>(TRSeq bFactory,
		                                            TElem2 initial,
		                                            Func<TElem2, TElem, TElem2> accumulator)
			where TRSeq : IAnyBuilderFactory<TElem2, IterableBuilder<TElem2>> {
			bFactory.IsNotNull("bFactory");
			accumulator.IsNotNull("accumulator");
			using (var builder = bFactory.EmptyBuilder)
			{
				Aggregate(initial, (r, v) =>
				                   {
					                   r = accumulator(r, v);
					                   builder.Add(r);
					                   return r;
				                   });
				return (TRSeq) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		/// (Implementation) Applies a selector on each element of the collection.
		/// </summary>
		/// <typeparam name="TElem2"> The type of the return element. </typeparam>
		/// <typeparam name="TRSeq"> The type of the return provider. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		protected virtual TRSeq Select<TElem2, TRSeq>(TRSeq bFactory, Func<TElem, TElem2> selector)
			where TRSeq : IAnyBuilderFactory<TElem2, IterableBuilder<TElem2>> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v => builder.Add(selector(v)));
				return (TRSeq) bFactory.IterableFrom(builder);
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
		protected virtual TOutIter SelectMany<TOut, TOutIter>(
	TOutIter bFactory, Func<TElem, IEnumerable<TOut>> selector)
			where TOutIter : IAnyBuilderFactory<TOut, IterableBuilder<TOut>> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v =>
				        {
					        var r = selector(v);
							  builder.AddMany(r);
				        });
				return (TOutIter) bFactory.IterableFrom(builder);
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
		protected virtual TRSeq SelectMany<TRElem, TRSeq, TElem2>(TRSeq bFactory, Func<TElem, IEnumerable<TElem2>> selector,
		                                                          Func<TElem, IEnumerable<TElem2>, TRElem> rSelector)
			where TRSeq : IAnyBuilderFactory<TRElem, IterableBuilder<TRElem>> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			rSelector.IsNotNull("rSelector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v =>
				        {
					        var vs = selector(v);
					        builder.Add(rSelector(v, vs));
				        });
				return (TRSeq) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		///   Returns the first and only item in the sequence. Throws an exception if the sequence doesn't have exactly one item.
		/// </summary>
		/// <returns> </returns>
        /// <exception cref="InvalidOperationException">Thrown if the collection is empty, or if it contains more than 1 element.</exception>
		public virtual TElem Single()
		{
			if (IsEmpty) throw Errors.Is_empty;
			if (Length > 1) throw Errors.Too_many_elements(1);
			return Find(x => true).Value;
		}

		public TElem[] ToArray()
		{
			var lst = new List<TElem>(Length);
			ForEach(lst.Add);
			return lst.ToArray();
		}


		/// <summary>
		/// (Implementation) Constructs a concrete map-like collection from the current collection.
		/// </summary>
		/// <typeparam name="TKey"> The type of the key. </typeparam>
		/// <typeparam name="TValue"> The type of the value. </typeparam>
		/// <typeparam name="TRMap"> The type of the provider returned. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="selector"> A selector that returns a key-value pair. </param>
		/// <returns> </returns>
		protected virtual TRMap ToMapLike<TKey, TValue, TRMap>(
			TRMap bFactory, Func<TElem, KeyValuePair<TKey, TValue>> selector)
			where TRMap : IAnyMapLike<TKey, TValue> {
			bFactory.IsNotNull("bFactory");
			selector.IsNotNull("selector");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach((v) =>
				        {
					        var kvp = selector(v);
					        builder.Add(kvp);
				        });
				return (TRMap) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		/// (Implementation) Constructs a concrete iterable collection from the current collection. 
		/// </summary>
		/// <typeparam name="TRSeq"> The type of the return provider. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <returns> </returns>
		protected virtual TRSeq ToIterable<TRSeq>(TRSeq bFactory)
			where TRSeq : IAnyBuilderFactory<TElem, IterableBuilder<TElem>> {
			bFactory.IsNotNull("bFactory");
			return Select(bFactory, x => x);
		}

		/// <summary>
		/// (Implementation) Constructs a concrete set-like collection from the current collection.
		/// </summary>
		/// <typeparam name="TRSet"></typeparam>
		/// <param name="bFactory"></param>
		/// <returns></returns>
		protected virtual TRSet ToSetLike<TRSet>(TRSet bFactory)
			where TRSet : IAnySetLike<TElem>
		{
			bFactory.IsNotNull("bFactory");
			using (var builder = bFactory.EmptyBuilder)
			{
				ForEach(v => { builder.Add(v); });
				return (TRSet) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		///   Filters the collection using the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public virtual TIterable Where(Func<TElem, bool> predicate)
		{
		    if (predicate == null) throw Errors.Argument_null("predicate");
			using (var builder = EmptyBuilder)
			{
				ForEach(v => { if (predicate(v)) builder.Add(v); });
				return ProviderFrom(builder);
			}
		}

		int IReadOnlyCollection<TElem>.Count {
			get {
				return this.Length;
			}
		}
	}
}