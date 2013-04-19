using System.Collections.Generic;

namespace Solid.Builders
{
	static class Builders
	{
		/// <summary>
		/// Returns an object that implicitly constructs a collection of type FlexibleList from the specified sequence..
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="seq">The sequence.</param>
		/// <returns></returns>
		public static FlexListBuilder<T> BuildFlexList<T>(this IEnumerable<T> seq)
		{
			return new FlexListBuilder<T>(seq);
		}

		/// <summary>
		///  Returns an object that implicitly constructs a collection of type FlexibleList from the specified sequence.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="seq">The seq.</param>
		/// <returns></returns>
		public static VectorBuilder<T> BuildVector<T>(this IEnumerable<T> seq)
		{
			return new VectorBuilder<T>(seq);
		}
	}
}
