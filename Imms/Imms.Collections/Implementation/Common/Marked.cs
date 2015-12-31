using System.Diagnostics;

namespace Imms.Implementation {

	/// <summary>
	///     A light-weight object used to mark other objects (such as nodes in a tree, etc).
	/// </summary>
	/// <typeparam name="T">The type of the object to be marker.</typeparam>
	/// <typeparam name="TMark">The type of the mark.</typeparam>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	class Marked<T, TMark> {
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public readonly T Object;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public TMark Mark;

		public Marked(T o, TMark mark) {
			Mark = mark;
			Object = o;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string DebuggerDisplay {
			get { return string.Format("Marked {0}", Mark.ToString()); }
		}

		public void SetMark(TMark mark) {
			Mark = mark;
		}

		/// <summary>
		///     Returns the underlying object.
		/// </summary>
		/// <param name="mObject">The marked object.</param>
		/// <returns></returns>
		public static implicit operator T(Marked<T, TMark> mObject) {
			return mObject.Object;
		}
	}

	/// <summary>
	///     Helper class for constructing Marked objects.
	/// </summary>
	static class Marked {
		/// <summary>
		///     Creates a new Marked object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMark"></typeparam>
		/// <param name="o"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public static Marked<T, TMark> Create<T, TMark>(T o, TMark mark) {
			return new Marked<T, TMark>(o, mark);
		}

		/// <summary>
		///     Marks the specified object with the specified value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMark"></typeparam>
		/// <param name="o"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public static Marked<T, TMark> Mark<T, TMark>(this T o, TMark mark) {
			return Create(o, mark);
		}

	}
}