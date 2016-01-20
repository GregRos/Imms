namespace Imms {
	/// <summary>
	/// A selector used in map operations. Called when a key-value pair exists in both maps. It selects the value that will appear in the result.
	/// </summary>
	/// <typeparam name="TKey">The type of the key (used in both maps).</typeparam>
	/// <typeparam name="TVal1">The type of the value used in the current map.</typeparam>
	/// <typeparam name="TVal2">The type of the value used in the input map.</typeparam>
	/// <typeparam name="TOut">The type of the value in the result map.</typeparam>
	/// <param name="key">The key that appears in both maps.</param>
	/// <param name="value1">The value appearing in the current map.</param>
	/// <param name="value2">The value appearing in the input map.</param>
	/// <returns>The value that should appear in the result map.</returns>
	public delegate TOut ValueSelector<in TKey, in TVal1, in TVal2, out TOut>(TKey key, TVal1 value1, TVal2 value2);
}