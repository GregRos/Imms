using System;
using System.CodeDom.Compiler;
using System.Linq;
using Imms.Abstract;

namespace Imms.Messing.CSharp {




	class Program {


		static void Main(string[] args) {
			ImmList<int> list = ImmList.Of(0, 1, 2);

			//access to the ends:
			list = list.AddLast(3).AddFirst(-1);
			list = list.AddLastRange(new[] {
				4, 5, 6
			});
			list = list.AddFirstRange(list);
			list = list.RemoveLast().RemoveFirst();

			//indexing:
			int firstItem = list[0];
			list = list.Update(0, firstItem + 1);
			list = list.Insert(0, firstItem);
			
			//slices:
			ImmList<int> sublist = list[1, 3]; //slices in range [1, 3], inclusive.

			ImmSet<int> set = ImmSet.Of(0, 1, 2);

			//add and remove:
			set = set.Add(3).Remove(0);
			
			//set-theoretic operations:
			set = set.Union(new[] {
				5, 6
			});
			set = set.Except(new[] {
				5
			});
			set = set.Intersect(set);

			ImmMap<int, int> map = ImmMap.Of(Kvp.Of(1, 1), Kvp.Of(2, 2), Kvp.Of(3, 3));
			map = map.Add(4, 4);
			map = map.Remove(2);
			int value = map[1];

		}
	}
}