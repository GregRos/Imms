using System;
using System.Collections.Generic;
using System.Diagnostics;

#pragma warning disable 279 //"doesn't implement collection pattern..." yes, I know.

namespace Imms.Abstract {

	/// <summary>
	///     Parent class of all list-like collections, where elements appear in a certain order and may be indexed.
	/// </summary>
	/// <typeparam name="TList"> The type of the underlying collection. </typeparam>
	/// <typeparam name="TElem"> The type of element stored in the collection. </typeparam>
	public abstract partial class AbstractSequential<TElem, TList>
		: AbstractIterable<TElem, TList, ISequentialBuilder<TElem, TList>>, 
			IEquatable<TList> where TList : AbstractSequential<TElem, TList> {


		static readonly IEqualityComparer<TElem> DefaultEquality = FastEquality<TElem>.Default;

		/// <summary>
		///     Returns a new collection containing a slice of the current collection, from index to index. The indexes may be
		///     negative.
		/// </summary>
		/// <param name="from"> The initial index. </param>
		/// <param name="to"> The end index. </param>
		/// <returns> </returns>
		public TList this[int from, int to] {
			get {
				var len = Length;
				from.CheckIsBetween("from", -len, len - 1);
				to.CheckIsBetween("to", -len, len - 1);
				to = to < 0 ? to + len : to;
				@from = @from < 0 ? @from + len : @from;
				to.CheckIsBetween("to", from, message:"The value of the index was converted to its positive form.");
				return GetRange(from, to - from + 1);
			}
		}

		internal virtual Tuple<TList, TList> Split(int atIndex) {
			atIndex.CheckIsBetween("atIndex", -Length, Length);
			if (atIndex == 0) {
				return Tuple.Create(Empty, (TList) this);
			}
			if (atIndex == Length) {
				return Tuple.Create((TList) this, Empty);
			}
			return Tuple.Create(Take(atIndex), Skip(atIndex));
		}

		/// <summary>
		///     Returns the element at the specified index.
		/// </summary>
		/// <param name="index"> </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified index doesn't exist.</exception>
		/// <returns> </returns>
		public TElem this[int index] {
			get {
				index.CheckIsBetween("index", -Length, Length - 1);
				index = index < 0 ? index + Length : index;
				return GetItem(index);
			}
		}

		/// <summary>
		/// Returns the element at the specified index, or None if no element was found.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public Optional<TElem> TryGet(int index) {
			index = index < 0 ? index + Length : index;
			if (index >= Length || index < 0) return Optional.None;
			return GetItem(index);
		}

