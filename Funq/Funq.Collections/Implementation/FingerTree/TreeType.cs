namespace Funq.Implementation {
	static partial class FingerTree<TValue> {
		abstract partial class FTree<TChild> where TChild : Measured<TChild>, new() {
			private static class TreeType {
				public const int Compound = 3;
				public const int Empty = 1;
				public const int Single = 2;
			}
		}
	}
}