using System.Collections;
using System.Collections.Generic;

namespace Funq.Implementation {
	/// <summary>
	///     Returns an iterator for iterating over a finger tree.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	class FingerTreeIterator<TValue> : IEnumerator<TValue> {
		readonly Stack<Marked<FingerTreeElement, int>> _future;
		Leaf<TValue> _current;

		public FingerTreeIterator(FingerTree<TValue>.FTree<Leaf<TValue>> e) {

			//var maxHeight =(int)(4 * Math.Log(e.Measure, 2.0)); //no way is the height bigger than this!
			_future = new Stack<Marked<FingerTreeElement, int>>();
			var wTyped = (FingerTreeElement) e;
			_future.Push(wTyped.Mark(-1));

		}

		public void Dispose() {}

		public bool MoveNext() {
			//_past.Push(_current);
			var top = _future.Peek();
#if ASSERTS
			AssertEx.AssertTrue(top.Object.ChildCount > 0 || top.Object.IsLeaf || _future.Count == 1); //only possible if tree is empty
#endif
			while (top.Mark >= top.Object.ChildCount - 1) {
				_future.Pop();
				//_past.Push(top.Object);
				if (_future.Count == 0) return false;
				top = _future.Peek();
			}
			var obj = top.Object;
			var nextObj = obj.GetChild(top.Mark + 1);
			if (nextObj.IsLeaf) {
				top.SetMark(top.Mark + 1);
				SetCurrent(nextObj);
				return true;
			}
			if (nextObj.ChildCount == 0) {
#if ASSERTS
	//this can only happen if we're in Compound(Digit, Empty, Digit).
				obj.ChildCount.AssertEqual(3);
				top.Mark.AssertEqual(0);
#endif
				top.SetMark(2);
				nextObj = obj.GetChild(2);
			} else top.SetMark(top.Mark + 1);
			for (obj = nextObj; !obj.IsLeaf;) {
				_future.Push(obj.Mark(0));
				obj = obj.GetChild(0);

			}
			SetCurrent(obj);
			return true;
		}

		public void Reset() {
			throw Errors.Reset_not_supported;
		}

		public TValue Current {
			get { return _current.Value; }
		}

		object IEnumerator.Current {
			get { return Current; }
		}

		void SetCurrent(object v) {
			_current = (Leaf<TValue>) v;
		}
	}
}