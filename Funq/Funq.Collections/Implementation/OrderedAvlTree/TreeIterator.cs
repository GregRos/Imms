using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	internal partial class OrderedAvlTree<TKey, TValue>
	{
		/// <summary>
		/// A class that iterates over the tree non-recursively.
		/// </summary>
		public class TreeIterator
		{
			private readonly List<Marked<Node, bool>> _future;
			private Node _current;
			private readonly IComparer<TKey> Comparer;
			public TreeIterator(int maxHeight)
			{
				_future = new List<Marked<Node, bool>>();
			}

			public TreeIterator(Node root)
				: this(root.MaxPossibleHeight)
			{
				Comparer = root.Comparer;
				if (!root.IsEmpty)_future.Add(Marked.Create(root, false));
			}

			public bool MoveNext()
			{
				while (_future.Count > 0)
				{
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
			/// Moves the iterator to the first element that is equal to or larger than the specified key. <br/>
			/// Returns the comparison result.
			/// </summary>
			/// <param name="key"></param>
			/// <param name="cmpResult"></param>
			/// <returns></returns>
			public bool SeekGreaterThan(TKey key, out int cmpResult)
			{
				var isNotEnded = SeekForwardCloseTo(key, out cmpResult);
				if (!isNotEnded) return false;
				if (cmpResult >= 0) return true;
				var tryNext = this.MoveNext();
				if (!tryNext) return false;
				var cur = Current;
				cmpResult = Comparer.Compare(cur.Key,key);
#if ASSERTS
				AssertEx.IsTrue(cmpResult >= 0);
#endif
				return true;
			}

			public bool IsEnded
			{
				get
				{
					return _future.Count == 0;
				}
			}

			public Node Current
			{
				get
				{
					return _current;
				}
			}

			private bool SetCurrent(Node node)
			{
				_current = node;
				return true;
			}

			/// <summary>
			/// Moves the iterator to a node with a key that is "close" to the specified key
			/// </summary>
			/// <param name="key"></param>
			/// <param name="cmpResult"></param>
			/// <returns></returns>
			private bool SeekForwardCloseTo(TKey key, out int cmpResult) {
				cmpResult = _current == null ? -1 : Comparer.Compare(_current.Key, key);
				//If we're already at the desired node, return true.
				if (cmpResult >= 0) {
					return true;
				}
				Marked<Node, bool> old = null;
				//Climb up until the current node is larger than the hash or until the root is reached.
				while (_future.Count > 1)
				{
					var cur = _future.PopLast();
					//We ignore all nodes other than parents we've already passed.
					if (!cur.Mark) continue;
					int compare = Comparer.Compare(cur.Object.Key,key);
					if (compare >= 0) {
						_future.Add(cur);
						if (!cur.Object.Left.IsEmpty) _future.Add(cur.Object.Left.Mark(false));
						break;
					}
				}
	
				//Now we climb down again, in order to find the node in question.
				while (_future.Count > 0)
				{
					var cur = _future.PopLast();
					var node = cur.Object;
					var compareResult = Comparer.Compare(node.Key, key); 
					if (cur.Mark && compareResult != 0) continue;
					if (compareResult < 0) {
						if (node.Right.IsEmpty) {
							cmpResult = -1;
							return SetCurrent(node);
						}
						_future.Add(node.Right.Mark(false));
					}
					else if (compareResult > 0) {
						if (!node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						if (node.Left.IsEmpty) {
							cmpResult = 1;
							return SetCurrent(node);
						}
						_future.Add(node.Mark(true));
						_future.Add(node.Left.Mark(false));
					}
					else{
						if (!cur.Mark && !node.Right.IsEmpty) _future.Add(node.Right.Mark(false));
						cmpResult = 0;
						return SetCurrent(node);
					}
				}
				return false;
			}
		}
	}
}
