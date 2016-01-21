using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Imms.Optional;
using Imms;
namespace Imms.Messing.CSharp {
	class Program {
		static void Main(string[] args) {
			
			var xf = NoneOf<int>();
			var s = ImmMap.Empty<int, int>();
			s.Merge(s, (key, value1, value2) => key);
			
		}
	}
}
