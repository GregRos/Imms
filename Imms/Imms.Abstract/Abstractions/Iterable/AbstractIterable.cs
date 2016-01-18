using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#pragma warning disable 279

namespace Imms.Abstract {

	/// <summary>
	///     A parent class for collections of all sorts (ordered, unordered, set-like, dictionary-like) that support a form of
	///     iteration.
	/// </summary>
	/// <typeparam name="TElem">The type of element stored in the collection.</typeparam>
	/// <typeparam name="TIterable">This self-reference to the concrete collection type that implements this class.</typeparam>
	/// <typeparam name="TBuilder">The type of collection builder used by the collection that implements this class. </typeparam>
	public abstract partial class AbstractIterable<TElem, TIterable, TBuilder>
		: IEnumerable<TElem>, IBuilderFactory<TBuilder>, IAnyIterable<TElem>
		where TBuilder : IIterableBuilder<TElem, TIterable>
		where TIterable : AbstractIterable<TElem, TIterable, TBuilder> {

		/// <summary>
		///   Filters the collection using the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public virtual TIterable Where(Func<TElem, bool> predicate) {
			if (predicate == null) throw Errors.Argument_null("predicate");
			using (var builder = EmptyBuilder) {
				ForEach(v => {
					if (predicate(v)) builder.Add(v);
				});
				return builder.Produce();
			}
		}

		TBuilder IBuilderFactory<TBuilder>.EmptyBuilder {
			get {
				return EmptyBuilder;
			}
		}


