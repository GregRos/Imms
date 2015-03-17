using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Collections.Common
{

	public enum ChangeType {
		AddedItem,
		RemovedItem,
		UpdatedItem
	}

	class ChangeMarker<T>
	{
		public ChangeMarker(ChangeType kind, Option<T> oldItem = default(Option<T>), Option<T> newItem = default(Option<T>)) {
			Kind = kind;
			OldItem = oldItem;
			NewItem = newItem;
		}

		public ChangeType Kind {
			get;
			private set;
		}

		public Option<T> OldItem {
			get;
			private set;
		}

		public Option<T> NewItem {
			get;
			private set;
		}
	}
}
