using System.Collections.Generic;

namespace Funq.Abstract
{
	public interface ITrait_SetLike<TElem> : ITrait_CollectionBuilderFactory<TElem, SetBuilder<TElem>>
	{
		/// <summary>
		///   Determines whether the collection contains the specified element using the
		/// </summary>
		/// <param name="elem"> The elem. </param>
		/// <returns> <c>true</c> if the collection contains the specified element otherwise, <c>false</c> . </returns>
		bool Contains(TElem elem);

		SetRelation RelatesTo(ITrait_SetLike<TElem> other);


	}
}