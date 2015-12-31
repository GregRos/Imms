using System.Diagnostics;

namespace Imms.Abstract {
	abstract partial class AbstractSequential<TElem, TList> {
		/// <summary>
		/// Provides a general debug view for a sequential collection.
		/// </summary>
		protected class SequentialDebugView {

			/// <summary>
			/// Constructs a debug view for the specified collection.
			/// </summary>
			/// <param name="list">The collection.</param>
			public SequentialDebugView(TList list) {
				zIterableView = new IterableDebugView(list);
			}

			/// <summary>
			/// Returns the first element of the collection.
			/// </summary>
			public TElem First {
				get { return zIterableView.Object.First; }
			}

			/// <summary>
			/// Returns the last element of the collection.
			/// </summary>
			public TElem Last {
				get { return zIterableView.Object.Last; }
			}

			/// <summary>
			/// Acts as though this type inherits from IterableDebugView. Actual inheritance is not used because this makes the debug view appear differently.
			/// </summary>
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IterableDebugView zIterableView { get; private set; }
		}

	}
}