using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using Imms.Abstract;
#pragma warning disable 618 //Obsolete warning about AnyNone

namespace Imms {

	/// <summary>
	/// Represents an optional value, where the underlying value type is unknown. User for abstracting over all optional types.
	/// </summary>
	public interface IAnyOptional {
		/// <summary>
		/// Indicates whether this instance wraps a value.
		/// </summary>
		bool IsSome { get; }

		/// <summary>
		/// True if this instance has no underlying value.
		/// </summary>
		bool IsNone { get; }

		/// <summary>
		/// Gets the underlying value, or throws an exception if none exists.
		/// </summary>
		object Value { get; }
	}

	/// <summary>
	///     A type that indicates an optional value of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of value.</typeparam>
	[Serializable]
	public struct Optional<T> : IEquatable<T>, IEquatable<Optional<T>>, IAnyOptional  {
		static readonly IEqualityComparer<T> Eq = FastEquality<T>.Default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly bool _isSome;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly T _value;

		Optional(T value) : this() {
			_value = value;
			_isSome = true;
		}

		object IAnyOptional.Value {
			get { return Value; }
		}

		/// <summary>
		///     Returns an instance indicating a missing value.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static Optional<T> None {
			get { return new Optional<T>(); }
		}

		/// <summary>
		/// Returns true if this optional value is None.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNone {
			get { return !_isSome; }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string DebuggerDisplay {
			get {
				if (IsSome) return string.Format("Some of {0}: {1}", typeof (T).PrettyName(), Value);
				return string.Format("None of {0}", typeof (T).PrettyName());
			}
		}

		/// <summary>
		/// Returns the underlying value, or the specified default value if no underlying value exists.
		/// </summary>
		/// <param name="default">The default value, returned if this instance has no value.</param>
		/// <returns></returns>
		public T Or(T @default) {
			return IsSome ? Value : @default;
		}

		/// <summary>
		/// Returns this instance if it has an underlying value, and otherwise returns the other optional value instance (whether it has a value or not).
		/// </summary>
		/// <param name="other">The other optional value instance.</param>
		/// <returns></returns>
		public Optional<T> Or(Optional<T> other) {
			return IsSome ? this : other;
		}

		/// <summary>
		/// Returns true if this optional value is Some.
		/// </summary>
		public bool IsSome {
			get { return _isSome; }
		}

