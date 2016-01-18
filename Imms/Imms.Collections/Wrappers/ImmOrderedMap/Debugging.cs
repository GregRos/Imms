using System.Collections.Generic;
using System.Diagnostics;
using Imms.Abstract;

namespace Imms {
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof (ImmSortedMapDebugView<,>))]
	partial class ImmOrderedMap<TKey, TValue> {

		protected override IMapBuilder<TKey, TValue, ImmOrderedMap<TKey, TValue>> BuilderFrom(ImmOrderedMap<TKey, TValue> collection) {
			return new Builder(collection);
		}
	}

	class ImmSortedMapDebugView<TKey, TValue> {
		private ImmOrderedMap<TKey, TValue> _inner;
		public ImmSortedMapDebugView(ImmOrderedMap<TKey, TValue> map) {
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