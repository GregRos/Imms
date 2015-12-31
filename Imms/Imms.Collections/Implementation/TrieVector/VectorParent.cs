using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imms.Implementation {
	static partial class TrieVector<TValue> {
		internal sealed class Parent : Node {
			readonly int _myBlock;
			readonly int _offs;
			private Node[] Arr;

			public Parent(int height, int length, Node[] arr, Lineage lineage, int arrSize)
				: base(height, length, arrSize == 32 && arr[arrSize - 1].Height == height - 1 && arr[arrSize - 1].IsFull, lineage, arrSize) {
#if ASSERTS
				arr.AssertNotNull();
#endif

				Arr = arr;
				_offs = height * 5;
				_myBlock = ((1 << 5) - 1) << _offs;
#if ASSERTS
				Length.AssertEqual(Arr.Resize(ArrSize).Aggregate(0, (tot, cur) => cur != null ? tot + cur.Length : tot));
				var leafIndex = Array.FindIndex(arr, node => node != null && !node.IsParent);
				var parentIndex = Array.FindLastIndex(arr, node => node != null && node.IsParent);
				leafIndex.AssertEqual(i => i == -1 || i > parentIndex);
#endif
			}

			public Parent(Node first, Node second, Lineage lineage)
				: base(first.Height + 1, first.Length + second.Length, false, lineage, 2) {
				Arr = new[] { first, second };
				_offs = Height * 5;
				_myBlock = ((1 << 5) - 1) << _offs;
			}

			public override TValue this[int index] {
				get { return Arr[(index & _myBlock) >> _offs][index]; }
			}

			public override bool IsParent {
				get { return true; }
			}

			private Optional<TValue> TryGet(int index) {
				var myIndex = (index & _myBlock) >> _offs;
				if (myIndex < ArrSize) return Arr[myIndex][index];
				return Optional.None;
			}

			Node _mutate(int count, int arrSize, Node[] arr) {
				Length = count;
				ArrSize = arrSize;
				Arr = arr;
				IsFull = arrSize == 32 && arr[arrSize-1].Height == Height - 1 && arr[arrSize - 1].IsFull;
				return this;
			}


			public override bool IterWhileFrom(int index, Func<TValue, bool> conditional) {
				var myIndex = (index & _myBlock) >> _offs;
				if (!Arr[myIndex].IterWhileFrom(index, conditional)) return false;
				for (var i = myIndex + 1; i < ArrSize; i++) if (!Arr[i].IterWhile(conditional)) return false;
				return true;
			}

			Node MutateOrCreate(int count, int arrSize, Node[] arr, Lineage lineage) {
				var ret = Lineage.AllowMutation(lineage)
					? _mutate(count, arrSize, arr) : new Parent(Height, count, arr, lineage, arrSize);
#if ASSERTS
				ret.Length.AssertEqual(arr.Resize(ret.ArrSize).Aggregate(0, (tot, cur) => cur != null ? tot + cur.Length : tot));
				var leafIndex = Array.FindIndex(arr, node => node != null && !node.IsParent);
				var parentIndex = Array.FindLastIndex(arr, node => node != null && node.IsParent);
				leafIndex.AssertEqual(i => i == -1 || i > parentIndex);
#endif
				return ret;
			}

			Node[] UpdateStore(int index, Node v, Lineage lineage) {
				if (Lineage.AllowMutation(lineage)) {
					if (Arr.Length < 32) Arr = Arr.Resize(32);
					Arr[index] = v;
					return Arr;
				} else return Arr.Update(index, v, ArrSize);
			}

			Node[] AddToStore(Node v, Lineage lineage) {
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

			public override Node Add(TValue item, Lineage lineage) {
				var lastIndex = ArrSize - 1;
				var lastNode = Arr[lastIndex];
				Node ret;
				var expectedLength = Length + 1;
				if (!lastNode.IsFull || lastNode.Height < Height - 1) {
					var newNode = lastNode.Add(item, lineage);
					var myCopy = UpdateStore(lastIndex, newNode, lineage);
					ret = MutateOrCreate(Length + 1, ArrSize, myCopy, lineage);
				} else if (ArrSize != 32) {
					var newNode = new Leaf(item, lineage);
					var newArr = AddToStore(newNode, lineage);
					ret = MutateOrCreate(Length + 1, ArrSize + 1, newArr, lineage);
				} else {
					var newArr = new Node[] { this, new Leaf(item, lineage) };
					ret = new Parent(Height + 1, Length + 1, newArr, lineage, 2);
				}
#if ASSERTS
				ret.Length.AssertEqual(expectedLength);
				ret[ret.Length - 1].AssertEqual(item);
#endif
				return ret;
			}
			
			public override Node AddRange(TValue[] arr, Lineage lineage, int maxHeight, ref int start, ref int count) {
				if (count == 0) return this;
#if ASSERTS
				var oldLength = Length;
#endif
				var newArr = Arr.Resize(32);
				var startCount = count;
				int i;
				for (i = ArrSize - 1; i < 32 && count > 0; i++) {
					newArr[i] = newArr[i] ?? Empty;
					newArr[i] = newArr[i].AddRange(arr, lineage, Height - 1, ref start, ref count);
				}
				Node ret = new Parent(Height, Length + startCount - count, newArr, lineage, i);
				if (Height < maxHeight && count != 0) {
					var myParent = new Parent(ret, Empty, lineage);
					ret = myParent.AddRange(arr, lineage, maxHeight, ref start, ref count);
				}
#if ASSERTS
				ret.Length.AssertEqual(oldLength + startCount - count);
				if (startCount - count > 0) ret[ret.Length - 1].AssertEqual(arr[start - 1]);
#endif
				return ret;
			}

			public override TrieVector<TOut>.Node Apply<TOut>(Func<TValue, TOut> transform, Lineage lineage) {
				var newArr = new TrieVector<TOut>.Node[ArrSize];
				for (var i = 0; i < ArrSize; i++) newArr[i] = Arr[i].Apply(transform, lineage);
				return new TrieVector<TOut>.Parent(Height, Length, newArr, lineage, ArrSize);
			}

			public override Node RemoveLast(Lineage lineage) {
				Node ret;
#if ASSERTS
				var expectedLength = Length - 1;
				var expectedLast = Length > 1 ? Optional.Some(this[Length - 2]) : Optional.None;
#endif


				if (ArrSize == 2 && Arr[1].Length == 1) ret = Arr[0];
				else if (Arr[ArrSize - 1].Length == 1) ret = MutateOrCreate(Length - 1, ArrSize - 1, Arr.Resize(ArrSize - 1), lineage);
				else {
					var newLast = Arr[ArrSize - 1].RemoveLast(lineage);
					var newArr = UpdateStore(ArrSize - 1, newLast, lineage);
					ret = MutateOrCreate(Length - 1, ArrSize, newArr, lineage);
				}
#if ASSERTS
				ret.Length.AssertEqual(expectedLength);
				if (expectedLast.IsSome) ret[ret.Length - 1].AssertEqual(expectedLast.Value);
#endif
				return ret;

			}

			public override IEnumerator<TValue> GetEnumerator(bool forward) {
				return new Enumerator(this, forward);
			}

			public override void Iter(Action<TValue> action) {
#if ASSERTS
				action.AssertNotNull();
#endif
				for (var i = 0; i < ArrSize; i++) Arr[i].Iter(action);
			}

			public override void IterBack(Action<TValue> action) {
				for (var i = ArrSize - 1; i >= 0; i--) Arr[i].IterBack(action);
			}

			public override bool IterBackWhile(Func<TValue, bool> conditional) {
				for (var i = ArrSize - 1; i >= 0; i--) if (!Arr[i].IterBackWhile(conditional)) return false;
				return true;
			}

			public override bool IterWhile(Func<TValue, bool> conditional) {
				for (var i = 0; i < ArrSize; i++) if (!Arr[i].IterWhile(conditional)) return false;
				return true;
			}

			public override int RecursiveTotalLength() {
				var total = 0;
				for (var i = 0; i < ArrSize; i++) total += Arr[i].RecursiveTotalLength();
				return total;
			}

			public override Node Take(int index, Lineage lineage) {
#if ASSERTS
				var expectedLast = TryGet(index);
#endif
				var myIndex = index & _myBlock;
				myIndex = myIndex >> (_offs);
				Node ret;
				if (myIndex == 0) ret = Arr[0].Take(index, lineage);
				else {
					var myArrFirst = Arr.Resize(myIndex + 1);
					var myNewLast = Arr[myIndex].Take(index, lineage);

					myArrFirst[myIndex] = myNewLast;
					ret = new Parent(Height, Arr[0].Length * (myIndex) + myNewLast.Length, myArrFirst, lineage, myIndex + 1);
				}

#if ASSERTS
				if (expectedLast.IsSome) ret[ret.Length - 1].AssertEqual(expectedLast.Value);
#endif
				return ret;
			}

			public override Node Update(int index, TValue value, Lineage lineage) {
#if ASSERTS
				var expectedLength = Length;
#endif
				var myIndex = (index) & _myBlock;
				myIndex = myIndex >> _offs;
				var updatedChild = Arr[myIndex].Update(index, value, lineage);
				var newArr = UpdateStore(myIndex, updatedChild, lineage);
				var ret = MutateOrCreate(Length, ArrSize, newArr, lineage);
#if ASSERTS
				ret.Length.AssertEqual(expectedLength);
				ret[index].AssertEqual(value);
#endif
				return ret;
			}

			public override Node Reverse(Lineage lineage) {
				var newArr = new Node[Arr.Length];
				for (var i = 0; i < ArrSize; i++) newArr[i] = Arr[ArrSize - i - 1].Reverse(lineage);
				return new Parent(Height, Length, newArr, lineage, ArrSize);
			}

			internal class Enumerator : IEnumerator<TValue> {
				readonly bool _forward;
				readonly Parent _node;
				IEnumerator<TValue> _current;
				int _index;

				public Enumerator(Parent node, bool forward, int startIndex = 0) {
					_node = node;
					_forward = forward;
					_index = startIndex - 1;
				}

				public TValue Current {
					get { return _current.Current; }
				}

				object IEnumerator.Current {
					get { return Current; }
				}

				public void Dispose() {}

				public bool MoveNext() {
					if (_index == -1) return TryNext();
					if (_current.MoveNext()) return true;
					return TryNext();
				}

				public void Reset() {
					_index = -1;
				}

				public bool TryNext() {
					_index++;
					if (_index < _node.ArrSize) {
						var ix = _forward ? _index : _node.ArrSize - 1 - _index;
						_current = _node.Arr[ix].GetEnumerator(_forward);
						return _current.MoveNext();
					}
					return false;
				}
			}
		}
	}
}