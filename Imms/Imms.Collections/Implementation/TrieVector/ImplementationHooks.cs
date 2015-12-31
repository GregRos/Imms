namespace Imms.Implementation {
	static partial class TrieVector<TValue> {
		internal abstract partial class Node {
			public static implicit operator ImmVector<TValue>(Node node) {
				return new ImmVector<TValue>(node);
			}

			public static implicit operator Node(ImmVector<TValue> vect) {
				return vect.Root;
			}
		}
	}
}