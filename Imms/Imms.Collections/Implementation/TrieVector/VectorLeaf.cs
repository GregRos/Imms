using System;
using System.Collections;
using System.Collections.Generic;

namespace Imms.Implementation {
	static partial class TrieVector<TValue> {
		internal sealed class Leaf : Node {
			const int MyBlock = (1 << 5) - 1;
			private TValue[] Arr;

			public Leaf(TValue[] arr, Lineage lineage, int arrSize)
				: base(0, arrSize, arrSize == 32, lineage, arrSize) {
				Arr = arr;
			}

			public Leaf(TValue item, Lineage lineage)
				: this(new[] { item }, lineage, 1) {}

			public override TValue this[int index] {
				get { return Arr[index & MyBlock]; }
			}

			public override bool IsParent {
				get { return false; }
			}

			TValue[] UpdateStore(int index, TValue v, Lineage lineage) {
				if (Lineage.AllowMutation(lineage)) {
					if (Arr.Length < 32) Arr = Arr.Resize(32);
					Arr[index] = v;
					return Arr;
				} else return Arr.Update(index, v, ArrSize);
			}

			TValue[] AddToStore(TValue v, Lineage lineage) {
				if (Lineage.AllowMutation(lineage)) {
					if (Arr.Length < 32) Arr = Arr.Resize(32);
					Arr[ArrSize] = v;
					return Arr;
				} else {
					var arr = Arr.Resize(ArrSize + 1);
					arr[ArrSize] = v;
					return arr;
				}
			}

			Node _mutate(TValue[] arr, int arrSize) {
				Arr = arr;
				Length = arrSize;
				ArrSize = arrSize;
				IsFull = arrSize == 32;
				return this;

			}

			Node MutateOrCreate(int arrSize, TValue[] arr, Lineage lineage) {
				return Lineage.AllowMutation(lineage) ? _mutate(arr, arrSize) : new Leaf(arr, lineage, arrSize);
			}

			public override Node Add(TValue item, Lineage lineage) {
				Node ret;
				var expectedLength = Length + 1;
				if (ArrSize < 32) {
					var myCopy = AddToStore(item, lineage);
					ret = MutateOrCreate(ArrSize + 1, myCopy, lineage);
				} else {
					var newLeaf = new Leaf(item, lineage);
					ret = new Parent(this, newLeaf, lineage);
				}
#if ASSERTS
				ret.Length.AssertEqual(expectedLength);
				ret[ret.Length - 1].AssertEqual(item);
#endif
				return ret;
			}

			public override int RecursiveTotalLength() {
				return ArrSize;
			}

			public override bool IterWhileFrom(int index, Func<TValue, bool> conditional) {
				var myIndex = index & MyBlock;
				for (var i = myIndex; i < ArrSize; i++) if (!conditional(Arr[i])) return false;
				return true;
			}

			public override Node AddRange(TValue[] arr, Lineage lineage, int maxHeight, ref int start, ref int count) {
				var oldLength = Length;
				if (count == 0) return this;
				var len = ArrSize + count > 32 ? 32 - ArrSize : count;
				var newArr = Arr.Resize(ArrSize + len);
				var origCount = count;
				Array.Copy(arr, start, newArr, Length, len);
				start += len;
				count -= len;
				Node ret = new Leaf(newArr, lineage, ArrSize + len);
				if (Height < maxHeight && count > 0) {
					var parentArr = ArrayExt.OfItems(32, ret, Empty);
					var myParent = new Parent(1, ret.Length, parentArr, lineage, 2);
					ret = myParent.AddRange(arr, lineage, maxHeight, ref start, ref count);
				}
#if ASSERTS
				ret.Length.AssertEqual(oldLength + origCount - count);
				if (len != 0) ret[ret.Length - 1].AssertEqual(arr[start - 1]);
#endif
				return ret;
			}

			public override TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage) {
				var newArr = new TOut[ArrSize];
				for (var i = 0; i < newArr.Length; i++) newArr[i] = transform(Arr[i]);
				return new TrieVector<TOut>.Leaf(newArr, lineage, ArrSize);
			}

