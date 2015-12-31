using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Funq.Abstract;

namespace Funq {
	/// <summary>
	///     Contains utility and extension methods for working with types and type members.
	/// </summary>
	static class ReflectExt {
		/// <summary>
		///     Returns a pretty name for the type, such as using angle braces for a generic type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string PrettyName(this Type type) {
			if (type.GetGenericArguments().Length == 0) return type.Name;
			var genericArguments = type.GetGenericArguments();
			var unmangledName = type.JustTypeName();
			return unmangledName + "<" + string.Join(",", genericArguments.Select(PrettyName)) + ">";
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

	}
}