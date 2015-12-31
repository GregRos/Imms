using System.Collections.Generic;
using System.Diagnostics;

namespace Funq.Implementation {
	partial class HashedAvlTree<TKey, TValue> {
		public class TreeIterator {
			readonly List<Marked<Node, bool>> _future;
			Node _current;
			Optional<Node> _oldNode = Optional.None;

			public TreeIterator() {
				_future = new List<Marked<Node, bool>>();
			}

			public TreeIterator(Node root)
				: this() {
				_future.Add(Marked.Create(root, false));
			}

			public bool IsEnded {
				get { return _future.Count == 0; }
			}

			public Node Current {
				get { return _current; }
			}

			public bool MoveNext() {
				if (_future.Count == 0 || _future[0].Object.IsEmpty) return false;
				while (_future.Count > 0) {
					var cur = _future.PopLast();
					if (cur.Mark) {
#if ASSERTS
						if (_oldNode.Map(x => x.Hash) == cur.Object.Hash)
						{
							Debugger.Break();
						}

						_oldNode = cur.Object;
#endif
						return SetCurrent(cur);
					}
					var node = cur.Object;
#if ASSERTS
					if (node.IsEmpty) {
						_future.Count.AssertEqual(1);
					}
#endif
					if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
					cur.SetMark(true);
					_future.Add(cur);
					if (!node.Left.IsEmpty) _future.Add(node.Left.Mark(false));
				}
				return false;
			}

			public bool SeekGreaterThan(int hash) {
				var isEnded = SeekForwardCloseTo(hash);
				if (!isEnded) return false;
				if (_current.Hash >= hash) return true;
				var res = MoveNext();
#if ASSERTS
				AssertEx.AssertTrue(_current.Hash >= hash || !res);
#endif
				return res;
			}

			bool SetCurrent(Node node) {
				_current = node;
				return true;
			}

			/// <summary>
			///     Skips until it reaches 1 after or 1 before the hash. Worst case O(logn), amortized O(1).
			/// </summary>
			/// <param name="hash"></param>
			/// <returns></returns>
			bool SeekForwardCloseTo(int hash) {
				//If we're already at the desired node, return true.
				if (_current != null && _current.Hash >= hash) return true;
				//Climb up until the current node is larger than the hash or until the root is reached.
				while (_future.Count > 1) {
					var cur = _future.PopLast();
#if ASSERTS
					cur.Object.IsEmpty.AssertFalse();
#endif
					//We ignore all nodes other than parents we've already passed.
					if (!cur.Mark) continue;
					//If we haven't found the right node, we stop here:
					if (cur.Object.Hash >= hash) {
						_future.Add(cur);
						if (!cur.Object.Left.IsEmpty) _future.Add(cur.Object.Left.Mark(false));
						break;
					}
				}

				//Now we climb down again, in order to find the node in question.
				while (_future.Count > 0) {
					var cur = _future.PopLast();
					var node = cur.Object;
#if ASSERTS
					node.IsEmpty.AssertFalse();
#endif
					//if (node.IsEmpty && _future.Count > 0) return SetCurrent(_future.LastItem());
					//if (node.IsEmpty) return false;
					if (cur.Mark) {
						if (node.Hash >= hash) return SetCurrent(node);
						continue;
					}
					if (hash > node.Hash) {
						if (node.Right.IsEmpty) return SetCurrent(node);
						_future.Add(node.Right.Mark(false));
					} else if (hash < node.Hash) {
						if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						if (node.Left.IsEmpty) return SetCurrent(node);
						_future.Add(node.Mark(true));
						_future.Add(node.Left.Mark(false));
					} else {
						if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						return SetCurrent(node);
					}
				}
				return false;
			}
		}
	}
}