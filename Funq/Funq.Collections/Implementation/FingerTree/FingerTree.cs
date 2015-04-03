using System;
using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation {


	internal enum MethodImplOptions {
		AggressiveInlining,
	}
	/// <summary>
	/// Faked because the assembly previously targeted 4.5 which has AggressiveInlining. <br/>
	/// </summary>
	internal class MethodImpl : Attribute {
		public MethodImpl(MethodImplOptions op) {
			
		}
	}
	static partial class FingerTree<TValue>
	{
		internal abstract class Measured<TObject> : FingerTreeElement
			where TObject : Measured<TObject>
		{
			public readonly Lineage Lineage;
			public int Measure;
			public Option<int> Hash = Option.None;
			protected Measured(int measure, Lineage lineage, int groupings) : base(groupings)
			{
				Measure = measure;
				Lineage = lineage;
			}

			public abstract Leaf<TValue> this[int index] { get; }

			public abstract bool IsFragment { get; }

			public abstract void Fuse(TObject after, out TObject firstRes, out TObject lastRes, Lineage lineage);

			public abstract void Insert(int index, Leaf<TValue> leaf, out TObject value, out TObject rightmost1, Lineage lineage);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> action);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> action);

			public abstract TObject Remove(int index, Lineage lineage);

			public abstract TObject Reverse(Lineage lineage);

			public abstract void Split(int count, out TObject leftmost, out TObject rightmost, Lineage lineage);

			public abstract TObject Update(int index, Leaf<TValue> leaf, Lineage lineage); 
			
		}
	}
}