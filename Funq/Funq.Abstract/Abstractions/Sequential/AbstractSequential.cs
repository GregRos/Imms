using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

#pragma warning disable 279 //"doesn't implement collection pattern..." yes, I know.
namespace Funq.Abstract
{
 
	/// <summary>
	///   Parent class of all list-like collections, where elements appear in a certain order and may be indexed.
	/// </summary>
	/// <typeparam name="TList"> The type of the underlying collection. </typeparam>
	/// <typeparam name="TElem"> The type of element stored in the collection. </typeparam>
	public abstract partial class AbstractSequential<TElem, TList>
		: AbstractIterable<TElem, TList, IterableBuilder<TElem>>, IAnySeqLikeWithBuilder<TElem>, IEquatable<AbstractSequential<TElem, TList>> where TList : AbstractSequential<TElem, TList>
	{

		static readonly IEqualityComparer<TElem> DefaultEquality = FastEquality<TElem>.Default;


		public static bool operator ==(AbstractSequential<TElem, TList> a, AbstractSequential<TElem, TList> b) {
			var boiler = EqualityHelper.Boilerplate(a, b);
			if (boiler.IsSome) return boiler.Value;
			return a.Equals(b);
		}

		public static bool operator !=(AbstractSequential<TElem, TList> a, AbstractSequential<TElem, TList> b) {
			return !(a == b);
		}

		public virtual bool Equals(AbstractSequential<TElem, TList> other)
		{
			return this.SequenceEquals(other, DefaultEquality);
		}

		public override bool Equals(object obj) {
			return obj is AbstractSequential<TElem, TList> && ((AbstractSequential<TElem, TList>)obj).Equals(this);
		}

		public override int GetHashCode() {
			return this.CompuateSeqHashCode(DefaultEquality);
		}

		/// <summary>
		///   Returns a new collection containing a slice of the current collection, from index to index.
		/// </summary>
		/// <param name="from"> The initial index. </param>
		/// <param name="to"> The end index. </param>
		/// <returns> </returns>
		public TList this[int from, int to]
		{
			get
			{
				var len = Length;
				to = to < 0 ? to + Length : to;
				@from = @from < 0 ? @from + len : @from;
				return GetRange(from, to - from + 1);
			}
		}

		/// <summary>
		/// Returns the element at the specified index. Doesn't  support negative indexing.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected virtual TElem GetItem(int index)
		{
			return Pick((v, i) => i == index ? Option.Some(v) : Option.None).Value;
		}

		/// <summary>
		/// Returns a range of elements. Doesn't support negative indexing.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected virtual TList GetRange(int from, int count)
		{
			var a = Skip(from);
			var b = a.Take(count);
			return b;
		}

		/// <summary>
		///   Returns the element at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified index doesn't exist.</exception>
		/// <returns> </returns>
		public TElem this[int index]
		{
			get
			{
				index = index < 0 ? index + Length : index;
				if (index < 0 || index > Length) throw Errors.Arg_out_of_range("index");
				return this.GetItem(index);
			}
		}

		/// <summary>
		///   Returns the first element in the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public virtual TElem First
		{
			get
			{
				var v = Find(x => true);
				if (v.IsNone) throw Errors.Is_empty;
				return v.Value;
			}
		}

		/// <summary>
		///   Gets the last element in the collection.
		/// </summary>
		/// <value> The last. </value>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public virtual TElem Last
		{
			get
			{
				var v =  FindLast((x) => true);
				if (v.IsNone) throw Errors.Is_empty;
				return v.Value;
			}
		}

		/// <summary>
		///   Returns the first element or None.
		/// </summary>
		/// <value> The first element, or None if the collection is empty.</value>
		public Option<TElem> TryFirst
		{
			get
			{
				return IsEmpty ? Option.None : Option.Some(First);
			}
		}

		/// <summary>
		///   Gets the last element or None.
		/// </summary>
		/// <value> The last element, or None if the collection is empty. </value>
		public Option<TElem> TryLast
		{
			get
			{
				return IsEmpty ? Option.None : Option.Some(Last);
			}
		}

		/// <summary>
		/// Applies an accumulator on every item in the sequence, from last to first.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="initial"> The initial value. </param>
		/// <param name="fold"> The accumulator. </param>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		/// <returns> </returns>
		public TResult AggregateBack<TResult>(TResult initial, Func<TResult, TElem, TResult> fold)
		{
			if (fold == null) throw Errors.Argument_null("fold");
			ForEachBack(v => initial = fold(initial, v));
			return initial;
		}

		/// <summary>
		///   Copies a range of elements from the collection to the specified array.
		/// </summary>
		/// <param name="arr"> The array. </param>
		/// <param name="myStart"> The index of the collection at which to start copying. </param>
		/// <param name="arrStart">The index of the array at which to start copying.</param>
		/// <param name="count"> The number of items to copy. </param>
		/// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
		/// /// <exception cref="ArgumentOutOfRangeException">Thrown if the array doesn't contain enough elements.</exception>
		public virtual void CopyTo(TElem[] arr, int myStart, int arrStart, int count)
		{
			if (arr == null) throw Errors.Argument_null("arr");
			if (arr.Length < arrStart + count || Length < myStart + count) throw Errors.Arg_out_of_range("count");
			var ix = 0;
			ForEachWhileI((v, i) =>
			              {
				              if (i < myStart) return true;
				              if (ix >= count) return false;
				              arr[myStart + ix] = v;
							  ix++;
				              return true;
			              });
		}

		/// <summary>
		///   Finds the index of the first item that matches the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the predicate is null.</exception>
		/// <returns> </returns>
		public Option<int> FindIndex(Func<TElem, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			return Pick((v, i) => predicate(v) ? Option.Some(i) : Option.None);
		}

		/// <summary>
		///   Returns the first index of the specified element using the default equality comparer.
		/// </summary>
		/// <param name="elem">The element to find.</param>
		/// <returns>The index of the element, or None if it wasn't found.</returns>
		public virtual Option<int> FindIndex(TElem elem)
		{
			return FindIndex(v => object.Equals(v, elem));
		}

		/// <summary>
		/// Returns true if this sequence is equal to the other sequence, using an optional value equality comparer.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public bool SequenceEquals(IEnumerable<TElem> other, IEqualityComparer<TElem> eq = null) {
			if (other is TList) {
				return SequenceEquals((TList) other, eq);
			}
			return EqualityHelper.Seq_Equals(this, other, eq);
		}

