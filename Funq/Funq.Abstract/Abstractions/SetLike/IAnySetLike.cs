using System.Collections.Generic;

namespace Funq.Abstract
{
	public interface IAnySetLike<TElem> : IAnyBuilderFactory<TElem, SetBuilder<TElem>>
	{
		/// <summary>
		///   Determines whether the collection contains the specified element.
		/// </summary>
		/// <param name="elem"> The elem. </param>
		/// <returns> <c>true</c> if the collection contains the specified element otherwise, <c>false</c> . </returns>
		bool Contains(TElem elem);

		/// <summary>
		/// Returns the set-theoretic relation between this set and the specified other set.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		SetRelation RelatesTo(IEnumerable<TElem> other);

		bool SetEquals(IEnumerable<TElem> other);
	}
}