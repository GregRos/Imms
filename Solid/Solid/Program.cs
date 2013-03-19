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

		var x = Enumerable.Range(0, 100).ToSequence();
		//x = x.Append(x);

		for (int i = 0; i < 50; i++)
		{
			x = x.Insert(i * 2, 0);
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