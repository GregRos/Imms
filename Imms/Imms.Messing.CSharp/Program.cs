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
			var list = ImmList.FromItems(1, 2, 3, 4);
			var vector = ImmVector.FromItems(1, 2, 3, 4, 5);

			var list2 = list
				.AddLast(5).AddFirst(0)
				.AddLastRange(list)
				.AddFirstRange(list)
				.Insert(0, 0).Update(0, 5)
				.RemoveAt(0).InsertRange(2, list)
				.Select(x => x + 1)
				.Where(x => x % 2 == 0)
				.AddFirst(1);

		}
	}
}
