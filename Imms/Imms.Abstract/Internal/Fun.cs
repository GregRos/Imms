using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Imms.Abstract;

namespace Imms {


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