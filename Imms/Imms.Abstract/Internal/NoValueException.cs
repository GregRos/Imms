using System;

namespace Imms {
	/// <summary>
	///     Indicates that an attempt was made to access the Value property of an Optional with no value.
	/// </summary>
	public class NoValueException : InvalidOperationException {

		private static string GetMessage(Type t, string message) {
			string typeName = t == null ? "an unknown type" : "type " + t.PrettyName();
			return string.Format("Tried to get the underlying value of an optional value of {0}, but no value exists. {1}",
				typeName, message ?? "");
		}

		/// <summary>
		///     Creates a new instance of NoValueException for the option type 't'.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="message">An optional extra message.</param>
		internal NoValueException(Type t = null, string message = "")
			: base(GetMessage(t, message)) {}
	}
}