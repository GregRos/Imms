namespace Funq.Collections.Implementation
{
	internal static partial class TrieVector<TValue>
	{
		internal abstract partial class Node
		{
			public static implicit operator FunqVector<TValue>(Node node)
			{
				return new FunqVector<TValue>(node);
			}

			public static implicit operator Node (FunqVector<TValue> vect)
			{
				return vect.root;
			}
		}
	}
}
