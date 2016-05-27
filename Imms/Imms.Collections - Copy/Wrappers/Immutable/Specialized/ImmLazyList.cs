using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imms.Specialized
{

	internal class ImmLazyList<T> {
		private abstract class Block {

			public abstract Optional<T> Item(ref int index);

			public abstract Block Update(ref int index, T value);

			public abstract Block Insert(ref int index, T value);

			public abstract Block Remove(ref int index);

			public abstract bool CanCompact { get; }
		}

		private class Generator : Block {

			public ImmList<T> Back { get; private set; }

			private readonly IEnumerable<T> _generator;
			private IEnumerator<T> _state;
			public bool IsEager;
			private bool _isOver;
			public Generator(ImmList<T> back, IEnumerable<T> generator, IEnumerator<T> state, bool isEager) {
				Back = back;
				_generator = generator;
				_state = state;
				IsEager = isEager;
			}

			private Generator New(
				ImmList<T> back = null, IEnumerable<T> generator = null, IEnumerator<T> state = null, bool? isEager = null) {
				return new Generator(back ?? Back, generator ?? _generator, state ?? _state, isEager ?? IsEager);
			}

			private void IterateTill(ref int index) {
				for (; index >= 0; index--) {
					if (!_state.MoveNext()) {
						_isOver = true;
						return;
					}
					Back = Back.AddLast(_state.Current);
				}
			}

			public override Optional<T> Item(ref int index) {
				if (index < Back.Length) {
					var res = Back[index];
					index = 0;
					return res;
				}
				IterateTill(ref index);
				if (index == 0) {
					return Back.Last;
				}
				return Optional.None;
			}

			public override Block Update(ref int index, T value) {
				if (index <= Back.Length) {
					var ret = New(back: Back.Update(index, value));
					index = 0;
					return ret;
				}
				IterateTill(ref index);
				if (index == 0) {
					return New(back: Back.Update(Back.Length - 1, value));
				}
				return this;
			}

			public override Block Insert(ref int index, T value) {
				if (index <= Back.Length) {
					var ret = New(back: Back.Insert(index, value));
					index = 0;
					return ret;
				}
				IterateTill(ref index);
				if (index == 0) {
					return New(back: Back.AddLast(value));
				}
				return this;
			}

			public override Block Remove(ref int index) {
				if (index <= Back.Length) {
					var ret = New(back: Back.RemoveAt(index));
					index = 0;
					return ret;
				}
				IterateTill(ref index);
				if (index == 0) {
					return New(back: Back.RemoveLast());
				}
				return this;
			}

			public override bool CanCompact => _isOver;

		}

		private class Evaluated : Block {

			private readonly ImmList<T> _inner;

			public Evaluated(ImmList<T> inner) {
				_inner = inner;
			}

			public override Optional<T> Item(ref int index) {
				if (index < _inner.Length) {
					var res = _inner[index];
					index = 0;
					return res;
				}
				index -= _inner.Length;
				return Optional<T>.None;
			}

			public override Block Update(ref int index, T value) {
				if (index < _inner.Length) {
					var res = _inner.Update(index, value);
					index = 0;
					return new Evaluated(inner:res);
				}
				index -= _inner.Length;
				return this;
			}

			public override Block Insert(ref int index, T value) {
					if (index < _inner.Length) {
					var res = _inner.Insert(index, value);
					index = 0;
					return new Evaluated(inner:res);
				}
				index -= _inner.Length;
				return this;			
			}

			public override Block Remove(ref int index) {
				if (index < _inner.Length) {
					var res = _inner.RemoveAt(index);
					index = 0;
					return new Evaluated(inner:res);
				}
				index -= _inner.Length;
				return this;
			}

			public override bool CanCompact => true;
		}

		private readonly ImmList<Block> _inner;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		private ImmLazyList(ImmList<Block> inner) {
			_inner = inner;
		}
	}
}
