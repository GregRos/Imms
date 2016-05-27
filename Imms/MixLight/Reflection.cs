using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MixLight {
	/// <summary>
	///     Contains utility and extension methods for working with types and type members.
	/// </summary>
	public static class ReflectExt {
		/// <summary>
		///     Returns a pretty name for the type, such as using angle braces for a generic type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string PrettyName(this Type type, bool full = false) {
			if (type.GetGenericArguments().Length == 0) return type.Name;
			var genericArguments = type.GetGenericArguments();
			var unmangledName = type.JustTypeName();
			Func<Type, string> nameGetter;
			if (full) {
				nameGetter = PrettyFullName;
			} else {
				nameGetter = x => PrettyName(type, false);
			}
			return unmangledName + "<" + string.Join(",", genericArguments.Select(nameGetter)) + ">";
		}

		public static string PrettyFullName(this Type type) {
			return $"{type.Namespace}.{type.PrettyName(true)}";
		}

		/// <summary>
		///     Returns the name of the type, without the ` symbol or generic type parameterization.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string JustTypeName(this Type type) {
			var typeDefeninition = type.Name;
			var indexOf = typeDefeninition.IndexOf("`", StringComparison.InvariantCulture);
			return indexOf < 0 ? typeDefeninition : typeDefeninition.Substring(0, indexOf);
		}

		public static IEnumerable<Type> OfRuntimeType(this IEnumerable<Type> self, Type t) {
			return self.Where(x => t.IsAssignableFrom(x) || (x.IsGenericType && x.GetGenericTypeDefinition() == t));
		}

		public static IEnumerable<Type> OfRuntimeType<T>(this IEnumerable<Type> self) {
			
			return self.OfRuntimeType(typeof (T));
		}


		public static string Join(this IEnumerable<string> self, string delim) {
			return String.Join(delim, self);
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider t, bool inherit) where T : Attribute {
			return t.GetCustomAttributes(typeof (T), inherit).Cast<T>();
		}
		

	}
}