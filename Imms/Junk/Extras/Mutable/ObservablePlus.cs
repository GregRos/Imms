using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imm.Collections.Mutable
{

	enum ActionType {
		InsertAt,
		RemoveAt,
		UpdateAt,
		AddLast,
		AddFirst,
		DropLast,
		DropFirst,
	}

	public class ChangeMarker<T> {
		public ActionType Kind;

		public int Index;

		public T OldValue;

		public T NewValue;
	}

	internal static class ChangeMarker {
		public static ChangeMarker<T> AddLast<T>(T newItem, int lastIndex) {
			return new ChangeMarker<T>() {Index = lastIndex, NewValue = newItem, Kind = ActionType.AddLast};
		} 
 		public static ChangeMarker<T> AddFirst<T>(T newItem) {
			return new ChangeMarker<T>() {NewValue = newItem, Kind = ActionType.AddFirst};
		} 
		public static ChangeMarker<T> DropLast<T>(T oldItem, int lastIndex) {
			return new ChangeMarker<T>() {Index = lastIndex, OldValue = oldItem, Kind = ActionType.DropLast};
		} 
		public static ChangeMarker<T> DropFirst<T>(T oldItem) {
			return new ChangeMarker<T>() {OldValue = oldItem, Kind = ActionType.DropFirst};
		} 

		public static ChangeMarker<T> InsertAt<T>(T newItem, int index) {
			return new ChangeMarker<T>() {Index = index, NewValue = newItem, Kind = ActionType.InsertAt};
		} 
		public static ChangeMarker<T> UpdateAt<T>(T oldItem, T newItem, int index) {
			return new ChangeMarker<T>() {Index = index, OldValue = oldItem, NewValue = newItem, Kind = ActionType.UpdateAt};
		}

		public static ChangeMarker<T> RemoveAt<T>(T oldItem, int index) {
			return new ChangeMarker<T>() {Index = index, OldValue = oldItem};
		} 
	}

	public class ListIterator<T> {
		public int Index {
			get;
			set;
		}

		public T Value {
			get;
			set;
		}
	}

	public class ObservableEnumerable<T> : IEnumerable<T> {
		private sealed class ObservableEnumerator : IEnumerator<T> {
			readonly IEnumerator<T> _inner;

			public ObservableEnumerator(IEnumerator<T> inner) {
				_inner = inner;
				IsDisposed = false;
			}

			public bool IsDisposed {
				get;
				private set;
			}
			internal event EventHandler Disposed;

			void OnDisposed() {
				var handler = Disposed;
				if (handler != null) handler(this, EventArgs.Empty);
			}

			public void Dispose() {
				_inner.Dispose();
				IsDisposed = true;
				OnDisposed();
			}

			private void CheckDisposed() {
				if (IsDisposed) {
					throw Errors.Disposed("Enumerator");
				}
			}

			public bool MoveNext() {
				CheckDisposed();
				return _inner.MoveNext();
			}

			public void Reset() {
				CheckDisposed();
				_inner.Reset();
			}

			public T Current {
				get {
					CheckDisposed();
					return _inner.Current;
				}
			}

			object IEnumerator.Current {
				get { return Current; }
			}
		}

		readonly IEnumerable<T> _inner;

		public ObservableEnumerable(IEnumerable<T> inner) {
			_inner = inner;
		}

		public IEnumerator<T> GetEnumerator() {
			return new ObservableEnumerator(_inner.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

	public class ObservablePlus<T> {
		public class HistoryEntry {
			public HistoryEntry(ChangeMarker<T> metadata, ImmList<T> snapshot, int index) {
				Snapshot = snapshot;
				Index = index;
				Metadata = metadata;
			}

			public ChangeMarker<T> Metadata {
				get;
				private set;
			}

			public ImmList<T> Snapshot {
				get;
				private set;
			}

			private int Index {
				get;
				set;
			}
		}

		ImmList<HistoryEntry> _history;
		ImmList<T> _current;
		
		private void AddHistory(ChangeMarker<T> marker) {
			_history = _history.AddLast(StructTuple.Create(marker, _current));
		}

		public IEnumerable<int> HistoryOpen {
			get {
				yield break;
			}
		}

		private IEnumerator<T> Text() {
			return null;
		}

		public void AddLast(T item) {
			foreach (var k in Text()) {
				
			}
			var newCurrent = _current.AddLast(item);
			AddHistory(ChangeMarker.AddLast(item, _current.Length - 1));
			_current = newCurrent;
		}

		public void AddFirst(T item) {
			var newCurrent = _current.AddFirst(item);
			AddHistory(ChangeMarker.AddFirst(item));
			_current = newCurrent;
		}

		public void DropLast() {
			var newCurrent = _current.DropLast();
			AddHistory(ChangeMarker.DropLast(_current.Last, _current.Length - 1));
			_current = newCurrent;
		}

		public void DropFirst() {
			var newCurrent = _current.DropFirst();
			AddHistory(ChangeMarker.DropFirst(_current.First));
			_current = newCurrent;
		}

		public void InsertAt(int index, T item) {
			var newCurrent = _current.Insert(index, item);
			AddHistory(ChangeMarker.InsertAt(item, index));
			_current = newCurrent;
		}

		public void RemoveAt(int index) {
			var newCurrent = _current.Remove(index);
			AddHistory(ChangeMarker.RemoveAt(_current[index], index));
			_current = newCurrent;
		}

		public void Update(int index, T item) {
			var newCurrent = _current.Update(index, item);
			var old = _current[index];
			AddHistory(ChangeMarker.UpdateAt(old, item, index));
			_current = newCurrent;
		}
	}
}
