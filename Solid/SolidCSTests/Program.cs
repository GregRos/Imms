using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Solid;


namespace SolidCSTests
{
	class Program
	{
		static void Main(string[] args)
		{
			
			var list1 = Enumerable.Range(0, 100000).ToFlexList();
			var list2 = list1.ToList();
			Action act = () =>
			             {
				             for (int i = 0; i < 1000000; i++)
				             {
					            list2.Insert(50000, 0);
				             }
			             };

			Console.WriteLine(Bench(act));
		}

		private static Stopwatch watch = new Stopwatch();
		public static long Bench(Action act)
		{
			act();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			var thread = new Thread(new ThreadStart(act));
			watch.Restart();
			thread.Start();
			thread.Join();
			watch.Stop();
			return watch.ElapsedMilliseconds;

		}
	}
}
