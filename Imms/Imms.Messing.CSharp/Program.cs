using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imms;
namespace Imms.Messing.CSharp {

	class Program {


		static void Main(string[] args) {
			int? a = 5;
			object b = null;

			var x = a.AsOptional();
			var y = 5.AsOptional();

		}
	}
}
