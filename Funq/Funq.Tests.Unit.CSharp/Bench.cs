using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Funq.FSharp;

namespace Funq.Tests.Unit.CSharp
{
	public abstract class TimeResult {
		public static implicit operator TimeResult(double d) {
			return new Time(d);
		}

		public static TimeResult TimeOut {
			get {
				return new TimedOut();
			}
		}
	}

	public class Time : TimeResult {
		public double Value;
		public Time(double value) {
			Value = value;
		}

		public override string ToString() {
			return Value.ToString(CultureInfo.InvariantCulture);
		}
	}

	public class TimedOut : TimeResult {
		
	}
	

	internal class Bench {
		public int Drops = 5;
		public int Runs = 10;
		public int MsTimeout = 20000;
		public Stopwatch Watch = new Stopwatch();

		public Action OnRun = () => {
			GC.Collect();
			GC.WaitForPendingFinalizers();
		};

		public TimeResult InvokeTest(Action act) {
			Action runner = () => {
				for (int i = 0; i < Drops; i++) {
					act();
				}
				Watch.Reset();
				OnRun();
				Watch.Start();
				for (int i = 0; i < Runs; i++) {
					act();
				}
				Watch.Stop();
			};

			var thread = new Thread(() => runner());
			thread.Start();
			var succeess = thread.Join(MsTimeout);
			if (succeess) {
				return Watch.Elapsed.TotalMilliseconds/Runs;
			}
			else {
				return new TimedOut();
			}
		}
	}
}
