using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MixLight;

namespace Imms.Mixins
{
	public class MForEach<TCollection, TElem> : Mixin where TCollection : IEnumerable<TElem> {
		public virtual bool ForEachWhile(TCollection self, Func<TElem, bool> function) {
			if (function == null) throw Errors.Argument_null("function");
			foreach (var item in self) if (!function(item)) return false;
			return true;
		}

		public virtual void ForEach(TCollection self, Action<TElem> action) {
			action.CheckNotNull("action");
			ForEachWhile(self, x => {
				action(x);
				return true;
			});
		}

		/// <summary>
		///     Determines if all the items in the collection fulfill the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public bool All(TCollection self, Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return ForEachWhile(self, predicate);
		}

		/// <summary>
		///     Returns true if any item in the collection fulfills the specified predicate.
		/// </summary>
		/// <param name="predicate"> The predicate. </param>
		/// <returns> </returns>
		public bool Any(TCollection self, Func<TElem, bool> predicate) {
			predicate.CheckNotNull("predicate");
			return !ForEachWhile(self, v => !predicate(v));
		}


	}
}
