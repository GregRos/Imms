using System;

namespace Imms.Implementation {

	static partial class FingerTree<TValue> {


		internal abstract class Measured<TObject> : FingerTreeElement
			where TObject : Measured<TObject> {
			protected readonly Lineage Lineage;
			public int Measure;
			public readonly int Nesting;

			/// <summary>
			/// Initializes a new Measured element, capable of being the child of a finger tree.
			/// </summary>
			/// <param name="measure">The measure, or count of the elements.</param>
			/// <param name="lineage">The lineage.</param>
			/// <param name="groupings">The number of groupings, for use with FingerTreeIterator.</param>
			/// <param name="nesting">The nesting level of instances of this type. Used for the ExampleChild trick.</param>
			protected Measured(int measure, Lineage lineage, int groupings, int nesting) : base(groupings) {
				Measure = measure;
				Lineage = lineage;
				Nesting = nesting;
			}

			/// <summary>
			///     Gets the leaf at the specified index.
			/// </summary>
			/// <value>
			///     The <see cref="Leaf{TValue}" />.
			/// </value>
			/// <param name="index">The index.</param>
			/// <returns></returns>
			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			/// <summary>
			///     Prints this instance.
			/// </summary>
			/// <returns></returns>
			public abstract string Print();

			/// <summary>
			///     Reforms this digit with the 'after' digit, returning the fixed digits in the output parameters. Both digits
			///     together have more than 6 elements.
			/// </summary>
			/// <param name="after">The after.</param>
			/// <param name="firstRes">The first output 2-3 digit.</param>
			/// <param name="lastRes">The last output 2-3 digit.</param>
			/// <param name="lineage">The lineage.</param>
			public abstract void Fuse(TObject after, out TObject firstRes, out TObject lastRes, Lineage lineage);

			/// <summary>
			///     Inserts the leaf at the specified index, returning the primary result in 'value' and a rightmost overflow result in
			///     'rightmost'
			/// </summary>
			/// <param name="index">The index.</param>
			/// <param name="leaf">The leaf.</param>
			/// <param name="value">The primary result digit. Cannot be null.</param>
			/// <param name="rightmost1">The rightmost overflow result, when the digit cannot contain the new leaf. May be null.</param>
			/// <param name="lineage">The lineage.</param>
			public abstract void Insert(int index, Leaf<TValue> leaf, out TObject value, out TObject rightmost1, Lineage lineage);

			/// <summary>
			///     Applies the specified action on every leaf.
			/// </summary>
			/// <param name="action">The action.</param>
			public abstract void Iter(Action<Leaf<TValue>> action);

			/// <summary>
			///     Constructs a Measured TObject with the 'proper' number of children for it, which is 3 for digits and 1 for leaves
			///     (obviously).
			/// </summary>
			/// <param name="arr">The array from which to load data.</param>
			/// <param name="i">The index at which to start reading the array. Incremented as data is loaded.</param>
			/// <param name="lin">The lineage.</param>
			/// <returns></returns>
			public abstract TObject Construct3(TValue[] arr, ref int i, Lineage lin);

			/// <summary>
			///     Iterates over the object from last to first.
			/// </summary>
			/// <param name="action">The action.</param>
			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> action);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> action);

			public abstract TObject Remove(int index, Lineage lineage);

			public abstract TObject Reverse(Lineage lineage);


			public abstract TObject Update(int index, Leaf<TValue> leaf, Lineage lineage);

		}
	}
}