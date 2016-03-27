using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imms;
using Microsoft.FSharp.Core;

namespace Imms.Messing.CSharp {

	public interface IMixin {
		
	}

	//instead of implementing IMixin, you implement IHas<Mixin> 

	public interface MForEach<out T> : IMixin {
		void ForEach(Action<T> iterator);

		bool ForEachWhile(Func<T, bool> iterator);
	}

	interface MForEachBack<out T> : IMixin {
		void ForEachBack(Action<T> iterator);

		bool ForEachBackWhile(Func<T, bool> iterator);
	}

	public interface Has<out TMixin> : IMixin {
		TMixin Implementation {
			get;
		}
	}

	public static class ForEachMixin {

	}

	class Program {


		static void Main(string[] args) {
			int? a = 5;
			object b = null;

			var x = a.AsOptional();
			var y = 5.AsOptional();
			FSharpOption<int>.Some()
		}

	}
}
