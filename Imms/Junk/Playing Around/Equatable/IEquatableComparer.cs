using System;
using System.Collections.Generic;

namespace Imm.Abstract {
	/// <summary>
	///     Guarantees that this comparer implements functional equality.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	interface IEquatableComparer<T> : IComparer<T>, IEquatable<IComparer<T>> {}
}