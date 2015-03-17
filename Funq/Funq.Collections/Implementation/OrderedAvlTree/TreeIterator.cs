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
			public TreeIterator(int maxHeight)
			{
				_future = new List<Marked<Node, bool>>(maxHeight);
			}

			public TreeIterator(Node root)
				: this(root.MaxPossibleHeight)
			{
				if (!root.IsNull)_future.Add(Marked.Create(root, false));
			}

			public bool MoveNext()
			{
				while (_future.Count > 0)
				{
					var cur = _future.PopLast();
					if (cur.Mark) return SetCurrent(cur);
					var node = cur.Object;
					if (node.IsNull) continue;
					if (!node.Right.IsNull) _future.Add(node.Right.Mark(false));			
					_future.Add(node.Mark(true));
					if (!node.Left.IsNull) _future.Add(node.Left.Mark(false));
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
			public bool SeekGreaterThan(ComparableKey<TKey> key, out Cmp cmpResult)
			{

				var isEnded = SeekForwardCloseTo(key, out cmpResult);
				if (!isEnded) return false;
				if (cmpResult == Cmp.Greater || cmpResult == Cmp.Equal) return true;
				var tryNext = this.MoveNext();
				if (!tryNext) return false;
				var cur = Current;
				cmpResult = cur.Key.CompareTo(key);
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
			/// <param name="hash"></param>
			/// <returns></returns>
			private bool SeekForwardCloseTo(ComparableKey<TKey> key, out Cmp cmpResult)
			{
				cmpResult = Cmp.Lesser;
				//If we're already at the desired node, return true.
				if (_current != null && _current.Key.CompareTo(key) != Cmp.Lesser) return true;
				//Climb up until the current node is larger than the hash or until the root is reached.
				while (_future.Count > 1)
				{
					var cur = _future.PopLast();
					//We ignore all nodes other than parents we've already passed.
					if (!cur.Mark) continue;
					switch (key.CompareTo(cur.Object.Key))
					{
						case Cmp.Equal:
						case Cmp.Greater:
							_future.Add(cur);
							goto end_climb_up;
						case Cmp.Lesser:
							continue;
					} 
				}
	end_climb_up:
				//Now we climb down again, in order to find the node in question.
				while (_future.Count > 0)
				{
					var cur = _future.PopLast();
					var node = cur.Object;
					var compareResult = node.Key.CompareTo(key);
					if (cur.Mark && compareResult != Cmp.Equal) continue;
					switch (compareResult)
					{
						case Cmp.Lesser:
							if (node.Right.IsNull)
							{
								cmpResult = Cmp.Lesser;
								return SetCurrent(node);
							}
							_future.Add(node.Right.Mark(false));
							break;
						case Cmp.Greater:
							if (!node.Right.IsNull) _future.Add(node.Right.Mark(false));
							if (node.Left.IsNull)
							{
								cmpResult = Cmp.Greater;
								return SetCurrent(node);
							}
							_future.Add(node.Mark(true));
							_future.Add(node.Left.Mark(false));
							break;
						case Cmp.Equal:
							if (!cur.Mark && !node.Right.IsNull) _future.Add(node.Right.Mark(false));
							cmpResult = Cmp.Equal;
							return SetCurrent(node);
					}
				}
				
				return false;
			}
		}
	}
}
