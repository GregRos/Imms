using System;

namespace Funq
{
	[Flags]
	public enum SetRelation
	{
		Equal = 0x1,
		ProperSubsetOf = 0x2,
		ProperSupersetOf = 0x4,
		Disjoint = 0x8,
		None = 0x10,
	}
	
}