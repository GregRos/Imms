namespace Imms.Implementation {
	/// <summary>
	///     A loosely typed finger tree element that could be a digit or a tree. Effectively hides type information, which can
	///     be quite complex and difficult to abstract over. Used for iterating over the finger tree.
	/// </summary>
	abstract class FingerTreeElement {
		/// <summary>
		///     The number of child elements this element has.
		/// </summary>
		public int ChildCount;

		protected FingerTreeElement(int childCount) {
			ChildCount = childCount;
		}

		/// <summary>
		///     Whether this element has a value (is a leaf node).
		/// </summary>
		public virtual bool HasValue {
			get { return false; }
		}

		/// <summary>
		///     Returns whether this element is a leaf.
		/// </summary>
		public bool IsLeaf {
			get { return ChildCount == 0 && HasValue; }
		}

		/// <summary>
		///     Returns the specified child of this node, by 0-based index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract FingerTreeElement GetChild(int index);
	}
}