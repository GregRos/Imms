using System;
using System.Collections.Generic;

namespace Funq.Abstract {
	/// <summary>
	///     Guarantees that the IEqualityComparer implements functional equality to some degree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	interface IEquatableEquality<T> : IEqualityComparer<T>, IEquatable<IEqualityComparer<T>>, ISafeToEquateInExpression {}
}