		/// <summary>
		///		Returns the underlying value, or throws an exception if none exists.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value {
			get {
				if (!IsSome) throw Errors.NoValue<T>();
				return _value;
			}
		}

		/// <summary>
		///		Determines whether this optional value instance is equal to the specified optional value instance.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Optional<T> other) {
			return other.IsNone ? IsNone : Equals(other.Value);
		}

		/// <summary>
		/// Determines if this optional value instance is equal to the specified concrete (non-optional) value.
		/// </summary>
		/// <param name="other">The concrete value.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		public bool Equals(T other) {
			return IsSome && Eq.Equals(Value, other);
		}

		/// <summary>
		///     A None-coalescing operator, similar to the ?? null-coalescing operator. Returns the underlying value, or the specified default value if none exists.
		/// </summary>
		/// <param name="optional">The optional value.</param>
		/// <param name="default">The default value to return if <paramref name="optional"/> has no underlying value.</param>
		/// <returns></returns>
		public static T operator |(Optional<T> optional, T @default) {
			return optional.Or(@default);
		}

		/// <summary>
		///     Converts the <see cref="AnyNone"/> token to a proper optional value of type <typeparamref name="T"/>, indicating a missing value.
		/// </summary>
		/// <param name="none">The none token.</param>
		public static implicit operator Optional<T>(AnyNone none) {
			return None;
		}

		/// <summary>
		///     Returns an optional value instance wrapping the specified value.
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Optional<T>(T v) {
			return Some(v);
		}

		/// <summary>
		///     Unwraps the optional value.
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator T(Optional<T> v) {
			return v.Value;
		}


		/// <summary>
		///     Determines equality between optional values, where the underlying value type of the right-hand instance is unknown.
		/// </summary>
		/// <param name="a">The left-hand instance, the type of which is known.</param>
		/// <param name="b">The right-hand instance, the type of which is unknown.</param>
		/// <returns></returns>
		public static bool operator ==(Optional<T> a, IAnyOptional b)
		{
			return a.Equals(b);
		}

		/// <summary>
		///     Determines inequality between optional value instances,
		///		 where the underlying value type of the right-hand instance is unknown. The inverse of the == operator.
		/// </summary>
		/// <param name="a">The left-hand instance, the type of which is known.</param>
		/// <param name="b">The right-hand instance, the type of which is unknown.</param>
		/// <returns></returns>
		public static bool operator !=(Optional<T> a, IAnyOptional b) {
			return !(a == b);
		}

		/// <summary>
		///     Determines equality between optional value instances, where the underlying value is of the same type.
		/// </summary>
		/// <returns></returns>
		public static bool operator ==(Optional<T> a, Optional<T> b) {
			return a.Equals(b);
		}

		/// <summary>
		///     Determines inequality between optional value instances. The inverse of the == operator.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(Optional<T> a, Optional<T> b) {
			return !(a == b);
		}

		/// <summary>
		///     Determines equality between an optional value and a concrete value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(Optional<T> a, T b) {
			return a.Equals(b);
		}

		/// <summary>
		///     Determines inequality between an optional value and a concrete value. The inverse of the == operator.
		/// </summary>
		/// <returns></returns>
		public static bool operator !=(Optional<T> a, T other) {
			return !(a == other);
		}

		/// <summary>
		///     Determines equality between an optional value and a concrete value.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool operator ==(T other, Optional<T> self) {
			return self.Equals(other);
		}

		/// <summary>
		///     Determines inequality between an optional value and a concrete value. The inverse of the == operator.
		/// </summary>
		/// <returns></returns>
		public static bool operator !=(T other, Optional<T> self) {
			return !(other == self);
		}

		/// <summary>
		///     Returns a hash code for this optional value instance. If it has an underlying value, the hash code of the underlying value is returned. Otherwise, a hash code of 0 is returned.
		/// </summary>
		public override int GetHashCode() {
			return IsNone ? 0 : Value.GetHashCode();
		}

		/// <summary>
		///     Wraps the specified value in an optional type.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Optional<T> Some(T item) {
			return new Optional<T>(item);
		}

		/// <summary>
		///		Casts the underlying value to a different type (if one exists), throwing InvalidCastException if the operation fails.
		/// </summary>
		/// <typeparam name="TOut">The type to cast to.</typeparam>
		/// <exception cref="InvalidCastException">Thrown if the conversion fails.</exception>
 		public Optional<TOut> Cast<TOut>() {
			return IsNone ? Optional.None : ((TOut) (object) Value).AsSome();
		}

		/// <summary>
		///     Casts the underlying value to a different type (if one exists), returning a missing value if the conversion fails.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <returns></returns>
		public Optional<TOut> As<TOut>() {
			return IsSome && Value is TOut ? ((TOut) (object) Value).AsSome() : Optional.None;
		}

		/// <summary>
		///     Determines equality between the optional value and another object, which may be another optional value or a concrete value.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			return obj is Optional<T>
				? Equals((Optional<T>) obj) : obj is IAnyOptional ? Equals((IAnyOptional) obj) : IsSome && Value.Equals(obj);
		}

		/// <summary>
		/// Determines equality between optional values of different types. None values are always equal, and Some values are equal if the underlying values are equal.
		/// </summary>
		/// <typeparam name="T2">The type of the second optional value.</typeparam>
		/// <param name="other">The other optional value.</param>
		/// <returns></returns>
		public bool Equals<T2>(Optional<T2> other) {
			return IsNone ? other.IsNone : IsSome && other.IsSome && Value.Equals(other.Value);
		}

		/// <summary>
		/// Determines if this optional value is equal to another optional value, where the underlying value type of the second optional value is unknown.
		/// </summary>
		/// <param name="other">The other optional value.</param>
		/// <returns></returns>
		public bool Equals(IAnyOptional other) {
			return (IsNone && other.IsNone) || (IsSome && other.IsSome && Value.Equals(other.Value));
		}

		/// <summary>
		///     Returns a string representation of this optional value. 
		/// </summary>
		/// <returns>
		///     A string that represents this optional value.
		/// </returns>
		public override string ToString() {
			return IsSome ? Value.ToString() : $"{{None<{typeof (T).PrettyName()}>}}";
		}


	}

	
	/// <summary>
	/// Contains extension and utility methods for dealing with Optional values.
	/// </summary>
	public static class Optional {

		/// <summary>
		///     Returns a special token that can be implicitly converted to a None optional value instance of any type.
		/// </summary>
		public static AnyNone None {
			get { return AnyNone.Instance; }
		}

		/// <summary>
		///     Returns an optional value instance indicating a missing value of type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Optional<T> NoneOf<T>() {
			return None;
		}

		/// <summary>
		///     Returns an optional value instance wrapping the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to wrap.</param>
		/// <returns></returns>
		public static Optional<T> Some<T>(T value) {
			return Optional<T>.Some(value);
		}
	}


	/// <summary>
	///     Static class with utility and extension methods for optional values.
	/// </summary>
	public static class OptionalExt {

		/// <summary>
		///     Returns an optional value instance wrapping the specified value. This method can wrap nulls in Some.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="x">The value to wrap.</param>
		/// <returns></returns>
		internal static Optional<T> AsSome<T>(this T x) {
			return Optional.Some(x);
		}


		/// <summary>
		/// Converts the specified nullable value to an optional value. Null is represented as None.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="x">The value.</param>
		/// <returns></returns>
		public static Optional<T> AsOptional<T>(this T? x)
		where T : struct {
			return x?.AsSome() ?? Optional.None;
		}

		/// <summary>
		/// Converts the specified value to an optional type. Null is represented as None.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="x">The value.</param>
		/// <returns></returns>
		public static Optional<T> AsOptional<T>(this T x)
		{
			return x?.AsSome() ?? Optional<T>.None;
		}

		/// <summary>
		///     Returns the underlying value of the optional value instance, or throws an exception if none exists.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="opt"></param>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static T ValueOrError<T>(this Optional<T> opt, Exception ex) {
			if (opt.IsNone) throw ex;
			return opt.Value;
		}

		/// <summary>
		/// Applies the specified function on the underlying value, if one exists, and returns the result. Otherwise, returns None.
		/// </summary>
		/// <typeparam name="T">The type of the input optional value.</typeparam>
		/// <typeparam name="TOut">The type of the output optional value..</typeparam>
		/// <param name="self">The optional value instance.</param>
		/// <param name="f">The function to apply, returning an optional value of a potentially different type.</param>
		/// <returns></returns>
		public static Optional<TOut> Map<T, TOut>(this Optional<T> self, Func<T, Optional<TOut>> f) {
			return self.IsNone ? Optional.NoneOf<TOut>() : f(self.Value);
		}

		/// <summary>
		///     Applies the specified function on the underlying value, if one exists, and wraps the result in an optional value. Otherwise, returns None. Similar to the conditional access ?. operator.
		/// </summary>
		/// <typeparam name="T">The type of the input optional value.</typeparam>
		/// <typeparam name="TOut">The type of the output optional value.</typeparam>
		/// <param name="self">The optional value instance.</param>
		/// <param name="f">The function to apply.</param>
		/// <returns></returns>
		public static Optional<TOut> Map<T, TOut>(this Optional<T> self, Func<T, TOut> f) {
			return self.IsNone ? Optional.NoneOf<TOut>() : f(self.Value).AsSome();
		}

		/// <summary>
		///		Applies the ToString method on the underlying value, if one exists, and wraps the result in an optional value. Otherwise, returns None.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="optional">The optional value on which the method is invoked. </param>
		/// <returns></returns>
		public static Optional<string> AsString<T>(this Optional<T> optional) {
			return optional.Map(v => v.ToString());
		}

		/// <summary>
		///     Applies the ToString method on the inner value, using the specified IFormatProvider, and wraps the result in an optional value. Otherwise, returns None.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="optional">The optional value on which the method is invoked.</param>
		/// <param name="provider">The format provider, used as a parameter when calling ToString on the underlying value (if any).</param>
		/// <returns></returns>
		public static Optional<string> AsString<T>(this Optional<T> optional, IFormatProvider provider)
			where T : IConvertible {
			return optional.Map(v => v.ToString(provider));
		}

		/// <summary>
		///     Compares an optional value instance to a concrete value. If an underlying value exists, compares it to the specified value. A missing value is smaller than any concrete value.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="optional">The optional value which is compared to the other value.</param>
		/// <param name="other">The other, concrete value.</param>
		/// <returns></returns>
		public static int CompareTo<T>(this Optional<T> optional, T other)
			where T : IComparable<T> {
			return optional.IsNone ? -1 : optional.Value.CompareTo(other);
		}

		/// <summary>
		///     Compares against another optional value instance, by comparing the underlying values (if those exist). A missing value is always smaller than an existing value. 
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="optional">The first optional value, which is compared to the other optional value.</param>
		/// <param name="other">The second optional value.</param>
		/// <returns></returns>
		public static int CompareTo<T>(this Optional<T> optional, Optional<T> other)
			where T : IComparable<T> {
			return other.IsSome ? optional.CompareTo(other.Value) : optional.IsNone ? 0 : 1;
		}

		/// <summary>
		/// Applies a filter on the underlying value, returning None if the filter returns false.
		/// </summary>
		/// <param name="optional"></param>
		/// <param name="filter"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Optional<T> Filter<T>(this Optional<T> optional, Func<T, bool> filter) {
			return optional.Map(x => filter(x) ? x.AsSome() : x);
		}

		/// <summary>
		/// Flattens a nested optional type, returning either the final underlying value, or None if no such value exists.
		/// </summary>
		/// <param name="optional"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Optional<T> Flatten<T>(this Optional<Optional<T>> optional) {
			return optional.IsSome ? optional.Value : Optional.None;
		}
	}

	/// <summary>
	///     Normally hidden. Used to indicate a generic None value that can be converted to a typed None. Also acts as a None value where the value type is unknown.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This type is not meant to be referenced directly in user code.")]
	[Serializable]
	[CompilerGenerated]
	public class AnyNone : IAnyOptional {
		/// <summary>
		/// Returns the instance of AnyNone.
		/// </summary>
		internal static readonly AnyNone Instance = new AnyNone();

		AnyNone() {}

		/// <summary>
		/// Returns false.
		/// </summary>
		public bool IsSome
		{
			get { return false; }
		}

		/// <summary>
		/// Returns true.
		/// </summary>
		public bool IsNone { get { return true; } }

		/// <summary>
		/// Throws a <see cref="NoValueException"/>.
		/// </summary>
		public object Value
		{
			get { throw Errors.NoValue(); }
		}
	}
}