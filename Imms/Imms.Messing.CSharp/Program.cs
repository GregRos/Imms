using System;
using System.CodeDom.Compiler;
using System.Linq;
using Imms.Abstract;

namespace Imms.Messing.CSharp {
	
	public interface Mixin {
	}

	public interface HasMixins {
		
	}

	public interface Has<out TMixin> : HasMixins
		where TMixin : Mixin {
		TMixin Mixin { get; }
	}

	public static class MixinUtils {
		public static TMixin Mixout<TMixin>(this Has<TMixin> what)
			where TMixin : Mixin {
			return what.Mixin;
		}

		[Obsolete("The object does not have this mixin.", true)]
		public static TSome Mixout<TSome>(this HasMixins something) where TSome : Mixin  {
			return default(TSome);
		}
	}

	public abstract class Mixin1 : Mixin {
	}

	public abstract class Mixin2 : Mixin {
	}

	public abstract class Mixin3 : Mixin {
	}

	public class Test : Has<Mixin1>, Has<Mixin2> {


		Mixin1 Has<Mixin1>.Mixin => Mixin1Impl.Instance;

		Mixin2 Has<Mixin2>.Mixin => Mixin2Impl.Instance;

		private class Mixin1Impl : Mixin1 {
			public static readonly Mixin1Impl Instance = new Mixin1Impl();
		}

		private class Mixin2Impl : Mixin2 {
			public static readonly Mixin2Impl Instance = new Mixin2Impl();
		}
	}

	static class TestThis {
		public static void run() {
			var t = new Test();
			var a = t.Mixout<Mixin1>();
			var b = t.Mixout<Mixin2>();

		}
	}


	class Program {


		static void Main(string[] args) {
			var list = ImmList.FromItems(1);
			list.Any()
		}
	}
}