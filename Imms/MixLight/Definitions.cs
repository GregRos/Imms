using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MixLight
{

	public interface Mixin {
		
	}

	public interface HasMixins {
		
	}

	public interface Has<out TMixin> : HasMixins where TMixin : Mixin {
		TMixin Impl { get; }
	}

	public static class MixHelper {
		public static TMixin Mixout<TMixin>(this Has<TMixin> target) where TMixin : Mixin {
			return target.Impl;
		}
	}

	public class InheritableAttribute : Attribute {
		
	}
}
