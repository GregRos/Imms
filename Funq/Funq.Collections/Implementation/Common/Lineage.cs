namespace Funq.Implementation {
	/// <summary>
	///     A special lock-type object used to control whether mutation is possible or not. <br />
	///     Used to guarantee persistence, while safely allowing some safe mutation to greatly reduce object construction
	///     overhead.
	/// </summary>
	sealed class Lineage {
		/// <summary>
		///     An instance of the Lineage which always denies mutation.
		/// </summary>
		public static readonly Lineage Immutable = new Lineage(true);

		private readonly bool _neverMutate;

		Lineage() {}

		Lineage(bool never) {
			_neverMutate = never;
		}

		/// <summary>
		///     Creates a new Lineage that allows controlled mutation for an operation with the right key.
		/// </summary>
		/// <returns></returns>
		public static Lineage Mutable() {
#if NO_MUTATION
			return Immutable;
#endif
			return new Lineage();
		}

		public bool AllowMutation(Lineage other) {
#if NO_MUTATION
			return false;
#endif
			return !_neverMutate && this == other;
		}
	}
}