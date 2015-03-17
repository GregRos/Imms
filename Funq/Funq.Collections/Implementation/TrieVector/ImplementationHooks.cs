namespace Funq.Collections.Implementation
{
	internal static partial class TrieVector<TValue>
	{
		internal abstract partial class Node
		{
			public static implicit operator FunqArray<TValue>(Node node)
			{
				return new FunqArray<TValue>(node);
			}

			public static implicit operator Node (FunqArray<TValue> vect)
			{
				return vect.root;
			}
		}
	}
}
