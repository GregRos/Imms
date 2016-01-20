using System.Collections.Generic;
using System.Diagnostics;
using Imms.Abstract;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmSortedMapDebugView<,>))]
	partial class ImmSortedMap<TKey, TValue> {

		protected override IMapBuilder<TKey, TValue, ImmSortedMap<TKey, TValue>> BuilderFrom(ImmSortedMap<TKey, TValue> collection) {
			return new Builder(collection);
		}
	}

	class ImmSortedMapDebugView<TKey, TValue> {
		private ImmSortedMap<TKey, TValue> _inner;
		public ImmSortedMapDebugView(ImmSortedMap<TKey, TValue> map) {
			_inner = map;
			zIterableView = new IterableDebugView<KeyValuePair<TKey, TValue>> (map);
		}

		public KeyValuePair<TKey, TValue> MaxItem {
			get { return _inner.MaxItem; }
		}

		public KeyValuePair<TKey, TValue> MinItem {
			get { return _inner.MinItem; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IterableDebugView<KeyValuePair<TKey, TValue>> zIterableView { get; set; }
	}
}