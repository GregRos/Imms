using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solid.Common;

namespace Solid.Lazy
{
	internal static class FlexListEx
	{
		static internal FlexibleList<T> SetFirst<T>(this FlexibleList<T> list, T item)
		{
			return list.IsEmpty ? list.AddLast(item) : list.DropFirst().AddFirst(item);
		}

		static internal FlexibleList<T> SetLast<T>(this FlexibleList<T> list, T item)
		{
			return list.IsEmpty ? list.AddLast(item) : list.DropLast().AddLast(item);
		}
	}

	public class LazyQueue<T>
	{
		internal interface INode
		{
			INode AddFirst(T item);

			INode AddFirstFlexList(FlexibleList<T> list);

			INode AddLast(T item);

			INode AddLastFlexList(FlexibleList<T> list);

			INode AddLastNode(INode next);

			INode AddFirstNode(INode next);
		}

		public class Node
		{
			private FlexibleList<T> _back;
			private readonly IEnumerator<T> _middle;
			private readonly FlexibleList<T> _front;

			public Node(FlexibleList<T> back, IEnumerator<T> middle, FlexibleList<T> front)
			{
				_back = back;
				_middle = middle;
				_front = front;
			}

			public Node AddFirst(T item)
			{
				return new Node(item & _back, _middle, _front);
			}

			public Node AddFirstFlexList(FlexibleList<T> list)
			{
				return new Node(list & _back, _middle, _front);
			}

			public Node AddLastFlexList(FlexibleList<T> list)
			{
				return new Node(_back, _middle, _front & list);
			}

			public Node AddLast(T item)
			{
				return new Node(_back, _middle, _front & item);
			}

			public static Node OfOne(T item)
			{
				return new Node(FlexList.OfItem(item), Enumerable.Empty<T>().GetEnumerator(), FlexList.Empty<T>());
			}

			public static Node OfSeq(IEnumerable<T> items)
			{
				return new Node(FlexList.Empty<T>(), items.GetEnumerator(), FlexList.Empty<T>());
			}

			public static Node OfList(FlexibleList<T> list)
			{
				return new Node(list, Enumerable.Empty<T>().GetEnumerator(), FlexList.Empty<T>());
			}

			public bool DropFirst(out Node res)
			{
				if (!_back.IsEmpty)
				{
					res= new Node(_back.DropFirst(), _middle, _front);
					return true;
				}
				if (Consume())
				{
					res= new Node(FlexibleList<T>.Empty, _middle, _front);
					return true;
				}
				if (!_front.IsEmpty)
				{
					res = Node.OfList(_front.DropFirst());
					return true;
				}
				res = this;
				return false;
			}

			public bool Consume()
			{
				if (_middle.MoveNext())
				{
					_back = _back.AddLast(_middle.Current);
					return true;
				}
				return false;
			}
		}

		private FlexibleList<Node> _nodes;

		public LazyQueue(FlexibleList<Node> nodes)
		{
			_nodes = nodes;
		}

		public LazyQueue<T> AddFirst(T item)
		{
			var first = _nodes.IsEmpty ? Node.OfOne(item) : _nodes.First.AddFirst(item);
			var nodes = _nodes.IsEmpty ? FlexList.OfItem(first) : _nodes.DropFirst().AddFirst(first);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddLast(T item)
		{
			var last = _nodes.IsEmpty ? Node.OfOne(item) : _nodes.Last.AddLast(item);
			var nodes = _nodes.IsEmpty ? FlexList.OfItem(last) : _nodes.DropLast().AddLast(last);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddLastRange(IEnumerable<T> items)
		{
			var last = Node.OfSeq(items);
			var nodes = _nodes.AddLast(last);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddFirstRange(IEnumerable<T> items)
		{
			var first = Node.OfSeq(items);
			var nodes = _nodes.AddFirst(first);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddFirstList(FlexibleList<T> list)
		{
			var first = _nodes.IsEmpty ? Node.OfList(list) : _nodes.First.AddFirstFlexList(list);
			var nodes = _nodes.IsEmpty ? FlexList.OfItem(first) : _nodes.DropFirst().AddFirst(first);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddLastList(FlexibleList<T> list)
		{
			var first = _nodes.IsEmpty ? Node.OfList(list) : _nodes.Last.AddLastFlexList(list);
			var nodes = _nodes.IsEmpty ? FlexList.OfItem(first) : _nodes.DropLast().AddLast(first);
			return new LazyQueue<T>(nodes);
		}

		public LazyQueue<T> AddLastQueue(LazyQueue<T> last)
		{
			return new LazyQueue<T>(_nodes & last._nodes);
		}

		public LazyQueue<T> AddFirstQueue(LazyQueue<T> first)
		{
			return new LazyQueue<T>(first._nodes & _nodes);
		}

		public LazyQueue<T> DropFirst()
		{

			
		}
	}
}
