using System.Diagnostics;
using Imms.Abstract;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmSortedSetDebugView<>))]
	partial class ImmOrderedSet<T> {


	}

	class ImmSortedSetDebugView<T> {
		private ImmOrderedSet<T> _inner; 
		public ImmSortedSetDebugView(ImmOrderedSet<T> set) {
			_inner = set;
			zIterableView = new IterableDebugView<T>(set);
		}

		public T MaxItem {
			get { return _inner.MaxItem; }
		}

		public T MinItem {
			get { return _inner.MinItem; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IterableDebugView<T> zIterableView { get; set; }
	}
}