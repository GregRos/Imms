namespace Funq
{
	internal struct StructTuple<T>
	{
		public readonly T First;

		public StructTuple(T first) : this()
		{
			First = first;
		}
	}

	internal struct StructTuple<T1, T2>
	{
		public readonly T1 First;
		public readonly T2 Second;

		public override string ToString() {
			return string.Format("({0}, {1})", First, Second);
		}

		public StructTuple(T1 first, T2 second) : this()
		{
			First = first;
			Second = second;
		}
	}

	internal static class StructTuple
	{
		public static StructTuple<T> Create<T>(T item)
		{
			return new StructTuple<T>(item);
		}

		public static StructTuple<T1, T2> Create<T1, T2>(T1 a, T2 b)
		{
			return new StructTuple<T1, T2>(a, b);
		}
	}

}
