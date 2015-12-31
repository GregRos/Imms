using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Funq.Abstract;

namespace Funq {

	public static class TupleTest {

		public static int Test<T1,T2>(this Tuple<T1, T2> a) {
			return 0;
		}

	}

	static partial class Fun {

		public static TOut CastObject<T, TOut>(T value)
			where TOut : T {
			return (TOut) value;
		}


		public static Func<T, TOut> HideIndex<T, TOut>(this Func<T, int, TOut> f) {
			var i = 0;
			return (x => f(x, i++));
		}
	}
}