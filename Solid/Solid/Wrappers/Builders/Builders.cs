using System.Collections.Generic;

namespace Solid.Builders
{
	static class Builders
	{
		public static FlexListBuilder<T> BuildFlexList<T>(this IEnumerable<T> seq)
		{
			return new FlexListBuilder<T>(seq);
		}

		public static VectorBuilder<T> BuildVector<T>(this IEnumerable<T> seq)
		{
			return new VectorBuilder<T>(seq);
		}
	}
}
