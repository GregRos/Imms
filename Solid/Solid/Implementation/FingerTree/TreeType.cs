namespace Solid
{
	static partial class FingerTree<TValue>
	{
		abstract partial class FTree<TChild>
		{
			internal static class TreeType
			{
				public const int Compound = 3;
				public const int Empty = 1;
				public const int Single = 2;
			}
		}
	}
}