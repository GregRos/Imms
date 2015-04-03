using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;

namespace Funq
{
	/// <summary>
	/// An option type that indicates a possible value.
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public struct Option<T> : IEquatable<T>, IEquatable<Option<T>> {
		static readonly IEqualityComparer<T> Eq = EqualityComparer<T>.Default;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool _isSome;
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private readonly T _value;
		private Option(T value) : this()
		{
			this._value = value;
			_isSome = true;
		}
		/// <summary>
		/// If none, this operator returns the alternate value.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="default"></param>
		/// <returns></returns>
		public static T operator |(Option<T> option, T @default)
		{
			return option.IsSome ? option.Value : @default;
		}
		///
		public static implicit operator Option<T>(NoneSingleton none)
		{
			return None;
		}

		/// <summary>
		/// Converts 'v' to Some(v).
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static implicit operator Option<T>(T v)
		{
			return Some(v);
		}

		/// <summary>
		/// Determines equality between option types.
		/// 
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator ==(Option<T> self, Option<T> other) {
			return self.Equals(other);
		}

		/// <summary>
		/// Determines inequality between options.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator !=(Option<T> self, Option<T> other) {
			return !(self == other);
		}

		/// <summary>
		/// Determines equality between an Option and a real value. If the option is None, it returns false.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator ==(Option<T> self, T other) {
			return self.Equals(other);
		}

		/// <summary>
		/// Determines inequality between an Option and a real value. If the option is None, it returns true.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator !=(Option<T> self, T other) {
			return !(self == other);
		}

		/// <summary>
		/// Determines equality between an Option and a real value. If the option is None, it returns false.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator ==(T other, Option<T> self) {
			return self.Equals(other);
		}
		/// <summary>
		/// Determines inequality between an Option and a real value. If the option is None, it returns true.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator !=(T other, Option<T> self) {
			return !(other == self);
		}

		public override int GetHashCode() {
			return IsNone ? 0 : Value.GetHashCode();
		}

		/// <summary>
		/// Returns a None value for this option type.
		/// </summary>
		public static Option<T> None
		{
			get
			{
				return new Option<T>();
			}
		}

		internal bool EqualsWith(T v, IEqualityComparer<T> eq)
		{
			if (this.IsNone) return false;
			return eq.Equals(this.Value, v);
		}
		/// <summary>
		/// Returns the Some value for this option type.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Option<T> Some(T item)
		{
			return new Option<T>(item);
		}
		
		/// <summary>
		/// Returns true if this option is None.
		/// </summary>
		public bool IsNone
		{
			get
			{
				return !_isSome;
			}
		}

		string DebuggerDisplay {
			get {
				if (IsSome) {
					return string.Format("Some of {0}: {1}", typeof (T).PrettyName(), Value.ToString());
				}
				else {
					return string.Format("None of {0}", typeof (T).PrettyName());
				}
			}
		}

		/// <summary>
		/// Checks for equality. Returns <c>false</c> if this instance is <c>None</c>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(T other) {
			return IsSome && Eq.Equals(Value, other);
		}
		/// <summary>
		/// Checks for equality.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Option<T> other) {
			return other.IsNone ? this.IsNone : Equals(other.Value);
		}

		/// <summary>
		/// Casts the internal value to a different type, throwing InvalidCastException if the operation fails.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <returns></returns>
		public Option<TOut> Cast<TOut>() {
			return IsNone ? Option.None : ((TOut) (object) Value).AsSome();
		}

		/// <summary>
		/// Casts the internal value to a compatible type, returning None if the conversion fails.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <returns></returns>
		public Option<TOut> As<TOut>() {
			return IsSome && Value is TOut ? ((TOut) (object) Value).AsSome() : Option.None;
		}

		public override bool Equals(object obj) {
			return obj is T ? Equals((T) obj) : obj is Option<T> && Equals((Option<T>) obj);
		}

		public override string ToString() {
			return IsSome ? Value.ToString() : "{Option.None}";
		}

		/// <summary>
		/// True if this Option is Some.
		/// </summary>
		public bool IsSome
		{
			get
			{
				return _isSome;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value
		{
			get
			{
				if (!IsSome) throw Errors.NoValue<T>();
				return _value;
			}
		}
	}

	/// <summary>
	/// Static class with utility and extension methods for options.
	/// </summary>
	public static class Option
	{
		/// <summary>
		/// Returns a None token that can be implicitly converted to the None value of any Option(T) type.
		/// </summary>
		public static NoneSingleton None
		{
			get
			{
				return NoneSingleton.Instance;
			}
		}

		public static Option<T> From<T>(T? n)
			where T : struct
		{
			return n.HasValue ? Some(n.Value) : None;
		}
		/// <summary>
		/// Returns the None value for the option type of <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Option<T> NoneOf<T>()
		{
			return None;
		}

		/// <summary>
		/// Returns Some(item)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Option<T> Some<T>(T item)
		{
			return Option<T>.Some(item);
		}

		/// <summary>
		/// Returns the value of 'opt' or throws the exception 'ex'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="opt"></param>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static T ValueOrError<T>(this Option<T> opt, Exception ex)
		{
			if (opt.IsNone)
				throw ex;
			return opt.Value;
		}

		/// <summary>
		/// If None, returns None. Otherwise, returns f(Value).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="self"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static Option<TOut> Bind<T, TOut>(this Option<T> self, Func<T, Option<TOut>> f) {
			return self.IsNone ? Option.NoneOf<TOut>() : f(self.Value);
		}
		/// <summary>
		/// If None, returns None. Otherwise, returns Some(f(Value)).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="self"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static Option<TOut> Map<T, TOut>(this Option<T> self, Func<T, TOut> f) {
			return self.IsNone ? Option.NoneOf<TOut>() : f(self.Value).AsSome();
		}

		/// <summary>
		/// Applies the ToString method on the inner value, or returns None.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		public static Option<string> AsString<T>(this Option<T> self) {
			return self.Map(v => v.ToString());
		}

		/// <summary>
		/// Applies the ToString method on the inner value, using the IFormatProvider, or returns None.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static Option<string> AsString<T>(this Option<T> self, IFormatProvider provider)
		where T : IConvertible {
			return self.Map(v => v.ToString(provider));
		}

		/// <summary>
		/// Compares against another value. If None, the result is -1.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static int CompareTo<T>(this Option<T> self, T other)
		where T : IComparable<T> {
			return self.IsNone ? -1 : self.Value.CompareTo(other);
		}

		/// <summary>
		/// Compares against another option. None is smaller than Some for any option.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static int CompareTo<T>(this Option<T> self, Option<T> other) 
		where T : IComparable<T> {
			return other.IsSome ? self.CompareTo(other.Value) : self.IsNone ? 0 : 1;
		}
	}

	/// <summary>
	/// Normally invisible. Used to indicate a generic None value that can be converted to a typed None.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NoneSingleton
	{
		internal static readonly NoneSingleton Instance = new NoneSingleton();

		private NoneSingleton()
		{
		}
	}
}