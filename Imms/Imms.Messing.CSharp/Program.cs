using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imms.Messing.CSharp {
	class Program {
		static void Main(string[] args) {
			var list1 = ImmList.FromItems(1, 2, 3);
			var s = ImmMap.Empty<int, int>();
			s.Merge(s, (key, value1, value2) => key);
			
		}
	}
}
