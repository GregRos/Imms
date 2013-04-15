using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solid;
namespace SolidCSTests
{
	class Program
	{
		static void Main(string[] args)
		{
			var fx = Vector<int>.Empty.AddLastRange(Enumerable.Range(0, 1000).ToFlexibleList());
			foreach (var item in fx)
			{
				Console.Write("{0}, ", item);
			}
		}
	}
}
