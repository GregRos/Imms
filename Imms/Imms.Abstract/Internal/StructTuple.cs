namespace Imms {

	/// <summary>
	///     Fast value type singleton tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the first element</typeparam>
	/// <typeparam name="T2">The type of the second element</typeparam>
	struct StructTuple<T1, T2> {
		public readonly T1 First;
		public readonly T2 Second;

		public StructTuple(T1 first, T2 second) : this() {
			First = first;
			Second = second;
		}

		public override string ToString() {
			return string.Format("({0}, {1})", First, Second);
		}
	}

	/// <summary>
	///     Utility methods for fast struct tuples.
	/// </summary>
	static class StructTuple {
		/// <summary>
		///     Creates a 2-StructTuple containing 'a' and 'b'
		/// </summary>
		/// <typeparam name="T1">The type of the first element</typeparam>
		/// <typeparam name="T2">The type of the second element</typeparam>
		/// <param name="a">The first element.</param>
		/// <param name="b">The second element.</param>
		/// <returns></returns>
		public static StructTuple<T1, T2> Create<T1, T2>(T1 a, T2 b) {
			return new StructTuple<T1, T2>(a, b);
		}
	}

}