		/// <summary>
		/// Returns an empty builder used to construct a new instance of the collection.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected abstract TBuilder EmptyBuilder {
			get;
		}

		/// <summary>
		/// Returns an empty collection of the current type.
		/// </summary>
		protected TIterable Empty {
			get {
				return EmptyBuilder.Produce();
			}
		}


		/// <summary>
		/// The type parameter TIterable is assumed to be the implementing collection, 
		/// but there is no way to enforce this through the type system, so the type system doesn't "know" it.
		/// This property is used to convert between instances of the current type and TIterable. By default, it uses a dynamic cast.
		/// Dynamic casts of generically typed values are expensive (they are more expensive than 20 method calls, and 5 times more expensive than boxing),
		/// so this property should be overriden in an implementing type. The fact that no type cast is actually necessary doesn't improve the performance of the operation one bit.
		/// In order to improve performance.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual TIterable UnderlyingCollection {
			get {
				//We use the method Fun.CastObject because this type also supports a used-defined conversion to TIterable
				//And we don't want to invoke it.
				return Fun.CastObject<AbstractIterable<TElem, TIterable, TBuilder>, TIterable>(this);
			}
		}

		/// <summary>
		///     Returns true if the collection is empty.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public virtual bool IsEmpty {
			get {
				return !ForEachWhile(v => false);
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
			where TRSeq : IBuilderFactory<IIterableBuilder<TElem2, TRSeq>> {
			bFactory.CheckNotNull("bFactory");
			selector.CheckNotNull("selector");
			using (var builder = bFactory.EmptyBuilder) {
				ForEach(v => builder.Add(selector(v)));
				var ib = builder.Produce();
				return ib;
			}
		}

		/// <summary>
		///     Returns the number of elements in the collection.
		/// </summary>
		public virtual int Length {
			get {
				return Count(x => true);
			}
		}

		/// <summary>
		///     Applies the specified function on every item in the collection, from last to first, and stops when the function returns false.
		/// </summary>
		/// <param name="function"> The function. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public virtual bool ForEachWhile(Func<TElem, bool> function) {
			if (function == null) throw Errors.Argument_null("function");
			foreach (var item in this) if (!function(item)) return false;
			return true;
		}

		/// <summary>
		///     Implicit conversion from the current type to the TIterable collection type.
		/// </summary>
		/// <param name="o"> The o. </param>
		/// <returns> </returns>
		public static implicit operator TIterable(AbstractIterable<TElem, TIterable, TBuilder> o) {
			return o == null ? null : o.UnderlyingCollection;
		}

		/// <summary>
		///     Copies the first several elements (according to order of iteration) of the collection to an array, starting at the
		///     specified array index.
		/// </summary>
		/// <param name="arr">The array to copy to.</param>
		/// <param name="arrStart">The array index at which to begin copying.</param>
		/// <param name="count">The number of elements to copy.</param>
		public virtual void CopyTo(TElem[] arr, int arrStart, int count) {
			var i = 0;
			arr.CheckNotNull("arr");
			if (count == 0 && (arrStart == 0 || arrStart == -1)) {
				return;
			}
			arrStart.CheckIsBetween("arrStart", -arr.Length, arr.Length - 1);
			arrStart = arrStart < 0 ? arrStart + arr.Length : arrStart;
			count.CheckIsBetween("count", 0, arr.Length - arrStart);
			ForEachWhile(x => {
				arr[arrStart + i] = x;
				i++;
				return i < count;
			});
		}

		/// <summary>
		///     Applies an accumulator on every item in the collection, with the specified initial value.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="initial"> The initial value. </param>
		/// <param name="accumulator"> The accumulator. </param>
		/// <returns> </returns>
		public TResult Aggregate<TResult>(TResult initial, Func<TResult, TElem, TResult> accumulator) {
			accumulator.CheckNotNull("accumulator");
			ForEach(v => initial = accumulator(initial, v));
			return initial;
		}

		/// <summary>
		///     Determines if all the items in the collection fulfill the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public bool All(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return ForEachWhile(predicate);
		}

		/// <summary>
		///     Returns true if any item in the collection fulfills the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public bool Any(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return !ForEachWhile(v => !predicate(v));
		}

		/// <summary>
		/// Returns a builder initialized with the specified collection. Assumed to be a cheap operation.
		/// </summary>
		/// <param name="collection">The collection to initialize the builder.</param>
		/// <returns></returns>
		protected abstract TBuilder BuilderFrom(TIterable collection);

		/// <summary>
		///     Returns a string representation of the elements of this collection.
		/// </summary>
		/// <param name="sep">A seperator string.</param>
		/// <param name="printFunc">Optionally, a function that converts every element to its string representation. </param>
		/// <returns></returns>
		public virtual string Print(string sep = "; ", Func<TElem, string> printFunc = null) {
			sep = sep ?? "";
			printFunc = printFunc ?? (x => x.ToString());
			var builder = new StringBuilder();
			bool isFirst = true;
			ForEach(item => {
				if (!isFirst) {
					builder.Append(sep);
				}
				builder.Append(printFunc == null ? item.ToString() : printFunc(item));
				isFirst = false;
			});
			return string.Format("{{{0}}}", builder.ToString());
		}

		/// <summary>
		///     Returns the number of items that satisfy a given predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public int Count(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return Aggregate(0, (r, v) => predicate(v) ? r + 1 : r);
		}

		/// <summary>
		///     Tries to find the first item that matches the specified predicate, returning None if no item was found.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public Optional<TElem> Find(Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			var item = Optional.NoneOf<TElem>();
			ForEachWhile(v => {
				if (predicate(v)) {
					item = v;
					return false;
				}
				return true;
			});
			return item;
		}

		/// <summary>
		///     Applies the specified delegate on every item in the collection, from first to last.
		/// </summary>
		/// <param name="action"> The action. </param>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public virtual void ForEach(Action<TElem> action) {
			action.CheckNotNull("action");
			ForEachWhile(x => {
				action(x);
				return true;
			});
		}


		public abstract IEnumerator<TElem> GetEnumerator();

		/// <summary>
		///     Finds the first item for which the specified selector returns Some.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		public Optional<TResult> Pick<TResult>(Func<TElem, Optional<TResult>> selector) {
			selector.CheckNotNull("selector");
			var item = Optional.NoneOf<TResult>();
			ForEachWhile(v => {
				var res = selector(v);
				if (res.IsSome) {
					item = res;
					return false;
				}
				return true;
			});
			return item;
		}

		/// <summary>
		///     Finds the first item for which the specified selector returns Some.
		/// </summary>
		/// <typeparam name="TResult"> The type of the result. </typeparam>
		/// <param name="selector"> The selector. </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument null.</exception>
		internal Optional<TResult> Pick<TResult>(Func<TElem, int, Optional<TResult>> selector) {
			selector.CheckNotNull("selector");
			return Pick(selector.HideIndex());
		}

		/// <summary>
		///     Applies an accumulator on each element in the sequence. Begins by applying the accumulator on the first two
		///     elements.
		/// </summary>
		/// <param name="fold"> The accumulator </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
		public TElem Aggregate(Func<TElem, TElem, TElem> fold) {
			fold.CheckNotNull("fold");
			return
				Aggregate(Optional.NoneOf<TElem>(), (st, cur) => st.IsSome ? fold(st.Value, cur).AsOptional() : cur.AsOptional())
					.ValueOrError(Errors.Is_empty);
		}

		/// <summary>
		///     Returns the first and only item in the sequence. Throws an exception if the sequence doesn't have exactly one item.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty, or if it contains more than 1 element.</exception>
		public virtual TElem Single() {
			if (IsEmpty) throw Errors.Is_empty;
			if (Length > 1) throw Errors.Too_many_elements(1);
			return Find(x => true).Value;
		}

		/// <summary>
		///     Copies the collection into the specified array, starting from the specified array index.
		/// </summary>
		/// <param name="array"> The array. </param>
		/// <param name="arrayIndex"> The index at which to start copying. </param>
		public void CopyTo(TElem[] array, int arrayIndex) {
			CopyTo(array, arrayIndex, Length);
		}

		/// <summary>
		///     Converts this collection to an array.
		/// </summary>
		/// <returns></returns>
		public TElem[] ToArray() {
			var arr = new TElem[Length];
			CopyTo(arr, 0);
			return arr;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}