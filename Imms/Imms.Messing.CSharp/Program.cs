using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imms.Messing.CSharp {
	class Program {
		static void Main(string[] args) {
			var list1 = ImmList.FromItems(1, 2, 3) == ImmList.FromItems(1, 2);
			var set1 = new[] {
				1,
				2,
				3
			}.ToImmSet();
		}
	}
}