		/// <summary>
		/// Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		protected virtual bool SequenceEquals(TList other, IEqualityComparer<TElem> eq = null) {
			return EqualityHelper.Seq_Equals(this, other, eq);
		}

		/// <summary>
		///   Finds the last item that matches the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the predicate is null.</exception>
		/// <returns> </returns>
		public Option<TElem> FindLast(Func<TElem, bool> predicate)
		{
			if (predicate == null) throw Errors.Argument_null("predicate");
			Option<TElem> found = Option.None;
			ForEach(x => {
				if (predicate(x)) {
					found = x;
				}
			});
			return found;
		}

		/// <summary>
		///   Applies the specified delegate on every item in the collection, from last to first.
		/// </summary>
		/// <param name="action"> The action. </param>
		/// <exception cref="ArgumentNullException">Thrown if the delegate is null.</exception>
		public virtual void ForEachBack(Action<TElem> action)
		{
			if (action == null) throw Errors.Argument_null("action");
	
			ForEachBackWhile(v =>
			                 {
				                 action(v);
				                 return true;
			                 });
		}

		/// <summary>
		///   Applies the specified delegate on every item in the collection,
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the iterator null.</exception>
		public virtual bool ForEachBackWhile(Func<TElem, bool> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			var list = new List<TElem>(Length);
			list.AddRange(this);
			for (int i = list.Count - 1; i >= 0; i--) {
				if (!iterator(list[i])) return false;
			}
			return true;
		}
		 
		/// <summary>
		///   Applies an iterator over each element-index pair in the collection, and stops if the iterator returns false.
		/// </summary>
		/// <param name="iterator"> The iterator. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		public bool ForEachWhileI(Func<TElem, int, bool> iterator)
		{
			if (iterator == null) throw Errors.Argument_null("iterator");
			return ForEachWhile(iterator.AttachIndex());
		}

		/// <summary>
		/// Sorts the current collection using the specified comparer.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns></returns>
		public virtual TList OrderBy(IComparer<TElem> comparer)
		{
			if (comparer == null) throw Errors.Argument_null("comparer");
			return base.OrderBy(this, comparer);
		}
		/// <summary>
		/// Sorts the current collection using a key selector.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keySelector">The key selector.</param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns></returns>
		public TList OrderBy<TKey>(Func<TElem, TKey> keySelector)
			where TKey : IComparable<TKey>
		{
			if (keySelector == null) throw Errors.Argument_null("keySelector");
			return OrderBy(Comparers.KeyComparer(keySelector));
		}
		/// <summary>
		/// Sorts the current collection using a key selector.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keySelector">The key selector.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public TList OrderByDescending<TKey>(Func<TElem, TKey> keySelector)
			where TKey : IComparable<TKey>
		{
			if (keySelector == null) throw Errors.Argument_null("keySelector");
			return OrderByDescending(Comparers.KeyComparer(keySelector));
		}
		/// <summary>
		/// Sorts the current collection using a comparer.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public TList OrderByDescending(IComparer<TElem> comparer)
		{
			if (comparer == null) throw Errors.Argument_null("comparer");
			return base.OrderByDescending(this, comparer);
		}

		/// <summary>
		///   Applies a selector on the first element to convert it, and applies the accumulator over the sequence. 
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="fold"> The accumulator </param>
		/// <param name="first"> A selector used to convert the first element to the appropriate type </param>
		/// <returns> The final result of the accumulator </returns>
		/// <exception cref="ArgumentNullException">Thrown if an argument null.</exception>
		public TResult ReduceBack<TResult>(Func<TElem, TResult> first,Func<TResult, TElem, TResult> fold)
		{
			if (first == null) throw Errors.Argument_null("first");
			if (fold == null) throw Errors.Argument_null("fold");
			return
				AggregateBack(Option.NoneOf<TResult>(), (r, v) => r.IsSome ? fold(r.Value, v) : first(v)).ValueOrError(Errors.Not_enough_elements);
		}

		/// <summary>
		///   Applies an accumulator on each element in the sequence. Begins by applying the accumulator on the first two elements. Runs from last to first.
		/// </summary>
		/// <param name="fold"> The accumulator </param>
		/// <returns> The final result of the accumulator </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public TElem ReduceBack(Func<TElem, TElem, TElem> fold)
		{
			return ReduceBack(x=>x,fold);
		}

		/// <summary>
		///   Returns a reversed collection.
		/// </summary>
		/// <returns> </returns>
		public virtual TList Reverse()
		{
			using (var builder = EmptyBuilder)
			{
				ForEachBack((v) => builder.Add(v));
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		///   Implementation for a method that incrementally applies an accumulator over the collection, and returns a sequence of partial results. Runs from last to first.
		/// </summary>
		/// <typeparam name="TElem2"> The type of the element in the return collection. </typeparam>
		/// <typeparam name="TRSeq"> The type of the return provider. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="initial"> The initial value for the accumulator. </param>
		/// <param name="accumulator"> The accumulator. </param>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		protected virtual TRSeq ScanBack<TElem2, TRSeq>(TRSeq bFactory, TElem2 initial, Func<TElem2, TElem, TElem2> accumulator)
			where TRSeq : IAnySeqLikeWithBuilder<TElem2>
		{
			bFactory.IsNotNull("bFactory");
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			using (var builder = bFactory.EmptyBuilder)
			{
				AggregateBack(initial, (r, v) =>
				                       {
					                       r = accumulator(r, v);
					                       builder.Add(r);
					                       return r;
				                       });
				return (TRSeq) bFactory.IterableFrom(builder);
			}
		}

		/// <summary>
		///   Returns a new collection without the specified initial number of elements. Returns empty if <paramref name="count"/> is greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
        /// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public virtual TList Skip(int count)
		{
		    if (count < 0) throw Errors.Invalid_arg_value("count", "0 or greater.");
			using (var builder = EmptyBuilder)
			{
				ForEachWhileI((v, i) =>
				              {
					              if (i >= count) builder.Add(v);
					              return true;
				              });
				return ProviderFrom(builder);
			}
		}

		internal TList Empty {
			get {
				return ProviderFrom(EmptyBuilder);
			}
		}

		/// <summary>
		///   Discards the initial elements in the collection until a predicate returns false.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		public TList SkipWhile(Func<TElem, bool> predicate)
		{
		    if (predicate == null) throw Errors.Argument_null("predicate");
			var index = FindIndex(predicate);
			if (index.IsNone) {
				return Empty;
			}
			return Skip(index.Value + 1);
		}

		public sealed override void CopyTo(TElem[] arr, int arrStart, int count) {
			CopyTo(arr,0, arrStart, count);
		}

		/// <summary>
        ///   Returns a subsequence consisting of the specified number of elements. Returns empty if <paramref name="count"/> is greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements.. </param>
        /// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public virtual TList Take(int count)
		{
            if (count < 0) throw Errors.Invalid_arg_value("count", "0 or greater.");
			using (var builder = EmptyBuilder)
			{
				ForEachWhileI((v, i) =>
				              {
					              if (i < count)
					              {
						              builder.Add(v);
						              return true;
					              }
					              return false;
				              });
				return ProviderFrom(builder);
			}
		}

		/// <summary>
		///   Returns the first items until the predicate returns false.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
        /// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		public virtual TList TakeWhile(Func<TElem, bool> predicate)
		{
		    if (predicate == null) throw Errors.Argument_null("predicate");
			var index = FindIndex(x => !predicate(x));
			if (index.IsNone) {
				return this;
			}
			return Take(index.Value + 1);
		}


		/// <summary>
		///   Implementation for a method that joins the elements of this collection with those of a sequence by index, and uses a result selector to construct a return collection.
		/// </summary>
		/// <typeparam name="TRElem"> The type of the R elem. </typeparam>
		/// <typeparam name="TRSeq"> The type of the R provider. </typeparam>
		/// <typeparam name="TInner"> The type of the inner. </typeparam>
		/// <param name="bFactory"> A prototype instance of the resulting collection provider, used as a builder factory. </param>
		/// <param name="o"> The o. </param>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		protected virtual TRSeq Zip<TRElem, TRSeq, TInner>(TRSeq bFactory, IEnumerable<TInner> o, Func<TElem, TInner, TRElem> selector)
			where TRSeq : IAnySeqLikeWithBuilder<TRElem>
		{
			using (var builder = bFactory.EmptyBuilder)
			using (var iterator = o.GetEnumerator())
			{
				ForEachWhile(otr =>
				             {
					             if (!iterator.MoveNext()) return false;
					             var inr = iterator.Current;
					             builder.Add(selector(otr, inr));
					             return true;
				             });
				return (TRSeq) bFactory.IterableFrom(builder);
			}
		}
	}
}