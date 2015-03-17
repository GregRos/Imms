using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Funq;

namespace Funq
{

	internal static class Ext
	{
		public static string PrettyName(this Type type)
		{
			if (type.GetGenericArguments().Length == 0)
			{
				return type.Name;
			}
			var genericArguments = type.GetGenericArguments();
			var unmangledName = type.JustTypeName();
			return unmangledName + "<" + String.Join(",", genericArguments.Select(PrettyName)) + ">";
		}

		public static string JustTypeName(this Type type)
		{
			var typeDefeninition = type.Name;
			var indexOf = typeDefeninition.IndexOf("`", StringComparison.InvariantCulture);
			return indexOf < 0 ? typeDefeninition : typeDefeninition.Substring(0, indexOf);
		}
	}
	internal static class Fun
	{
		public interface ITryCast<TFrom>
		{
			TTo To<TTo>();
		}

		public static Option<TOut> AsSome<TOut>(this TOut x)
		{
			return Option.Some(x);
		}

		public static Func<T, TOut> AttachIndex<T, TOut>(this Func<T, int, TOut> f)
		{
			var i = 0;
			return (x => f(x, i++));
		}

		public static TOut Cast<TOut>(this object x)
		{
			return (TOut) x;
		}

		public static Func<TIn, TOut> Chain<TIn, TIn1, TOut>(this Func<TIn, TIn1> left, Func<TIn1, TOut> right)
		{
			return (v => left(v).Pipe(right));
		}

		public static Func<TIn2, TOut> Curry<TIn1, TIn2, TOut>(this Func<TIn1, TIn2, TOut> f, TIn1 o)
		{
			return (v => f(o, v));
		}

		internal static void ForEach<TElem>(IEnumerable<TElem> seq, Action<TElem> act)
		{
			ForEachWhile(seq, x =>
			                  {
				                  act(x);
				                  return true;
			                  });
		}

		internal static bool ForEachWhile<TElem>(IEnumerable<TElem> seq, Func<TElem, bool> act)
		{
			foreach (var item in seq)
			{
				if (!act(item)) return false;
			}
			return true;
		}

		public static Func<T, T> Id<T>()
		{
			return x => x;
		}

		public static Func<T> Empty<T>()
		{
			return () => default(T);
		}

		public static Func<T, int, TOut> IgnoreIndex<T, TOut>(this Func<T, TOut> f)
		{
			return ((v, i) => f(v));
		}

		public static Func<T1, T2, int, TOut> IgnoreIndex<T1, T2, TOut>(this Func<T1, T2, TOut> f)
		{
			return ((a, b, i) => f(a, b));
		}

		public static Func<T, bool> Not<T>(this Func<T, bool> f)
		{
			return x => !f(x);
		}

		public static TOut Pipe<TIn, TOut>(this TIn o, Func<TIn, TOut> f)
		{
			return f(o);
		}

		public static Option<TOut> TryCast<TOut>(this object x)
		{
			return (TOut) x;
		}

	}
}