		/// <summary>
		///     Returns the first element in the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public virtual TElem First {
			get {
				var v = Find(x => true);
				if (v.IsNone) throw Errors.Is_empty;
				return v.Value;
			}
		}

		/// <summary>
		///     Gets the last element in the collection.
		/// </summary>
		/// <value> The last. </value>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public virtual TElem Last {
			get {
				var v = FindLast(x => true);
				if (v.IsNone) throw Errors.Is_empty;
				return v.Value;
			}
		}

		/// <summary>
		///     Returns the first element or None.
		/// </summary>
		/// <value> The first element, or None if the collection is empty.</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Optional<TElem> TryFirst {
			get { return IsEmpty ? Optional.None : Optional.Some(First); }
		}

		/// <summary>
		///     Gets the last element or None.
		/// </summary>
		/// <value> The last element, or None if the collection is empty. </value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Optional<TElem> TryLast {
			get { return IsEmpty ? Optional.None : Optional.Some(Last); }
		}

		/// <summary>
		///     Copies a range of elements from the collection to the specified array.
		/// </summary>
		/// <param name="arr"> The array. </param>
		/// <param name="myStart"> The index of the collection at which to start copying. May be negative.</param>
		/// <param name="arrStart">The index of the array at which to start copying. May be negative.</param>
		/// <param name="count"> The number of items to copy. Must be non-negative.</param>
		/// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     Thrown if the array isn't long enough, or one of the parameters is
		///     invalid.
		/// </exception>
		public virtual void CopyTo(TElem[] arr, int myStart, int arrStart, int count) {
			arr.CheckNotNull("arr");

			if (count != 0 || (myStart != 0 && myStart != -1)) {
				myStart.CheckIsBetween("myStart", -Length, Length - 1);
			}
			if (count != 0 || (arrStart != 0 && arrStart != -1)) {
				arrStart.CheckIsBetween("arrStart", -arr.Length, arr.Length - 1);
			}

			if (count == 0) {
				return;
			}
			
			myStart = myStart < 0 ? myStart + Length : myStart;
			arrStart = arrStart < 0 ? arrStart + arr.Length : arrStart;

			count.CheckIsBetween("count", 0, Length - myStart, "It was out of bounds of this collection.");
			count.CheckIsBetween("count", 0, arr.Length - arrStart, "It was out of bounds of the array.");

			var ix = 0;
			ForEachWhileI((v, i) => {
				if (i < myStart) return true;
				if (ix >= count) return false;
				arr[myStart + ix] = v;
				ix++;
				return true;
			});
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(TList other) {
			return SequenceEquals(other, DefaultEquality);
		}

		/// <summary>
		///     Determines structural equality between the sequential collections.
		/// </summary>
		/// <param name="a">The first collection</param>
		/// <param name="b">The second collection.</param>
		/// <returns>
		///     Whether the two collections are equal.
		/// </returns>
		public static bool operator ==(AbstractSequential<TElem, TList> a, AbstractSequential<TElem, TList> b) {
			var boiler = EqualityHelper.BoilerEquality(a, b);
			if (boiler.IsSome) return boiler.Value;
			return 
				
				a.Equals(b);
		}

		/// <summary>
		///     Determines structural inequality.
		/// </summary>
		/// <param name="a">The first collection</param>
		/// <param name="b">The second collection.</param>
		/// <returns>
		///     Whether the two collections are unequal.
		/// </returns>
		public static bool operator !=(AbstractSequential<TElem, TList> a, AbstractSequential<TElem, TList> b) {
			return !(a == b);
		}

		/// <summary>
		///     Determines structural inequality with obj. Two sequential collections are equal if they contain the same sequence of elements, and are of the same type.
		/// </summary>
		/// <param name="obj">Another object (a sequential collection) to compare with this instance.</param>
		public override bool Equals(object obj) {
			var elems = obj as AbstractSequential<TElem, TList>;
			return elems != null && elems.Equals(this);
		}

		/// <summary>
		///     Returns a structural hash code for this collection. Uses the default hash function of the elements.
		/// </summary>
		public override int GetHashCode() {
			return this.CompuateSeqHashCode(DefaultEquality);
		}

		/// <summary>
		///     Returns the element at the specified index. Doesn't  support negative indexing.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected virtual TElem GetItem(int index) {
			return Pick((v, i) => i == index ? Optional.Some(v) : Optional.None).Value;
		}

		/// <summary>
		///     Returns a range of elements. Doesn't support negative indexing.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected virtual TList GetRange(int from, int count) {

			using (var builder = EmptyBuilder)
			{
				ForEachWhileI((v, i) => {
					if (i >= from + count) return false;
					if (i < from) return true;
					builder.Add(v);
					return true;
				});
				return builder.Produce();
			}
		}

		/// <summary>
		///     Applies an accumulator on every item in the sequence, from last to first.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="initial"> The initial value. </param>
		/// <param name="fold"> The accumulator. </param>
		/// <exception cref="ArgumentNullException">Thrown if the accumulator is null.</exception>
		/// <returns> </returns>
		public TResult AggregateBack<TResult>(TResult initial, Func<TResult, TElem, TResult> fold) {
			fold.CheckNotNull("fold");
			ForEachBack(v => initial = fold(initial, v));
			return initial;
		}

		/// <summary>
		///     Finds the index of the first item that matches the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the predicate is null.</exception>
		/// <returns> </returns>
		public Optional<int> FindIndex(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return Pick((v, i) => predicate(v) ? Optional.Some(i) : Optional.None);
		}

		/// <summary>
		///     Returns the first index of the specified element using the default equality comparer.
		/// </summary>
		/// <param name="elem">The element to find.</param>
		/// <returns>The index of the element, or None if it wasn't found.</returns>
		public virtual Optional<int> FindIndex(TElem elem) {
			return FindIndex(v => Equals(v, elem));
		}

		/// <summary>
		///     Returns true if this sequence is equal to the other sequence, using an optional value equality comparer.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		public virtual bool SequenceEquals(IEnumerable<TElem> other, IEqualityComparer<TElem> eq = null) {
			other.CheckNotNull("other");
			TList list = other as TList;
			if (list != null) return SequenceEquals(list, eq);
			return EqualityHelper.SeqEquals(this, other, eq);
		}

		/// <summary>
		///     Override this to provide an efficient implementation for the operation.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="eq"></param>
		/// <returns></returns>
		protected virtual bool SequenceEquals(TList other, IEqualityComparer<TElem> eq = null) {
			other.CheckNotNull("other");
			return EqualityHelper.SeqEquals(this, other, eq);
		}

		/// <summary>
		/// Returns the index of the last element that satisfies the given predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public Optional<int> FindLastIndex(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			Optional<int> found = Optional.None;
			var i = 0;
			ForEachBackWhile(v => {
				if (predicate(v)) {
					found = i++;
					return false;
				}
				i++;
				return true;
			});
			return found.Map(ix => Length - 1 - ix);
		}

		/// <summary>
		///     Tries to find the last item that matches the specified predicate, returning None if no such item was found.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the predicate is null.</exception>
		/// <returns> </returns>
		public Optional<TElem> FindLast(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			Optional<TElem> found = Optional.None;
			ForEachBackWhile(x => {
				if (predicate(x)) {
					found = x;
					return false;
				}
				return true;
			});
			return found;
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from last to first.
		/// </summary>
		/// <param name="action"> The action. </param>
		/// <exception cref="ArgumentNullException">Thrown if the delegate is null.</exception>
		public virtual void ForEachBack(Action<TElem> action) {
			action.CheckNotNull("action");

			ForEachBackWhile(v => {
				action(v);
				return true;
			});
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from last to first, until it returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the function null.</exception>
		public virtual bool ForEachBackWhile(Func<TElem, bool> function) {
			function.CheckNotNull("function");
			var list = new List<TElem>(Length);
			list.AddRange(this);
			for (var i = list.Count - 1; i >= 0; i--) if (!function(list[i])) return false;
			return true;
		}

		/// <summary>
		///     Applies an function over each element-index pair in the collection, from first to last, and stops if the function
		///     returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		private bool ForEachWhileI(Func<TElem, int, bool> function) {
			function.CheckNotNull("function");
			return ForEachWhile(function.HideIndex());
		}

		/// <summary>
		///     Applies an accumulator on each element in the sequence. Begins by applying the accumulator on the first two
		///     elements. Runs from last to first.
		/// </summary>
		/// <param name="fold"> The accumulator </param>
		/// <returns> The final result of the accumulator </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public TElem AggregateBack(Func<TElem, TElem, TElem> fold) {
			fold.CheckNotNull("fold");
			return
				AggregateBack(Optional.NoneOf<TElem>(), (r, v) => r.IsSome ? fold(r.Value, v).AsOptional() : v.AsOptional())
					.ValueOrError(Errors.Is_empty);
		}

		/// <summary>
		///     Returns a reversed collection.
		/// </summary>
		/// <returns> </returns>
		public virtual TList Reverse() {
			if (Length <= 1) return this;
			using (var builder = EmptyBuilder) {
				ForEachBack(v => builder.Add(v));
				return builder.Produce();
			}
		}

		/// <summary>
		///     Returns a new collection without the specified initial number of elements. Returns empty if
		///     <paramref name="count" /> is equal or greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements to skip. </param>
		/// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public virtual TList Skip(int count) {
			count.CheckIsBetween("count", 0);
			if (count >= Length) return Empty;
			if (count == 0) return this;
			return GetRange(count, Length - count);
		}

		/// <summary>
		///     Discards the initial elements in the collection until the predicate returns false.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		/// <returns> </returns>
		public TList SkipWhile(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			var index = FindIndex(x => !predicate(x));
			return index.IsNone ? Empty : Skip(index.Value);
		}


		public sealed override void CopyTo(TElem[] arr, int arrStart, int count) {
			CopyTo(arr, 0, arrStart, count);
		}

		/// <summary>
		///     Returns a subsequence consisting of the specified number of elements. Returns empty if <paramref name="count" /> is
		///     greater than Length.
		/// </summary>
		/// <param name="count"> The number of elements.. </param>
		/// <exception cref="ArgumentException">Thrown if the argument is smaller than 0.</exception>
		/// <returns> </returns>
		public virtual TList Take(int count) {
			count.CheckIsBetween("count", 0);
			if (count == 0) return Empty;
			if (count >= Length) return this;
			return GetRange(0, count);
		}

		/// <summary>
		///     Returns the first items until the predicate returns false.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		/// <returns> </returns>
		public TList TakeWhile(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			var index = FindIndex(x => !predicate(x));
			return index.IsNone ? (TList)this : Take(index.Value);
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
		protected virtual TRSeq _ScanBack<TElem2, TRSeq>(TRSeq bFactory, TElem2 initial, Func<TElem2, TElem, TElem2> accumulator)
			where TRSeq : IBuilderFactory<ISequentialBuilder<TElem2, TRSeq>>
		{
			bFactory.CheckNotNull("bFactory");
			if (accumulator == null) throw Errors.Argument_null("accumulator");
			using (var builder = bFactory.EmptyBuilder)
			{
				AggregateBack(initial, (r, v) =>
				                       {
					                       r = accumulator(r, v);
					                       builder.Add(r);
					                       return r;
				                       });
				return builder.Produce();
			}
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
		protected virtual TRSeq _Zip<TRElem, TRSeq, TInner>(TRSeq bFactory, IEnumerable<TInner> o, Func<TElem, TInner, TRElem> selector)
			where TRSeq : IBuilderFactory<ISequentialBuilder<TRElem, TRSeq>>
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
				return builder.Produce();
			}
		}

	}

}