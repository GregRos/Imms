using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Solid;
using System.IO.IsolatedStorage;
using System.IO.MemoryMappedFiles;

namespace SolidCSTests
{
	class Program
	{
		static void Main(string[] args)
		{
			

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