			/// <summary>
			///     Removes the last element.
			/// </summary>
			/// <param name="lineage"></param>
			/// <returns></returns>
			public override Node RemoveLast(Lineage lineage) {
#if ASSERTS
				var oldLength = Length;
				var oldArrSize = ArrSize;
				var beforeLast = Length > 1 ? Optional.Some(this[Length - 2]) : Optional.None;
#endif
				Node ret;
				if (Lineage.AllowMutation(lineage)) ret = Remove_MUTATES();
				else {
					var newArr = Arr.Resize(ArrSize - 1);
					ret = new Leaf(newArr, lineage, ArrSize - 1);
				}
#if ASSERTS
				ret.Length.AssertEqual(oldLength - 1);
				ret.ArrSize.AssertEqual(oldArrSize - 1);
				if (beforeLast.IsSome) ret[ret.Length - 1].AssertEqual(beforeLast.Value);
#endif
				return ret;
			}

			Node Remove_MUTATES() {
				ArrSize--;
				Length--;
				return this;
			}

			public override IEnumerator<TValue> GetEnumerator(bool heading) {
				return new Enumerator(this, heading);
			}

			public override void Iter(Action<TValue> action) {
#if ASSERTS
				action.AssertNotNull();
#endif
				for (var i = 0; i < ArrSize; i++) action(Arr[i]);
			}

			public override void IterBack(Action<TValue> action) {
#if ASSERTS
				action.AssertNotNull();
#endif
				for (var i = ArrSize - 1; i >= 0; i--) action(Arr[i]);
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional) {
#if ASSERTS
				conditional.AssertNotNull();
#endif
				for (var i = ArrSize - 1; i >= 0; i--) if (!conditional(Arr[i])) return false;
				return true;
			}

			public override bool IterWhile(Func<TValue, bool> conditional) {
				for (var i = 0; i < ArrSize; i++) if (!conditional(Arr[i])) return false;
				return true;
			}

			public override Node Take(int index, Lineage lineage) {
#if ASSERTS
				var expectedLast = index < Length ? this[index].AsSome() : Optional.None;
#endif
				var bits = index & MyBlock;
				var newArr = Arr.Resize(bits + 1);
				Node ret = new Leaf(newArr, lineage, bits + 1);
#if ASSERTS
				ret.Length.AssertEqual(bits + 1);
				if (expectedLast.IsSome) ret[ret.Length - 1].AssertEqual(expectedLast.Value);
#endif
				return ret;
			}

			public override Node Update(int index, TValue value, Lineage lineage) {
				var bits = index & MyBlock;
#if ASSERTS
				var expectedLength = Length;
				bits.AssertEqual(i => i >= 0 && i < Length);
#endif
				var myCopy = UpdateStore(bits, value, lineage);
				var ret = MutateOrCreate(ArrSize, myCopy, lineage);
#if ASSERTS
				ret[index].AssertEqual(value);
				ret.Length.AssertEqual(expectedLength);
#endif
				return ret;
			}

			public override Node Reverse(Lineage lineage) {
				var newArr = new TValue[Arr.Length];
				for (var i = 0; i < ArrSize; i++) newArr[i] = Arr[ArrSize - i - 1];
				return new Leaf(newArr, lineage, ArrSize);
			}

			internal class Enumerator : IEnumerator<TValue> {
				readonly bool _forward;
				readonly Leaf _node;
				int _index = -1;

				public Enumerator(Leaf node, bool forward, int startIndex = 0) {
					_node = node;
					_forward = forward;
					_index = startIndex - 1;
				}

				public TValue Current {
					get {
						var ix = _forward ? _index : _node.ArrSize - 1 - _index;
						return _node.Arr[ix];
					}
				}

				object IEnumerator.Current {
					get { return Current; }
				}

				public void Dispose() {}

				public bool MoveNext() {
					_index++;
					return _index < _node.Length;
				}

				public void Reset() {
					_index = -1;
				}
			}


		}
	}
}