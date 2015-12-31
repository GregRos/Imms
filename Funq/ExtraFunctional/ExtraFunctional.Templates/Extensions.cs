using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtraFunctional.CSharp{

	public static class TypeNames {

		private static int longIndex = 3;
		private static int floatIndex = 1;
		private static string[] _sPriorities = {
			"double",
			"float",
			"decimal",
			"long",
			"int",
			"short",
			"sbyte"
		};

		private static string[] _uPriorities = new[] {
			"ulong",
			"uint",
			"ushort",
			"byte"
		};

		public static string GetOpResult(string left, string right) {
			var lPriority = Array.IndexOf(_sPriorities, left);
			var rPriority = Array.IndexOf(_sPriorities, right);
			var lSigned = lPriority >= 0;
			var rSigned = rPriority >= 0;
			lPriority = lPriority < 0 ? Array.IndexOf(_uPriorities, left) : lPriority;
			rPriority = rPriority < 0 ? Array.IndexOf(_uPriorities, right) : rPriority;
			if (lPriority < 0 || rPriority < 0) {
				Debugger.Break();
				throw new Exception("Type not found!");
			}
			if (!lSigned && !rSigned) {
				return _uPriorities[Math.Min(lPriority, rPriority)];
			}
			if (lSigned && rSigned) {
				return _sPriorities[Math.Min(lPriority, rPriority)];
			}
			var sPriority = lSigned ? lPriority : rPriority;
			var uPriority = lSigned ? rPriority : lPriority;
			if (uPriority + longIndex == sPriority) {
				return sPriority == longIndex ? _sPriorities[longIndex] : _sPriorities[sPriority - 1];
			}
			
			if (uPriority + longIndex < sPriority) {
				return _sPriorities[sPriority - 1];
			}
			return _sPriorities[sPriority];
		}

		public static string GetFractionalDivResult(string left, string right) {
			var lPriority = Array.IndexOf(_sPriorities, left);
			var rPriority = Array.IndexOf(_uPriorities, right);
			var lSigned = lPriority >= 0;
			var rSigned = rPriority >= 0;
			var lFractional = lSigned && lPriority < longIndex;
			var rFractional = rSigned && rPriority < longIndex;
			if (!lFractional && !rFractional) {
				return _sPriorities[floatIndex];
			}
			if (lFractional && rFractional) {
				return _sPriorities[Math.Min(lPriority, rPriority)];
			}
			if (lFractional) {
				return _sPriorities[lPriority];
			}
			return _sPriorities[rPriority];
		}
	}
	
	public static class Fun {
        public static Func<T1> Of<T1>(Func<T1> f) {

            return f;
        }

        public static Func<T1, T2> Of<T1, T2>(Func<T1, T2> f)
        {
            return f;
        }

        public static Func<T1, T2, T3> Of<T1, T2, T3>(Func<T1, T2, T3> f)
        {
            return f;
        }
    }

	public static class Extensions
	{

		public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) {
			return left.Zip(right, Tuple.Create);
		}

		public static IEnumerable<TOut> Select<T1, T2, TOut>(this IEnumerable<Tuple<T1, T2>> seq, Func<T1, T2, TOut> selector) {
			return seq.Select(tuple => selector(tuple.Item1, tuple.Item2));
		} 

		public static IEnumerable<string> SelectFormat<T1, T2>(this IEnumerable<Tuple<T1, T2>> seq, string format, Func<T1, T2, object[]> objs) {
			return seq.Select((x, y) => format.FormatWith(objs(x, y)));
		} 

		public static IEnumerable<string> SelectFormat<T>(this IEnumerable<T> seq, string format, Func<T, object[]> objs) {
			return seq.Select(x => format.FormatWith(new object[] { x }.Concat(objs(x)).ToArray()));
		} 

		public static IEnumerable<T> ReplaceAt<T>(this IEnumerable<T> seq, int index, T element) {
			return seq.Select((x, i) => i == index ? element : x);
		}
 		public static bool EqAny(this object some, params object[] args) {
			return args.Contains(some);
		}
		public static IEnumerable<string> SelectFormat<T>(this IEnumerable<T> seq, string format, params object[] objs) {
			return seq.SelectFormat(format, x => objs);
		}

	    public static string FormatWith(this string format, params object[] args) {
	        return string.Format(format, args);
	    }

		public static IEnumerable<string> ZipFormat<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right, string format,
		   Func<T1, T2, object[]> args)
		{
			return left.Zip(right, (l, r) => String.Format(format, new object[] { l, r }.Concat(args(l, r)).ToArray()));
		} 

	    public static IEnumerable<string> ZipFormat<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right, string format,
	        params object[] args) {
	        return left.Zip(right, (l, r) => String.Format(format, new object[] {l, r}.Concat(args).ToArray()));
	    } 

	    public static string Join<T>(this IEnumerable<T> seq, string sep) {
	        return String.Join(sep, seq);
	    }

        public static string JoinStart<T>(this IEnumerable<T> seq, string sep) {
            return seq.Aggregate("", (str, cur) => str + sep + cur.ToString());
        }
	}
}
