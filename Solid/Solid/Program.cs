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
		var items = Enumerable.Range(0, 1000).ToFlexibleList();

		for (int i = 0; i < 500; i++)
		{
			items = items.Remove(100);
		}

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