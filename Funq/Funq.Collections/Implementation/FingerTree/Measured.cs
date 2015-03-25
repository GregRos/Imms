using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{

	internal class FingerTreeIterator<TValue> : IEnumerator<TValue> {
		private const int NotVisited = -1;
		private readonly Stack<Marked<WeaklyTypedElement, int>> _future;
		private readonly Stack<WeaklyTypedElement> _past = new Stack<WeaklyTypedElement>();
		private Leaf<TValue> _current;
		public FingerTreeIterator(FingerTree<TValue>.FTree<Leaf<TValue>> e) {
			
			//var maxHeight =(int)(4 * Math.Log(e.Measure, 2.0)); //no way is the height bigger than this!
			_future = new Stack<Marked<WeaklyTypedElement, int>>();
			var wTyped = (WeaklyTypedElement) e;
			_future.Push(wTyped.Mark(-1));
			
		}


		public void Dispose() {
			
		}

		public bool MoveBack() {
			return false;
		}

		private void SetCurrent(object v) {
			_current = (Leaf<TValue>) v;
		}
		public bool MoveNext() {
			//_past.Push(_current);
			var top = _future.Peek();
#if DEBUG
			AssertEx.IsTrue(top.Object.NumberOfGroupings > 0 || top.Object.CanProvideValue || _future.Count == 1); //only possible if tree is empty
#endif
			while (top.Mark >= top.Object.NumberOfGroupings - 1) {
				_future.Pop();
				//_past.Push(top.Object);
				if (_future.Count == 0) {
					return false;
				}
				top = _future.Peek();
			}
			var obj = top.Object;
			var nextObj = obj.GetGrouping(top.Mark + 1);
			if (nextObj.CanProvideValue) {
				top.SetMark(top.Mark + 1);
				SetCurrent(nextObj);
				return true;
			}
			if (nextObj.NumberOfGroupings == 0) {
#if DEBUG
				//this can only happen if we're in Compound(Digit, Empty, Digit).
				obj.NumberOfGroupings.Is(3);
				top.Mark.Is(0);
#endif
				top.SetMark(2);
				nextObj = obj.GetGrouping(2);
			}
			else {
				top.SetMark(top.Mark + 1);
			}
			for (obj = nextObj; !obj.CanProvideValue; ) {
				_future.Push(obj.Mark(0));
				if (obj.NumberOfGroupings != 0) {
					obj = obj.GetGrouping(0);
				}
			}
			SetCurrent(obj);
			return true;
		}

		public void Reset() {
			throw new NotImplementedException();
		}

		public TValue Current {
			get {
				return _current.Value;
			}
		}

		object IEnumerator.Current {
			get { return Current; }
		}
	}

	public abstract class WeaklyTypedElement {
		protected WeaklyTypedElement(int numberOfGroupings) {
			NumberOfGroupings = numberOfGroupings;
		}

		public int NumberOfGroupings;

		public virtual bool HasValue {
			get {
				return false;
			}
		}

		public bool CanProvideValue {
			get {
				return NumberOfGroupings == 0 && HasValue;
			}
		}

		public abstract WeaklyTypedElement GetGrouping(int index);
	}

	static partial class FingerTree<TValue>
	{
		static class Measured
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int Sum<T2>(T2 a, T2 b)
				where T2 : Measured<T2>
			{
				return a.Measure + b.Measure;
			}



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int Sum<T2>(T2 a, T2 b, T2 c)
				where T2 : Measured<T2>
			{
				return a.Measure + b.Measure + c.Measure;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int SumTree<T2, T3>(T2 a, T3 b, T2 c)
				where T2 : Measured<T2>
				where T3 : FTree<T2>
			{
				return a.Measure + b.Measure + c.Measure;
			}

			public static int Sum<T2>(T2 a, T2 b, T2 c, T2 d)
				where T2 : Measured<T2>
			{
				return a.Measure + b.Measure + c.Measure + d.Measure;
			}
		}
		internal interface IReusableEnumerator<in TOver> : IEnumerator<Leaf<TValue>>
			where TOver : Measured<TOver>
		{
			void Retarget(TOver next);
		}

		internal abstract class Measured<TObject> : WeaklyTypedElement
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

			public abstract IReusableEnumerator<TObject> GetEnumerator(bool forward);

			public abstract void Insert(int index, Leaf<TValue> leaf, out TObject value, out TObject rightmost1, Lineage lineage);

			public abstract void Iter(Action<Leaf<TValue>> action);

			public abstract void IterBack(Action<Leaf<TValue>> action);

			public abstract bool IterBackWhile(Func<Leaf<TValue>, bool> action);

			public abstract bool IterWhile(Func<Leaf<TValue>, bool> action);

			public abstract TObject Remove(int index, Lineage lineage);

			public abstract TObject Reverse(Lineage lineage);

			public abstract void Split(int index, out TObject leftmost, out TObject rightmost, Lineage lineage);

			public abstract TObject Update(int index, Leaf<TValue> leaf, Lineage lineage); 
			
		}
	}
}