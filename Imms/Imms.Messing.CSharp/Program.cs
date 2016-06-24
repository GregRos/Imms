using System.Collections.Generic;
using System.Linq;

namespace Imms.Messing.CSharp {


	class Program {


		static void Main(string[] args) {
			

			ImmList<string> names = new[] {
				"Bob", "Frank", "Joe", "Steve", "Allen", "Greg", "Mike", "Joey", "Jill", "Marcus", "Alex"
			}.ToImmList();
			

			string[] names2 = new[] {
				"Bob", "Alex", "Jill", "Fred", "Linda"
			};

			ImmList<KeyValuePair<char, IEnumerable<string>>> namesByFirstLetter =
				from name in names
				where !name.StartsWith("M")
				let firstLetter = name[0]
				group name by firstLetter into namesByThisLetter
				orderby namesByThisLetter.Key
				select namesByThisLetter;

			ImmList<string> peopleInBothLists =
				from name in names
				join name2 in names2 on name equals name2
				orderby name
				select name;




		}
	}
}