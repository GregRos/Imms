using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Funq.Abstract
{
	partial class AbstractIterable<TElem, TIterable, TBuilder>
	{
		/// <summary>
		/// A string representation of this object, for use with the DebuggerDisplay attribute.
		/// </summary>
		protected internal virtual string DebuggerDisplay
		{
			get
			{
				return string.Format("{0}, Length = {1}", this.GetType().PrettyName(), this.Length);
			}
		}

		protected internal class IterableDebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public TIterable Object
			{
				get; set;
			}

			public IterableDebugView(TIterable list)
			{
				Object = list;
			}

			public int Length
			{
				get
				{
					return Object.Length;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public TElem[] zItems
			{
				get
				{
					return Object.ToArray();
				}
			}
		}
	}
}
