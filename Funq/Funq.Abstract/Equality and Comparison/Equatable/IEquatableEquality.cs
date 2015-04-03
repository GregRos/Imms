using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	/// <summary>
	/// Guarantees that the IEqualityComparer implements functional equality to some degree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal interface IEquatableEquality<T> : IEqualityComparer<T>, IEquatable<IEqualityComparer<T>>, ISafeToEquateInExpression
	{
	}
}
