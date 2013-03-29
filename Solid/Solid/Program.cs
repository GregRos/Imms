using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Solid.FingerTree;
using Solid.TrieMap;
using Solid.TrieVector;

namespace Solid
{




internal static class Program
{
		
	private static void Main(string[] args)
	{
		var v = Vector<int>.Empty;
		var nums = Enumerable.Range(0, 1000000).ToArray();
		v = v.AddLastRange(nums);

	}

	



		private static Stopwatch sw = new Stopwatch();
		private static TimeSpan Bench(Action act, int iter)
		{
			
		
			GC.Collect();
			Action b =
				() =>
				{
					for (int i = 0; i < iter; i++)
					{
						act();
					}
				};
			
			var thread = new Thread(new ThreadStart(b));
			sw.Restart();
			thread.Start();
			thread.Join();
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			return sw.Elapsed;
		}
	}
}