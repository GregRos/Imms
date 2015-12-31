using System.Collections.Generic;

namespace Funq.Implementation {
	partial class OrderedAvlTree<TKey, TValue> {
		/// <summary>
		///     A class that iterates over the tree non-recursively.
		/// </summary>
		public class TreeIterator {
			readonly IComparer<TKey> _comparer;
			readonly List<Marked<Node, bool>> _future;
			Node _current;

			public TreeIterator() {
				_future = new List<Marked<Node, bool>>();
			}

			public TreeIterator(Node root)
				: this() {
				_comparer = root.Comparer;
				if (!root.IsEmpty) _future.Add(Marked.Create(root, false));
			}

			public bool IsEnded {
				get { return _future.Count == 0; }
			}

			public Node Current {
				get { return _current; }
			}

			public bool MoveNext() {
				while (_future.Count > 0) {
					var cur = _future.PopLast();
					if (cur.Mark) return SetCurrent(cur);
					var node = cur.Object;
					if (node.IsEmpty) continue;
					if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
					cur.SetMark(true);
					_future.Add(cur);
					if (!node.Left.IsEmpty) _future.Add(node.Left.Mark(false));
				}
				return false;
			}

			/// <summary>
			///     Moves the iterator to the first element that is equal to or larger than the specified key. <br />
			///     Returns the comparison result.
			/// </summary>
			/// <param name="key"></param>
			/// <param name="cmpResult"></param>
			/// <returns></returns>
			public bool SeekGreaterThan(TKey key, out int cmpResult) {
				var isNotEnded = SeekForwardCloseTo(key, out cmpResult);
				if (!isNotEnded) return false;
				if (cmpResult >= 0) return true;
				var tryNext = MoveNext();
				if (!tryNext) return false;
				var cur = Current;
				cmpResult = _comparer.Compare(cur.Key, key);
#if ASSERTS
				AssertEx.AssertTrue(cmpResult >= 0);
#endif
				return true;
			}

			bool SetCurrent(Node node) {
				_current = node;
				return true;
			}

			/// <summary>
			///     Moves the iterator to a node with a key that is "close" to the specified key
			/// </summary>
			/// <param name="key"></param>
			/// <param name="cmpResult"></param>
			/// <returns></returns>
			bool SeekForwardCloseTo(TKey key, out int cmpResult) {
				cmpResult = _current == null ? -1 : _comparer.Compare(_current.Key, key);
				//If we're already at the desired node, return true.
				if (cmpResult >= 0) return true;
				//Climb up until the current node is larger than the hash or until the root is reached.
				while (_future.Count > 1) {
					var cur = _future.PopLast();
					//We ignore all nodes other than parents we've already passed.
					if (!cur.Mark) continue;
					var compare = _comparer.Compare(cur.Object.Key, key);
					if (compare >= 0) {
						_future.Add(cur);
						if (!cur.Object.Left.IsEmpty) _future.Add(cur.Object.Left.Mark(false));
						break;
					}
				}

				//Now we climb down again, in order to find the node in question.
				while (_future.Count > 0) {
					var cur = _future.PopLast();
					var node = cur.Object;
					var compareResult = _comparer.Compare(node.Key, key);
					if (cur.Mark) {
						if (compareResult >= 0) {
							cmpResult = compareResult;
							return SetCurrent(node);
						}
						continue;
					}
					if (compareResult < 0) {
						if (node.Right.IsEmpty) {
							cmpResult = -1;
							return SetCurrent(node);
						}
						_future.Add(node.Right.Mark(false));
					} else if (compareResult > 0) {
						if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						if (node.Left.IsEmpty) {
							cmpResult = 1;
							return SetCurrent(node);
						}
						_future.Add(node.Mark(true));
						_future.Add(node.Left.Mark(false));
					} else {
						if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						cmpResult = 0;
						return SetCurrent(node);
					}
				}
				return false;
			}
		}
	}
}