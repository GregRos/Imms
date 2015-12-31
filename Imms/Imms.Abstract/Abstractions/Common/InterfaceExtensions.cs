using System;
using System.Collections.Generic;

namespace Imms.Abstract {
	static class InterfaceExtensions {

		public static TIterable ToIterable<TElem, TIterable>(this IBuilderFactory<IIterableBuilder<TElem, TIterable>> factory, IEnumerable<TElem> values)
		{
			using (var builder = factory.EmptyBuilder) {
				builder.AddRange(values);
				return builder.Produce();
			}
		}

		public static void Set<TKey, TValue>(this IAnyMapBuilder<TKey, TValue> builder, TKey key, TValue value) {
			builder.Add(Kvp.Of(key, value));
		}

	}
}