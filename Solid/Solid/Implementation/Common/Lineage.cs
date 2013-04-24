using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Common
{
	internal sealed class Lineage1
	{
		public bool CanMutate;
		public static readonly Lineage1 Immutable = new Lineage1(false);
		private Lineage1(bool canMutate = false)
		{
			
		}


		public static Lineage1 Mutable
		{
			get
			{
				return new Lineage1(true);
			}
		}

		public void Lock()
		{
#if DEBUG
			this.CanMutate.Is(true);
#endif
			this.CanMutate = false;
		}
	}
}
